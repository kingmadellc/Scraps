using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace Jimothy.Editor {
public static class NeighborWorldNavigationAudit {
 [Serializable] public class Case {public string kind,obstacle;public bool passed,unreachableStopped;public int spawnRepairs,patrolGoalsReached;public float maxBodyPenetration,traveled,maxTravelFacingAngle;public Vector3 requested,repaired,end;}
 [Serializable] public class Report {public bool passed;public int propColliders;public string error,method="Actual district geometry, production species collider/clearance configuration, CharacterController.Move at60Hz using production 95deg/s patrol and 155deg/s dog chase turns/acceleration. All nine production spawn coordinates plus embedded spawn. No GameSession/save access.";public List<Case> cases=new();}
 static readonly Collider[] overlaps=new Collider[64];const float Dt=1f/60f;
 static float Penetration(NeighborNavigation nav,BoxCollider probe,ref string obstacle){
  Vector3 half=new(nav.ClearanceRadius,nav.ClearanceHeight*.5f-.035f,nav.ClearanceRadius),center=nav.transform.position+Vector3.up*(nav.ClearanceHeight*.5f+.035f);probe.size=half*2;
  int count=Physics.OverlapBoxNonAlloc(center,half,overlaps,Quaternion.identity,1,QueryTriggerInteraction.Ignore);float max=0;
  for(int i=0;i<count;i++){var c=overlaps[i];if(c is CharacterController||c.transform.IsChildOf(nav.transform)||c==probe||c.bounds.max.y<=nav.transform.position.y+.245f)continue;
   if(Physics.ComputePenetration(probe,center,Quaternion.identity,c,c.transform.position,c.transform.rotation,out _,out float depth)&&depth>max){max=depth;obstacle=c.name;}}
  return count==overlaps.Length?100:max;
 }
 [MenuItem("Jimothy/Audit neighbors in actual district")]
 public static void Run(){
  var report=new Report();var oldMode=Physics.simulationMode;
  try{
   EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);Physics.simulationMode=SimulationMode.Script;ClosingTimeWorld.Build(null);RooftopConnections.Build(null);NeighborWorldObstacles.Ensure();Physics.SyncTransforms();report.propColliders=UnityEngine.Object.FindFirstObjectByType<NeighborWorldObstacles>().ColliderCount;
   var probeObject=new GameObject("Disabled body clearance probe");var probe=probeObject.AddComponent<BoxCollider>();probe.enabled=false;
   var kinds=new[]{NeighborKind.Dog,NeighborKind.Cat,NeighborKind.AngryHuman,NeighborKind.KindHuman,NeighborKind.Fisherman,NeighborKind.Gull,NeighborKind.Passerby,NeighborKind.UnhousedNeighbor,NeighborKind.ImpairedPasserby,NeighborKind.Dog,NeighborKind.Dog,NeighborKind.Cat,NeighborKind.KindHuman};
   for(int i=0;i<kinds.Length;i++){
    var kind=kinds[i];Vector3 spawn=i<9?new Vector3(i%2==0?-7.2f:7.2f,.25f,-29+i*7.1f):i==9?new Vector3(9.7f,.25f,-30):new Vector3(6.25f,.25f,7);
    var result=new Case{kind=kind+(i==9?" embedded spawn":i>=10?" cafe crossing":""),requested=spawn};report.cases.Add(result);
    var actor=new GameObject("Audit "+kind);actor.transform.position=spawn;actor.transform.localScale=Vector3.one*(kind==NeighborKind.Cat?.38f:kind==NeighborKind.Gull?.55f:1f);
    var cc=actor.AddComponent<CharacterController>();NeighborAI.ConfigureCollider(cc,kind);var nav=actor.AddComponent<NeighborNavigation>();var footprint=NeighborAI.NavigationFootprint(kind);nav.Initialize(cc,footprint.x,footprint.y);Physics.SyncTransforms();
    result.repaired=actor.transform.position;result.spawnRepairs=nav.SpawnRepairs;Vector3 home=actor.transform.position,target=home;
    result.maxBodyPenetration=Penetration(nav,probe,ref result.obstacle);bool chosen;if(i>=10){target=new Vector3(8.65f,.2f,7);chosen=nav.Plan(target);}else chosen=nav.TryPatrolTarget(home,i*.81f,out target);
    for(int frame=0;frame<(i>=10?1920:1440)&&result.patrolGoalsReached<2;frame++){
     Vector3 before=actor.transform.position;nav.Step(target,kind==NeighborKind.Dog?3.4f:.8f,Dt,false,kind==NeighborKind.Dog?155:95);Physics.SyncTransforms();Physics.Simulate(Dt);result.traveled+=Vector3.Distance(before,actor.transform.position);var horizontal=Vector3.ProjectOnPlane(actor.transform.position-before,Vector3.up);if(horizontal.magnitude>.005f)result.maxTravelFacingAngle=Mathf.Max(result.maxTravelFacingAngle,Vector3.Angle(actor.transform.forward,horizontal));
     if(frame%4==0)result.maxBodyPenetration=Mathf.Max(result.maxBodyPenetration,Penetration(nav,probe,ref result.obstacle));
     if(chosen&&Vector2.Distance(new(target.x,target.z),new(actor.transform.position.x,actor.transform.position.z))<.15f){result.patrolGoalsReached++;if(i>=10){target=home;chosen=nav.Plan(target);}else chosen=nav.TryPatrolTarget(home,i*.81f+result.patrolGoalsReached*2.4f,out target);}
    }
    // Target well inside a solid building must yield no path and no wall walking.
    var impossible=new Vector3(14.6f,0,-30);bool unreachable=!nav.Plan(impossible);Vector3 stopped=actor.transform.position;
    for(int f=0;f<90;f++){nav.Step(impossible,3.4f,Dt,false,155);Physics.SyncTransforms();Physics.Simulate(Dt);}
    result.unreachableStopped=unreachable&&Vector3.ProjectOnPlane(actor.transform.position-stopped,Vector3.up).magnitude<.05f;
    result.end=actor.transform.position;result.passed=result.patrolGoalsReached>=2&&result.maxBodyPenetration<.025f&&result.unreachableStopped&&(i!=9||result.spawnRepairs>0);
    Debug.Log("NEIGHBOR_WORLD_CASE "+result.kind+" passed="+result.passed+" goals="+result.patrolGoalsReached+" penetration="+result.maxBodyPenetration+" obstacle="+result.obstacle);UnityEngine.Object.DestroyImmediate(actor);
   }
   report.passed=report.cases.TrueForAll(c=>c.passed)&&report.propColliders>=124;
  }catch(Exception e){report.error=e.ToString();Debug.LogException(e);}
  finally{Physics.simulationMode=oldMode;string path=Path.GetFullPath(Path.Combine(Application.dataPath,"../../Logs/neighbor-world-navigation-audit.json"));Directory.CreateDirectory(Path.GetDirectoryName(path));File.WriteAllText(path,JsonUtility.ToJson(report,true));Debug.Log("NEIGHBOR_WORLD_NAVIGATION_AUDIT "+(report.passed?"PASS":"FAIL")+" "+path);}
  if(Application.isBatchMode)EditorApplication.Exit(report.passed?0:2);
 }
}
}
