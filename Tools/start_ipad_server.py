#!/usr/bin/env python3
"""Start the local game server independently of the terminal session."""
from pathlib import Path
import subprocess,urllib.request,sys
root=Path(__file__).resolve().parents[1]
if not (root/'Builds/Web/index.html').exists():root=Path('/Volumes/Jimothy Dev/Projects/Jimothy')
if not (root/'Builds/Web/index.html').exists():raise SystemExit('Connect the Jimothy Dev drive before starting the game server.')
try:
 with urllib.request.urlopen('http://127.0.0.1:8765/build-info.json',timeout=2) as response:
  if response.status==200:
   print('Jimothy server already running: http://192.168.4.23:8765/');raise SystemExit(0)
except OSError:pass
(root/'Logs').mkdir(exist_ok=True)
with (root/'Logs/ipad-server.log').open('ab') as log:
 p=subprocess.Popen([sys.executable,str(root/'Tools/serve_ipad.py'),'--directory',str(root/'Builds/Web'),'--port','8765'],cwd=root,stdin=subprocess.DEVNULL,stdout=log,stderr=log,start_new_session=True)
(root/'Logs/ipad-server.pid').write_text(str(p.pid));print('Server started, PID '+str(p.pid)+': http://192.168.4.23:8765/')
