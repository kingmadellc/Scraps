"""Continuous sculpt and directional groom from the three user references.
Blender 4.5+: blender -b --python Tools/build_jimothy.py
"""
import sys, bisect
from pathlib import Path
sys.path.insert(0,str(Path(__file__).resolve().parent))
from blender_common import *
reset();random.seed(172)
coat=mat('Jimothy painted coat',(.05,.06,.07),0,.84)
nosemat=mat('Soft black nose',(.009,.012,.015),0,.4)
eyemat=mat('Natural dark eyes',(.012,.008,.004),0,.15)
forms=[]
def sculpt(name,center,radii):
 o=mesh(name,center,radii,coat,'sphere');forms.append(o);return o
def segment(name,a,b,r1,r2):
 a,b=Vector(a),Vector(b);o=sculpt(name,(a+b)/2,(r1,r2,(b-a).length*.58));o.rotation_euler=(b-a).to_track_quat('Z','Y').to_euler();return o
# A wide compact pear/egg, with a rounded shoulder line rather than an upright mound.
sculpt('Compressed broad back',(0,.05,1.56),(.59,.72,.60))
sculpt('Shoulder mantle',(0,-.30,1.51),(.49,.40,.43))
sculpt('Round haunch',(0,.51,1.38),(.44,.29,.41))
sculpt('Low chest',(0,-.26,1.14),(.35,.32,.32))
# Face grows out of the leading edge. No stuck-on cheeks or black eye sockets.
sculpt('Integrated head',(0,-.60,1.46),(.30,.285,.32))
# Four cross-sections make a tapered, downward-pointing raccoon muzzle.
verts=[];faces=[]
sections=[(-.68,1.385,.22,.135),(-.82,1.34,.155,.11),(-.95,1.285,.079,.060),(-1.035,1.28,.048,.031)]
for y,z,rx,rz in sections:
 for j in range(24):
  a=math.tau*j/24;verts.append((rx*math.cos(a),y,z+rz*math.sin(a)))
for i in range(len(sections)-1):
 for j in range(24):
  a=i*24+j;b=i*24+(j+1)%24;faces.append((a,b,b+24,a+24))
faces.extend([tuple(reversed(range(24))),tuple(range(72,96))])
d=bpy.data.meshes.new('Muzzle wedge');d.from_pydata(verts,[],faces);d.update();o=bpy.data.objects.new('Sculpted tapering muzzle',d);bpy.context.collection.objects.link(o);d.materials.append(coat);forms.append(o)
leg_points={}
for side,suffix in [(-1,'_L'),(1,'_R')]:
 for kind,hip,elbow,wrist in [
  ('front',(side*.37,-.38,1.32),(side*.405,-.49,.72),(side*.43,-.72,.115)),
  ('rear',(side*.405,.34,1.24),(side*.46,.48,.67),(side*.50,.73,.105))]:
  name=kind+suffix;leg_points[name]=(hip,elbow,wrist)
  segment(name+' upper',hip,elbow,.102 if kind=='front' else .13,.105)
  segment(name+' forearm',elbow,wrist,.058,.060)
  sculpt(name+' wrist',wrist,(.06,.065,.055))
  sculpt(name+' hand',(wrist[0],wrist[1]-.065,.065),(.073,.082,.032))
  for i in range(4):
   xx=wrist[0]+(i-1.5)*.043
   segment(name+' finger',(xx,wrist[1]-.075,.061),(xx,wrist[1]-.235+abs(i-1.5)*.030,.035),.013,.016)
  segment(name+' thumb',(wrist[0]-side*.048,wrist[1]-.042,.065),(wrist[0]-side*.115,wrist[1]-.11,.047),.017,.018)
# Short inconspicuous tail, close to the rump as seen in the references.
segment('Short tail',(0,.61,1.32),(.04,1.02,1.24),.16,.16)
sculpt('Tail tip',(.04,1.06,1.235),(.12,.15,.11))
# Fuse to one continuous surface, then reduce while retaining organic forms.
bpy.ops.object.select_all(action='DESELECT')
for o in forms:o.select_set(True)
bpy.context.view_layer.objects.active=forms[0];bpy.ops.object.join();skin=bpy.context.object;skin.name='Jimothy_ContinuousSkin'
bpy.ops.object.transform_apply(location=True,rotation=True,scale=True)
rem=skin.modifiers.new('Continuous sculpt','REMESH');rem.mode='VOXEL';rem.voxel_size=.010;rem.use_smooth_shade=True;bpy.ops.object.modifier_apply(modifier=rem.name)
sm=skin.modifiers.new('Organic joins','SMOOTH');sm.factor=.8;sm.iterations=4;bpy.ops.object.modifier_apply(modifier=sm.name)
dec=skin.modifiers.new('Production source reduction','DECIMATE');dec.ratio=.21;bpy.ops.object.modifier_apply(modifier=dec.name)
for p in skin.data.polygons:p.use_smooth=True
# Embedded painted markings: charcoal mask, soft silver brow/cheeks and dark hands.
def smooth(a,b,v):
 t=max(0,min(1,(v-a)/(b-a)));return t*t*(3-2*t)
def mix(a,b,t):return tuple(a[i]*(1-t)+b[i]*t for i in range(3))
def color_at(v):
 x,y,z=v;ax=abs(x);noise=.5+.5*math.sin(x*37+math.sin(y*23)*2+z*27)
 c=mix((.016,.019,.023),(.039,.044,.049),noise*.6)
 face=(1-smooth(-.67,-.52,y))*smooth(1.07,1.20,z)
 cheek=face*(1-smooth(1.43,1.54,z));c=mix(c,(.23,.25,.245),cheek)
 # Broad black mask sweeps down the cheek; brow is a broken silver arch.
 line=1.515-.30*(ax-.16)
 band=face*math.exp(-((z-line)/.087)**4)*smooth(.045,.125,ax)
 c=mix(c,(.003,.005,.008),band)
 brow=face*math.exp(-((z-(line+.125))/.049)**2)*smooth(.05,.16,ax)
 c=mix(c,(.29,.32,.33),brow)
 stripe=face*(1-smooth(.035,.082,ax))*smooth(1.38,1.50,z)*(1-smooth(1.64,1.75,z));c=mix(c,(.006,.010,.014),stripe)
 muzzle=(1-smooth(-.88,-.77,y))*(1-smooth(1.39,1.46,z));c=mix(c,(.30,.33,.33),muzzle)
 c=mix(c,(.008,.012,.015),1-smooth(.14,.32,z))
 if y>.79:c=mix(c,(.009,.015,.02),(.5+.5*math.sin(y*33))*.72)
 return c

attr=skin.data.color_attributes.new(name='CoatColor',type='FLOAT_COLOR',domain='POINT')
for i,v in enumerate(skin.data.vertices):attr.data[i].color=(*color_at(v.co),1)
nt=coat.node_tree;p=nt.nodes.get('Principled BSDF');attribute=nt.nodes.new('ShaderNodeVertexColor');attribute.layer_name='CoatColor';nt.links.new(attribute.outputs['Color'],p.inputs['Base Color'])
# Anatomical details are placed against the sculpted surface.
from mathutils.bvhtree import BVHTree
bpy.context.view_layer.update();bvh=BVHTree.FromObject(skin,bpy.context.evaluated_depsgraph_get())
extras=[]
earrim=mat('Silver ear rim',(.13,.15,.16),0,.88)
earinner=mat('Ear velvet',(.017,.023,.027),0,.94)
lidmat=mat('Soft eyelid',(.007,.011,.014),0,.72)
clawmat=mat('Tiny worn claws',(.035,.04,.044),0,.6)
for side in [-1,1]:
 hit,normal,_,_=bvh.ray_cast(Vector((.206*side,-2,1.52)),Vector((0,1,0)))
 center=hit-normal*.015
 e=mesh('Recessed almond eye',center,(.029,.025,.023),eyemat,'sphere');extras.append((e,'head'))
 # Raised inner and outer eyelid arcs surround the exposed cornea.
 for top in [-1,1]:
  last=None
  for j in range(9):
   a=math.pi*j/8;v=hit+Vector((.031*math.cos(a),-.003,top*.018*math.sin(a)))
   if last is not None:
    o=bar('Fine eyelid',last,v,.0023,lidmat);extras.append((o,'head'))
   last=v
 # Cupped ear: concentric sculpted rings, with an irregular triangular-round edge.
 vertices=[];faces=[];segments=32
 for r,depth in [(1,.0),(.80,-.025),(.57,.009),(0,.035)]:
  for j in range(segments):
   a=math.tau*j/segments
   xx=.091*r*math.cos(a)*(1-.20*math.sin(a));zz=.125*r*math.sin(a)
   vertices.append((side*.232+xx,-.54+depth,1.81+zz))
 for ring in range(3):
  for j in range(segments):faces.append((ring*segments+j,ring*segments+(j+1)%segments,(ring+1)*segments+(j+1)%segments,(ring+1)*segments+j))
 d=bpy.data.meshes.new('Cupped ear');d.from_pydata(vertices,[],faces);d.update();o=bpy.data.objects.new('Cupped natural ear',d);bpy.context.collection.objects.link(o);d.materials.append(earrim);d.materials.append(earinner)
 for poly in d.polygons:poly.material_index=0 if poly.index<32 else 1;poly.use_smooth=True
 solid=o.modifiers.new('Ear thickness','SOLIDIFY');solid.thickness=.012
 sub=o.modifiers.new('Ear edge smoothing','SUBSURF');sub.levels=1;sub.render_levels=2
 extras.append((o,'head'))
# A triangular nose with nostril indent impressions and a short mouth seam.
nose=mesh('Sculpted black nose',(0,-1.042,1.286),(.051,.031,.029),nosemat,'sphere')
for v in nose.data.vertices:v.co.x*=.78+.28*(v.co.z/.03+.5)
extras.append((nose,'head'))
for side in [-1,1]:
 nostril=mesh('Nostril',(side*.027,-1.067,1.287),(.011,.004,.006),lidmat,'sphere');extras.append((nostril,'head'))
lip=mesh('Quiet mouth seam',(0,-.997,1.249),(.045,.034,.0035),lidmat,'sphere');extras.append((lip,'head'))
# Nails and individual fingers remain visible beyond the ankle fur.
for name,(_,_,wrist) in leg_points.items():
 for j in range(4):
  x=wrist[0]+(j-1.5)*.043;y=wrist[1]-.235+abs(j-1.5)*.030
  o=segment('Temporary claw',(x,y+.014,.035),(x,y-.013,.028),.004,.005);forms.remove(o);o.data.materials.clear();o.data.materials.append(clawmat);extras.append((o,name+'_shin'))
# Nose microstructure, subtle enough to avoid a glossy toy finish.
pnose=nosemat.node_tree.nodes.get('Principled BSDF');n=nosemat.node_tree.nodes.new('ShaderNodeTexNoise');n.inputs['Scale'].default_value=170
bump=nosemat.node_tree.nodes.new('ShaderNodeBump');bump.inputs['Strength'].default_value=.15;bump.inputs['Distance'].default_value=.003;nosemat.node_tree.links.new(n.outputs['Fac'],bump.inputs['Height']);nosemat.node_tree.links.new(bump.outputs[0],pnose.inputs['Normal'])
# Rig in reference neutral stance. Skin weights blend across elbows and shoulders.
bpy.ops.object.armature_add();rig=bpy.context.object;rig.name='Jimothy_Rig';bpy.ops.object.mode_set(mode='EDIT');rig.data.edit_bones.remove(rig.data.edit_bones[0])
bones=[('body',(0,.1,1.05),(0,.1,1.85),None),('head',(0,-.47,1.43),(0,-.7,1.64),'body'),('tail',(0,.66,1.32),(0,1.05,1.24),'body')]
for name,(hip,elbow,wrist) in leg_points.items():bones.extend([(name,hip,elbow,'body'),(name+'_shin',elbow,wrist,name)])
for name,head,tail,parent in bones:
 b=rig.data.edit_bones.new(name);b.head=head;b.tail=tail
 if parent:b.parent=rig.data.edit_bones[parent]
bpy.ops.object.mode_set(mode='OBJECT')
def bone_weights(pos):
 x,y,z=pos;ax=abs(x)
 if z<.23:
  nearest=min(leg_points,key=lambda name:(x-leg_points[name][2][0])**2+(y-leg_points[name][2][1])**2)
  return {nearest+'_shin':1}
 h=(1-smooth(-.74,-.44,y))*smooth(1.10,1.24,z)*(1-smooth(.26,.36,ax))
 if h>.02:return {'body':1-h,'head':h}
 tail=smooth(.72,.96,y)*smooth(1.04,1.16,z)*(1-smooth(.18,.30,ax))
 if tail>.02:return {'body':1-tail,'tail':tail}
 best=None;distance=100
 for name,(hip,elbow,wrist) in leg_points.items():
  for first,last in [(hip,elbow),(elbow,wrist)]:
   a,b=Vector(first),Vector(last);ab=b-a;t=max(0,min(1,(pos-a).dot(ab)/ab.length_squared));d=(pos-(a+ab*t)).length
   if d<distance:distance=d;best=name
 hip_z=leg_points[best][0][2]
 leg=(1-smooth(.11,.235,distance))*(1-smooth(.98,hip_z+.055,z))
 if leg<.001:return {'body':1}
 lower=1-smooth(.57,.81,z)
 return {'body':1-leg,best:leg*(1-lower),best+'_shin':leg*lower}
for name,_,_,_ in bones:skin.vertex_groups.new(name=name)
for i,v in enumerate(skin.data.vertices):
 for name,w in bone_weights(v.co).items():
  if w>0:skin.vertex_groups[name].add([i],w,'REPLACE')
mod=skin.modifiers.new('Continuous deformation rig','ARMATURE');mod.object=rig;skin.parent=rig
for o,bone in extras:
 bpy.context.view_layer.objects.active=o;bpy.ops.object.select_all(action='DESELECT');o.select_set(True);bpy.ops.object.transform_apply(location=True,rotation=True,scale=True)
 vg=o.vertex_groups.new(name=bone);vg.add(list(range(len(o.data.vertices))),1,'REPLACE');m=o.modifiers.new('Head rig','ARMATURE');m.object=rig;o.parent=rig
 # Ears use a neutral coat fallback so absent vertex attributes cannot turn black.
 if False:
  earcoat=mat('Ear silver',(.12,.14,.145),0,.85);o.data.materials.clear();o.data.materials.append(earcoat)
# Real 2K albedo atlas for export. Bake the vertex painting, not any source photograph.
bpy.ops.object.select_all(action='DESELECT');skin.select_set(True);bpy.context.view_layer.objects.active=skin
bpy.ops.object.mode_set(mode='EDIT');bpy.ops.mesh.select_all(action='SELECT');bpy.ops.uv.smart_project(angle_limit=1.15,island_margin=.008);bpy.ops.object.mode_set(mode='OBJECT')
baked=bpy.data.images.new('Jimothy_Albedo_2K',width=2048,height=2048);tex=nt.nodes.new('ShaderNodeTexImage');tex.image=baked;nt.nodes.active=tex
output=nt.nodes.get('Material Output');em=nt.nodes.new('ShaderNodeEmission');nt.links.new(attribute.outputs['Color'],em.inputs['Color']);nt.links.new(em.outputs[0],output.inputs['Surface'])
bpy.context.scene.render.engine='CYCLES';bpy.context.scene.cycles.samples=1;bpy.ops.object.bake(type='EMIT',margin=8)
baked.filepath_raw=str(OUT/'Jimothy_Albedo_2K.png');baked.file_format='PNG';baked.save();baked.pack();coat.diffuse_color=(1,1,1,1);p.inputs['Base Color'].default_value=(1,1,1,1);nt.links.new(p.outputs[0],output.inputs['Surface']);nt.links.new(tex.outputs['Color'],p.inputs['Base Color'])
# A scrambling bound: front reach and delayed rear kick, rather than a tidy diagonal trot.
for name,length in [('Idle',61),('Waddle',25),('Jump',31)]:
 rig.animation_data_create();act=bpy.data.actions.new(name);rig.animation_data.action=act
 for f in range(1,length+1):
  ph=(f-1)/(length-1)*math.tau
  for b in rig.pose.bones:b.rotation_mode='XYZ';b.rotation_euler=(0,0,0);b.location=(0,0,0)
  root=rig.pose.bones['body']
  if name=='Waddle':
   root.location.y=.048*math.sin(ph*2);root.rotation_euler.z=.08*math.sin(ph);root.rotation_euler.x=.08*math.sin(ph*2+.3)
   for i,n in enumerate(['front_L','front_R','rear_L','rear_R']):
    q=ph+[0,2.65,1.7,4.7][i];wave=math.sin(q)
    rig.pose.bones[n].rotation_euler.x=wave*(.63 if i<2 else .53)
    rig.pose.bones[n].rotation_euler.z=.07*math.sin(q+.5)
    rig.pose.bones[n+'_shin'].rotation_euler.x=max(0,math.sin(q-.35))*(.90 if i<2 else -.65)
   rig.pose.bones['head'].rotation_euler.x=-.08*math.sin(ph*2)
  elif name=='Idle':root.location.y=.012*math.sin(ph)
  else:
   root.location.y=.11*math.sin(ph/2)
   for n in leg_points:rig.pose.bones[n].rotation_euler.x=-.3*math.sin(ph/2);rig.pose.bones[n+'_shin'].rotation_euler.x=.7*math.sin(ph/2)
  for b in rig.pose.bones:b.keyframe_insert('rotation_euler',frame=f);b.keyframe_insert('location',frame=f)
 act.use_fake_user=True
rig.animation_data.action=bpy.data.actions['Idle'];bpy.context.scene.frame_set(1)
bpy.ops.object.select_all(action='DESELECT');skin.select_set(True);rig.select_set(True)
for o,bone in extras:o.select_set(True)
export('Jimothy',True,True)
# Directional, tapered fur strands follow the body and flow downward. Render-only.
# Unlike normal-aligned particle spikes, these curves lay against the surface.
furmats=[]
for i,c in enumerate([(.007,.010,.013),(.020,.025,.030),(.060,.068,.075),(.20,.21,.215),(.36,.38,.38),(.002,.003,.004)]):furmats.append(mat('Groom fiber '+str(i),c,0,.9))
curvedata=bpy.data.curves.new('Combed grizzled coat','CURVE');curvedata.dimensions='3D';curvedata.resolution_u=1;curvedata.bevel_depth=.00040;curvedata.bevel_resolution=0;curvedata.resolution_u=1
for m in furmats:curvedata.materials.append(m)
groom=bpy.data.objects.new('RENDER_ONLY_directional_fur',curvedata);bpy.context.collection.objects.link(groom)
# Evaluated triangles from the rest mesh (actual surface areas control density).
skin.data.calc_loop_triangles();triangles=list(skin.data.loop_triangles);cumulative=[];total=0
for t in triangles:
 a,b,c=[skin.data.vertices[j].co for j in t.vertices];total+=(b-a).cross(c-a).length/2;cumulative.append(total)
for k in range(210000):
 t=triangles[bisect.bisect_left(cumulative,random.random()*total)];a,b,c=[skin.data.vertices[j].co for j in t.vertices]
 u=random.random();v=random.random()
 if u+v>1:u=1-u;v=1-v
 pos=a+(b-a)*u+(c-a)*v
 x,y,z=pos
 if z<.18:continue
 normal=t.normal.normalized();face=y<-.62 and z>1.12
 # Three fiber populations avoid the combed/plastic surface of the prior pass.
 undercoat=k%3!=0
 length=random.uniform(.020,.043) if undercoat else random.uniform(.065,.135)
 if face:length*=.40
 if z<1:length*=.72
 # Top hair flows along the spine; flanks fall in small irregular locks.
 crown=smooth(1.68,1.98,z)
 flow=Vector((x*.20,.86,-.70))
 if face:flow=Vector((x*1.1,-.22,-.73))
 # Coherent small clumps, not one converging swirl over the whole head/back.
 cell=Vector((math.floor(x/.038)*.038+.019,math.floor(y/.038)*.038+.019,math.floor(z/.038)*.038+.019))
 clump=(cell-pos);clump-=normal*clump.dot(normal)
 tangent=flow-normal*flow.dot(normal)
 if tangent.length<.1:tangent=Vector((.1,1,-.2))
 tangent.normalize()
 lift=.62 if undercoat else .37
 direction=(tangent*(1-lift)+normal*lift).normalized()
 across=normal.cross(tangent).normalized()
 phase=random.random()*math.tau;curl=random.uniform(.001,.0045) if undercoat else random.uniform(.002,.008)
 spline=curvedata.splines.new('POLY');spline.points.add(4)
 basecol=color_at(pos)
 if max(basecol)<.018:material=5
 elif max(basecol)>.19:material=4 if k%4 else 2
 else:material=random.choices([0,1,2,3],[42,28,23,7] if undercoat else [22,23,38,17])[0]
 spline.material_index=material
 for j in range(5):
  f=j/4
  wave=math.sin(f*math.tau*1.25+phase)-math.sin(phase)
  pnt=pos+normal*.001+direction*(length*f)+normal*(math.sin(f*math.pi)*length*.2)+across*(wave*curl*f)+clump*(f*.22)
  spline.points[j].co=(*pnt,1);spline.points[j].radius=(.7 if undercoat else 1.05)*(1-f*.96)
# Soft ear fringes break up the hard sculpted rim and follow its cupped contour.
for side in [-1,1]:
 for k in range(6500):
  angle=random.uniform(0,math.tau);r=random.uniform(.65,1.025)
  xx=.091*r*math.cos(angle)*(1-.20*math.sin(angle));zz=.125*r*math.sin(angle)
  pos=Vector((side*.232+xx,-.545,1.81+zz))
  d=Vector((xx*.8,-.3,zz*3)).normalized();length=random.uniform(.012,.026)
  spl=curvedata.splines.new('POLY');spl.points.add(3);spl.material_index=3 if k%4 else 1
  for j in range(4):
   f=j/3;point=pos+d*length*f;spl.points[j].co=(*point,1);spl.points[j].radius=.62*(1-.94*f)
# Convert the offline groom to a skinned mesh. This remains outside the FBX.
# Each strand keeps its root's weights along its entire length. This prevents
# stretched triangular fur artifacts across shoulders/elbows during animation.
strand_roots=[(Vector(spl.points[0].co[:3]),len(spl.points)) for spl in curvedata.splines]
bpy.ops.object.select_all(action='DESELECT');groom.select_set(True);bpy.context.view_layer.objects.active=groom;bpy.ops.object.convert(target='MESH');groom=bpy.context.object
control_count=sum(count for _,count in strand_roots);ring_size=len(groom.data.vertices)//control_count
assert len(groom.data.vertices)==control_count*ring_size and ring_size==4,'Unexpected groom conversion topology'
bins={}
for name,_,_,_ in bones:groom.vertex_groups.new(name=name)
offset=0
for root,count in strand_roots:
 weights=bone_weights(root);size=count*ring_size
 for name,w in weights.items():
  quantized=round(w*40)
  if quantized>0:bins.setdefault((name,quantized),[]).extend(range(offset,offset+size))
 offset+=size
for (name,quantized),vertices in bins.items():groom.vertex_groups[name].add(vertices,quantized/40,'REPLACE')
m=groom.modifiers.new('Offline groom follows skeleton','ARMATURE');m.object=rig;groom.parent=rig
# Fine natural whiskers, pointing away from the muzzle.
whitemat=mat('Whisker silver',(.42,.45,.44),0,.65)
for side in [-1,1]:
 for i in range(4):
  whisker=bar('Whisker',(side*.082,-.975,1.29-i*.01),(side*(.28+i*.020),-1.01,1.29+(i-1.5)*.027),.0011,whitemat)
  bpy.ops.object.select_all(action='DESELECT');whisker.select_set(True);bpy.context.view_layer.objects.active=whisker;bpy.ops.object.transform_apply(location=True,rotation=True,scale=True)
  group=whisker.vertex_groups.new(name='head');group.add(list(range(len(whisker.data.vertices))),1,'REPLACE');wm=whisker.modifiers.new('Whiskers follow muzzle','ARMATURE');wm.object=rig;whisker.parent=rig
# Low-angle neutral studio view; no perspective trick to exaggerate the silhouette.
plinth=mat('Dark green studio',(.016,.043,.038));mesh('Display base',(0,0,-.035),(2,2,.028),plinth,'cylinder',.018)
stage((-4.4,-3.2,2.5),(0,-.02,1.09),3.15)
for o in bpy.data.objects:
 if o.type=='LIGHT':o.data.energy*=.52
s=bpy.context.scene;s.cycles.samples=64
bpy.data.objects['Golden hour'].data.color=(1,.88,.76)
bpy.data.objects['Sky fill'].data.color=(.77,.85,1)
bpy.data.objects['Sky fill'].data.energy=370
bpy.data.objects['Rim'].data.color=(1,.63,.36)
render('jimothy-model-v4.png',1920,1920)
cam=s.camera;cam.location=(-5,-.03,2.05);cam.rotation_euler=(Vector((0,0,1.09))-cam.location).to_track_quat('-Z','Y').to_euler();s.render.filepath=str(ROOT/'Art/Renders/jimothy-side-v4.png');bpy.ops.render.render(write_still=True)
# Render the actual deformed, groomed Blender mesh in motion.
groom.hide_render=False
cam.location=(-4.4,-3.2,2.5);cam.rotation_euler=(Vector((0,-.02,1.09))-cam.location).to_track_quat('-Z','Y').to_euler();rig.animation_data.action=bpy.data.actions['Waddle'];s.render.resolution_x=640;s.render.resolution_y=640;s.cycles.samples=8
for f in range(1,25):
 s.frame_set(f);s.render.filepath=str(ROOT/'Art/Renders'/('run-v4-%02d.png'%f));bpy.ops.render.render(write_still=True)
groom.hide_render=False;rig.animation_data.action=bpy.data.actions['Idle'];s.frame_set(1);s.render.resolution_x=1920;s.render.resolution_y=1920;s.cycles.samples=40
bpy.ops.wm.save_as_mainfile(filepath=str(ROOT/'Art/Blender/Jimothy.blend'),compress=True)
report={'character_triangles':sum(len(p.vertices)-2 for p in skin.data.polygons),'bones':11,'clips':['Idle','Waddle','Jump'],'texture':'2048x2048 albedo','notes':'Continuous sculpt and blended limb weights. Directional groom is skinned for Blender motion but excluded from FBX; runtime uses the painted albedo pending optimized fur cards. Rig timing inferred from supplied stills.'}
(ROOT/'Art/Blender/asset-report.json').write_text(json.dumps(report,indent=2));print('JIMOTHY_REFINEMENT_V4_COMPLETE')

import runpy
runpy.run_path(str(Path(__file__).resolve().parent/'export_refined_character.py'),run_name='__main__')
