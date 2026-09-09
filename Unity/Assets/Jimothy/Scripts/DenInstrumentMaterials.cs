using System.Collections.Generic;
using UnityEngine;
namespace Jimothy {
public sealed class DenInstrumentMaterials:MonoBehaviour {
 readonly List<Material> owned=new();
 public static void Apply(GameObject target){var owner=target.AddComponent<DenInstrumentMaterials>();var cache=new Dictionary<string,Material>();foreach(var renderer in target.GetComponentsInChildren<Renderer>()){var source=renderer.sharedMaterials;for(int i=0;i<source.Length;i++){string name=source[i]?source[i].name.Replace(" (Instance)",""):"Den_BlackPlastic";if(!cache.TryGetValue(name,out var material)){
   material=new Material(Shader.Find("Universal Render Pipeline/Lit")){name=name+" URP"};var texture=Resources.Load<Texture2D>("DenAssets/"+name+"_BaseColor");Color color=name.Contains("Nickel")?new(.67f,.70f,.72f):name.Contains("Brass")?new(.47f,.31f,.12f):name.Contains("Ivory")?new(.78f,.73f,.58f):name.Contains("Butterscotch")?new(.62f,.35f,.10f):name.Contains("Maple")?new(.67f,.49f,.27f):new(.04f,.043f,.04f);if(texture){material.SetTexture("_BaseMap",texture);color=Color.white;}
   material.SetColor("_BaseColor",color);material.SetFloat("_Metallic",name.Contains("Nickel")||name.Contains("Brass")?.82f:0);material.SetFloat("_Smoothness",name.Contains("Nickel")?.62f:name.Contains("Butterscotch")?.44f:name.Contains("Maple")?.25f:name.Contains("Grille")?.04f:.13f);cache.Add(name,material);owner.owned.Add(material);
  }source[i]=material;}renderer.sharedMaterials=source;}}
 void OnDestroy(){foreach(var m in owned)if(m)Destroy(m);}
}}
