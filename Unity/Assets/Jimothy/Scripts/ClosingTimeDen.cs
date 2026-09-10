using System.Collections.Generic;
using UnityEngine;
namespace Jimothy {
public static partial class ClosingTimeWorld {
 public const float DenFloor=-2.4f;
 public static readonly Vector3 DenInterior=new(-14,-2.35f,59),DenEntrance=new(-14,.05f,49),StreetStart=new(-3,.05f,44);
 public static readonly List<Vector3> DenDisplaySlots=new();
 public static bool DenContains(Vector3 p)=>p.x>=-19.7f&&p.x<=-8.3f&&p.z>=56&&p.z<=63.2f&&p.y>=-2.6f&&p.y<=1.4f;
 // Same outer footprint/top as the district slab, with only the annex excavation omitted.
 static void DenGround(){
  Box("Ground west",new(-30,-.25f,-2),new(20,.5f,150),"asphalt",true);
  Box("Ground east",new(16,-.25f,-2),new(48,.5f,150),"asphalt",true);
  Box("Ground south of den",new(-14,-.25f,-13.5f),new(12,.5f,127),"asphalt",true);
  Box("Ground north of den",new(-14,-.25f,68.25f),new(12,.5f,9.5f),"asphalt",true);
 }
 static void DenRoom(){
  DenDisplaySlots.Clear();
  Mat("denPlaster",C(.32f,.35f,.32f),.08f);Mat("denVelvet",C(.23f,.30f,.23f),.04f);Mat("denRug",C(.30f,.12f,.11f),.01f);Mat("denInk",C(.075f,.065f,.07f),.04f);Mat("denTape",C(.67f,.55f,.30f),.01f);Mat("denFlannel",C(.37f,.12f,.10f),.02f);Mat("denPaper",C(.70f,.67f,.53f),.02f);
  foreach(var key in new[]{"denVelvet","denRug","denPaper"}){var tint=mats[key].GetColor("_BaseColor");mats[key].SetTexture("_BaseMap",SurfaceTexture("Worn textile "+key,tint*.65f,tint*1.4f));mats[key].SetColor("_BaseColor",Color.white);mats[key].SetTextureScale("_BaseMap",Vector2.one*(key=="denPaper"?1.8f:2.4f));}
  // Low utility-building annex: its basement is physically below street grade.
  Box("Den basement floor",new(-14,DenFloor-.13f,56.75f),new(12,.26f,13.5f),"cedar",true);
  Box("Den west masonry",new(-19.85f,-.4f,56.75f),new(.30f,4,13.5f),"brickUmber",true);
  Box("Den east masonry",new(-8.15f,-.4f,56.75f),new(.30f,4,13.5f),"brickUmber",true);
  Box("Den north masonry",new(-14,-.4f,63.35f),new(11.4f,4,.30f),"brickUmber",true);
  Box("Den ceiling",new(-14,1.70f,56.75f),new(12,.20f,13.5f),"wood",true);
  Box("Annex roof flashing",new(-14,1.83f,56.75f),new(12.2f,.06f,13.7f),"copper");
  Box("Hole left masonry",new(-17.5f,-.4f,49.9f),new(5,4,.2f),"brickRust",true);
  Box("Hole right masonry",new(-10.5f,-.4f,49.9f),new(5,4,.2f),"brickRust",true);
  Box("Hole lintel",new(-14,1.47f,49.9f),new(2,.26f,.2f),"brickRust",true);
  // Uneven exposed brick edges describe a dug-out hole without hidden collision lips.
  for(int i=0;i<5;i++)foreach(int side in new[]{-1,1})Box("Broken hole brick",new(-14+side*(1.02f+(i%2)*.07f),.13f+i*.23f,49.75f),new(.18f,.16f,.16f),i%2==0?"brickUmber":"brickRust",true,Quaternion.Euler(0,0,side*(i%3-1)*5));
  DenWordmark(new(-14,1.48f,49.74f));
  Text("DOWNSTAIRS  /  ALL PAWS WELCOME",new(-14,1.14f,49.69f),.0095f,"cream",Quaternion.identity);
  // Ramp top plane connects (z50,y0) to (z56,y-2.4) exactly; no stair or teleport.
  var slope=Quaternion.Euler(Mathf.Atan2(2.4f,6)*Mathf.Rad2Deg,0,0);
  Box("Den descending ramp",new Vector3(-14,-1.2f,53)-slope*Vector3.up*.11f,new(3,.22f,Mathf.Sqrt(41.76f)),"wood",true,slope);
  foreach(int side in new[]{-1,1}){
   Box("Den ramp retaining wall",new(-14+side*1.65f,-.4f,53),new(.30f,4,6),"brickUmber",true);
   for(int i=0;i<7;i++)Box("Ramp amber edge",new(-14+side*1.36f,-i*.36f+.016f,50+i*.9f),new(.07f,.02f,.25f),"amber",false,slope);
  }
  Text("STREET  ↑",new(-14,-.5f,55.96f),.024f,"cream",Quaternion.Euler(0,180,0));
  // Broad walkable room, with an open route between ramp, collection, couch, and exit.
  Box("Threadbare rug",new(-14.7f,DenFloor+.014f,60),new(5.4f,.022f,4.2f),"denRug");
  foreach(int side in new[]{-1,1}){
   Box("Rug long border",new(-14.7f+side*2.53f,DenFloor+.029f,60),new(.14f,.012f,3.95f),"denPaper");
   Box("Rug short border",new(-14.7f,DenFloor+.030f,60+side*1.93f),new(4.94f,.012f,.14f),"denPaper");
  }
  for(int i=0;i<7;i++)Box("Rug worn diamond",new(-16.65f+i*.65f,DenFloor+.032f,60),new(.25f,.008f,.25f),"denTape",false,Quaternion.Euler(0,45,0));
  // Sparse woven geometry breaks the empty carpet into a deliberate rehearsal-room pattern.
  for(int row=0;row<3;row++)for(int col=0;col<5;col++){
   var motif=new Vector3(-16.25f+col*.78f,DenFloor+.039f,58.72f+row*1.28f);
   Box("Woven rug outer lozenge",motif,new(.25f,.004f,.25f),"denTape",false,Quaternion.Euler(0,45,0));
   Box("Woven rug inset lozenge",motif+Vector3.up*.003f,new(.16f,.003f,.16f),"denRug",false,Quaternion.Euler(0,45,0));
  }
  DenArchitecturalDetail();
  DenCouch(new(-14.7f,DenFloor,62.25f));DenCoffeeTable(new(-15.3f,DenFloor,60.1f));
  DenShelf(-8.63f,57.45f,8,1);DenShelf(-19.37f,57.45f,6,-1);
  DenAmp(new(-18.65f,DenFloor,62.20f));DenGuitar(new(-17.55f,DenFloor+.06f,61.95f));
  // Low table lamp lights the collection and worn sofa, separate from ceiling practicals.
  Cylinder("Den reading lamp stem",new(-16.8f,DenFloor,62.7f),new(-16.8f,-.55f,62.7f),.03f,"iron",8);
  RingProfile(new(-16.8f,0,62.7f),new[]{-.64f,-.24f},new[]{.31f,.19f},"denPaper",16);Light(new(-16.8f,-.72f,62.7f),C(1,.73f,.45f),4.3f,3.0f);
  DenPoster(new(-17.7f,.24f,63.17f),"LOW TIDE","BASEMENT TAPES\nBALLARD / 1993",1.5f,1.72f,-4);
  DenPoster(new(-15.45f,.10f,63.17f),"STATIC BLOOM","ALL AGES\nSATURDAY / $5",1.6f,1.95f,3);
  DenPoster(new(-12.9f,.2f,63.17f),"SMELLS LIKE\nDINNER","NO COVER\nBRING EARPLUGS",1.55f,1.78f,-2);
  // Flannel draped over one couch arm; woven bands keep the reference original.
  Box("Flannel drape",new(-13.15f,DenFloor+.79f,62.20f),new(.50f,.045f,.82f),"denFlannel",false,Quaternion.Euler(0,0,-12));
  for(int i=0;i<5;i++)Box("Flannel dark check",new(-13.37f+i*.1f,DenFloor+.83f,62.20f),new(.035f,.012f,.79f),"denInk");
  for(int i=0;i<6;i++)Box("Flannel cross check",new(-13.15f,DenFloor+.842f,61.83f+i*.14f),new(.49f,.011f,.028f),"denTape");
  // Warm lamps and a bare bulb; parent controls interior atmosphere/audio/camera.
  foreach(var p in new[]{new Vector3(-18.3f,.9f,61.0f),new Vector3(-9.3f,.9f,61),new Vector3(-14,.9f,58)}){Cylinder("Den bulb cord",new(p.x,1.59f,p.z),p,.018f,"iron",5);Ellipsoid(p,new(.08f,.11f,.08f),"bulb",8,5);Light(p+Vector3.down*.15f,C(1,.67f,.34f),7,4.2f);}
  Light(new(-14,.55f,50.4f),C(1,.66f,.29f),4,2.5f);Light(new(-14,-.6f,54.6f),C(1,.66f,.29f),4,2.5f);
  Text("DEN / BASEMENT",new(-4.4f,.75f,44.6f),.020f,"cream",Quaternion.Euler(0,180,0));
  Box("Street den direction stake",new(-4.4f,.35f,44.55f),new(.06f,.7f,.06f),"wood",true);
  Spots.Add(new("welcome_snack","item_002",new(-3,.20f,43)));
 }
 static void DenCouch(Vector3 p){
  Box("Couch base",p+new Vector3(0,.30f,0),new(3.2f,.38f,.95f),"denVelvet",true);
  Box("Couch back",p+new Vector3(0,.65f,.40f),new(3.2f,.90f,.23f),"denVelvet",true);
  foreach(int side in new[]{-1,1})Box("Couch arm",p+new Vector3(side*1.53f,.51f,0),new(.25f,.72f,1.0f),"denVelvet",true);
  for(int i=-1;i<=1;i++){Ellipsoid(p+new Vector3(i*.98f,.52f,-.05f),new(.47f,.11f,.40f),"denVelvet",10,5);Box("Couch worn seam",p+new Vector3(i*.98f,.53f,-.45f),new(.86f,.018f,.018f),"denTape");}
  // Soft back pads and arm rolls retain the original collision envelope.
  for(int i=-1;i<=1;i++){
   Ellipsoid(p+new Vector3(i*.98f,.80f,.22f),new(.46f,.27f,.115f),"denVelvet",12,7);
   for(int button=0;button<2;button++)Ellipsoid(p+new Vector3(i*.98f+(button-.5f)*.35f,.81f,.103f),new(.025f,.022f,.010f),"denInk",8,4);
  }
  foreach(int side in new[]{-1,1})Ellipsoid(p+new Vector3(side*1.53f,.86f,0),new(.145f,.08f,.45f),"denVelvet",12,6);
  // Old repair patches and a folded throw give the unpurchased sofa its own history.
  Box("Sofa hand sewn patch",p+new Vector3(-.72f,.34f,-.481f),new(.24f,.13f,.008f),"denTrim",false,Quaternion.Euler(0,0,-7));
  for(int stitch=0;stitch<5;stitch++)Box("Sofa visible repair stitch",p+new Vector3(-.81f+stitch*.045f,.41f,-.49f),new(.006f,.037f,.005f),"denPaper");
  for(int fold=0;fold<5;fold++){
   Box("Old wool throw seat fold",p+new Vector3(-1.16f+fold*.055f,.655f,-.20f),new(.060f,.025f,.42f),fold%2==0?"denPaper":"denTape");
   Box("Old wool throw hanging fold",p+new Vector3(-1.16f+fold*.055f,.44f,-.455f),new(.060f,.40f,.024f),fold%2==0?"denPaper":"denTape");
  }
  foreach(int side in new[]{-1,1})foreach(int z in new[]{-1,1})Box("Couch foot",p+new Vector3(side*1.30f,.10f,z*.34f),new(.12f,.20f,.12f),"wood");
 }
 static void DenCoffeeTable(Vector3 p){
  Box("Coffee table top",p+Vector3.up*.48f,new(1.9f,.10f,.9f),"cedar",true);
  foreach(int x in new[]{-1,1})foreach(int z in new[]{-1,1})Box("Coffee table leg",p+new Vector3(x*.77f,.23f,z*.31f),new(.09f,.46f,.09f),"wood",true);
  for(int i=0;i<3;i++)DenDisplaySlots.Add(p+new Vector3(-.63f+i*.63f,.53f,0));
  // Two loose tapes remain at a corner outside the collection slots.
  for(int i=0;i<2;i++){var tape=p+new Vector3(.63f,.55f+i*.035f,.30f);Box("Cassette shell",tape,new(.24f,.03f,.15f),"denInk",false,Quaternion.Euler(0,i*12,0));Box("Cassette paper label",tape+Vector3.up*.018f,new(.19f,.006f,.08f),"denPaper");foreach(int side in new[]{-1,1})Ellipsoid(tape+new Vector3(side*.055f,.023f,0),new(.022f,.004f,.022f),"iron",8,4);}
 }
 static void DenShelf(float x,float z,int count,int side){
  float length=(count-1)*.64f+.55f;
  foreach(float y in new[]{DenFloor+.65f,DenFloor+1.25f}){
   Box("Keepsake wall shelf",new(x,y-.045f,z+length*.5f-.25f),new(.66f,.09f,length),"cedar",true);
   for(int i=0;i<count;i++)DenDisplaySlots.Add(new(x,y,z+i*.64f));
   foreach(float end in new[]{z-.06f,z+length-.44f})Box("Shelf bracket",new(x+side*.18f,y-.20f,end),new(.12f,.30f,.06f),"iron");
  }
 }
 static void DenAmp(Vector3 p){
  // A low touring case lifts the grille into the raccoon-height sightline.
  Box("Scuffed amplifier road case",p+Vector3.up*.09f,new(.86f,.18f,.54f),"denInk",true);
  foreach(int side in new[]{-1,1}){
   Box("Road case aluminum edge",p+new Vector3(side*.41f,.09f,0),new(.025f,.18f,.55f),"denSteel");
   Box("Road case corner protector",p+new Vector3(side*.37f,.10f,-.281f),new(.12f,.12f,.025f),"denSteel");
  }
  Box("Road case old gaffer label",p+new Vector3(0,.11f,-.281f),new(.26f,.065f,.012f),"denTape",false,Quaternion.Euler(0,0,-4));
  DenImportedInstrument("Amp",p+Vector3.up*.18f,.56f);
  var collider=new GameObject("Amplifier cabinet collision");collider.transform.SetParent(root,false);collider.transform.position=p+Vector3.up*.46f;collider.AddComponent<BoxCollider>().size=new(.65f,.56f,.32f);
  // Keep this collection plinth on the east lounge edge, outside the music sightline.
  var shelf=new Vector3(-11.65f,DenFloor,61.20f);Crate(shelf+Vector3.up*.26f,new(.67f,.52f,.64f));DenDisplaySlots.Add(shelf+Vector3.up*.52f);
 }
 static void DenGuitar(Vector3 p){
  DenImportedInstrument("Guitar",p,1.10f);
  var basePoint=new Vector3(p.x,DenFloor,p.z);Cylinder("Guitar stand upright",basePoint,basePoint+Vector3.up*.60f,.015f,"iron",8);
  foreach(int side in new[]{-1,1}){Cylinder("Guitar stand foot",basePoint+Vector3.up*.12f,basePoint+new Vector3(side*.20f,0,-.16f),.016f,"iron",8);Cylinder("Guitar stand padded cradle",basePoint+new Vector3(0,.13f,0),basePoint+new Vector3(side*.13f,.13f,-.05f),.021f,"iron",8);}
 }
 static void DenPoster(Vector3 p,string title,string subtitle,float width,float height,float tilt){
  var q=Quaternion.Euler(0,0,tilt);Box("Original basement gig flyer",p,new(width,height,.022f),"denPaper",false,q);
  Box("Flyer ink field",p+q*new Vector3(0,.18f,-.016f),new(width*.88f,height*.52f,.006f),"denInk",false,q);
  // Torn-ring artwork: original angular rays, not an existing band logo or album cover.
  if(title=="LOW TIDE"){
   for(int wave=0;wave<3;wave++)for(int segment=0;segment<12;segment++){float x=(segment-5.5f)*width*.06f;var at=p+q*new Vector3(x,.05f+wave*.13f+Mathf.Sin(segment*.65f+wave)*.06f,-.025f);Box("Flyer cut paper wave",at,new(width*.065f,.027f,.008f),"denPaper",false,q*Quaternion.Euler(0,0,Mathf.Cos(segment*.65f+wave)*20));}
  }else if(title.Contains("DINNER")){
   Ellipsoid(p+q*new Vector3(0,.16f,-.031f),new(width*.23f,width*.23f,.005f),"denPaper",28,6);
   Ellipsoid(p+q*new Vector3(0,.16f,-.041f),new(width*.065f,width*.065f,.005f),"denInk",16,6);
  }else for(int i=0;i<9;i++){float a=i*40*Mathf.Deg2Rad;var at=p+q*new Vector3(Mathf.Sin(a)*width*.25f,.1f+Mathf.Cos(a)*height*.15f,-.024f);Box("Flyer xerox ray",at,new(.055f,.29f,.008f),"denPaper",false,q*Quaternion.Euler(0,0,-i*40+20));}
  Text(title,p+q*new Vector3(0,height*.34f,-.032f),.027f,"cream",q);
  Text(subtitle,p+q*new Vector3(0,-height*.34f,-.034f),.015f,"denInk",q);
  foreach(int side in new[]{-1,1})Box("Flyer masking tape",p+q*new Vector3(side*width*.37f,height*.48f,-.031f),new(.16f,.075f,.012f),"denTape",false,q*Quaternion.Euler(0,0,side*16));
 }
}}
