using UnityEngine;
namespace Jimothy {
public static partial class ClosingTimeWorld {
 // Reserved: street-facing vent/loot lane, chimney find, north entry, crossing ramps.
 // Furnishings use outward half u[0,4.2], local z[-4.9,0]; lookout gets a narrow outer strip.
 static void RoofPersonalization(){
  Mat("roofCanvas",C(.62f,.59f,.44f),.04f);Mat("roofTeal",C(.12f,.32f,.31f),.07f);Mat("roofClay",C(.60f,.29f,.17f),.06f);Mat("roofMat",C(.30f,.37f,.32f),.015f);Mat("roofEnamel",C(.75f,.77f,.65f),.28f);Mat("roofSteel",C(.29f,.34f,.33f),.62f,.65f);Mat("roofInk",C(.075f,.10f,.095f),.02f);
  foreach(int side in new[]{-1,1})for(int row=0;row<5;row++){
   float z=-30+row*16,y=new[]{7.2f,8.1f,9f,7.2f,8.1f}[row]+.18f;
   Vector3 P(float u,float v)=>new(side*(14.6f+u),y,z+v);
   RoofLantern(P(3.8f,-4.9f));
   RoofActivitySetting(side,row,P(2.1f,-2.45f));
   if(side<0&&row==0){
    for(int i=0;i<3;i++){var p=P(.4f+i*1.25f,-2.7f);Box("Yoga woven mat",p+Vector3.up*.018f,new(.72f,.03f,1.85f),i==1?"roofTeal":"roofMat");for(int stripe=0;stripe<9;stripe++)Box("Yoga mat rib",p+new Vector3(0,.035f,-.8f+stripe*.2f),new(.66f,.004f,.015f),"roofCanvas");Box("Cork yoga block",p+new Vector3(.27f,.065f,.75f),new(.18f,.12f,.12f),"wood");RoofBottle(p+new Vector3(-.25f,0,.82f));}
    RoofBench(P(2,-.45f),0,2.5f,"roofCanvas");RoofBasket(P(3.8f,-4.25f));RoofBoard(P(.35f,-4.7f),"BREATHE\nSUNRISE YOGA",0);
   }else if(side>0&&row==0){
    RoofOven(P(2.1f,-4.35f),180);RoofKitchen(P(2.2f,-1.4f),0);RoofStool(P(.3f,-2.6f));RoofStool(P(.3f,-3.5f));RoofBoard(P(3.9f,-.4f),"ROOF SOCIAL\nPIZZA AT SIX",0);
   }else if(side<0&&row==1){
    RoofTable(P(1.8f,-2.5f),2.8f,1.05f);for(int i=-1;i<=1;i++)foreach(int edge in new[]{-1,1})RoofChair(P(1.8f+i*.95f,-2.5f+edge*1.05f),edge>0?180:0,"roofCanvas");
    for(int i=-1;i<=1;i++){var p=P(1.8f+i*.85f,-2.5f)+Vector3.up*.80f;Box("Open work notebook",p,new(.34f,.016f,.25f),"paper",false,Quaternion.Euler(0,i*9,0));RoofCup(p+new Vector3(.27f,0,.1f));}RoofBoard(P(3.9f,-.5f),"LUNCH + IDEAS\nOUT OF OFFICE",0);
   }else if(side>0&&row==1){
    RoofBench(P(2,-3.9f),0,2.6f,"roofTeal");RoofChair(P(.4f,-1.5f),35,"roofCanvas");RoofTable(P(2,-2.1f),1.4f,.70f,.42f);
    var record=P(3.7f,-1.1f);Box("Record cabinet",record+Vector3.up*.38f,new(.75f,.76f,.65f),"wood",true);Box("Portable turntable",record+Vector3.up*.81f,new(.56f,.1f,.44f),"roofInk");RoofDisc(record+Vector3.up*.87f,.17f,"iron");Cylinder("Record player tone arm",record+new Vector3(.21f,.90f,.15f),record+new Vector3(.03f,.89f,0),.013f,"roofSteel",6);RoofBoard(P(.4f,-4.6f),"SIDE B\nLISTENING ROOF",0);
   }else if(side<0&&row==2){
    RoofEasel(P(.7f,-2.4f),-18);RoofEasel(P(2.9f,-1.6f),15);RoofTable(P(2.1f,-4.2f),2.8f,.75f);
    for(int i=0;i<5;i++){var p=P(1.1f+i*.45f,-4.2f)+Vector3.up*.79f;RoofCup(p);Cylinder("Artist brush",p+Vector3.up*.08f,p+new Vector3(.045f,.36f,0),.009f,"wood",5);}RoofStool(P(1.8f,-3.1f));
   }else if(side>0&&row==2){
    RoofTable(P(2.1f,-4.3f),2.8f,.8f);for(int i=0;i<4;i++)RoofPot(P(1.1f+i*.62f,-4.3f)+Vector3.up*.79f,.16f);
    RoofBed(P(.8f,-1.8f),new(1.1f,.45f,1.85f));RoofBed(P(2.85f,-1.8f),new(1.1f,.45f,1.85f));
    foreach(float u in new[]{.8f,2.85f}){var p=P(u,-.7f);for(int i=-1;i<=1;i++)Box("Garden trellis upright",p+new Vector3(i*.4f,.9f,0),new(.035f,1.8f,.035f),"cedar");for(int i=0;i<5;i++)Box("Garden trellis crosspiece",p+new Vector3(0,.4f+i*.3f,0),new(1.1f,.035f,.035f),"cedar");}RoofBoard(P(3.9f,-4.7f),"SHARE THE HARVEST",0);
   }else if(side<0&&row==3){
    RoofRoundTable(P(.8f,-3.8f));RoofRoundTable(P(2.8f,-1.5f));RoofChair(P(.8f,-4.8f),0,"roofClay");RoofChair(P(.8f,-2.8f),180,"roofClay");RoofChair(P(2.8f,-2.5f),0,"roofTeal");RoofChair(P(2.8f,-.5f),180,"roofTeal");
    foreach(var p in new[]{P(.8f,-3.8f),P(2.8f,-1.5f)}){RoofDisc(p+Vector3.up*.81f,.18f,"roofEnamel");RoofCup(p+new Vector3(.25f,.81f,0));RoofBottle(p+new Vector3(-.2f,.81f,.18f));}
   }else if(side>0&&row==3){
    RoofTable(P(1.6f,-2.5f),1.2f,1.1f,.68f);for(int x=0;x<8;x++)for(int v=0;v<8;v++)Box("Chess square",P(1.6f,-2.5f)+new Vector3((x-3.5f)*.10f,.738f,(v-3.5f)*.10f),new(.098f,.008f,.098f),(x+v)%2==0?"roofInk":"roofEnamel");
    for(int i=0;i<4;i++){var p=P(1.6f,-2.5f)+new Vector3((i-1.5f)*.1f,.75f,-.25f);Ellipsoid(p+Vector3.up*.045f,new(.023f,.045f,.023f),"wood",7,4);}RoofChair(P(1.6f,-3.65f),0,"roofCanvas");RoofChair(P(1.6f,-1.35f),180,"roofCanvas");RoofBench(P(3.8f,-2.5f),90,2,"roofTeal");RoofBoard(P(.4f,-4.6f),"ONE MORE GAME",0);
   }else if(side<0&&row==4){
    // Existing lookout occupies u[-1.7,1.7], z[-4.5,-1.5]. Preserve its deck/find.
    RoofBench(P(3.65f,-2.4f),90,1.8f,"roofCanvas");RoofBoard(P(3.55f,-4.25f),"NORTH SKY\nFIELD NOTES",0);RoofPot(P(3.65f,-.9f),.3f);
   }else{
    RoofLounger(P(.7f,-2.5f),-8);RoofLounger(P(2.8f,-2.5f),8);RoofRoundTable(P(1.75f,-.7f),.35f,.48f);RoofCup(P(1.75f,-.7f)+Vector3.up*.49f);RoofBasket(P(3.9f,-4.5f));RoofBoard(P(.35f,-4.7f),"SLOW MORNINGS\nCOFFEE CLUB",0);
   }
  }
 }
 static void RoofActivitySetting(int side,int row,Vector3 center){
  if(side<0&&row==4)return; // Existing lookout owns this patch.
  // Only 12mm above the roof, inset from all reserved access lanes.
  for(int j=0;j<13;j++)Box("Weathered activity deck board",center+new Vector3(0,.006f,-2.22f+j*.36f),new(4.15f,.012f,.345f),"cedar");
  foreach(int edge in new[]{-1,1}){var p=center+new Vector3(side*1.7f,0,edge*1.75f);RoofPot(p,.21f);Hosta(p+Vector3.up*.32f,.73f);}
  if(row==0||row==1){
   Vector3 A=center+new Vector3(side*1.95f,0,-2.2f),B=center+new Vector3(side*1.95f,0,2.2f);
   RoofTube("Terrace light support",A,A+Vector3.up*2.55f,.025f,"iron",true);RoofTube("Terrace light support",B,B+Vector3.up*2.55f,.025f,"iron",true);
   Vector3 At(float t)=>Vector3.Lerp(A,B,t)+Vector3.up*(2.55f-Mathf.Sin(t*Mathf.PI)*.32f);
   for(int k=0;k<10;k++){RoofTube("Terrace festoon cable",At(k/10f),At((k+1)/10f),.008f,"iron");Ellipsoid(At((k+.5f)/10f)-Vector3.up*.08f,new(.032f,.046f,.032f),"bulb",8,4);}
   Light((A+B)*.5f+Vector3.up*2.15f,C(1,.74f,.43f),5.8f,6.0f);
  }
 }
 static void RoofMeshCollider(string name,Batch batch,int firstVertex,int firstTriangle){var mesh=new Mesh{name=name+" collision mesh"};mesh.SetVertices(batch.v.GetRange(firstVertex,batch.v.Count-firstVertex));var triangles=batch.t.GetRange(firstTriangle,batch.t.Count-firstTriangle);for(int i=0;i<triangles.Count;i++)triangles[i]-=firstVertex;mesh.SetTriangles(triangles,0);mesh.RecalculateBounds();createdMeshes.Add(mesh);var go=new GameObject(name+" collision");go.transform.SetParent(root,false);go.AddComponent<MeshCollider>().sharedMesh=mesh;}
 static void RoofTube(string name,Vector3 a,Vector3 b,float radius,string material,bool solid=false){Cylinder(name,a,b,radius,material,8);if(solid){var go=new GameObject(name+" collision");go.transform.SetParent(root,false);go.transform.localPosition=(a+b)*.5f;go.transform.localRotation=Quaternion.FromToRotation(Vector3.up,(b-a).normalized);var c=go.AddComponent<CapsuleCollider>();c.radius=radius;c.height=(b-a).magnitude;c.direction=1;}}
 static void RoofChair(Vector3 p,float yaw,string fabric){var q=Quaternion.Euler(0,yaw,0);Vector3 At(Vector3 v)=>p+q*v;Box("Outdoor chair seat",At(new(0,.44f,0)),new(.48f,.07f,.47f),"cedar",true,q);Box("Woven chair cushion",At(new(0,.49f,0)),new(.43f,.04f,.42f),fabric,false,q);for(int i=0;i<4;i++)Box("Chair slatted back",At(new(0,.63f+i*.09f,.21f)),new(.47f,.055f,.055f),"cedar",true,q);foreach(int x in new[]{-1,1})foreach(int z in new[]{-1,1})RoofTube("Chair angled leg",At(new(x*.22f,0,z*.22f)),At(new(x*.17f,.44f,z*.17f)),.025f,"iron",true);}
 static void RoofBench(Vector3 p,float yaw,float length,string fabric){var q=Quaternion.Euler(0,yaw,0);Vector3 At(Vector3 v)=>p+q*v;Box("Roof bench seat",At(new(0,.42f,0)),new(length,.12f,.65f),"cedar",true,q);Box("Roof bench cushion",At(new(0,.51f,-.02f)),new(length-.16f,.07f,.55f),fabric,false,q);for(int i=0;i<4;i++)Box("Bench back slat",At(new(0,.62f+i*.1f,.30f)),new(length,.065f,.06f),"cedar",true,q);foreach(int side in new[]{-1,1})Box("Bench iron sled foot",At(new(side*(length*.5f-.2f),.20f,0)),new(.065f,.4f,.62f),"iron",true,q);}
 static void RoofTable(Vector3 p,float length,float depth,float top=.78f){Box("Shared table slab",p+Vector3.up*(top-.05f),new(length,.1f,depth),"cedar",true);foreach(int x in new[]{-1,1})foreach(int z in new[]{-1,1})RoofTube("Table steel leg",p+new Vector3(x*(length*.5f-.15f),0,z*(depth*.5f-.12f)),p+new Vector3(x*(length*.5f-.2f),top-.1f,z*(depth*.5f-.12f)),.035f,"iron",true);for(int i=1;i<5;i++)Box("Table board joint",p+new Vector3(0,top+.003f,(i/5f-.5f)*depth),new(length-.03f,.007f,.009f),"roofInk");}
 static void RoofRoundTable(Vector3 p,float radius=.54f,float top=.80f){RoofDisc(p+Vector3.up*(top-.035f),radius,"cedar",.07f,true);RoofTube("Bistro pedestal",p,p+Vector3.up*(top-.07f),.045f,"iron",true);foreach(int side in new[]{-1,1})RoofTube("Bistro splayed foot",p+Vector3.up*.1f,p+new Vector3(side*.35f,.025f,0),.025f,"iron",true);}
 static void RoofStool(Vector3 p){RoofDisc(p+Vector3.up*.68f,.23f,"cedar",.08f,true);foreach(int i in new[]{0,1,2,3}){float a=i*Mathf.PI*.5f;RoofTube("Bar stool leg",p+new Vector3(Mathf.Sin(a)*.21f,0,Mathf.Cos(a)*.21f),p+new Vector3(Mathf.Sin(a)*.14f,.64f,Mathf.Cos(a)*.14f),.023f,"iron",true);}}
 static void RoofDisc(Vector3 p,float radius,string material,float thickness=.012f,bool solid=false){var b=GetBatch(material,p);int firstVertex=b.v.Count,firstTriangle=b.t.Count;const int n=16;for(int i=0;i<n;i++){float a=i*Mathf.PI*2/n,a2=(i+1)*Mathf.PI*2/n;int k=b.v.Count;b.v.AddRange(new[]{p+Vector3.up*thickness*.5f,p+new Vector3(Mathf.Sin(a)*radius,thickness*.5f,Mathf.Cos(a)*radius),p+new Vector3(Mathf.Sin(a2)*radius,thickness*.5f,Mathf.Cos(a2)*radius)});b.n.AddRange(new[]{Vector3.up,Vector3.up,Vector3.up});b.uv.AddRange(new[]{new Vector2(.5f,.5f),new Vector2((Mathf.Sin(a)+1)*.5f,(Mathf.Cos(a)+1)*.5f),new Vector2((Mathf.Sin(a2)+1)*.5f,(Mathf.Cos(a2)+1)*.5f)});b.t.AddRange(new[]{k,k+1,k+2});}for(int i=0;i<n;i++){float a=i*Mathf.PI*2/n,a2=(i+1)*Mathf.PI*2/n;int k=b.v.Count;b.v.AddRange(new[]{p-Vector3.up*thickness*.5f,p+new Vector3(Mathf.Sin(a2)*radius,-thickness*.5f,Mathf.Cos(a2)*radius),p+new Vector3(Mathf.Sin(a)*radius,-thickness*.5f,Mathf.Cos(a)*radius)});b.n.AddRange(new[]{Vector3.down,Vector3.down,Vector3.down});b.uv.AddRange(new[]{Vector2.zero,Vector2.right,Vector2.up});b.t.AddRange(new[]{k,k+1,k+2});}Cylinder("Round table edge",p-Vector3.up*thickness*.5f,p+Vector3.up*thickness*.5f,radius,material,16);if(solid)RoofMeshCollider("Round furniture",b,firstVertex,firstTriangle);}
 static void RoofLantern(Vector3 p){Box("Shielded roof lantern base",p+Vector3.up*.04f,new(.23f,.08f,.23f),"iron",true);Box("Roof lantern cap",p+Vector3.up*.36f,new(.26f,.05f,.26f),"iron");foreach(int x in new[]{-1,1})foreach(int z in new[]{-1,1})RoofTube("Lantern corner",p+new Vector3(x*.09f,.07f,z*.09f),p+new Vector3(x*.09f,.34f,z*.09f),.012f,"iron");Ellipsoid(p+Vector3.up*.20f,new(.055f,.10f,.055f),"bulb",8,5);Light(p+Vector3.up*.28f,C(1,.70f,.39f),5.0f,4.6f);}
 static void RoofCup(Vector3 p){RoofTube("Enamel cup",p,p+Vector3.up*.12f,.055f,"roofEnamel");RoofDisc(p+Vector3.up*.121f,.045f,"roofInk");RoofTube("Cup handle",p+new Vector3(.052f,.09f,0),p+new Vector3(.09f,.04f,0),.012f,"roofEnamel");}
 static void RoofBottle(Vector3 p){RoofTube("Reusable bottle",p,p+Vector3.up*.25f,.047f,"roofTeal");RoofTube("Bottle cap",p+Vector3.up*.25f,p+Vector3.up*.28f,.035f,"roofSteel");}
 static void RoofPot(Vector3 p,float radius){var b=GetBatch("roofClay",p);int firstVertex=b.v.Count,firstTriangle=b.t.Count;RoofTube("Terracotta pot",p,p+Vector3.up*radius*1.5f,radius,"roofClay");RoofMeshCollider("Terracotta pot",b,firstVertex,firstTriangle);RoofDisc(p+Vector3.up*(radius*1.5f+.002f),radius*.88f,"moss",.012f,true);Fern(p+Vector3.up*radius*1.5f,radius*1.3f);}
 static void RoofBasket(Vector3 p){Box("Wicker blanket basket",p+Vector3.up*.22f,new(.55f,.44f,.40f),"wood",true);for(int i=0;i<6;i++)Box("Basket wicker weave",p+new Vector3(0,.05f+i*.065f,-.207f),new(.55f,.015f,.015f),"roofCanvas");Box("Folded throw",p+new Vector3(0,.47f,0),new(.45f,.075f,.34f),"roofTeal");}
 static void RoofBed(Vector3 p,Vector3 size){Box("Raised edible garden bed",p+Vector3.up*size.y*.5f,size,"cedar",true);Box("Garden soil",p+Vector3.up*(size.y+.008f),new(size.x-.12f,.02f,size.z-.12f),"moss");for(int i=0;i<5;i++)Fern(p+new Vector3((i%2-.5f)*size.x*.45f,size.y+.025f,-size.z*.35f+i*size.z*.17f),.35f);}
 static void RoofBoard(Vector3 p,string words,float yaw){var q=Quaternion.Euler(0,yaw+180,0);Box("Handwritten rooftop board",p+Vector3.up*.65f,new(.70f,.65f,.045f),"roofInk",true,q);RoofTube("Board stand",p,p+Vector3.up*.6f,.028f,"cedar",true);Text(words,p+Vector3.up*.65f+q*new Vector3(0,0,-.028f),.012f,"cream",q);}
 static void RoofLounger(Vector3 p,float yaw){var q=Quaternion.Euler(0,yaw,0);Box("Reclining cedar frame",p+Vector3.up*.28f,new(.70f,.10f,1.65f),"cedar",true,q);Box("Lounger cushion",p+Vector3.up*.36f,new(.64f,.07f,1.1f),"roofTeal",false,q);var back=q*Quaternion.Euler(-35,0,0);Box("Angled lounger back",p+q*new Vector3(0,.64f,.65f),new(.70f,.08f,.90f),"cedar",true,back);foreach(int x in new[]{-1,1})foreach(int z in new[]{-1,1})Box("Lounger foot",p+q*new Vector3(x*.27f,.12f,z*.6f),new(.06f,.24f,.06f),"iron",true,q);}
 static void RoofEasel(Vector3 p,float yaw){var q=Quaternion.Euler(0,yaw,0);Vector3 At(Vector3 v)=>p+q*v;foreach(int side in new[]{-1,1})RoofTube("Easel angled leg",At(new(side*.38f,0,0)),At(new(side*.21f,1.7f,0)),.026f,"cedar",true);RoofTube("Easel rear leg",At(new(0,0,.5f)),At(new(0,1.5f,0)),.026f,"cedar",true);Box("Artist canvas",At(new(0,1.08f,-.045f)),new(.65f,.85f,.035f),"roofEnamel",true,q);for(int i=0;i<6;i++)Box("Original painted color study",At(new((i%2-.5f)*.28f,.83f+(i/2)*.23f,-.068f)),new(.26f,.18f,.008f),i%3==0?"roofTeal":i%3==1?"roofClay":"roofMat",false,q);}
 static void RoofKitchen(Vector3 p,float yaw){var q=Quaternion.Euler(0,yaw,0);Vector3 At(Vector3 v)=>p+q*v;Box("Outdoor kitchen cabinet",At(new(0,.47f,0)),new(2.3f,.94f,.62f),"cedar",true,q);Box("Stone kitchen worktop",At(new(0,.98f,0)),new(2.45f,.08f,.72f),"stone",true,q);for(int i=0;i<4;i++){Box("Cabinet door joint",At(new(-.9f+i*.6f,.45f,-.317f)),new(.018f,.78f,.012f),"roofInk",false,q);Box("Cabinet pull",At(new(-.67f+i*.6f,.70f,-.34f)),new(.15f,.026f,.022f),"roofSteel",false,q);}
  Box("Inset sink bowl",At(new(-.58f,1.026f,0)),new(.55f,.018f,.43f),"roofSteel",false,q);Box("Sink dark basin",At(new(-.58f,1.038f,0)),new(.42f,.01f,.31f),"roofInk",false,q);RoofTube("Sink tap upright",At(new(-.58f,1.02f,.27f)),At(new(-.58f,1.33f,.27f)),.022f,"roofSteel");RoofTube("Sink tap spout",At(new(-.58f,1.33f,.27f)),At(new(-.58f,1.33f,.02f)),.022f,"roofSteel");Box("Pizza chopping board",At(new(.57f,1.034f,0)),new(.72f,.025f,.46f),"wood",false,q);RoofDisc(At(new(.57f,1.058f,0)),.18f,"roofClay");RoofBottle(At(new(1,1.02f,.15f)));}
 static void RoofOven(Vector3 p,float yaw){var q=Quaternion.Euler(0,yaw,0);Vector3 At(Vector3 v)=>p+q*v;Box("Pizza oven masonry base",At(new(0,.48f,0)),new(1.5f,.96f,1.25f),"brickRust",true,q);Box("Pizza oven hearth",At(new(0,1,0)),new(1.72f,.12f,1.50f),"stone",true,q);
  var center=At(new(0,1.06f,0));var b=GetBatch("roofClay",center);int firstVertex=b.v.Count,firstTriangle=b.t.Count;const int n=20,rings=7;
  for(int j=0;j<rings;j++)for(int i=0;i<n;i++){float a=(i+.5f)*Mathf.PI*2/n,v=(j+.5f)*Mathf.PI*.5f/rings;if(Mathf.Cos(a)<-.83f&&Mathf.Sin(v)<.50f)continue;int k=b.v.Count;foreach(var uv in new[]{new Vector2(i,j),new Vector2(i+1,j),new Vector2(i+1,j+1),new Vector2(i,j+1)}){float aa=uv.x*Mathf.PI*2/n,vv=uv.y*Mathf.PI*.5f/rings;var normal=new Vector3(Mathf.Sin(aa)*Mathf.Cos(vv),Mathf.Sin(vv),Mathf.Cos(aa)*Mathf.Cos(vv));b.v.Add(center+q*Vector3.Scale(normal,new(.76f,.72f,.67f)));b.n.Add(q*normal);b.uv.Add(new(uv.x/n,uv.y/rings));}b.t.AddRange(new[]{k,k+1,k+2,k,k+2,k+3});}
  // Solid upper shell and side cheeks preserve a real open firebox mouth.
  RoofMeshCollider("Pizza oven open dome",b,firstVertex,firstTriangle);foreach(int side in new[]{-1,1})Box("Oven firebox cheek",At(new(side*.54f,1.27f,0)),new(.25f,.42f,.95f),"roofClay",true,q);
  Box("Dark oven rear",At(new(0,1.26f,.45f)),new(.75f,.4f,.08f),"roofInk",true,q);RoofTube("Oven flue",At(new(0,1.57f,.28f)),At(new(0,2.35f,.28f)),.105f,"iron",true);RoofDisc(At(new(0,2.35f,.28f)),.14f,"iron",.05f);Box("Glowing embers",At(new(0,1.074f,.05f)),new(.60f,.018f,.35f),"amber");
  for(int i=0;i<7;i++){float a=i*Mathf.PI/6;Box("Oven brick arch",At(new(Mathf.Cos(a)*.36f,1.13f+Mathf.Sin(a)*.38f,-.61f)),new(.17f,.15f,.18f),"terracotta",false,q*Quaternion.Euler(0,0,a*Mathf.Rad2Deg-90));}
  RoofTube("Pizza peel handle",At(new(.95f,0,-.2f)),At(new(.85f,1.15f,-.1f)),.021f,"wood");Box("Pizza peel paddle",At(new(.85f,1.32f,-.1f)),new(.32f,.40f,.035f),"roofSteel",false,q);
 }
}}
