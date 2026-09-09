"""Rebind fused jacket by anatomical region; stage asset outside Unity for review."""
import bpy,sys,math,json
from pathlib import Path
from mathutils import Vector,Matrix
ROOT=Path('/Volumes/Jimothy Dev/Projects/Jimothy');sys.path.insert(0,str(ROOT/'Tools'))
from build_neighbors_v5 import aim
bpy.ops.wm.open_mainfile(filepath=str(ROOT/'Art/Blender/BallardHuman.blend'))
rig=next(o for o in bpy.data.objects if o.type=='ARMATURE');skin=next(o for o in bpy.data.objects if o.type=='MESH' and any(m.type=='ARMATURE' for m in o.modifiers))
def smooth(a,b,x):
 t=max(0,min(1,(x-a)/(b-a)));return t*t*(3-2*t)
ids={v for p in skin.data.polygons if 'Neighbor jacket' in skin.data.materials[p.material_index].name for v in p.vertices}
changed=0
for i in ids:
 v=skin.data.vertices[i];x,y,z=v.co;edge=.190+.027*smooth(1.25,1.39,z)
 arm=smooth(edge,edge+.020+.048*smooth(1.23,1.39,z),abs(x));upper=smooth(1.10,1.19,z);spine=smooth(.98,1.16,z);side='L' if x<0 else 'R'
 weights={'body':(1-arm)*(1-spine),'spine':(1-arm)*spine,'arm_'+side:arm*upper,'arm_'+side+'_fore':arm*(1-upper)}
 for group in skin.vertex_groups:group.remove([i])
 for name,w in weights.items():
  if w>1e-6:skin.vertex_groups[name].add([i],w,'REPLACE')
 changed+=1
out=ROOT/'Art/Staged/NPC';out.mkdir(parents=True,exist_ok=True)
# Preserve original actions and skin topology; export before applying review-only pose.
rig.animation_data.action=bpy.data.actions['Idle'];bpy.context.scene.frame_set(1)
bpy.ops.object.select_all(action='DESELECT');rig.select_set(True);skin.select_set(True);bpy.context.view_layer.objects.active=rig
bpy.ops.wm.save_as_mainfile(filepath=str(out/'BallardHuman-jacket-fixed.blend'),compress=True)
bpy.ops.export_scene.fbx(filepath=str(out/'BallardHuman.fbx'),use_selection=True,object_types={'MESH','ARMATURE'},add_leaf_bones=False,axis_forward='-Z',axis_up='Y',bake_anim=True,bake_anim_use_all_actions=True,bake_anim_use_nla_strips=False,bake_anim_step=.25,bake_anim_simplify_factor=0,path_mode='COPY',embed_textures=True)
rig.animation_data.action=None
for b in rig.pose.bones:b.matrix_basis=Matrix.Identity(4)
# Match the measured-wall Unity audit: fixed feet, slight upper-body lean, wrist supported.
spine=rig.pose.bones['spine'];head=rig.pose.bones['head'];spine.matrix=Matrix.Translation(spine.head)@Matrix.Rotation(math.radians(7),4,'Y')@Matrix.Translation(-spine.head)@spine.matrix;bpy.context.view_layer.update()
u,l,h=[rig.pose.bones[n] for n in ['arm_R','arm_R_fore','hand_R']];target=Vector((.435,0,1.15));a=u.head.copy();l1=u.length;l2=l.length;delta=target-a;d=delta.length;direction=delta.normalized();bend=Vector((0,0,-1));bend-=direction*bend.dot(direction);bend.normalize();along=(l1*l1-l2*l2+d*d)/(2*d);elbow=a+direction*along+bend*math.sqrt(max(0,l1*l1-along*along));aim(u,a,elbow);aim(l,elbow,target)
# Existing review camera and lights are outside exported selection.
scene=bpy.context.scene;scene.camera.location=(2.6,-4,2);scene.camera.rotation_euler=(Vector((0,0,1.0))-scene.camera.location).to_track_quat('-Z','Y').to_euler();scene.camera.data.type='ORTHO';scene.camera.data.ortho_scale=2.2;scene.render.resolution_x=900;scene.render.resolution_y=900;scene.render.resolution_percentage=100;scene.cycles.samples=24;scene.render.filepath=str(out/'supported-jacket.png');bpy.ops.render.render(write_still=True)
report={'reweighted_jacket_vertices':changed,'mesh_vertices_unchanged':len(skin.data.vertices),'bones_unchanged':len(rig.data.bones),'supported_wrist_error_m':(h.head-target).length,'note':'Fixed torso/upper sleeve/fore sleeve regional weights on existing mesh; same bones and gait actions. Blender review pose approximates measured Unity wall-support pose; runtime re-audit required.'};(out/'jacket-weight-repair.json').write_text(json.dumps(report,indent=2));print(report)
