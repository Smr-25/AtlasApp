import SwiftUI
import AppKit

struct AppearanceSettings: View {
    @ObservedObject var model: Workbench
    var body: some View {
        VStack(alignment: .leading, spacing: 24) {
            Eyebrow(text: "Make it yours")
            Text("Appearance").font(.system(size: 32, weight: .bold))
            Text("Choose a look, or let Atlas follow your Mac.").foregroundStyle(.secondary)
            Picker("Theme", selection: Binding(get: { model.settings.theme ?? "system" }, set: { model.settings.theme = $0; model.persist() })) {
                Text("System").tag("system"); Text("Light").tag("light"); Text("Dark").tag("dark")
            }.pickerStyle(.segmented).frame(width: 360)
            Spacer()
        }.padding(32).frame(maxWidth: .infinity, maxHeight: .infinity, alignment: .topLeading)
    }
}

struct SnippetView: View {
    @ObservedObject var model: Workbench
    @State private var name = ""
    @State private var text = ""
    @State private var editing: UUID?
    var body: some View {
        HSplitView {
            VStack(alignment: .leading, spacing: 16) {
                HStack { Text("Snippets").font(.title2.bold()); Spacer(); Button("New") { editing = nil; name = ""; text = "" } }
                Text("Only text you explicitly save is kept. Clipboard history is not recorded.").font(.caption).foregroundStyle(.secondary)
                List(model.settings.snippets ?? []) { item in
                    HStack {
                        Button { editing = item.id; name = item.name; text = item.text } label: { Text(item.name).frame(maxWidth: .infinity, alignment: .leading).contentShape(Rectangle()) }.buttonStyle(.plain)
                        Button { model.settings.snippets?.removeAll { $0.id == item.id }; model.persist(); if editing == item.id { editing = nil; name = ""; text = "" } } label: { Image(systemName: "trash") }.help("Delete snippet")
                    }
                }
            }.padding(24).frame(minWidth: 230, idealWidth: 280, maxWidth: 340)
            VStack(alignment: .leading, spacing: 16) {
                TextField("Snippet name", text: $name).textFieldStyle(.roundedBorder)
                TextEditor(text: $text).font(.system(size: 13, design: .monospaced)).padding(10).atlasPanel()
                HStack {
                    Button("Paste") { text = NSPasteboard.general.string(forType: .string) ?? "" }
                    Button("Copy") { NSPasteboard.general.clearContents(); NSPasteboard.general.setString(text, forType: .string) }
                    Spacer()
                    PrimaryButton(title: "Save snippet", icon: "bookmark") {
                        let item = Snippet(id: editing ?? UUID(), name: name.trimmingCharacters(in: .whitespacesAndNewlines), text: text)
                        var items = model.settings.snippets ?? []; items.removeAll { $0.id == item.id }; items.append(item)
                        model.settings.snippets = items; editing = item.id; model.persist()
                    }.disabled(name.trimmingCharacters(in: .whitespacesAndNewlines).isEmpty || text.utf8.count > 1_000_000)
                }
            }.padding(24).frame(minWidth: 350)
        }
    }
}

struct AppAppearance: ViewModifier {
    @ObservedObject var model: Workbench
    @Environment(\.colorScheme) private var scheme
    private var chosen: ColorScheme? { model.settings.theme == "dark" ? .dark : model.settings.theme == "light" ? .light : nil }
    func body(content: Content) -> some View {
        content.preferredColorScheme(chosen)
            .onAppear { updateIcon() }
            .onChange(of: model.settings.theme) { updateIcon() }
            .onChange(of: scheme) { updateIcon() }
    }
    private func updateIcon() { NSApplication.shared.applicationIconImage = AtlasAppIcon.image(dark: (chosen ?? scheme) == .dark) }
}
