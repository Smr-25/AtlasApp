"""Black-box tests of the shipped newline-delimited JSON protocol; no third-party packages."""
import json
from pathlib import Path
import subprocess
import sys
import tempfile
import unittest

ENGINE = Path(sys.argv.pop(1)).resolve() if len(sys.argv) > 1 else Path(__file__).resolve().parents[1] / "src/Backend/Atlas/Presentation/Atlas.Desktop/bin/Debug/net10.0/Atlas.Desktop.dll"


class EngineProtocolTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        if not ENGINE.is_file():
            raise RuntimeError(f"Build the backend before testing: {ENGINE}")

    def invoke(self, request):
        payload = request if isinstance(request, str) else json.dumps(request, ensure_ascii=False)
        process = subprocess.run(["dotnet", str(ENGINE)], input=payload, text=True, encoding="utf-8", capture_output=True, timeout=30)
        self.assertEqual(process.returncode, 0, process.stderr)
        events = [json.loads(line) for line in process.stdout.splitlines()]
        self.assertTrue(events, "Engine did not emit a protocol event")
        terminal = [event for event in events if event["type"] in ("result", "error", "cancelled")]
        self.assertEqual(len(terminal), 1, events)
        self.assertEqual(events[-1], terminal[0], events)
        return events

    def test_invalid_json_emits_error_instead_of_crashing(self):
        for payload in ("", "{broken", "null", "[]"):
            with self.subTest(payload=payload):
                self.assertEqual(self.invoke(payload)[-1]["type"], "error")

    def test_unknown_tool_emits_error(self):
        event = self.invoke({"tool": "not-a-tool"})[-1]
        self.assertEqual(event["type"], "error")
        self.assertIn("Unknown tool", event["text"])

    def test_unicode_and_newlines_survive_protocol(self):
        value = "Salam 🌍\nAzərbaycan\t\"quoted\"\\path"
        event = self.invoke({"tool": "text.clean", "input": value, "options": {"mode": "replace", "find": "Salam", "replace": "Hello"}})[-1]
        self.assertEqual(event["type"], "result")
        self.assertEqual(event["text"], value.replace("Salam", "Hello"))

    def test_request_and_tool_size_limits(self):
        for size, message in ((8_000_001, "8 million"), (16_000_001, "16 MB")):
            with self.subTest(size=size):
                event = self.invoke({"tool": "text.clean", "input": "x" * size})[-1]
                self.assertEqual(event["type"], "error")
                self.assertIn(message, event["text"])

    def test_structured_csv_rows_and_escaping(self):
        event = self.invoke({"tool": "text.csv", "input": 'name,note\nAtlas,"line 1\nline 2"'})[-1]
        self.assertEqual(event["type"], "result")
        self.assertEqual(event["rows"], [["name", "note"], ["Atlas", "line 1\nline 2"]])

    def test_progress_precedes_actual_archive_result(self):
        with tempfile.TemporaryDirectory(prefix="atlas-protocol-") as directory:
            root = Path(directory)
            source = root / "input.txt"
            source.write_text("safe fixture", encoding="utf-8")
            events = self.invoke({"tool": "files.zip", "paths": [str(source)], "outputDirectory": directory})
            progress = [e["progress"] for e in events if e["type"] == "progress"]
            self.assertTrue(progress)
            self.assertTrue(all(0 <= p <= 1 for p in progress))
            self.assertEqual(progress, sorted(progress))
            self.assertEqual(events[-1]["type"], "result")
            self.assertTrue(Path(events[-1]["files"][0]).is_file())
            self.assertEqual(source.read_text(encoding="utf-8"), "safe fixture")

    def test_command_preview_does_not_execute(self):
        with tempfile.TemporaryDirectory(prefix="atlas-protocol-") as directory:
            marker = Path(directory) / "must-not-exist"
            event = self.invoke({"tool": "scripts.run", "input": "/bin/sh", "secondary": json.dumps(["-c", "touch must-not-exist"]), "outputDirectory": directory})[-1]
            self.assertEqual(event["type"], "result")
            self.assertIn("Review before running", event["text"])
            self.assertFalse(marker.exists())


if __name__ == "__main__":
    unittest.main(verbosity=2)
