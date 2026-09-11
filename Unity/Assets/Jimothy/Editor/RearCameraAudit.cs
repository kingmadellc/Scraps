using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace Jimothy.Editor {
public static class RearCameraAudit {
 [Serializable] public class Sample {public string name,obstruction;public Vector3 player,camera;public float heading,rearDot,distance;public bool behind,clear,outsideCharacter;}
 [Serializable] public class Report {public bool horizonVisible,routeAheadVisible;public float horizonViewportY,routeViewportY;public bool passed,forwardPassed,reversePassed,turnPassed,leftTurnPassed,lookTurnPassed,jumpPassed;public float forwardDistance,reverseDistance,turnDegrees,leftTurnDegrees,lookTurnDegrees,jumpRise;public int failedSamples;public string error,method;public List<Sample> samples=new();}
 static RaccoonMotor motor;static Camera camera;static Report report;const float Dt=1f/60f;
 static void Tick(Vector2 input,Vector2 look=default,bool jump=false){Physics.SyncTransforms();motor.SimulateSteering(input,look,jump,Dt);Physics.Simulate(Dt);motor.UpdateFollowCamera(Dt);}
 static bool Relevant(Collider c)=>c&&!c.transform.IsChildOf(motor.transform)&&!c.GetComponentInParent<NeighborAI>();
 static void SampleCamera(string name){
  Vector3 target=motor.transform.position+Vector3.up*.32f,ray=camera.transform.position-target,horizontal=Vector3.ProjectOnPlane(ray,Vector3.up);
  var s=new Sample{name=name,player=motor.transform.position,camera=camera.transform.position,heading=motor.visual.eulerAngles.y,distance=ray.magnitude};
  s.rearDot=horizontal.sqrMagnitude>.000001f?Vector3.Dot(horizontal.normalized,motor.visual.forward):-1;
  s.behind=horizontal.sqrMagnitude>.000001f&&s.rearDot<-.99f;s.outsideCharacter=s.distance>=.65f;s.clear=true;
  // Independent static geometry check with slightly smaller probe than the motor's .10m.
  // Check the lens as well: SphereCast alone does not report initial overlap.
  foreach(var hit in Physics.SphereCastAll(target,.09f,ray.normalized,ray.magnitude,~(1<<2),QueryTriggerInteraction.Ignore))if(Relevant(hit.collider)){s.clear=false;s.obstruction=hit.collider.name;break;}
  foreach(var c in Physics.OverlapSphere(camera.transform.position,.09f,~(1<<2),QueryTriggerInteraction.Ignore))if(Relevant(c)){s.clear=false;s.obstruction=c.name;break;}
  if(!s.behind||!s.clear||!s.outsideCharacter)report.failedSamples++;report.samples.Add(s);
 }
 static void Settle(Vector3 position){motor.Teleport(position);for(int i=0;i<45;i++)Tick(Vector2.zero);}
 static void Heading(float degrees){Tick(Vector2.zero,new Vector2(Mathf.DeltaAngle(motor.visual.eulerAngles.y,degrees),0));}
 [MenuItem("Scraps/Audit rear follow camera")]
 public static void Run(){
  report=new Report{method="Fresh unsaved scene; actual world colliders, CharacterController, SimulateSteering and UpdateFollowCamera at 60Hz. No GameSession or save access. Camera checked at every steering tick and 24 headings at each reported obstruction. Outside-character is a conservative .65m target-to-lens threshold, not a rendered fur test."};var oldMode=Physics.simulationMode;
  try {
   EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);Physics.simulationMode=SimulationMode.Script;ClosingTimeWorld.Build(null);RooftopConnections.Build(null);NeighborWorldObstacles.Ensure();
   camera=new GameObject("Rear camera audit camera").AddComponent<Camera>();var go=new GameObject("Rear camera audit player");go.AddComponent<CharacterController>();motor=go.AddComponent<RaccoonMotor>();motor.visual=new GameObject("Audit visual heading").transform;motor.visual.SetParent(go.transform,false);motor.Initialize(camera);motor.Running=false;
   Settle(new Vector3(0,.07f,30));Heading(180);
   camera.fieldOfView=62;camera.aspect=16f/9;motor.RecenterCamera();motor.UpdateFollowCamera(Dt);
   var distant=camera.WorldToViewportPoint(camera.transform.position+motor.visual.forward*1000);var route=camera.WorldToViewportPoint(motor.transform.position+motor.visual.forward*25);
   report.horizonViewportY=distant.y;report.routeViewportY=route.y;report.horizonVisible=distant.z>0&&distant.y>.40f&&distant.y<.80f;report.routeAheadVisible=route.z>0&&route.y>.30f&&route.y<.80f;
   Vector3 before=motor.transform.position,forward=motor.visual.forward;
   for(int i=0;i<30;i++){Tick(Vector2.up);SampleCamera("free forward "+i);}report.forwardDistance=Vector3.Dot(motor.transform.position-before,forward);report.forwardPassed=report.forwardDistance>1.2f;
   before=motor.transform.position;float heading=motor.visual.eulerAngles.y;
   for(int i=0;i<30;i++){Tick(Vector2.down);SampleCamera("free reverse "+i);}report.reverseDistance=-Vector3.Dot(motor.transform.position-before,forward);report.reversePassed=report.reverseDistance>1.2f&&Mathf.Abs(Mathf.DeltaAngle(heading,motor.visual.eulerAngles.y))<.1f;
   before=motor.transform.position;heading=motor.visual.eulerAngles.y;
   for(int i=0;i<30;i++){Tick(Vector2.right);SampleCamera("steer in place "+i);}report.turnDegrees=Mathf.DeltaAngle(heading,motor.visual.eulerAngles.y);report.turnPassed=Mathf.Abs(report.turnDegrees-62.5f)<1&&Vector3.ProjectOnPlane(motor.transform.position-before,Vector3.up).magnitude<.02f;
   heading=motor.visual.eulerAngles.y;for(int i=0;i<30;i++){Tick(Vector2.left);SampleCamera("left steer in place "+i);}report.leftTurnDegrees=Mathf.DeltaAngle(heading,motor.visual.eulerAngles.y);report.leftTurnPassed=Mathf.Abs(report.leftTurnDegrees+62.5f)<1;
   heading=motor.visual.eulerAngles.y;Tick(Vector2.zero,new Vector2(90,0));SampleCamera("right drag 90");report.lookTurnDegrees=Mathf.DeltaAngle(heading,motor.visual.eulerAngles.y);report.lookTurnPassed=Mathf.Abs(report.lookTurnDegrees-90)<.1f;
   float baseY=motor.transform.position.y,apex=baseY;for(int i=0;i<90;i++){Tick(Vector2.zero,Vector2.zero,i==0);apex=Mathf.Max(apex,motor.transform.position.y);SampleCamera("tap jump "+i);}report.jumpRise=apex-baseY;report.jumpPassed=report.jumpRise>1.7f;
   var locations=new[]{new Vector3(-7.1f,.27f,41),new Vector3(9.16f,.27f,23.95f),ClosingTimeWorld.Home};
   for(int p=0;p<locations.Length;p++){Settle(locations[p]);for(int a=0;a<24;a++){Heading(a*15);SampleCamera((p==0?"tree":p==1?"wall":"den")+" immediate "+a*15);for(int i=0;i<12;i++)Tick(Vector2.zero);SampleCamera((p==0?"tree":p==1?"wall":"den")+" settled "+a*15);}}
   report.passed=report.horizonVisible&&report.routeAheadVisible&&report.forwardPassed&&report.reversePassed&&report.turnPassed&&report.leftTurnPassed&&report.lookTurnPassed&&report.jumpPassed&&report.failedSamples==0;
  }catch(Exception e){report.error=e.ToString();Debug.LogException(e);}
  finally{Physics.simulationMode=oldMode;string path=Path.GetFullPath(Path.Combine(Application.dataPath,"../Logs/rear-camera-audit.json"));Directory.CreateDirectory(Path.GetDirectoryName(path));File.WriteAllText(path,JsonUtility.ToJson(report,true));Debug.Log("REAR_CAMERA_AUDIT "+(report.passed?"PASS":"FAIL")+" failedSamples="+report.failedSamples+" "+path);}
  if(Application.isBatchMode)EditorApplication.Exit(report.passed?0:2);
 }
}
}
