using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace Jimothy.Editor {
public static class MotorPersistenceAudit {
 const float Dt=1f/60;static RaccoonMotor motor;
 [Serializable] class Check {public string name;public bool passed;public string evidence;}
 [Serializable] class Report {public bool passed;public string method="Actual CharacterController stepping and JSON snapshot round trips in an unsaved isolated scene; no GameSession/user saves. Checks post-landing snapshots, not a UI reload flow.";public List<Check> checks=new();}
 static void Tick(Vector3 direction,bool jump=false){Physics.SyncTransforms();motor.SimulateMovement(direction,jump,Dt);Physics.Simulate(Dt);}
 static void New(Vector3 position){if(motor)UnityEngine.Object.DestroyImmediate(motor.gameObject);motor=new GameObject("Persistence fixture motor").AddComponent<RaccoonMotor>();motor.Initialize(null);motor.Running=true;motor.Teleport(position);}
 static MotorSaveState Copy(MotorSaveState s)=>JsonUtility.FromJson<MotorSaveState>(JsonUtility.ToJson(s));
 static bool Reload(){var s=Copy(motor.CaptureState());var position=motor.transform.position;New(position);return motor.RestoreState(s);}
 static GameObject Box(Vector3 p,Vector3 scale){var g=new GameObject("Persistence fixture solid");g.transform.position=p;g.AddComponent<BoxCollider>().size=scale;return g;}
 public static void Run(){var report=new Report();var mode=Physics.simulationMode;var routes=new List<Vector3[]>(ClosingTimeWorld.RoofRoutes);EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);Physics.simulationMode=SimulationMode.Script;
 void CheckThat(string name,bool okay,string evidence=""){report.checks.Add(new Check{name=name,passed=okay,evidence=evidence});}
 try{
  Box(new(0,-.25f,0),new(100,.5f,100));New(new(0,9,0));for(int i=0;i<150;i++)Tick(Vector3.zero);float reference=motor.LastFallDamage;
  New(new(0,9,0));for(int i=0;i<30;i++)Tick(Vector3.zero);float velocity=motor.VerticalSpeed;var before=motor.CaptureState();bool restored=Reload();
  CheckThat("Midfall JSON restores velocity and original apex",restored&&Mathf.Abs(motor.VerticalSpeed-velocity)<.0001f&&motor.CaptureState().fallApex==before.fallApex,$"velocity={velocity} apex={before.fallApex}");
  for(int i=0;i<120;i++)Tick(Vector3.zero);CheckThat("Reloaded 9m fall damage equals uninterrupted fall",reference>70&&Mathf.Abs(motor.LastFallDamage-reference)<.02f,$"continuous={reference} resumed={motor.LastFallDamage}");
  var settled=motor.CaptureState();CheckThat("Resolved landing snapshot contains no pending flight",!settled.airTracking&&settled.flightTricks==0&&!settled.trickActive);
  Reload();for(int i=0;i<30;i++)Tick(Vector3.zero);CheckThat("Settled reload cannot repeat fall damage",motor.LastFallDamage==0);
  New(new(0,.04f,0));for(int i=0;i<30;i++)Tick(Vector3.zero);Tick(Vector3.zero,true);for(int i=0;i<5;i++)Tick(Vector3.zero);motor.RequestTrick();for(int i=0;i<8;i++)Tick(Vector3.zero);float phase=motor.CaptureState().trickElapsed;
  restored=Reload();CheckThat("Partial intentional flip retains phase",restored&&motor.IsTricking&&Mathf.Abs(motor.CaptureState().trickElapsed-phase)<.0001f,$"phase={phase}");
  for(int i=0;i<100;i++)Tick(Vector3.zero);CheckThat("Resumed flip earns one clean landing",motor.CompletedTricks==1&&motor.LastLandingPoints==150&&motor.LastLandingClean,$"tricks={motor.CompletedTricks} points={motor.LastLandingPoints}");
  Reload();for(int i=0;i<30;i++)Tick(Vector3.zero);CheckThat("Settled reward snapshot cannot award again",motor.LastLandingPoints==0&&motor.CompletedTricks==0);
  motor.FaceDirection(73);motor.SimulateSteering(Vector2.zero,new Vector2(0,-15),false,Dt);var view=motor.CaptureState();Reload();CheckThat("View heading and pitch survive reload",Mathf.Abs(Mathf.DeltaAngle(motor.ViewHeading,73))<.01f&&Mathf.Abs(motor.CaptureState().pitch-view.pitch)<.001f);
  var invalid=motor.CaptureState();invalid.vertical=float.NaN;CheckThat("Nonfinite snapshot rejected",!motor.RestoreState(invalid));
  ClosingTimeWorld.RoofRoutes.Clear();var path=new Vector3[4];for(int i=0;i<4;i++){float h=(i+1)*.9f;path[i]=new(2+i*1.03f,h+.04f,15);Box(new(path[i].x,h*.5f,15),new(1.5f,h,1.5f));}ClosingTimeWorld.RoofRoutes.Add(path);
  New(new(.5f,.04f,15));for(int i=0;i<30;i++)Tick(Vector3.zero);Tick(Vector3.right,true);for(int i=0;i<240&&!motor.IsMantling;i++)Tick(Vector3.right);
  var mantle=Copy(motor.CaptureState());var mantlePosition=motor.transform.position;bool began=motor.IsMantling;restored=Reload();CheckThat("Actual stair mantle resumes validated remaining path",began&&restored&&motor.IsMantling,$"stage={mantle.mantleStage} position={mantlePosition}");
  for(int i=0;i<160;i++){var delta=path[3]-motor.transform.position;delta.y=0;Tick(delta.magnitude>.15f?delta.normalized:Vector3.zero);}CheckThat("Restored mantle continues stair traversal",motor.transform.position.y>2.5f,$"position={motor.transform.position}");
  New(mantlePosition);var obstacle=Box(mantle.landing+Vector3.up*.45f,new(.5f,.9f,.5f));Physics.SyncTransforms();bool accepted=motor.RestoreState(mantle);
  CheckThat("New landing obstruction cancels mantle without erasing fall",began&&accepted&&!motor.IsMantling&&motor.CaptureState().airTracking&&motor.CaptureState().fallApex>=mantle.fallApex&&motor.VerticalSpeed<=-2);
 }catch(Exception e){CheckThat("Exception",false,e.ToString());}
 finally{ClosingTimeWorld.RoofRoutes.Clear();ClosingTimeWorld.RoofRoutes.AddRange(routes);Physics.simulationMode=mode;report.passed=report.checks.Count>=12&&report.checks.TrueForAll(c=>c.passed);var output=Path.GetFullPath("../PlaytestCaptures/motor-persistence-audit.json");Directory.CreateDirectory(Path.GetDirectoryName(output));File.WriteAllText(output,JsonUtility.ToJson(report,true));Debug.Log("MOTOR_PERSISTENCE_AUDIT "+report.passed);if(Application.isBatchMode)EditorApplication.Exit(report.passed?0:1);}
 }
}
}
