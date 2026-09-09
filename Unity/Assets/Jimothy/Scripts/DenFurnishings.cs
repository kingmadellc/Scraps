using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
namespace Jimothy {
/// <summary>Persistent cosmetic choices. No hidden movement/stat upgrades or inventory conversion.</summary>
public static class DenFurnishings {
 public sealed class Upgrade{public string id,name;public int cost,finds;public Upgrade(string id,string name,int cost,int finds=0){this.id=id;this.name=name;this.cost=cost;this.finds=finds;}}
 public static readonly Dictionary<string,Upgrade> Catalog=new(){
  {"cushion",new("cushion","Patchwork cushion",20)}, {"glass-floats",new("glass-floats","Glass float collection",35)},
  {"vinyl-wall",new("vinyl-wall","Vinyl gallery",35,2)}, {"plant-corner",new("plant-corner","Fern corner",30,2)},
  {"string-lights",new("string-lights","Backstage lights",45,3)}, {"record-crate",new("record-crate","Crate of records",25,1)},
  {"rainier-print",new("rainier-print","Rainier travel print",55,5)}, {"mushroom-lamp",new("mushroom-lamp","Amber mushroom lamp",40,4)},
  {"reading-nook",new("reading-nook","Reading nook",60,6)}, {"pedalboard",new("pedalboard","Pedal collection",45,5)},
  {"harbor-shelf",new("harbor-shelf","Harbor wall display",65,8)}, {"gold-paw",new("gold-paw","Golden paw wall plaque",90,12)}
 };
 static readonly Color wood=new(.24f,.13f,.065f),iron=new(.035f,.04f,.035f),cream=new(.77f,.69f,.5f),green=new(.12f,.26f,.17f),gold=new(.67f,.43f,.13f),red=new(.40f,.12f,.075f);
 public static void Build(Transform parent,List<string> decor){
  var b=new FurnishingMesh(parent);
  if(decor.Contains("vinyl-wall")){for(int i=0;i<3;i++){var p=new Vector3(-11.4f+i*.96f,-.04f+(i%2)*.28f,63.08f);b.Part(PrimitiveType.Cylinder,p,new(.62f,.018f,.62f),iron,Quaternion.Euler(90,0,0));b.Part(PrimitiveType.Cylinder,p+Vector3.back*.024f,new(.21f,.009f,.21f),i%2==0?red:cream,Quaternion.Euler(90,0,0));b.Part(PrimitiveType.Cylinder,p+Vector3.back*.036f,new(.022f,.005f,.022f),iron,Quaternion.Euler(90,0,0));}}
  if(decor.Contains("plant-corner")){for(int i=0;i<3;i++){var p=new Vector3(-8.8f-i*.32f,-2.4f,62.60f);b.Part(PrimitiveType.Cylinder,p+Vector3.up*(.18f+i*.035f),new(.28f,.18f+i*.035f,.28f),red);b.Part(PrimitiveType.Cylinder,p+Vector3.up*(.37f+i*.07f),new(.265f,.012f,.265f),iron);for(int leaf=0;leaf<14;leaf++){float a=leaf*2.4f;var tip=p+new Vector3(Mathf.Cos(a)*.20f,.45f+(leaf%4)*.04f+i*.07f,Mathf.Sin(a)*.20f);b.Part(PrimitiveType.Sphere,tip,new(.055f,.025f,.35f),leaf%3==0?green*1.6f:green,Quaternion.Euler(-18,leaf*137.5f,0));}}}
  if(decor.Contains("string-lights")){Vector3 prev=new(-19.4f,.84f,63.0f);for(int i=1;i<=28;i++){float x=-19.4f+i*.39f;var p=new Vector3(x,.84f-Mathf.Sin(i/28f*Mathf.PI)*.23f,63.0f);b.Rod(prev,p,.009f,iron);if(i%2==0)b.Part(PrimitiveType.Sphere,p-Vector3.up*.05f,new(.05f,.073f,.05f),new(1,.72f,.28f),emission:3);prev=p;}b.Light(new(-15,.46f,62.80f),new(1,.73f,.40f),5,2);}
  if(decor.Contains("record-crate")){var p=new Vector3(-12.05f,-2.4f,62.35f);b.Box(p+Vector3.up*.06f,new(.69f,.12f,.45f),wood);foreach(int s in new[]{-1,1})b.Box(p+new Vector3(s*.31f,.21f,0),new(.06f,.34f,.45f),wood);for(int k=0;k<7;k++){var q=p+new Vector3((k-3)*.073f,.28f,0);b.Box(q,new(.028f,.44f,.39f),k%3==0?red:k%3==1?green:cream,Quaternion.Euler(0,0,-8+k*2));}}
  if(decor.Contains("rainier-print")){var p=new Vector3(-8.44f,-.16f,59.4f);b.Box(p,new(.075f,1.28f,1.65f),wood);b.Box(p+Vector3.left*.043f,new(.012f,1.12f,1.48f),new(.13f,.29f,.34f));b.Part(PrimitiveType.Sphere,p+new Vector3(-.058f,.29f,.46f),new(.025f,.22f,.22f),gold);for(int j=0;j<12;j++){float z=(j-5.5f)*.115f,h=.64f*Mathf.Exp(-z*z/ .16f);b.Box(p+new Vector3(-.057f,-.36f+h*.5f,z),new(.015f,h,.117f),j<3||j>8?green:cream);}}
  if(decor.Contains("mushroom-lamp")){var p=new Vector3(-16.9f,-2.4f,62.1f);b.Part(PrimitiveType.Cylinder,p+Vector3.up*.33f,new(.10f,.33f,.10f),cream);b.Part(PrimitiveType.Sphere,p+Vector3.up*.65f,new(.52f,.22f,.52f),new(.76f,.34f,.10f),emission:.3f);b.Light(p+Vector3.up*.60f,new(1,.60f,.27f),2.3f,1.5f);}
  if(decor.Contains("reading-nook")){var p=new Vector3(-10.05f,-2.4f,62.25f);b.Part(PrimitiveType.Sphere,p+Vector3.up*.23f,new(1,.46f,.82f),green);for(int i=0;i<5;i++)b.Box(new(-9.5f,-2.36f+i*.065f,61.5f),new(.42f,.06f,.31f),i%2==0?cream:red,Quaternion.Euler(0,i*8,0));}
  if(decor.Contains("pedalboard")){var p=new Vector3(-17.2f,-2.37f,62.0f);b.Box(p,new(.76f,.045f,.46f),iron);for(int i=0;i<4;i++){var c=p+new Vector3((i-1.5f)*.17f,.06f,0);b.Box(c,new(.14f,.075f,.28f),i%2==0?green:red);b.Part(PrimitiveType.Cylinder,c+new Vector3(0,.055f,-.07f),new(.025f,.02f,.025f),cream);for(int k=0;k<2;k++)b.Part(PrimitiveType.Cylinder,c+new Vector3((k-.5f)*.07f,.05f,.07f),new(.025f,.015f,.025f),iron);}}
  if(decor.Contains("harbor-shelf")){var p=new Vector3(-19.48f,-.18f,60.4f);b.Box(p,new(.36f,.075f,1.5f),wood);for(int i=0;i<3;i++){var q=p+new Vector3(.12f,.19f,(i-1)*.43f);b.Part(PrimitiveType.Sphere,q,new(.23f,.25f,.23f),i%2==0?green:gold);b.Rod(q+Vector3.up*.13f,q+Vector3.up*.24f,.018f,cream);}}
  if(decor.Contains("gold-paw")){var p=new Vector3(-14,.92f,63.12f);b.Part(PrimitiveType.Cylinder,p,new(.52f,.023f,.52f),wood,Quaternion.Euler(90,0,0));b.Part(PrimitiveType.Sphere,p+new Vector3(0,-.05f,-.042f),new(.21f,.16f,.04f),gold);for(int i=0;i<4;i++)b.Part(PrimitiveType.Sphere,p+new Vector3((i-1.5f)*.087f,.075f+(.04f-Mathf.Abs(i-1.5f)*.02f),-.042f),new(.064f,.085f,.04f),gold);}
  b.Finish();
 }
 sealed class FurnishingMesh {
  readonly Transform parent;readonly Dictionary<string,Material> materials=new();readonly Dictionary<Material,List<CombineInstance>> parts=new();
  public FurnishingMesh(Transform p){parent=p;}
  public void Box(Vector3 p,Vector3 size,Color color,Quaternion rotation=default)=>Part(PrimitiveType.Cube,p,size,color,rotation);
  public void Rod(Vector3 a,Vector3 b,float radius,Color c)=>Part(PrimitiveType.Cylinder,(a+b)*.5f,new(radius*2,(b-a).magnitude*.5f,radius*2),c,Quaternion.FromToRotation(Vector3.up,b-a));
  public void Part(PrimitiveType type,Vector3 position,Vector3 scale,Color c,Quaternion rotation=default,float emission=0){if(rotation==default)rotation=Quaternion.identity;string key=c.ToString()+emission;if(!materials.TryGetValue(key,out var mat)){mat=new Material(Shader.Find("Universal Render Pipeline/Lit")){name="Den custom furnishing"};mat.SetColor("_BaseColor",c);mat.SetFloat("_Smoothness",.23f);if(emission>0){mat.EnableKeyword("_EMISSION");mat.SetColor("_EmissionColor",c*emission);}materials.Add(key,mat);parent.gameObject.AddComponent<DenOwnedMaterial>().material=mat;}
   var temp=GameObject.CreatePrimitive(type);temp.SetActive(false);var mesh=temp.GetComponent<MeshFilter>().sharedMesh;if(!parts.TryGetValue(mat,out var list)){list=new();parts.Add(mat,list);}list.Add(new CombineInstance{mesh=mesh,transform=Matrix4x4.TRS(position,rotation,scale)});Object.Destroy(temp);
  }
  public void Light(Vector3 position,Color c,float range,float power){var go=new GameObject("Decorative Den lamp");go.transform.SetParent(parent,false);go.transform.position=position;var light=go.AddComponent<Light>();light.type=LightType.Point;light.color=c;light.range=range;light.intensity=power;light.shadows=LightShadows.None;go.AddComponent<ClosingLight>();}
  public void Finish(){foreach(var pair in parts){var mesh=new Mesh{name="Combined personal Den furnishings",indexFormat=IndexFormat.UInt32};mesh.CombineMeshes(pair.Value.ToArray());var go=new GameObject("Personal Den furnishings");go.transform.SetParent(parent,false);go.AddComponent<MeshFilter>().sharedMesh=mesh;go.AddComponent<MeshRenderer>().sharedMaterial=pair.Key;go.AddComponent<ClosingWorldCleanup>().ownedMeshes=new[]{mesh};}}
 }
}}
