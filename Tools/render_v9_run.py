import bpy
from pathlib import Path
from mathutils import Vector
root=Path(__file__).resolve().parents[1];bpy.ops.wm.open_mainfile(filepath=str(root/'Art/Blender/Jimothy-v9.blend'))
scene=bpy.context.scene;scene.render.engine='CYCLES';scene.cycles.samples=16;scene.render.resolution_x=900;scene.render.resolution_y=900;scene.render.resolution_percentage=100
rig=bpy.data.objects['Jimothy_Rig'];rig.animation_data.action=bpy.data.actions['Waddle'];scene.camera.location=(-4,-3.7,2.1);scene.camera.rotation_euler=(Vector((0,0,.9))-scene.camera.location).to_track_quat('-Z','Y').to_euler()
folder=root/'Art/Renders/v9-run';folder.mkdir(exist_ok=True)
for frame in range(1,14):
 scene.frame_set(frame);scene.render.filepath=str(folder/f'run-{frame:02d}.png');bpy.ops.render.render(write_still=True)
print('V9_RUN_FRAMES_READY')
