import Foundation

// Foundation Process supports signalling from another thread. The lock also covers cancellation before launch.
final class EngineClient: @unchecked Sendable {
    private let lock = NSLock()
    private var process: Process?
    private var cancelled = false

    func cancel() {
        lock.withLock { cancelled = true; if process?.isRunning == true { process?.terminate() } }
    }

    func run(_ request: EngineRequest, onEvent: @escaping @Sendable (EngineEvent) async -> Void) async throws {
        let task = Task.detached { [self] in
            let executable = Bundle.main.url(forResource: "Atlas.Desktop", withExtension: nil, subdirectory: "Engine")
                ?? ProcessInfo.processInfo.environment["ATLAS_ENGINE_PATH"].map { URL(fileURLWithPath: $0) }
            guard let executable, FileManager.default.isExecutableFile(atPath: executable.path) else {
                throw AtlasError("Atlas Engine is missing. Rebuild the app with src/Scripts/build.sh.")
            }
            let child = Process()
            child.executableURL = executable
            if let rootURL = Bundle.main.url(forResource: "dotnet-root", withExtension: "txt"),
               let root = try? String(contentsOf: rootURL, encoding: .utf8) {
                var environment = ProcessInfo.processInfo.environment
                environment["DOTNET_ROOT"] = root.trimmingCharacters(in: .whitespacesAndNewlines)
                child.environment = environment
            }
            let input = Pipe(), output = Pipe()
            child.standardInput = input; child.standardOutput = output
            try lock.withLock {
                if cancelled { throw CancellationError() }
                process = child
                try child.run()
            }
            defer { lock.withLock { process = nil } }
            do {
                try input.fileHandleForWriting.write(contentsOf: JSONEncoder().encode(request))
                try input.fileHandleForWriting.close()
                var buffer = Data()
                var terminalEvent = false
                while let chunk = try output.fileHandleForReading.read(upToCount: 65536), !chunk.isEmpty {
                    buffer.append(chunk)
                    while let end = buffer.firstIndex(of: 10) {
                        let line = buffer[..<end]
                        buffer.removeSubrange(...end)
                        if line.isEmpty { continue }
                        let event = try JSONDecoder().decode(EngineEvent.self, from: line)
                        if ["result", "error", "cancelled"].contains(event.type) { terminalEvent = true }
                        await onEvent(event)
                    }
                }
                child.waitUntilExit()
                if !terminalEvent {
                    if lock.withLock({ cancelled }) { throw CancellationError() }
                    throw AtlasError("Atlas Engine ended without a result (exit \(child.terminationStatus)).")
                }
            } catch {
                if child.isRunning { child.terminate(); child.waitUntilExit() }
                throw error
            }
        }
        try await withTaskCancellationHandler { try await task.value } onCancel: { self.cancel() }
    }
}

struct AtlasError: LocalizedError {
    let message: String
    init(_ message: String) { self.message = message }
    var errorDescription: String? { message }
}
