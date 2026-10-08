import AppKit
import PDFKit
import SwiftUI

private actor EventCollector {
    var events: [EngineEvent] = []
    func append(_ event: EngineEvent) { events.append(event) }
    func result() -> EngineEvent? { events.last { $0.type == "result" } }
}

enum AppChecks {
    @MainActor static func run() async throws {
        func check(_ value: Bool, _ message: String) throws { if !value { throw AtlasError(message) } }
        try check(Set(toolCatalog.map(\.id)).count == toolCatalog.count, "Duplicate tool IDs")
        let root = FileManager.default.temporaryDirectory.appendingPathComponent("atlas-native-test-\(UUID())")
        try FileManager.default.createDirectory(at: root, withIntermediateDirectories: true)
        defer { try? FileManager.default.removeItem(at: root) }
        let source = root.appendingPathComponent("source.pdf")
        var box = CGRect(x: 0, y: 0, width: 200, height: 300)
        guard let context = CGContext(source as CFURL, mediaBox: &box, nil) else { throw AtlasError("PDF fixture") }
        for _ in 0..<3 { context.beginPDFPage(nil); context.setFillColor(CGColor(gray: 0.5, alpha: 1)); context.fill(CGRect(x: 20,y:20,width:50,height:50)); context.endPDFPage() }
        context.closePDF()
        func run(_ request: EngineRequest) async throws -> EngineEvent {
            let collector = EventCollector()
            try await EngineClient().run(request) { await collector.append($0) }
            guard let result = await collector.result() else { throw AtlasError("Missing PDF result") }
            return result
        }
        let merged = try await run(.init(tool: "pdf.merge", paths: [source.path, source.path], outputDirectory: root.path))
        try check(PDFDocument(url: URL(fileURLWithPath: merged.files![0]))?.pageCount == 6, "PDF merge")
        let arranged = try await run(.init(tool: "pdf.pages", paths: [source.path], outputDirectory: root.path, options: ["pages":"3,1"]))
        try check(PDFDocument(url: URL(fileURLWithPath: arranged.files![0]))?.pageCount == 2, "PDF page extraction")
        let split = try await run(.init(tool: "pdf.split", paths: [source.path], outputDirectory: root.path))
        try check(split.files?.count == 3, "PDF split")
        let text = try await run(.init(tool: "pdf.text", paths: [source.path]))
        try check(text.text?.contains("Page 3") == true, "PDF text page boundaries")
        try check(PDFDocument(url: source)?.pageCount == 3, "Original PDF preserved")
        let engineEvents = EventCollector()
        try await EngineClient().run(.init(tool: "json", input: "{\"bridge\":true}")) { await engineEvents.append($0) }
        let engineResult = await engineEvents.result()
        try check(engineResult?.text?.contains("bridge") == true, "Swift to C# bridge")
        let cancelledClient = EngineClient()
        cancelledClient.cancel()
        do { try await cancelledClient.run(.init(tool: "json", input: "{}")) { _ in }; throw AtlasError("Cancelled engine launched") }
        catch is CancellationError { }

        let suite = "atlas-check-\(UUID())"
        let defaults = UserDefaults(suiteName: suite)!
        defer { defaults.removePersistentDomain(forName: suite) }
        let model = Workbench(defaults: defaults)
        let json = toolCatalog.first { $0.id == "json" }!
        model.select(json)
        model.request.input = "{\"queue\":true}"
        model.enterSearch()
        model.select(json)
        try check(model.request.input.contains("queue"), "Search preserves draft")
        model.favorite("json")
        model.recipeName = "Compact JSON"
        model.request.options["mode"] = "minify"
        model.saveRecipe()
        model.enqueue()
        model.enqueue()
        model.cancel(model.jobs[1].id)
        for _ in 0..<100 where !model.jobs[0].isFinished { try await Task.sleep(for: .milliseconds(50)) }
        try check(model.jobs[0].state == "Completed" && model.jobs[1].state == "Stopped", "Sequential queue and queued cancellation")
        try check(model.request.input.isEmpty && model.currentJob != nil, "Success clears inputs and retains results")
        model.backToCategory(); try check(model.section == "Text", "Back opens the tool category")
        model.select(json); model.request.input = "{bad}"
        model.enqueue()
        for _ in 0..<100 where !model.jobs.last!.isFinished { try await Task.sleep(for: .milliseconds(50)) }
        try check(model.jobs.last?.state == "Failed" && model.request.input == "{bad}", "Failure preserves input")
        model.request.input = "{}"; model.enqueue(); model.request.input = "new draft"
        for _ in 0..<100 where !model.jobs.last!.isFinished { try await Task.sleep(for: .milliseconds(50)) }
        try check(model.request.input == "new draft", "Completion preserves newer edits")
        model.request.input = "{}"; model.enqueue(); model.library()
        for _ in 0..<100 where !model.jobs.last!.isFinished { try await Task.sleep(for: .milliseconds(50)) }
        model.select(json); try check(model.request.input.isEmpty, "Completed background tool clears matching draft")
        model.settings.theme = "dark"
        model.settings.snippets = [.init(name: "Greeting", text: "Salam")]; model.persist()
        model.openJob(model.jobs[0])
        try check(model.currentJob?.id == model.jobs[0].id, "History opens selected job")
        model.clearFinishedJobs(); try check(model.jobs.isEmpty, "Clear finished history")
        model.select(toolCatalog.first { $0.id == "scripts.run" }!)
        model.request.input = "/bin/sh"; model.request.secondary = "[\"-c\",\"pwd\"]"; model.request.outputDirectory = root.path
        model.request.options["confirmed"] = "true"; model.recipeName = "Where am I?"; model.saveRecipe()
        model.loadRecipe(model.settings.recipes.last!)
        try check(model.request.input == "/bin/sh" && model.request.options["confirmed"] == nil && model.jobs.isEmpty, "Saved command never inherits execution approval")
        model.enqueue()
        for _ in 0..<100 where !model.jobs.last!.isFinished { try await Task.sleep(for: .milliseconds(50)) }
        try check(model.canRunScript && model.request.input == "/bin/sh", "Command preview preserves executable for confirmation")
        model.enqueue(confirmedScript: true)
        for _ in 0..<100 where !model.jobs.last!.isFinished { try await Task.sleep(for: .milliseconds(50)) }
        try check(model.request.input.isEmpty && model.request.secondary.isEmpty && model.request.outputDirectory == root.path, "Command completion clears input but retains working directory")
        let renamed = root.appendingPathComponent("rename.txt"); try "test".write(to: renamed, atomically: true, encoding: .utf8)
        model.select(toolCatalog.first { $0.id == "files.rename" }!)
        model.request.paths = [renamed.path]; model.request.outputDirectory = root.path; model.request.options["prefix"] = "done-"
        model.enqueue()
        for _ in 0..<100 where !model.jobs.last!.isFinished { try await Task.sleep(for: .milliseconds(50)) }
        try check(model.canApplyRename && model.request.paths == [renamed.path], "Rename preview preserves selection")
        model.enqueue(applyRename: true)
        for _ in 0..<100 where !model.jobs.last!.isFinished { try await Task.sleep(for: .milliseconds(50)) }
        try check(model.request.paths.isEmpty && model.request.options["prefix"] == "done-", "Rename apply clears selection, keeps options")
        let reopened = Workbench(defaults: defaults)
        try check(reopened.settings.favorites == ["json"] && reopened.settings.recent.contains("json"), "Favorites and recent persistence")
        try check(reopened.settings.recipes.first?.options["mode"] == "minify", "Recipe persistence")
        try check(reopened.settings.theme == "dark" && reopened.settings.snippets?.first?.text == "Salam", "Theme and snippets persist")
        try check(reopened.request.input.isEmpty && reopened.jobs.isEmpty, "Inputs and results are session-only")
    }

    @MainActor static func renderIcons(to folder: String) throws {
        try FileManager.default.createDirectory(atPath: folder, withIntermediateDirectories: true)
        for dark in [false, true] {
            guard let img = AtlasAppIcon.image(dark: dark), let tiff = img.tiffRepresentation,
                  let bitmap = NSBitmapImageRep(data: tiff), let png = bitmap.representation(using: .png, properties: [:]) else { throw AtlasError("Icon render failed") }
            try png.write(to: URL(fileURLWithPath: folder).appendingPathComponent(dark ? "icon-dark.png" : "icon-light.png"))
        }
    }

    @MainActor static func renderPreviews(to folder: String) throws {
        try FileManager.default.createDirectory(atPath: folder, withIntermediateDirectories: true)
        _ = NSApplication.shared
        try renderIcons(to: folder)
        for (name, dark, toolID, width, height) in [("atlas-light", false, "", 1280.0, 840.0), ("atlas-dark", true, "", 1280.0, 840.0), ("workbench-light", false, "json", 1280.0, 840.0), ("workbench-dark", true, "json", 1280.0, 840.0), ("settings-dark", true, "Settings", 900.0, 600.0), ("compact-home", true, "", 640.0, 440.0), ("compact-workbench", true, "json", 640.0, 440.0)] {
            let suite = "atlas-preview-\(UUID())"
            let previewDefaults = UserDefaults(suiteName: suite)!
            defer { previewDefaults.removePersistentDomain(forName: suite) }
            let model = Workbench(defaults: previewDefaults)
            if toolID == "Settings" { model.library("Settings") }
            if let tool = toolCatalog.first(where: { $0.id == toolID }) {
                model.select(tool)
                model.request.input = "{\"name\":\"Atlas\",\"local\":true,\"tools\":33}"
                var job = ToolJob(request: model.request)
                job.state = "Completed"
                job.text = "{\n  \"name\": \"Atlas\",\n  \"local\": true,\n  \"tools\": 33\n}"
                model.jobs = [job]
            }
            let view = AtlasView(model: model).frame(width: width, height: height).environment(\.colorScheme, dark ? .dark : .light)
            let hosting = NSHostingView(rootView: view)
            let window = NSWindow(contentRect: CGRect(x: 0, y: 0, width: width, height: height), styleMask: [.borderless], backing: .buffered, defer: false)
            window.appearance = NSAppearance(named: dark ? .darkAqua : .aqua)
            window.contentView = hosting
            hosting.frame = CGRect(x: 0, y: 0, width: width, height: height)
            hosting.layoutSubtreeIfNeeded()
            RunLoop.current.run(until: Date().addingTimeInterval(0.3))
            hosting.layoutSubtreeIfNeeded()
            guard let bitmap = hosting.bitmapImageRepForCachingDisplay(in: hosting.bounds) else { throw AtlasError("Could not render preview.") }
            hosting.cacheDisplay(in: hosting.bounds, to: bitmap)
            guard let png = bitmap.representation(using: .png, properties: [:]) else { throw AtlasError("Could not encode preview.") }
            try png.write(to: URL(fileURLWithPath: folder).appendingPathComponent(name + ".png"))
            window.contentView = nil
        }
    }
}
