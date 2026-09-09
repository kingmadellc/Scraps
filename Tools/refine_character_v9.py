"""V9 coordinated body/fur/bind-shape refinement, compact catch/gather gait. Source V8 untouched."""
import bpy,sys,json,math,bmesh
from pathlib import Path
from mathutils import Vector,Matrix,kdtree
root=Path(__file__).resolve().parents[1];sys.path.insert(0,str(root/'Tools'))
bpy.ops.wm.open_mainfile(filepath=str(root/'Art/Blender/Jimothy-v8.blend'))
rig=bpy.data.objects['Jimothy_Rig'];rig.animation_data.action=None
for b in rig.pose.bones:b.matrix_basis=Matrix.Identity(4)
bpy.context.view_layer.update()
def smooth(a,b,x):
 t=max(0,min(1,(x-a)/(b-a)));return t*t*(3-2*t)
def warp(p):
 x,y,z=p;upper=smooth(.8,1.23,z);rear=smooth(-.05,.68,y);head=(1-smooth(-.70,-.43,y))*smooth(.89,1.14,z)
 # Narrow the rump/haunches and bring feet underneath their hips. Keep the tall shoulder hunch.
 xx=x*(1-.16*rear*upper-.06*upper-.18*(1-smooth(.18,1.15,z)))
 yy=y*(1-.10*rear*upper)-.13*head
 zz=z-.12*rear*smooth(1.2,1.8,z)+.09*head
 # Slight fuller visible face/ears, without raising the head into an upright mascot pose.
 xx*=1+.10*head;zz+=(z-1.31)*.07*head
 return Vector((xx,yy,zz))
# Fine, low-contrast facial guards over micro-normal undercoat; avoid both bald plastic and dark dots.
fur=bpy.data.objects['Jimothy_FurCards'];skin=bpy.data.objects['Jimothy_ContinuousSkin'];maskIds={i for p in skin.data.polygons if 'painted coat' in skin.data.materials[p.material_index].name for i in p.vertices}
maskTree=kdtree.KDTree(len(maskIds))
for i in maskIds:maskTree.insert(skin.data.vertices[i].co,i)
maskTree.balance();coatColor=skin.data.color_attributes['CoatColor'];fiberColor=fur.data.color_attributes['FurColor'];tipUV=[0]*len(fur.data.vertices)
for loop,uv in zip(fur.data.loops,fur.data.uv_layers.active.data):tipUV[loop.vertex_index]=uv.uv.y
remove=set();start=0
while start<len(fur.data.vertices):
 end=start+2
 while end<len(tipUV)-1 and tipUV[end]<.999:end+=1
 origin=(fur.data.vertices[start].co+fur.data.vertices[start+1].co)*.5
 if origin.y<-.53 and origin.z>.92:
  if end-start==2:remove.update(range(start,end+1))
  else:
   color=coatColor.data[maskTree.find(origin)[1]].color
   for i in range(start,end+1):
    v=fur.data.vertices[i];v.co=origin+(v.co-origin)*.30;fiberColor.data[i].color=tuple([color[k]*.93 for k in range(3)]+[.08])
 start=end+1
bm=bmesh.new();bm.from_mesh(fur.data);bm.verts.ensure_lookup_table();bmesh.ops.delete(bm,geom=[bm.verts[i] for i in remove],context='VERTS');bm.to_mesh(fur.data);bm.free();fur.data.update()
coat=bpy.data.materials.get('Jimothy painted coat');nt=coat.node_tree;bsdf=nt.nodes.get('Principled BSDF');noise=nt.nodes.new('ShaderNodeTexNoise');noise.inputs['Scale'].default_value=180;noise.inputs['Detail'].default_value=2;bump=nt.nodes.new('ShaderNodeBump');bump.inputs['Strength'].default_value=.22;bump.inputs['Distance'].default_value=.012;nt.links.new(noise.outputs['Fac'],bump.inputs['Height']);nt.links.new(bump.outputs['Normal'],bsdf.inputs['Normal'])
skin=bpy.data.objects['Jimothy_ContinuousSkin'];eyes={v for p in skin.data.polygons if skin.data.materials[p.material_index].name=='Natural dark eyes' for v in p.vertices}
for i in eyes:
 v=skin.data.vertices[i];x,y,z=v.co
 if y<-.70 and 1.50<z<1.70 and .10<abs(x)<.26:
  center=Vector((-.175 if x<0 else .175,-.758,1.598));v.co=center+(v.co-center)*1.08+Vector((0,.014,0))
# Re-establish continuous skin normals after topology removal (BMesh may discard custom normals).
kd=kdtree.KDTree(len(skin.data.vertices))
for v in skin.data.vertices:kd.insert(v.co,v.index)
kd.balance();fur.data.normals_split_custom_set_from_vertices([tuple(skin.data.vertices[kd.find(v.co)[1]].normal) for v in fur.data.vertices])
meshes=[]
for o in bpy.data.objects:
 if o.type=='MESH' and any(m.type=='ARMATURE' and m.object==rig for m in o.modifiers):
  meshes.append(o);oldNormals=[v.normal.copy() for v in o.data.vertices];oldCoords=[v.co.copy() for v in o.data.vertices]
  if o.data.has_custom_normals:
   for loop,corner in zip(o.data.loops,o.data.corner_normals):oldNormals[loop.vertex_index]=corner.vector.copy()
  for v in o.data.vertices:v.co=warp(v.co)
  o.data.update()
  # Warp imported smooth fiber normals using a local numerical inverse-transpose Jacobian.
  if o.name.startswith('Jimothy_Fur'):
   out=[];eps=.0001
   for p,n in zip(oldCoords,oldNormals):
    cols=[(warp(p+Vector(axis)*eps)-warp(p-Vector(axis)*eps))/(2*eps) for axis in [(1,0,0),(0,1,0),(0,0,1)]]
    jac=Matrix(cols).transposed();out.append(tuple((jac.inverted().transposed()@n).normalized()))
   o.data.normals_split_custom_set_from_vertices(out)
bpy.context.view_layer.objects.active=rig;bpy.ops.object.mode_set(mode='EDIT')
for b in rig.data.edit_bones:b.head=warp(b.head);b.tail=warp(b.tail)
bpy.ops.object.mode_set(mode='OBJECT')
import refine_gait_v5,refine_gait_v9,refine_jump_v9
from gait_kinematics_v9 import _solve
refine_gait_v5._solve=_solve
base=refine_gait_v5.refine(rig);run=refine_gait_v9.refine(rig);jump=refine_jump_v9.refine(rig)
assert run['max_reach_clamp']<.001 and run['max_solver_error']<.001,run
assert jump['max_target_clamp']<.001 and jump['max_solver_error']<.001,jump
rig.animation_data.action=bpy.data.actions['Idle'];bpy.context.scene.frame_set(1)
bpy.ops.wm.save_as_mainfile(filepath=str(root/'Art/Blender/Jimothy-v9.blend'),compress=True)
report={'source':'Jimothy-v8.blend','skin_and_fur_warped_together':True,'new_bones':0,'base_regeneration':base,'run':run,'jump':jump,'vertices':sum(len(o.data.vertices) for o in meshes),'triangles':sum(sum(len(p.vertices)-2 for p in o.data.polygons) for o in meshes)}
(root/'Documentation/character-v9.json').write_text(json.dumps(report,indent=2));print('V9_READY',report)
