#!/usr/bin/env python3
"""Exercise the real C# HTTP client against a temporary loopback server."""
import http.server, json, pathlib, subprocess, threading
class Handler(http.server.BaseHTTPRequestHandler):
    def do_POST(self):
        body = self.rfile.read(int(self.headers['Content-Length']))
        assert self.headers['X-Atlas-Test'] == 'local'
        self.send_response(201); self.send_header('Content-Type', 'application/json'); self.end_headers(); self.wfile.write(body)
    def log_message(self, *_): pass
server = http.server.HTTPServer(('127.0.0.1', 0), Handler)
thread = threading.Thread(target=server.serve_forever, daemon=True); thread.start()
try:
    root = pathlib.Path(__file__).resolve().parents[2]
    req = {'tool':'tech.http','input':f'http://127.0.0.1:{server.server_port}/check','secondary':'{"hello":"Atlas"}','options':{'method':'POST','headers':'{"X-Atlas-Test":"local"}'}}
    p = subprocess.run(['dotnet', str(root/'src/Build/Engine/Atlas.Desktop.dll')], input=json.dumps(req), text=True, capture_output=True, check=True)
    event = json.loads(p.stdout.splitlines()[-1])
    assert event['type'] == 'result' and 'HTTP 201' in event['text'] and '"hello":"Atlas"' in event['text'], event
    print('PASS: HTTP POST body, custom headers, response status and body over loopback.')
finally:
    server.shutdown(); server.server_close(); thread.join()
