import bpy,math
from pathlib import Path
from mathutils import Vector
root=Path(__file__).resolve().parents[1]
bpy.ops.wm.open_mainfile(filepath=str(root/'Art/Blender/Jimothy-v5.blend'))
s=bpy.context.scene;s.render.engine='CYCLES';s.cycles.samples=24;s.render.resolution_x=1400;s.render.resolution_y=1400;s.render.resolution_percentage=100
rig=bpy.data.objects['Jimothy_Rig'];rig.animation_data.action=bpy.data.actions['Idle'];s.frame_set(1)
s.camera.location=(-4.4,-3.2,2.4);s.camera.rotation_euler=(Vector((0,0,1.0))-s.camera.location).to_track_quat('-Z','Y').to_euler()
s.render.filepath=str(root/'Art/Renders/jimothy-v5-studio.png');bpy.ops.render.render(write_still=True)
s.render.resolution_x=640;s.render.resolution_y=640;s.cycles.samples=8
rig.animation_data.action=bpy.data.actions['Waddle']
for f in range(1,9):
 s.frame_set(f);s.render.filepath=str(root/'Art/Renders'/('gait-v5-%02d.png'%f));bpy.ops.render.render(write_still=True)
print('V5_STUDIO_RENDERED')
