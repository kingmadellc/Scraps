"""Run with Blender 4.5+: blender -b --python Tools/build_assets.py.
Original prototype meshes; source photographs are research only and are not packaged.
"""
import bpy, math, random, json
from pathlib import Path
from mathutils import Vector
ROOT=Path(__file__).resolve().parents[1]
OUT=ROOT/'Unity/Assets/Jimothy/Resources'
random.seed(41)
def reset():
 bpy.ops.object.select_all(action='SELECT'); bpy.ops.object.delete(use_global=False)
 for d in list(bpy.data.materials): bpy.data.materials.remove(d)
def mat(name,color,metal=0,rough=.65):
 m=bpy.data.materials.new(name); m.diffuse_color=(*color,1); m.use_nodes=True
 p=m.node_tree.nodes.get('Principled BSDF'); p.inputs['Base Color'].default_value=(*color,1); p.inputs['Roughness'].default_value=rough; p.inputs['Metallic'].default_value=metal
 return m
def mesh(name,loc,scale,m,kind='cube',bevel=0):
 if kind=='sphere': bpy.ops.mesh.primitive_uv_sphere_add(segments=24,ring_count=16,location=loc)
 elif kind=='ico': bpy.ops.mesh.primitive_ico_sphere_add(subdivisions=2,location=loc)
 elif kind=='cylinder': bpy.ops.mesh.primitive_cylinder_add(vertices=16,radius=1,depth=2,location=loc)
 else:
  verts=[(x*scale[0],y*scale[1],z*scale[2]) for x,y,z in [(-1,-1,-1),(-1,-1,1),(-1,1,-1),(-1,1,1),(1,-1,-1),(1,-1,1),(1,1,-1),(1,1,1)]]
  data=bpy.data.meshes.new(name);data.from_pydata(verts,[],[(0,4,6,2),(1,3,7,5),(0,1,5,4),(2,6,7,3),(0,2,3,1),(4,5,7,6)]);data.update()
  o=bpy.data.objects.new(name,data);bpy.context.collection.objects.link(o);o.location=loc;o.data.materials.append(m)
  if bevel:
   b=o.modifiers.new('Soft edges','BEVEL');b.width=bevel;b.segments=2
  return o
 o=bpy.context.object; o.name=name; o.scale=scale; bpy.ops.object.transform_apply(location=False,rotation=False,scale=True); o.data.materials.append(m)
 if kind in ('sphere','cylinder'): 
  for p in o.data.polygons:p.use_smooth=True
 if bevel:
  b=o.modifiers.new('Soft manufactured edges','BEVEL'); b.width=bevel;b.segments=2
 return o
def bar(name,a,b,r,m):
 o=mesh(name,(Vector(a)+Vector(b))/2,(r,r,(Vector(b)-Vector(a)).length/2),m,'cylinder');o.rotation_euler=(Vector(b)-Vector(a)).to_track_quat('Z','Y').to_euler();return o
def text3(name,string,loc,size,m,rot=(math.pi/2,0,math.pi/2)):
 c=bpy.data.curves.new(name,'FONT');c.body=string;c.align_x='CENTER';c.size=size;c.extrude=.004
 o=bpy.data.objects.new(name,c);bpy.context.collection.objects.link(o);o.location=loc;o.rotation_euler=rot;o.data.materials.append(m);return o
def export(name,selected=False,anim=False):
 bpy.ops.export_scene.fbx(filepath=str(OUT/(name+'.fbx')),use_selection=selected,object_types={'MESH','ARMATURE'},add_leaf_bones=False,axis_forward='-Z',axis_up='Y',bake_anim=anim,bake_anim_use_all_actions=True,bake_anim_use_nla_strips=False,path_mode='AUTO')
def stage(camera,target,ortho=0):
 w=bpy.data.worlds.new('Puget Sound atmosphere');bpy.context.scene.world=w;w.use_nodes=True;w.node_tree.nodes['Background'].inputs[0].default_value=(.13,.23,.26,1);w.node_tree.nodes['Background'].inputs[1].default_value=.5
 for name,loc,power,size,col in [('Golden hour',(5,-5,8),1500,6,(1,.73,.48)),('Sky fill',(-5,-2,5),1100,5,(.49,.74,1)),('Rim',(1,5,6),1900,4,(1,.52,.29))]:
  d=bpy.data.lights.new(name,'AREA');d.energy=power;d.shape='DISK';d.size=size;d.color=col;o=bpy.data.objects.new(name,d);bpy.context.collection.objects.link(o);o.location=loc;o.rotation_euler=(Vector(target)-o.location).to_track_quat('-Z','Y').to_euler()
 d=bpy.data.cameras.new('Review camera');o=bpy.data.objects.new('Review camera',d);bpy.context.collection.objects.link(o);o.location=camera;o.rotation_euler=(Vector(target)-o.location).to_track_quat('-Z','Y').to_euler();bpy.context.scene.camera=o
 if ortho:d.type='ORTHO';d.ortho_scale=ortho
 s=bpy.context.scene;s.render.engine='CYCLES';s.cycles.samples=24;s.cycles.use_denoising=True;s.render.resolution_percentage=100;s.view_settings.view_transform='AgX'
def render(name,x=1920,y=1080):
 s=bpy.context.scene;s.render.resolution_x=x;s.render.resolution_y=y;s.render.filepath=str(ROOT/'Art/Renders'/name);bpy.ops.wm.save_as_mainfile(filepath=str(ROOT/'Art/Blender'/('Jimothy.blend' if 'jimothy' in name else 'OldBallard.blend')));bpy.ops.render.render(write_still=True)