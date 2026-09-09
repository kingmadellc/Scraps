using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace Jimothy.Editor {
public static class TraversalAudit {
 [Serializable] public class StepResult {public int step,frames;public bool reached;public Vector3 target,actual,min,max;public bool grounded;public int mantles;}
 [Serializable] public class RouteResult {public int route,failedStep=-1;public bool reached;public Vector3 start;public List<StepResult> steps=new();}
 [Serializable] public class Report {public string utc,method;public bool passed,tapJumpPassed,blockedOverheadPassed;public float tapJumpRise;public int blockedMantles;public float blockedMaxPenetration;public Vector3 blockedFinal;public List<RouteResult> routes=new();}
 const float Dt=1f/60f;
 static RaccoonMotor motor;
 static void Tick(Vector3 direction,bool jump=false){Physics.SyncTransforms();motor.SimulateMovement(direction,jump,Dt);Physics.Simulate(Dt);}
 static void Settle(Vector3 position){motor.Teleport(position);Physics.SyncTransforms();for(int i=0;i<45;i++)Tick(Vector3.zero);}
 static GameObject Block(string name,Vector3 center,Vector3 size){var go=new GameObject(name);go.transform.position=center;go.AddComponent<BoxCollider>().size=size;return go;}
 public static void Run(){
  // Use a fresh unsaved scene; this command never edits a saved project scene.
  EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
  var oldSimulation=Physics.simulationMode;Physics.simulationMode=SimulationMode.Script;
  var report=new Report{utc=DateTime.UtcNow.ToString("O"),method="Actual CharacterController.Move through RaccoonMotor.SimulateMovement at 60Hz; Physics.Simulate advances each tick. No geometric-only pass criteria."};
  try {
   ClosingTimeWorld.Build(null);RooftopConnections.Build(null);
   var paths=new List<Vector3[]>(ClosingTimeWorld.RoofRoutes);var starts=new List<Vector3>(ClosingTimeWorld.RouteStarts);
   foreach(var connector in RooftopConnections.Paths){paths.Add(connector);starts.Add(connector[0]);}
   var player=new GameObject("Traversal audit Jimothy");player.AddComponent<CharacterController>();motor=player.AddComponent<RaccoonMotor>();motor.Initialize(null);motor.Running=false;
   for(int r=0;r<paths.Count;r++){
    var route=new RouteResult{route=r,start=starts[r]};report.routes.Add(route);Settle(route.start);
    var points=paths[r];
    for(int p=0;p<points.Length;p++){
     Vector3 target=points[p];
     // Aim for the exposed center of each tread, 20cm back from the authored center.
     // The following riser overlaps the platform: this avoids steering into its wall.
     if(r<ClosingTimeWorld.RoofRoutes.Count && p<points.Length-1){Vector3 next=points[p+1]-target;next.y=0;if(next.sqrMagnitude>.01f)target-=next.normalized*.20f;}
     var step=new StepResult{step=p,target=points[p],min=motor.transform.position,max=motor.transform.position};route.steps.Add(step);
     int groundedFrames=0;bool launched=false;float lastJump=-10;
     for(int frame=0;frame<240;frame++){
      Vector3 delta=target-motor.transform.position;delta.y=0;
      bool needRise=points[p].y-motor.transform.position.y>.25f;
      bool request=needRise && motor.Grounded && frame*Dt-lastJump>.35f;
      if(request){launched=true;lastJump=frame*Dt;}
      Vector3 input=delta.magnitude>.07f?delta.normalized*Mathf.Clamp01(delta.magnitude/.3f):Vector3.zero;
      Tick(input,request);step.frames=frame+1;Vector3 actual=motor.transform.position;
      step.min=Vector3.Min(step.min,actual);step.max=Vector3.Max(step.max,actual);
      Vector3 offset=actual-points[p];offset.y=0;
      bool supported=motor.Grounded && offset.magnitude<.56f && Mathf.Abs(actual.y-points[p].y)<.18f;
      groundedFrames=supported?groundedFrames+1:0;
      if(groundedFrames>=5){step.reached=true;break;}
      if(actual.y<-2 || (launched && frame>120 && actual.y<points[p].y-2))break;
     }
     step.actual=motor.transform.position;step.grounded=motor.Grounded;step.mantles=motor.CompletedMantles;
     if(!step.reached){route.failedStep=p;break;}
    }
    route.reached=route.failedStep<0 && route.steps.Count==points.Length;
    Debug.Log("TRAVERSAL_ROUTE "+r+" reached="+route.reached+" failedStep="+route.failedStep);
   }
   // Isolated pad removes world geometry from the one-frame jump test.
   Block("Tap jump ground",new Vector3(1000,-.25f,0),new Vector3(20,.5f,20));Settle(new Vector3(1000,.06f,0));
   float ground=motor.transform.position.y,apex=ground;Tick(Vector3.zero,true);
   for(int i=0;i<120;i++){Tick(Vector3.zero);apex=Mathf.Max(apex,motor.transform.position.y);}
   report.tapJumpRise=apex-ground;report.tapJumpPassed=report.tapJumpRise>1.70f&&report.tapJumpRise<2.0f&&motor.Grounded;
   // A low ceiling makes this landing too short for the capsule. Keep pressing into it.
   var ledge=Block("Blocked mantle ledge",new Vector3(1002,.55f,0),new Vector3(2,1.1f,3)).GetComponent<BoxCollider>();
   // Cover takeoff AND landing: the original narrow fixture could legitimately be jumped onto.
   var ceiling=Block("Blocked mantle ceiling",new Vector3(1001,1.1f+motor.GetComponent<CharacterController>().height*.8f+.15f,0),new Vector3(10,.3f,6)).GetComponent<BoxCollider>();
   Settle(new Vector3(1000,.06f,0));int before=motor.CompletedMantles;
   var capsule=motor.GetComponent<CharacterController>();
   var probeObject=new GameObject("Audit capsule penetration shape");var probe=probeObject.AddComponent<CapsuleCollider>();probe.enabled=false;probe.radius=capsule.radius;probe.height=capsule.height;probe.center=capsule.center;probe.direction=1;
   for(int i=0;i<180;i++){
    Tick(Vector3.right,i%45==0);
    foreach(var obstacle in new[]{ledge,ceiling}){
     if(Physics.ComputePenetration(probe,motor.transform.position,motor.transform.rotation,obstacle,obstacle.transform.position,obstacle.transform.rotation,out _,out float depth))
      report.blockedMaxPenetration=Mathf.Max(report.blockedMaxPenetration,depth);
    }
   }
   report.blockedMantles=motor.CompletedMantles-before;report.blockedFinal=motor.transform.position;
   report.blockedOverheadPassed=report.blockedMantles==0 && motor.transform.position.x<1001.05f && report.blockedMaxPenetration<=capsule.skinWidth+.025f;
   report.passed=report.tapJumpPassed&&report.blockedOverheadPassed&&report.routes.Count>0&&report.routes.TrueForAll(x=>x.reached);
  } catch(Exception e){Debug.LogException(e);report.passed=false;}
  finally {
   Physics.simulationMode=oldSimulation;
   string path=Path.GetFullPath(Path.Combine(Application.dataPath,"../Logs/traversal-audit.json"));Directory.CreateDirectory(Path.GetDirectoryName(path));File.WriteAllText(path,JsonUtility.ToJson(report,true));
   Debug.Log("TRAVERSAL_AUDIT "+(report.passed?"PASS":"FAIL")+" "+path);
  }
  if(Application.isBatchMode)EditorApplication.Exit(report.passed?0:2);
 }
}
}
