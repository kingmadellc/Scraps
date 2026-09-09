using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
namespace Jimothy {
/// <summary>Keep a find's independent availability while reducing its decorative draw calls.</summary>
public static class FindVisualOptimization {
 public static void Combine(GameObject root){
  var groups=new Dictionary<Material,List<CombineInstance>>();
  var parts=root.GetComponentsInChildren<MeshFilter>();
  foreach(var part in parts){
   var renderer=part.GetComponent<MeshRenderer>();if(!renderer||!part.sharedMesh)continue;
   var materials=renderer.sharedMaterials;
   for(int sub=0;sub<part.sharedMesh.subMeshCount;sub++){
    var material=materials[sub];if(!groups.TryGetValue(material,out var instances)){instances=new List<CombineInstance>();groups.Add(material,instances);}
    instances.Add(new CombineInstance{mesh=part.sharedMesh,subMeshIndex=sub,transform=root.transform.worldToLocalMatrix*part.transform.localToWorldMatrix});
   }
  }
  var meshes=new List<Mesh>();var renderers=new List<Renderer>();
  foreach(var group in groups){
   var mesh=new Mesh{name=root.name+" combined find",indexFormat=IndexFormat.UInt32};mesh.CombineMeshes(group.Value.ToArray(),true,true);mesh.RecalculateBounds();mesh.UploadMeshData(true);meshes.Add(mesh);
   var go=new GameObject("Find surface");go.transform.SetParent(root.transform,false);go.AddComponent<MeshFilter>().sharedMesh=mesh;
   var renderer=go.AddComponent<MeshRenderer>();renderer.sharedMaterial=group.Key;renderer.shadowCastingMode=ShadowCastingMode.Off;renderer.receiveShadows=false;renderers.Add(renderer);
  }
  // Detach before deferred destruction so LootNode.Initialize never caches dead renderers.
  foreach(var part in parts){var renderer=part.GetComponent<Renderer>();if(renderer)renderer.enabled=false;part.transform.SetParent(null,true);Object.Destroy(part.gameObject);}
  root.AddComponent<ClosingWorldCleanup>().ownedMeshes=meshes.ToArray();
  root.AddComponent<FindVisualDistance>().renderers=renderers.ToArray();
 }
}
public class FindVisualDistance:MonoBehaviour {
 public Renderer[] renderers;float nextCheck;
 void LateUpdate(){
  if(Time.unscaledTime<nextCheck)return;nextCheck=Time.unscaledTime+.25f;
  var game=GameSession.Instance;bool hidden=game&&game.Player&&(game.Player.transform.position-transform.position).sqrMagnitude>900;
  // This is independent of renderer.enabled, which remains owned by collection/respawn logic.
  foreach(var renderer in renderers)if(renderer)renderer.forceRenderingOff=hidden;
 }
}
}
