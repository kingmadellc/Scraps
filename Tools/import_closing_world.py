"""Import the exact Unity-exported geometry into an editable, packed Blender scene.
Run Blender --background --python Tools/import_closing_world.py.
"""
import bpy, pathlib, math, json
from mathutils import Vector
ROOT=pathlib.Path(__file__).resolve().parents[1]
SOURCE=ROOT/'Art/Exports/ClosingTime'
DEST=ROOT/'Art/Blender/Old-Ballard-Closing-Time.blend'
bpy.ops.object.select_all(action='SELECT');bpy.ops.object.delete(use_global=False)
materials={}; current=None
for line in (SOURCE/'ClosingTime.mtl').read_text().splitlines():
 p=line.split()
 if not p:continue
 if p[0]=='newmtl':
  current=bpy.data.materials.new(p[1]);current.use_nodes=True;materials[p[1]]=current
 elif current:
  bs=current.node_tree.nodes.get('Principled BSDF')
  if p[0]=='Kd':bs.inputs['Base Color'].default_value=(*map(float,p[1:4]),1)
  elif p[0]=='Pr':bs.inputs['Roughness'].default_value=float(p[1])
  elif p[0]=='Pm':bs.inputs['Metallic'].default_value=float(p[1])
  elif p[0]=='Ke':
   bs.inputs['Emission Color'].default_value=(*map(float,p[1:4]),1);bs.inputs['Emission Strength'].default_value=1
  elif p[0]=='map_Kd':
   node=current.node_tree.nodes.new('ShaderNodeTexImage');node.image=bpy.data.images.load(str(SOURCE/p[-1]),check_existing=True)
   # Multiply albedo by Unity's material tint, retaining original runtime appearance.
   tint=tuple(bs.inputs['Base Color'].default_value);mix=current.node_tree.nodes.new('ShaderNodeMixRGB');mix.blend_type='MULTIPLY';mix.inputs[0].default_value=1;mix.inputs[2].default_value=tint
   current.node_tree.links.new(node.outputs['Color'],mix.inputs[1]);current.node_tree.links.new(mix.outputs[0],bs.inputs['Base Color'])
# Supplement MTL with exact Unity UV transform and original uncompressed-channel PBR sources.
metadata=SOURCE/'ClosingTime.materials.json'
if metadata.exists():
 for entry in json.loads(metadata.read_text())['materials']:
  material=materials[entry['name']];tree=material.node_tree;bs=tree.nodes.get('Principled BSDF')
  uv=tree.nodes.new('ShaderNodeTexCoord');mapping=tree.nodes.new('ShaderNodeMapping');mapping.inputs['Scale'].default_value=(entry['scaleX'],entry['scaleY'],1);mapping.inputs['Location'].default_value=(entry['offsetX'],entry['offsetY'],0);tree.links.new(uv.outputs['UV'],mapping.inputs['Vector'])
  for node in list(tree.nodes):
   if node.type=='TEX_IMAGE':tree.links.new(mapping.outputs['Vector'],node.inputs['Vector'])
  def data_texture(filename):
   node=tree.nodes.new('ShaderNodeTexImage');node.image=bpy.data.images.load(str(SOURCE/filename),check_existing=True);node.image.colorspace_settings.name='Non-Color';tree.links.new(mapping.outputs['Vector'],node.inputs['Vector']);return node
  if entry.get('normalFile'):
   texture=data_texture(entry['normalFile']);normal=tree.nodes.new('ShaderNodeNormalMap');normal.inputs['Strength'].default_value=entry['normalStrength'];tree.links.new(texture.outputs['Color'],normal.inputs['Color']);tree.links.new(normal.outputs['Normal'],bs.inputs['Normal'])
  if entry.get('roughnessFile'):
   texture=data_texture(entry['roughnessFile']);scale=entry['smoothnessMultiplier']
   # URP uses smoothness=(1-roughness)*_Smoothness. Blender expects its complement.
   mathnode=tree.nodes.new('ShaderNodeMath');mathnode.operation='MULTIPLY_ADD';mathnode.inputs[1].default_value=scale;mathnode.inputs[2].default_value=1-scale
   tree.links.new(texture.outputs['Color'],mathnode.inputs[0]);tree.links.new(mathnode.outputs[0],bs.inputs['Roughness'])
vertices=[];uvs=[];normals=[];groups=[];group=None;mat=None
for line in (SOURCE/'ClosingTime.obj').read_text().splitlines():
 p=line.split()
 if not p:continue
 if p[0]=='v':vertices.append(tuple(map(float,p[1:4])))
 elif p[0]=='vt':uvs.append(tuple(map(float,p[1:3])))
 elif p[0]=='vn':normals.append(tuple(map(float,p[1:4])))
 elif p[0]=='o':group={'name':p[1],'faces':[]};groups.append(group)
 elif p[0]=='usemtl':mat=p[1]
 elif p[0]=='f':group['faces'].append(([(int(v.split('/')[0])-1,int(v.split('/')[1])-1,int(v.split('/')[2])-1) for v in p[1:]],mat))
for group in groups:
 used=sorted({v[0] for face,_ in group['faces'] for v in face});mapping={v:i for i,v in enumerate(used)}
 mesh=bpy.data.meshes.new(group['name']);mesh.from_pydata([vertices[i] for i in used],[],[[mapping[v[0]] for v in face] for face,_ in group['faces']]);mesh.update()
 obj=bpy.data.objects.new(group['name'],mesh);bpy.context.collection.objects.link(obj)
 slots={}
 for _,name in group['faces']:
  if name not in slots:slots[name]=len(slots);mesh.materials.append(materials[name])
 layer=mesh.uv_layers.new(name='Unity UV');custom=[]
 for poly,(face,name) in zip(mesh.polygons,group['faces']):
  poly.material_index=slots[name]
  for loop,v in zip(poly.loop_indices,face):layer.data[loop].uv=uvs[v[1]];custom.append(normals[v[2]])
 mesh.normals_split_custom_set(custom)
 obj['source']='Exact Unity ClosingTimeWorld native mesh; no generated-image geometry'
scene=bpy.context.scene;scene.render.engine='CYCLES';scene.cycles.samples=32
scene.world.color=(.04,.06,.10)
def area(name,pos,color,power,size):
 light=bpy.data.lights.new(name,'AREA');light.energy=power;light.color=color;light.shape='DISK';light.size=size
 obj=bpy.data.objects.new(name,light);scene.collection.objects.link(obj);obj.location=pos;obj.rotation_euler=(Vector((0,5,0))-obj.location).to_track_quat('-Z','Y').to_euler()
area('Cool moon fill',(10,10,32),(.4,.58,1),3500,40)
for side in (-1,1):
 for z in (-31,-15,1,17,33):
  data=bpy.data.lights.new('Warm street lantern','POINT');data.energy=110;data.color=(1,.55,.22);data.shadow_soft_size=.3
  obj=bpy.data.objects.new('Warm street lantern',data);scene.collection.objects.link(obj);obj.location=(side*5.95,z,4.12)
camdata=bpy.data.cameras.new('Street review');cam=bpy.data.objects.new('Street review',camdata);scene.collection.objects.link(cam);cam.location=(0,42,3.5);cam.rotation_euler=(Vector((-3,-7,3))-cam.location).to_track_quat('-Z','Y').to_euler();camdata.lens=24;scene.camera=cam
scene.render.resolution_x=1600;scene.render.resolution_y=900;scene.render.resolution_percentage=100
scene['source_mesh_count']=len(groups);scene['source_vertex_count']=len(vertices)
scene['scope']='Editable runtime architecture and props with packed surface maps. UI, character, NPCs, text signs and Unity postprocessing are separate runtime systems.'
for image in bpy.data.images:
 if image.source=='FILE':image.pack()
DEST.parent.mkdir(parents=True,exist_ok=True)
bpy.ops.wm.save_as_mainfile(filepath=str(DEST),compress=True)
(DEST.parent/'closing-time-blender-report.json').write_text(json.dumps({'source_meshes':len(groups),'blender_meshes':sum(o.type=='MESH' for o in scene.objects),'source_vertices':len(vertices),'blender_vertices':sum(len(o.data.vertices) for o in scene.objects if o.type=='MESH'),'packed_images':sum(bool(i.packed_file) for i in bpy.data.images),'path':str(DEST)},indent=2))
print('CLOSING_TIME_BLEND_SAVED',DEST,'objects',len(groups),'vertices',len(vertices))
