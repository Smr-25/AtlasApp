# Atlas tests

Backend tests live in `Backend/` and compile into the existing `Atlas.Desktop --self-test` entry point. They need no additional test framework or package. Each run creates disposable fixtures and cleans them up; it does not use real user files or require a Docker engine.

```sh
dotnet build src/Backend/Atlas/Atlas.sln -c Debug
dotnet src/Backend/Atlas/Presentation/Atlas.Desktop/bin/Debug/net10.0/Atlas.Desktop.dll --self-test
python3 tests/test_engine_protocol.py
```

- `Backend/SelfTest.cs`: JSON/text, CSV, hashes, images, PDF, Compose validation and command preview/execution/cancellation.
- `Backend/FileSafetyTests.cs`: batch rename preflight and rollback, undo conflicts and repeatability, archive collisions, cancellation cleanup and extraction failures. Rejection checks require the expected exception type.
- `test_engine_protocol.py`: launches the actual engine and verifies JSON events, malformed and oversized requests, Unicode, structured rows, progress, outputs and command preview isolation. Uses Python's standard library. An optional first argument selects a different built engine DLL.

The backend suite and protocol tests run in GitHub Actions on Linux. On macOS with Xcode 26+, run the complete app checks:

```sh
dotnet restore src/Backend/Atlas/Atlas.sln
sh src/Scripts/build.sh
sh src/Scripts/check.sh
python3 src/Scripts/http-check.py
```

The macOS checks also exercise the native Swift test suite and the packaged engine. HTTP checks use a temporary loopback server. These checks do not certify visual layout, external services, notarization or every tool's behavior.
