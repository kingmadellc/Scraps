using UnityEngine;
using System.Collections.Generic;
namespace Jimothy {
public enum NeighborKind { Dog,Cat,AngryHuman,KindHuman,Fisherman,Gull,Passerby,UnhousedNeighbor,ImpairedPasserby }
public class NeighborAI:MonoBehaviour {
 public NeighborKind Kind; public bool IsThreat {get;private set;}
 readonly List<Mesh> visualMeshes=new();
 NeighborNavigation navigation; CharacterController controller; Vector3 home,patrolTarget; Transform body,net; TextMesh tell; float cooldown,clock,friendlyCue,patrolTimer,restTimer; int patrolRound; Camera viewCamera; bool animal; Animator animator; Transform hand; bool rigged; int motionState=-1; float displayedSpeed;
 readonly NeighborAwareness awareness=new();NeighborBodyLanguage language;RaycastHit restSurface;bool supportedRest;
 static readonly RaycastHit[] sightHits=new RaycastHit[32];
 public float Suspicion01=>CanBecomeHostile?awareness.Value:0;
 public bool HasLineOfSight {get;private set;}
 public bool CanBecomeHostile=>Kind==NeighborKind.Dog||Kind==NeighborKind.AngryHuman||Kind==NeighborKind.Fisherman||Kind==NeighborKind.Gull;
 public Vector3 LastSeenPosition {get;private set;}
 public bool IsInvestigating=>CanBecomeHostile&&!IsThreat&&HasLineOfSight&&Suspicion01>=.55f;
 public string DetectionLabel=>IsThreat?(HasLineOfSight?"Chasing":"Searching"):IsInvestigating?"Investigating":Suspicion01>.04f?"Noticing":"Unaware";
 public void Initialize(NeighborKind kind) {
  Kind=kind;viewCamera=Camera.main;home=transform.position;animal=kind==NeighborKind.Dog||kind==NeighborKind.Cat||kind==NeighborKind.Gull;
  transform.localScale=Vector3.one*(kind==NeighborKind.Cat?.38f:kind==NeighborKind.Gull?.55f:1f);
  controller=gameObject.AddComponent<CharacterController>();ConfigureCollider(controller,kind);
  BuildVisual(kind);BatchStaticVisuals();
  transform.localScale=Vector3.one*(kind==NeighborKind.Cat?.38f:kind==NeighborKind.Dog?1f:kind==NeighborKind.Gull?.55f:1f);
  navigation=gameObject.AddComponent<NeighborNavigation>();var footprint=NavigationFootprint(kind);navigation.Initialize(controller,footprint.x,footprint.y);
  home=transform.position;patrolTarget=home;
  language=gameObject.AddComponent<NeighborBodyLanguage>();language.Initialize(body,kind);
  var label=new GameObject("Behavior cue");label.transform.SetParent(transform);label.transform.localPosition=Vector3.up*(kind==NeighborKind.Dog?1.0f:animal?1.4f:1.98f);tell=label.AddComponent<TextMesh>();tell.fontSize=32;tell.characterSize=.06f;tell.anchor=TextAnchor.MiddleCenter;tell.color=new Color(.97f,.91f,.72f);
 }
 // World-space body dimensions are independent of the visual model scale.
 public static Vector2 NavigationFootprint(NeighborKind kind)=>kind==NeighborKind.Dog?new Vector2(.62f,.84f):kind==NeighborKind.Cat?new Vector2(.32f,.46f):kind==NeighborKind.Gull?new Vector2(.28f,.49f):new Vector2(.37f,1.82f);
 public static void ConfigureCollider(CharacterController c,NeighborKind kind){
  bool dog=kind==NeighborKind.Dog,cat=kind==NeighborKind.Cat,gull=kind==NeighborKind.Gull;Vector3 s=c.transform.lossyScale;
  float worldHeight=dog?.80f:cat?.42f:gull?.45f:1.78f,worldRadius=dog?.28f:cat?.13f:gull?.16f:.29f;
  c.radius=worldRadius/Mathf.Max(.01f,Mathf.Max(s.x,s.z));c.height=worldHeight/Mathf.Max(.01f,s.y);c.center=Vector3.up*c.height*.5f;
  c.stepOffset=.22f/Mathf.Max(.01f,s.y);c.skinWidth=.025f/Mathf.Max(.01f,s.y);c.minMoveDistance=0;c.slopeLimit=48;
 }
 bool BuildRiggedVisual(NeighborKind kind) {
  if(kind==NeighborKind.Cat||kind==NeighborKind.Gull)return false;
  string asset=kind==NeighborKind.Dog?"BallardDog":"BallardHuman";
  var prefab=Resources.Load<GameObject>(asset);if(!prefab)return false;
  var visual=Instantiate(prefab,transform);visual.name=asset+" animated neighbor";visual.transform.localPosition=Vector3.down*(kind==NeighborKind.Dog?.019f:.016f);visual.transform.localRotation=Quaternion.identity;visual.transform.localScale=Vector3.one;
  body=visual.transform;rigged=true;animator=visual.GetComponent<Animator>();if(!animator)animator=visual.AddComponent<Animator>();
  animator.runtimeAnimatorController=Resources.Load<RuntimeAnimatorController>(asset+"Controller");animator.applyRootMotion=false;animator.cullingMode=AnimatorCullingMode.CullUpdateTransforms;
  foreach(var bone in visual.GetComponentsInChildren<Transform>())if(bone.name=="hand_R")hand=bone;
  if(kind!=NeighborKind.Dog){
   var color=kind==NeighborKind.Fisherman?new Color(.52f,.29f,.045f):kind==NeighborKind.AngryHuman?new Color(.31f,.075f,.042f):kind==NeighborKind.KindHuman?new Color(.065f,.21f,.12f):new Color(.07f,.12f,.19f);
   foreach(var renderer in visual.GetComponentsInChildren<Renderer>())for(int i=0;i<renderer.sharedMaterials.Length;i++)if(renderer.sharedMaterials[i]&&renderer.sharedMaterials[i].name.Contains("Neighbor jacket")){var block=new MaterialPropertyBlock();renderer.GetPropertyBlock(block,i);// The baked cloth map already contains blue albedo; compensate it to preserve each role color.
    var material=renderer.sharedMaterials[i];bool hasClothMap=material.HasProperty("_BaseMap")&&material.GetTexture("_BaseMap");
    block.SetColor("_BaseColor",hasClothMap?new Color(color.r/.10f,color.g/.17f,color.b/.23f,1):color);renderer.SetPropertyBlock(block,i);}
  }
  if(kind==NeighborKind.Fisherman){
   var g=GameSession.Instance;var cream=new Color(.79f,.76f,.64f);net=new GameObject("Fishing net pivot").transform;net.SetParent(transform,false);net.localPosition=new Vector3(.29f,.90f,.05f);
   g.Piece(net,"Ash net handle",PrimitiveType.Cylinder,new(0,.35f,0),new(.025f,.39f,.025f),new Color(.42f,.29f,.13f));
   for(int k=0;k<16;k++){float a=k*Mathf.PI/8;var segment=g.Piece(net,"Net rim",PrimitiveType.Cube,new(Mathf.Sin(a)*.23f,.86f+Mathf.Cos(a)*.23f,0),new(.10f,.016f,.016f),cream);segment.localRotation=Quaternion.Euler(0,0,-a*Mathf.Rad2Deg);}
   for(int k=-1;k<=1;k++)g.Piece(net,"Net mesh",PrimitiveType.Cube,new(k*.10f,.86f,.01f),new(.009f,.40f,.009f),new Color(.27f,.35f,.30f));
  }
  return true;
 }
 void AnimateMovement(float speed,float turning,float dt){
  if(!animator||!animator.runtimeAnimatorController)return;
  animator.enabled=true;displayedSpeed=Mathf.Lerp(displayedSpeed,speed,1-Mathf.Exp(-dt*10));
  bool pivot=turning>20&&restTimer<=0;float motion=pivot?Mathf.Max(displayedSpeed,.27f):displayedSpeed;
  int state=motionState==2?(motion<1.35f?1:2):motion>1.65f?2:1;if(motion<.06f)state=0;
  if(state!=motionState){animator.CrossFadeInFixedTime(state==0?"Idle":state==1?"Walk":"Run",state==0?.20f:.16f);motionState=state;}
  float nominal=state==1?(Kind==NeighborKind.Dog?.933333f:1f):(Kind==NeighborKind.Dog?3.428571f:3f);
  animator.speed=state==0?1:Mathf.Clamp(motion/nominal,.22f,1.6f);
 }
 public void SenseTarget(Vector3 position,float playerSpeed,bool safe,float dt){
  var to=position-transform.position;float distance=to.magnitude;float range=Kind==NeighborKind.Dog?10:7;
  bool territory=Kind!=NeighborKind.AngryHuman||Vector3.Distance(position,home)<6;
  bool facing=Vector3.Angle(transform.forward,Vector3.ProjectOnPlane(to,Vector3.up))<70||distance<2.2f;
  Vector3 pursuitGround=default;bool ground=navigation&&navigation.TryPursuitGround(position,out pursuitGround);
  HasLineOfSight=!safe&&ground&&territory&&distance<range&&facing&&(SightClear(position+Vector3.up*.18f)||SightClear(position+Vector3.up*.36f));
  awareness.Step(dt,HasLineOfSight,1-distance/range,Mathf.InverseLerp(.25f,2.5f,playerSpeed),CanBecomeHostile,safe);
  if(HasLineOfSight)LastSeenPosition=pursuitGround;
  IsThreat=CanBecomeHostile&&awareness.Chasing&&!safe;
 }
 bool SightClear(Vector3 target){float eye=Kind==NeighborKind.Dog?.66f:Kind==NeighborKind.Cat?.32f:Kind==NeighborKind.Gull?.34f:1.55f;var origin=transform.position+Vector3.up*eye;var delta=target-origin;int count=Physics.RaycastNonAlloc(origin,delta.normalized,sightHits,delta.magnitude,1,QueryTriggerInteraction.Ignore);if(count==sightHits.Length)return false;for(int i=0;i<count;i++){var c=sightHits[i].collider;if(!c||c.transform.IsChildOf(transform)||c is CharacterController||c.GetComponentInParent<NeighborAI>())continue;return false;}return true;}
 void BuildVisual(NeighborKind kind) {
  if(BuildRiggedVisual(kind))return;
  var g=GameSession.Instance;var dark=new Color(.045f,.055f,.06f);var cream=new Color(.79f,.76f,.64f);var skin=new Color(.62f,.39f,.25f);
  Transform P(string n,PrimitiveType type,Vector3 p,Vector3 size,Color c)=>g.Piece(transform,n,type,p,size,c);
  if(kind==NeighborKind.Gull){
   body=P("Gull breast",PrimitiveType.Sphere,new(0,.48f,0),new(.36f,.48f,.65f),cream);
   P("White gull head",PrimitiveType.Sphere,new(0,.76f,.24f),Vector3.one*.25f,cream);P("Amber beak",PrimitiveType.Cube,new(0,.73f,.43f),new(.09f,.07f,.22f),new Color(.95f,.55f,.08f));
   for(int side=-1;side<=1;side+=2){P("Folded slate wing",PrimitiveType.Sphere,new(side*.15f,.50f,-.05f),new(.10f,.30f,.52f),new Color(.28f,.34f,.37f));P("Webbed foot",PrimitiveType.Cube,new(side*.09f,.15f,.12f),new(.10f,.045f,.21f),new Color(.9f,.46f,.10f));P("Eye",PrimitiveType.Sphere,new(side*.105f,.80f,.30f),Vector3.one*.035f,dark);}return;
  }
  if(animal){
   bool cat=kind==NeighborKind.Cat;var coat=cat?new Color(.14f,.16f,.17f):new Color(.47f,.27f,.11f);
   body=P("Animal ribcage",PrimitiveType.Sphere,new(0,.5f,0),new(.55f,.45f,.9f),coat);
   P("Shoulder ruff",PrimitiveType.Sphere,new(0,.55f,.25f),new(.47f,.48f,.44f),coat);
   P("Animal head",PrimitiveType.Sphere,new(0,.74f,.42f),new(.39f,.38f,.40f),coat);
   P("Cream muzzle",PrimitiveType.Sphere,new(0,.67f,.63f),cat?new(.22f,.14f,.10f):new(.26f,.20f,.30f),cream);
   P("Nose",PrimitiveType.Sphere,new(0,.7f,cat?.68f:.79f),new(.10f,.07f,.07f),dark);
   for(int side=-1;side<=1;side+=2){
    var ear=P(cat?"Pointed cat ear":"Floppy dog ear",PrimitiveType.Cube,new(side*.18f,cat?.95f:.81f,.4f),cat?new(.11f,.21f,.14f):new(.13f,.29f,.18f),coat);ear.localRotation=Quaternion.Euler(0,0,side*(cat?-18:14));
    P("Bright watchful eye",PrimitiveType.Sphere,new(side*.12f,.79f,.59f),new(.055f,.045f,.025f),cat?new Color(.64f,.74f,.18f):dark);
    for(int back=-1;back<=1;back+=2){P("Slender leg",PrimitiveType.Capsule,new(side*.18f,.23f,back*.28f),new(.12f,.18f,.13f),coat);P("Paw",PrimitiveType.Sphere,new(side*.18f,.085f,back*.28f+.06f),new(.16f,.12f,.22f),cream);}
   }
   P("Red collar",PrimitiveType.Cylinder,new(0,.56f,.35f),new(.46f,.045f,.46f),new Color(.53f,.09f,.065f)).localRotation=Quaternion.Euler(90,0,0);
   P("Collar tag",PrimitiveType.Sphere,new(0,.36f,.41f),new(.09f,.12f,.04f),new Color(.82f,.59f,.19f));
   P("Expressive tail",PrimitiveType.Capsule,new(0,cat?.78f:.58f,-.60f),new(cat?.085f:.13f,.31f,.10f),coat).localRotation=Quaternion.Euler(cat?-12:-57,0,0);return;
  }
  var coatColor=kind==NeighborKind.Fisherman?new Color(.78f,.47f,.07f):kind==NeighborKind.AngryHuman?new Color(.42f,.16f,.10f):kind==NeighborKind.KindHuman?new Color(.17f,.31f,.25f):new Color(.16f,.21f,.28f);
  body=P("Tailored jacket",PrimitiveType.Cube,new(0,1.15f,0),new(.60f,.69f,.37f),coatColor);
  P("Jacket shoulder line",PrimitiveType.Sphere,new(0,1.46f,0),new(.68f,.22f,.40f),coatColor);
  P("Shirt collar",PrimitiveType.Cube,new(0,1.44f,.20f),new(.22f,.13f,.035f),cream);
  P("Jacket placket",PrimitiveType.Cube,new(0,1.14f,.196f),new(.035f,.48f,.02f),dark);
  P("Work trousers",PrimitiveType.Cube,new(0,.77f,0),new(.46f,.25f,.34f),dark);
  for(int side=-1;side<=1;side+=2){
   P("Trouser leg",PrimitiveType.Cube,new(side*.15f,.46f,0),new(.21f,.58f,.27f),dark);
   P("Shoe sole",PrimitiveType.Cube,new(side*.15f,.115f,.07f),new(.25f,.12f,.43f),dark);
   P("Leather shoe",PrimitiveType.Sphere,new(side*.15f,.17f,.10f),new(.25f,.17f,.40f),new Color(.18f,.105f,.07f));
   P("Sleeved arm",PrimitiveType.Capsule,new(side*.38f,1.13f,.015f),new(.21f,.33f,.23f),coatColor).localRotation=Quaternion.Euler(-12,0,side*8);
   P("Cuff",PrimitiveType.Cube,new(side*.40f,.86f,.07f),new(.20f,.08f,.23f),cream);P("Hand",PrimitiveType.Sphere,new(side*.40f,.78f,.10f),new(.14f,.18f,.14f),skin);
   P("Jacket pocket",PrimitiveType.Cube,new(side*.17f,1.02f,.207f),new(.18f,.15f,.025f),coatColor*.8f);
  }
  P("Neck",PrimitiveType.Cylinder,new(0,1.60f,0),new(.17f,.11f,.18f),skin);P("Face",PrimitiveType.Sphere,new(0,1.78f,.02f),new(.32f,.39f,.30f),skin);
  P("Nose",PrimitiveType.Sphere,new(0,1.79f,.18f),new(.08f,.11f,.075f),skin);
  P("Hair and beanie",PrimitiveType.Sphere,new(0,1.94f,0),new(.36f,.19f,.34f),kind==NeighborKind.Fisherman?dark:coatColor*.6f);
  P("Hat rolled brim",PrimitiveType.Cylinder,new(0,1.94f,0),new(.37f,.025f,.35f),dark);
  if(kind==NeighborKind.AngryHuman||kind==NeighborKind.KindHuman){P("Restaurant apron",PrimitiveType.Cube,new(0,.99f,.23f),new(.43f,.62f,.045f),cream);P("Apron bib",PrimitiveType.Cube,new(0,1.35f,.23f),new(.30f,.22f,.045f),cream);}
  if(kind==NeighborKind.Passerby||kind==NeighborKind.UnhousedNeighbor)P("Canvas backpack",PrimitiveType.Cube,new(0,1.22f,-.29f),new(.43f,.57f,.22f),new Color(.31f,.27f,.18f));
  if(kind==NeighborKind.Fisherman){
   net=new GameObject("Fishing net pivot").transform;net.SetParent(transform,false);net.localPosition=new(.46f,.83f,.22f);
   g.Piece(net,"Ash net handle",PrimitiveType.Cylinder,new(0,.54f,0),new(.035f,.60f,.035f),new Color(.42f,.29f,.13f));
   // An open hoop reads as a net, rather than a solid oversized sphere.
   for(int k=0;k<12;k++){float a=k*Mathf.PI/6;var segment=g.Piece(net,"Net rim",PrimitiveType.Cube,new(Mathf.Sin(a)*.31f,1.22f+Mathf.Cos(a)*.31f,0),new(.17f,.025f,.025f),cream);segment.localRotation=Quaternion.Euler(0,0,-a*Mathf.Rad2Deg);}
   for(int k=-1;k<=1;k++)g.Piece(net,"Net mesh",PrimitiveType.Cube,new(k*.14f,1.22f,.01f),new(.012f,.53f,.012f),new Color(.27f,.35f,.30f));
  }
 }
 void BatchStaticVisuals(){
  var groups=new Dictionary<Material,List<CombineInstance>>();var sources=new List<GameObject>();
  foreach(var filter in GetComponentsInChildren<MeshFilter>()){
   if(filter.transform==body||(net&&filter.transform.IsChildOf(net)))continue;
   var renderer=filter.GetComponent<MeshRenderer>();if(!renderer||!filter.sharedMesh)continue;
   var material=renderer.sharedMaterial;if(!groups.TryGetValue(material,out var list)){list=new List<CombineInstance>();groups.Add(material,list);}list.Add(new CombineInstance{mesh=filter.sharedMesh,transform=transform.worldToLocalMatrix*filter.transform.localToWorldMatrix});sources.Add(filter.gameObject);renderer.enabled=false;
  }
  foreach(var entry in groups){var m=new Mesh{name="NPC clothing and silhouette"};m.CombineMeshes(entry.Value.ToArray(),true,true);visualMeshes.Add(m);var go=new GameObject("Dressed character surface");go.transform.SetParent(transform,false);go.AddComponent<MeshFilter>().sharedMesh=m;go.AddComponent<MeshRenderer>().sharedMaterial=entry.Key;}
  foreach(var go in sources)Destroy(go);
 }
 void OnDestroy(){foreach(var mesh in visualMeshes)if(mesh)Destroy(mesh);}
 void Update(){var g=GameSession.Instance;if(!g||!g.Playing||g.Paused){if(animator)animator.speed=0;if(language)language.Set(false,false,default,false);return;}
  float dt=Time.deltaTime;clock+=dt;cooldown-=dt;friendlyCue-=dt;restTimer=Mathf.Max(0,restTimer-dt);var player=g.Player.transform.position;float distance=Vector3.Distance(player,transform.position);
  SenseTarget(player,g.Player.HorizontalSpeed,g.AtHome,dt);
  if(distance>28){tell.text="";if(animator)animator.enabled=false;if(language)language.Set(false,false,default,false);return;}
  bool noticing=CanBecomeHostile&&HasLineOfSight&&!IsThreat&&Suspicion01>.06f;
  bool warning=IsInvestigating;
  bool catHiss=Kind==NeighborKind.Cat&&HasLineOfSight&&distance<3;
  if(IsThreat&&HasLineOfSight&&!navigation.IsBlocked)restTimer=0;
  patrolTimer-=dt;
  if(!IsThreat&&!warning&&restTimer<=0&&(patrolTimer<=0||navigation.TargetUnreachable||Vector2.Distance(new(transform.position.x,transform.position.z),new(patrolTarget.x,patrolTarget.z))<.15f)){
   bool found=navigation.TryPatrolTarget(home,GetInstanceID()*.31f+(patrolRound++)*1.73f,out patrolTarget);patrolTimer=7+(GetInstanceID()&3);if(!found){restTimer=1.5f;supportedRest=navigation.TryRestSurface(out restSurface);}
  }
  Vector3 target=IsThreat?LastSeenPosition:patrolTarget;var direction=target-transform.position;direction.y=0;
  if(catHiss){body.localScale=new Vector3(.5f,.7f,.75f);body.localPosition=Vector3.up*.65f;}else if(Kind==NeighborKind.Cat){body.localScale=new Vector3(.55f,.45f,.9f);body.localPosition=Vector3.up*.5f;}
  if(Kind==NeighborKind.KindHuman&&HasLineOfSight&&distance<3&&cooldown<=0){g.Data.hunger=Mathf.Min(100,g.Data.hunger+12);cooldown=45;friendlyCue=3;}
  if(IsThreat&&HasLineOfSight&&distance<(Kind==NeighborKind.Dog?.8f:Kind==NeighborKind.Gull?.55f:1f)&&cooldown<=0){g.Hurt(Kind==NeighborKind.Dog?12:8);cooldown=2;}
  var beforeTurn=transform.rotation;Vector3 moved=navigation.Step(target,IsThreat?Kind==NeighborKind.Dog?3.4f:2.9f:.8f,dt,warning||catHiss||restTimer>0||direction.sqrMagnitude<(IsThreat?.2f:.01f),IsThreat?155:95);
  if(restTimer<=0&&navigation.IsBlocked){restTimer=1.5f+(GetInstanceID()&3)*.3f;supportedRest=navigation.TryRestSurface(out restSurface);patrolTimer=0;}
  Vector3 heading=moved.sqrMagnitude>.00001f?moved:warning?player-transform.position:restTimer>0&&supportedRest?Vector3.Cross(restSurface.normal,Vector3.up):Vector3.zero;heading.y=0;float turned=Quaternion.Angle(beforeTurn,transform.rotation)/Mathf.Max(dt,.0001f);
  if(moved.sqrMagnitude<.00001f&&(warning||restTimer>0)&&heading.sqrMagnitude>.001f){var before=transform.rotation;transform.rotation=Quaternion.RotateTowards(before,Quaternion.LookRotation(heading),dt*(IsThreat?155:95));turned=Quaternion.Angle(before,transform.rotation)/Mathf.Max(dt,.0001f);}
  AnimateMovement(moved.magnitude/Mathf.Max(dt,.0001f),turned,dt);
  if(language)language.Set(restTimer>0,supportedRest,restSurface,warning||IsThreat);
  tell.text=catHiss?"HISSS!":friendlyCue>0?"A snack for you, little guy.":IsThreat?(HasLineOfSight?(Kind==NeighborKind.Dog?"BARK!":"Hey! Stop there!"):"Where did it go?"):noticing?(Suspicion01>.65f?"!  Back away":"?  What's that?"):"";
  tell.color=Color.Lerp(new Color(1,.85f,.55f),new Color(1,.40f,.22f),Suspicion01);
  if(net){if(hand)net.localPosition=transform.InverseTransformPoint(hand.position);net.localRotation=Quaternion.Euler(IsThreat?Mathf.Sin(clock*5)*40:15,0,-15);}
  if(viewCamera)tell.transform.rotation=Quaternion.LookRotation(tell.transform.position-viewCamera.transform.position);
 }
}
}
