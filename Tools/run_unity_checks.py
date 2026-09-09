#!/usr/bin/env python3
"""Run self-exiting Unity checks sequentially; stop on the first failure."""
import pathlib, subprocess, sys, time
root=pathlib.Path(__file__).resolve().parent.parent
unity='/Volumes/Jimothy Dev/Unity/Editors/6000.3.0f1/Unity.app/Contents/MacOS/Unity'
for name in sys.argv[1:]:
    log=root/'Logs'/('v045-'+name+'.log')
    print('START '+name,flush=True)
    start=time.monotonic()
    try:
        result=subprocess.run([unity,'-batchmode','-projectPath',str(root/'Unity'),'-executeMethod','Jimothy.Editor.'+name+'.Run','-logFile',str(log)],timeout=360)
    except subprocess.TimeoutExpired:
        print('TIMEOUT '+name,flush=True);sys.exit(2)
    print(('PASS' if result.returncode==0 else 'FAIL')+' '+name+' '+str(round(time.monotonic()-start,1))+'s',flush=True)
    if result.returncode:
        for line in log.read_text(errors='replace').splitlines():
            if any(token in line for token in ['error CS','Exception:','_AUDIT FAIL','_AUDIT false']): print(line,flush=True)
        sys.exit(result.returncode)
