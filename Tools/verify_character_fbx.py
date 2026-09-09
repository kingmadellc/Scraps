"""Round-trip checks of the actual exported character; not Unity import validation."""
import bpy,json
from pathlib import Path
root=Path(__file__).resolve().parents[1]
bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.fbx(filepath=str(root/'Unity/Assets/Jimothy/Resources/Jimothy.fbx'))
meshes=[o for o in bpy.context.scene.objects if o.type=='MESH'];rigs=[o for o in bpy.context.scene.objects if o.type=='ARMATURE']
assert len(meshes)==1 and len(rigs)==1
mesh=meshes[0];rig=rigs[0]
assert len(rig.data.bones)==11 and len(mesh.data.uv_layers)>0
assert all(sum(g.weight for g in v.groups)>.98 for v in mesh.data.vertices)
actions=list(bpy.data.actions)
assert all(any(clip in a.name for a in actions) for clip in ['Idle','Waddle','Jump'])
report={'imported_meshes':1,'bones':11,'vertices':len(mesh.data.vertices),'triangles':sum(len(p.vertices)-2 for p in mesh.data.polygons),'materials':len(mesh.data.materials),'uv_layers':len(mesh.data.uv_layers),'all_vertices_weighted':True,'actions':[a.name for a in actions]}
(root/'Documentation/character-fbx-roundtrip.json').write_text(json.dumps(report,indent=2))
print('FBX_ROUNDTRIP_PASS',json.dumps(report))
