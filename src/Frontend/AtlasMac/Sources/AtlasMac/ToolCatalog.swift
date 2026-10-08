import Foundation

struct ToolOption: Identifiable, Sendable {
    let id: String
    let title: String
    var value = ""
    var choices: [String] = []
    var toggle = false
}

struct AtlasTool: Identifiable, Sendable {
    let id: String
    let name: String
    let category: String
    let icon: String
    let summary: String
    var inputHint = "Paste or type here…"
    var secondaryHint: String? = nil
    var acceptsFiles = false
    var needsOutput = false
    var noText = false
    var options: [ToolOption] = []
    var defaults: [String: String] { Dictionary(uniqueKeysWithValues: options.map { ($0.id, $0.value) }) }
    var isPDF: Bool { id.hasPrefix("pdf.") }
}

let toolCatalog: [AtlasTool] = [
    .init(id: "files.rename", name: "Batch Rename", category: "Files", icon: "character.cursor.ibeam", summary: "Give a whole batch a better name. Preview every change first.", acceptsFiles: true, needsOutput: true, noText: true, options: [
        .init(id: "prefix", title: "Prefix"), .init(id: "suffix", title: "Suffix"), .init(id: "find", title: "Find"), .init(id: "replace", title: "Replace with"), .init(id: "number", title: "Add sequence", value: "false", toggle: true), .init(id: "start", title: "Start at", value: "1"), .init(id: "date", title: "Add modified date", value: "false", toggle: true)]),
    .init(id: "files.undo", name: "Undo Rename", category: "Files", icon: "arrow.uturn.backward", summary: "Restore names using an Atlas rename record. Modified files are protected.", acceptsFiles: true, noText: true),
    .init(id: "files.duplicates", name: "Duplicate Finder", category: "Files", icon: "doc.on.doc", summary: "Find identical content with SHA256. Files are never deleted.", acceptsFiles: true, noText: true),
    .init(id: "files.info", name: "File Inspector", category: "Files", icon: "doc.text.magnifyingglass", summary: "Inspect file sizes and verify SHA256 checksums.", acceptsFiles: true, noText: true),
    .init(id: "files.zip", name: "Create ZIP", category: "Files", icon: "archivebox", summary: "Package files and folders into a new archive.", acceptsFiles: true, needsOutput: true, noText: true),
    .init(id: "files.unzip", name: "Extract ZIP", category: "Files", icon: "shippingbox", summary: "Safely extract one ZIP into a fresh folder. Up to 2 GB and 10,000 entries.", acceptsFiles: true, needsOutput: true, noText: true),
    .init(id: "images.batch", name: "Image Workshop", category: "Images", icon: "photo.stack", summary: "Resize, convert, strip metadata and rename a batch. Keep the originals.", acceptsFiles: true, needsOutput: true, noText: true, options: [
        .init(id: "format", title: "Format", value: "jpg", choices: ["jpg", "png", "webp"]), .init(id: "width", title: "Max width · 0 = auto", value: "0"), .init(id: "height", title: "Max height · 0 = auto", value: "0"), .init(id: "quality", title: "Quality · JPEG / WebP", value: "85"), .init(id: "strip", title: "Remove metadata", value: "false", toggle: true), .init(id: "prefix", title: "Filename prefix"), .init(id: "number", title: "Number filenames", value: "false", toggle: true), .init(id: "zip", title: "ZIP the results", value: "false", toggle: true)]),
    .init(id: "images.metadata", name: "Image Metadata", category: "Images", icon: "info.bubble", summary: "Read dimensions, format and embedded camera information.", acceptsFiles: true, noText: true),
    .init(id: "images.palette", name: "Color Palette", category: "Images", icon: "paintpalette", summary: "Extract dominant colors from the actual pixels of one image.", acceptsFiles: true, noText: true, options: [.init(id: "count", title: "Colors · 1–16", value: "5")]),
    .init(id: "images.contrast", name: "Contrast Check", category: "Images", icon: "circle.lefthalf.filled", summary: "Check foreground and background contrast for normal-size text.", inputHint: "Foreground · #FF5733", secondaryHint: "Background · #FFFFFF"),
    .init(id: "pdf.merge", name: "Merge PDFs", category: "PDF", icon: "doc.on.doc.fill", summary: "Combine PDFs in the order listed below.", acceptsFiles: true, needsOutput: true, noText: true),
    .init(id: "pdf.split", name: "Split PDF", category: "PDF", icon: "scissors", summary: "Save every page of one PDF as a separate document.", acceptsFiles: true, needsOutput: true, noText: true),
    .init(id: "pdf.pages", name: "Arrange Pages", category: "PDF", icon: "rectangle.3.group", summary: "Extract or reorder pages into a new PDF. Use 3,1,2 or 1-4,8.", acceptsFiles: true, needsOutput: true, noText: true, options: [.init(id: "pages", title: "Page order", value: "1")]),
    .init(id: "pdf.images", name: "Images to PDF", category: "PDF", icon: "doc.richtext", summary: "Turn images into a PDF, one image per page.", acceptsFiles: true, needsOutput: true, noText: true),
    .init(id: "pdf.text", name: "PDF to Text", category: "PDF", icon: "text.viewfinder", summary: "Extract selectable text. Scanned pages need OCR, planned for a later release.", acceptsFiles: true, noText: true),
    .init(id: "json", name: "JSON Studio", category: "Text", icon: "curlybraces", summary: "Format, validate and minify your data.", inputHint: "{\"hello\": \"Atlas\"}", options: [.init(id: "mode", title: "Operation", value: "format", choices: ["format", "minify"])]),
    .init(id: "text.clean", name: "Text Cleanup", category: "Text", icon: "text.badge.checkmark", summary: "Clean whitespace, remove duplicates and reshape lines.", options: [.init(id: "mode", title: "Operation", value: "trim", choices: ["trim", "empty", "unique", "sort", "number", "upper", "lower", "replace"]), .init(id: "find", title: "Find"), .init(id: "replace", title: "Replace with")]),
    .init(id: "text.diff", name: "Text Compare", category: "Text", icon: "arrow.left.arrow.right", summary: "See added and removed lines. Up to 2,000 lines per side.", inputHint: "Original text…", secondaryHint: "Updated text…"),
    .init(id: "text.workflow", name: "Text Recipe", category: "Text", icon: "point.3.connected.trianglepath.dotted", summary: "Chain steps in order and save them as a reusable recipe.", options: [.init(id: "steps", title: "Steps · separated by commas", value: "trim,empty,unique"), .init(id: "find", title: "Find · for replace step"), .init(id: "replace", title: "Replace with")]),
    .init(id: "text.csv", name: "CSV Table", category: "Text", icon: "tablecells", summary: "Inspect quoted CSV, select columns and export cleaned data.", options: [.init(id: "delimiter", title: "Delimiter", value: ",", choices: [",", ";", "tab"]), .init(id: "columns", title: "Columns · e.g. 1,3; blank = all"), .init(id: "unique", title: "Remove duplicate rows", value: "false", toggle: true)]),
    .init(id: "text.markdown", name: "Markdown Preview", category: "Text", icon: "text.book.closed", summary: "Preview your writing with local rendering and export HTML."),
    .init(id: "text.base64", name: "Base64", category: "Text", icon: "arrow.triangle.2.circlepath", summary: "Encode and decode UTF-8 text.", options: [.init(id: "mode", title: "Operation", value: "encode", choices: ["encode", "decode"])]),
    .init(id: "text.url", name: "URL Encoding", category: "Text", icon: "link", summary: "Encode or decode URL component text.", options: [.init(id: "mode", title: "Operation", value: "encode", choices: ["encode", "decode"])]),
    .init(id: "scripts.run", name: "Script Runner", category: "Technical", icon: "terminal", summary: "Review a command, choose its working folder and run it with live output.", inputHint: "/bin/zsh", secondaryHint: "Arguments as JSON · [\"-c\", \"pwd\"]", needsOutput: true),
    .init(id: "tech.http", name: "HTTP Client", category: "Technical", icon: "arrow.up.arrow.down", summary: "Send an HTTP request and inspect response headers, body and timing.", inputHint: "https://example.com", secondaryHint: "Optional request body", options: [.init(id: "method", title: "Method", value: "GET", choices: ["GET", "POST", "PUT", "PATCH", "DELETE", "HEAD", "OPTIONS"]), .init(id: "headers", title: "Headers · JSON object", value: "{}"), .init(id: "contentType", title: "Content type", value: "application/json")]),
    .init(id: "tech.jwt", name: "JWT Inspector", category: "Technical", icon: "key.horizontal", summary: "Read token payloads locally. Decoding does not verify a signature."),
    .init(id: "tech.regex", name: "Regex Lab", category: "Technical", icon: "asterisk", summary: "Find matches and capture groups with a one-second regex timeout.", inputHint: "Text to search…", secondaryHint: "Regular expression…"),
    .init(id: "tech.hash", name: "Hash Generator", category: "Technical", icon: "number", summary: "Calculate text digests. Use SHA256 or SHA512 for modern checksums.", options: [.init(id: "algorithm", title: "Algorithm", value: "SHA256", choices: ["SHA256", "SHA512", "SHA1", "MD5"])]),
    .init(id: "tech.uuid", name: "UUID Generator", category: "Technical", icon: "square.grid.3x3", summary: "Generate random UUIDs, ready to copy.", noText: true, options: [.init(id: "count", title: "Count · 1–1000", value: "1")]),
    .init(id: "tech.time", name: "Timestamp", category: "Technical", icon: "clock", summary: "Convert Unix seconds to ISO dates and back.", inputHint: "Unix seconds or 2026-10-07T12:00:00Z"),
    .init(id: "tech.dns", name: "DNS Lookup", category: "Technical", icon: "network", summary: "Resolve a hostname using your current network. Internet may be required.", inputHint: "example.com"),
    .init(id: "tech.tls", name: "TLS Certificate", category: "Technical", icon: "lock.shield", summary: "Inspect a trusted HTTPS certificate on port 443. Uses the network.", inputHint: "example.com"),
    .init(id: "tech.ports", name: "Listening Ports", category: "Technical", icon: "externaldrive.connected.to.line.below", summary: "See TCP listeners and processes visible to your macOS account.", noText: true)
]

struct EngineRequest: Codable, Sendable {
    var tool: String
    var input = ""
    var secondary = ""
    var paths: [String] = []
    var outputDirectory = ""
    var options: [String: String] = [:]
}
struct EngineEvent: Codable, Sendable {
    var type: String
    var text: String?
    var progress: Double?
    var files: [String]?
    var rows: [[String]]?
}
struct Recipe: Codable, Identifiable {
    var id = UUID()
    var name: String
    var tool: String
    var options: [String: String]
    var command: EngineRequest? = nil
}
struct LocalSettings: Codable {
    var favorites: [String] = []
    var recent: [String] = []
    var recipes: [Recipe] = []
    var theme: String? = nil
    var snippets: [Snippet]? = nil
}

struct Snippet: Codable, Identifiable {
    var id = UUID()
    var name: String
    var text: String
}
