"""Non-looping authored jump adaptation; no claim of footage-matched jump kinematics.
Call refine(rig) AFTER refine_gait_v5.refine(rig), before export. Never saves or opens.
Forward is -Y. Runtime JumpPhase samples this action from vertical motion and landing.
"""
import math
import bpy
from mathutils import Vector, Matrix
from refine_gait_v5 import LEGS, _solve, _new_action, _curves, _smooth
from gait_kinematics_v9 import _solve

# phase, body compression, body pitch, front forward, front lift, rear forward, rear lift
# Distances scaled to current limb length so shortened v6 legs retain safe reach.
POSES = [
    (0.00, -.30, -.055, 0.00, 0.00, 0.00, 0.00), # compressed takeoff
    (0.16, -.08, -.045, -.22, .17, .09, .18),     # forepaws reach; hind push/gather
    (0.48, -.11, .035, -.15, .36, -.12, .43),     # hind knees tucked at apex
    (0.76, -.07, .075, -.16, .035, .04, .17),     # forefeet prepare contact
    (0.84, -.34, .040, -.08, 0.00, .02, 0.00),   # landing cushion
    (1.00, -.055, 0.00, 0.00, 0.00, 0.00, 0.00),# restore idle stance
]

def _pose(t):
    for a,b in zip(POSES,POSES[1:]):
        if t <= b[0]:
            u=_smooth(max(0,min(1,(t-a[0])/(b[0]-a[0]))))
            return [x+(y-x)*u for x,y in zip(a[1:],b[1:])]
    return list(POSES[-1][1:])

def refine(rig):
    """Replace only Jump; return joint/contact/mesh metrics without exporting."""
    required={'body','head','tail',*LEGS,*(n+'_shin' for n in LEGS)}
    if not required.issubset(rig.pose.bones.keys()):
        raise ValueError('Jump v6 requires the Jimothy deform rig')
    scene=bpy.context.scene; saved_frame=scene.frame_current
    rig.animation_data_create(); action=_new_action(rig,'Jump')
    feet={n:rig.data.bones[n+'_shin'].tail_local.copy() for n in LEGS}
    scale=sum(rig.data.bones[n].length+rig.data.bones[n+'_shin'].length for n in LEGS)/8
    meshes=[o for o in scene.objects if o.type=='MESH' and any(m.type=='ARMATURE' and m.object==rig for m in o.modifiers)]
    report={'version':9,'frames':61,'loop':False,'limb_scale':scale,'max_ankle_gap':0.,'max_knee_gap':0.,'max_solver_error':0.,'max_target_clamp':0.,'max_paw_orientation_error':0.,'mesh_vertex_count':0,'poses':[]}
    previous={}
    for frame in range(1,62):
        scene.frame_set(frame); t=(frame-1)/60
        for pb in rig.pose.bones:
            pb.rotation_mode='QUATERNION';pb.matrix_basis=Matrix.Identity(4)
        compression,pitch,fy,fz,ry,rz=_pose(t)
        body=rig.pose.bones['body']; basis=body.bone.matrix_local.to_3x3()
        # Idle's .055 root offset is unscaled so the last pose matches existing Idle.
        recover=_smooth(max(0,min(1,(t-.84)/.16)))
        down=compression*scale+(-.055+.055*scale)*recover
        body.location=body.bone.matrix_local.to_quaternion().inverted()@Vector((0,0,down))
        body.rotation_quaternion=(basis.inverted()@Matrix.Rotation(pitch,3,'X')@basis).to_quaternion()
        bpy.context.view_layer.update()
        for i,n in enumerate(LEGS):
            target=feet[n]+Vector((0,(fy if i<2 else ry)*scale,(fz if i<2 else rz)*scale))
            # Explicitly constrain reach to avoid stretching as root geometry changes.
            upper=rig.pose.bones[n]; lower=rig.pose.bones[n+'_shin']
            hip=(body.matrix@body.bone.matrix_local.inverted())@upper.bone.head_local
            delta=target-hip; distance=delta.length
            safe=max(abs(upper.bone.length-lower.bone.length)+.0001,min(upper.bone.length+lower.bone.length-.0001,distance))
            if distance>0:target=hip+delta*(safe/distance)
            report['max_target_clamp']=max(report['max_target_clamp'],abs(distance-safe))
            report['max_solver_error']=max(report['max_solver_error'],_solve(rig,n,target))
            report['max_knee_gap']=max(report['max_knee_gap'],(upper.tail-lower.head).length)
            paw=rig.pose.bones.get(n+'_paw')
            if paw:
                report['max_ankle_gap']=max(report['max_ankle_gap'],(paw.head-lower.tail).length)
                report['max_paw_orientation_error']=max(report['max_paw_orientation_error'],paw.matrix.to_quaternion().rotation_difference(paw.bone.matrix_local.to_quaternion()).angle)
        rig.pose.bones['head'].rotation_quaternion=Matrix.Rotation(-pitch*.55,4,'X').to_quaternion()
        rig.pose.bones['tail'].rotation_quaternion=Matrix.Rotation(-pitch*.65,4,'X').to_quaternion()
        for pb in rig.pose.bones:
            q=pb.rotation_quaternion
            if pb.name in previous and q.dot(previous[pb.name])<0:q.negate()
            previous[pb.name]=q.copy()
            pb.keyframe_insert('rotation_quaternion',frame=frame,group=pb.name)
            pb.keyframe_insert('location',frame=frame,group=pb.name)
        if frame in (1,11,30,47,51,61):
            bpy.context.view_layer.update();deps=bpy.context.evaluated_depsgraph_get();lo=float('inf');hi=-lo;count=0
            for o in meshes:
                evaluated=o.evaluated_get(deps);mesh=evaluated.to_mesh()
                for v in mesh.vertices:
                    p=rig.matrix_world.inverted()@evaluated.matrix_world@v.co
                    if not all(math.isfinite(x) for x in p):raise ValueError('Nonfinite jump mesh vertex')
                    lo=min(lo,p.z);hi=max(hi,p.z);count+=1
                evaluated.to_mesh_clear()
            report['mesh_vertex_count']=count
            report['poses'].append({'frame':frame,'phase':t,'mesh_min_z':lo if count else None,'mesh_max_z':hi if count else None})
    for curve in _curves(action):
        for key in curve.keyframe_points:key.interpolation='LINEAR'
    action['loop']=False;action['jump_v6']=True;action['phase_driven']=True
    action['phase_notes']='0 crouch; .16 reach; .48 apex tuck; .76 descent; .84 cushion; 1 idle'
    rig.animation_data.action=bpy.data.actions.get('Idle',action);scene.frame_set(saved_frame);bpy.context.view_layer.update()
    return report

