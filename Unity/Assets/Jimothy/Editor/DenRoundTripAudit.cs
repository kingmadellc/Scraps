using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
namespace Jimothy.Editor {
public static class DenRoundTripAudit {
 [Serializable] public class Check {public string route,leg;public int waypoint,frames,jumpTaps;public bool passed;public Vector3 expected,actual;}
 [Serializable] public class Report {public bool passed;public string error;public string method="Two continuous Den-to-north-roof-checkpoint-to-Den round trips. Actual RaccoonMotor and CharacterController, explicit world-direction input at60Hz, jump taps only on ascent, initial placement only per route. No teleport recovery, trick shortcut, GameSession or save access. A scripted waypoint test, not a human readability test.";public List<Check> checks=new();}
 public static void Run(){var result=new Report();var previous=Physics.simulationMode;try{
  EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);Physics.simulationMode=SimulationMode.Script;ClosingTimeWorld.Build(null);NeighborWorldObstacles.Ensure();RooftopConnections.Build(null);Physics.SyncTransforms();
  foreach(var sample in new[]{
   (new Vector3(-7,.24f,40),new Vector3(-7,.24f,44)),
   (new Vector3(7,.24f,40),new Vector3(7,.24f,44)),
   (new Vector3(7,.24f,44),new Vector3(-10,.05f,44)),
   (new Vector3(-10,.05f,44),ClosingTimeWorld.DenEntrance),
   (ClosingTimeWorld.DenEntrance,new Vector3(-14,-1.95f,55)),
   (new Vector3(-14,-1.95f,55),ClosingTimeWorld.DenInterior)}){
   var target=ClosingTimeWorld.ReturnStreetWaypoint(sample.Item1);result.checks.Add(new Check{route="Shared",leg="Street return guidance checkpoint",passed=Vector3.Distance(target,sample.Item2)<.001f,expected=sample.Item2,actual=target});
  }
  foreach(var route in ClosingTimeWorld.DenRoundTrips){
   var go=new GameObject("Round trip audit "+route.Id);go.AddComponent<CharacterController>();var motor=go.AddComponent<RaccoonMotor>();motor.Initialize(null);motor.Running=false;motor.Teleport(route.Outbound[0]);
   void Tick(Vector3 direction,bool jump){Physics.SyncTransforms();motor.SimulateMovement(direction,jump,1f/60);Physics.Simulate(1f/60);}
   for(int f=0;f<45;f++)Tick(Vector3.zero,false);bool failed=false;
   foreach(bool returning in new[]{false,true}){var path=returning?route.Return:route.Outbound;for(int i=1;i<path.Length;i++){
    var check=new Check{route=route.Id,leg=returning?"Return to Den":"Outbound",waypoint=i,expected=path[i]};result.checks.Add(check);int stable=0,jumpCooldown=0;
    for(int f=0;f<720;f++){
     var difference=path[i]-go.transform.position;var flat=Vector3.ProjectOnPlane(difference,Vector3.up);bool jump=!returning&&difference.y>.35f&&motor.Grounded&&!motor.IsMantling&&jumpCooldown<=0;if(jump){check.jumpTaps++;jumpCooldown=45;}jumpCooldown--;
     Tick(flat.magnitude>.055f?flat.normalized*Mathf.Clamp01(flat.magnitude/.32f):Vector3.zero,jump);check.frames++;
     var offset=path[i]-go.transform.position;stable=motor.Grounded&&!motor.IsMantling&&Vector3.ProjectOnPlane(offset,Vector3.up).magnitude<.20f&&Mathf.Abs(offset.y)<.20f?stable+1:0;
     if(stable>=4){check.passed=true;break;}
     // A missed roof jump is reported, never repaired by repositioning.
     if(path[i].y>3&&go.transform.position.y<path[i].y-2.5f)break;
    }
    check.actual=go.transform.position;if(!check.passed){failed=true;break;}
   }if(failed)break;}
   result.checks.Add(new Check{route=route.Id,leg="Returned inside physical Den",passed=!failed&&ClosingTimeWorld.DenContains(go.transform.position),actual=go.transform.position});
   var spot=ClosingTimeWorld.Spots.Find(s=>s.id==route.CacheId);var end=route.Outbound[route.Outbound.Length-1];result.checks.Add(new Check{route=route.Id,leg="Existing cache has validated search approaches",passed=spot.rooftop&&ScavengeSites.ValidSites(spot).Count>=2,expected=spot.position,actual=end});
   int progress=ClosingTimeWorld.NextWaypoint(route.Outbound[7],false,route.Outbound,7);result.checks.Add(new Check{route=route.Id,leg="Airborne stair proximity does not advance guidance",passed=progress==7});

   var npc=new GameObject("Ground pursuit safeguard");var cc=npc.AddComponent<CharacterController>();NeighborAI.ConfigureCollider(cc,NeighborKind.Dog);npc.transform.position=ClosingTimeWorld.RouteStarts[route.Id=="A"?4:9];var navigation=npc.AddComponent<NeighborNavigation>();navigation.Initialize(cc,.62f,.84f);result.checks.Add(new Check{route=route.Id,leg="Roof cannot become NPC ground destination",passed=!navigation.Plan(end)&&!navigation.TryPursuitGround(end,out _)});UnityEngine.Object.DestroyImmediate(npc);UnityEngine.Object.DestroyImmediate(go);
  }
  result.passed=ClosingTimeWorld.DenRoundTrips.Count==2&&result.checks.TrueForAll(c=>c.passed);
 }catch(Exception e){result.error=e.ToString();Debug.LogException(e);}finally{Physics.simulationMode=previous;var path=Path.GetFullPath("../Logs/den-round-trip-audit.json");Directory.CreateDirectory(Path.GetDirectoryName(path));File.WriteAllText(path,JsonUtility.ToJson(result,true));Debug.Log("DEN_ROUND_TRIP_AUDIT "+result.passed);}if(Application.isBatchMode)EditorApplication.Exit(result.passed?0:2);
 }
}
}
