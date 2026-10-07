// swift-tools-version: 6.0
import PackageDescription

let package = Package(
    name: "AtlasMac",
    platforms: [.macOS(.v15)],
    products: [.executable(name: "AtlasMac", targets: ["AtlasMac"])],
    targets: [.executableTarget(name: "AtlasMac")]
)
