import SwiftUI
import AppKit
import CoreImage

struct AtlasMark: Shape {
    func path(in rect: CGRect) -> Path {
        var p = Path()
        p.move(to: CGPoint(x: 0.06, y: 1))
        p.addQuadCurve(to: CGPoint(x: 0.01, y: 0.9), control: CGPoint(x: -0.02, y: 1))
        p.addLine(to: CGPoint(x: 0.40, y: 0.07))
        p.addQuadCurve(to: CGPoint(x: 0.49, y: 0), control: CGPoint(x: 0.43, y: 0))
        p.addLine(to: CGPoint(x: 0.55, y: 0))
        p.addQuadCurve(to: CGPoint(x: 0.63, y: 0.07), control: CGPoint(x: 0.60, y: 0))
        p.addLine(to: CGPoint(x: 0.99, y: 0.91))
        p.addQuadCurve(to: CGPoint(x: 0.95, y: 1), control: CGPoint(x: 1.03, y: 1))
        p.addLine(to: CGPoint(x: 0.73, y: 1))
        p.addQuadCurve(to: CGPoint(x: 0.66, y: 0.97), control: CGPoint(x: 0.70, y: 1))
        p.addLine(to: CGPoint(x: 0.42, y: 0.76))
        p.addQuadCurve(to: CGPoint(x: 0.40, y: 0.69), control: CGPoint(x: 0.40, y: 0.74))
        p.addLine(to: CGPoint(x: 0.40, y: 0.55))
        p.addLine(to: CGPoint(x: 0.71, y: 0.74))
        p.addLine(to: CGPoint(x: 0.50, y: 0.30))
        p.addLine(to: CGPoint(x: 0.23, y: 0.93))
        p.addQuadCurve(to: CGPoint(x: 0.14, y: 1), control: CGPoint(x: 0.20, y: 1))
        p.closeSubpath()
        return p.applying(CGAffineTransform(scaleX: rect.width, y: rect.height).concatenating(.init(translationX: rect.minX, y: rect.minY)))
    }
}

struct AtlasWordmark: View {
    static let image: NSImage = {
        // Use the approved bitmap's luminance as transparency, preserving its letterforms.
        let source = CIImage(contentsOf: Bundle.module.url(forResource: "AtlasWordmark", withExtension: "png")!)!
        let mask = source.applyingFilter("CIColorControls", parameters: [kCIInputContrastKey: 3])
            .applyingFilter("CIMaskToAlpha")
        let crop = CGRect(x: 275, y: 270, width: 1240, height: 345)
        let image = NSImage(cgImage: CIContext().createCGImage(mask, from: crop)!, size: crop.size)
        image.isTemplate = true
        return image
    }()

    var body: some View {
        Image(nsImage: Self.image).resizable().scaledToFit()
            .foregroundStyle(AtlasStyle.accent).frame(width: 155, height: 47)
            .accessibilityLabel("Atlas")
    }
}

struct AtlasAppIcon: View {
    var dark = false
    var body: some View {
        RoundedRectangle(cornerRadius: 210, style: .continuous)
            .fill(dark ? Color(red: 0.035, green: 0.035, blue: 0.04) : AtlasStyle.accent)
            .overlay { AtlasMark().fill(dark ? AtlasStyle.accent : .white).frame(width: 590, height: 475) }
            .padding(55).frame(width: 1024, height: 1024)
    }
    @MainActor static func image(dark: Bool) -> NSImage? {
        ImageRenderer(content: AtlasAppIcon(dark: dark)).nsImage
    }
}
