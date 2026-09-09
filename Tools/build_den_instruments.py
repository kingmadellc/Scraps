"""Original unbranded single-cut electric guitar and worn combo amp; Blender background.
Build/export only these two assets. No downloaded meshes, logos or texture sources.
"""
import bpy,math,json,sys
from pathlib import Path
from mathutils import Vector
import numpy as np
ROOT=Path(__file__).resolve().parents[1];OUT=ROOT/'Unity/Assets/Jimothy/Resources/DenAssets';OUT.mkdir(parents=True,exist_ok=True)
sys.path.insert(0,str(ROOT/'Tools'))
from blender_common import reset,mat,mesh,bar,stage
reset();parts=[]
def cube(name,p,size,m,bevel=.003):
 o=mesh(name,p,tuple(x*.5 for x in size),m,bevel=bevel);parts.append(o);return o
def ell(name,p,size,m):
 bpy.ops.mesh.primitive_uv_sphere_add(segments=12,ring_count=8,location=p);o=bpy.context.object;o.name=name;o.scale=size;bpy.ops.object.transform_apply(location=False,rotation=False,scale=True);o.data.materials.append(m)
 for face in o.data.polygons:face.use_smooth=True
 parts.append(o);return o
def tube(name,a,b,r,m):
 o=bar(name,a,b,r,m);parts.append(o);return o
def plate(name,outline,depth,y,m,bevel=.003,straight=False):
 c=bpy.data.curves.new(name,'CURVE');c.dimensions='2D';c.resolution_u=5;c.fill_mode='BOTH';c.extrude=depth*.5;c.bevel_depth=bevel;c.bevel_resolution=3
 s=c.splines.new('BEZIER');s.bezier_points.add(len(outline)-1)
 for pt,xy in zip(s.bezier_points,outline):pt.co=(xy[0],xy[1],0);pt.handle_left_type=pt.handle_right_type=('VECTOR' if straight else 'AUTO')
 s.use_cyclic_u=True;o=bpy.data.objects.new(name,c);bpy.context.collection.objects.link(o);o.rotation_euler.x=math.pi*.5;o.location.y=y;c.materials.append(m);parts.append(o);return o
def textured(name,base,kind,rough):
 m=mat(name,base,0,rough);n=512;yy,xx=np.mgrid[0:n,0:n];rng=np.random.default_rng(44);noise=rng.random((n,n))
 if kind=='wood':f=.93+.025*np.sin(xx*.09+np.sin(yy*.008)*2)+.008*np.sin(xx*.49+np.sin(yy*.031))+.012*noise
 elif kind=='cloth':f=.54+.25*((xx%8)<4)+.17*((yy%8)<4)+noise*.08
 else:f=.73+.13*noise+.08*np.sin(xx*.49)*np.sin(yy*.61)+.045*np.sin(xx*.17+yy*.2)
 rgba=np.ones((n,n,4),dtype=np.float32);rgba[:,:,:3]=np.array(base)*f[:,:,None]
 im=bpy.data.images.new(name+'_BaseColor',width=n,height=n);im.pixels.foreach_set(rgba.ravel());im.filepath_raw=str(OUT/(name+'_BaseColor.png'));im.file_format='PNG';im.save();im.pack()
 nd=m.node_tree.nodes.new('ShaderNodeTexImage');nd.image=im;m.node_tree.links.new(nd.outputs['Color'],m.node_tree.nodes['Principled BSDF'].inputs['Base Color']);m.diffuse_color=(1,1,1,1);return m
wood=textured('Den_Maple',(.78,.55,.25),'wood',.30);bodymat=textured('Den_Butterscotch',(.75,.51,.18),'wood',.27)
tolex=textured('Den_CharcoalTolex',(.11,.105,.09),'tolex',.73);cloth=textured('Den_WovenGrille',(.45,.39,.28),'cloth',.88)
chrome=mat('Den_BrushedNickel',(.53,.56,.57),.82,.25);black=mat('Den_BlackPlastic',(.018,.021,.023),0,.34);ivory=mat('Den_AgedIvory',(.71,.66,.48),0,.4);rubber=mat('Den_Rubber',(.025,.027,.023),0,.92);copper=mat('Den_PatinaBrass',(.38,.25,.10),.7,.42)
def guitar():
 parts.clear()
 outline=[(-.04,.412),(-.08,.426),(-.14,.402),(-.169,.35),(-.171,.29),(-.15,.23),(-.166,.17),(-.177,.105),(-.16,.041),(-.105,.009),(0,0),(.103,.012),(.157,.055),(.17,.117),(.143,.194),(.112,.247),(.136,.31),(.130,.350),(.101,.364),(.075,.339),(.064,.297),(.038,.300),(.030,.410)]
 plate('Single-cut contoured solid body',outline,.040,0,bodymat,.005)
 # Separate fingerboard and rounded neck back, tapered toward the nut.
 neck=[(-.029,.365),(-.029,.47),(-.023,.87),(.023,.87),(.029,.47),(.029,.365)]
 plate('Maple rounded neck',neck,.020,.001,wood,.004,True);plate('Maple fingerboard',neck,.003,-.025,wood,.001,True)
 head=[(-.022,.863),(-.028,.884),(-.033,.967),(-.024,1.065),(.018,1.089),(.045,1.069),(.058,1.018),(.052,.914),(.026,.873)]
 plate('Asymmetric inline headstock',head,.018,.003,wood,.003)
 pickguard=[(-.029,.398),(-.112,.389),(-.138,.339),(-.124,.279),(-.136,.227),(-.125,.205),(-.057,.213),(-.048,.302),(.044,.315),(.071,.272),(.088,.302),(.071,.348),(.047,.363),(.027,.399)]
 plate('Unbranded black pickguard',pickguard,.002,-.027,black,.0008)
 # Six-saddle bridge and its slanted bridge pickup.
 cube('Stamped steel bridge plate',(0,-.029,.224),(.086,.004,.134),chrome,.003)
 for i in range(6):
  x=(i-2.5)*.0102;tube('Individual barrel saddle',(x-.0047,-.040,.190),(x+.0047,-.040,.190),.004,chrome)
  tube('Saddle adjustment screw',(x,-.036,.160),(x,-.036,.179),.0012,chrome)
 bridge=cube('Angled bridge pickup',(0,-.038,.266),(.075,.012,.017),black,.004);bridge.rotation_euler.y=.24
 cube('Chrome neck pickup',(0,-.033,.356),(.064,.012,.018),chrome,.007)
 for i in range(6):ell('Bridge pickup pole',((i-2.5)*.011,-.045,.266-(i-2.5)*.0025),(.002,.0015,.002),chrome)
 # Tele-style separate control plate, two knurled knobs and selector tip.
 plate('Rounded control plate',[(.087,.152),(.087,.281),(.108,.294),(.127,.281),(.127,.152),(.108,.139)],.003,-.026,chrome,.001)
 for z in [.167,.236]:
  tube('Metal control knob',(.107,-.031,z),(.107,-.051,z),.012,chrome)
  for i in range(16):a=i*math.tau/16;tube('Knob knurl',(.107+math.sin(a)*.0122,-.032,z+math.cos(a)*.0122),(.107+math.sin(a)*.0122,-.050,z+math.cos(a)*.0122),.00045,black)
 tube('Pickup selector lever',(.107,-.034,.275),(.107,-.056,.283),.0016,chrome);ell('Selector tip',(.107,-.058,.285),(.004,.004,.007),ivory)
 cube('Bone nut',(0,-.029,.872),(.047,.008,.004),ivory,.0005)
 # Equal-tempered fret positions from a 0.682m speaking string length.
 for fret in range(1,23):
  z=.872-.682*(1-2**(-fret/12));width=.023+(.872-z)/(.872-.365)*.006
  tube('Fret %02d'%fret,(-width,-.0295,z),(width,-.0295,z),.0008,chrome)
 for fret in [3,5,7,9,12,15,17,19,21]:
  za=.872-.682*(1-2**(-fret/12));zb=.872-.682*(1-2**(-(fret-1)/12));z=(za+zb)/2
  for x in ([-.009,.009] if fret==12 else [0]):ell('Fingerboard position marker',(x,-.0294,z),(.0028,.0005,.0028),black)
 for i in range(6):
  x=(i-2.5)*.0102;nx=(i-2.5)*.0074;hz=.910+i*.027
  tube('Steel string %d'%(i+1),(x,-.046,.184),(nx,-.034,.875),.00035+i*.000055,chrome)
  tube('String beyond nut',(nx,-.034,.875),(.036,-.012,hz),.00035+i*.000055,chrome)
  tube('Tuner capstan',(.036,-.012,hz),(.036,-.020,hz),.004,chrome)
  tube('Inline tuner shaft',(.040,.005,hz),(.068,.005,hz),.003,chrome)
  ell('Inline tuner button',(.074,.005,hz),(.010,.005,.009),chrome)
 for x,z in [(-.10,.375),(-.127,.285),(-.095,.215),(.056,.33),(.04,.381)]:ell('Pickguard screw',(x,-.030,z),(.002,.001,.002),chrome)
 tube('Lower strap button',(0,.0,-.003),(0,.0,-.009),.006,chrome)
 # Side-mounted output jack hardware on the lower treble edge.
 ell('Output jack cup',(.163,0,.105),(.003,.011,.012),chrome);ell('Output jack socket',(.166,0,.105),(.0015,.005,.005),black)
 return finish('Guitar',1.10)
def amp():
 parts.clear()
 # Open-front cabinet construction makes the grille visibly recessed behind the rounded frame.
 for x in [-.305,.305]:cube('Rounded tolex cabinet side',(x,0,.276),(.04,.30,.50),tolex,.010)
 for z in [.046,.506]:cube('Rounded tolex cabinet rail',(0,0,z),(.61,.30,.04),tolex,.010)
 cube('Rear cabinet panel',(0,.139,.276),(.59,.020,.43),tolex,.004)
 cube('Recessed speaker baffle',(0,-.120,.252),(.565,.023,.375),black,.006)
 cube('Woven speaker grille',(0,-.136,.247),(.545,.009,.365),cloth,.004)
 # A subtle piping frame, and four visible retaining screws.
 for x in [-.279,.279]:tube('Grille piping',(x,-.144,.068),(x,-.144,.434),.0035,ivory)
 for z in [.068,.434]:tube('Grille piping',(-.279,-.144,z),(.279,-.144,z),.0035,ivory)
 for x in [-.269,.269]:
  for z in [.078,.424]:ell('Baffle screw',(x,-.149,z),(.0025,.0014,.0025),chrome)
 cube('Recessed amplifier control panel',(0,-.133,.472),(.554,.010,.058),black,.003)
 for i in range(6):
  x=-.19+i*.055;tube('Amp rotary knob',(x,-.140,.473),(x,-.156,.473),.010,ivory)
  cube('Knob indicator',(x,-.158,.479),(.0015,.001,.005),black,.0002)
 for i in range(2):
  x=-.254+i*.023;tube('Input jack socket',(x,-.141,.472),(x,-.147,.472),.005,chrome);ell('Input jack dark hole',(x,-.150,.472),(.0025,.001,.0025),black)
 tube('Power toggle',(.186,-.145,.472),(.186,-.159,.479),.002,chrome);ell('Power pilot jewel',(.224,-.148,.473),(.004,.003,.004),copper)
 # Raised flexible carrying handle, fixed by two nickel brackets.
 for x in [-.095,.095]:cube('Handle foot bracket',(x,0,.531),(.036,.032,.010),chrome,.003)
 path=[(-.095,0,.537),(-.071,0,.561),(-.035,0,.572),(.035,0,.572),(.071,0,.561),(.095,0,.537)]
 for a,b in zip(path,path[1:]):tube('Rubber carrying handle',a,b,.010,rubber)
 for x in [-.305,.305]:
  for y in [-.136,.136]:
   for z in [.042,.510]:
    cube('Folded corner guard front',(x,y+math.copysign(.012,y),z),(.045,.008,.045),chrome,.006)
    cube('Folded corner guard top',(x,y,z+(.015 if z>.2 else -.015)),(.045,.043,.008),chrome,.006)
    cube('Folded corner guard side',(x+math.copysign(.015,x),y,z),(.008,.043,.045),chrome,.006)
 for x in [-.255,.255]:
  for y in [-.10,.10]:cube('Rubber cabinet foot',(x,y,.014),(.056,.047,.028),rubber,.006)
 # Rear ventilation reveals a shallow dark service recess without adding logos.
 for z in [.28,.31,.34]:cube('Rear ventilation slot',(0,.151,z),(.39,.006,.014),black,.004)
 return finish('Amp')
def finish(name,height=None):
 bpy.ops.object.select_all(action='DESELECT')
 for o in parts:o.select_set(True)
 bpy.context.view_layer.objects.active=parts[0];bpy.ops.object.convert(target='MESH');bpy.ops.object.join();o=bpy.context.object;o.name=name
 bpy.context.scene.cursor.location=(0,0,0);bpy.ops.object.origin_set(type='ORIGIN_CURSOR');bpy.ops.object.transform_apply(location=True,rotation=True,scale=True)
 low=min(v.co.z for v in o.data.vertices)
 for v in o.data.vertices:v.co.z-=low
 if height:
  factor=height/max(v.co.z for v in o.data.vertices)
  for v in o.data.vertices:v.co*=factor
 # Cube projection gives all material-image textures real UVs for the FBX import.
 bpy.ops.object.mode_set(mode='EDIT');bpy.ops.mesh.select_all(action='SELECT');bpy.ops.uv.cube_project(cube_size=.18);bpy.ops.object.mode_set(mode='OBJECT')
 # Export normals and bevels are already baked. Keep materials as named slots, one runtime mesh.
 bpy.ops.export_scene.fbx(filepath=str(OUT/(name+'.fbx')),use_selection=True,object_types={'MESH'},axis_forward='-Z',axis_up='Y',apply_unit_scale=True,bake_anim=False,path_mode='COPY',embed_textures=True)
 lo=[min(v.co[i] for v in o.data.vertices) for i in range(3)];hi=[max(v.co[i] for v in o.data.vertices) for i in range(3)];o.data.calc_loop_triangles()
 info={'bounds_blender_min':lo,'bounds_blender_max':hi,'dimensions_blender_xyz':[hi[i]-lo[i] for i in range(3)],'triangles':len(o.data.loop_triangles),'materials':[m.name for m in o.data.materials],'front':'Blender -Y; FBX standard -Z forward, Y up','origin':'ground, body center horizontally'}
 return o,info
g,gi=guitar();a,ai=amp();g.location.x=-.48;a.location.x=.30
stage((2.0,-3.2,1.65),(-.1,0,.55),1.75);s=bpy.context.scene;s.render.resolution_x=1400;s.render.resolution_y=1200;s.cycles.samples=32
floor=mesh('Studio floor',(0,0,-.025),(3,3,.02),mat('StudioFloor',(.09,.095,.10),0,.8))
bpy.ops.wm.save_as_mainfile(filepath=str(ROOT/'Art/Blender/DenInstruments.blend'))
for name,target,camera,ortho in [('den-instruments.png',(-.1,0,.55),(1.3,-3,1.45),1.7),('den-guitar-close.png',(-.48,0,.57),(.28,-2.9,1.17),1.20),('den-amp-close.png',(.30,0,.29),(1.10,-1.8,.87),.90)]:
 s.camera.location=camera;s.camera.rotation_euler=(Vector(target)-s.camera.location).to_track_quat('-Z','Y').to_euler();s.camera.data.ortho_scale=ortho;s.render.filepath=str(ROOT/'Art/Renders'/name);bpy.ops.render.render(write_still=True)
(ROOT/'Documentation/den-instruments.json').write_text(json.dumps({'Guitar':gi,'Amp':ai,'textures':'Authored procedural image textures packed in blend and FBX, PNG copies beside FBX.'},indent=2));print('DEN_INSTRUMENTS_READY',json.dumps({'Guitar':gi,'Amp':ai}))
