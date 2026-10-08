# Contributing to Atlas

Atlas uses a native SwiftUI interface and a local C# engine. Keep processing in the existing `src/Backend/Atlas` projects; UI state and macOS presentation belong in `src/Frontend/AtlasMac`.

1. Open an issue describing the user problem before a substantial feature.
2. Branch from `main`, make one focused change and use English commit messages.
3. Restore the .NET solution, run `sh src/Scripts/build.sh`, then `sh src/Scripts/check.sh` and `python3 src/Scripts/http-check.py` on macOS. Xcode 26+ is required. The backend self-test also runs on Linux.
4. Check both light and dark appearances for visible changes. Include screenshots in the pull request.
5. Describe behavior, verification and remaining limitations. Do not commit credentials, build outputs or real user files.

Use the existing request/result protocol and tool catalog. Avoid a new layer for a single operation. Validate external input, preserve original files, expose partial failures and support cancellation for long work. Add a small behavioral check for nontrivial changes.

The current local build uses the Debug .NET configuration because ImageSharp 4 requires a build-time license key for Release. This is a tracked release blocker, not a suppression of the license check. See THIRD_PARTY_NOTICES.md.
