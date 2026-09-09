"""Original rigged neighborhood human and dog assets. Blender4.5 background builder.
Continuous garment/coat meshes, anatomical limb chains and planted in-place clips.
No downloaded meshes/textures; reference proportions are authored here in metres.
"""
import bpy,sys,math,json
from pathlib import Path
from mathutils import Vector,Matrix
sys.path.insert(0,str(Path(__file__).resolve().parent))
from blender_common import reset,mat,mesh,bar,stage,ROOT,OUT

REPORT={}

def ell(name,p,r,material):
    return mesh(name,p,r,material,'sphere')

def limb(name,a,b,r1,r2,material):
    a,b=Vector(a),Vector(b);axis=(b-a).normalized();u=axis.cross(Vector((0,1,0)))
    if u.length<.01:u=axis.cross(Vector((1,0,0)))
    u.normalize();v=axis.cross(u);verts=[];faces=[];steps=16
    for t,factor in [(0,.80),(.15,1.0),(.5,.96),(.85,.78),(1,.68)]:
        c=a+(b-a)*t
        for j in range(steps):
            ph=j*math.tau/steps;verts.append(c+u*r1*factor*math.cos(ph)+v*r2*factor*math.sin(ph))
    for k in range(4):
        for j in range(steps):faces.append((k*steps+j,k*steps+(j+1)%steps,(k+1)*steps+(j+1)%steps,(k+1)*steps+j))
    faces.extend([tuple(reversed(range(steps))),tuple(range(4*steps,5*steps))])
    d=bpy.data.meshes.new(name);d.from_pydata(verts,[],faces);d.update();o=bpy.data.objects.new(name,d);bpy.context.collection.objects.link(o);d.materials.append(material)
    for p in d.polygons:p.use_smooth=True
    return o

def fuse(name,parts,voxel=.012,ratio=.45):
    bpy.ops.object.select_all(action='DESELECT')
    for o in parts:o.select_set(True)
    bpy.context.view_layer.objects.active=parts[0];bpy.ops.object.join();o=bpy.context.object;o.name=name
    bpy.ops.object.transform_apply(location=True,rotation=True,scale=True)
    m=o.modifiers.new('Continuous anatomy','REMESH');m.mode='VOXEL';m.voxel_size=voxel;m.use_smooth_shade=True;bpy.ops.object.modifier_apply(modifier=m.name)
    m=o.modifiers.new('Surface relax','SMOOTH');m.factor=.65;m.iterations=3;bpy.ops.object.modifier_apply(modifier=m.name)
    m=o.modifiers.new('Mobile surface','DECIMATE');m.ratio=ratio;bpy.ops.object.modifier_apply(modifier=m.name)
    for p in o.data.polygons:p.use_smooth=True
    return o

def texture_surface(o,material,name,scale):
    bpy.ops.object.select_all(action='DESELECT');o.select_set(True);bpy.context.view_layer.objects.active=o
    bpy.ops.object.mode_set(mode='EDIT');bpy.ops.mesh.select_all(action='SELECT');bpy.ops.uv.smart_project(angle_limit=1.15,island_margin=.008);bpy.ops.object.mode_set(mode='OBJECT')
    nt=material.node_tree;p=nt.nodes.get('Principled BSDF');out=nt.nodes.get('Material Output');base=tuple(material.diffuse_color[:3])
    texcoord=nt.nodes.new('ShaderNodeTexCoord');noise=nt.nodes.new('ShaderNodeTexNoise');noise.inputs['Scale'].default_value=scale;noise.inputs['Detail'].default_value=2
    nt.links.new(texcoord.outputs['Generated'],noise.inputs['Vector']);ramp=nt.nodes.new('ShaderNodeValToRGB');ramp.color_ramp.elements[0].color=(*[x*.65 for x in base],1);ramp.color_ramp.elements[1].color=(*[min(1,x*1.17) for x in base],1);nt.links.new(noise.outputs['Fac'],ramp.inputs[0])
    em=nt.nodes.new('ShaderNodeEmission');nt.links.new(ramp.outputs['Color'],em.inputs['Color']);nt.links.new(em.outputs[0],out.inputs['Surface'])
    im=bpy.data.images.new(name,width=1024,height=1024);target=nt.nodes.new('ShaderNodeTexImage');target.image=im;nt.nodes.active=target
    bpy.context.scene.render.engine='CYCLES';bpy.context.scene.cycles.samples=1;bpy.ops.object.bake(type='EMIT',margin=8)
    im.filepath_raw=str(OUT/(name+'.png'));im.file_format='PNG';im.save();im.pack();nt.links.new(p.outputs[0],out.inputs['Surface']);nt.links.new(target.outputs['Color'],p.inputs['Base Color']);material.diffuse_color=(1,1,1,1)

def ringform(name,rings,material):
    verts=[];faces=[];n=24
    for z,rx,ry,y in rings:
        for j in range(n):
            a=j*math.tau/n;verts.append((rx*math.cos(a),y+ry*math.sin(a),z))
    for i in range(len(rings)-1):
        for j in range(n):faces.append((i*n+j,i*n+(j+1)%n,(i+1)*n+(j+1)%n,(i+1)*n+j))
    faces.extend([tuple(reversed(range(n))),tuple(range((len(rings)-1)*n,len(rings)*n))])
    d=bpy.data.meshes.new(name);d.from_pydata(verts,[],faces);d.update();o=bpy.data.objects.new(name,d);bpy.context.collection.objects.link(o);d.materials.append(material)
    for p in d.polygons:p.use_smooth=True
    return o

def rigging(name,bones):
    bpy.ops.object.armature_add();rig=bpy.context.object;rig.name=name+'_Rig';bpy.ops.object.mode_set(mode='EDIT');rig.data.edit_bones.remove(rig.data.edit_bones[0])
    for n,h,t,parent in bones:
        b=rig.data.edit_bones.new(n);b.head=h;b.tail=t
        if parent:b.parent=rig.data.edit_bones[parent]
    bpy.ops.object.mode_set(mode='OBJECT');return rig

def skinmesh(o,rig,weights):
    bpy.ops.object.select_all(action='DESELECT');o.select_set(True);bpy.context.view_layer.objects.active=o
    bpy.ops.object.transform_apply(location=True,rotation=True,scale=True)
    for n in rig.data.bones:o.vertex_groups.new(name=n.name)
    for v in o.data.vertices:
        values=weights(v.co);total=sum(values.values())
        for n,w in values.items():
            if w>1e-5:o.vertex_groups[n].add([v.index],w/total,'REPLACE')
    m=o.modifiers.new('Deform','ARMATURE');m.object=rig;o.parent=rig

def distseg(p,a,b):
    d=b-a;t=max(0,min(1,(p-a).dot(d)/d.length_squared));return (p-a-d*t).length

def nearest_weights(rig,p,names,sharp=100):
    ds={n:distseg(p,rig.data.bones[n].head_local,rig.data.bones[n].tail_local) for n in names}
    near=min(ds.values());return {n:math.exp(-sharp*(d-near)) for n,d in ds.items() if d<near+.15}

def aim(pb,a,b):
    rest=pb.bone;q=(rest.tail_local-rest.head_local).rotation_difference(b-a)
    m=(q.to_matrix()@rest.matrix_local.to_3x3()).to_4x4();m.translation=a;pb.matrix=m
    bpy.context.view_layer.update()

def ik(rig,n,target,pole):
    u,l=rig.pose.bones[n],rig.pose.bones[n+'_shin'];parent=u.parent
    delta=parent.matrix@parent.bone.matrix_local.inverted() if parent else Matrix.Identity(4)
    a=delta@u.bone.head_local;v=target-a;d=max(.00001,min(v.length,u.bone.length+l.bone.length-.00001));direction=v.normalized()
    x=(u.bone.length**2-l.bone.length**2+d*d)/(2*d);h=math.sqrt(max(0,u.bone.length**2-x*x));p=Vector(pole);p-=direction*p.dot(direction)
    elbow=a+direction*x+p.normalized()*h;end=a+direction*d;aim(u,a,elbow);aim(l,elbow,end)
    return (end-target).length

def flat(rig,n,point):
    p=rig.pose.bones[n];m=p.bone.matrix_local.copy();m.translation=point;p.matrix=m;bpy.context.view_layer.update()

def trajectory(t,duty,stride,lift):
    t%=1
    if t<duty:return -stride/2+stride*t/duty,0
    q=(t-duty)/(1-duty);m=stride*(1-duty)/duty
    y=(2*q**3-3*q*q+1)*stride/2+(q**3-2*q*q+q)*m+(-2*q**3+3*q*q)*(-stride/2)+(q**3-q*q)*m
    return y,lift*math.sin(math.pi*q)**2

def animate(rig,kind):
    rig.animation_data_create();fps=24;bpy.context.scene.render.fps=fps;errors={};speeds={}
    for clip,end,stride,duty,lift in ([('Idle',49,0,1,0),('Walk',25,.64,.64,.085),('Run',17,.80,.40,.18)] if kind=='human' else [('Idle',49,0,1,0),('Walk',25,.56,.60,.09),('Run',9,.48,.42,.10)]):
        act=bpy.data.actions.new(clip);act.use_fake_user=True;rig.animation_data.action=act;err=0;quats={}
        for f in range(1,end+1):
            bpy.context.scene.frame_set(f);t=(f-1)/(end-1);ph=t*math.tau
            for b in rig.pose.bones:b.rotation_mode='QUATERNION';b.matrix_basis=Matrix.Identity(4)
            body=rig.pose.bones['body'];run=clip=='Run';moving=clip!='Idle'
            z=((- .13 if run else -.078) if kind=='human' else (-.09 if run else -.09)) if moving else -.025
            z+=(.013 if moving else .003)*math.sin(ph*(2 if kind=='human' else 1))
            body.location=body.bone.matrix_local.to_quaternion().inverted()@Vector((0,0,z))
            # Small forward lean; hips stay balanced, not a rigid sliding mannequin.
            basis=body.bone.matrix_local.to_3x3();q=Matrix.Rotation((-.065 if run else -.01) if kind=='human' else .025*math.sin(ph),3,'X')
            body.rotation_quaternion=(basis.inverted()@q@basis).to_quaternion();bpy.context.view_layer.update()
            if kind=='human':
                for i,s in enumerate(('L','R')):
                    n='leg_'+s;y,zlift=trajectory(t-i*.5,duty,stride,lift) if moving else (0,0)
                    target=rig.data.bones[n+'_shin'].tail_local.copy();target.y=y;target.z+=zlift
                    err=max(err,ik(rig,n,target,(0,-1,0)));flat(rig,'foot_'+s,rig.pose.bones[n+'_shin'].tail)
                    # Arms counter the ipsilateral leg, with soft bent elbows.
                    upper=rig.pose.bones['arm_'+s];fore=rig.pose.bones['arm_'+s+'_fore'];parent=upper.parent
                    delta=parent.matrix@parent.bone.matrix_local.inverted();shoulder=delta@upper.bone.head_local
                    swing=(.65 if run else .28)*math.sin(ph+i*math.pi) if moving else .035*math.sin(ph)
                    arm_vec=Matrix.Rotation(swing,3,'X')@(upper.bone.tail_local-upper.bone.head_local)
                    elbow=shoulder+arm_vec;aim(upper,shoulder,elbow)
                    fv=Matrix.Rotation(swing-(.85 if run else .20),3,'X')@(fore.bone.tail_local-fore.bone.head_local)
                    aim(fore,elbow,elbow+fv)
            else:
                offsets=(0,.48,.52,.04) if run else (0,.5,.72,.22)
                for i,n in enumerate(('front_L','front_R','rear_L','rear_R')):
                    y,zlift=trajectory(t-offsets[i],duty,stride,lift) if moving else (0,0)
                    foot=rig.data.bones[n+'_paw'].head_local.copy();foot.y=rig.data.bones[n].head_local.y+y;foot.z+=zlift
                    if n.startswith('rear'):
                        hock=foot+Vector((0,.10,.13));err=max(err,ik(rig,n,hock,(0,-.35,-1)))
                        aim(rig.pose.bones[n+'_hock'],rig.pose.bones[n+'_shin'].tail,foot)
                    else:err=max(err,ik(rig,n,foot,(0,.25,-1)))
                    flat(rig,n+'_paw',foot)
                rig.pose.bones['tail'].rotation_quaternion=Matrix.Rotation(.12*math.sin(ph),4,'Z').to_quaternion()
                rig.pose.bones['head'].rotation_quaternion=Matrix.Rotation(-.03*math.sin(ph),4,'X').to_quaternion()
            for b in rig.pose.bones:
                q=b.rotation_quaternion
                if b.name in quats and q.dot(quats[b.name])<0:q.negate()
                quats[b.name]=q.copy();b.keyframe_insert('rotation_quaternion',frame=f);b.keyframe_insert('location',frame=f)
        for fc in act.fcurves:
            for key in fc.keyframe_points:key.interpolation='LINEAR'
        speeds[clip]=stride/duty/((end-1)/fps) if moving else 0
        act['nominal_speed']=speeds[clip];errors[clip]=err
    rig.animation_data.action=bpy.data.actions['Idle'];bpy.context.scene.frame_set(1)
    return {'nominal_speeds':speeds,'max_ik_target_errors':errors}

def finish(name,rig,objects,info):
    # Single skin renderer, material regions retained for faction jacket tinting.
    bpy.ops.object.select_all(action='DESELECT')
    for o in objects:o.select_set(True)
    bpy.context.view_layer.objects.active=objects[0];bpy.ops.object.join();skin=bpy.context.object;skin.name=name+'_Skin'
    bpy.ops.object.material_slot_remove_unused()
    reduce=skin.modifiers.new('NPC render budget','DECIMATE');reduce.ratio=.40 if name=='BallardHuman' else .32;bpy.ops.object.modifier_apply(modifier=reduce.name)
    info.update(vertices=len(skin.data.vertices),triangles=sum(len(p.vertices)-2 for p in skin.data.polygons),bones=len(rig.data.bones))
    # FBX local front -Y becomes Unity +Z with conventional export basis.
    bpy.ops.object.select_all(action='DESELECT');skin.select_set(True);rig.select_set(True)
    bpy.ops.export_scene.fbx(filepath=str(OUT/(name+'.fbx')),use_selection=True,object_types={'MESH','ARMATURE'},add_leaf_bones=False,axis_forward='-Z',axis_up='Y',bake_anim=True,bake_anim_use_all_actions=True,bake_anim_use_nla_strips=False,path_mode='AUTO')
    stage((2.8,-4,2.0) if name=='BallardHuman' else (2.1,-2.9,1.35),(0,0,.88 if name=='BallardHuman' else .40),2.1 if name=='BallardHuman' else 1.65)
    s=bpy.context.scene;s.render.resolution_x=720;s.render.resolution_y=720;s.cycles.samples=12
    floor=mat('Review ground',(.16,.19,.17));mesh('Review ground',(0,0,-.04),(1.7,1.7,.04),floor,'cylinder')
    bpy.ops.wm.save_as_mainfile(filepath=str(ROOT/'Art/Blender'/(name+'.blend')),compress=True)
    for clip,frame in [('Idle',1),('Walk',7),('Run',5)]:
        rig.animation_data.action=bpy.data.actions[clip];s.frame_set(frame);s.render.filepath=str(ROOT/'Art/Renders'/(name+'-'+clip+'.png'));bpy.ops.render.render(write_still=True)
    REPORT[name]=info

def human():
    reset()
    for a in list(bpy.data.actions):bpy.data.actions.remove(a)
    jacket=mat('Neighbor jacket',(.10,.17,.23));trousers=mat('Work trousers',(.045,.055,.075));shoe=mat('Brown leather',(.07,.045,.026));skinmat=mat('Warm skin',(.42,.24,.14));hair=mat('Hair',(.035,.025,.021));white=mat('Ivory details',(.69,.67,.59));eye=mat('Eyes',(.018,.027,.030));lip=mat('Lip',(.25,.11,.08))
    bones=[('body',(0,0,.94),(0,0,1.13),None),('spine',(0,0,1.13),(0,0,1.46),'body'),('head',(0,0,1.46),(0,0,1.72),'spine')]
    legs={};arms={}
    for sign,s in [(-1,'L'),(1,'R')]:
        h=(sign*.105,0,.94);k=(sign*.112,-.035,.52);a=(sign*.112,0,.105);legs[s]=(h,k,a)
        shoulder=(sign*.225,0,1.40);e=(sign*.282,.015,1.145);w=(sign*.270,-.025,.93);arms[s]=(shoulder,e,w)
        bones.extend([('leg_'+s,h,k,'body'),('leg_'+s+'_shin',k,a,'leg_'+s),('foot_'+s,a,(sign*.112,-.18,.065),'leg_'+s+'_shin'),('arm_'+s,shoulder,e,'spine'),('arm_'+s+'_fore',e,w,'arm_'+s),('hand_'+s,w,(sign*.270,-.035,.81),'arm_'+s+'_fore')])
    rig=rigging('BallardHuman',bones);objects=[]
    parts=[ringform('Tapered jacket',[(.94,.165,.105,0),(1.02,.178,.105,0),(1.18,.18,.11,0),(1.37,.235,.115,0),(1.45,.16,.09,0)],jacket)]
    for s,(a,e,w) in arms.items():parts.extend([limb('Upper sleeve',a,e,.083,.085,jacket),limb('Lower sleeve',e,w,.060,.068,jacket)])
    o=fuse('Continuous fitted jacket',parts,.012,.50);texture_surface(o,jacket,'NeighborJacketCloth',105);skinmesh(o,rig,lambda p:nearest_weights(rig,p,['body','spine','arm_L','arm_L_fore','arm_R','arm_R_fore'],70));objects.append(o)
    parts=[ell('Pelvis',(0,0,.925),(.178,.11,.145),trousers)]
    for s,(h,k,a) in legs.items():parts.extend([limb('Trouser thigh',h,k,.100,.095,trousers),limb('Trouser calf',k,a,.070,.077,trousers)])
    o=fuse('Continuous trousers',parts,.011,.5);skinmesh(o,rig,lambda p:nearest_weights(rig,p,['body','leg_L','leg_L_shin','leg_R','leg_R_shin'],85));objects.append(o)
    headparts=[ell('Cranium',(0,0,1.65),(.099,.100,.130),skinmat),ell('Jaw',(0,-.024,1.586),(.078,.075,.069),skinmat),ell('Nose bridge',(0,-.093,1.650),(.023,.039,.052),skinmat),ell('Nose tip',(0,-.119,1.625),(.027,.025,.022),skinmat),limb('Neck',(0,0,1.43),(0,0,1.55),.054,.05,skinmat)]
    for side in [-1,1]:headparts.append(ell('Ear',(side*.100,0,1.646),(.018,.024,.039),skinmat))
    o=fuse('Face neck and ears',headparts,.006,.42);skinmesh(o,rig,lambda p:{'head':1});objects.append(o)
    details=[(ell('Short textured hair',(0,.018,1.722),(.100,.093,.057),hair),'head'),(ell('Lower lip',(0,-.092,1.592),(.028,.006,.007),lip),'head')]
    for side in [-1,1]:
        details.extend([(ell('Eye socket',(side*.041,-.087,1.663),(.024,.009,.013),skinmat),'head'),(ell('Eye white',(side*.041,-.094,1.663),(.017,.005,.008),white),'head'),(ell('Dark iris',(side*.041,-.099,1.663),(.006,.003,.006),eye),'head'),(limb('Eyebrow',(side*.024,-.098,1.68),(side*.061,-.094,1.681),.005,.005,hair),'head')])
    details.append((ringform('Shirt collar',[(1.443,.066,.061,0),(1.48,.057,.05,0)],white),'head'))
    for sign,s in [(-1,'L'),(1,'R')]:
        h,k,a=legs[s];details.append((ell('Shaped leather shoe',(sign*.112,-.055,.071),(.079,.147,.055),shoe),'foot_'+s))
        w=Vector(arms[s][2]);parts=[ell('Palm',w+Vector((0,-.005,-.061)),(.039,.024,.059),skinmat)]
        for j in range(4):
            x=w.x+(j-1.5)*.017;parts.append(limb('Finger',(x,-.028,.865),(x,-.043,.815+abs(j-1.5)*.010),.009,.010,skinmat))
        parts.append(limb('Thumb',w+Vector((-sign*.029,-.010,-.039)),w+Vector((-sign*.045,-.035,-.079)),.013,.013,skinmat))
        o=fuse('Hand '+s,parts,.005,.4);skinmesh(o,rig,lambda p,s=s:{'hand_'+s:1});objects.append(o)
        details.append((limb('Jacket pocket seam',(sign*.12,-.109,1.12),(sign*.12,-.113,1.22),.006,.006,jacket),'spine'))
    details.append((limb('Jacket zipper',(0,-.111,1.01),(0,-.117,1.41),.004,.004,shoe),'spine'))
    for o,b in details:skinmesh(o,rig,lambda p,b=b:{b:1});objects.append(o)
    info=animate(rig,'human');finish('BallardHuman',rig,objects,info)

def dog():
    reset()
    for a in list(bpy.data.actions):bpy.data.actions.remove(a)
    coat=mat('Dog golden sable',(.30,.17,.065));dark=mat('Dog charcoal muzzle',(.045,.035,.025));nose=mat('Dog nose and eyes',(.012,.016,.015));cream=mat('Dog chest',(.56,.44,.27));collar=mat('Dog red collar',(.34,.035,.022));tag=mat('Dog brass tag',(.50,.31,.055),.6,.35)
    bones=[('body',(0,.05,.48),(0,.05,.67),None),('head',(0,-.32,.58),(0,-.56,.74),'body'),('tail',(0,.40,.56),(0,.75,.57),'body')];points={}
    for sign,s in [(-1,'L'),(1,'R')]:
        a=(sign*.145,-.28,.57);k=(sign*.145,-.23,.32);w=(sign*.135,-.30,.075);points['front_'+s]=(a,k,w)
        bones.extend([('front_'+s,a,k,'body'),('front_'+s+'_shin',k,w,'front_'+s),('front_'+s+'_paw',w,(sign*.135,-.41,.05),'front_'+s+'_shin')])
        a=(sign*.155,.29,.55);k=(sign*.175,.16,.34);h=(sign*.145,.41,.20);w=(sign*.145,.31,.07);points['rear_'+s]=(a,k,h)
        bones.extend([('rear_'+s,a,k,'body'),('rear_'+s+'_shin',k,h,'rear_'+s),('rear_'+s+'_hock',h,w,'rear_'+s+'_shin'),('rear_'+s+'_paw',w,(sign*.145,.20,.05),'rear_'+s+'_hock')])
    rig=rigging('BallardDog',bones)
    parts=[ell('Deep rib cage',(0,-.10,.49),(.175,.31,.175),coat),ell('Tucked waist',(0,.20,.48),(.133,.20,.13),coat),ell('Rounded haunch',(0,.31,.49),(.167,.16,.15),coat),limb('Sloped neck',(0,-.25,.48),(0,-.43,.68),.135,.125,coat),ell('Skull',(0,-.48,.71),(.105,.125,.108),coat),ell('Long canine muzzle',(0,-.635,.665),(.075,.15,.060),coat)]
    for n,(a,k,w) in points.items():
        rear=n.startswith('rear');parts.extend([limb('Muscular thigh' if rear else 'Foreleg',a,k,.073 if rear else .052,.074 if rear else .047,coat),limb('Lower limb',k,w,.029,.032,coat)])
        parts.append(ell('Rounded elbow or stifle',k,(.043,.042,.044) if rear else (.033,.034,.037),coat))
        parts.append(ell('Joint blend at shoulder',a,(.060,.059,.056),coat))
        if rear:
            b=rig.data.bones[n+'_hock'];parts.append(limb('Sloping pastern',b.head_local,b.tail_local,.026,.027,coat))
        p=rig.data.bones[n+'_paw'].head_local;parts.append(ell('Paw',(p.x,p.y-.025,.052),(.045,.076,.033),coat))
        for j in range(3):parts.append(ell('Toe',(p.x+(j-1)*.021,p.y-.078,.040),(.016,.028,.018),coat))
    o=fuse('Continuous canine anatomy',parts,.007,.45)
    def weights(p):
        def smooth(a,b,x):
            t=max(0,min(1,(x-a)/(b-a)));return t*t*(3-2*t)
        head=(1-smooth(-.46,-.30,p.y))*smooth(.42,.56,p.z)
        if head>.995:return {'head':1}
        if p.z<.13:
            n=min(points,key=lambda n:(p-rig.data.bones[n+'_paw'].head_local).length);return {n+'_paw':1}
        n=min(points,key=lambda n:min(distseg(p,rig.data.bones[x].head_local,rig.data.bones[x].tail_local) for x in [n,n+'_shin']))
        names=[n,n+'_shin']+([n+'_hock'] if n.startswith('rear') else [])
        distance=min(distseg(p,rig.data.bones[x].head_local,rig.data.bones[x].tail_local) for x in names)
        influence=(1-smooth(.065,.13,distance))*(1-smooth(.50,.585,p.z))*smooth(.075,.13,abs(p.x))
        result={x:w*influence for x,w in nearest_weights(rig,p,names,70).items()}
        total=sum(result.values())
        result={x:w/(total or 1)*influence for x,w in result.items()}
        result['body']=(1-influence)*(1-head);result['head']=(1-influence)*head
        return result
    texture_surface(o,coat,'BallardDogCoat',75);skinmesh(o,rig,weights);objects=[o]
    details=[(ell('Nose',(0,-.77,.674),(.060,.027,.037),nose),'head'),(ell('Lower jaw',(0,-.62,.620),(.057,.119,.023),dark),'head')]
    for side in [-1,1]:
        details.extend([(ell('Eye',(side*.083,-.535,.745),(.010,.010,.010),nose),'head')])
        # Folded, tapered pinna, a curved mesh rather than a rotated cube.
        verts=[(side*x,y,z) for x,y,z in [( .075,-.455,.796),(.133,-.435,.785),(.153,-.425,.724),(.126,-.473,.655),(.093,-.495,.720),(.106,-.455,.760)]]
        faces=[(0,1,5),(1,2,5),(2,3,5),(3,4,5),(4,0,5)]
        d=bpy.data.meshes.new('Folded pinna');d.from_pydata(verts,[],faces);d.update();e=bpy.data.objects.new('Natural folded ear',d);bpy.context.collection.objects.link(e);d.materials.append(dark)
        sol=e.modifiers.new('Ear thickness','SOLIDIFY');sol.thickness=.012;sub=e.modifiers.new('Soft ear','SUBSURF');sub.levels=2
        bpy.ops.object.select_all(action='DESELECT');bpy.context.view_layer.objects.active=e;e.select_set(True);bpy.ops.object.convert(target='MESH');details.append((bpy.context.object,'head'))
    # Tail curves gently out behind, tapering to a soft point.
    tailparts=[limb('Tail base',(0,.39,.56),(0,.59,.60),.057,.05,coat),limb('Tail tip',(0,.57,.60),(0,.79,.56),.035,.033,coat)]
    details.append((fuse('Tapered canine tail',tailparts,.009,.4),'tail'))
    for o,b in details:skinmesh(o,rig,lambda p,b=b:{b:1});objects.append(o)
    # Narrow collar is aligned to sloped neck and deforms with head.
    bpy.ops.mesh.primitive_torus_add(major_radius=.127,minor_radius=.013,major_segments=32,minor_segments=8,location=(0,-.345,.60),rotation=(math.pi/3,0,0));c=bpy.context.object;c.name='Red fabric collar';c.data.materials.append(collar);skinmesh(c,rig,lambda p:{'head':1});objects.append(c)
    c=ell('Brass ID tag',(0,-.402,.487),(.022,.009,.029),tag);skinmesh(c,rig,lambda p:{'head':1});objects.append(c)
    info=animate(rig,'dog');finish('BallardDog',rig,objects,info)

if __name__=='__main__':
    human();dog();(ROOT/'Documentation/neighbors-v5.json').write_text(json.dumps(REPORT,indent=2));print('NEIGHBOR_ASSETS_READY',json.dumps(REPORT))
