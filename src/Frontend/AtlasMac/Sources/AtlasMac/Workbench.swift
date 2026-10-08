import AppKit
import Combine
import UniformTypeIdentifiers

struct ToolJob: Identifiable {
    let id = UUID()
    let request: EngineRequest
    var state = "Queued"
    var progress = 0.0
    var message = ""
    var text = ""
    var files: [String] = []
    var rows: [[String]] = []
    var isFinished: Bool { ["Completed", "Failed", "Stopped"].contains(state) }
}

@MainActor
final class Workbench: ObservableObject {
    @Published var selectedID: String?
    @Published var section = "All tools"
    @Published var search = ""
    @Published var request = EngineRequest(tool: "json")
    @Published var settings = LocalSettings()
    @Published var jobs: [ToolJob] = []
    @Published var notice: String?
    @Published var recipeName = ""
    @Published var showQueue = false
    @Published var viewedJobID: UUID?
    let containers = ContainerModel()
    private var drafts: [String: EngineRequest] = [:]
    private var activeTask: Task<Void, Never>?
    private var activeID: UUID?
    private let defaults: UserDefaults

    init(defaults: UserDefaults = .standard) {
        self.defaults = defaults
        if let data = defaults.data(forKey: "atlas.local.settings"),
           let value = try? JSONDecoder().decode(LocalSettings.self, from: data) { settings = value }
    }
    var selectedTool: AtlasTool? { toolCatalog.first { $0.id == selectedID } }
    var visibleTools: [AtlasTool] {
        toolCatalog.filter { tool in
            let sectionMatches = section == "All tools" || section == tool.category ||
                (section == "Favorites" && settings.favorites.contains(tool.id)) ||
                (section == "Recent" && settings.recent.contains(tool.id))
            return sectionMatches && (search.isEmpty || "\(tool.name) \(tool.summary) \(tool.category)".localizedCaseInsensitiveContains(search))
        }.sorted { a, b in
            section == "Recent" ? (settings.recent.firstIndex(of: a.id) ?? 999) < (settings.recent.firstIndex(of: b.id) ?? 999) : a.name < b.name
        }
    }
    var currentJob: ToolJob? { jobs.first { $0.id == viewedJobID && $0.request.tool == selectedID } ?? jobs.last { $0.request.tool == selectedID } }
    var canRunScript: Bool {
        guard let job = currentJob, job.state == "Completed", job.request.options["confirmed"] != "true" else { return false }
        return job.request.input == request.input && job.request.secondary == request.secondary && job.request.outputDirectory == request.outputDirectory
    }
    func openJob(_ job: ToolJob) {
        guard let tool = toolCatalog.first(where: { $0.id == job.request.tool }) else { return }
        select(tool); request = job.request; request.options.removeValue(forKey: "confirmed"); request.options.removeValue(forKey: "apply"); viewedJobID = job.id
    }
    func clearFinishedJobs() { jobs.removeAll { $0.isFinished }; viewedJobID = nil }
    var busyCount: Int { jobs.filter { !$0.isFinished }.count }
    var canApplyRename: Bool {
        guard let job = currentJob, job.state == "Completed", job.request.options["apply"] != "true" else { return false }
        var options = job.request.options; options.removeValue(forKey: "apply")
        return request.paths == job.request.paths && request.options == options
    }
    func persist() {
        do { defaults.set(try JSONEncoder().encode(settings), forKey: "atlas.local.settings") }
        catch { notice = error.localizedDescription }
    }
    func select(_ tool: AtlasTool) {
        if let id = selectedID { drafts[id] = request }
        viewedJobID = nil
        selectedID = tool.id
        request = drafts[tool.id] ?? EngineRequest(tool: tool.id, options: tool.defaults)
    }
    func library(_ category: String = "All tools") {
        if let id = selectedID { drafts[id] = request }
        selectedID = nil; section = category; search = ""
    }
    func backToCategory() { library(selectedTool?.category ?? "All tools") }
    func enterSearch() {
        if let id = selectedID { drafts[id] = request }
        selectedID = nil
        section = "All tools"
    }
    func favorite(_ id: String) {
        if settings.favorites.contains(id) { settings.favorites.removeAll { $0 == id } }
        else { settings.favorites.append(id) }
        persist()
    }
    func addFiles(_ urls: [URL]) {
        for url in urls where !request.paths.contains(url.path) { request.paths.append(url.path) }
    }
    func chooseFiles() {
        let panel = NSOpenPanel(); panel.allowsMultipleSelection = true
        panel.canChooseDirectories = selectedTool?.category == "Files"
        panel.canChooseFiles = true
        if panel.runModal() == .OK { addFiles(panel.urls) }
    }
    func chooseOutput() {
        let panel = NSOpenPanel(); panel.canChooseDirectories = true; panel.canChooseFiles = false
        panel.canCreateDirectories = true
        if panel.runModal() == .OK { request.outputDirectory = panel.url?.path ?? "" }
    }
    func importText() {
        let panel = NSOpenPanel(); panel.canChooseDirectories = false
        if panel.runModal() == .OK, let url = panel.url {
            do {
                let size = try url.resourceValues(forKeys: [.fileSizeKey]).fileSize ?? 0
                guard size <= 8_000_000 else { throw AtlasError("Choose a UTF-8 text file smaller than 8 MB.") }
                request.input = try String(contentsOf: url, encoding: .utf8)
            } catch { notice = error.localizedDescription }
        }
    }
    func saveText(_ text: String, extension ext: String = "txt") {
        let panel = NSSavePanel(); panel.nameFieldStringValue = "Atlas-result.\(ext)"
        if panel.runModal() == .OK, let url = panel.url {
            do { try text.write(to: url, atomically: true, encoding: .utf8) }
            catch { notice = error.localizedDescription }
        }
    }
    func saveRecipe() {
        guard let tool = selectedTool else { return }
        let name = recipeName.trimmingCharacters(in: .whitespacesAndNewlines)
        guard !name.isEmpty else { notice = "Give this recipe a name."; return }
        var recipe = Recipe(name: name, tool: tool.id, options: request.options)
        if tool.id == "scripts.run" {
            recipe.command = request; recipe.command?.options.removeValue(forKey: "confirmed")
            recipe.options.removeValue(forKey: "confirmed")
        }
        settings.recipes.append(recipe)
        recipeName = ""; persist()
    }
    func loadRecipe(_ recipe: Recipe) {
        guard let tool = toolCatalog.first(where: { $0.id == recipe.tool }) else { return }
        select(tool); request.options = tool.defaults.merging(recipe.options) { _, new in new }
        if recipe.tool == "scripts.run", let command = recipe.command, command.tool == "scripts.run" { request = command; request.options.removeValue(forKey: "confirmed") }
    }
    func exportSettings() {
        do {
            let encoder = JSONEncoder(); encoder.outputFormatting = [.prettyPrinted, .sortedKeys]
            saveText(String(decoding: try encoder.encode(settings), as: UTF8.self), extension: "json")
        } catch { notice = error.localizedDescription }
    }
    func importSettings() {
        let panel = NSOpenPanel(); panel.allowedContentTypes = [.json]
        if panel.runModal() == .OK, let url = panel.url {
            do {
                let data = try Data(contentsOf: url)
                guard data.count < 1_000_000 else { throw AtlasError("Settings file is too large.") }
                let imported = try JSONDecoder().decode(LocalSettings.self, from: data)
                let ids = Set(toolCatalog.map(\.id))
                settings.favorites = Array(Set(settings.favorites + imported.favorites.filter(ids.contains)))
                settings.recipes.append(contentsOf: imported.recipes.filter { ids.contains($0.tool) && !settings.recipes.map(\.id).contains($0.id) })
                persist()
            } catch { notice = error.localizedDescription }
        }
    }
    func enqueue(applyRename: Bool = false, confirmedScript: Bool = false) {
        guard selectedTool != nil else { return }
        var snapshot = request
        if snapshot.tool == "files.rename" { snapshot.options["apply"] = applyRename ? "true" : "false" }
        if snapshot.tool == "scripts.run" { snapshot.options["confirmed"] = confirmedScript ? "true" : "false" }
        viewedJobID = nil
        jobs.append(ToolJob(request: snapshot))
        settings.recent.removeAll { $0 == snapshot.tool }; settings.recent.insert(snapshot.tool, at: 0)
        settings.recent = Array(settings.recent.prefix(12)); persist()
        startNext()
    }
    func cancel(_ id: UUID) {
        if activeID == id { activeTask?.cancel() }
        else if let index = jobs.firstIndex(where: { $0.id == id && $0.state == "Queued" }) { jobs[index].state = "Stopped" }
    }
    private func receive(_ event: EngineEvent, id: UUID) {
        guard let index = jobs.firstIndex(where: { $0.id == id }) else { return }
        if let progress = event.progress { jobs[index].progress = min(1, max(0, progress)) }
        if let files = event.files { jobs[index].files = files }
        if let rows = event.rows { jobs[index].rows = rows }
        if event.type == "progress" {
            jobs[index].message = event.text ?? "Working…"
            if jobs[index].request.tool == "scripts.run" { jobs[index].text = String((jobs[index].text + (event.text ?? "")).suffix(100_000)) }
        }
        else {
            jobs[index].text = event.text ?? ""
            jobs[index].state = event.type == "result" ? "Completed" : event.type == "cancelled" ? "Stopped" : "Failed"
            let partialFailure = event.rows?.contains { $0.contains { $0.hasPrefix("Error:") } } ?? false
            if event.type == "result" && !partialFailure { clearCompletedInputs(jobs[index].request) }
        }
    }
    private func clearCompletedInputs(_ completed: EngineRequest) {
        if completed.tool == "files.rename" && completed.options["apply"] != "true" { return }
        if completed.tool == "scripts.run" && completed.options["confirmed"] != "true" { return }
        func normalized(_ value: EngineRequest) -> EngineRequest {
            var copy = value; copy.options.removeValue(forKey: "apply"); copy.options.removeValue(forKey: "confirmed"); return copy
        }
        func cleared(_ value: EngineRequest) -> EngineRequest {
            var copy = normalized(value); copy.input = ""; copy.secondary = ""; copy.paths = []; return copy
        }
        // A completed job must never erase input edited while it was running.
        if selectedID == completed.tool {
            if normalized(request) == normalized(completed) { request = cleared(request); drafts[completed.tool] = request }
        } else if let draft = drafts[completed.tool], normalized(draft) == normalized(completed) {
            drafts[completed.tool] = cleared(draft)
        }
    }
    private func startNext() {
        guard activeTask == nil, let index = jobs.firstIndex(where: { $0.state == "Queued" }) else { return }
        let id = jobs[index].id, request = jobs[index].request
        jobs[index].state = "Running"; activeID = id
        activeTask = Task {
            do {
                let report: @Sendable (EngineEvent) async -> Void = { event in await self.receive(event, id: id) }
                try await EngineClient().run(request, onEvent: report)
            } catch is CancellationError {
                receive(.init(type: "cancelled", text: "Stopped. Any completed outputs remain available."), id: id)
            } catch { receive(.init(type: "error", text: error.localizedDescription), id: id) }
            activeTask = nil; activeID = nil; startNext()
        }
    }
}
