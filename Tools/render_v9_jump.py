import bpy
from pathlib import Path
from mathutils import Vector
root=Path(__file__).resolve().parents[1];bpy.ops.wm.open_mainfile(filepath=str(root/'Art/Blender/Jimothy-v9.blend'))
s=bpy.context.scene;s.render.engine='CYCLES';s.cycles.samples=12;s.render.resolution_x=900;s.render.resolution_y=900;s.render.resolution_percentage=100
rig=bpy.data.objects['Jimothy_Rig'];rig.animation_data.action=bpy.data.actions['Jump'];s.camera.location=(-4,-3.7,2.1);s.camera.rotation_euler=(Vector((0,0,.9))-s.camera.location).to_track_quat('-Z','Y').to_euler()
folder=root/'Art/Renders/v9-jump';folder.mkdir(exist_ok=True)
for frame in [1,11,30,47,51,61]:
 s.frame_set(frame);s.render.filepath=str(folder/f'jump-{frame:02d}.png');bpy.ops.render.render(write_still=True)
