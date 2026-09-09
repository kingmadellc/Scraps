"""V7 anatomical refinement and short, directional geometric fur. V6 source remains untouched."""
import bpy,math,random,bisect,json,sys
from pathlib import Path
from mathutils import Vector
root=Path(__file__).resolve().parents[1];sys.path.insert(0,str(root/'Tools'))
bpy.ops.wm.open_mainfile(filepath=str(root/'Art/Blender/Jimothy-v6.blend'))
rig=bpy.data.objects['Jimothy_Rig'];rig.animation_data.action=None
for bone in rig.pose.bones:bone.matrix_basis.identity()
bpy.context.view_layer.update()
for o in list(bpy.data.objects):
 if o.name.startswith('Jimothy_FurCards'):bpy.data.objects.remove(o,do_unlink=True)
skin=bpy.data.objects['Jimothy_ContinuousSkin']
def smooth(a,b,x):
 t=max(0,min(1,(x-a)/(b-a)));return t*t*(3-2*t)
def warp(p):
 x,y,z=p;upper=smooth(.75,1.20,z);head=(1-smooth(-.69,-.39,y))*smooth(.82,1.07,z)
 # Less barrel depth, hunch carried high behind the low, relatively small head. Long limbs preserved.
 hump=math.exp(-((y-.00)/.40)**2)*smooth(1.13,1.49,z)
 return Vector((x*(1-.065*upper-.04*head),y*(1-.10*upper)+.035*head,z+.065*hump-.10*head-.065*smooth(.05,.60,y)*upper))
for o in bpy.data.objects:
 if o.type=='MESH' and any(m.type=='ARMATURE' and m.object==rig for m in o.modifiers):
  for v in o.data.vertices:v.co=warp(v.co)
bpy.context.view_layer.objects.active=rig;bpy.ops.object.mode_set(mode='EDIT')
for b in rig.data.edit_bones:b.head=warp(b.head);b.tail=warp(b.tail)
bpy.ops.object.mode_set(mode='OBJECT')
skin.data.update();skin.data.calc_loop_triangles();ts=[];cdf=[];area=0
for tri in skin.data.loop_triangles:
 if 'painted coat' not in skin.data.materials[tri.material_index].name:continue
 a,b,c=[skin.data.vertices[i].co for i in tri.vertices];area+=(b-a).cross(c-a).length*.5;ts.append(tri);cdf.append(area)
random.seed(707);verts=[];faces=[];uv=[];colors=[];weights=[];attr=skin.data.color_attributes.get('CoatColor');regions={}
for k in range(80000):
 tri=ts[bisect.bisect_left(cdf,random.random()*area)];u,v=random.random(),random.random()
 if u+v>1:u,v=1-u,1-v
 bary=[1-u-v,u,v];points=[skin.data.vertices[i] for i in tri.vertices];p=sum((q.co*w for q,w in zip(points,bary)),Vector());n=sum((q.normal*w for q,w in zip(points,bary)),Vector()).normalized()
 if p.z<.19:continue
 face=p.y<-.52 and p.z>.90;leg=p.z<.79;tail=p.y>.70
 region='face' if face else 'legs' if leg else 'tail' if tail else 'body';regions[region]=regions.get(region,0)+1
 length=random.uniform(.028,.052);width=random.uniform(.0012,.0021)
 guard=random.random()<.16
 if guard:length*=1.55;width*=.70
 if face:length*=.28;width*=.70
 elif leg:length*=.48;width*=.80
 elif tail:length*=1.35
 shoulder=not(face or leg or tail) and -.40<p.y<.10 and p.z>1.05
 flank=not(face or leg or tail) and abs(p.x)>.28 and p.z<1.28
 crest=not(face or leg or tail) and n.z>.5 and p.z>1.35
 if shoulder:length*=1.8
 elif flank:length*=1.5
 elif crest:length*=1.35
 flow=Vector((p.x*.10,.90,-.65))
 if face:flow=Vector((p.x*.55,.08,-1))
 elif leg:flow=Vector((p.x*.05,.13,-1))
 tangent=flow-n*flow.dot(n)
 if tangent.length<.01:tangent=Vector((0,1,0))
 tangent.normalize();side=n.cross(tangent).normalized();lift=.34 if shoulder else .27 if flank or crest else .16
 direction=(tangent*.96+n*lift).normalized()
 col=Vector((.045,.049,.052))
 if attr and attr.domain=='POINT':col=sum((Vector(attr.data[q.index].color[:3])*w for q,w in zip(points,bary)),Vector())
 # Warm charcoal undercoat, sparse cool silver guards. No uniform white splinters.
 luminance=max(.018,min(.32,sum(col)/3))
 if face:base=luminance*.80
 else:base=max(.023,min(.075,luminance*.90))
 if guard and not face:base=random.uniform(.065,.125)
 tint=Vector((base*.98,base,base*1.025));wg={}
 for q,w in zip(points,bary):
  for g in q.groups:
   name=skin.vertex_groups[g.group].name;wg[name]=wg.get(name,0)+g.weight*w
 start=len(verts)
 samples=[(0,-1),(0,1),(.53,-1),(.53,1),(1,0)] if guard else [(0,-1),(0,1),(1,0)]
 for f,sign in samples:
  center=p+n*.0007+direction*length*f+n*math.sin(f*math.pi)*length*(.22 if shoulder or flank else .11)
  verts.append(center+side*width*.5*sign*(1-f*.75));uv.append(((sign+1)/2,f));colors.append((*tuple(tint*(.64+.36*f)),1));weights.append(wg)
 if guard:faces.extend([(start,start+1,start+3),(start,start+3,start+2),(start+2,start+3,start+4)])
 else:faces.append((start,start+1,start+2))
mesh=bpy.data.meshes.new('Fine tapered directional guard fibers');mesh.from_pydata(verts,[],faces);mesh.update();fur=bpy.data.objects.new('Jimothy_FurCards',mesh);bpy.context.collection.objects.link(fur)
layer=mesh.uv_layers.new(name='RootToTip');color=mesh.color_attributes.new(name='FurColor',type='FLOAT_COLOR',domain='POINT')
for poly in mesh.polygons:
 poly.use_smooth=True
 for li in poly.loop_indices:layer.data[li].uv=uv[mesh.loops[li].vertex_index]
for i,c in enumerate(colors):color.data[i].color=c
for name in {name for w in weights for name in w}:
 group=fur.vertex_groups.new(name=name)
 for i,w in enumerate(weights):
  if w.get(name,0)>.001:group.add([i],w[name],'REPLACE')
mat=bpy.data.materials.new('Jimothy realtime fur v7 geometric');mat.use_nodes=True;nt=mat.node_tree;bsdf=nt.nodes.get('Principled BSDF');vc=nt.nodes.new('ShaderNodeVertexColor');vc.layer_name='FurColor';nt.links.new(vc.outputs['Color'],bsdf.inputs['Base Color']);bsdf.inputs['Roughness'].default_value=.91;bsdf.inputs['Specular IOR Level'].default_value=.17;mesh.materials.append(mat);mod=fur.modifiers.new('Rooted skinning','ARMATURE');mod.object=rig;fur.parent=rig
# Dense directional microtexture supplies undercoat between geometric guard fibers.
coat=bpy.data.materials.get('Jimothy painted coat')
if coat:
 nt=coat.node_tree;bsdf=nt.nodes.get('Principled BSDF');original=bsdf.inputs['Base Color'].links[0].from_socket if bsdf.inputs['Base Color'].is_linked else None
 coords=nt.nodes.new('ShaderNodeTexCoord');stretch=nt.nodes.new('ShaderNodeVectorMath');stretch.operation='MULTIPLY';stretch.inputs[1].default_value=(2.5,.6,2.2);nt.links.new(coords.outputs['Generated'],stretch.inputs[0])
 noise=nt.nodes.new('ShaderNodeTexNoise');noise.inputs['Scale'].default_value=125;noise.inputs['Detail'].default_value=2;nt.links.new(stretch.outputs['Vector'],noise.inputs['Vector'])
 ramp=nt.nodes.new('ShaderNodeValToRGB');ramp.color_ramp.elements[0].position=.25;ramp.color_ramp.elements[0].color=(.28,.30,.32,1);ramp.color_ramp.elements[1].position=.75;ramp.color_ramp.elements[1].color=(.86,.88,.90,1);nt.links.new(noise.outputs['Fac'],ramp.inputs[0])
 if original:
  mix=nt.nodes.new('ShaderNodeMixRGB');mix.blend_type='MULTIPLY';mix.inputs[0].default_value=.48;nt.links.new(original,mix.inputs[1]);nt.links.new(ramp.outputs['Color'],mix.inputs[2]);nt.links.new(mix.outputs[0],bsdf.inputs['Base Color'])
 bsdf.inputs['Roughness'].default_value=.96;bsdf.inputs['Specular IOR Level'].default_value=.12
if (root/'Tools/refine_gait_v7.py').exists():
 import refine_gait_v7
 gait_report=refine_gait_v7.refine(rig)
 (root/'Documentation/gait-v7.json').write_text(json.dumps(gait_report,indent=2,default=str))
rig.animation_data.action=bpy.data.actions.get('Idle');bpy.context.scene.frame_set(1)
bpy.ops.wm.save_as_mainfile(filepath=str(root/'Art/Blender/Jimothy-v7.blend'),compress=True)
report={'source':'Jimothy-v7.blend','regions':regions,'geometric_fibers':sum(regions.values()),'fur_triangles':len(faces),'base_triangles':sum(len(p.vertices)-2 for p in skin.data.polygons),'alpha_cards':False,'original_source_untouched':'Jimothy-v6.blend'}
(root/'Documentation/character-v7.json').write_text(json.dumps(report,indent=2));print('CHARACTER_V7_SAVED',report)
if '--export' in sys.argv:
 bpy.ops.object.select_all(action='DESELECT')
 for o in bpy.data.objects:
  if o.type=='MESH' and any(m.type=='ARMATURE' and m.object==rig for m in o.modifiers):o.select_set(True)
 rig.select_set(True);bpy.context.view_layer.objects.active=rig
 bpy.ops.export_scene.fbx(filepath=str(root/'Unity/Assets/Jimothy/Resources/Jimothy.fbx'),use_selection=True,object_types={'MESH','ARMATURE'},add_leaf_bones=False,axis_forward='-Z',axis_up='Y',bake_anim=True,bake_anim_step=.25,bake_anim_simplify_factor=0,bake_anim_use_all_actions=True,bake_anim_use_nla_strips=False,path_mode='AUTO')
