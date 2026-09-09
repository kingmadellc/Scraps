import bpy,json,math,sys
from pathlib import Path
root=Path(__file__).resolve().parents[1];sys.path.insert(0,str(root/'Tools'))
from refine_gait_v9 import CONTACTS,DUTIES
from refine_gait_v5 import LEGS
bpy.ops.wm.open_mainfile(filepath=str(root/'Art/Blender/Jimothy-v9.blend'))
r=bpy.data.objects['Jimothy_Rig'];s=bpy.context.scene
report={'bones':len(r.data.bones),'loop_errors':{},'max_joint_gap':0,'run_stance_z_range':{},'stance_x_range':{}}
def pose(action,frame):
 r.animation_data.action=bpy.data.actions[action];s.frame_set(int(frame),subframe=frame-int(frame));bpy.context.view_layer.update();return {b.name:[v for row in b.matrix for v in row] for b in r.pose.bones}
for action,end in [('Idle',61),('Walk',17),('Waddle',13)]:
 a=pose(action,1);b=pose(action,end);err=max(abs(x-y) for key in a for x,y in zip(a[key],b[key]));assert err<.0001,(action,err);report['loop_errors'][action]=err
z={n:[] for n in LEGS};x={n:[] for n in LEGS}
for i in range(193):
 t=i/192;pose('Waddle',1+t*12)
 for j,n in enumerate(LEGS):
  upper=r.pose.bones[n];lower=r.pose.bones[n+'_shin'];paw=r.pose.bones[n+'_paw'];gap=max((upper.tail-lower.head).length,(lower.tail-paw.head).length);report['max_joint_gap']=max(report['max_joint_gap'],gap)
  phase=(t-CONTACTS[j])%1
  if .02<phase<DUTIES[j]-.02:z[n].append(lower.tail.z);x[n].append(lower.tail.x)
for n in LEGS:
 report['run_stance_z_range'][n]=max(z[n])-min(z[n]);report['stance_x_range'][n]=max(x[n])-min(x[n]);assert report['run_stance_z_range'][n]<.002
assert report['max_joint_gap']<.001
a=pose('Idle',1);b=pose('Jump',61);report['landing_to_idle_matrix_error']=max(abs(v-w) for k in a for v,w in zip(a[k],b[k]));assert report['landing_to_idle_matrix_error']<.0001
report['passed']=True;(root/'Documentation/character-v9-gait-audit.json').write_text(json.dumps(report,indent=2));print(report)
