using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
namespace Jimothy {
/// <summary>Original solid geometry. One mesh/material per reward, with baked vertex colours.</summary>
public static partial class ItemVisuals {
 static Material surface;
 static readonly Color crust=new(.55f,.24f,.065f), bread=new(.88f,.66f,.32f), cream=new(.94f,.86f,.65f), gold=new(.83f,.51f,.10f), dark=new(.055f,.075f,.08f), green=new(.18f,.38f,.12f), silver=new(.58f,.68f,.72f);
 public static string ShapeKey(ItemDefinition item){
  switch(item.id){case "trophy_04":return "compass";case "trophy_05":return "tiny-troll";case "trophy_06":return "crown";case "item_136":return "fishing-lure";case "item_048":return "taco";case "item_016":return "cardamom-knot";case "item_054":return "pizza";case "item_070":return "arcade-token";case "item_090":return "bottle-opener";case "item_030":return "croissant";case "item_188":return "cassette";case "item_220":return "thermos";case "trophy_00":return "salmon-sculpture";case "trophy_01":return "bell-clapper";case "trophy_02":return "rainier-diorama";case "trophy_03":return "golden-bagel";case "item_000":case "item_001":case "item_050":case "item_051":return "salmon";case "item_002":case "item_003":return "sesame-half";case "item_006":case "item_007":return "everything-bagel";case "item_010":case "item_011":return "rye-sandwich";}
  return CatalogKey(item);
 }
 public static GameObject Create(ItemDefinition item,Transform parent,string spot="reward"){
  var root=new GameObject(item.name+" · "+spot);root.transform.SetParent(parent,false);
  if(!surface){surface=new Material(Resources.Load<Shader>("Shaders/FindSurface"));surface.name="Find vertex colour surface";}
  var mesh=Resources.Load<Mesh>("FindMeshes/"+item.id);bool generated=!mesh;if(generated)mesh=BuildMesh(item);
  int seed=0;foreach(char c in spot)seed=(seed*31+c)&32767;root.transform.localRotation=Quaternion.Euler(0,seed%35-17,0);
  root.AddComponent<MeshFilter>().sharedMesh=mesh;var renderer=root.AddComponent<MeshRenderer>();renderer.sharedMaterial=surface;renderer.shadowCastingMode=ShadowCastingMode.Off;
  if(generated)root.AddComponent<ClosingWorldCleanup>().ownedMeshes=new[]{mesh};return root;
 }
 static Mesh LegacyMesh(ItemDefinition item,string forceKey=null){
  var b=new Builder();string key=forceKey??ShapeKey(item);bool trophy=item.category=="trophy";
  if(trophy)b.Box(new(0,.025f,0),new(.42f,.05f,.28f),dark);
  switch(key){
   case "compass":
    b.Lathe(new(0,.055f,0),new[]{new Vector2(.17f,0),new Vector2(.17f,.045f)},gold);b.Sphere(new(0,.10f,0),new(.145f,.007f,.145f),cream);b.Ring(new(0,.115f,0),.15f,.01f,gold,Quaternion.identity);b.Wedge(new(0,.12f,0),new(.055f,.009f,.24f),new(.7f,.06f,.035f));b.Ring(new(0,.075f,.205f),.04f,.012f,gold,Quaternion.identity);break;
   case "tiny-troll":
    b.Sphere(new(0,.18f,0),new(.105f,.13f,.075f),green);b.Sphere(new(0,.34f,0),new(.105f,.085f,.08f),green);b.Sphere(new(0,.32f,-.10f),new(.045f,.035f,.05f),bread);
    for(int side=-1;side<=1;side+=2){b.Sphere(new(side*.07f,.07f,-.035f),new(.045f,.035f,.065f),dark);b.Sphere(new(side*.105f,.19f,0),new(.04f,.085f,.04f),green);b.Sphere(new(side*.04f,.36f,-.07f),Vector3.one*.014f,cream);}b.Cone(new(0,.39f,0),.07f,.10f,crust);break;
   case "crown":
    b.Ring(new(0,.12f,0),.13f,.027f,gold,Quaternion.identity);for(int i=0;i<7;i++){float a=i*Mathf.PI*2/7;var p=new Vector3(Mathf.Sin(a)*.13f,.12f,Mathf.Cos(a)*.13f);b.Cone(p,.035f,.15f,gold);b.Sphere(p+Vector3.up*.15f,Vector3.one*.02f,i%2==0?new Color(.65f,.04f,.05f):silver);}break;
   case "fishing-lure":
    b.Sphere(new(0,.055f,0),new(.15f,.045f,.055f),silver);b.Sphere(new(.045f,.09f,0),new(.07f,.012f,.04f),new(.75f,.08f,.025f));b.Ring(new(-.18f,.04f,0),.05f,.008f,dark,Quaternion.identity,Mathf.PI*1.7f);b.Sphere(new(.11f,.075f,-.04f),Vector3.one*.012f,dark);break;
   case "taco":
    b.TacoShell(bread);for(int i=0;i<5;i++)b.Sphere(new((i-2)*.045f,.095f,0),new(.04f,.025f,.045f),i%2==0?green:new Color(.75f,.23f,.09f));break;
   case "pizza":
    b.Wedge(new(0,.025f,0),new(.35f,.025f,.36f),crust);b.Wedge(new(0,.054f,0),new(.30f,.013f,.31f),gold);for(int i=0;i<3;i++)b.Sphere(new(-.08f+i*.065f,.073f,-.09f),new(.033f,.005f,.033f),new(.62f,.10f,.06f));break;
   case "cardamom-knot":
    for(int i=0;i<3;i++)b.Ring(new((i-1)*.05f,.07f,0),.075f,.026f,i%2==0?bread:crust,Quaternion.Euler(i*20,0,i*35));break;
   case "croissant":
    for(int i=0;i<9;i++){float a=(i-4)*.30f;float size=.025f+.035f*(1-Mathf.Abs(i-4)/4f);b.Sphere(new(Mathf.Sin(a)*.15f,.055f,Mathf.Cos(a)*.13f-.08f),new(size,.045f,size*1.15f),i%2==0?bread:crust);}break;
   case "arcade-token":
    b.Lathe(Vector3.zero,new[]{new Vector2(.12f,0),new Vector2(.12f,.025f)},gold);b.Ring(new(0,.027f,0),.10f,.006f,crust,Quaternion.identity);for(int i=0;i<4;i++)b.Box(new(0,.032f,0),new(.035f,.01f,.14f),cream,Quaternion.Euler(0,i*45,0));break;
   case "bottle-opener":
    b.Ring(new(-.085f,.025f,0),.06f,.016f,silver,Quaternion.identity);b.Box(new(.05f,.025f,0),new(.22f,.03f,.047f),silver);b.Box(new(-.08f,.025f,.028f),new(.065f,.03f,.025f),silver);break;
   case "cassette":
    b.Box(new(0,.045f,0),new(.32f,.07f,.21f),dark);b.Box(new(0,.083f,0),new(.28f,.008f,.155f),new(.68f,.22f,.065f));for(int i=-1;i<=1;i+=2){b.Ring(new(i*.075f,.091f,0),.036f,.009f,cream,Quaternion.identity);b.Sphere(new(i*.075f,.09f,0),new(.02f,.005f,.02f),dark);}b.Box(new(0,.09f,.074f),new(.16f,.01f,.025f),silver);break;
   case "thermos":
    b.Lathe(Vector3.zero,new[]{new Vector2(.073f,0),new Vector2(.073f,.25f),new Vector2(.056f,.28f)},new(.09f,.28f,.30f));b.Lathe(new(0,.28f,0),new[]{new Vector2(.077f,0),new Vector2(.077f,.065f)},silver);b.Ring(new(.09f,.18f,0),.063f,.012f,dark,Quaternion.Euler(90,0,0));break;
   case "sesame-half": Bagel(b,false,true,bread);break;
   case "everything-bagel": Bagel(b,true,false,crust);break;
   case "golden-bagel": Bagel(b,true,false,gold,true);b.Box(new(0,.075f,0),new(.045f,.10f,.05f),gold);break;
   case "salmon": Fish(b,false);break;case "salmon-sculpture":Fish(b,true);break;
   case "bell-clapper":
    b.Lathe(new(0,.08f,0),new[]{new Vector2(.14f,0),new Vector2(.14f,.03f),new Vector2(.09f,.15f),new Vector2(.035f,.20f)},gold);
    b.Ring(new(0,.325f,0),.047f,.014f,gold,Quaternion.Euler(90,0,0));b.Sphere(new(0,.07f,0),new(.045f,.06f,.045f),dark);break;
   case "rainier-diorama":
    b.Box(new(0,.07f,0),new(.37f,.06f,.23f),new(.09f,.24f,.29f));
    b.Cone(new(-.045f,.10f,0),.16f,.29f,silver);b.Cone(new(-.045f,.29f,0),.062f,.105f,cream);
    b.Cone(new(.12f,.10f,.025f),.09f,.15f,new(.18f,.34f,.36f));
    b.Sphere(new(.14f,.34f,.08f),Vector3.one*.05f,gold);break;
   case "rye-sandwich":case "sandwich":
    for(int i=0;i<5;i++)b.Wedge(new(0,.025f+i*.035f,0),new(.35f,.035f,.31f),i==0||i==4?crust:i==1?cream:i==2?green:new Color(.72f,.25f,.18f));break;
   case "cup":
    b.Lathe(Vector3.zero,new[]{new Vector2(.075f,0),new Vector2(.10f,.24f)},cream);b.Ring(new(0,.245f,0),.09f,.013f,gold,Quaternion.identity);b.Sphere(new(0,.236f,0),new(.084f,.008f,.084f),crust);b.Ring(new(.11f,.13f,0),.055f,.016f,cream,Quaternion.Euler(90,0,0));break;
   case "fries":
    b.Box(new(0,.07f,0),new(.18f,.14f,.12f),new(.65f,.12f,.08f));for(int i=0;i<7;i++)b.Box(new((i%4-1.5f)*.035f,.18f+(i%3)*.015f,(i/4-.5f)*.045f),new(.025f,.18f,.025f),bread,Quaternion.Euler(0,0,(i-3)*4));break;
   case "cheese":b.Wedge(new(0,.06f,0),new(.28f,.12f,.24f),gold);for(int i=0;i<3;i++)b.Sphere(new(-.07f+i*.06f,.122f,-.035f),new(.017f,.004f,.017f),crust);break;
   case "fruit":b.Sphere(new(0,.13f,0),new(.13f,.135f,.12f),new(.68f,.16f,.06f));b.Box(new(0,.275f,0),new(.018f,.06f,.018f),crust);b.Sphere(new(.04f,.27f,0),new(.07f,.009f,.025f),green);break;
   case "berries":for(int i=0;i<9;i++)b.Sphere(new((i%3-1)*.065f,.04f+(i/3)*.035f,(i/3-1)*.055f),Vector3.one*.047f,new(.12f,.08f,.23f));break;
   case "braided-bread":for(int i=0;i<7;i++)b.Sphere(new((i-3)*.046f,.065f+(i%2)*.012f,Mathf.Sin(i*2)*.03f),new(.065f,.06f,.07f),i%2==0?bread:crust);break;
   case "key":b.Ring(new(-.09f,.03f,0),.065f,.018f,gold,Quaternion.identity);b.Box(new(.04f,.03f,0),new(.20f,.035f,.035f),gold);for(int i=0;i<2;i++)b.Box(new(.09f+i*.04f,.03f,.035f),new(.025f,.035f,.07f),gold);break;
   case "coin-stack":for(int i=0;i<3;i++){b.Lathe(new((i-1)*.05f,i*.022f,0),new[]{new Vector2(.085f,0),new Vector2(.085f,.02f)},i==1?silver:gold);b.Ring(new((i-1)*.05f,i*.022f+.021f,0),.062f,.003f,dark,Quaternion.identity);}break;
   case "star-medal":for(int i=0;i<5;i++)b.Wedge(new(0,.13f,0),new(.16f,.035f,.22f),gold,Quaternion.Euler(0,i*72,0));b.Ring(new(0,.21f,0),.045f,.012f,gold,Quaternion.Euler(90,0,0));break;
   default:
    b.Sphere(new(0,.13f,0),Vector3.one*.125f,key=="glass-float"?(item.id=="item_096"?new Color(.84f,.39f,.055f):new Color(.12f,.52f,.51f)):bread);
    if(key!="glass-float")for(int i=0;i<3;i++)b.Ring(new(0,.13f,0),.128f,.008f,cream,Quaternion.Euler(i*60,0,0));else b.Sphere(new(-.04f,.21f,-.065f),new(.025f,.013f,.015f),cream);break;
  }
  return b.Build();
 }
 static void Bagel(Builder b,bool seeds,bool half,Color color,bool upright=false){var center=new Vector3(0,upright?.22f:.065f,0);var q=upright?Quaternion.Euler(90,0,0):Quaternion.identity;b.Ring(center,.12f,.052f,color,q,half?Mathf.PI:Mathf.PI*2);for(int i=0;i<(half?12:24);i++){float a=half?(i+.5f)*Mathf.PI/12:i*Mathf.PI*2/24;b.Sphere(center+q*new Vector3(Mathf.Sin(a)*.12f,.044f,Mathf.Cos(a)*.12f),new(.007f,.004f,.013f),seeds&&i%3==0?dark:cream);}}
 static void Fish(Builder b,bool trophy){float y=trophy?.22f:.08f;var c=trophy?gold:new Color(.83f,.34f,.20f);b.Sphere(new(0,y,0),new(.19f,.075f,.075f),c);b.Wedge(new(-.22f,y,0),new(.12f,.035f,.18f),trophy?gold:silver,Quaternion.Euler(0,90,0));b.Sphere(new(.13f,y+.02f,-.063f),Vector3.one*.011f,dark);for(int i=0;i<4;i++)b.Box(new(-.08f+i*.055f,y+.069f,0),new(.007f,.008f,.085f),cream,Quaternion.Euler(0,-20,0));if(trophy)b.Box(new(0,.11f,0),new(.04f,.16f,.04f),gold);}
 sealed class Builder {
  readonly List<Vector3> v=new();readonly List<Color> colors=new();readonly List<int> t=new();
  void Tri(Vector3 a,Vector3 b,Vector3 c,Color color){int n=v.Count;v.Add(a);v.Add(b);v.Add(c);colors.Add(color);colors.Add(color);colors.Add(color);t.Add(n);t.Add(n+1);t.Add(n+2);}
  public void Box(Vector3 p,Vector3 s,Color c,Quaternion q=default){if(q==default)q=Quaternion.identity;var a=new Vector3[8];for(int i=0;i<8;i++)a[i]=p+q*Vector3.Scale(new Vector3((i&1)==0?-.5f:.5f,(i&2)==0?-.5f:.5f,(i&4)==0?-.5f:.5f),s);int[] ix={0,2,1,1,2,3,4,5,6,5,7,6,0,1,4,1,5,4,2,6,3,3,6,7,0,4,2,2,4,6,1,3,5,3,7,5};for(int i=0;i<ix.Length;i+=3)Tri(a[ix[i]],a[ix[i+1]],a[ix[i+2]],c);}
  public void Wedge(Vector3 p,Vector3 s,Color c,Quaternion q=default){if(q==default)q=Quaternion.identity;Vector3[] a={new(-.5f,0,-.5f),new(.5f,0,-.5f),new(-.5f,0,.5f),new(-.5f,1,-.5f),new(.5f,1,-.5f),new(-.5f,1,.5f)};for(int i=0;i<a.Length;i++)a[i]=p+q*Vector3.Scale(a[i],s);int[] ix={0,1,2,3,5,4,0,3,1,1,3,4,1,4,2,2,4,5,2,5,0,0,5,3};for(int i=0;i<ix.Length;i+=3)Tri(a[ix[i]],a[ix[i+1]],a[ix[i+2]],c);}
  public void TacoShell(Color color){Vector3 P(int side,int i,float r){float a=i*Mathf.PI/16;return new Vector3(side*.15f,.13f-Mathf.Sin(a)*r,Mathf.Cos(a)*r);}for(int i=0;i<16;i++){Tri(P(-1,i,.11f),P(1,i,.11f),P(-1,i+1,.11f),color);Tri(P(1,i,.11f),P(1,i+1,.11f),P(-1,i+1,.11f),color);}}
  public void Sphere(Vector3 p,Vector3 s,Color c){const int n=10,m=6;Vector3 P(int i,int j){float a=i*Mathf.PI*2/n,b=j*Mathf.PI/m;return p+Vector3.Scale(new(Mathf.Sin(b)*Mathf.Cos(a),Mathf.Cos(b),Mathf.Sin(b)*Mathf.Sin(a)),s);}for(int i=0;i<n;i++)for(int j=0;j<m;j++){Tri(P(i,j),P(i+1,j),P(i,j+1),c);Tri(P(i+1,j),P(i+1,j+1),P(i,j+1),c);}}
  public void Ring(Vector3 p,float r,float tube,Color c,Quaternion q,float arc=Mathf.PI*2){const int n=24,m=6;Vector3 P(int i,int j){float a=i*arc/n,b=j*Mathf.PI*2/m;return p+q*new Vector3(Mathf.Cos(a)*(r+Mathf.Cos(b)*tube),Mathf.Sin(b)*tube,Mathf.Sin(a)*(r+Mathf.Cos(b)*tube));}for(int i=0;i<n;i++)for(int j=0;j<m;j++){Tri(P(i,j),P(i,j+1),P(i+1,j),c);Tri(P(i+1,j),P(i,j+1),P(i+1,j+1),c);}}
  public void Cone(Vector3 p,float r,float h,Color c){Lathe(p,new[]{new Vector2(r,0),new Vector2(0,h)},c);}
  public void Lathe(Vector3 p,Vector2[] profile,Color c){const int n=16;Vector3 P(int i,int j){float a=i*Mathf.PI*2/n;return p+new Vector3(Mathf.Cos(a)*profile[j].x,profile[j].y,Mathf.Sin(a)*profile[j].x);}for(int j=0;j<profile.Length-1;j++)for(int i=0;i<n;i++){Tri(P(i,j),P(i,j+1),P(i+1,j),c);Tri(P(i+1,j),P(i,j+1),P(i+1,j+1),c);}}
  public void Star(Vector3 center,float outer,float inner,float height,int points,Color color){for(int i=0;i<points*2;i++){float a=i*Mathf.PI/points,z=(i+1)*Mathf.PI/points;var p=center+new Vector3(Mathf.Sin(a)*(i%2==0?outer:inner),0,Mathf.Cos(a)*(i%2==0?outer:inner));var q=center+new Vector3(Mathf.Sin(z)*(i%2==0?inner:outer),0,Mathf.Cos(z)*(i%2==0?inner:outer));var up=Vector3.up*height;Tri(center+up,p+up,q+up,color);Tri(center,q,p,color);Tri(p,q+up,p+up,color);Tri(p,q,q+up,color);}}
  public void Append(Mesh mesh){int offset=v.Count;v.AddRange(mesh.vertices);var cs=mesh.colors;if(cs.Length==mesh.vertexCount)colors.AddRange(cs);else for(int i=0;i<mesh.vertexCount;i++)colors.Add(cream);foreach(int index in mesh.triangles)t.Add(offset+index);if(Application.isPlaying)Object.Destroy(mesh);else Object.DestroyImmediate(mesh);}
  public void Tube(Vector3 a,Vector3 b,float radius,Color color){var q=Quaternion.FromToRotation(Vector3.up,(b-a).normalized);const int n=10;for(int i=0;i<n;i++){float t=i*Mathf.PI*2/n,u=(i+1)*Mathf.PI*2/n;var r=q*new Vector3(Mathf.Cos(t)*radius,0,Mathf.Sin(t)*radius);var s=q*new Vector3(Mathf.Cos(u)*radius,0,Mathf.Sin(u)*radius);Tri(a+r,b+r,b+s,color);Tri(a+r,b+s,a+s,color);Tri(a,a+r,a+s,color);Tri(b,b+s,b+r,color);}}
  public void Condition(bool food,int variant){if(food){for(int i=0;i<4;i++)Sphere(new Vector3(.11f+i*.025f,.011f,-.07f+i*.033f),Vector3.one*(.007f+i*.0015f),crust);}else {Box(new Vector3(-.08f,.018f,-.13f),new Vector3(.075f,.02f,.045f),silver,Quaternion.Euler(0,22+variant%30,0));}}
  public Mesh Build(){var mesh=new Mesh{name="Authored reward mesh",indexFormat=IndexFormat.UInt32};mesh.SetVertices(v);mesh.SetColors(colors);mesh.SetTriangles(t,0);mesh.RecalculateNormals();mesh.RecalculateBounds();return mesh;}
 }
}
}
