#!/bin/sh
set -eu

root="$(CDPATH= cd -- "$(dirname -- "$0")/../.." && pwd)"
dotnet_dir="$(dirname "$(realpath "$(command -v dotnet)")")"
if [ ! -d "$dotnet_dir/shared" ]; then dotnet_dir="$(dirname "$dotnet_dir")/libexec"; fi
export DOTNET_ROOT="$dotnet_dir"
dotnet build "$root/macos/AtlasEngine/AtlasEngine.csproj" -v:q
export ATLAS_ENGINE_PATH="$root/macos/AtlasEngine/bin/Debug/net10.0/AtlasEngine"
cd "$root/macos/AtlasMac"
swift run AtlasMac
