using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace Jimothy.Editor {
/// <summary>Physical district regression. Builds an unsaved isolated scene, never loads user progress.</summary>
public static class DistrictGeometryAudit {
 [Serializable] public class Walk {public string name;public bool reached;public int failedWaypoint=-1,frames;public Vector3 start,actual,target;public float minimumY=100,maxPenetration;public string obstacle;}
 [Serializable] public class Report {public string utc,method,error;public bool passed,roadSupported,bellClear,bendsLeft;public int roadSamples;public float renderedBendEndpointX;public List<string> problems=new();public List<Walk> walks=new();}
 const float Dt=1f/60f;static RaccoonMotor motor;static CharacterController cc;static CapsuleCollider probe;static Report report;
 static void Tick(Vector3 direction){Physics.SyncTransforms();motor.SimulateMovement(direction,false,Dt);Physics.Simulate(Dt);}
 static void Settle(Vector3 p){motor.Teleport(p);Physics.SyncTransforms();for(int i=0;i<45;i++)Tick(Vector3.zero);}
 static Walk Follow(string name,Vector3 start,IList<Vector3> points){
  var result=new Walk{name=name,start=start};report.walks.Add(result);Settle(start);
  for(int point=0;point<points.Count;point++){
   Vector3 target=points[point];result.target=target;bool reached=false;int budget=Mathf.CeilToInt(Vector3.Distance(target,motor.transform.position)/RaccoonMotor.MoveSpeed/Dt)+180;
   for(int frame=0;frame<budget;frame++){
    Vector3 delta=target-motor.transform.position;delta.y=0;Tick(delta.magnitude>.06f?delta.normalized*Mathf.Clamp01(delta.magnitude/.25f):Vector3.zero);result.frames++;result.minimumY=Mathf.Min(result.minimumY,motor.transform.position.y);
    // Check actual capsule penetration, including unnamed/hidden blockers; floor skin contact is tolerated.
    Vector3 center=motor.transform.position+cc.center;float half=cc.height*.5f-cc.radius;
    foreach(var hit in Physics.OverlapCapsule(center+Vector3.up*half,center-Vector3.up*half,cc.radius,~(1<<2),QueryTriggerInteraction.Ignore)){
     if(hit==cc||hit==probe)continue;
     if(Physics.ComputePenetration(probe,motor.transform.position,motor.transform.rotation,hit,hit.transform.position,hit.transform.rotation,out _,out float depth)&&depth>result.maxPenetration){result.maxPenetration=depth;result.obstacle=hit.name;}
    }
    Vector3 remaining=target-motor.transform.position;remaining.y=0;
    if(remaining.magnitude<.14f&&motor.Grounded&&Mathf.Abs(motor.transform.position.y-target.y)<.13f){reached=true;break;}
    if(motor.transform.position.y<-2)break;
   }
   if(!reached){result.failedWaypoint=point;break;}
  }
  result.actual=motor.transform.position;result.reached=result.failedWaypoint<0&&result.maxPenetration<=cc.skinWidth+.025f;return result;
 }
 [MenuItem("Scraps/Audit district walking and bell clearance")]
 public static void Run(){
  if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play Mode before the isolated district geometry audit.");
  EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);var old=Physics.simulationMode;Physics.simulationMode=SimulationMode.Script;
  report=new Report{utc=DateTime.UtcNow.ToString("O"),method="Actual RaccoonMotor/CharacterController movement at 60Hz, no jumps or traversal teleports. Teleport only resets independent fixture starts. Ground raycasts and capsule penetration/clearance queries supplement walks."};
  try {
   ClosingTimeWorld.Build(null);Physics.SyncTransforms();
   var player=new GameObject("District audit Jimothy");cc=player.AddComponent<CharacterController>();motor=player.AddComponent<RaccoonMotor>();motor.Initialize(null);motor.Running=false;
   var shape=new GameObject("District penetration probe");probe=shape.AddComponent<CapsuleCollider>();probe.enabled=false;probe.height=cc.height;probe.radius=cc.radius;probe.center=cc.center;
   // User-requested left turn from forward -Z: camera-left is world +X. Park is opposite at -8.
   Vector3 bendEnd=new(10,.06f,-68),park=new(-12,.26f,-52);float endpointSum=0;int endpointVertices=0;
   foreach(var filter in UnityEngine.Object.FindObjectsByType<MeshFilter>(FindObjectsSortMode.None)){
    var renderer=filter.GetComponent<MeshRenderer>();if(!renderer||!renderer.sharedMaterial||renderer.sharedMaterial.name!="streetBrick"||!filter.sharedMesh)continue;
    foreach(var local in filter.sharedMesh.vertices){var world=filter.transform.TransformPoint(local);if(world.z< -67){endpointSum+=world.x;endpointVertices++;}}
   }
   report.renderedBendEndpointX=endpointVertices>0?endpointSum/endpointVertices:0;
   report.bendsLeft=endpointVertices>0&&report.renderedBendEndpointX>2;
   if(!report.bendsLeft)report.problems.Add("Rendered brick bend does not extend camera-left (+X) from forward -Z.");
   report.roadSupported=true;
   for(int i=0;i<=60;i++){
    float t=i/60f;Vector3 center=new(10*t*t,0,-38-30*t),tangent=new(20*t,0,-30);Vector3 lateral=Vector3.Cross(Vector3.up,tangent).normalized;
    foreach(float lane in new[]{-4.5f,0,4.5f}){Vector3 sample=center+lateral*lane;report.roadSamples++;
     if(!Physics.Raycast(sample+Vector3.up,Vector3.down,out var ground,2,~(1<<2),QueryTriggerInteraction.Ignore)||ground.point.y<-.03f||ground.point.y>.09f){report.roadSupported=false;report.problems.Add("Road ground support outside [-.03,.09] at "+sample+" hit "+(ground.collider?ground.collider.name:"none"));}
    }
   }
   var bend=new List<Vector3>{new(-3,.06f,40),new(0,.06f,37),new(0,.06f,-38)};
   for(int i=1;i<=10;i++){float t=i/10f;bend.Add(new Vector3(10*t*t,.06f,-38-30*t));}
   Follow("Spawn to bend endpoint on foot",new(-3,.46f,44),bend);
   Follow("Bend endpoint back to avenue",bendEnd,new[]{new Vector3(8.1f,.06f,-65),new Vector3(4.9f,.06f,-59),new Vector3(2.5f,.06f,-53),new Vector3(.9f,.06f,-47),new Vector3(0,.06f,-38)});
   Follow("Avenue to garden and through bell",new(0,.06f,-38),new[]{new Vector3(1,.06f,-48),new Vector3(-5,.16f,-48),new Vector3(-12,.26f,-49),new Vector3(-12,.26f,-49),new Vector3(-12,.26f,-55)});
   Follow("Bell reverse passage",new(-12,.26f,-55),new[]{new Vector3(-12,.26f,-49)});
   // The expected walk-through lane includes floor support but must contain no blocking tower volume.
   report.bellClear=true;
   for(float z=-53.5f;z<=-50.5f;z+=.15f){Vector3 center=new Vector3(park.x,.26f,z)+cc.center;float half=cc.height*.5f-cc.radius;
    foreach(var obstacle in Physics.OverlapCapsule(center+Vector3.up*half,center-Vector3.up*half,cc.radius,~(1<<2),QueryTriggerInteraction.Ignore))if(obstacle!=cc&&obstacle!=probe){report.bellClear=false;report.problems.Add("Bell passage blocked by "+obstacle.name+" at z="+z);}
   }
   report.passed=report.roadSupported&&report.bellClear&&report.bendsLeft&&report.walks.TrueForAll(w=>w.reached);
  }catch(Exception ex){report.error=ex.ToString();report.passed=false;Debug.LogException(ex);}
  finally{Physics.simulationMode=old;var path=Path.GetFullPath("Logs/district-geometry-audit.json");Directory.CreateDirectory(Path.GetDirectoryName(path));File.WriteAllText(path,JsonUtility.ToJson(report,true));Debug.Log("DISTRICT_GEOMETRY_AUDIT "+(report.passed?"PASS":"FAIL")+" "+path);}
  if(Application.isBatchMode)EditorApplication.Exit(report.passed?0:2);
 }
}
}
