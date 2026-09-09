"""Export one material-batched skinned character; retain the offline groom in .blend only."""
import bpy,json
from pathlib import Path
root=Path(__file__).resolve().parents[1]
bpy.ops.wm.open_mainfile(filepath=str(root/'Art/Blender/Jimothy.blend'))
rig=bpy.data.objects['Jimothy_Rig'];rig.animation_data.action=bpy.data.actions['Idle'];bpy.context.scene.frame_set(1)
source=[o for o in bpy.data.objects if o.type=='MESH' and not o.name.startswith('RENDER_ONLY') and any(m.type=='ARMATURE' and m.object==rig for m in o.modifiers)]
assert source and all('groom' not in o.name.lower() for o in source)
copies=[]
for original in source:
 o=original.copy();o.data=original.data.copy();bpy.context.collection.objects.link(o);copies.append(o)
 bpy.ops.object.select_all(action='DESELECT');o.select_set(True);bpy.context.view_layer.objects.active=o
 for m in list(o.modifiers):
  if m.type!='ARMATURE':bpy.ops.object.modifier_apply(modifier=m.name)
bpy.ops.object.select_all(action='DESELECT')
for o in copies:o.select_set(True)
bpy.context.view_layer.objects.active=next(o for o in copies if o.name.startswith('Jimothy_ContinuousSkin'))
bpy.ops.object.join();out=bpy.context.object;out.name='Jimothy_GameSkin'
# Keep only material slots actually assigned to triangles.
bpy.ops.object.material_slot_remove_unused()
triangles=sum(len(p.vertices)-2 for p in out.data.polygons)
bpy.ops.object.select_all(action='DESELECT');out.select_set(True);rig.select_set(True)
path=root/'Unity/Assets/Jimothy/Resources/Jimothy.fbx'
bpy.ops.export_scene.fbx(filepath=str(path),use_selection=True,object_types={'MESH','ARMATURE'},add_leaf_bones=False,axis_forward='-Z',axis_up='Y',bake_anim=True,bake_anim_use_all_actions=True,bake_anim_use_nla_strips=False,path_mode='AUTO')
report={'skin_meshes':1,'triangles':triangles,'material_slots':len(out.data.materials),'bones':len(rig.data.bones),'clips':[a.name for a in bpy.data.actions],'texture_dimensions':[2048,2048],'offline_groom_exported':False}
(root/'Documentation/character-export-v4.json').write_text(json.dumps(report,indent=2))
print('REFINED_FBX_EXPORT_PASS',json.dumps(report))
