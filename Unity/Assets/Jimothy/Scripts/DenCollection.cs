using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Jimothy {
/// <summary>A physical, deterministic showcase of owned finds. Never removes or rerolls inventory.</summary>
public static class DenCollection {
 static int Rarity(string value)=>value=="legendary"?4:value=="rare"?3:value=="uncommon"?2:1;
 public static List<string> Select(SaveData data,Dictionary<string,ItemDefinition> catalog,int capacity)=>data.trophies.Concat(data.pantry).Distinct().Where(catalog.ContainsKey)
  .OrderByDescending(id=>data.favoriteFinds!=null&&data.favoriteFinds.Contains(id)).ThenByDescending(id=>catalog[id].category=="trophy").ThenByDescending(id=>catalog[id].category!="food").ThenByDescending(id=>Rarity(catalog[id].rarity)).ThenByDescending(id=>catalog[id].value).ThenBy(id=>id,System.StringComparer.Ordinal).Take(capacity).ToList();
 public static void Build(Transform parent,IReadOnlyList<string> ids,Dictionary<string,ItemDefinition> catalog,List<string> decor){
  for(int i=0;i<ids.Count&&i<ClosingTimeWorld.DenDisplaySlots.Count;i++){
   var item=catalog[ids[i]];var model=ItemVisuals.Create(item,parent,"den display");model.name="Den find · "+item.id;model.AddComponent<DenDisplayedFind>().itemId=item.id;
   var renderer=model.GetComponent<Renderer>();var bounds=renderer.localBounds;float scale=Mathf.Min(1.15f,.55f/Mathf.Max(.01f,bounds.size.x),.48f/Mathf.Max(.01f,bounds.size.y),.50f/Mathf.Max(.01f,bounds.size.z));model.transform.localScale=Vector3.one*scale;
   Vector3 at=ClosingTimeWorld.DenDisplaySlots[i];float angle=at.x<-17?90:at.x>-11?270:180;model.transform.rotation=Quaternion.Euler(0,angle,0);model.transform.position=at+Vector3.up*.012f;
   model.transform.position+=Vector3.up*(at.y+.012f-renderer.bounds.min.y);
  }
  DenFurnishings.Build(parent,decor);
  if(decor.Contains("cushion")){var item=new ItemDefinition{id="den-cushion",name="Patchwork cushion",category="curio"};var g=GameObject.CreatePrimitive(PrimitiveType.Sphere);g.name=item.name;g.transform.SetParent(parent,false);g.transform.position=new Vector3(-15.6f,ClosingTimeWorld.DenFloor+.66f,62.2f);g.transform.localScale=new Vector3(.7f,.16f,.5f);Object.Destroy(g.GetComponent<Collider>());var mat=new Material(Shader.Find("Universal Render Pipeline/Lit"));mat.SetColor("_BaseColor",new Color(.38f,.12f,.075f));g.GetComponent<Renderer>().material=mat;g.AddComponent<DenOwnedMaterial>().material=mat;}
  if(decor.Contains("glass-floats")){for(int i=0;i<3;i++){var floatItem=new ItemDefinition{id="den-float",name="Glass float",category="curio"};var item=ItemVisuals.Create(floatItem,parent,"glass float collection");item.transform.position=new Vector3(-10.5f+i*.35f,-1.30f,62.25f);item.transform.localScale=Vector3.one*.65f;var line=new GameObject("Glass float hanging cord");line.transform.SetParent(parent,false);var cord=line.AddComponent<LineRenderer>();cord.positionCount=2;cord.SetPosition(0,new Vector3(item.transform.position.x,1.56f,62.25f));cord.SetPosition(1,item.transform.position+Vector3.up*.20f);cord.startWidth=cord.endWidth=.008f;var mat=new Material(Shader.Find("Universal Render Pipeline/Lit"));mat.SetColor("_BaseColor",new Color(.30f,.21f,.11f));cord.sharedMaterial=mat;line.AddComponent<DenOwnedMaterial>().material=mat;}}
 }
}
public sealed class DenDisplayedFind:MonoBehaviour{public string itemId;}
public sealed class DenOwnedMaterial:MonoBehaviour{public Material material;void OnDestroy(){if(material)Destroy(material);}}
}
