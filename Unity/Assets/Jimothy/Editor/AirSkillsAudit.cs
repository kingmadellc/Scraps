using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace Jimothy.Editor {
public static class AirSkillsAudit {
 const float Dt=1f/60f;static RaccoonMotor motor;
 [Serializable] class Check {public string name;public bool passed;public string evidence;}
 [Serializable] class Report {public bool passed;public string method="Actual CharacterController fixed-step input fixtures in unsaved scene; no GameSession or player save access.";public List<Check> checks=new();}
 static void Tick(Vector3 input,bool jump=false){Physics.SyncTransforms();motor.SimulateMovement(input,jump,Dt);Physics.Simulate(Dt);}
 static GameObject Block(string name,Vector3 p,Vector3 size){var go=new GameObject(name);go.transform.position=p;go.AddComponent<BoxCollider>().size=size;return go;}
 static void Place(Vector3 p,bool settle=true){motor.Teleport(p);if(settle)for(int i=0;i<30;i++)Tick(Vector3.zero);}
 public static void Run(){var report=new Report();var old=Physics.simulationMode;var routes=new List<Vector3[]>(ClosingTimeWorld.RoofRoutes);EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);Physics.simulationMode=SimulationMode.Script;
 void Check(string name,bool okay,string evidence){report.checks.Add(new Check{name=name,passed=okay,evidence=evidence});if(!okay)Debug.LogError("AIR_SKILLS_FAIL "+name+" "+evidence);}
 try{
 var player=new GameObject("Motor skills test");motor=player.AddComponent<RaccoonMotor>();motor.Initialize(null);motor.Running=true;Block("Ground",new Vector3(0,-.25f,0),new Vector3(100,.5f,100));Place(new Vector3(-20,.04f,0));
 int count=motor.CompletedTricks;motor.RequestTrick();Tick(Vector3.zero);Check("Ground trick is no-op",motor.CompletedTricks==count&&!motor.IsTricking,"completed="+motor.CompletedTricks);
 Tick(Vector3.zero,true);for(int i=0;i<5;i++)Tick(Vector3.zero);motor.RequestTrick();for(int i=0;i<100;i++)Tick(Vector3.zero);Check("Intentional full jump flip awards only after clean landing",motor.CompletedTricks==count+1&&motor.LastLandingPoints>=150&&motor.LastLandingClean&&motor.LastFallDamage==0,"tricks="+motor.CompletedTricks+" points="+motor.LastLandingPoints+" damage="+motor.LastFallDamage);
 Tick(Vector3.zero,true);for(int i=0;i<5;i++)Tick(Vector3.zero);motor.RequestTrick();for(int i=0;i<100;i++)Tick(Vector3.zero);Check("Repeated stationary practice flip cannot farm combo",motor.LastLandingPoints==150,"points="+motor.LastLandingPoints);
 Tick(Vector3.zero,true);for(int i=0;i<36;i++)Tick(Vector3.zero);motor.RequestTrick();for(int i=0;i<80;i++)Tick(Vector3.zero);Check("Late incomplete flip earns no landing points",motor.LastLandingPoints==0,"points="+motor.LastLandingPoints);
 Place(new Vector3(-15,9,0),false);for(int i=0;i<180;i++)Tick(Vector3.zero);Check("Unbroken large fall causes significant damage",motor.LastFallHeight>8.7f&&motor.LastFallDamage>70,"height="+motor.LastFallHeight+" damage="+motor.LastFallDamage);
 var wall=Block("Wall brush must not reset fall",new Vector3(-9.5f,5,0),new Vector3(.25f,12,5));Place(new Vector3(-10,9,0),false);for(int i=0;i<180;i++)Tick(Vector3.right);Check("Wall contact does not reset fall accumulation",motor.LastFallHeight>8.7f&&motor.LastFallDamage>70,"height="+motor.LastFallHeight+" damage="+motor.LastFallDamage);
 var shelf=Block("Supported intermediate landing",new Vector3(15,4.75f,0),new Vector3(2,.5f,3));Place(new Vector3(15,9,0),false);for(int i=0;i<100;i++)Tick(Vector3.zero);float first=motor.LastFallHeight;for(int i=0;i<200;i++)Tick(Vector3.right);Check("Supported landing breaks fall into two descents",first<4.2f&&first>3.7f&&motor.LastFallHeight<5.2f&&motor.LastFallHeight>4.7f,"first="+first+" second="+motor.LastFallHeight);
 Block("Roof takeoff",new Vector3(-30,5.75f,20),new Vector3(2,.5f,4));Block("Roof across real gap",new Vector3(-26,5.75f,20),new Vector3(2,.5f,4));Place(new Vector3(-29.25f,6.04f,20));Tick(Vector3.right,true);for(int i=0;i<60;i++){Tick(motor.transform.position.x< -26.65f?Vector3.right:Vector3.zero);}Check("Real rooftop gap transfer rewards without flip",motor.LastLandingPoints>=100&&motor.LastLandingClean&&motor.transform.position.y>5.8f,"points="+motor.LastLandingPoints+" position="+motor.transform.position);
 ClosingTimeWorld.RoofRoutes.Clear();var path=new Vector3[4];for(int i=0;i<4;i++){float h=(i+1)*.9f;path[i]=new Vector3(2+i*1.03f,h+.04f,15);Block("Authored step "+i,new Vector3(path[i].x,h*.5f,15),new Vector3(1.5f,h,1.5f));}ClosingTimeWorld.RoofRoutes.Add(path);Place(new Vector3(.5f,.04f,15));for(int i=0;i<90;i++)Tick(Vector3.right);Check("First access still needs jump",motor.transform.position.y<.2f&&motor.CompletedMantles==0,"position="+motor.transform.position);
 Tick(Vector3.right,true);for(int i=0;i<240;i++){Vector3 delta=path[3]-motor.transform.position;delta.y=0;Tick(delta.magnitude>.15f?delta.normalized:Vector3.zero);}Check("After first jump successive stairs need no repeat jumping",motor.transform.position.y>3.5f&&motor.CompletedMantles>=2,"position="+motor.transform.position+" mantles="+motor.CompletedMantles+" reason="+motor.LastMantleBlockReason+" route="+typeof(RaccoonMotor).GetField("stairRoute",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).GetValue(motor)+" step="+typeof(RaccoonMotor).GetField("stairStep",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).GetValue(motor));
 var flipCamera=new GameObject("Independent flip camera").AddComponent<Camera>();var flipObject=new GameObject("Rendered-heading flip fixture");var flipMotor=flipObject.AddComponent<RaccoonMotor>();flipMotor.visual=new GameObject("Stable heading visual").transform;flipMotor.visual.SetParent(flipObject.transform,false);flipMotor.Initialize(flipCamera);flipMotor.Running=true;bool rearOkay=true,inverted=false;int cameraSamples=0;
 foreach(float heading in new[]{0f,90f,180f,270f}){flipMotor.Teleport(new Vector3(0,.07f,-15));flipMotor.FaceDirection(heading);for(int i=0;i<30;i++){flipMotor.SimulateSteering(Vector2.zero,Vector2.zero,false,Dt);Physics.Simulate(Dt);}for(int i=0;i<75;i++){if(i==5)flipMotor.RequestTrick();flipMotor.SimulateSteering(Vector2.up,Vector2.zero,i==0,Dt);Physics.Simulate(Dt);flipMotor.UpdateFollowCamera(Dt);Vector3 backwards=Vector3.ProjectOnPlane(flipCamera.transform.position-flipMotor.transform.position,Vector3.up).normalized;float dot=Vector3.Dot(backwards,Quaternion.Euler(0,heading,0)*Vector3.forward);rearOkay&=dot<-.99f;inverted|=Vector3.Dot(flipMotor.visual.up,Vector3.up)<-.9f;cameraSamples++;}}
 Check("Somersault rotates visual while camera stays behind all four headings",rearOkay&&inverted,"rear="+rearOkay+" invertedVisual="+inverted+" samples="+cameraSamples);

 }catch(Exception e){Debug.LogException(e);report.checks.Add(new Check{name="exception",passed=false,evidence=e.ToString()});}
 finally{ClosingTimeWorld.RoofRoutes.Clear();ClosingTimeWorld.RoofRoutes.AddRange(routes);Physics.simulationMode=old;report.passed=report.checks.Count>=11&&report.checks.TrueForAll(c=>c.passed);string path=Path.GetFullPath("../PlaytestCaptures/air-skills-audit.json");Directory.CreateDirectory(Path.GetDirectoryName(path));File.WriteAllText(path,JsonUtility.ToJson(report,true));Debug.Log("AIR_SKILLS_AUDIT "+report.passed);if(Application.isBatchMode)EditorApplication.Exit(report.passed?0:1);}
 }
}
}
