"""Send one observed-input command to an isolated development-build session."""
import argparse,json,time,os
from pathlib import Path
p=argparse.ArgumentParser();p.add_argument('--session',required=True);p.add_argument('--action',default='wait');p.add_argument('--frames',type=int,default=1);p.add_argument('--x',type=float,default=0);p.add_argument('--y',type=float,default=0);p.add_argument('--look-x',type=float,default=0);p.add_argument('--look-y',type=float,default=0);p.add_argument('--jump',action='store_true');p.add_argument('--click-x',type=float,default=0);p.add_argument('--click-y',type=float,default=0);a=p.parse_args()
d=Path(a.session);d.mkdir(parents=True,exist_ok=True)
previous=-1
for name in ['cmd.json','result.json']:
 try:previous=max(previous,json.loads((d/name).read_text())['id'])
 except (OSError,ValueError,KeyError):pass
cmd=dict(id=previous+1,action=a.action,frames=max(1,min(180,a.frames)),inputX=a.x,inputY=a.y,lookX=a.look_x,lookY=a.look_y,jump=a.jump,clickX=a.click_x,clickY=a.click_y)
tmp=d/'cmd.tmp';tmp.write_text(json.dumps(cmd));os.replace(tmp,d/'cmd.json')
limit=time.monotonic()+40
while time.monotonic()<limit:
 try:
  r=json.loads((d/'result.json').read_text())
  if r['id']==cmd['id']:
   (d/f'result-{r["id"]}.json').write_text(json.dumps(r,indent=2));print(json.dumps(r));break
 except (OSError,ValueError,KeyError):pass
 time.sleep(.15)
else:raise SystemExit('No matching result within40sec. Inspect session player.log and process status before retrying.')
