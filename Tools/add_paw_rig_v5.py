"""Add four deform paws to an already-open rig; transfer low shin weights.

Call AFTER rest-shape edits/reduction and BEFORE generating fur:
    report = add_paws(rig, skin)  # discovers other meshes bound to this rig
    # Or pass meshes=[skin, claw_mesh, ...] explicitly.
No scene open/save/export. Coordinates and thresholds are armature-local units.
"""
import bpy
from mathutils import Vector

LEGS = ('front_L','front_R','rear_L','rear_R')

def _smooth(t):
    t = max(0.0, min(1.0,t))
    return t*t*(3-2*t)

def add_paws(rig, skin, meshes=None):
    if rig.type != 'ARMATURE' or skin.type != 'MESH':
        raise ValueError('Expected armature rig and skinned mesh')
    for name in LEGS:
        if name+'_shin' not in rig.data.bones:
            raise ValueError('Missing '+name+'_shin')
    selected = list(bpy.context.selected_objects)
    active = bpy.context.view_layer.objects.active
    old_mode = bpy.context.object.mode if bpy.context.object else 'OBJECT'
    if old_mode != 'OBJECT':
        bpy.ops.object.mode_set(mode='OBJECT')
    bpy.ops.object.select_all(action='DESELECT')
    rig.select_set(True)
    bpy.context.view_layer.objects.active = rig
    added = []
    bpy.ops.object.mode_set(mode='EDIT')
    for name in LEGS:
        paw_name = name+'_paw'
        if paw_name in rig.data.edit_bones:
            continue
        shin = rig.data.edit_bones[name+'_shin']
        paw = rig.data.edit_bones.new(paw_name)
        paw.head = shin.tail.copy()
        paw.tail = paw.head+Vector((0,-.18,-.04))
        paw.parent = shin
        paw.use_connect = True
        paw.use_deform = True
        paw.roll = 0
        added.append(paw_name)
    bpy.ops.object.mode_set(mode='OBJECT')
    if meshes is None:
        meshes = [o for o in bpy.data.objects if o.type=='MESH' and
                  any(m.type=='ARMATURE' and m.object==rig for m in o.modifiers)]
    meshes = list(dict.fromkeys([skin,*meshes]))
    report = {'bones_added':added,'meshes':{},'threshold':'full below wrist+.005, fade to zero at wrist+.095'}
    for mesh in meshes:
        if mesh.type != 'MESH':
            continue
        if mesh.get('paw_weights_v5',False):
            report['meshes'][mesh.name] = {'already_done':True}
            continue
        matrix = rig.matrix_world.inverted() @ mesh.matrix_world
        transfers = {name:0 for name in LEGS}
        max_sum_error = 0.0
        # Only shift existing lower-limb influence: preserve all other groups.
        for name in LEGS:
            shin = mesh.vertex_groups.get(name+'_shin')
            if shin is None:
                continue
            paw = mesh.vertex_groups.get(name+'_paw') or mesh.vertex_groups.new(name=name+'_paw')
            wrist_z = rig.data.bones[name+'_shin'].tail_local.z
            for v in mesh.data.vertices:
                weight = next((g.weight for g in v.groups if g.group==shin.index),0.0)
                if weight <= 1e-7:
                    continue
                z = (matrix @ v.co).z
                blend = 1.0-_smooth((z-(wrist_z+.005))/.09)
                if blend <= 1e-7:
                    continue
                amount = weight*blend
                old_paw = next((g.weight for g in v.groups if g.group==paw.index),0.0)
                before = sum(g.weight for g in v.groups)
                paw.add([v.index],old_paw+amount,'REPLACE')
                if weight-amount <= 1e-7:
                    shin.remove([v.index])
                else:
                    shin.add([v.index],weight-amount,'REPLACE')
                max_sum_error = max(max_sum_error,abs(sum(g.weight for g in v.groups)-before))
                transfers[name] += 1
        mesh['paw_weights_v5'] = True
        report['meshes'][mesh.name] = {'transferred_vertices':transfers,'max_weight_sum_error':max_sum_error}
    rig['paw_rig_v5'] = True
    bpy.ops.object.select_all(action='DESELECT')
    for o in selected:
        o.select_set(True)
    bpy.context.view_layer.objects.active = active
    if active and old_mode != 'OBJECT':
        bpy.ops.object.mode_set(mode=old_mode)
    bpy.context.view_layer.update()
    return report
