"""Corrected Jimothy v2 from the user's primary photograph. Blender 4.5."""
import sys
from pathlib import Path
sys.path.insert(0,str(Path(__file__).resolve().parent))
from blender_common import *
reset()
fur=mat('Charcoal silver grizzled coat',(.045,.058,.068),0,.88)
cream=mat('Silver cheek fur',(.34,.37,.36),0,.8)
mask=mat('Natural black eye mask',(.009,.013,.016),0,.75)
black=mat('Leathery nose and fingers',(.012,.016,.018),0,.35)
eye=mat('Dark brown eyes',(.022,.013,.008),0,.12)
# Coat variation and directional surface fibers, retained in the Blender source.
for m in [fur,cream]:
 nt=m.node_tree;p=nt.nodes.get('Principled BSDF');tex=nt.nodes.new('ShaderNodeTexNoise');tex.inputs['Scale'].default_value=220;tex.inputs['Detail'].default_value=3
 ramp=nt.nodes.new('ShaderNodeValToRGB');base=m.diffuse_color
 ramp.color_ramp.elements[0].position=.25;ramp.color_ramp.elements[0].color=(base[0]*.3,base[1]*.3,base[2]*.3,1)
 ramp.color_ramp.elements[1].position=.73;ramp.color_ramp.elements[1].color=(base[0]*1.9,base[1]*1.9,base[2]*1.9,1)
 nt.links.new(tex.outputs['Fac'],ramp.inputs[0]);nt.links.new(ramp.outputs['Color'],p.inputs['Base Color'])
 bump=nt.nodes.new('ShaderNodeBump');bump.inputs['Strength'].default_value=.10;bump.inputs['Distance'].default_value=.014;nt.links.new(tex.outputs['Fac'],bump.inputs['Height']);nt.links.new(bump.outputs[0],p.inputs['Normal'])
parts=[]
def part(name,loc,scale,m,bone='body',hair=False):
 o=mesh(name,loc,scale,m,'sphere');g=o.vertex_groups.new(name=bone);g.add(list(range(len(o.data.vertices))),1,'REPLACE')
 if hair:
  vg=o.vertex_groups.new(name='FurDensity');vg.add(list(range(len(o.data.vertices))),1,'REPLACE')
 parts.append(o);return o
def limb(name,a,b,r1,r2,bone,m=fur,hair=True):
 # Rounded tapered limb with joint-aligned ellipsoid; slender rather than plush.
 a,b=Vector(a),Vector(b);o=part(name,(a+b)/2,(r1,r2,(b-a).length*.56),m,bone,hair);o.rotation_euler=(b-a).to_track_quat('Z','Y').to_euler();return o
part('High domed hunchback',(0,.10,1.96),(.56,.57,.73),fur,hair=True)
part('Raised shoulders',(0,-.22,2.09),(.51,.41,.55),fur,hair=True)
part('Tucked rump',(0,.44,1.82),(.45,.32,.51),fur,hair=True)
part('Under chest',(0,-.36,1.61),(.33,.19,.34),cream,hair=True)
part('Low forward head',(0,-.60,1.91),(.34,.30,.35),fur,'head',True)
for side in [-1,1]:
 part('Flat cheek',(.235*side,-.69,1.79),(.16,.19,.19),cream,'head',True)
 part('Mask',(.195*side,-.846,1.935),(.146,.050,.092),mask,'head').rotation_euler[1]=-.30*side
 part('Eye',(.18*side,-.890,1.958),(.036,.022,.038),eye,'head')
 part('Brow silver flash',(.18*side,-.823,2.035),(.13,.018,.018),cream,'head')
 part('Ear',(.27*side,-.50,2.205),(.092,.064,.135),cream,'head',True)
 part('Ear inset',(.27*side,-.558,2.212),(.067,.012,.10),mask,'head')
 # Long awkward limbs dominate the clearance beneath the high body.
 for label,y,hipz,elbow_y,foot_y in [('front',-.36,1.65,-.48,-.57),('rear',.39,1.61,.61,.82)]:
  bone=label+('_L' if side<0 else '_R');lower=bone+'_shin';x=side*(.34 if label=='front' else .36)
  limb('Upper '+bone,(x,y,hipz),(x*1.08,elbow_y,.85),.115,.12,bone)
  limb('Long skinny '+lower,(x*1.08,elbow_y,.85),(x*1.12,foot_y,.17),.065,.075,lower)
  part('Small handlike paw',(x*1.12,foot_y-.065,.11),(.090,.14,.065),black,lower)
  for finger in range(4):
   part('Long toe',(x*1.12+(finger-1.5)*.037,foot_y-.16,.095),(.019,.074,.022),black,lower)
# Pointed muzzle projects below the mask; no inflated plush muzzle lobes.
part('Tapered muzzle',(0,-.878,1.78),(.135,.215,.11),cream,'head')
part('Small black nose',(0,-1.066,1.79),(.072,.045,.041),black,'head')
part('Lower lip',(0,-.984,1.716),(.076,.07,.016),mask,'head')
for i in range(6):
 part('Short fluffy tail ring %02d'%i,(.03,.70+i*.09,1.74-i*.05),(.16-i*.014,.15,.15-i*.01),fur if i%2==0 else mask,'tail',True)
# Whiskers are sparse tapered mesh rods, exportable to the game.
for side in [-1,1]:
 for i in range(3):
  o=bar('Whisker',(side*.10,-.975,1.77-i*.02),(side*(.35+i*.03),-1.0,1.77+(i-1)*.055),.002,cream)
  vg=o.vertex_groups.new(name='head');vg.add(list(range(len(o.data.vertices))),1,'REPLACE');parts.append(o)
# Fuse same-material forms by anatomical region to remove construction seams.
for bone,material in [('body',fur),('head',fur),('head',cream)]:
 candidates=[o for o in parts if o.data.materials[0]==material and bone in o.vertex_groups and not o.name.startswith('Whisker')]
 if len(candidates)<2:continue
 bpy.ops.object.select_all(action='DESELECT')
 for o in candidates:o.select_set(True);parts.remove(o)
 bpy.context.view_layer.objects.active=candidates[0];bpy.ops.object.join();o=bpy.context.object
 rem=o.modifiers.new('Unified organic silhouette','REMESH');rem.mode='VOXEL';rem.voxel_size=.021;rem.use_smooth_shade=True;bpy.ops.object.modifier_apply(modifier=rem.name)
 sm=o.modifiers.new('Soften sculpt transitions','SMOOTH');sm.factor=1.1;sm.iterations=5;bpy.ops.object.modifier_apply(modifier=sm.name)
 for g in list(o.vertex_groups):o.vertex_groups.remove(g)
 for group in [bone,'FurDensity']:
  vg=o.vertex_groups.new(name=group);vg.add(list(range(len(o.data.vertices))),1,'REPLACE')
 dec=o.modifiers.new('Game topology reduction','DECIMATE');dec.ratio=.42;bpy.ops.object.modifier_apply(modifier=dec.name)
 parts.append(o)
# An articulated rig with individual shin bones makes the gangly stride possible.
bpy.ops.object.armature_add();rig=bpy.context.object;rig.name='Jimothy_Rig';bpy.ops.object.mode_set(mode='EDIT');rig.data.edit_bones.remove(rig.data.edit_bones[0])
bones=[('body',(0,.1,1.4),(0,.1,2.3),None),('head',(0,-.48,1.88),(0,-.72,2.12),'body'),('tail',(0,.65,1.75),(0,1.22,1.5),'body')]
for label,y,hipz,ey,fy in [('front',-.36,1.65,-.48,-.57),('rear',.39,1.61,.61,.82)]:
 for suffix,side in [('_L',-1),('_R',1)]:
  x=side*(.34 if label=='front' else .36);n=label+suffix;bones += [(n,(x,y,hipz),(x*1.08,ey,.85),'body'),(n+'_shin',(x*1.08,ey,.85),(x*1.12,fy,.17),n)]
for name,head,tail,parent in bones:
 b=rig.data.edit_bones.new(name);b.head=head;b.tail=tail
 if parent:b.parent=rig.data.edit_bones[parent]
bpy.ops.object.mode_set(mode='OBJECT');bpy.ops.object.select_all(action='DESELECT')
for o in parts:o.select_set(True)
bpy.context.view_layer.objects.active=parts[0];bpy.ops.object.join();character=bpy.context.object;character.name='Jimothy_Gangly_SkinnedMesh';mod=character.modifiers.new('11 bone awkward stride rig','ARMATURE');mod.object=rig;character.parent=rig
# Exact loop endpoints. Opposite feet cycle with deliberately unequal extension and toe drag.
for name,length in [('Idle',61),('Waddle',25),('Jump',31)]:
 rig.animation_data_create();act=bpy.data.actions.new(name);rig.animation_data.action=act
 for f in range(1,length+1):
  phase=(f-1)/(length-1)*math.tau
  for b in rig.pose.bones:b.rotation_mode='XYZ';b.rotation_euler=(0,0,0);b.location=(0,0,0)
  root=rig.pose.bones['body']
  if name=='Waddle':
   root.location.z=.045*math.sin(phase*2);root.rotation_euler[1]=.095*math.sin(phase);root.rotation_euler[0]=.05*math.cos(phase*2)
   for i,n in enumerate(['front_L','front_R','rear_L','rear_R']):
    p=phase+[0,math.pi,math.pi+.35,.35][i];stride=math.sin(p)
    rig.pose.bones[n].rotation_euler[0]=stride*(.48 if i<2 else .39)
    rig.pose.bones[n].rotation_euler[2]=.07*math.cos(p)*(1 if i%2 else -1)
    rig.pose.bones[n+'_shin'].rotation_euler[0]=max(0,stride)*(.65 if i<2 else -.38)
   rig.pose.bones['head'].rotation_euler[0]=-.08*math.cos(phase*2);rig.pose.bones['tail'].rotation_euler[1]=.15*math.sin(phase)
  elif name=='Idle':root.location.z=.018*math.sin(phase);rig.pose.bones['head'].rotation_euler[1]=.025*math.sin(phase)
  else:
   root.location.z=.13*math.sin(phase/2)
   for n in ['front_L','front_R','rear_L','rear_R']:rig.pose.bones[n].rotation_euler[0]=-.3*math.sin(phase/2);rig.pose.bones[n+'_shin'].rotation_euler[0]=.6*math.sin(phase/2)
  for b in rig.pose.bones:b.keyframe_insert('rotation_euler',frame=f);b.keyframe_insert('location',frame=f)
 act.use_fake_user=True
rig.animation_data.action=bpy.data.actions['Idle'];bpy.context.scene.frame_set(1)
bpy.ops.object.select_all(action='DESELECT');character.select_set(True);rig.select_set(True);export('Jimothy',True,True)
# Render-only groom. Game export deliberately excludes costly hair, requiring a later fur-card pass.
bpy.context.view_layer.objects.active=character;rig.select_set(False);bpy.ops.object.particle_system_add();ps=character.particle_systems[-1];ps.settings.type='HAIR';ps.settings.count=18000;ps.settings.hair_length=.065;ps.settings.hair_step=3;ps.settings.child_type='INTERPOLATED';ps.settings.child_percent=3;ps.settings.rendered_child_count=12;ps.settings.root_radius=.003;ps.settings.tip_radius=.0005;ps.settings.radius_scale=.32;ps.vertex_group_density='FurDensity'
tri=sum(len(p.vertices)-2 for p in character.data.polygons)
(ROOT/'Art/Blender/asset-report.json').write_text(json.dumps({'character_triangles':tri,'bones':len(rig.data.bones),'clips':['Idle','Waddle','Jump'],'reference':'Art/References/jimothy-user-primary.png','notes':'Corrected long limbs, high hunchback, low head, small eyes, short tail. Blender groom is render-only; FBX uses shaded surface. Prototype segmented skinning needs deformation polish.'},indent=2))
plinth=mat('Den green',(.025,.075,.065));mesh('Display plinth',(0,0,-.06),(2,2,.055),plinth,'cylinder',.03)
stage((4.3,-5.5,3),(0,-.02,1.34),3.9)
# Lower studio energy maintains charcoal coat instead of blowing it out.
for o in bpy.data.objects:
 if o.type=='LIGHT':o.data.energy*=.55
render('jimothy-model-v2.png',1920,1920)
# True side view is the silhouette approval image.
cam=bpy.context.scene.camera;cam.location=(5,-.05,2.4);cam.rotation_euler=(Vector((0,0,1.35))-cam.location).to_track_quat('-Z','Y').to_euler();bpy.context.scene.render.resolution_x=1920;bpy.context.scene.render.resolution_y=1920;bpy.context.scene.render.filepath=str(ROOT/'Art/Renders/jimothy-side-v2.png');bpy.ops.render.render(write_still=True)
# Small loop frames to inspect actual skeleton motion.
rig.animation_data.action=bpy.data.actions['Waddle'];cam.location=(4.3,-5.5,3);cam.rotation_euler=(Vector((0,-.02,1.34))-cam.location).to_track_quat('-Z','Y').to_euler();bpy.context.scene.render.resolution_x=640;bpy.context.scene.render.resolution_y=640;bpy.context.scene.cycles.samples=8
for f in [1,5,9,13,17,21]:
 bpy.context.scene.frame_set(f);bpy.context.scene.render.filepath=str(ROOT/'Art/Renders'/('gait-%02d.png'%f));bpy.ops.render.render(write_still=True)
rig.animation_data.action=bpy.data.actions['Idle'];bpy.context.scene.frame_set(1);bpy.ops.wm.save_as_mainfile(filepath=str(ROOT/'Art/Blender/Jimothy.blend'));print('CORRECTED_JIMOTHY_COMPLETE')
