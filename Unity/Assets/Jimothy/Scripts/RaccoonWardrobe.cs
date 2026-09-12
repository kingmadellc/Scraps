using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Jimothy {
// Small solid meshes follow the existing head/body bones. Cosmetics add no collision or stats.
public sealed class RaccoonWardrobe:MonoBehaviour {
 GameObject worn;readonly List<Material> mats=new();readonly List<Mesh> meshes=new();
 void Clear(){if(worn){worn.SetActive(false);Destroy(worn);}foreach(var m in mats)if(m)Destroy(m);mats.Clear();foreach(var m in meshes)if(m)Destroy(m);meshes.Clear();}
 Material Mat(Color color,float metal=0){var m=new Material(Shader.Find("Universal Render Pipeline/Lit"));m.color=color;m.SetFloat("_Smoothness",.25f);m.SetFloat("_Cull",0);m.SetFloat("_Metallic",metal);mats.Add(m);return m;}
 Transform Part(PrimitiveType shape,string name,Vector3 p,Vector3 scale,Material m){var g=GameObject.CreatePrimitive(shape);g.name=name;g.layer=gameObject.layer;g.transform.SetParent(worn.transform,false);g.transform.localPosition=p;g.transform.localScale=scale;var c=g.GetComponent<Collider>();c.enabled=false;Destroy(c);g.GetComponent<Renderer>().sharedMaterial=m;return g.transform;}
 void Face(string name,Vector3[] points,int[] triangles,Material m){var mesh=new Mesh{name=name};mesh.vertices=points;mesh.triangles=triangles;mesh.RecalculateNormals();mesh.RecalculateBounds();meshes.Add(mesh);var g=new GameObject(name);g.layer=gameObject.layer;g.transform.SetParent(worn.transform,false);g.AddComponent<MeshFilter>().sharedMesh=mesh;g.AddComponent<MeshRenderer>().sharedMaterial=m;}
 public void Apply(string id,int coat){Clear();GetComponent<JimothyCoat>()?.SetTint(WardrobeRules.Tint(coat));if(id=="bare")return;
  var skins=GetComponentsInChildren<SkinnedMeshRenderer>();if(skins.Length==0)return;var b=skins[0].bounds;foreach(var r in skins)b.Encapsulate(r.bounds);float h=b.size.y;
  bool back=id=="roadie"||id=="inspector";var bone=GetComponentsInChildren<Transform>().FirstOrDefault(t=>t.name==(back?"body":"head"));if(!bone)bone=transform;
  worn=new GameObject("Scavenged look · "+id);worn.layer=gameObject.layer;worn.transform.SetParent(bone,false);worn.transform.position=bone.position+transform.up*h*(back?.29f:id=="bandana"?.15f:.25f)+(back?-transform.forward*h*.14f:Vector3.zero);worn.transform.rotation=transform.rotation;var s=bone.lossyScale;worn.transform.localScale=new Vector3(h/Mathf.Abs(s.x),h/Mathf.Abs(s.y),h/Mathf.Abs(s.z));worn.transform.localScale*=id=="bandana"?.70f:id=="crown"?.67f:.80f;
  var red=Mat(new Color(.48f,.065f,.045f));var cream=Mat(new Color(.77f,.68f,.45f));var dark=Mat(new Color(.025f,.035f,.035f));var brass=Mat(new Color(.72f,.44f,.12f),.6f);
  if(id=="bandana"){
   Face("Folded bandana",new[]{new Vector3(-.24f,-.12f,.14f),new Vector3(.24f,-.12f,.14f),new Vector3(0,-.38f,.26f),new Vector3(0,-.11f,.22f)},new[]{0,3,2,3,1,2},red);
   Part(PrimitiveType.Sphere,"Side knot",new(.23f,-.12f,.13f),new(.09f,.075f,.075f),red);for(int i=0;i<3;i++)Part(PrimitiveType.Cube,"Hand stitch",new(-.06f+i*.06f,-.18f,.197f),new(.025f,.008f,.008f),cream);
  }else if(id=="crown"){
   for(int i=0;i<10;i++){float a=i*Mathf.PI*.2f;var at=new Vector3(Mathf.Cos(a)*.20f,0,Mathf.Sin(a)*.17f);var cap=Part(PrimitiveType.Cylinder,"Crimped bottle cap",at,new(.12f,.018f,.12f),i%2==0?brass:red);cap.localRotation=Quaternion.Euler(80,0,-i*36);for(int k=0;k<8;k++){float q=k*Mathf.PI/4;Part(PrimitiveType.Cube,"Cap tooth",at+new Vector3(Mathf.Cos(q)*.055f,.015f,Mathf.Sin(q)*.055f),new(.016f,.025f,.016f),brass);}if(i%2==0)Part(PrimitiveType.Cube,"Royal bent tab",at+Vector3.up*.08f,new(.035f,.10f,.018f),brass);}
  }else if(id=="roadie"){
   Part(PrimitiveType.Cube,"Cassette satchel",new(0,.015f,-.08f),new(.42f,.28f,.13f),dark);Part(PrimitiveType.Cube,"Duct tape label",new(0,.035f,-.15f),new(.35f,.17f,.012f),cream);
   for(int i=-1;i<=1;i+=2){var spool=Part(PrimitiveType.Cylinder,"Tape spool",new(i*.095f,.035f,-.16f),new(.09f,.008f,.09f),dark);spool.localRotation=Quaternion.Euler(90,0,0);Part(PrimitiveType.Cube,"Harness strap",new(i*.15f,-.09f,.04f),new(.03f,.38f,.045f),red);}
   Part(PrimitiveType.Cube,"Tape window",new(0,.035f,-.17f),new(.07f,.05f,.009f),dark);Part(PrimitiveType.Cube,"Carry handle",new(0,.19f,-.08f),new(.14f,.025f,.05f),red);
  }else if(id=="boat"){
   Face("Folded paper hull",new[]{new Vector3(-.30f,0,0),new Vector3(.30f,0,0),new Vector3(0,-.06f,.19f),new Vector3(0,-.06f,-.19f),new Vector3(0,.19f,0)},new[]{0,2,1,0,1,3,0,4,2,2,4,1,1,4,3,3,4,0},cream);
   for(int i=0;i<3;i++)Part(PrimitiveType.Cube,"Newspaper print",new(-.07f+i*.065f,.026f,.105f),new(.038f,.006f,.038f),dark);
  }else if(id=="inspector"){
   var hi=Mat(new Color(.88f,.50f,.025f));for(int side=-1;side<=1;side+=2){var strap=Part(PrimitiveType.Cube,"Safety vest shoulder",new(side*.18f,.04f,-.02f),new(.065f,.23f,.20f),hi);strap.localRotation=Quaternion.Euler(-22,0,side*12);}Part(PrimitiveType.Cube,"Hi-vis back panel",new(0,-.03f,-.13f),new(.46f,.27f,.065f),hi);for(int i=-1;i<=1;i+=2)Part(PrimitiveType.Cube,"Reflective strap",new(i*.14f,-.025f,-.17f),new(.04f,.25f,.014f),cream);Part(PrimitiveType.Cube,"Stolen inspector badge",new(0,-.06f,-.177f),new(.11f,.09f,.012f),brass);
  }
  Batch();
 }
 void Batch(){var filters=worn.GetComponentsInChildren<MeshFilter>();foreach(var group in filters.GroupBy(f=>f.GetComponent<Renderer>().sharedMaterial)){var merged=new Mesh{name="Batched scavenged accessory"};merged.CombineMeshes(group.Select(f=>new CombineInstance{mesh=f.sharedMesh,transform=worn.transform.worldToLocalMatrix*f.transform.localToWorldMatrix}).ToArray(),true,true);meshes.Add(merged);var g=new GameObject("Accessory material group");g.layer=gameObject.layer;g.transform.SetParent(worn.transform,false);g.AddComponent<MeshFilter>().sharedMesh=merged;g.AddComponent<MeshRenderer>().sharedMaterial=group.Key;}foreach(var f in filters){f.gameObject.SetActive(false);Destroy(f.gameObject);}}
 void OnDestroy(){Clear();}
}
}
