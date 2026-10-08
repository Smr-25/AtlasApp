import SwiftUI
import AppKit
import Combine

struct LocalContainerApp: Codable, Identifiable {
    let id: String
    let name: String
    let source: String
    let context: String
    let review: String
    var state: String
    var url: String
}

@MainActor final class ContainerModel: ObservableObject {
    @Published var apps: [LocalContainerApp] = []
    @Published var context = "colima-atlas"
    @Published var contexts = ["colima-atlas"]
    @Published var status = "Check or start a runtime to get connected."
    @Published var output = ""
    @Published var busy = false
    @Published var error: String?
    @Published var selected: LocalContainerApp?
    private var task: Task<Void, Never>?

    func run(_ request: EngineRequest, completion: (@MainActor @Sendable (String) -> Void)? = nil) {
        guard !busy else { return }
        busy = true; output = ""; error = nil
        task = Task {
            do {
                try await EngineClient().run(request) { event in
                    await self.receive(event, completion: completion)
                }
            } catch is CancellationError { output += "\nStopped. Refresh to check application state." }
            catch { self.error = error.localizedDescription }
            busy = false; task = nil
        }
    }
    private func receive(_ event: EngineEvent, completion: (@MainActor @Sendable (String) -> Void)?) {
        if event.type == "progress" { output = String((output + (event.text ?? "")).suffix(100_000)) }
        else if event.type == "result" { output = event.text ?? ""; completion?(output) }
        else { error = event.text ?? "Operation failed." }
    }
    func refresh() {
        run(.init(tool: "containers.list")) { text in
            do { self.apps = try JSONDecoder().decode([LocalContainerApp].self, from: Data(text.utf8)); self.output = "Application list refreshed." }
            catch { self.error = error.localizedDescription }
        }
    }
    func runtime(_ action: String) {
        run(.init(tool: "containers.runtime", options: ["action": action, "context": context, "confirmed": "true"])) { text in self.status = text }
    }
    func loadContexts() {
        run(.init(tool: "containers.runtime", options: ["action": "contexts"])) { text in
            self.contexts = Array(Set(["colima-atlas"] + text.split(separator: "\n").map(String.init))).sorted()
            self.output = "Docker contexts loaded."
        }
    }
    func action(_ action: String, app: LocalContainerApp) {
        run(.init(tool: "containers.action", input: app.id, options: ["action": action, "confirmed": "true"]))
    }
    func stop() { task?.cancel() }
}

struct ContainerView: View {
    @ObservedObject var model: ContainerModel
    @State private var kind = "github"
    @State private var source = ""
    @State private var name = ""
    @State private var environment = "{}"
    @State private var port = "8080"
    @State private var targetPort = "8080"
    @State private var filePath = ""
    @State private var pending: (() -> Void)?
    @State private var confirmation = ""
    @State private var showConfirmation = false

    var body: some View {
        ScrollView {
            VStack(alignment: .leading, spacing: 22) {
                HStack {
                    VStack(alignment: .leading, spacing: 5) {
                        Eyebrow(text: "Local applications")
                        Text("A home for your apps.").font(.system(size: 32, weight: .bold))
                        Text("Install and run container applications. Your data stays on this Mac.").foregroundStyle(.secondary)
                    }
                    Spacer()
                    if model.busy { ProgressView().controlSize(.small); Button("Stop operation") { model.stop() } }
                }
                runtimePanel
                HStack(alignment: .top, spacing: 18) {
                    importPanel.frame(maxWidth: .infinity)
                    VStack(alignment: .leading, spacing: 14) {
                        HStack { Eyebrow(text: "Your applications"); Spacer(); Button("Refresh") { model.refresh() }.disabled(model.busy) }
                        if model.apps.isEmpty { Text("Import a recipe, review it, then start your first app.").foregroundStyle(.secondary).padding(.vertical) }
                        ForEach(model.apps) { app in appCard(app) }
                    }.padding(20).atlasPanel().frame(maxWidth: .infinity)
                }
                if let error = model.error { Label(error, systemImage: "exclamationmark.triangle").foregroundStyle(.red).textSelection(.enabled) }
                if !model.output.isEmpty {
                    VStack(alignment: .leading, spacing: 10) {
                        HStack { Eyebrow(text: "Activity / Logs"); Spacer(); Button("Clear") { model.output = "" }.disabled(model.busy) }
                        ScrollView([.vertical, .horizontal]) { Text(model.output).font(.system(size: 11, design: .monospaced)).textSelection(.enabled).frame(maxWidth: .infinity, alignment: .leading) }.frame(height: 180).defaultScrollAnchor(.bottomLeading)
                    }.padding(18).atlasPanel()
                }
            }.padding(28)
        }
        .onAppear { if model.apps.isEmpty { model.refresh() } }
        .confirmationDialog(confirmation, isPresented: $showConfirmation, titleVisibility: .visible) {
            Button("Continue") { pending?(); pending = nil }
            Button("Cancel", role: .cancel) { pending = nil }
        }
        .sheet(item: $model.selected) { app in
            VStack(alignment: .leading, spacing: 18) {
                Text(app.name).font(.title.bold())
                Text(app.source).font(.caption).foregroundStyle(.secondary)
                Text(app.review).textSelection(.enabled)
                Text("Starting downloads and executes third-party software. Review the source and configuration first. Configuration values are stored locally with access restricted to your user.").font(.callout).foregroundStyle(.secondary)
                HStack {
                    Button("Close") { model.selected = nil }
                    Button("View configuration") { model.selected = nil; model.run(.init(tool: "containers.inspect", input: app.id)) }
                    Spacer()
                    PrimaryButton(title: "Start application", icon: "play.fill") { model.selected = nil; model.action("start", app: app) }
                }
            }.padding(26).frame(width: 630)
        }
    }
    private func confirm(_ message: String, _ action: @escaping () -> Void) { confirmation = message; pending = action; showConfirmation = true }
    private var runtimePanel: some View {
        VStack(alignment: .leading, spacing: 12) {
            HStack {
                Image(systemName: "shippingbox").foregroundStyle(AtlasStyle.accent)
                Text("Container runtime").font(.headline)
                Spacer()
                Picker("Context", selection: $model.context) { ForEach(model.contexts, id: \.self) { Text($0).tag($0) } }.frame(width: 250)
                Button("Load contexts") { model.loadContexts() }.disabled(model.busy)
            }
            Text(model.status).font(.callout).foregroundStyle(.secondary).lineLimit(4)
            HStack {
                Button("Check connection") { model.runtime("status") }
                Button("Start Atlas runtime") { confirm("Start the separate Atlas virtual machine (2 CPUs, 4 GB RAM, up to 30 GB disk)?") { model.runtime("start") } }
                Button("Stop Atlas runtime") { confirm("Stop Atlas runtime and all applications running in it? Saved volumes remain.") { model.runtime("stop") } }
                Spacer()
                Button("Install requirements…") { confirm("Install Colima, Docker CLI and Compose using your existing Homebrew installation? This downloads software and uses disk space.") { model.runtime("setup") } }
            }.disabled(model.busy)
            Text("Docker Desktop is optional. Atlas runtime uses Colima. Closing this window leaves applications running. Assisted setup requires Homebrew.").font(.caption).foregroundStyle(.secondary)
        }.padding(20).atlasPanel()
    }
    private var importPanel: some View {
        VStack(alignment: .leading, spacing: 14) {
            Eyebrow(text: "Add application")
            Picker("Source", selection: $kind) { Text("GitHub Compose").tag("github"); Text("Compose file").tag("file"); Text("Image").tag("image") }
                .onChange(of: kind) { _, value in source = "" }
            TextField("Application name", text: $name).textFieldStyle(.roundedBorder)
            if kind == "file" {
                Button(filePath.isEmpty ? "Choose Compose file…" : URL(fileURLWithPath: filePath).lastPathComponent) {
                    let panel = NSOpenPanel(); panel.canChooseDirectories = false
                    if panel.runModal() == .OK { filePath = panel.url?.path ?? "" }
                }
            } else { TextField(kind == "image" ? "ghcr.io/owner/image:tag" : "https://github.com/owner/repo/blob/main/compose.yaml", text: $source).textFieldStyle(.roundedBorder) }
            if kind == "image" { HStack { TextField("Mac port", text: $port); TextField("Container port", text: $targetPort) } }
            if kind == "github" || kind == "file" {
                Text("Environment variables · JSON object").font(.caption).foregroundStyle(.secondary)
                TextEditor(text: $environment).font(.system(size: 11, design: .monospaced)).frame(height: 75)
                Text("Published images, named volumes and localhost ports only. File references and local builds are not imported.").font(.caption).foregroundStyle(.secondary)
            }
            PrimaryButton(title: "Import & review", icon: "arrow.down") {
                let r = EngineRequest(tool: "containers.import", input: source, secondary: environment, paths: filePath.isEmpty ? [] : [filePath], options: ["kind": kind, "context": model.context, "name": name, "port": port, "targetPort": targetPort])
                model.run(r) { text in
                    do {
                        let app = try JSONDecoder().decode(LocalContainerApp.self, from: Data(text.utf8))
                        model.apps.append(app); model.selected = app; model.output = "Recipe imported. Review before starting."
                    } catch { model.error = error.localizedDescription }
                }
            }.disabled(model.busy || name.trimmingCharacters(in: .whitespaces).isEmpty)
        }.padding(20).atlasPanel()
    }
    private func appCard(_ app: LocalContainerApp) -> some View {
        VStack(alignment: .leading, spacing: 10) {
            HStack { Text(app.name).font(.headline); Spacer(); Text(app.state).font(.caption).foregroundStyle(.secondary) }
            Text(app.context).font(.caption).foregroundStyle(.secondary)
            HStack {
                Button("Start / Review") { model.selected = app }
                Button("Stop") { model.action("stop", app: app) }
                Button("Restart") { confirm("Restart \(app.name)?") { model.action("restart", app: app) } }
                Button("Logs") { model.action("logs", app: app) }
                if let url = URL(string: app.url), !app.url.isEmpty { Button("Open") { NSWorkspace.shared.open(url) } }
            }.disabled(model.busy)
            Menu("Remove…") {
                Button("Remove containers, keep data") { confirm("Remove \(app.name)'s containers? Saved data and the recipe remain.") { model.action("remove", app: app) } }
                Button("Erase containers and saved data", role: .destructive) { confirm("Permanently erase \(app.name)'s containers and named volumes? This cannot be undone.") { model.action("erase", app: app) } }
            }.disabled(model.busy)
        }.padding(14).background(.primary.opacity(0.035), in: RoundedRectangle(cornerRadius: 12))
    }
}
