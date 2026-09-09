"""Refine existing authored sculpt; export mobile fur ribbons with rooted skin weights."""
import bpy, math, random, bisect, json, sys
from pathlib import Path
from mathutils import Vector
root=Path(__file__).resolve().parents[1];sys.path.insert(0,str(root/'Tools'))
bpy.ops.wm.open_mainfile(filepath=str(root/'Art/Blender/Jimothy.blend'))
for im in bpy.data.images:
 if im.name.startswith('Jimothy_Albedo'):
  im.filepath_raw=str(root/'Unity/Assets/Jimothy/Resources/Jimothy_Albedo_2K.png');im.file_format='PNG';im.save()
rig=bpy.data.objects['Jimothy_Rig'];rig.animation_data.action=bpy.data.actions['Idle'];bpy.context.scene.frame_set(1)
# Preserve the heavy offline groom in the archived v4 source; the new source is the actual runtime asset.
for o in list(bpy.data.objects):
 if o.name.startswith('RENDER_ONLY'):bpy.data.objects.remove(o,do_unlink=True)
skin=bpy.data.objects['Jimothy_ContinuousSkin']
# Compact the spine depth slightly and lift the broad hump, retaining long slender forearms.
def warp(p):
 x,y,z=p
 # Shorter limbs, narrower rear haunches and a head that clears the shoulder line.
 high=max(0,min(1,(z-.75)/.5));rear=max(0,min(1,(y+.05)/.65))
 head=max(0,min(1,(-y-.36)/.30))*max(0,min(1,(z-.90)/.35))
 return Vector((x*(1-.14*rear*high),y-.09*head,z-.14*max(0,min(1,(z-.09)/.91))-.09*rear*high+.32*head))
for o in bpy.data.objects:
 if o.type=='MESH' and any(m.type=='ARMATURE' and m.object==rig for m in o.modifiers):
  for v in o.data.vertices:v.co=warp(v.co)
bpy.context.view_layer.objects.active=rig;bpy.ops.object.mode_set(mode='EDIT')
for b in rig.data.edit_bones:b.head=warp(b.head);b.tail=warp(b.tail)
bpy.ops.object.mode_set(mode='OBJECT')
# Reduce the continuous base surface: its silhouette is now supported by real fur geometry.
bpy.context.view_layer.objects.active=skin
for m in list(skin.modifiers):
 if m.type=='SUBSURF':skin.modifiers.remove(m)
if len(skin.data.polygons)>15000:
 dec=skin.modifiers.new('Mobile base reduction','DECIMATE');dec.ratio=.52;bpy.ops.object.modifier_apply(modifier=dec.name)
import add_paw_rig_v5
paw_report=add_paw_rig_v5.add_paws(rig,skin)
random.seed(606);skin.data.calc_loop_triangles();ts=list(skin.data.loop_triangles);cdf=[];area=0
for t in ts:
 a,b,c=[skin.data.vertices[i].co for i in t.vertices];area+=(b-a).cross(c-a).length/2;cdf.append(area)
verts=[];faces=[];uv=[];colors=[];weights=[]
colorattr=skin.data.color_attributes.get('CoatColor')
for k in range(6200):
 tri=ts[bisect.bisect_left(cdf,random.random()*area)];u,v=random.random(),random.random()
 if u+v>1:u,v=1-u,1-v
 bary=[1-u-v,u,v];points=[skin.data.vertices[i] for i in tri.vertices];pos=sum((p.co*w for p,w in zip(points,bary)),Vector());n=tri.normal.normalized()
 if pos.z<.20:continue
 face=pos.y<-.61 and pos.z>1.08
 length=random.uniform(.07,.18)*( .30 if face else (.48 if pos.z<.85 else 1))
 if not face and pos.z>.95 and random.random()<.16:length*=1.5
 width=random.uniform(.010,.025)*( .38 if face else (.55 if pos.z<.85 else 1))
 flow=Vector((pos.x*.35,.8,-.7)) if not face else Vector((pos.x*.9,-.3,-.8))
 tangent=flow-n*flow.dot(n)
 if tangent.length<.01:tangent=Vector((0,1,0))
 tangent.normalize();side=n.cross(tangent).normalized();direction=(tangent*.96+n*.15).normalized()
 col=Vector((.045,.055,.062))
 if colorattr and colorattr.domain=='POINT':col=sum((Vector(colorattr.data[p.index].color[:3])*w for p,w in zip(points,bary)),Vector())
 # Cool silver guard hairs scattered through charcoal undercoat, with dark roots.
 grizzle=random.random(); gain=1.3 if grizzle<.93 else 2.1
 col=Vector(tuple(min(.48,max(.02,c*gain*2.5)) for c in col))
 wg={}
 for p,w in zip(points,bary):
  for g in p.groups:
   name=skin.vertex_groups[g.group].name;wg[name]=wg.get(name,0)+g.weight*w
 start=len(verts)
 for j in range(4):
  f=j/3;center=pos+n*.003+direction*length*f+n*math.sin(f*math.pi)*length*.14
  for sign in [-1,1]:
   verts.append(center+side*width*.5*sign*(1-.3*f));uv.append(((sign+1)/2,f));colors.append((*tuple(col*(.72+.40*f)),1));weights.append(wg)
 for j in range(3):faces.append((start+j*2,start+j*2+1,start+j*2+3,start+j*2+2))
mesh=bpy.data.meshes.new('Rooted fur ribbons');mesh.from_pydata(verts,[],faces);mesh.update();fur=bpy.data.objects.new('Jimothy_FurCards',mesh);bpy.context.collection.objects.link(fur)
layer=mesh.uv_layers.new(name='FurStrands')
for poly in mesh.polygons:
 poly.use_smooth=True
 for li in poly.loop_indices:layer.data[li].uv=uv[mesh.loops[li].vertex_index]
attr=mesh.color_attributes.new(name='FurColor',type='FLOAT_COLOR',domain='POINT')
for i,c in enumerate(colors):attr.data[i].color=c
for name in {name for w in weights for name in w}:
 group=fur.vertex_groups.new(name=name)
 for i,w in enumerate(weights):
  if w.get(name,0)>.001:group.add([i],w[name],'REPLACE')
mat=bpy.data.materials.new('Jimothy realtime fur');mat.use_nodes=True;mat.diffuse_color=(.14,.17,.19,1)
nt=mat.node_tree;p=nt.nodes.get('Principled BSDF');vc=nt.nodes.new('ShaderNodeVertexColor');vc.layer_name='FurColor';nt.links.new(vc.outputs['Color'],p.inputs['Base Color']);p.inputs['Roughness'].default_value=.86
texpath=root/'Unity/Assets/Jimothy/Resources/FurStrands.png'
if texpath.exists():
 im=bpy.data.images.load(str(texpath));im.pack();tex=nt.nodes.new('ShaderNodeTexImage');tex.image=im;nt.links.new(tex.outputs['Alpha'],p.inputs['Alpha']);mat.surface_render_method='DITHERED'
mesh.materials.append(mat);mod=fur.modifiers.new('Rooted skinning','ARMATURE');mod.object=rig;fur.parent=rig
# Keep the light silver face and tactile undercoat visible beneath fibers.
coat=bpy.data.materials.get('Jimothy painted coat')
if coat:
 p=coat.node_tree.nodes.get('Principled BSDF');p.inputs['Roughness'].default_value=.88
 noise=coat.node_tree.nodes.new('ShaderNodeTexNoise');noise.inputs['Scale'].default_value=165;noise.inputs['Detail'].default_value=2
 bump=coat.node_tree.nodes.new('ShaderNodeBump');bump.inputs['Strength'].default_value=.28;bump.inputs['Distance'].default_value=.006;coat.node_tree.links.new(noise.outputs['Fac'],bump.inputs['Height']);coat.node_tree.links.new(bump.outputs['Normal'],p.inputs['Normal'])
if (root/'Tools/refine_gait_v5.py').exists():
 import refine_gait_v5
 refine_gait_v5.refine(rig)
 import refine_jump_v6
 jump_report=refine_jump_v6.refine(rig)
 (root/"Documentation/jump-v6.json").write_text(json.dumps(jump_report,indent=2))
# Batch all non-fur skin details into one renderer while retaining material regions.
base=[o for o in bpy.data.objects if o.type=='MESH' and o!=fur and any(m.type=='ARMATURE' and m.object==rig for m in o.modifiers)]
bpy.ops.object.select_all(action='DESELECT')
for o in base:o.select_set(True)
bpy.context.view_layer.objects.active=skin;bpy.ops.object.join();bpy.ops.object.material_slot_remove_unused()
# One coat, one dark detail and one silver detail material keep the hero's render passes bounded.
slots=list(skin.data.materials);dark_index=next(i for i,m in enumerate(slots) if m.name=='Natural dark eyes');silver_index=next(i for i,m in enumerate(slots) if m.name=='Silver ear rim')
for poly in skin.data.polygons:
 name=slots[poly.material_index].name
 if name in ['Soft eyelid','Ear velvet','Soft black nose','Tiny worn claws']:poly.material_index=dark_index
 elif name=='Whisker silver':poly.material_index=silver_index
bpy.ops.object.material_slot_remove_unused()

rig.animation_data.action=bpy.data.actions['Idle'];bpy.context.scene.frame_set(1)
bpy.ops.wm.save_as_mainfile(filepath=str(root/'Art/Blender/Jimothy-v6.blend'),compress=True)
# Export base and fur as separate renderers so fur shadows/LOD can be controlled.
sources=[o for o in bpy.data.objects if o.type=='MESH' and any(m.type=='ARMATURE' and m.object==rig for m in o.modifiers)]
bpy.ops.object.select_all(action='DESELECT')
for o in sources:o.select_set(True)
rig.select_set(True);bpy.context.view_layer.objects.active=rig
bpy.ops.export_scene.fbx(filepath=str(root/'Unity/Assets/Jimothy/Resources/Jimothy.fbx'),use_selection=True,object_types={'MESH','ARMATURE'},add_leaf_bones=False,axis_forward='-Z',axis_up='Y',bake_anim=True,bake_anim_step=.25,bake_anim_simplify_factor=0,bake_anim_use_all_actions=True,bake_anim_use_nla_strips=False,path_mode='AUTO')
(root/'Documentation/character-v6.json').write_text(json.dumps({'fur_cards':len(faces)//3,'fur_triangles':len(faces)*2,'base_triangles':sum(len(p.vertices)-2 for p in skin.data.polygons),'source':'Jimothy-v6.blend','gait_actions':[a.name for a in bpy.data.actions]},indent=2))
print('CHARACTER_V6_EXPORTED')
