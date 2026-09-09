using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace Jimothy.Editor {
/// <summary>Independent static roof connectivity check. Existing motor audits separately cover real traversal.</summary>
public static class EnvironmentAccessAudit {
 [Serializable] public class Report{public bool passed;public int roofGrids,reachableFindApproaches,validSites;public List<string> checks=new();public string error,method="0.25m roof grid, real floor rays and raccoon capsule clearance; flood fill from each north entry; unobstructed last approach to each roof find. Supplemented by separate actual motor crossing/traversal audits. No game session or save loads.";}
 const float Step=.25f;const int Width=41,Depth=47;
 static void Check(bool success,string label,Report report){if(!success)throw new Exception(label);report.checks.Add(label);}
 static bool Clear(Vector3 feet)=>!Physics.CheckCapsule(feet+Vector3.up*.18f,feet+Vector3.up*.36f,.17f,1<<0,QueryTriggerInteraction.Ignore);
 static bool Link(Vector3 a,Vector3 b){Vector3 delta=b-a;float distance=delta.magnitude;if(distance<.001f)return true;return !Physics.CapsuleCast(a+Vector3.up*.18f,a+Vector3.up*.36f,.17f,delta/distance,distance,1<<0,QueryTriggerInteraction.Ignore);}
 public static void Run(){EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);var report=new Report();try{
  ClosingTimeWorld.Build(null);NeighborWorldObstacles.Ensure();RooftopConnections.Build(null);Physics.SyncTransforms();
  foreach(var spot in ClosingTimeWorld.Spots){var sites=ScavengeSites.ValidSites(spot);report.validSites+=sites.Count;Check(sites.Count>=(spot.id=="welcome_snack"?1:2),spot.id+" retains multiple supported search sites",report);}
  Check(report.validSites==63,"All63 authored search sites remain accessible at their endpoints",report);
  foreach(int side in new[]{-1,1})foreach(int row in new[]{0,2,4}){
   float baseY=new[]{7.2f,8.1f,9f,7.2f,8.1f}[row]+.18f,z=-30+row*16;
   var points=new Vector3[Width*Depth];var clear=new bool[points.Length];var reached=new bool[points.Length];
   for(int j=0;j<Depth;j++)for(int i=0;i<Width;i++){int id=j*Width+i;var at=new Vector3(side*(9.65f+i*Step),baseY,z-5.55f+j*Step);if(Physics.Raycast(at+Vector3.up*3,Vector3.down,out var hit,3.15f,1<<0,QueryTriggerInteraction.Ignore)&&hit.normal.y>.9f&&Mathf.Abs(hit.point.y-baseY)<.13f){points[id]=hit.point+Vector3.up*.025f;clear[id]=Clear(points[id]);}}
   Vector3 entry=ClosingTimeWorld.RoofRoutes[(side<0?0:5)+row].Last();int start=-1;float nearest=1.1f;
   for(int id=0;id<points.Length;id++)if(clear[id]){float d=Vector3.Distance(points[id],entry);if(d<nearest){nearest=d;start=id;}}
   Check(start>=0,"Roof "+side+"/"+row+" has clear north-entry grid seed",report);var queue=new Queue<int>();queue.Enqueue(start);reached[start]=true;
   while(queue.Count>0){int a=queue.Dequeue(),x=a%Width,y=a/Width;foreach(var n in new[]{new Vector2Int(x-1,y),new Vector2Int(x+1,y),new Vector2Int(x,y-1),new Vector2Int(x,y+1)}){if(n.x<0||n.x>=Width||n.y<0||n.y>=Depth)continue;int b=n.y*Width+n.x;if(clear[b]&&!reached[b]&&Link(points[a],points[b])){reached[b]=true;queue.Enqueue(b);}}}
   var spot=ClosingTimeWorld.Spots.First(s=>s.id=="roof_"+side+"_"+row);foreach(var goal in ScavengeSites.ValidSites(spot)){bool connected=false;for(int id=0;id<points.Length;id++)if(reached[id]&&Vector3.Distance(points[id],goal.approach)<.45f&&Link(points[id],goal.approach)){connected=true;break;}Check(connected,spot.id+" reachable from north entry: "+goal.context,report);report.reachableFindApproaches++;}report.roofGrids++;
  }
  Check(report.roofGrids==6&&report.reachableFindApproaches==18,"Six roof grids connect all18 regular rooftop find approaches",report);report.passed=true;
 }catch(Exception e){report.error=e.ToString();Debug.LogException(e);}finally{string path=Path.GetFullPath("../Logs/environment-access-v044.json");Directory.CreateDirectory(Path.GetDirectoryName(path));File.WriteAllText(path,JsonUtility.ToJson(report,true));Debug.Log("ENVIRONMENT_ACCESS_AUDIT "+(report.passed?"PASS":"FAIL"));}if(Application.isBatchMode)EditorApplication.Exit(report.passed?0:1);}
}
}
