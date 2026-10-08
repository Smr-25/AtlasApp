import SwiftUI
import AppKit
import WebKit

enum AtlasStyle {
    static let accent = Color(red: 0.97, green: 0.31, blue: 0.19)
    static let ember = Color(red: 0.84, green: 0.18, blue: 0.15)
    static let background = Color(nsColor: NSColor(name: nil) { appearance in
        appearance.bestMatch(from: [.darkAqua, .aqua]) == .darkAqua ? NSColor(srgbRed: 0.045, green: 0.045, blue: 0.052, alpha: 1) : .windowBackgroundColor
    })
    static let surface = Color(nsColor: NSColor(name: nil) { appearance in
        appearance.bestMatch(from: [.darkAqua, .aqua]) == .darkAqua ? NSColor(srgbRed: 0.085, green: 0.085, blue: 0.095, alpha: 1) : .textBackgroundColor
    })
    static let gradient = LinearGradient(colors: [accent, ember], startPoint: .topLeading, endPoint: .bottomTrailing)
    static func color(_ category: String) -> Color {
        switch category {
        case "Files": return accent
        case "Images": return .yellow
        case "PDF": return .red
        case "Text": return .blue
        default: return Color(red: 0.25, green: 0.95, blue: 0.43)
        }
    }
    static func icon(_ category: String) -> String {
        switch category {
        case "Files": return "folder"
        case "Images": return "photo"
        case "PDF": return "doc.richtext"
        case "Text": return "text.alignleft"
        default: return "terminal"
        }
    }
}

extension View {
    func atlasGlass(_ radius: CGFloat = 16) -> some View { modifier(AtlasGlass(radius: radius)) }
    func atlasPanel(_ radius: CGFloat = 18) -> some View {
        self.background(AtlasStyle.surface.opacity(0.75), in: RoundedRectangle(cornerRadius: radius))
            .overlay(RoundedRectangle(cornerRadius: radius).stroke(.primary.opacity(0.07)))
    }
}

private struct AtlasGlass: ViewModifier {
    let radius: CGFloat
    func body(content: Content) -> some View {
        content.background {
            if #available(macOS 26, *) {
                Color.clear.glassEffect(.regular, in: RoundedRectangle(cornerRadius: radius))
            } else { RoundedRectangle(cornerRadius: radius).fill(.regularMaterial) }
        }
    }
}

struct Eyebrow: View {
    let text: String
    var body: some View { Text(text.uppercased()).font(.system(size: 10, weight: .bold)).tracking(1.5).foregroundStyle(.secondary) }
}
struct ToolBadge: View {
    let tool: AtlasTool
    var size: CGFloat = 44
    var body: some View {
        Image(systemName: tool.icon).font(.system(size: size * 0.42, weight: .medium))
            .foregroundStyle(AtlasStyle.color(tool.category)).frame(width: size, height: size)

    }
}
struct PrimaryButton: View {
    let title: String
    var icon = "arrow.right"
    let action: () -> Void
    var body: some View {
        Button(action: action) {
            Label(title, systemImage: icon).font(.system(size: 12, weight: .semibold))
                .foregroundStyle(.white).padding(.horizontal, 18).frame(height: 38)
                .background(AtlasStyle.gradient, in: RoundedRectangle(cornerRadius: 11)).contentShape(Rectangle())
        }.buttonStyle(.plain)
    }
}

struct HTMLPreview: NSViewRepresentable {
    let html: String
    @Environment(\.colorScheme) private var scheme
    func makeNSView(context: Context) -> WKWebView {
        let configuration = WKWebViewConfiguration()
        configuration.defaultWebpagePreferences.allowsContentJavaScript = false
        let view = WKWebView(frame: .zero, configuration: configuration)
        view.navigationDelegate = context.coordinator
        view.setValue(false, forKey: "drawsBackground")
        return view
    }
    func makeCoordinator() -> Coordinator { Coordinator() }
    func updateNSView(_ view: WKWebView, context: Context) {
        let color = scheme == .dark ? "#e8e7e5" : "#292725"
        let page = "<meta http-equiv='Content-Security-Policy' content=\"default-src 'none'; style-src 'unsafe-inline'; img-src data:;\"><style>body{font:14px -apple-system;color:\(color);line-height:1.6;padding:12px;overflow-wrap:anywhere}pre{white-space:pre-wrap;background:#8882;padding:12px;border-radius:10px}a{color:#f65335}img{max-width:100%}table{border-collapse:collapse}td,th{border:1px solid #8885;padding:6px}</style>" + html
        if context.coordinator.lastHTML != page { context.coordinator.lastHTML = page; view.loadHTMLString(page, baseURL: nil) }
    }
    @MainActor final class Coordinator: NSObject, WKNavigationDelegate {
        var lastHTML = ""
        func webView(_ webView: WKWebView, decidePolicyFor navigationAction: WKNavigationAction, decisionHandler: @escaping @MainActor (WKNavigationActionPolicy) -> Void) {
            decisionHandler(navigationAction.navigationType == .linkActivated ? .cancel : .allow)
        }
    }
}
