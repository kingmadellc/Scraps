"""Layered, region-groomed fur over the unchanged V9 skin, skeleton and actions."""
import bpy,math,random,bisect,json,sys
from pathlib import Path
from mathutils import Vector
root=Path(__file__).resolve().parents[1]
bpy.ops.wm.open_mainfile(filepath=str(root/'Art/Blender/Jimothy-v9.blend'))
rig=bpy.data.objects['Jimothy_Rig'];rig.animation_data.action=None
for b in rig.pose.bones:b.matrix_basis.identity()
bpy.context.view_layer.update()
for o in list(bpy.data.objects):
 if o.name.startswith('Jimothy_FurCards'):bpy.data.objects.remove(o,do_unlink=True)
skin=bpy.data.objects['Jimothy_ContinuousSkin'];skin.data.update();skin.data.calc_loop_triangles()
triangles=[];cdf=[];area=0
for tri in skin.data.loop_triangles:
 if 'painted coat' not in skin.data.materials[tri.material_index].name:continue
 a,b,c=[skin.data.vertices[i].co for i in tri.vertices];area+=(b-a).cross(c-a).length*.5;triangles.append(tri);cdf.append(area)
random.seed(1050);verts=[];faces=[];uvs=[];colors=[];weights=[];normals=[];regions={};lengths=[]
attr=skin.data.color_attributes['CoatColor']
for k in range(63000):
 tri=triangles[bisect.bisect_left(cdf,random.random()*area)];u,v=random.random(),random.random()
 if u+v>1:u,v=1-u,1-v
 points=[skin.data.vertices[i] for i in tri.vertices];bary=[1-u-v,u,v]
 p=sum((q.co*w for q,w in zip(points,bary)),Vector());n=sum((q.normal*w for q,w in zip(points,bary)),Vector()).normalized()
 if p.z<.20:continue
 face=p.y<-.55 and p.z>.91;leg=p.z<.82;tail=p.y>.68
 region='face' if face else 'legs' if leg else 'tail' if tail else 'body'
 guard=random.random()<(.12 if face else .28)
 if face and not guard:continue
 regions[region]=regions.get(region,0)+1
 # Correlated variation forms locks, rather than a uniform radial halo.
 lock=.5+.5*math.sin(p.x*34+math.sin(p.y*18)*1.5+p.z*21)
 length=random.uniform(.029,.055)*(1+.32*lock);width=random.uniform(.0026,.0042)
 if guard:length*=1.65;width*=.70
 if face:length=random.uniform(.006,.015);width*=.50
 elif leg:length*=.40;width*=.65
 elif tail:length*=1.30
 elif abs(p.x)>.25 and p.z<1.36:length*=1.5
 elif -.43<p.y<.05 and p.z>1.15:length*=1.28
 lengths.append(length)
 flow=Vector((p.x*.16,.95,-.72))
 if face:flow=Vector((p.x*.8,-.15,-.9))
 elif leg:flow=Vector((p.x*.1,.14,-1))
 elif tail:flow=Vector((p.x*.1,1,-.30))
 tangent=flow-n*flow.dot(n)
 if tangent.length<.01:tangent=Vector((0,1,0))
 tangent.normalize();side=n.cross(tangent).normalized()
 tangent=(tangent+side*(.10*math.sin(p.y*29+p.z*23)+random.uniform(-.09,.09))).normalized()
 direction=(tangent+n*(.10 if face or leg else .15)).normalized()
 source=sum((Vector(attr.data[q.index].color[:3])*w for q,w in zip(points,bary)),Vector())
 lum=sum(source)/3
 if face:base=max(.02,min(.38,lum))
 else:base=max(.028,min(.13,lum*.85))
 # Dark roots, brown-grey midshafts, sparse silver tips. Facial mask remains sampled from skin.
 rootcol=Vector((base*.93,base*.94,base*.98))
 silver=guard and not face and random.random()<.56
 tipbase=random.uniform(.14,.25) if silver else base*random.uniform(1.10,1.65)
 tipcol=Vector((tipbase*.98,tipbase,tipbase*1.035))
 wg={}
 for q,w in zip(points,bary):
  for g in q.groups:
   name=skin.vertex_groups[g.group].name;wg[name]=wg.get(name,0)+g.weight*w
 total=sum(wg.values());wg={name:w/total for name,w in wg.items() if w>.00001}
 start=len(verts);samples=[(0,-1),(0,1),(.56,-1),(.56,1),(1,0)] if guard else [(0,-1),(0,1),(1,0)]
 for f,sign in samples:
  center=p+n*.0005+direction*length*f+n*math.sin(f*math.pi)*length*.065
  verts.append(center+side*width*.5*sign*(1-f*.85));uvs.append(((sign+1)/2,f));colors.append((*tuple(rootcol.lerp(tipcol,f*f)),min(1,length/.14)));weights.append(wg);normals.append(tuple(n))
 if guard:faces.extend([(start,start+1,start+3),(start,start+3,start+2),(start+2,start+3,start+4)])
 else:faces.append((start,start+1,start+2))
mesh=bpy.data.meshes.new('Layered charcoal and silver directional groom');mesh.from_pydata(verts,[],faces);mesh.update()
fur=bpy.data.objects.new('Jimothy_FurCards',mesh);bpy.context.collection.objects.link(fur)
uv=mesh.uv_layers.new(name='RootToTip');col=mesh.color_attributes.new(name='FurColor',type='FLOAT_COLOR',domain='POINT')
for poly in mesh.polygons:
 poly.use_smooth=True
 for li in poly.loop_indices:uv.data[li].uv=uvs[mesh.loops[li].vertex_index]
for i,c in enumerate(colors):col.data[i].color=c
for name in {name for w in weights for name in w}:
 g=fur.vertex_groups.new(name=name)
 for i,w in enumerate(weights):
  if name in w:g.add([i],w[name],'REPLACE')
mesh.normals_split_custom_set_from_vertices(normals)
mat=bpy.data.materials.new('Jimothy realtime fur v10 layered');mat.use_nodes=True;nt=mat.node_tree;bsdf=nt.nodes.get('Principled BSDF');vc=nt.nodes.new('ShaderNodeVertexColor');vc.layer_name='FurColor';nt.links.new(vc.outputs['Color'],bsdf.inputs['Base Color']);bsdf.inputs['Roughness'].default_value=.68;bsdf.inputs['Specular IOR Level'].default_value=.28
mesh.materials.append(mat);mod=fur.modifiers.new('Rooted skinning','ARMATURE');mod.object=rig;fur.parent=rig
rig.animation_data.action=bpy.data.actions['Idle'];bpy.context.scene.frame_set(1)
bpy.ops.wm.save_as_mainfile(filepath=str(root/'Art/Blender/Jimothy-v10.blend'),compress=True)
report={'source':'Jimothy-v9.blend','regions':regions,'fur_triangles':len(faces),'fibers':sum(regions.values()),'length_range_model_units':[min(lengths),max(lengths)],'skin_rig_actions_unchanged':True,'texture_alpha_layers':0}
(root/'Documentation/character-v10.json').write_text(json.dumps(report,indent=2));print('V10_GROOM_READY',report)
