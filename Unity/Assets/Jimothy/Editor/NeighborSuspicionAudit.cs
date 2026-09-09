using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
namespace Jimothy.Editor {
public static class NeighborSuspicionAudit {
 [Serializable] class Check {public string name;public bool passed;public float value;}
 [Serializable] class Report {public string method="Isolated physical ground/wall fixtures, actual NeighborAI perception and CharacterController navigation, imported dog/human rigs. No GameSession or save access.";public List<Check> checks=new();}
 static Report report;
 static void CheckThat(string name,bool passed,float value=0){report.checks.Add(new Check{name=name,passed=passed,value=value});}
 static GameObject Box(string name,Vector3 p,Vector3 size){var g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=name;g.transform.position=p;g.transform.localScale=size;return g;}
 public static void Run(){
  report=new Report();try{
   EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
   Box("Ground",new(0,-.25f,0),new(40,.5f,40));Physics.SyncTransforms();
   foreach(var species in new[]{NeighborKind.Dog,NeighborKind.AngryHuman})TestSpecies(species);
  }catch(Exception e){CheckThat(e.ToString(),false);}
  var path=Path.GetFullPath("../Logs/neighbor-suspicion-audit.json");Directory.CreateDirectory(Path.GetDirectoryName(path));File.WriteAllText(path,JsonUtility.ToJson(report,true));bool pass=report.checks.TrueForAll(c=>c.passed);Debug.Log("Neighbor suspicion audit: "+report.checks.Count+" checks; passed="+pass);if(Application.isBatchMode)EditorApplication.Exit(pass?0:1);
 }
 static void Capture(GameObject actor,string name){
  var cameraObject=new GameObject("Audit evidence camera");var cam=cameraObject.AddComponent<Camera>();cam.transform.position=actor.transform.position+new Vector3(-2.4f,1.8f,3.2f);cam.transform.LookAt(actor.transform.position+Vector3.up*.85f);cam.fieldOfView=38;cam.clearFlags=CameraClearFlags.SolidColor;cam.backgroundColor=new Color(.10f,.12f,.14f);
  var lamp=new GameObject("Evidence soft key");var light=lamp.AddComponent<Light>();light.type=LightType.Directional;light.intensity=2.2f;lamp.transform.rotation=Quaternion.Euler(35,-30,0);RenderSettings.ambientMode=AmbientMode.Flat;RenderSettings.ambientLight=new Color(.4f,.4f,.4f);
  var rt=new RenderTexture(960,720,24);rt.Create();cam.targetTexture=rt;RenderPipeline.SubmitRenderRequest(cam,new RenderPipeline.StandardRequest{destination=rt});var old=RenderTexture.active;RenderTexture.active=rt;var image=new Texture2D(960,720,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,960,720),0,0);image.Apply();var folder=Path.GetFullPath("../PlaytestCaptures/neighbor-suspicion");Directory.CreateDirectory(folder);File.WriteAllBytes(Path.Combine(folder,name+".png"),image.EncodeToPNG());RenderTexture.active=old;cam.targetTexture=null;rt.Release();UnityEngine.Object.DestroyImmediate(rt);UnityEngine.Object.DestroyImmediate(image);UnityEngine.Object.DestroyImmediate(cameraObject);UnityEngine.Object.DestroyImmediate(lamp);
 }

 static void TestSpecies(NeighborKind kind){
  var go=new GameObject("Audit "+kind);go.transform.position=new(0,.04f,0);var ai=go.AddComponent<NeighborAI>();ai.Initialize(kind);var nav=go.GetComponent<NeighborNavigation>();Physics.SyncTransforms();Vector3 ground=new(0,.04f,2);
  void Sense(Vector3 p,int frames,bool safe=false){for(int i=0;i<frames;i++)ai.SenseTarget(p,3,safe,1f/60);}
  Sense(ground,1);CheckThat(kind+" visible does not instantly chase",ai.HasLineOfSight&&!ai.IsThreat&&ai.Suspicion01>0&&ai.Suspicion01<.1f,ai.Suspicion01);
  Sense(ground,20);CheckThat(kind+" brief noticing stays below investigation threshold",ai.Suspicion01>.06f&&ai.Suspicion01<.55f&&!ai.IsInvestigating&&!ai.IsThreat,ai.Suspicion01);
  Sense(ground+Vector3.up,30);CheckThat(kind+" ordinary 1m jump remains visible",ai.HasLineOfSight&&ai.Suspicion01>.1f,ai.Suspicion01);
  Sense(ground,20);CheckThat(kind+" sustained noticing begins investigation before chase",ai.IsInvestigating&&!ai.IsThreat,ai.Suspicion01);
  Sense(ground,240);CheckThat(kind+" sustained exposure reaches chase",ai.IsThreat&&ai.Suspicion01>.99f,ai.Suspicion01);
  CheckThat(kind+" remembered chase destination remains ground",nav.IsGroundTarget(ai.LastSeenPosition),ai.LastSeenPosition.y);
  var wall=Box("Opaque wall",new(0,1.5f,1),new(4,3,.25f));Physics.SyncTransforms();var seen=ai.LastSeenPosition;Sense(new(.2f,.04f,2),1);CheckThat(kind+" wall occludes sight without instant memory reset",!ai.HasLineOfSight&&ai.Suspicion01>.9f&&ai.LastSeenPosition==seen,ai.Suspicion01);
  Sense(ground,200);CheckThat(kind+" hidden target ends chase and decays",!ai.IsThreat&&ai.Suspicion01<.6f,ai.Suspicion01);
  UnityEngine.Object.DestroyImmediate(wall);Physics.SyncTransforms();Sense(ground,240);float before=ai.Suspicion01;Sense(new(0,.04f,35),1);CheckThat(kind+" leaving 28m decays rather than resets",ai.Suspicion01>before-.02f&&!ai.HasLineOfSight,ai.Suspicion01);
  Sense(new(-14,-2.35f,59),1,true);CheckThat(kind+" Den safety ends threat but preserves smooth decay",!ai.IsThreat&&!ai.HasLineOfSight&&ai.Suspicion01>.8f,ai.Suspicion01);
  Sense(ground,240);Sense(new(0,4,2),200);CheckThat(kind+" roof loses contact and pursuit",!ai.HasLineOfSight&&!ai.IsThreat,ai.Suspicion01);
  bool plan=nav.Plan(new(0,0,6));CheckThat(kind+" valid street route exists",plan&&nav.HasPath);
  Vector3 start=go.transform.position;for(int i=0;i<120;i++)nav.Step(new(0,1.0f,6),3,1f/60);CheckThat(kind+" raised target clears stale path and cannot climb",nav.TargetUnreachable&&!nav.HasPath&&Vector2.Distance(new(start.x,start.z),new(go.transform.position.x,go.transform.position.z))<.02f&&go.transform.position.y<.2f,go.transform.position.y);
  CheckThat(kind+" basement target rejected",!nav.Plan(new(0,-2.4f,6)));
  go.transform.rotation=Quaternion.Euler(0,180,0);Physics.SyncTransforms();Vector3 first=nav.Step(new(0,0,6),3,1f/60,false,95);CheckThat(kind+" reverses by pivoting before translating",first.magnitude<.001f,first.magnitude);
  float worstAngle=0;for(int i=0;i<300;i++){var moved=nav.Step(new(0,0,6),3,1f/60,false,95);if(moved.magnitude>.005f)worstAngle=Mathf.Max(worstAngle,Vector3.Angle(go.transform.forward,moved));}
  CheckThat(kind+" travel follows facing through turn",worstAngle<56,worstAngle);CheckThat(kind+" turn then accelerates to destination",Vector2.Distance(new(go.transform.position.x,go.transform.position.z),new(0,6))<.15f);
  // Place a measured wall beside the actual imported rig, then evaluate additive joints.
  var support=Box("Supported rest wall",go.transform.position+go.transform.right*.62f+Vector3.up*1.4f,new(.20f,2.8f,2));Physics.SyncTransforms();bool found=nav.TryRestSurface(out var hit);CheckThat(kind+" wall support is physically measured",found);
  var language=go.GetComponent<NeighborBodyLanguage>();var anim=go.GetComponentInChildren<Animator>();anim.cullingMode=AnimatorCullingMode.AlwaysAnimate;anim.Rebind();anim.Play("Idle");anim.Update(0);var root=go.transform.position;var feet=new Dictionary<Transform,Vector3>();foreach(var t in go.GetComponentsInChildren<Transform>())if(t.name.StartsWith("foot_"))feet[t]=t.position;
  language.Set(true,found,hit,false);for(int i=0;i<60;i++){language.ResetPose();anim.Update(0);language.SamplePose(1f/60,i/60f);}
  float footMotion=0;foreach(var f in feet)footMotion=Mathf.Max(footMotion,Vector3.Distance(f.Value,f.Key.position));CheckThat(kind+" rest pose keeps root and foot joints planted",root==go.transform.position&&footMotion<.001f,footMotion);
  if(kind==NeighborKind.AngryHuman){float reach=float.MaxValue;foreach(var t in go.GetComponentsInChildren<Transform>())if(t.name=="hand_L"||t.name=="hand_R")reach=Mathf.Min(reach,Vector3.Distance(t.position,hit.point+hit.normal*.085f));CheckThat(kind+" supported human hand reaches wall",reach<.025f,reach);}
  Capture(go,kind.ToString()+"-supported-rest");UnityEngine.Object.DestroyImmediate(support);
  UnityEngine.Object.DestroyImmediate(go);
 }
}
}
