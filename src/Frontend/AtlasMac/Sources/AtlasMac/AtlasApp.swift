import SwiftUI
import AppKit

struct AtlasApp: App {
    @StateObject private var model = Workbench()
    var body: some Scene {
        WindowGroup("Atlas") { AtlasView(model: model).modifier(AppAppearance(model: model)) }
            .defaultSize(width: 1280, height: 840)
            .windowStyle(.hiddenTitleBar)
            .commands {
                CommandGroup(replacing: .newItem) {
                    Button("All Tools") { model.library() }.keyboardShortcut("1")
                    Button("Settings…") { model.library("Settings") }.keyboardShortcut(",")
                    Button("Job Queue") { model.showQueue = true }.keyboardShortcut("j")
                }
            }
    }
}

@main
struct AtlasMain {
    @MainActor static func main() async {
        if let index = CommandLine.arguments.firstIndex(of: "--render-icons"), CommandLine.arguments.count > index + 1 {
            do { try AppChecks.renderIcons(to: CommandLine.arguments[index + 1]) }
            catch { FileHandle.standardError.write(Data("FAIL: \(error)\n".utf8)); exit(1) }
        } else if let index = CommandLine.arguments.firstIndex(of: "--render-previews"), CommandLine.arguments.count > index + 1 {
            do { try AppChecks.renderPreviews(to: CommandLine.arguments[index + 1]) }
            catch { FileHandle.standardError.write(Data("FAIL: \(error)\n".utf8)); exit(1) }
        } else if CommandLine.arguments.contains("--self-test") {
            do { try await AppChecks.run(); print("PASS: C# PDF operations, page validation, tool catalogue, Swift–C# bridge, cancellation, queue, drafts and local settings.") }
            catch { FileHandle.standardError.write(Data("FAIL: \(error)\n".utf8)); exit(1) }
        } else {
            NSApplication.shared.setActivationPolicy(.regular)
            AtlasApp.main()
        }
    }
}
