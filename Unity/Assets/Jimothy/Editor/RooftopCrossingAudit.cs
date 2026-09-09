using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
namespace Jimothy.Editor {
public static class RooftopCrossingAudit {
 [Serializable] public class Result {public int crossing;public bool reverse,reached;public int failedPoint=-1;public Vector3 end;public float minY;}
 [Serializable] public class Report {public string method="Actual RaccoonMotor.SimulateMovement / CharacterController.Move at 60Hz through all four crossing Paths in both directions, no jump requests; initial placement only. Physics advances each tick. Isolated unsaved scene, no GameSession/save load.";public bool passed;public List<Result> checks=new();}
 public static void Run(){EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);var previous=Physics.simulationMode;Physics.simulationMode=SimulationMode.Script;var report=new Report();try{
  ClosingTimeWorld.Build(null);NeighborWorldObstacles.Ensure();RooftopConnections.Build(null);var go=new GameObject("Rooftop crossing audit Jimothy");go.AddComponent<CharacterController>();var motor=go.AddComponent<RaccoonMotor>();motor.Initialize(null);motor.Running=false;
  void Tick(Vector3 input){Physics.SyncTransforms();motor.SimulateMovement(input,false,1f/60);Physics.Simulate(1f/60);}
  for(int crossing=0;crossing<RooftopConnections.Paths.Count;crossing++)foreach(bool reverse in new[]{false,true}){
   var path=(Vector3[])RooftopConnections.Paths[crossing].Clone();if(reverse)Array.Reverse(path);motor.Teleport(path[0]+Vector3.up*.07f);for(int i=0;i<45;i++)Tick(Vector3.zero);
   var result=new Result{crossing=crossing,reverse=reverse,minY=motor.transform.position.y};report.checks.Add(result);
   for(int point=0;point<path.Length;point++){int stable=0;bool reached=false;for(int frame=0;frame<240;frame++){var delta=path[point]-motor.transform.position;delta.y=0;Tick(delta.magnitude>.07f?delta.normalized*Mathf.Clamp01(delta.magnitude/.35f):Vector3.zero);var actual=motor.transform.position;result.minY=Mathf.Min(result.minY,actual.y);var offset=actual-path[point];offset.y=0;stable=motor.Grounded&&offset.magnitude<.22f&&Mathf.Abs(actual.y-path[point].y)<.18f?stable+1:0;if(stable>=5){reached=true;break;}if(actual.y<6)break;}if(!reached){result.failedPoint=point;break;}}
   result.reached=result.failedPoint<0;result.end=motor.transform.position;
  }
  report.passed=report.checks.Count==8&&report.checks.TrueForAll(r=>r.reached);
 }catch(Exception e){Debug.LogException(e);report.passed=false;}finally{Physics.simulationMode=previous;var path=Path.GetFullPath("../PlaytestCaptures/rooftop-crossing-audit.json");Directory.CreateDirectory(Path.GetDirectoryName(path));File.WriteAllText(path,JsonUtility.ToJson(report,true));Debug.Log("ROOFTOP_CROSSING_AUDIT "+report.passed);}
 if(Application.isBatchMode)EditorApplication.Exit(report.passed?0:2);
 }
}
}
