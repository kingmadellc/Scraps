using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace Jimothy.Editor {
public static class DenGeometryAudit {
 [Serializable] public class Check {public string name;public bool passed;public Vector3 position;}
 [Serializable] public class Report {public bool passed;public string error;public List<Check> checks=new();public string method="Fresh unsaved district, actual RaccoonMotor.SimulateMovement and CharacterController at60Hz. Continuous street-to-room-to-street movement with no jumps or teleports after initial spawn. Shelf support raycasts. No GameSession or save reads/writes.";}
 public static void Run(){var report=new Report();var old=Physics.simulationMode;try{
  EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);Physics.simulationMode=SimulationMode.Script;ClosingTimeWorld.Build(null);NeighborWorldObstacles.Ensure();RooftopConnections.Build(null);
  var go=new GameObject("Den audit player");go.AddComponent<CharacterController>();var motor=go.AddComponent<RaccoonMotor>();motor.Initialize(null);motor.Running=false;motor.Teleport(ClosingTimeWorld.StreetStart);
  void Tick(Vector3 input){Physics.SyncTransforms();motor.SimulateMovement(input,false,1f/60);Physics.Simulate(1f/60);}
  for(int i=0;i<45;i++)Tick(Vector3.zero);
  var points=new[]{new Vector3(-10,.05f,44),ClosingTimeWorld.DenEntrance,new Vector3(-14,-1.95f,55),ClosingTimeWorld.DenInterior,new Vector3(-10,-2.35f,59),new Vector3(-18.3f,-2.35f,59),ClosingTimeWorld.DenInterior,new Vector3(-14,-1.95f,55),ClosingTimeWorld.DenEntrance,ClosingTimeWorld.StreetStart};
  for(int i=0;i<points.Length;i++){bool reached=false;for(int f=0;f<600;f++){var delta=points[i]-go.transform.position;delta.y=0;Tick(delta.magnitude>.06f?delta.normalized*Mathf.Clamp01(delta.magnitude/.35f):Vector3.zero);if(delta.magnitude<.16f&&Mathf.Abs(go.transform.position.y-points[i].y)<.20f&&motor.Grounded){reached=true;break;}}report.checks.Add(new Check{name="Continuous waypoint "+i,passed=reached,position=go.transform.position});if(!reached)break;}
  foreach(var slot in ClosingTimeWorld.DenDisplaySlots){bool supported=Physics.Raycast(slot+Vector3.up*.05f,Vector3.down,out var hit,.12f,1,QueryTriggerInteraction.Ignore)&&Mathf.Abs(hit.point.y-slot.y)<.015f;report.checks.Add(new Check{name="Display support",passed=supported,position=slot});}
  report.passed=report.checks.Count==42&&report.checks.TrueForAll(x=>x.passed);
 }catch(Exception e){report.error=e.ToString();Debug.LogException(e);}finally{Physics.simulationMode=old;string path=Path.GetFullPath("../Logs/den-geometry-audit.json");File.WriteAllText(path,JsonUtility.ToJson(report,true));Debug.Log("DEN_GEOMETRY_AUDIT "+(report.passed?"PASS":"FAIL"));}if(Application.isBatchMode)EditorApplication.Exit(report.passed?0:2);}
}}
