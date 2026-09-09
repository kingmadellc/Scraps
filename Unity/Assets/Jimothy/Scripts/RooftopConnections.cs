using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
namespace Jimothy {
/// <summary>Optional roof-to-roof loops, clear of the tested alley entry routes.</summary>
public static class RooftopConnections {
 public static readonly List<Vector3[]> Paths=new();
 static Material timber,rope,metal,cloth,leaf,bulb,denim,rustCloth;
 static Transform root; static Mesh cube,cylinder,globe;
 static readonly Mesh[] garments=new Mesh[3];
 static readonly List<CombineInstance> planks=new(),ropes=new(),metals=new(),fabrics=new(),leaves=new(),bulbs=new(),blueLaundry=new(),redLaundry=new();
 static Material Make(string name,Color color){var m=new Material(Shader.Find("Universal Render Pipeline/Lit")){name=name,enableInstancing=true};m.SetColor("_BaseColor",color);m.SetFloat("_Smoothness",.22f);return m;}
 static void Piece(Vector3 p,Vector3 size,Quaternion rotation,List<CombineInstance> batch,bool solid=false){
  // Reuse Unity's cube mesh; decorative components are combined into three material draws.
  batch.Add(new CombineInstance{mesh=cube,transform=Matrix4x4.TRS(p,rotation,size)});
  if(solid){var go=new GameObject("Rooftop bridge walkable deck");go.transform.SetParent(root,false);go.transform.localPosition=p;go.transform.localRotation=rotation;go.AddComponent<BoxCollider>().size=size;}
 }
 static void Beam(Vector3 a,Vector3 b,float width,List<CombineInstance> batch){Piece((a+b)*.5f,new Vector3(width,(b-a).magnitude,width),Quaternion.FromToRotation(Vector3.up,b-a),batch);}
 static void Bake(List<CombineInstance> pieces,Material material,string name,List<Mesh> owned){if(pieces.Count==0)return;var mesh=new Mesh{name=name};mesh.indexFormat=IndexFormat.UInt32;mesh.CombineMeshes(pieces.ToArray(),true,true);owned.Add(mesh);var go=new GameObject(name);go.transform.SetParent(root,false);go.AddComponent<MeshFilter>().sharedMesh=mesh;go.AddComponent<MeshRenderer>().sharedMaterial=material;}
 static Mesh GarmentMesh(int kind){
  if(garments[kind])return garments[kind];
  float[] widths=kind==0?new[]{.14f,.33f,.31f,.19f,.19f,.20f}:kind==1?new[]{.12f,.12f,.115f,.12f,.21f,.22f}:new[]{.26f,.27f,.27f,.26f,.27f,.26f};
  float[] ys=kind==0?new[]{0f,-.08f,-.23f,-.28f,-.48f,-.64f}:kind==1?new[]{0f,-.10f,-.24f,-.35f,-.43f,-.48f}:new[]{0f,-.14f,-.30f,-.45f,-.61f,-.74f};
  var vertices=new List<Vector3>();var uv=new List<Vector2>();var triangles=new List<int>();const int columns=8;
  for(int row=0;row<6;row++)for(int x=0;x<=columns;x++){float u=x/(float)columns,t=row/5f;float px=(u*2-1)*widths[row]+(kind==1&&row>3?.09f:0);float y=ys[row];if(kind==0&&row==0)y-=Mathf.Sin(u*Mathf.PI)*.055f;float fold=Mathf.Sin(u*Mathf.PI*4+.4f)*(.013f+t*.016f)+t*t*.045f;vertices.Add(new(px,y,fold));uv.Add(new(u,t));if(row<5&&x<columns){int i=row*(columns+1)+x;triangles.AddRange(new[]{i,i+columns+1,i+1,i+1,i+columns+1,i+columns+2});}}
  var mesh=new Mesh{name=kind==0?"Draped short sleeve shirt":kind==1?"Knitted hanging sock":"Folded hanging towel"};mesh.SetVertices(vertices);mesh.SetUVs(0,uv);mesh.SetTriangles(triangles,0);mesh.RecalculateNormals();mesh.RecalculateBounds();garments[kind]=mesh;return mesh;
 }
 static void Laundry(Vector3 anchor,Quaternion rotation,int variant){
  int kind=variant%3;var batch=variant%4==0?blueLaundry:variant%4==1?redLaundry:fabrics;var scale=Vector3.one*(kind==1?.9f:1f);
  batch.Add(new CombineInstance{mesh=GarmentMesh(kind),transform=Matrix4x4.TRS(anchor,rotation,scale)});
  // Two real wooden pegs touch the line and garment shoulder/top edge.
  foreach(int side in new[]{-1,1})Piece(anchor+rotation*new Vector3(side*(kind==1?.085f:kind==0?.115f:.22f),-.022f,0),new(.035f,.075f,.028f),rotation,planks);
  if(kind==1){blueLaundry.Add(new CombineInstance{mesh=GarmentMesh(1),transform=Matrix4x4.TRS(anchor+rotation*Vector3.right*.27f,rotation,scale)});Piece(anchor+rotation*new Vector3(.27f,-.022f,0),new(.035f,.075f,.028f),rotation,planks);}
 }
 static Mesh GlobeMesh(){if(globe)return globe;var v=new List<Vector3>();var n=new List<Vector3>();var t=new List<int>();const int slices=12,rings=7;for(int y=0;y<=rings;y++)for(int x=0;x<=slices;x++){float a=x*2*Mathf.PI/slices,b=y*Mathf.PI/rings;var p=new Vector3(Mathf.Sin(b)*Mathf.Cos(a),Mathf.Cos(b),Mathf.Sin(b)*Mathf.Sin(a))*.5f;v.Add(p);n.Add(p.normalized);if(y<rings&&x<slices){int i=y*(slices+1)+x;t.AddRange(new[]{i,i+1,i+slices+1,i+1,i+slices+2,i+slices+1});}}globe=new Mesh{name="Low polygon smooth globe bulb"};globe.SetVertices(v);globe.SetNormals(n);globe.SetTriangles(t,0);globe.RecalculateBounds();return globe;}
 static void Tube(Vector3 a,Vector3 b,float radius,List<CombineInstance> batch){batch.Add(new CombineInstance{mesh=cylinder,transform=Matrix4x4.TRS((a+b)*.5f,Quaternion.FromToRotation(Vector3.up,b-a),new Vector3(radius*2,(b-a).magnitude*.5f,radius*2))});}
 static void Surface(Vector3 a,Vector3 b,float width,float thickness,List<CombineInstance> batch,string name){var q=Quaternion.FromToRotation(Vector3.forward,b-a);var size=new Vector3(width,thickness,(b-a).magnitude);var center=(a+b)*.5f-q*Vector3.up*thickness*.5f;
  // Visible spans meet at their authored ends; only collision volumes overlap for safe seams.
  Piece(center,size,q,batch,false);var support=new GameObject(name);support.transform.SetParent(root,false);support.transform.localPosition=center;support.transform.localRotation=q;support.AddComponent<BoxCollider>().size=size+Vector3.forward*.10f;}
 static void Festoon(Vector3 a,Vector3 b,bool lights,float postHeight=1.4f){
  if(lights){var go=new GameObject("Rooftop festoon warm pool");go.transform.SetParent(root,false);go.transform.localPosition=(a+b)*.5f-Vector3.up*.52f; // .22m cable sag + .30m below the midpoint wire.
   var light=go.AddComponent<Light>();light.type=LightType.Point;light.range=4.5f;light.intensity=3.5f;light.color=new Color(1f,.76f,.46f);light.shadows=LightShadows.None;go.AddComponent<ClosingLight>();
  }
  foreach(var end in new[]{a,b})Beam(end-Vector3.up*postHeight,end,.045f,metals);
  Vector3 previous=a;for(int i=1;i<=18;i++){float t=i/18f;var p=Vector3.Lerp(a,b,t)-Vector3.up*Mathf.Sin(t*Mathf.PI)*.22f;Tube(previous,p,.016f,ropes);if(lights&&i%2==0){Tube(p,p-Vector3.up*.12f,.015f,metals);bulbs.Add(new CombineInstance{mesh=GlobeMesh(),transform=Matrix4x4.TRS(p-Vector3.up*.16f,Quaternion.identity,new Vector3(.115f,.13f,.115f))});}previous=p;}
 }
 static void RoofLife(int side,int row){
  if(side<0&&row==4)return; // Existing lookout already occupies this back-roof cluster.
  float z=-30+row*16,h=new[]{7.2f,8.1f,9f,7.2f,8.1f}[row];var p=new Vector3(side*16.8f,h+.18f,z-2.8f);
  // Back-half clusters avoid avenue-facing crossover ramps, trophy deck and north alley landing.
  if(row%3==0){
   foreach(float offset in new[]{-.65f,.65f}){Piece(p+new Vector3(offset,.22f,0),new(.9f,.44f,.72f),Quaternion.identity,planks,true);Piece(p+new Vector3(offset,.46f,0),new(.79f,.03f,.61f),Quaternion.identity,metals);for(int i=0;i<7;i++){var plant=p+new Vector3(offset+Mathf.Sin(i*2.4f)*.26f,.51f,Mathf.Cos(i*2.4f)*.18f);Tube(plant,plant+new Vector3(.07f,.28f,.03f),.008f,ropes);for(int j=0;j<3;j++)Piece(plant+new Vector3(.06f,.09f+j*.07f,0),new(.06f,.025f,.20f),Quaternion.Euler(-20,i*53+j*45,25),leaves);}}
   Festoon(p+new Vector3(-1.3f,1.75f,-.35f),p+new Vector3(1.3f,1.75f,-.35f),true,1.75f);
  }else if(row%3==1){
   Piece(p+new Vector3(0,.37f,0),new(1.7f,.10f,.55f),Quaternion.identity,planks,true);Piece(p+new Vector3(0,.68f,-.25f),new(1.7f,.58f,.08f),Quaternion.Euler(-8,0,0),planks);
   foreach(int edge in new[]{-1,1})Piece(p+new Vector3(edge*.65f,.17f,0),new(.07f,.34f,.43f),Quaternion.identity,metals);Piece(p+new Vector3(1.1f,.31f,.5f),new(.55f,.62f,.55f),Quaternion.identity,planks,true);Piece(p+new Vector3(.4f,.45f,0),new(.50f,.07f,.44f),Quaternion.identity,fabrics);
  }else{
   Piece(p+new Vector3(0,.34f,0),new(1.4f,.68f,.8f),Quaternion.identity,planks,true);foreach(int edge in new[]{-1,1})Piece(p+new Vector3(edge*.55f,.36f,0),new(.08f,.73f,.85f),Quaternion.identity,metals);
   Festoon(p+new Vector3(-1.2f,1.8f,1),p+new Vector3(1.2f,1.8f,1),false,1.8f);for(int i=0;i<3;i++){float t=(.5f+i*.7f)/2.4f;Laundry(p+new Vector3(-.7f+i*.7f,1.8f-Mathf.Sin(t*Mathf.PI)*.22f,1),Quaternion.Euler(0,i*8-8,0),row+i);}
  }
 }
 public static GameObject Build(Transform parent){
  if(!timber){timber=Make("Bridge cedar",new Color(.37f,.23f,.12f));var color=Resources.Load<Texture2D>("Surfaces/Wood_BaseColor");if(color){timber.SetTexture("_BaseMap",color);timber.SetColor("_BaseColor",new Color(.74f,.61f,.42f));timber.SetTextureScale("_BaseMap",new Vector2(2,5));}rope=Make("Bridge hemp",new Color(.57f,.48f,.31f));metal=Make("Lookout iron",new Color(.06f,.10f,.11f));}
  cloth=cloth?cloth:Make("Roof linen laundry",new Color(.68f,.61f,.46f));leaf=leaf?leaf:Make("Roof garden sage",new Color(.17f,.29f,.10f));if(!bulb){bulb=Make("Roof string light warm glass",new Color(.95f,.66f,.26f));bulb.SetColor("_EmissionColor",new Color(4f,2.5f,1.1f));bulb.EnableKeyword("_EMISSION");}
  if(!denim)denim=Make("Washed indigo laundry",new Color(.16f,.28f,.43f));if(!rustCloth)rustCloth=Make("Faded coral laundry",new Color(.53f,.22f,.15f));foreach(var fabric in new[]{cloth,denim,rustCloth})fabric.SetFloat("_Cull",0);
  if(!cylinder){var source=GameObject.CreatePrimitive(PrimitiveType.Cylinder);cylinder=source.GetComponent<MeshFilter>().sharedMesh;source.SetActive(false);if(Application.isPlaying)Object.Destroy(source);else Object.DestroyImmediate(source);}
  if(!cube){var source=GameObject.CreatePrimitive(PrimitiveType.Cube);cube=source.GetComponent<MeshFilter>().sharedMesh;source.SetActive(false);if(Application.isPlaying)Object.Destroy(source);else Object.DestroyImmediate(source);}
  Paths.Clear();planks.Clear();ropes.Clear();metals.Clear();fabrics.Clear();leaves.Clear();bulbs.Clear();blueLaundry.Clear();redLaundry.Clear();var result=new GameObject("Rooftop loops and lookout");result.transform.SetParent(parent,false);root=result.transform;
  int crossing=0;
  foreach(int side in new[]{-1,1})foreach(int row in new[]{0,3}){
   float z=-30+row*16,h=7.2f,nextH=8.1f,x=side*12f;int style=crossing++;
   // Supported ramps begin within both roofs. The main span clears the full destination parapet.
   var start=new Vector3(x,h+.25f,z+3.8f);var a=new Vector3(x,h+.95f,z+5.3f);
   var b=new Vector3(x,nextH+.95f,z+10.7f);var finish=new Vector3(x,nextH+.25f,z+12.6f);
   float width=style==0?.56f:style==1?.66f:style==2?.86f:1.25f;
   Surface(start,a,width+.18f,.10f,metals,"Rooftop entry service ramp");
   Surface(b,finish,width+.18f,.10f,metals,"Rooftop exit service ramp");
   if(style==0){
    // Twin load-bearing utility cables with visible narrow steel tread straps, festoon lamps above.
    Surface(a,b,width,.07f,metals,"Paired utility cable footing");
    foreach(int edge in new[]{-1,1})Tube(a+new Vector3(edge*.17f,-.12f,0),b+new Vector3(edge*.17f,-.12f,0),.13f,ropes);
    Festoon(a+new Vector3(-.42f,1.4f,0),b+new Vector3(-.42f,1.4f,0),true,2.17f);
   }else if(style==1){
    Surface(a,b,width,.12f,planks,"Clothesline maintenance balance rail");
    foreach(int edge in new[]{-1,1}){Tube(a+new Vector3(edge*.29f,-.12f,0),b+new Vector3(edge*.29f,-.12f,0),.045f,metals);Festoon(a+new Vector3(edge*.50f,1.5f,0),b+new Vector3(edge*.50f,1.5f,0),false,2.27f);}
    for(int i=1;i<=5;i++){float t=i/6f;var anchor=Vector3.Lerp(a,b,t)+new Vector3(.50f,1.5f-Mathf.Sin(t*Mathf.PI)*.22f,0);Laundry(anchor,Quaternion.Euler(0,90+(i%2==0?7:-7),0),i-1);}
   }else if(style==2){
    Surface(a,b,width,.22f,metals,"Flat topped ventilation service duct");
    foreach(int edge in new[]{-1,1})Tube(a+new Vector3(edge*.50f,-.08f,0),b+new Vector3(edge*.50f,-.08f,0),.12f,metals);
    for(int i=0;i<=7;i++){var p=Vector3.Lerp(a,b,i/7f);Piece(p+Vector3.down*.10f,new(1.20f,.27f,.055f),Quaternion.FromToRotation(Vector3.forward,b-a),metals);}
   }else{
    Surface(a,b,width,.12f,planks,"Cedar rooftop catwalk");
    var q=Quaternion.FromToRotation(Vector3.forward,b-a);for(int i=0;i<23;i++){var p=Vector3.Lerp(a,b,i/22f);Piece(p+Vector3.up*.006f,new(width+.06f,.025f,.16f),q,planks);}
    foreach(int edge in new[]{-1,1}){var off=Vector3.right*edge*(width*.5f+.12f);for(int i=0;i<=3;i++){var center=Vector3.Lerp(a,b,i/3f);var p=center+off;Beam(center-Vector3.up*.06f,p-Vector3.up*.06f,.075f,metals);Beam(p,p+Vector3.up*.70f,.05f,metals);}Beam(a+off+Vector3.up*.70f,b+off+Vector3.up*.70f,.035f,ropes);}
   }
   // Visible anchoring saddles sit on actual rooftop support, outside the walking centerline.
   foreach(var p in new[]{start,finish})foreach(int edge in new[]{-1,1})Piece(p+new Vector3(edge*(width*.5f+.15f),-.03f,0),new(.20f,.12f,.50f),Quaternion.identity,metals);
   Paths.Add(new[]{start+Vector3.up*.04f,a+Vector3.up*.04f,(a+b)*.5f+Vector3.up*.04f,b+Vector3.up*.04f,finish+Vector3.up*.04f});
  }
  // Distinct living spaces are authored by ClosingTimeWorld.RoofPersonalization.
  // Northern roof overlooks the avenue. Keep its reward away from the entry landing.
  Vector3 deck=new Vector3(-14.6f,8.48f,31.0f);
  Piece(deck,new Vector3(3.6f,.32f,3.3f),Quaternion.identity,planks,true);
  foreach(float x in new[]{-16.25f,-12.95f}){Beam(new Vector3(x,8.65f,29.5f),new Vector3(x,9.4f,29.5f),.055f,metals);}
  Beam(new Vector3(-16.25f,9.4f,29.5f),new Vector3(-12.95f,9.4f,29.5f),.045f,metals);
  Piece(new Vector3(-15.3f,9.08f,31.8f),new Vector3(1.6f,.12f,.50f),Quaternion.identity,planks);
  Piece(new Vector3(-15.3f,9.42f,32.02f),new Vector3(1.6f,.62f,.08f),Quaternion.identity,planks);
  foreach(float x in new[]{-15.9f,-14.7f})Beam(new Vector3(x,8.64f,31.8f),new Vector3(x,9.06f,31.8f),.065f,metals);
  Beam(new Vector3(-13.5f,8.65f,30.1f),new Vector3(-13.5f,9.65f,30.1f),.09f,metals);
  foreach(float x in new[]{-13.63f,-13.37f})Piece(new Vector3(x,9.67f,30.02f),new Vector3(.18f,.18f,.44f),Quaternion.Euler(-12,0,0),metals);
  ClosingTimeWorld.Spots.Add(new ClosingTimeWorld.ScavengeSpot("north_roof_lookout","trophy_03",new Vector3(-14.0f,8.87f,31.0f),true));
  var owned=new List<Mesh>();Bake(planks,timber,"Combined cedar bridges",owned);Bake(ropes,rope,"Combined bridge ropes",owned);Bake(metals,metal,"Combined rooftop service metal",owned);Bake(fabrics,cloth,"Combined rooftop laundry",owned);Bake(blueLaundry,denim,"Combined indigo shirts and socks",owned);Bake(redLaundry,rustCloth,"Combined coral laundry",owned);Bake(leaves,leaf,"Combined rooftop herbs",owned);Bake(bulbs,bulb,"Combined rooftop festoon lamps",owned);result.AddComponent<ClosingWorldCleanup>().ownedMeshes=owned.ToArray();foreach(var mesh in owned)mesh.UploadMeshData(Application.isPlaying);planks.Clear();ropes.Clear();metals.Clear();fabrics.Clear();leaves.Clear();bulbs.Clear();blueLaundry.Clear();redLaundry.Clear();return result;
 }
}
}
