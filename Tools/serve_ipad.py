#!/usr/bin/env python3
"""Serve only the exported game on the local network; no project/save access."""
import argparse,functools,http.server,mimetypes
from pathlib import Path
class Handler(http.server.SimpleHTTPRequestHandler):
 def list_directory(self,path):self.send_error(404);return None
 def guess_type(self,path):
  clean=path[:-3] if path.endswith('.gz') else path
  if clean.endswith('.wasm'):return 'application/wasm'
  if clean.endswith('.js'):return 'application/javascript'
  if clean.endswith('.data'):return 'application/octet-stream'
  return super().guess_type(clean)
 def end_headers(self):
  if self.path.split('?')[0].endswith('.gz'):self.send_header('Content-Encoding','gzip')
  self.send_header('Cache-Control','no-cache')
  super().end_headers()
if __name__=='__main__':
 p=argparse.ArgumentParser();p.add_argument('--port',type=int,default=8765);p.add_argument('--directory',default=str(Path(__file__).resolve().parents[1]/'Builds/Web'));a=p.parse_args()
 root=Path(a.directory).resolve()
 if not (root/'index.html').exists():raise SystemExit('Export the Web game before starting the server.')
 http.server.ThreadingHTTPServer(('0.0.0.0',a.port),functools.partial(Handler,directory=str(root))).serve_forever()
