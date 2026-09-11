using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace Jimothy.Editor {
public static class MantleRecoveryAudit {
 [Serializable] public class Result { public bool passed,settlingCleared,movementRecovered,jumpRecovered,blockedCleared; public int settlingFrames,blockedFrames; public float jumpRise,movementDistance; public Vector3 initial,settled,final; public string error,method; }
 const float Dt=1f/60f; static RaccoonMotor motor;
 static void Field(string name,object value) { var f=typeof(RaccoonMotor).GetField(name,BindingFlags.Instance|BindingFlags.NonPublic);if(f==null)throw new MissingFieldException(name);f.SetValue(motor,value); }
 static void Tick(Vector3 direction,bool jump=false) { Physics.SyncTransforms();motor.SimulateMovement(direction,jump,Dt);Physics.Simulate(Dt); }
 static void Seed(Vector3 position,Vector3 goal,int stage) { motor.Teleport(position);Physics.SyncTransforms();Field("mantleRaisedStart",goal);Field("mantleRaisedEnd",goal);Field("mantleLanding",goal);Field("mantleStage",stage);Field("mantling",true); }
 [MenuItem("Scraps/Audit mantle recovery")]
 public static void Run() {
  var report=new Result{initial=new Vector3(8.683109f,2.0699999f,-22.909088f),method="Fresh unsaved scene, real world colliders and CharacterController.Move through SimulateMovement at 60Hz; reflection injects only an interrupted mantle. No GameSession or save access."};
  var oldMode=Physics.simulationMode;
  try {
   EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);Physics.simulationMode=SimulationMode.Script;
   ClosingTimeWorld.Build(null);RooftopConnections.Build(null);
   var go=new GameObject("Mantle recovery audit player");go.AddComponent<CharacterController>();motor=go.AddComponent<RaccoonMotor>();motor.Initialize(null);motor.Running=false;
   Seed(report.initial,new Vector3(8.683109f,2.04f,-22.909088f),2);
   for(int i=0;i<60;i++){Tick(Vector3.zero);if(!motor.IsMantling&&report.settlingFrames==0)report.settlingFrames=i+1;}
   report.settlingCleared=!motor.IsMantling;report.settled=motor.transform.position;
   float baseY=motor.transform.position.y,apex=baseY;
   for(int i=0;i<30;i++){Tick(Vector3.zero,i==0);apex=Mathf.Max(apex,motor.transform.position.y);}
   report.jumpRise=apex-baseY;report.jumpRecovered=report.jumpRise>1f;
   for(int i=0;i<90;i++)Tick(Vector3.zero);
   Vector3 before=motor.transform.position;for(int i=0;i<12;i++)Tick(Vector3.left);
   Vector3 travelled=motor.transform.position-before;travelled.y=0;report.movementDistance=travelled.magnitude;report.movementRecovered=travelled.magnitude>.25f;report.final=motor.transform.position;
   // Stage 1 is deliberately asked to descend 3cm through a solid pad. This
   // reproduces negligible blocked motion outside the final landing stage,
   // exercising the recovery deadline even when positional error is small.
   var pad=new GameObject("Blocked mantle timeout pad");pad.transform.position=new Vector3(1000,-.5f,0);pad.AddComponent<BoxCollider>().size=new Vector3(8,1,8);
   motor.Teleport(new Vector3(1000,.07f,0));Physics.SyncTransforms();for(int i=0;i<45;i++)Tick(Vector3.zero);
   Vector3 blockedStart=motor.transform.position;Seed(blockedStart,blockedStart-Vector3.up*.03f,1);
   for(int i=0;i<180;i++){Tick(Vector3.zero);if(!motor.IsMantling){report.blockedFrames=i+1;break;}}
   report.blockedCleared=!motor.IsMantling;
   report.passed=report.settlingCleared&&report.jumpRecovered&&report.movementRecovered&&report.blockedCleared;
  } catch(Exception e){report.error=e.ToString();Debug.LogException(e);}
  finally {Physics.simulationMode=oldMode;string path=Path.GetFullPath(Path.Combine(Application.dataPath,"../Logs/mantle-recovery-audit.json"));Directory.CreateDirectory(Path.GetDirectoryName(path));File.WriteAllText(path,JsonUtility.ToJson(report,true));Debug.Log("MANTLE_RECOVERY_AUDIT "+(report.passed?"PASS":"FAIL")+" "+path);}
  if(Application.isBatchMode)EditorApplication.Exit(report.passed?0:2);
 }
}
}
