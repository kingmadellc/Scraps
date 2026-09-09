import bpy,json,math
from pathlib import Path
root=Path('/Volumes/Jimothy Dev/Projects/Jimothy')
report={}
for source in ['blend','fbx']:
 bpy.ops.wm.read_factory_settings(use_empty=True)
 if source=='blend':bpy.ops.wm.open_mainfile(filepath=str(root/'Art/Blender/BallardHuman.blend'))
 else:bpy.ops.import_scene.fbx(filepath=str(root/'Unity/Assets/Jimothy/Resources/BallardHuman.fbx'))
 rig=next(o for o in bpy.data.objects if o.type=='ARMATURE')
 report[source]={}
 for clip,duty,speed in [('Walk',.64,1),('Run',.40,3)]:
  act=next(a for a in bpy.data.actions if a.name.split('|')[-1]==clip or a.name==clip)
  rig.animation_data.action=act;
  if hasattr(act,"slots") and len(act.slots):rig.animation_data.action_slot=act.slots[0]
  begin,end=act.frame_range;period=(end-begin)/bpy.context.scene.render.fps
  samples=[];maxerr=0;maxz=0;baseline=None;first=None;last=None
  for i in range(241):
   phase=i/240;bpy.context.scene.frame_set(int(begin+phase*(end-begin)),subframe=(begin+phase*(end-begin))%1);p=rig.pose.bones['foot_L'].head.copy()
   if source=='fbx':p=rig.matrix_world@p
   if i==0:first=p.copy()
   if i==240:last=p.copy()
   if .03<phase<duty-.03:
    world=p.copy();world.y-=speed*phase*period
    if baseline is None:baseline=world.copy()
    maxerr=max(maxerr,(world-baseline).length);maxz=max(maxz,abs(world.z-baseline.z))
  report[source][clip]={'stance_compensated_drift_m':maxerr,'vertical_stance_error_m':maxz,'loop_foot_error_m':(last-first).length,'frames':[begin,end],'fps':bpy.context.scene.render.fps}
(root/'Documentation/neighbor-motion-review.json').write_text(json.dumps(report,indent=2));print(json.dumps(report))
