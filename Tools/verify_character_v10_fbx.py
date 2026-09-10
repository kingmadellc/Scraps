"""Read-only round trip of exported V10 FBX; independent of Unity rendering."""
import bpy,json,math
from pathlib import Path
root=Path(__file__).resolve().parents[1];bpy.ops.wm.read_factory_settings(use_empty=True);bpy.ops.import_scene.fbx(filepath=str(root/'Art/Exports/Jimothy-v10.fbx'),colors_type='LINEAR')
meshes=[o for o in bpy.context.scene.objects if o.type=='MESH'];rigs=[o for o in bpy.context.scene.objects if o.type=='ARMATURE'];actions=list(bpy.data.actions)
assert len(meshes)==2 and len(rigs)==1
assert all(any(clip in a.name for a in actions) for clip in ['Idle','Walk','Waddle','Jump'])
checks=[]
for mesh in meshes:
 weights=[sum(g.weight for g in v.groups) for v in mesh.data.vertices]
 assert all(w>.98 for w in weights)
 checks.append({'mesh':mesh.name,'vertices':len(mesh.data.vertices),'triangles':sum(len(p.vertices)-2 for p in mesh.data.polygons),'color_layers':[c.name for c in mesh.data.color_attributes],'min_weight_sum':min(weights),'uv_layers':len(mesh.data.uv_layers)})
report={'passed':True,'meshes':checks,'bones':len(rigs[0].data.bones),'actions':[{'name':a.name,'range':list(a.frame_range)} for a in actions],'note':'FBX roundtrip structure passes; native/browser import, visible gait and performance are separate checks.'}
(root/'Documentation/character-v10-fbx-roundtrip.json').write_text(json.dumps(report,indent=2));print('V10_FBX_ROUNDTRIP_PASS',json.dumps(report))
