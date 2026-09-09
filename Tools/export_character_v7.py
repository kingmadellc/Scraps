"""Export reviewed V7 mesh/actions plus baked undercoat color; archives prior runtime albedo."""
import bpy,sys,shutil,json,bmesh
from pathlib import Path
root=Path(__file__).resolve().parents[1];sys.path.insert(0,str(root/'Tools'))
bpy.ops.wm.open_mainfile(filepath=str(root/'Art/Blender/Jimothy-v7.blend'))
rig=bpy.data.objects['Jimothy_Rig'];skin=bpy.data.objects['Jimothy_ContinuousSkin']
if (root/'Tools/refine_gait_v7.py').exists():
 import refine_gait_v7
 report=refine_gait_v7.refine(rig)
 (root/'Documentation/gait-v7.json').write_text(json.dumps(report,indent=2,default=str))
scene=bpy.context.scene
if '--skip-bake' not in sys.argv:
 scene.render.engine='CYCLES';scene.cycles.samples=8
 image=bpy.data.images.new('Jimothy V7 baked directional undercoat',width=2048,height=2048,alpha=True)
 bakeSkin=skin.copy();bakeSkin.data=skin.data.copy();bpy.context.collection.objects.link(bakeSkin)
 bm=bmesh.new();bm.from_mesh(bakeSkin.data);bmesh.ops.delete(bm,geom=[f for f in bm.faces if 'painted coat' not in bakeSkin.data.materials[f.material_index].name],context='FACES');bm.to_mesh(bakeSkin.data);bm.free()
 for mat in skin.data.materials:
  if not mat.use_nodes:continue
  node=mat.node_tree.nodes.new('ShaderNodeTexImage');node.image=image;mat.node_tree.nodes.active=node
 bpy.ops.object.select_all(action='DESELECT');bakeSkin.select_set(True);bpy.context.view_layer.objects.active=bakeSkin
 scene.render.bake.use_pass_direct=False;scene.render.bake.use_pass_indirect=False;scene.render.bake.use_pass_color=True;scene.render.bake.margin=12
 bpy.ops.object.bake(type='DIFFUSE')
 bakedMesh=bakeSkin.data;bpy.data.objects.remove(bakeSkin,do_unlink=True);bpy.data.meshes.remove(bakedMesh)
 path=root/'Unity/Assets/Jimothy/Resources/Jimothy_Albedo_2K.png';archive=root/'Art/Textures/Jimothy_Albedo_v6.png';archive.parent.mkdir(exist_ok=True)
 if not archive.exists():shutil.copy2(path,archive)
 image.filepath_raw=str(path);image.file_format='PNG';image.save();image.pack()
rig.animation_data.action=bpy.data.actions.get('Idle');scene.frame_set(1)
bpy.ops.wm.save_as_mainfile(filepath=str(root/'Art/Blender/Jimothy-v7.blend'),compress=True)
if '--bake-only' in sys.argv:
 print('V7_UNDERCOAT_BAKE_READY');sys.exit(0)
bpy.ops.object.select_all(action='DESELECT')
for o in bpy.data.objects:
 if o.type=='MESH' and any(m.type=='ARMATURE' and m.object==rig for m in o.modifiers):o.select_set(True)
rig.select_set(True);bpy.context.view_layer.objects.active=rig
bpy.ops.export_scene.fbx(filepath=str(root/'Unity/Assets/Jimothy/Resources/Jimothy.fbx'),use_selection=True,object_types={'MESH','ARMATURE'},add_leaf_bones=False,axis_forward='-Z',axis_up='Y',bake_anim=True,bake_anim_step=.125,bake_anim_simplify_factor=0,bake_anim_use_all_actions=True,bake_anim_use_nla_strips=False,path_mode='AUTO')
print('CHARACTER_V7_EXPORTED_BAKED')
