import bpy,json,math
from pathlib import Path
root=Path(__file__).resolve().parents[1];bpy.ops.wm.open_mainfile(filepath=str(root/'Art/Blender/Jimothy-v7.blend'))
rig=bpy.data.objects['Jimothy_Rig'];checks=[]
for obj in bpy.data.objects:
 if obj.type!='MESH' or not any(m.type=='ARMATURE' and m.object==rig for m in obj.modifiers):continue
 sums=[sum(g.weight for g in v.groups) for v in obj.data.vertices]
 checks.append({'object':obj.name,'vertices':len(obj.data.vertices),'triangles':sum(len(p.vertices)-2 for p in obj.data.polygons),'unweighted':sum(w<.001 for w in sums),'nonfinite_vertices':sum(not all(math.isfinite(c) for c in v.co) for v in obj.data.vertices),'min_weight_sum':min(sums),'max_weight_sum':max(sums),'missing_bone_groups':[g.name for g in obj.vertex_groups if g.name not in rig.data.bones]})
report={'checks':checks,'passed':all(c['unweighted']==0 and c['nonfinite_vertices']==0 and not c['missing_bone_groups'] for c in checks),'note':'Source mesh structural checks only; FBX import, animation and rendering require separate verification.'}
(root/'Documentation/character-v7-structure.json').write_text(json.dumps(report,indent=2));print(json.dumps(report))
