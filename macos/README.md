# Atlas for Mac — first slice

The SwiftUI app currently has one complete tool: JSON Studio. It formats and
minifies JSON through the local C# engine. Input is processed on this Mac.

Requirements: .NET 10 SDK and a working SwiftUI toolchain. Run:

```sh
sh macos/scripts/run.sh
```

The script builds the C# engine, sets its local path, and starts the Swift app.
For a quick engine check:

```sh
dotnet run --project macos/AtlasEngine/AtlasEngine.csproj -- --self-test
```
