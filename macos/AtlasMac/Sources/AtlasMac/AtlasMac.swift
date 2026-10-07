import AppKit
import SwiftUI

private enum Palette {
    static let orange = Color(red: 0.98, green: 0.35, blue: 0.16)
    static let red = Color(red: 0.87, green: 0.18, blue: 0.19)
    static let ink = Color.primary
    static let muted = Color.secondary
}

@main
struct AtlasMacApp: App {
    var body: some Scene {
        WindowGroup("Atlas") {
            AtlasView()
                .frame(minWidth: 900, minHeight: 620)
        }
        .windowStyle(.hiddenTitleBar)
    }
}

private enum Page {
    case home, json
}

private struct EngineRequest: Encodable {
    let input: String
    let operation: String
}

private struct EngineResponse: Decodable {
    let result: String?
    let error: String?
}

private enum EngineClient {
    static func run(input: String, operation: String) throws -> String {
        guard let path = ProcessInfo.processInfo.environment["ATLAS_ENGINE_PATH"],
              FileManager.default.isExecutableFile(atPath: path) else {
            throw EngineError.message("Atlas Engine is unavailable. Start Atlas with macos/scripts/run.sh.")
        }

        let process = Process()
        process.executableURL = URL(fileURLWithPath: path)
        let request = Pipe()
        let response = Pipe()
        process.standardInput = request
        process.standardOutput = response

        try process.run()
        request.fileHandleForWriting.write(try JSONEncoder().encode(EngineRequest(input: input, operation: operation)))
        try request.fileHandleForWriting.close()
        let data = response.fileHandleForReading.readDataToEndOfFile()
        process.waitUntilExit()

        guard process.terminationStatus == 0 else {
            throw EngineError.message("Atlas Engine stopped unexpectedly.")
        }
        let reply = try JSONDecoder().decode(EngineResponse.self, from: data)
        if let error = reply.error { throw EngineError.message(error) }
        return reply.result ?? ""
    }
}

private enum EngineError: LocalizedError {
    case message(String)

    var errorDescription: String? {
        if case let .message(text) = self { return text }
        return nil
    }
}

private struct AtlasView: View {
    @State private var page: Page = .home
    @State private var input = ""
    @State private var output = ""
    @State private var error: String?
    @State private var isRunning = false
    @State private var operation = "format"

    var body: some View {
        HStack(spacing: 0) {
            sidebar
            Rectangle()
                .fill(.primary.opacity(0.08))
                .frame(width: 1)
            Group {
                switch page {
                case .home: home
                case .json: jsonStudio
                }
            }
            .frame(maxWidth: .infinity, maxHeight: .infinity)
            .background(Color(nsColor: .windowBackgroundColor))
        }
        .tint(Palette.orange)
    }

    private var sidebar: some View {
        VStack(alignment: .leading, spacing: 0) {
            HStack(spacing: 11) {
                ZStack {
                    RoundedRectangle(cornerRadius: 13)
                        .fill(LinearGradient(colors: [Palette.orange, Palette.red], startPoint: .topLeading, endPoint: .bottomTrailing))
                    Image(systemName: "square.grid.2x2.fill")
                        .font(.system(size: 18, weight: .semibold))
                        .foregroundStyle(.white)
                }
                .frame(width: 38, height: 38)
                VStack(alignment: .leading, spacing: 1) {
                    Text("ATLAS")
                        .font(.system(size: 17, weight: .heavy, design: .rounded))
                        .tracking(2)
                    Text("The everyday workbench")
                        .font(.system(size: 10, weight: .medium))
                        .foregroundStyle(Palette.muted)
                }
            }
            .padding(.top, 37)
            .padding(.bottom, 39)

            sectionLabel("WORKSPACE")
            navItem("Home", icon: "house", destination: .home)
                .padding(.top, 9)

            sectionLabel("TOOLS")
                .padding(.top, 30)
            navItem("JSON Studio", icon: "curlybraces", destination: .json)
                .padding(.top, 9)

            Spacer()

            HStack(spacing: 10) {
                Image(systemName: "circle.grid.2x2")
                    .font(.system(size: 15))
                    .foregroundStyle(Palette.orange)
                VStack(alignment: .leading, spacing: 2) {
                    Text("Made for your Mac")
                        .font(.system(size: 11, weight: .semibold))
                    Text("A quieter way to get things done")
                        .font(.system(size: 10))
                        .foregroundStyle(Palette.muted)
                }
            }
            .padding(13)
            .frame(maxWidth: .infinity, alignment: .leading)
            .atlasGlass(radius: 15)
            .padding(.bottom, 22)
        }
        .padding(.horizontal, 17)
        .frame(width: 238)
        .background(.ultraThinMaterial)
    }

    private func sectionLabel(_ title: String) -> some View {
        Text(title)
            .font(.system(size: 10, weight: .bold))
            .tracking(1.6)
            .foregroundStyle(Palette.muted)
            .padding(.leading, 11)
    }

    private func navItem(_ title: String, icon: String, destination: Page) -> some View {
        Button {
            page = destination
        } label: {
            HStack(spacing: 12) {
                Image(systemName: icon)
                    .font(.system(size: 16, weight: .medium))
                    .frame(width: 19)
                Text(title)
                    .font(.system(size: 13, weight: .semibold))
                Spacer()
                if page == destination {
                    Circle().fill(Palette.orange).frame(width: 5, height: 5)
                }
            }
            .foregroundStyle(page == destination ? Palette.orange : Palette.ink)
            .padding(.horizontal, 13)
            .frame(height: 40)
            .background(page == destination ? Palette.orange.opacity(0.12) : .clear, in: RoundedRectangle(cornerRadius: 12))
        }
        .buttonStyle(.plain)
    }

    private var home: some View {
        ScrollView {
            VStack(alignment: .leading, spacing: 25) {
                HStack {
                    Text("YOUR WORKSPACE")
                        .font(.system(size: 10, weight: .bold))
                        .tracking(2)
                        .foregroundStyle(Palette.muted)
                    Spacer()
                    Image(systemName: "sparkle")
                        .foregroundStyle(Palette.orange)
                }

                VStack(alignment: .leading, spacing: 17) {
                    Text("A little less friction.\nA lot more flow.")
                        .font(.system(size: 44, weight: .bold, design: .rounded))
                        .tracking(-1.6)
                        .fixedSize(horizontal: false, vertical: true)
                    Text("Your useful tools, right where you need them.")
                        .font(.system(size: 15))
                        .foregroundStyle(Palette.muted)
                    Button {
                        page = .json
                    } label: {
                        HStack(spacing: 9) {
                            Text("Open JSON Studio")
                            Image(systemName: "arrow.up.right")
                        }
                        .font(.system(size: 13, weight: .semibold))
                        .foregroundStyle(.white)
                        .padding(.horizontal, 18)
                        .frame(height: 40)
                        .background(LinearGradient(colors: [Palette.orange, Palette.red], startPoint: .leading, endPoint: .trailing), in: Capsule())
                    }
                    .buttonStyle(.plain)
                    .padding(.top, 10)
                }
                .frame(maxWidth: .infinity, alignment: .leading)
                .padding(35)
                .background {
                    RoundedRectangle(cornerRadius: 26)
                        .fill(LinearGradient(colors: [Palette.orange.opacity(0.16), Palette.red.opacity(0.07), .clear], startPoint: .topLeading, endPoint: .bottomTrailing))
                }
                .atlasGlass(radius: 26)

                HStack {
                    Text("THE TOOLKIT")
                        .font(.system(size: 10, weight: .bold))
                        .tracking(2)
                        .foregroundStyle(Palette.muted)
                    Spacer()
                    Text("01 TOOL")
                        .font(.system(size: 10, weight: .bold, design: .monospaced))
                        .foregroundStyle(Palette.muted)
                }
                .padding(.top, 8)

                Button {
                    page = .json
                } label: {
                    HStack(alignment: .top, spacing: 17) {
                        Image(systemName: "curlybraces")
                            .font(.system(size: 21, weight: .semibold))
                            .foregroundStyle(Palette.orange)
                            .frame(width: 48, height: 48)
                            .background(Palette.orange.opacity(0.12), in: RoundedRectangle(cornerRadius: 14))
                        VStack(alignment: .leading, spacing: 7) {
                            Text("JSON Studio")
                                .font(.system(size: 17, weight: .bold, design: .rounded))
                                .foregroundStyle(Palette.ink)
                            Text("Format and minify JSON with instant, clear results.")
                                .font(.system(size: 12))
                                .foregroundStyle(Palette.muted)
                        }
                        Spacer()
                        Image(systemName: "arrow.up.right")
                            .foregroundStyle(Palette.muted)
                    }
                    .padding(21)
                    .frame(maxWidth: .infinity, alignment: .leading)
                    .atlasGlass(radius: 20)
                }
                .buttonStyle(.plain)
            }
            .frame(maxWidth: 960)
            .padding(34)
            .frame(maxWidth: .infinity)
        }
    }

    private var jsonStudio: some View {
        VStack(alignment: .leading, spacing: 21) {
            HStack(alignment: .top) {
                VStack(alignment: .leading, spacing: 7) {
                    Text("TEXT & DATA  /  JSON")
                        .font(.system(size: 10, weight: .bold))
                        .tracking(1.7)
                        .foregroundStyle(Palette.orange)
                    Text("JSON Studio")
                        .font(.system(size: 34, weight: .bold, design: .rounded))
                        .tracking(-0.8)
                    Text("Clean up your data. Keep the details intact.")
                        .font(.system(size: 13))
                        .foregroundStyle(Palette.muted)
                }
                Spacer()
                Image(systemName: "curlybraces.square.fill")
                    .font(.system(size: 29))
                    .foregroundStyle(Palette.orange)
                    .padding(14)
                    .atlasGlass(radius: 16)
            }

            HStack(spacing: 8) {
                operationButton("Format", icon: "text.alignleft", value: "format")
                operationButton("Minify", icon: "line.3.horizontal.decrease", value: "minify")
                Spacer()
                Button("Load example") {
                    input = "{\"project\":\"Atlas\",\"tools\":[\"format\",\"minify\"],\"ready\":true}"
                    error = nil
                }
                .buttonStyle(.plain)
                .font(.system(size: 11, weight: .medium))
                .foregroundStyle(Palette.muted)
            }

            HStack(spacing: 14) {
                editorPanel(title: "INPUT", symbol: "square.and.pencil", text: $input, placeholder: "Paste JSON here…", isEditable: true)
                editorPanel(title: "RESULT", symbol: "checkmark.circle", text: $output, placeholder: "Your result appears here.", isEditable: false)
            }

            HStack(spacing: 12) {
                Button {
                    transform()
                } label: {
                    HStack(spacing: 9) {
                        if isRunning { ProgressView().controlSize(.small) }
                        Text(operation == "format" ? "Format JSON" : "Minify JSON")
                        Image(systemName: "arrow.right")
                    }
                    .font(.system(size: 13, weight: .semibold))
                    .foregroundStyle(.white)
                    .padding(.horizontal, 19)
                    .frame(height: 42)
                    .background(LinearGradient(colors: [Palette.orange, Palette.red], startPoint: .leading, endPoint: .trailing), in: RoundedRectangle(cornerRadius: 12))
                }
                .buttonStyle(.plain)
                .disabled(isRunning)
                .keyboardShortcut(.return, modifiers: [.command])

                if let error {
                    Image(systemName: "exclamationmark.circle.fill")
                        .foregroundStyle(Palette.red)
                    Text(error)
                        .font(.system(size: 12))
                        .foregroundStyle(Palette.red)
                        .lineLimit(2)
                } else {
                    Image(systemName: "lock.shield")
                        .foregroundStyle(Palette.muted)
                    Text("Processed on your Mac")
                        .font(.system(size: 11))
                        .foregroundStyle(Palette.muted)
                }
                Spacer()
            }
        }
        .padding(30)
    }

    private func operationButton(_ title: String, icon: String, value: String) -> some View {
        Button {
            operation = value
        } label: {
            Label(title, systemImage: icon)
                .font(.system(size: 12, weight: .semibold))
                .foregroundStyle(operation == value ? Palette.orange : Palette.muted)
                .padding(.horizontal, 15)
                .frame(height: 35)
                .background(operation == value ? Palette.orange.opacity(0.12) : .clear, in: RoundedRectangle(cornerRadius: 10))
        }
        .buttonStyle(.plain)
    }

    private func editorPanel(title: String, symbol: String, text: Binding<String>, placeholder: String, isEditable: Bool) -> some View {
        VStack(alignment: .leading, spacing: 13) {
            HStack {
                Label(title, systemImage: symbol)
                    .font(.system(size: 10, weight: .bold))
                    .tracking(1.3)
                    .foregroundStyle(Palette.muted)
                Spacer()
                Button {
                    if isEditable {
                        input = NSPasteboard.general.string(forType: .string) ?? input
                        error = nil
                    } else {
                        NSPasteboard.general.clearContents()
                        NSPasteboard.general.setString(output, forType: .string)
                    }
                } label: {
                    Label(isEditable ? "Paste" : "Copy", systemImage: isEditable ? "doc.on.clipboard" : "doc.on.doc")
                        .font(.system(size: 11, weight: .medium))
                }
                .buttonStyle(.plain)
                .disabled(!isEditable && output.isEmpty)
            }
            ZStack(alignment: .topLeading) {
                if text.wrappedValue.isEmpty {
                    Text(placeholder)
                        .font(.system(size: 12, design: .monospaced))
                        .foregroundStyle(Palette.muted.opacity(0.7))
                        .padding(.top, 7)
                        .padding(.leading, 5)
                }
                TextEditor(text: text)
                    .font(.system(size: 12, design: .monospaced))
                    .scrollContentBackground(.hidden)
                    .opacity(text.wrappedValue.isEmpty ? 0.25 : 1)
                    .disabled(!isEditable)
                    .accessibilityLabel(isEditable ? "JSON input" : "JSON result")
            }
            .frame(maxWidth: .infinity, maxHeight: .infinity)
        }
        .padding(17)
        .frame(maxWidth: .infinity, maxHeight: .infinity)
        .atlasGlass(radius: 19)
    }

    private func transform() {
        let source = input
        let selectedOperation = operation
        isRunning = true
        error = nil
        Task {
            let result = await Task.detached(priority: .userInitiated) {
                Result { try EngineClient.run(input: source, operation: selectedOperation) }
            }.value
            isRunning = false
            switch result {
            case let .success(value): output = value
            case let .failure(failure):
                output = ""
                error = failure.localizedDescription
            }
        }
    }
}

private extension View {
    @ViewBuilder
    func atlasGlass(radius: CGFloat) -> some View {
        if #available(macOS 26, *) {
            self.glassEffect(.regular, in: RoundedRectangle(cornerRadius: radius))
        } else {
            self.background(.regularMaterial, in: RoundedRectangle(cornerRadius: radius))
                .overlay(RoundedRectangle(cornerRadius: radius).stroke(.primary.opacity(0.07)))
        }
    }
}
