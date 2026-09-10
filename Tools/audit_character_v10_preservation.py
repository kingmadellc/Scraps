"""Compare V9/V10 anatomy, bind skeleton, and every action key. Does not write assets."""
import bpy,json,hashlib
from pathlib import Path
root=Path(__file__).resolve().parents[1]
def signature(version):
 bpy.ops.wm.open_mainfile(filepath=str(root/f'Art/Blender/Jimothy-{version}.blend'))
 rig=bpy.data.objects['Jimothy_Rig'];skin=bpy.data.objects['Jimothy_ContinuousSkin']
 data={'skin':[tuple(v.co) for v in skin.data.vertices], 'bones':[(b.name,tuple(b.head_local),tuple(b.tail_local),tuple(tuple(row) for row in b.matrix_local)) for b in rig.data.bones], 'actions':[(a.name,[(f.data_path,f.array_index,[(tuple(k.co),tuple(k.handle_left),tuple(k.handle_right),k.interpolation) for k in f.keyframe_points]) for f in a.fcurves]) for a in bpy.data.actions]}
 return {key:hashlib.sha256(repr(value).encode()).hexdigest() for key,value in data.items()}
a=signature('v9');b=signature('v10');assert a==b,(a,b)
report={'passed':True,'identical_skin_bind_skeleton_and_action_keys':a}
(root/'Documentation/character-v10-preservation.json').write_text(json.dumps(report,indent=2));print(report)
