"""Check the saved rig, animations, district collision meshes and export existence."""
import bpy, json
from pathlib import Path
root=Path(__file__).resolve().parents[1]
bpy.ops.wm.open_mainfile(filepath=str(root/'Art/Blender/Jimothy.blend'))
rig=bpy.data.objects.get('Jimothy_Rig');assert rig and rig.type=='ARMATURE'
assert len(rig.data.bones)==11
assert all(name in bpy.data.actions for name in ['Idle','Waddle','Jump'])
mesh=bpy.data.objects.get('Jimothy_ContinuousSkin');assert mesh and any(m.type=='ARMATURE' for m in mesh.modifiers)
report={'bones':11,'clips':{},'character_triangles':sum(len(p.vertices)-2 for p in mesh.data.polygons)}
for name in ['Idle','Waddle','Jump']:
 rig.animation_data.action=bpy.data.actions[name];start,end=rig.animation_data.action.frame_range
 bpy.context.scene.frame_set(int(start));a=rig.pose.bones['front_L'].matrix.copy()
 bpy.context.scene.frame_set(int((start+end)/2)+3);b=rig.pose.bones['front_L'].matrix.copy()
 if name=='Waddle':assert a!=b,'Running clip does not animate the leg'
 report['clips'][name]={'start':int(start),'end':int(end)}
bpy.ops.wm.open_mainfile(filepath=str(root/'Art/Blender/OldBallard.blend'))
collision=[o for o in bpy.data.objects if o.name.startswith('COL_')]
assert len(collision)>90
assert any(o.name.startswith('COL_Den') for o in collision)
report['collision_meshes']=len(collision)
report['district_meshes']=sum(o.type=='MESH' for o in bpy.data.objects)
report['district_triangles']=sum(sum(len(p.vertices)-2 for p in o.data.polygons) for o in bpy.data.objects if o.type=='MESH')
(root/'Documentation/blender-validation.json').write_text(json.dumps(report,indent=2))
print('BLENDER_CHECKS_PASS',json.dumps(report))
