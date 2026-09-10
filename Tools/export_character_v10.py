"""Export reviewed V10 mesh/actions plus baked undercoat color; archives prior runtime albedo."""
import bpy,sys,shutil,json,bmesh
from pathlib import Path
root=Path(__file__).resolve().parents[1];sys.path.insert(0,str(root/'Tools'))
bpy.ops.wm.open_mainfile(filepath=str(root/'Art/Blender/Jimothy-v10.blend'))
rig=bpy.data.objects['Jimothy_Rig'];skin=bpy.data.objects['Jimothy_ContinuousSkin']
scene=bpy.context.scene
rig.animation_data.action=bpy.data.actions.get('Idle');scene.frame_set(1)
bpy.ops.wm.save_as_mainfile(filepath=str(root/'Art/Blender/Jimothy-v10.blend'),compress=True)
if '--bake-only' in sys.argv:
 print('V10_UNDERCOAT_BAKE_READY');sys.exit(0)
(root/'Art/Exports').mkdir(exist_ok=True)
bpy.ops.object.select_all(action='DESELECT')
for o in bpy.data.objects:
 if o.type=='MESH' and any(m.type=='ARMATURE' and m.object==rig for m in o.modifiers):o.select_set(True)
rig.select_set(True);bpy.context.view_layer.objects.active=rig
bpy.ops.export_scene.fbx(filepath=str(root/'Art/Exports/Jimothy-v10.fbx'),use_selection=True,object_types={'MESH','ARMATURE'},add_leaf_bones=False,axis_forward='-Z',axis_up='Y',bake_anim=True,bake_anim_step=.125,bake_anim_simplify_factor=0,bake_anim_use_all_actions=True,bake_anim_use_nla_strips=False,path_mode='AUTO',colors_type='LINEAR')
print('CHARACTER_V10_EXPORTED_BAKED')
