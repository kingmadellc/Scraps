"""Waddle-only run correction from inspected Kiana Hall / Seattle Times moving footage.
Call refine(rig) after rest-shape changes and existing gait initialization. No file I/O.
The partly hidden far-side contacts are an authored approximation, not motion capture.
"""
import math
import bpy
from mathutils import Vector, Matrix
from refine_gait_v5 import LEGS,_solve,_new_action,_curves,_smooth
from gait_kinematics_v9 import _solve

# Forefeet catch separately, followed by a close hind pair; ample gathered flight.
CONTACTS=(0.,.09,.46,.52)
DUTIES=(.22,.22,.22,.22)
FRAMES=13
NOMINAL=9.75  # Preserve runtime speed denominator at .22 scale: 2.145m/s.

# phase, down, pitch; a quick catch/compression and slower hindquarter recovery.
BODY=((0.,-.24,.065),(.13,-.28,.095),(.32,-.25,.04),(.51,-.22,-.025),(.68,-.20,-.035),(.84,-.22,.018),(1.,-.24,.065))
def body_pose(t):
    for a,b in zip(BODY,BODY[1:]):
        if t<=b[0]:
            u=_smooth((t-a[0])/(b[0]-a[0]));return a[1]+(b[1]-a[1])*u,a[2]+(b[2]-a[2])*u
    return BODY[-1][1:]

def trajectory(phase,duty,stride,front):
    phase%=1
    if phase<duty:return -stride*.5+stride*phase/duty,0.,0.
    u=(phase-duty)/(1-duty)
    # Matched stance velocity at release/contact, with conspicuous late forward reach.
    # Uneven recovery replaces the old symmetric sinusoidal pumping.
    m=stride*(1-duty)/duty
    knots=((0.,stride*.5,m),(.22,stride*.5+.04,0.),(.78,-stride*.5-.025,0.),(1.,-stride*.5,m))
    for a,b in zip(knots,knots[1:]):
        if u<=b[0]:
            span=b[0]-a[0];q=(u-a[0])/span
            y=(2*q**3-3*q*q+1)*a[1]+(q**3-2*q*q+q)*span*a[2]+(-2*q**3+3*q*q)*b[1]+(q**3-q*q)*span*b[2]
            break
    lift=(.18 if front else .29)*math.sin(math.pi*u)**1.35
    # Paw points downward briefly during the rearward kick then levels before catch.
    curl=(.12 if front else .22)*math.sin(math.pi*u)**2
    return y,lift,curl

def refine(rig):
    rig.animation_data_create();scene=bpy.context.scene;original=scene.frame_current
    action=_new_action(rig,'Waddle');rest={n:rig.data.bones[n+'_shin'].tail_local.copy() for n in LEGS}
    duration=(FRAMES-1)/scene.render.fps
    report={'version':9,'frames':FRAMES,'duration_seconds':duration,'nominal_armature_speed':NOMINAL,'nominal_world_speed_at_scale_022':NOMINAL*.22,'max_solver_error':0.,'max_reach_clamp':0.,'max_joint_gap':0.,'samples':0,'source':'Kiana Hall footage, Seattle Times RqlQYJUz-j8 opening run; see GAIT-STUDY-V7.md'}
    previous={}
    for sample in range((FRAMES-1)*8+1):
        frame=1+sample/8;t=(frame-1)/(FRAMES-1);scene.frame_set(int(frame),subframe=frame-int(frame))
        for pb in rig.pose.bones:pb.rotation_mode='QUATERNION';pb.matrix_basis=Matrix.Identity(4)
        down,pitch=body_pose(t);body=rig.pose.bones['body'];basis=body.bone.matrix_local.to_3x3()
        body.location=body.bone.matrix_local.to_quaternion().inverted()@Vector((.016*math.sin(math.tau*(t-.08)),0,down))
        rot=Matrix.Rotation(pitch,3,'X')@Matrix.Rotation(.025*math.sin(math.tau*t),3,'Y')
        body.rotation_quaternion=(basis.inverted()@rot@basis).to_quaternion();bpy.context.view_layer.update()
        for i,n in enumerate(LEGS):
            y,z,curl=trajectory(t-CONTACTS[i],DUTIES[i],NOMINAL*duration*DUTIES[i],i<2)
            target=rest[n].copy();target.y=rig.data.bones[n].head_local.y+y+(-.035 if i<2 else .015);target.z+=z
            target.x+=(-1 if n.endswith('_L') else 1)*z*(.025 if i<2 else .035)
            upper=rig.pose.bones[n];lower=rig.pose.bones[n+'_shin'];hip=(body.matrix@body.bone.matrix_local.inverted())@upper.bone.head_local
            delta=target-hip;distance=delta.length;safe=max(abs(upper.bone.length-lower.bone.length)+.0001,min(upper.bone.length+lower.bone.length-.0001,distance))
            report['max_reach_clamp']=max(report['max_reach_clamp'],abs(distance-safe))
            if distance>0:target=hip+delta*(safe/distance)
            report['max_solver_error']=max(report['max_solver_error'],_solve(rig,n,target))
            paw=rig.pose.bones.get(n+'_paw')
            if paw:
                # Rotation about ankle only: no sliding the connected paw root.
                m=(Matrix.Rotation(curl,4,'X')@paw.bone.matrix_local);m.translation=lower.tail;paw.matrix=m;bpy.context.view_layer.update()
                report['max_joint_gap']=max(report['max_joint_gap'],(paw.head-lower.tail).length)
            report['max_joint_gap']=max(report['max_joint_gap'],(upper.tail-lower.head).length)
        rig.pose.bones['head'].rotation_quaternion=Matrix.Rotation(-pitch*.55,4,'X').to_quaternion()
        rig.pose.bones['tail'].rotation_quaternion=Matrix.Rotation(.05*math.sin(math.tau*(t-.1)),4,'X').to_quaternion()
        for pb in rig.pose.bones:
            q=pb.rotation_quaternion
            if pb.name in previous and q.dot(previous[pb.name])<0:q.negate()
            previous[pb.name]=q.copy();pb.keyframe_insert('rotation_quaternion',frame=frame,group=pb.name);pb.keyframe_insert('location',frame=frame,group=pb.name)
        report['samples']+=1
    for fc in _curves(action):
        for key in fc.keyframe_points:key.interpolation='LINEAR'
    action['loop']=True;action['gait_v9']=True;action['nominal_forward_speed_armature_units_per_second']=NOMINAL
    rig.animation_data.action=bpy.data.actions.get('Idle');scene.frame_set(original);bpy.context.view_layer.update();return report

