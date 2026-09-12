using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
namespace Jimothy {
public static class DenNeighbors {
 public static readonly Vector3 CollectorPosition=new(-10.35f,-2.4f,58.4f);
 public static void Build(Transform parent,SaveData data){
  var root=new GameObject("Wardrobe rail and Crimp's counter");root.transform.SetParent(parent,false);var art=root.AddComponent<DenNeighborArt>();
  Color wood=new(.23f,.12f,.065f),metal=new(.20f,.23f,.22f),cream=new(.77f,.70f,.5f),red=new(.43f,.07f,.035f),gold=new(.76f,.45f,.12f);
  // Wall-mounted rail between the west collection shelf and music corner.
  for(int side=-1;side<=1;side+=2){float z=60+side*.86f;art.Rod(new(-19.65f,.10f,z),new(-19.05f,.10f,z),.018f,metal);art.Box(new(-19.62f,.07f,z),new(.06f,.20f,.10f),wood);}
  art.Rod(new(-19.05f,.10f,59.12f),new(-19.05f,.10f,60.88f),.025f,metal);
  for(int i=1;i<WardrobeRules.Looks.Length;i++){
   float z=59.25f+(i-1)*.36f;art.Rod(new(-19.05f,.10f,z),new(-18.94f,-.08f,z),.012f,gold);
   if(!WardrobeRules.Unlocked(data,WardrobeRules.Looks[i].id))continue;
   var hook=new GameObject("Wardrobe display · "+WardrobeRules.Looks[i].id);hook.transform.SetParent(root.transform,false);hook.transform.position=new(-18.94f,-.11f,z);hook.transform.rotation=Quaternion.Euler(0,-90,0);hook.AddComponent<RaccoonWardrobe>().ShowOnHook(WardrobeRules.Looks[i].id,.62f);
  }
  art.Letter("WARDROBE",new(-19.45f,.52f,60),Quaternion.Euler(0,-90,0),.035f,cream);
  // The counter occupies a side alcove; the central ramp-to-sofa route stays open.
  Vector3 p=CollectorPosition;
  art.Box(p+new Vector3(0,.27f,0),new(.94f,.54f,.58f),wood,true);
  art.Box(p+new Vector3(0,.57f,0),new(1.10f,.075f,.72f),wood);
  for(int i=0;i<5;i++)art.Box(p+new Vector3((i-2)*.19f,.27f,-.304f),new(.025f,.48f,.02f),metal);
  art.Letter("CRIMP",p+new Vector3(0,.34f,-.319f),Quaternion.identity,.036f,cream);
  var model=Object.Instantiate(Resources.Load<GameObject>("Jimothy"),root.transform);model.name="Crimp · junk appraiser";model.transform.localScale=new Vector3(1.22f,.78f,1.05f)*.22f;model.transform.position=p+new Vector3(.12f,.62f,.02f);model.transform.rotation=Quaternion.Euler(0,180,0);
  model.AddComponent<JimothyCoat>().Initialize();model.AddComponent<RaccoonWardrobe>().Apply("boat",2);var animator=model.GetComponentInChildren<Animator>();if(animator){animator.runtimeAnimatorController=Resources.Load<RuntimeAnimatorController>("JimothyMotion");animator.applyRootMotion=false;}
  model.AddComponent<DenRaccoonHost>();
  art.Part(PrimitiveType.Cylinder,p+new Vector3(.40f,.64f,.18f),new(.16f,.025f,.16f),metal);art.Rod(p+new Vector3(.40f,.64f,.18f),p+new Vector3(.40f,1.08f,.18f),.012f,metal);art.Rod(p+new Vector3(.40f,1.08f,.18f),p+new Vector3(.24f,1.08f,.02f),.012f,metal);art.Part(PrimitiveType.Sphere,p+new Vector3(.24f,1.07f,.02f),new(.16f,.09f,.16f),gold);
  var lamp=new GameObject("Crimp desk lamp");lamp.transform.SetParent(root.transform,false);lamp.transform.position=p+new Vector3(.24f,1.00f,.02f);var light=lamp.AddComponent<Light>();light.type=LightType.Point;light.range=2.2f;light.intensity=1;light.color=new Color(1,.78f,.47f);light.shadows=LightShadows.None;lamp.AddComponent<ClosingLight>();
  art.Part(PrimitiveType.Cylinder,p+new Vector3(-.35f,.72f,-.15f),new(.15f,.12f,.15f),metal);
  for(int i=0;i<9;i++)art.Part(PrimitiveType.Cylinder,p+new Vector3(-.35f+(i%3-1)*.04f,.85f+i/3*.009f,-.15f+(i/3-1)*.03f),new(.044f,.008f,.044f),i%2==0?gold:red);
  // A earned keepsake uses its own small wall shelf, away from the instrument corner.
  if(data.collectorTrades>=3){Vector3 q=new(-8.65f,-.50f,60.25f);art.Box(q,new(.50f,.065f,.65f),wood);art.Part(PrimitiveType.Sphere,q+new Vector3(-.04f,.20f,0),new(.17f,.20f,.43f),gold);art.Part(PrimitiveType.Cube,q+new Vector3(-.04f,.20f,.27f),new(.035f,.19f,.19f),gold,Quaternion.Euler(45,0,0));art.Part(PrimitiveType.Sphere,q+new Vector3(-.132f,.235f,-.12f),new(.018f,.025f,.025f),metal);art.Box(q+new Vector3(-.08f,.057f,0),new(.30f,.055f,.46f),metal);}
  art.Finish();
 }
}
public sealed class DenNeighborArt:MonoBehaviour {
 readonly Dictionary<Color,Material> materials=new();readonly Dictionary<Material,List<CombineInstance>> groups=new();readonly List<Mesh> owned=new();
 Material Mat(Color c){if(!materials.TryGetValue(c,out var m)){m=new Material(Shader.Find("Universal Render Pipeline/Lit"));m.color=c;m.SetFloat("_Smoothness",.25f);materials.Add(c,m);}return m;}
 public void Box(Vector3 p,Vector3 size,Color c,bool solid=false){Part(PrimitiveType.Cube,p,size,c);if(solid){var g=new GameObject("Crimp counter collider");g.transform.SetParent(transform,false);g.transform.position=p;g.AddComponent<BoxCollider>().size=size;}}
 public void Rod(Vector3 a,Vector3 b,float r,Color c)=>Part(PrimitiveType.Cylinder,(a+b)*.5f,new(r*2,(b-a).magnitude*.5f,r*2),c,Quaternion.FromToRotation(Vector3.up,b-a));
 public void Part(PrimitiveType shape,Vector3 p,Vector3 size,Color c,Quaternion rotation=default){if(rotation==default)rotation=Quaternion.identity;var g=GameObject.CreatePrimitive(shape);g.SetActive(false);var mesh=g.GetComponent<MeshFilter>().sharedMesh;var m=Mat(c);if(!groups.TryGetValue(m,out var pieces)){pieces=new();groups.Add(m,pieces);}pieces.Add(new CombineInstance{mesh=mesh,transform=Matrix4x4.TRS(p,rotation,size)});Destroy(g);}
 public void Letter(string text,Vector3 p,Quaternion rotation,float size,Color c){var f=GameTypography.Display;f.RequestCharactersInTexture(text,72);var g=new GameObject(text+" sign");g.transform.SetParent(transform,false);g.transform.position=p;g.transform.rotation=rotation;var t=g.AddComponent<TextMesh>();t.font=f;t.text=text;t.fontSize=72;t.characterSize=size;t.anchor=TextAnchor.MiddleCenter;t.color=c;var m=new Material(Shader.Find("Jimothy/WorldLettering"));m.mainTexture=f.material.mainTexture;g.GetComponent<Renderer>().sharedMaterial=m;var binding=g.AddComponent<DenWordmarkBinding>();binding.font=f;binding.material=m;}
 public void Finish(){foreach(var group in groups){var mesh=new Mesh{name="Den social props"};mesh.CombineMeshes(group.Value.ToArray());owned.Add(mesh);var g=new GameObject("Den social details");g.transform.SetParent(transform,false);g.AddComponent<MeshFilter>().sharedMesh=mesh;g.AddComponent<MeshRenderer>().sharedMaterial=group.Key;}}
 void OnDestroy(){foreach(var m in materials.Values)if(m)Destroy(m);foreach(var m in owned)if(m)Destroy(m);}
}
public sealed class DenRaccoonHost:MonoBehaviour {
 Renderer[] renderers;bool visible=true;
 void Awake(){renderers=GetComponentsInChildren<Renderer>();foreach(var r in renderers)r.shadowCastingMode=ShadowCastingMode.Off;}
 void Update(){var game=GameSession.Instance;bool show=game&&game.Playing&&game.AtHome;if(show==visible)return;visible=show;foreach(var r in renderers)if(r)r.enabled=show;}
}
}
