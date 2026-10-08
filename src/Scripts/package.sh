#!/bin/sh
set -eu
root="$(CDPATH= cd -- "$(dirname -- "$0")/../.." && pwd)"
app="$root/src/Build/Atlas.app"
if [ ! -d "$app" ]; then echo 'Run src/Scripts/build.sh first.' >&2; exit 1; fi
if [ -f "$app/Contents/Resources/dotnet-root.txt" ]; then
    echo 'Distribution blocked: this local build depends on the installed .NET runtime. Build with DISTRIBUTION=1 first.' >&2
    exit 1
fi
stage="$root/src/Build/DiskImage"
mkdir -p "$stage"
rm -rf "$stage/Atlas.app"
cp -R "$app" "$stage/Atlas.app"
ln -sfn /Applications "$stage/Applications"
hdiutil create -volname Atlas -srcfolder "$stage" -ov -format UDZO "$root/src/Build/Atlas-1.0.0-$(uname -m).dmg"
shasum -a 256 "$root/src/Build/Atlas-1.0.0-$(uname -m).dmg" > "$root/src/Build/SHA256SUMS.txt"
echo 'Unsigned local artifact only. Developer ID signing and notarization are required before public distribution.'
