import bpy,math
from pathlib import Path
from mathutils import Vector
root=Path(__file__).resolve().parents[1]
bpy.ops.wm.open_mainfile(filepath=str(root/'Art/Blender/Jimothy-v10.blend'))
s=bpy.context.scene;s.render.engine='CYCLES';s.cycles.samples=24;s.render.resolution_x=1400;s.render.resolution_y=1400;s.render.resolution_percentage=100
rig=bpy.data.objects['Jimothy_Rig'];rig.animation_data.action=bpy.data.actions['Idle'];s.frame_set(1)
s.render.resolution_x=900;s.render.resolution_y=900;s.cycles.samples=12
for name,loc in [('rear',(0,5,3.4)),('side',(-5,.2,2.5)),('front',(-3,-4,2.7))]:
 s.camera.location=loc;s.camera.rotation_euler=(Vector((0,0,.9))-s.camera.location).to_track_quat('-Z','Y').to_euler()
 s.render.filepath=str(root/'Art/Renders'/('jimothy-v10-'+name+'.png'));bpy.ops.render.render(write_still=True)
