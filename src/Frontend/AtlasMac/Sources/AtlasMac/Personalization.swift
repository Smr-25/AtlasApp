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
