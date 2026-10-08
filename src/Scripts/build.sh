#!/bin/sh
set -eu
root="$(CDPATH= cd -- "$(dirname -- "$0")/../.." && pwd)"
if [ -z "${DEVELOPER_DIR:-}" ] && [ -d /Applications/Xcode.app/Contents/Developer ]; then export DEVELOPER_DIR=/Applications/Xcode.app/Contents/Developer; fi
output="$root/src/Build"
engine="$root/src/Backend/Atlas/Presentation/Atlas.Desktop"
frontend="$root/src/Frontend/AtlasMac"
mkdir -p "$output"
rm -rf "$output/Engine" "$output/Atlas.app/Contents/Resources/Engine"
configuration="${CONFIGURATION:-Debug}"
if [ "${DISTRIBUTION:-0}" = 1 ]; then
    configuration=Release
    case "$(uname -m)" in arm64) runtime=osx-arm64 ;; x86_64) runtime=osx-x64 ;; *) exit 1 ;; esac
    dotnet publish "$engine/Atlas.Desktop.csproj" -c "$configuration" -r "$runtime" --self-contained true -o "$output/Engine" -v:q
else
    dotnet publish "$engine/Atlas.Desktop.csproj" -c "$configuration" --self-contained false --no-restore -o "$output/Engine" -v:q -p:UseSharedCompilation=false
fi
xcrun swift build --package-path "$frontend" -c release
swift_bin="$(xcrun swift build --package-path "$frontend" -c release --show-bin-path)"
app="$output/Atlas.app"
mkdir -p "$app/Contents/MacOS" "$app/Contents/Resources/Engine"
cp "$swift_bin/AtlasMac" "$app/Contents/MacOS/AtlasMac"
cp -R "$output/Engine/." "$app/Contents/Resources/Engine/"
dotnet_dir="$(dirname "$(realpath "$(command -v dotnet)")")"
if [ ! -d "$dotnet_dir/shared" ]; then dotnet_dir="$(dirname "$dotnet_dir")/libexec"; fi
if [ "${DISTRIBUTION:-0}" != 1 ]; then
    printf '%s' "$dotnet_dir" > "$app/Contents/Resources/dotnet-root.txt"
else
    rm -f "$app/Contents/Resources/dotnet-root.txt"
fi
"$swift_bin/AtlasMac" --render-icons "$output/Icons"
iconset="$output/Icons/Atlas.iconset"
mkdir -p "$iconset"
for size in 16 32 128 256 512; do
    sips -z "$size" "$size" "$output/Icons/icon-light.png" --out "$iconset/icon_${size}x${size}.png" >/dev/null
    double=$((size * 2))
    sips -z "$double" "$double" "$output/Icons/icon-light.png" --out "$iconset/icon_${size}x${size}@2x.png" >/dev/null
done
iconutil -c icns "$iconset" -o "$app/Contents/Resources/Atlas.icns"
if [ -f "$root/THIRD_PARTY_NOTICES.md" ]; then cp "$root/THIRD_PARTY_NOTICES.md" "$app/Contents/Resources/"; fi
if [ -d "$root/src/Docs/Licenses" ]; then cp -R "$root/src/Docs/Licenses" "$app/Contents/Resources/"; fi
cat > "$app/Contents/Info.plist" <<'PLIST'
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0"><dict>
<key>CFBundleName</key><string>Atlas</string>
<key>CFBundleDisplayName</key><string>Atlas</string>
<key>CFBundleIdentifier</key><string>app.atlas.local</string>
<key>CFBundleExecutable</key><string>AtlasMac</string>
<key>CFBundlePackageType</key><string>APPL</string>
<key>CFBundleShortVersionString</key><string>1.0.0</string>
<key>CFBundleVersion</key><string>1</string>
<key>LSMinimumSystemVersion</key><string>15.0</string>
<key>CFBundleIconFile</key><string>Atlas</string>
<key>NSHighResolutionCapable</key><true/>
</dict></plist>
PLIST
codesign --force --deep --sign - "$app"
printf 'Built %s\n' "$app"
