import math
import bpy
from mathutils import Vector
from refine_gait_v5 import _aim
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
    # Anatomical sagittal bend: fore elbows back, hind knees forward, not sideways.
    pole=Vector((0,1 if name.startswith('front') else -1,-.10))
    pole-=direction*pole.dot(direction)
    if pole.length<1e-5:pole=Vector((0,0,1))-direction*direction.z
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
