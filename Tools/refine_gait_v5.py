"""Bake reference-informed in-place gait onto an already-open Jimothy_Rig.

Usage: import refine_gait_v5; report = refine_gait_v5.refine(rig)
Never opens/saves a blend or exports. Existing action IDs are remapped safely.
Forward is armature -Y; Z up. No helper bones or runtime IK required.
See Documentation/GAIT-STUDY.md for observation limits and speed matching.
"""
import math
import bpy
from mathutils import Vector, Matrix

LEGS = ('front_L', 'front_R', 'rear_L', 'rear_R')

def _smooth(t):
    return t*t*(3-2*t)

def _trajectory(phase, duty, stride, lift):
    """Constant stance velocity, zero-height stance; smooth lifted recovery."""
    phase %= 1.0
    if phase < duty:
        return -stride/2 + stride*phase/duty, 0.0
    t = (phase-duty)/(1-duty)
    # Cubic Hermite: end velocities match stance, avoiding a contact snap.
    m = stride*(1-duty)/duty
    y = (2*t**3-3*t*t+1)*stride/2 + (t**3-2*t*t+t)*m
    y += (-2*t**3+3*t*t)*(-stride/2) + (t**3-t*t)*m
    return y, lift*math.sin(math.pi*t)**2

def _aim(pb, head, tail):
    rest = pb.bone
    q = (rest.tail_local-rest.head_local).rotation_difference(tail-head)
    m = (q.to_matrix() @ rest.matrix_local.to_3x3()).to_4x4()
    m.translation = head
    pb.matrix = m

def _solve(rig, name, target):
    """Analytic two-link IK in armature space, using rest elbow as pole."""
    upper, lower = rig.pose.bones[name], rig.pose.bones[name+'_shin']
    body = rig.pose.bones['body']
    parent_delta = body.matrix @ body.bone.matrix_local.inverted()
    hip = parent_delta @ upper.bone.head_local
    rest_knee = parent_delta @ upper.bone.tail_local
    a, b = upper.bone.length, lower.bone.length
    toward = target-hip
    distance = toward.length
    direction = toward.normalized()
    # No limb stretching: lower the body / shorten stride if clamping is reported.
    d = max(abs(a-b)+1e-5, min(a+b-1e-5, distance))
    along = (a*a-b*b+d*d)/(2*d)
    height = math.sqrt(max(0.0, a*a-along*along))
    rest_end = parent_delta @ lower.bone.tail_local
    rest_axis = (rest_end-hip).normalized()
    pole = rest_knee-hip
    pole -= rest_axis*pole.dot(rest_axis)
    pole -= direction*pole.dot(direction)
    if pole.length < 1e-5:
        pole = Vector((0, 1 if name.startswith('front') else -1, 0))
        pole -= direction*pole.dot(direction)
    knee = hip + direction*along + pole.normalized()*height
    end = hip + direction*d
    _aim(upper, hip, knee)
    bpy.context.view_layer.update()
    _aim(lower, knee, end)
    bpy.context.view_layer.update()
    paw = rig.pose.bones.get(name+'_paw')
    if paw is not None:
        # Keep authored sole orientation in armature space, independently of shin.
        # Translation follows actual ankle, so connected joint cannot separate.
        m = paw.bone.matrix_local.copy()
        m.translation = lower.tail
        paw.matrix = m
        bpy.context.view_layer.update()
    return (lower.tail-target).length

def _new_action(rig, name):
    old = bpy.data.actions.get(name)
    if old:
        old.name = name+'__before_gait_v5'
    action = bpy.data.actions.new(name)
    rig.animation_data.action = action
    action.use_fake_user = True
    # Preserve external action references, e.g. NLA strips, without duplicates.
    if old:
        old.user_remap(action)
        bpy.data.actions.remove(old)
    return action

def _curves(action):
    # Blender 4.5 layered actions plus legacy actions.
    try:
        yield from action.fcurves
        return
    except AttributeError:
        pass
    for layer in action.layers:
        for strip in layer.strips:
            for bag in strip.channelbags:
                yield from bag.fcurves

def refine(rig):
    """Replace Idle/Walk/Waddle/Jump in memory; return numeric validation report."""
    required = {'body', 'head', 'tail', *LEGS, *(n+'_shin' for n in LEGS)}
    missing = required-set(rig.pose.bones.keys())
    if missing:
        raise ValueError('Unsupported Jimothy rig; missing '+', '.join(sorted(missing)))
    if any(b.constraints for b in rig.pose.bones):
        raise ValueError('Expected unconstrained deform rig; do not double-apply IK')
    rig.animation_data_create()
    scene = bpy.context.scene
    original_frame = scene.frame_current
    specs = {
        'Idle': dict(frames=61, stride=0, duty=1, lift=0),
        'Walk': dict(frames=17, stride=1.35, duty=.60, lift=.20),
        'Waddle': dict(frames=9, stride=1.30, duty=.40, lift=.32),
        'Jump': dict(frames=31, stride=0, duty=1, lift=0),
    }
    rest_feet = {n: rig.data.bones[n+'_shin'].tail_local.copy() for n in LEGS}
    report = {'version':5, 'bones_added':0, 'articulated_paws':sum(n+'_paw' in rig.pose.bones for n in LEGS), 'clips':{}, 'max_wrist_error':0.0}
    previous_quats = {}
    for clip, spec in specs.items():
        action = _new_action(rig, clip)
        previous_quats.clear()
        errors = []
        # Dense keys preserve analytic paw plants between the short run cycle's frames.
        # Fractional frames retain existing clip duration and nominal speeds.
        for sample in range((spec['frames']-1)*4+1):
            frame = 1+sample/4
            scene.frame_set(int(frame),subframe=frame-int(frame))
            t = (frame-1)/(spec['frames']-1)
            ph = math.tau*t
            for pb in rig.pose.bones:
                pb.rotation_mode = 'QUATERNION'
                pb.matrix_basis = Matrix.Identity(4)
            body = rig.pose.bones['body']
            if clip in ('Walk', 'Waddle'):
                fast = clip == 'Waddle'
                # Torso stays a substantial stable mass over separate contacts.
                bob = (.035 if fast else .014)*math.sin(ph*2+.4)
                roll = (.028 if fast else .022)*math.sin(ph+.2)
                pitch = (.040 if fast else .014)*math.sin(ph-.5)
                down = -.32 if fast else -.29
                world_shift = Vector((.013*math.sin(ph), 0, down+bob))
                body.location = body.bone.matrix_local.to_quaternion().inverted() @ world_shift
                world_rot = Matrix.Rotation(pitch,4,'X') @ Matrix.Rotation(roll,4,'Y')
                basis = body.bone.matrix_local.to_3x3()
                body.rotation_quaternion = (basis.inverted() @ world_rot.to_3x3() @ basis).to_quaternion()
                bpy.context.view_layer.update()
                # Walk: staggered individual contacts; run: front pair then rear,
                # with a slight lead rather than simultaneous mirrored pumping.
                offsets = (0,.10,.51,.63) if fast else (0,.49,.70,.21)
                for i, name in enumerate(LEGS):
                    y,z = _trajectory(t-offsets[i],spec['duty'],spec['stride'],spec['lift'])
                    target = rest_feet[name].copy()
                    target.y = rig.data.bones[name].head_local.y + y
                    target.z += z
                    # Small outward recovery only; planted wrists stay on their lane.
                    target.x += (-1 if name.endswith('_L') else 1)*z*.12
                    errors.append(_solve(rig,name,target))
                rig.pose.bones['head'].rotation_quaternion = Matrix.Rotation(-pitch*.55,4,'X').to_quaternion()
                rig.pose.bones['tail'].rotation_quaternion = Matrix.Rotation(-roll*.45,4,'Z').to_quaternion()
            elif clip == 'Idle':
                body.location = body.bone.matrix_local.to_quaternion().inverted() @ Vector((0,0,-.055+.006*math.sin(ph)))
                bpy.context.view_layer.update()
                for name in LEGS:
                    errors.append(_solve(rig,name,rest_feet[name]))
                rig.pose.bones['head'].rotation_quaternion = Matrix.Rotation(.018*math.sin(ph),4,'X').to_quaternion()
            else:
                # Airborne clip only. Motor supplies ballistic translation, not root.
                gather = math.sin(math.pi*t)**2
                body.rotation_quaternion = Matrix.Rotation(.07*math.sin(ph),4,'X').to_quaternion()
                bpy.context.view_layer.update()
                for i,name in enumerate(LEGS):
                    target = rest_feet[name].copy()
                    target.z += (.31 if i<2 else .41)*gather
                    target.y += (-.10 if i<2 else -.18)*gather
                    errors.append(_solve(rig,name,target))
            for pb in rig.pose.bones:
                # q and -q represent same orientation; keep continuous FBX curves.
                q = pb.rotation_quaternion
                prev = previous_quats.get(pb.name)
                if prev is not None and q.dot(prev)<0:
                    q.negate()
                previous_quats[pb.name] = q.copy()
                pb.keyframe_insert('rotation_quaternion',frame=frame,group=pb.name)
                pb.keyframe_insert('location',frame=frame,group=pb.name)
            action['gait_v5'] = True
        for fc in _curves(action):
            for key in fc.keyframe_points:
                key.interpolation = 'LINEAR'  # every frame baked; no spline foot overshoot
        duration = (spec['frames']-1)/scene.render.fps
        nominal_speed = spec['stride']/spec['duty']/duration if spec['stride'] else 0.0
        action['nominal_forward_speed_armature_units_per_second'] = nominal_speed
        action['loop'] = clip != 'Jump'
        report['clips'][clip] = {'frames':spec['frames'], 'samples':(spec['frames']-1)*4+1, 'duration_seconds':duration,
            'nominal_forward_speed':nominal_speed, 'max_wrist_error':max(errors,default=0)}
        report['max_wrist_error'] = max(report['max_wrist_error'],max(errors,default=0))
    rig.animation_data.action = bpy.data.actions['Idle']
    scene.frame_set(original_frame)
    bpy.context.view_layer.update()
    return report
