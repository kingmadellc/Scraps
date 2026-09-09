"""Render the actual refined Blender asset in a pose/framing comparable to the concept.
Does not alter the approved AI concept image or the game asset mesh.
"""
import sys
from pathlib import Path
sys.path.insert(0,str(Path(__file__).resolve().parent))
from blender_common import *
bpy.ops.wm.open_mainfile(filepath=str(ROOT/'Art/Blender/Jimothy.blend'))
s=bpy.context.scene;rig=bpy.data.objects['Jimothy_Rig'];skin=bpy.data.objects['Jimothy_ContinuousSkin']
rig.animation_data.action=None
for b in rig.pose.bones:b.rotation_euler=(0,0,0);b.location=(0,0,0)
rig.pose.bones['front_R'].rotation_euler.x=-.78
rig.pose.bones['front_R_shin'].rotation_euler.x=1.85
rig.pose.bones['front_L'].rotation_euler.x=-.055
rig.pose.bones['rear_L'].rotation_euler.x=.12
rig.pose.bones['rear_L_shin'].rotation_euler.x=-.19
rig.pose.bones['rear_R'].rotation_euler.x=-.08
rig.pose.bones['body'].rotation_euler.z=.025
bpy.context.view_layer.update();evaluated=skin.evaluated_get(bpy.context.evaluated_depsgraph_get());m=evaluated.to_mesh();lowest=min(v.co.z for v in m.vertices);evaluated.to_mesh_clear();rig.pose.bones['body'].location.y=.015-lowest
# Save the pose as an explicit editable action.
rig.animation_data.action=bpy.data.actions.new('ReferenceStep')
for b in rig.pose.bones:b.keyframe_insert('rotation_euler',frame=1);b.keyframe_insert('location',frame=1)
cam=s.camera;cam.location=(-4.8,-2.7,2.4);cam.rotation_euler=(Vector((0,-.04,1.10))-cam.location).to_track_quat('-Z','Y').to_euler();cam.data.type='ORTHO';cam.data.ortho_scale=3.2
s.render.resolution_x=640;s.render.resolution_y=640;s.cycles.samples=8;s.render.filepath=str(ROOT/'Art/Renders/pose-check-v4.png');bpy.ops.render.render(write_still=True)
