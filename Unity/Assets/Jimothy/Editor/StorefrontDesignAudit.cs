using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace Jimothy.Editor {
public static class StorefrontDesignAudit {
 [Serializable] class Report {public bool passed;public string error,method="Fresh authored district, actual collider clearance and shop lettering bounds. Checks identity/entry labels and street approach safety. Does not claim legibility from a human playtest or judge the final rendered visual polish.";public string[] doorwayLabels;public int letters,materials;public long meshTriangles;public List<string> checks=new();}
 public static void Run(){var report=new Report();try{
  EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);ClosingTimeWorld.Build(null);NeighborWorldObstacles.Ensure();RooftopConnections.Build(null);Physics.SyncTransforms();
  void Check(bool test,string description){if(!test)throw new Exception(description);report.checks.Add(description);}
  var words=UnityEngine.Object.FindObjectsByType<TextMesh>(FindObjectsSortMode.None);report.letters=words.Count(t=>t.name.StartsWith("Shop wordmark:"));
  var doors=words.Where(t=>t.text.Contains("BALLARD AVE")).ToArray();report.doorwayLabels=doors.Select(t=>t.text).ToArray();Check(doors.All(t=>t.text.Split('\n').Length==2&&!t.text.Contains("\\n")&&t.text.Split('\n')[1].Length>0),"Door information uses a real line break and nonempty opening/service line");Check(doors.Length==10,"Ten individual street-number/opening-time doorway labels");Check(doors.Select(t=>t.text).Distinct().Count()==10,"All ten door addresses are unique");
  string[] notices={"MARINE REPAIRS","KITCHEN PREP","TOMORROW'S BREAD","LAST CALL","FRESH CATCH","CELLAR NOTES","LEAGUE NIGHT","FRESH CUTTINGS","SLOW COFFEE","LISTEN HERE"};
  foreach(var text in notices)Check(words.Count(t=>t.text==text)==1,"One authored working-window identity: "+text);
  foreach(var label in doors){var p=label.transform.position;var b=label.GetComponent<Renderer>().bounds;Check(float.IsFinite(b.size.x)&&float.IsFinite(b.size.y)&&float.IsFinite(b.size.z)&&b.size.y>0&&b.size.y<.65f&&Mathf.Max(b.size.x,b.size.z)<1.4f,"Door lettering fits its glazing: "+label.text.Replace('\n',' '));float side=Mathf.Sign(p.x);var eye=new Vector3(side*6.95f,.65f,p.z);Check(!Physics.CheckCapsule(eye-Vector3.up*.20f,eye+Vector3.up*.1f,.17f,1,QueryTriggerInteraction.Ignore),"Street door sight position remains unobstructed: "+p);Check(!Physics.Linecast(eye,p,1,QueryTriggerInteraction.Ignore),"Door label faces a clear street sightline: "+p);}
  var renderers=UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None);var materials=renderers.SelectMany(r=>r.sharedMaterials).Where(m=>m).Distinct().ToArray();report.materials=materials.Length;Check(materials.All(m=>m.shader&&!m.shader.name.Contains("InternalError")),"No missing/error shader materials in rebuilt storefront world");report.meshTriangles=UnityEngine.Object.FindObjectsByType<MeshFilter>(FindObjectsSortMode.None).Where(f=>f.sharedMesh).Sum(f=>(long)f.sharedMesh.triangles.Length/3);Check(ClosingTimeWorld.RoofRoutes.Count==10&&ClosingTimeWorld.DenRoundTrips.Count==2,"Ten roof accesses and both named round trips retained");report.passed=true;
 }catch(Exception e){report.error=e.ToString();Debug.LogException(e);}var path=Path.GetFullPath("../Logs/storefront-design-audit.json");Directory.CreateDirectory(Path.GetDirectoryName(path));File.WriteAllText(path,JsonUtility.ToJson(report,true));Debug.Log("STOREFRONT_DESIGN_AUDIT "+report.passed);if(Application.isBatchMode)EditorApplication.Exit(report.passed?0:2);}
}
}
