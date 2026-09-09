import sys
from pathlib import Path
sys.path.insert(0,str(Path(__file__).resolve().parent))
from blender_common import *

reset()
brick=[mat('Brick '+str(i),c) for i,c in enumerate([(.32,.13,.085),(.44,.24,.12),(.23,.115,.08),(.52,.32,.19)])];stone=mat('Weathered sandstone',(.56,.49,.36));green=mat('Painted evergreen',(.026,.105,.085));teal=mat('Oxidized copper',(.11,.29,.25),.3);glass=mat('Blue smoked windows',(.075,.16,.18),.45,.22);warm=mat('Shop warm interior',(.62,.38,.16));road=mat('Asphalt',(.10,.125,.125));walk=mat('Sidewalk',(.32,.34,.31));iron=mat('Ironwork',(.04,.055,.052),.6);leaf=[mat('Leaves'+str(i),c) for i,c in enumerate([(.08,.20,.09),(.16,.29,.105),(.24,.35,.13)])];wood=mat('Cedar',(.23,.12,.058));white=mat('Linen',(.80,.75,.59));salmon=mat('Salmon paint',(.57,.23,.14));gold=mat('Old brass',(.58,.37,.11),.7)
mesh('COL_Street',(0,0,-.22),(32,53,.22),road)
for side in [-1,1]:
 mesh('COL_Sidewalk',(side*8.4,0,.05),(2.2,48,.12),walk)
 for y in range(-45,46,3):mesh('Paving joint',(side*8.4,y,.177),(2.15,.018,.005),stone)
 for row in range(5):
  x=side*15;y=-32+row*16;h=[8,10.5,7,9,11][row];w=5.1;d=6.7;front=x-side*w
  mesh('COL_Building_%d_%d'%(side,row),(x,y,h/2),(w,d,h/2),brick[(row+(side==1))%4],bevel=.055)
  mesh('COL_Roof',(x,y,h+.12),(w+.18,d+.18,.12),walk)
  for z in [.42,3.35,h-.35,h+.35]:mesh('Cornice',(x,y,z),(w+.23,d+.23,.13),stone,bevel=.03)
  for yy in [y-d,y+d]:mesh('Parapet',(x,yy,h+.58),(w+.12,.14,.40),brick[row%4])
  mesh('Parapet back',(x+side*w,y,h+.58),(.14,d,.40),brick[row%4])
  for yy in [y-4.8,y-1.6,y+1.6,y+4.8]:
   for z in [1.85]+list(range(5,int(h),3)):
    mesh('Window surround',(front-side*.07,yy,z),(.10,1.03,1.10),stone,bevel=.03)
    mesh('Window',(front-side*.18,yy,z),(.055,.84,.92),warm if z<2 else glass)
    mesh('Window mullion',(front-side*.25,yy,z),(.045,.035,.96),green)
    mesh('Window transom',(front-side*.25,yy,z),(.045,.86,.035),green)
    if z>2:mesh('Window sill',(front-side*.29,yy,z-1.02),(.30,1.10,.09),stone)
   mesh('Awning',(front-side*.70,yy,3.05),(.85,1.19,.085),green if row%2 else salmon).rotation_euler[1]=side*.18
   for s in [-.8,-.4,0,.4,.8]:mesh('Awning stripe',(front-side*.72,yy+s,3.12),(.84,.075,.015),white).rotation_euler[1]=side*.18
  # Visible brick courses on street side; merged at export below by material.
  for z in [i*.28+.65 for i in range(int((h-.9)/.28))]:
   for yy in [y-d+.3+i*.65+(int(z/.28)%2)*.3 for i in range(20)]:
    if any(abs(yy-v)<1.1 and any(abs(z-vz)<1.15 for vz in [1.85]+list(range(5,int(h),3))) for v in [y-4.8,y-1.6,y+1.6,y+4.8]):continue
    mesh('Brick face',(front-side*.008,yy,z),(.016,.29,.12),brick[(int(z*10)+int(yy))%4])
  name=['SALMON & SONS','BALLARD AVENUE','THE OLD NET LOFT','CEDAR & COFFEE','DOCKSIDE GOODS'][row]
  text3('Storefront sign',name,(front-side*.29,y,3.72),.32,white,(math.pi/2,0,-side*math.pi/2))
  # Readable vertical route along outer alley: bin -> crates -> AC -> balcony -> roof.
  route_y=y+d+1.05
  for j in range(7):
   px=x-side*4+j*side*(8/6);pz=.65+j*(h-.65)/6
   mesh('COL_Climb_%d_%d_%d'%(side,row,j),(px,route_y,pz/2),(1.0,.9,pz/2),teal if j%2 else wood,bevel=.06)
  mesh('COL_RoofStep',(x+side*4,route_y,h-.6),(1.2,1.0,.2),iron)
  mesh('COL_AC',(x-1,y+1,h+.8),(1.0,1.25,.6),teal,bevel=.09)
  for j in range(9):mesh('AC grille',(x-2.01,y+.05+j*.23,h+.8),(.02,.05,.42),iron)
  mesh('Chimney',(x+2,y-3,h+.8),(.55,.55,.8),brick[0])
  for yy in [y-3,y+3]:
   mesh('COL_Balcony',(front-side*.8,yy,5.5),(.9,1.4,.12),iron)
   for dy in [-1.3,-.65,0,.65,1.3]:bar('Balcony rail',(front-side*1.55,yy+dy,5.5),(front-side*1.55,yy+dy,6.5),.025,iron)
   bar('Balcony handrail',(front-side*1.55,yy-1.4,6.5),(front-side*1.55,yy+1.4,6.5),.04,iron)
 for yy in range(-40,41,10):
  x=side*7.7;mesh('Tree grate',(x,yy,.20),(.7,.7,.035),iron)
  bar('Tree trunk',(x,yy,.2),(x+.2,yy,4.8),.16,wood)
  for i in range(5):mesh('Tree canopy',(x+random.uniform(-1,1),yy+random.uniform(-1,1),4.6+random.uniform(0,1.4)),(1.4,1.35,1.2),leaf[i%3],'ico')
  bar('Streetlamp',(side*6.35,yy+3,.2),(side*6.35,yy+3,4),.065,iron);mesh('Lantern',(side*6.35,yy+3,4.1),(.20,.20,.3),warm,bevel=.05)
  mesh('Planter',(side*9,yy+4,.5),(.55,.85,.32),teal,bevel=.06)
  for k in range(6):mesh('Fern',(side*9+random.uniform(-.3,.3),yy+4+random.uniform(-.6,.6),.9),(.35,.15,.36),leaf[k%3],'ico')
# Cross-alley clothesline platform routes.
for y in [-17,15]:
 bar('Clothesline',(-9,y,6),(9,y,6),.025,iron)
 for x in range(-6,7,2):mesh('Laundry',(x,y,5.6),(.42,.035,.42),white if x%3 else salmon)
 mesh('COL_Balance rail',(0,y,6),(9,.10,.07),wood)
# Fixed den in the near pocket park.
mesh('COL_Den platform',(-4,-43,.3),(2.6,2,.3),wood,bevel=.07)
mesh('Den back',(-4,-44.8,1.3),(2.6,.12,.9),green)
mesh('Den canopy',(-4,-43,2.5),(2.9,2.3,.12),teal,bevel=.05)
for x in [-6.4,-1.6]:bar('Den post',(x,-41.2,.4),(x,-41.2,2.5),.07,wood)
text3('Den sign','THE TRASH PALACE',(-4,-41,2.05),.26,white,(math.pi/2,0,0))
# Marvin's Garden bell inspired landmark, approximate not a survey.
for x in [-2,2]:bar('Bell tower',(x,43,.1),(x,43,8),.16,iron)
bar('Bell lintel',(-2.2,43,7.5),(2.2,43,7.5),.16,iron);mesh('Historic bell',(0,43,6.7),(.85,.85,.8),gold,'sphere');text3('Bell plaque','MARVIN\'S GARDEN',(0,42.8,2),.3,stone,(math.pi/2,0,0))
# Convert text; keep collision volumes separate and combine decoration by material.
for o in list(bpy.context.scene.objects):
 if o.type=='FONT':bpy.ops.object.select_all(action='DESELECT');o.select_set(True);bpy.context.view_layer.objects.active=o;bpy.ops.object.convert(target='MESH')
groups={}
for o in list(bpy.context.scene.objects):
 if o.type=='MESH' and not o.name.startswith('COL_'):groups.setdefault(o.data.materials[0].name,[]).append(o)
for name,objects in groups.items():
 bpy.ops.object.select_all(action='DESELECT')
 for o in objects:o.select_set(True)
 bpy.context.view_layer.objects.active=objects[0];bpy.ops.object.convert(target='MESH');bpy.ops.object.join();bpy.context.object.name='Decor_'+name
export('OldBallard')
stage((42,-58,39),(0,-3,3),72)
s=bpy.context.scene
sun_data=bpy.data.lights.new('Summer sun','SUN');sun_data.energy=3;sun_data.angle=.12;sun_data.color=(1,.75,.52);sun=bpy.data.objects.new('Summer sun',sun_data);bpy.context.collection.objects.link(sun);sun.rotation_euler=(.5,-.4,-.7)
render('old-ballard-block.png',2560,1440)
print('JIMOTHY_ASSET_BUILD_COMPLETE')
