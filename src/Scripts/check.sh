#!/bin/sh
set -eu
root="$(CDPATH= cd -- "$(dirname -- "$0")/../.." && pwd)"
if [ -f "$root/src/Build/Atlas.app/Contents/Resources/dotnet-root.txt" ]; then
    export DOTNET_ROOT="$(cat "$root/src/Build/Atlas.app/Contents/Resources/dotnet-root.txt")"
fi
"$root/src/Build/Atlas.app/Contents/Resources/Engine/Atlas.Desktop" --self-test
"$root/src/Build/Atlas.app/Contents/MacOS/AtlasMac" --self-test
