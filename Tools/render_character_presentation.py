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
s.render.resolution_x=1920;s.render.resolution_y=1920;s.cycles.samples=48;s.render.filepath=str(ROOT/'Art/Renders/jimothy-reference-pose-v4.png')
if '--draft' not in sys.argv:bpy.ops.render.render(write_still=True)
# A physical rooftop AC unit gives the same useful scale cue as the target artwork.
for name in ['Display base']:
 o=bpy.data.objects.get(name)
 if o:bpy.data.objects.remove(o,do_unlink=True)
copper=mat('Weathered rooftop enamel',(.12,.23,.19),.38,.57);iron=mat('Fan grille',(.016,.025,.023),.7,.45);rust=mat('Exposed copper edges',(.32,.15,.063),.5,.6)
nt=copper.node_tree;p=nt.nodes.get('Principled BSDF');n=nt.nodes.new('ShaderNodeTexNoise');n.inputs['Scale'].default_value=5;n.inputs['Detail'].default_value=5;r=nt.nodes.new('ShaderNodeValToRGB');r.color_ramp.elements[0].position=.30;r.color_ramp.elements[0].color=(.075,.036,.017,1);r.color_ramp.elements[1].position=.46;r.color_ramp.elements[1].color=(.11,.22,.17,1);nt.links.new(n.outputs['Fac'],r.inputs[0]);nt.links.new(r.outputs[0],p.inputs['Base Color']);bump=nt.nodes.new('ShaderNodeBump');bump.inputs['Strength'].default_value=.18;bump.inputs['Distance'].default_value=.022;nt.links.new(n.outputs['Fac'],bump.inputs['Height']);nt.links.new(bump.outputs[0],p.inputs['Normal'])
mesh('Rooftop AC casing',(0,.05,-.48),(1.10,1.42,.47),copper,bevel=.045)
mesh('AC top cap',(0,.05,-.014),(1.14,1.46,.025),copper,bevel=.018)
for z in [-.9,-.55,-.2]:mesh('Raised reinforcement',(-1.107,.05,z),(.018,1.38,.025),rust,bevel=.009)
for i in range(19):mesh('Cooling louvre',(-1.128,-1.20+i*.135,-.5),(.026,.024,.25),iron,bevel=.008)
for y in [-1.25,1.35]:
 for z in [-.16,-.80]:mesh('Captive screw',(-1.149,y,z),(.012,.025,.025),iron,'sphere')
for radius in [.14,.25,.36,.47,.55]:
 bpy.ops.mesh.primitive_torus_add(major_radius=radius,minor_radius=.006,major_segments=64,minor_segments=6,location=(.0,.81,.023));bpy.context.object.name='Concentric fan guard';bpy.context.object.data.materials.append(iron)
for i in range(16):
 a=math.tau*i/16;bar('Radial guard',(0,.81,.025),(.56*math.cos(a),.81+.56*math.sin(a),.025),.005,iron)
# Bagel in the mouth, a removable modeled prop.
bread=mat('Toasted bagel',(.43,.18,.045),0,.85);sesame=mat('Sesame seed',(.65,.48,.22),0,.9)
bpy.ops.mesh.primitive_torus_add(major_radius=.135,minor_radius=.040,major_segments=64,minor_segments=16,location=(0,-1.10,1.105),rotation=(math.pi/2,0,0));bagel=bpy.context.object;bagel.name='Removable bagel prop';bagel.data.materials.append(bread)
for v in bagel.data.vertices:v.co*=1+random.uniform(-.018,.018)
for poly in bagel.data.polygons:poly.use_smooth=True
for i in range(60):
 a=random.uniform(0,math.tau);r=random.uniform(.112,.16);o=mesh('Sesame seed',(r*math.cos(a),-1.137,1.105+r*math.sin(a)),(.0025,.003,.006),sesame,'sphere');o.rotation_euler[1]=random.uniform(0,math.pi)
# Original modeled district behind the character, with shallow depth of field.
with bpy.data.libraries.load(str(ROOT/'Art/Blender/OldBallard.blend'),link=False) as (src,dst):dst.objects=[name for name in src.objects if name.startswith('COL_') or name.startswith('Decor_')]
parent=bpy.data.objects.new('Ballard backdrop',None);bpy.context.collection.objects.link(parent);parent.location=(45,18,-11)
for o in dst.objects:
 if o:bpy.context.collection.objects.link(o);o.parent=parent
# Scenic mountain profile, modeled for this presentation (not a geographic survey).
rock=mat('Rainier distant rock',(.30,.38,.43));snow=mat('Rainier summer snow',(.70,.76,.77));verts=[];faces=[]
segments=64
for ring in range(8):
 radius=(1-ring/8)*16;z=(ring/8)**1.4*19
 for j in range(segments):
  a=math.tau*j/segments;rr=radius*(1+.10*math.sin(a*7)+.04*math.sin(a*13));verts.append((75+rr*math.cos(a),70+rr*.65*math.sin(a),-10+z*.68+random.uniform(-.4,.4)))
for i in range(7):
 for j in range(segments):faces.append((i*segments+j,i*segments+(j+1)%segments,(i+1)*segments+(j+1)%segments,(i+1)*segments+j))
verts.append((75,70,4));tip=len(verts)-1
for j in range(segments):faces.append((7*segments+j,7*segments+(j+1)%segments,tip))
d=bpy.data.meshes.new('Mountain ridge');d.from_pydata(verts,[],faces);d.materials.append(rock);d.materials.append(snow);o=bpy.data.objects.new('Scenic Rainier',d);bpy.context.collection.objects.link(o)
for p in d.polygons:p.material_index=1 if p.center.z>5 else 0
# Tiny seaplane silhouette, fully modeled.
plane=bpy.data.objects.new('Seaplane',None);bpy.context.collection.objects.link(plane);plane.location=(25,12,4);plane.rotation_euler.z=-.2
ivory=mat('Floatplane ivory',(.7,.69,.56));red=mat('Floatplane stripe',(.35,.08,.035))
for name,loc,scale in [('Fuselage',(0,0,0),(.13,.60,.14)),('Wing',(0,0,.15),(.85,.16,.025)),('Tail wing',(0,.48,.06),(.31,.10,.02)),('Tail fin',(0,.46,.16),(.03,.16,.18)),('Float left',(-.19,0,-.23),(.08,.56,.07)),('Float right',(.19,0,-.23),(.08,.56,.07))]:
 o=mesh(name,loc,scale,red if name=='Tail fin' else ivory,'sphere' if name in ['Fuselage','Float left','Float right'] else 'cube');o.parent=plane
# Warm backlight with a neutral face light preserves the charcoal coat.
for name,loc,power,col,size in [('Golden hour',(-4,-4,5),650,(1,.91,.81),4),('Sky fill',(2,-4,4),340,(.64,.78,1),4),('Rim',(-2,3,4),1800,(1,.60,.29),3)]:
 o=bpy.data.objects[name];o.location=loc;o.rotation_euler=(Vector((0,0,1.2))-o.location).to_track_quat('-Z','Y').to_euler();o.data.energy=power;o.data.color=col;o.data.size=size
ld=bpy.data.lights.new('Sun over Ballard','SUN');ld.energy=.3;ld.angle=.15;ld.color=(1,.77,.51);lo=bpy.data.objects.new('Sun over Ballard',ld);bpy.context.collection.objects.link(lo);lo.rotation_euler=(.6,-.3,-.5)
w=s.world;w.use_nodes=True;sky=w.node_tree.nodes.new('ShaderNodeTexSky');sky.sky_type='NISHITA';sky.sun_elevation=.13;sky.sun_rotation=2;sky.sun_intensity=.3;sky.dust_density=2;w.node_tree.nodes['Background'].inputs[1].default_value=.025;w.node_tree.links.new(sky.outputs[0],w.node_tree.nodes['Background'].inputs[0])
cam.location=(-7.2,-4.6,1.65);target=Vector((0,0,1.0));rotation=(target-cam.location).to_track_quat('-Z','Y');right=rotation@Vector((1,0,0));target-=right*.72;cam.rotation_euler=(target-cam.location).to_track_quat('-Z','Y').to_euler();cam.data.type='PERSP';cam.data.lens=52
focus=bpy.data.objects.new('Focus on Jimothy',None);bpy.context.collection.objects.link(focus);focus.location=(0,-.25,1.25);cam.data.dof.use_dof=True;cam.data.dof.focus_object=focus;cam.data.dof.aperture_fstop=1.4
s.render.resolution_x=3840;s.render.resolution_y=2160;s.cycles.samples=64;s.render.filepath=str(ROOT/'Art/Renders/jimothy-rooftop-4k.png')
if '--draft' in sys.argv:
 s.render.resolution_x=1280;s.render.resolution_y=720;s.cycles.samples=12;s.render.filepath=str(ROOT/'Art/Renders/jimothy-rooftop-draft.png')
else:bpy.ops.wm.save_as_mainfile(filepath=str(ROOT/'Art/Blender/Jimothy-Presentation.blend'),compress=True)
bpy.ops.render.render(write_still=True)
print('PRESENTATION_RENDER_COMPLETE')
