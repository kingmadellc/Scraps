using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace Jimothy.Editor {
public static class DenSceneryClearanceAudit {
 public static void Run(){int candidates=0,meshes=0;try{
  EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);ClosingTimeWorld.Build(null);
  // Render-only district scenery has no colliders. Inspect actual triangle footprints,
  // not Physics: broad bounds alone miss triangles spanning the entire basement.
  foreach(var mf in UnityEngine.Object.FindObjectsByType<MeshFilter>(FindObjectsSortMode.None)){
   if(!mf.name.StartsWith("Architecture neighborhood_"))continue;
   var mesh=mf.sharedMesh;if(!mesh)continue;meshes++;var v=mesh.vertices;var t=mesh.triangles;
   for(int k=0;k<t.Length;k+=3){var a=mf.transform.TransformPoint(v[t[k]]);var b=mf.transform.TransformPoint(v[t[k+1]]);var c=mf.transform.TransformPoint(v[t[k+2]]);
    float low=Mathf.Min(a.y,b.y,c.y),high=Mathf.Max(a.y,b.y,c.y);if(high< -1.3f||low>1.35f)continue;
    if(Mathf.Max(a.x,b.x,c.x)<-19.7f||Mathf.Min(a.x,b.x,c.x)>-8.3f||Mathf.Max(a.z,b.z,c.z)<56||Mathf.Min(a.z,b.z,c.z)>63.15f)continue;candidates++;
    for(float x=-19.6f;x< -8.3f;x+=.25f)for(float z=56.1f;z<63.15f;z+=.25f)if(Inside(new Vector2(x,z),new Vector2(a.x,a.z),new Vector2(b.x,b.z),new Vector2(c.x,c.z)))throw new Exception("Distant scenery intrudes into Den: "+mf.name+" triangle "+k/3);
   }
  }
  if(meshes<8)throw new Exception("Scenery inspection did not find neighborhood meshes");
  File.WriteAllText(Path.GetFullPath("../Logs/den-scenery-clearance.json"),"{\"passed\":true,\"candidateTriangles\":"+candidates+",\"method\":\"actual rendered neighborhood triangle footprints across basement, no player saves\"}");
  Debug.Log("DEN_SCENERY_CLEARANCE PASS");if(Application.isBatchMode)EditorApplication.Exit(0);
 }catch(Exception e){Debug.LogException(e);if(Application.isBatchMode)EditorApplication.Exit(2);}}
 static bool Inside(Vector2 p,Vector2 a,Vector2 b,Vector2 c){float area=Cross(b-a,c-a);if(Mathf.Abs(area)<.00001f)return false;float u=Cross(b-p,c-p)/area,v=Cross(c-p,a-p)/area,w=1-u-v;return u>=0&&v>=0&&w>=0;}
 static float Cross(Vector2 a,Vector2 b)=>a.x*b.y-a.y*b.x;
}
}
