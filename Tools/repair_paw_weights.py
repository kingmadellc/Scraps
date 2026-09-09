import bpy, runpy
from pathlib import Path
root=Path(__file__).resolve().parents[1]
bpy.ops.wm.open_mainfile(filepath=str(root/'Art/Blender/Jimothy.blend'))
rig=bpy.data.objects['Jimothy_Rig'];skin=bpy.data.objects['Jimothy_ContinuousSkin']
shins=[b for b in rig.data.bones if b.name.endswith('_shin')]
for v in skin.data.vertices:
 if v.co.z<.23:
  name=min(shins,key=lambda b:(v.co.x-b.tail_local.x)**2+(v.co.y-b.tail_local.y)**2).name
  for group in skin.vertex_groups:group.remove([v.index])
  skin.vertex_groups[name].add([v.index],1,'REPLACE')
scene=bpy.context.scene;rig.animation_data.action=bpy.data.actions['Waddle'];scene.render.resolution_x=640;scene.render.resolution_y=640;scene.cycles.samples=8
for f in range(1,25):
 scene.frame_set(f);scene.render.filepath=str(root/'Art/Renders'/('run-v4-%02d.png'%f));bpy.ops.render.render(write_still=True)
rig.animation_data.action=bpy.data.actions['Idle'];scene.frame_set(1);scene.render.resolution_x=1920;scene.render.resolution_y=1920
bpy.ops.wm.save_as_mainfile(filepath=str(root/'Art/Blender/Jimothy.blend'),compress=True)
runpy.run_path(str(root/'Tools/export_refined_character.py'),run_name='__main__')
