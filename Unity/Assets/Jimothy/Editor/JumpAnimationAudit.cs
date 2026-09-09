using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace Jimothy.Editor {
public static class JumpAnimationAudit {
 [Serializable] public class Report {public bool passed,phaseConfigured,nonLooping,posesDiffer,phaseHolds;public float takeoffToApex,apexToLanding,heldPoseDrift;public int vertices;public string error,method;}
 static Vector3[] Capture(Animator animator,float phase){animator.SetFloat("JumpPhase",phase);animator.Play("Jump",0,0);animator.Update(.001f);return Vertices(animator);}
 static Vector3[] Vertices(Animator animator){var renderer=animator.GetComponentsInChildren<SkinnedMeshRenderer>().OrderByDescending(r=>r.sharedMesh.vertexCount).First();var mesh=new Mesh();renderer.BakeMesh(mesh);var vertices=mesh.vertices;UnityEngine.Object.DestroyImmediate(mesh);return vertices;}
 static float Difference(Vector3[] a,Vector3[] b){if(a.Length!=b.Length)throw new InvalidOperationException("Mesh count changed");float max=0;for(int i=0;i<a.Length;i++){if(float.IsNaN(b[i].x)||float.IsInfinity(b[i].x))throw new InvalidOperationException("Nonfinite skin vertex");max=Mathf.Max(max,Vector3.Distance(a[i],b[i]));}return max;}
 [MenuItem("Jimothy/Audit phase driven jump animation")]
 public static void Run(){
  var report=new Report{method="Actual imported FBX, AnimatorController and Animator.Update sampled into baked skinned vertices. Tests phase-dependent poses and held-phase stability; no GameSession/save access or physics claim."};
  try{
   EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
   var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Jimothy/Resources/Jimothy.fbx");if(!prefab)throw new InvalidOperationException("Missing Jimothy FBX");
   var model=UnityEngine.Object.Instantiate(prefab);var animator=model.GetComponent<Animator>();if(!animator)animator=model.AddComponent<Animator>();
   var controller=AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/Jimothy/Resources/JimothyMotion.controller");animator.runtimeAnimatorController=controller;animator.cullingMode=AnimatorCullingMode.AlwaysAnimate;animator.applyRootMotion=false;
   var state=controller.layers[0].stateMachine.states.Select(s=>s.state).First(s=>s.name=="Jump");
   report.phaseConfigured=state.timeParameterActive&&state.timeParameter=="JumpPhase"&&controller.parameters.Any(p=>p.name=="JumpPhase"&&p.type==AnimatorControllerParameterType.Float);
   report.nonLooping=state.motion is AnimationClip clip&&!clip.isLooping;
   animator.Rebind();animator.Update(0);var takeoff=Capture(animator,0);var apex=Capture(animator,.48f);var landing=Capture(animator,.84f);report.vertices=apex.Length;
   report.takeoffToApex=Difference(takeoff,apex);report.apexToLanding=Difference(apex,landing);report.posesDiffer=report.takeoffToApex>.015f&&report.apexToLanding>.015f;
   var held=Capture(animator,.48f);for(int i=0;i<120;i++)animator.Update(1f/60f);report.heldPoseDrift=Difference(held,Vertices(animator));report.phaseHolds=report.heldPoseDrift<.0001f;
   report.passed=report.phaseConfigured&&report.nonLooping&&report.posesDiffer&&report.phaseHolds;
  }catch(Exception e){report.error=e.ToString();Debug.LogException(e);}
  string path=Path.GetFullPath(Path.Combine(Application.dataPath,"../Logs/jump-animation-audit.json"));Directory.CreateDirectory(Path.GetDirectoryName(path));File.WriteAllText(path,JsonUtility.ToJson(report,true));Debug.Log("JUMP_ANIMATION_AUDIT "+(report.passed?"PASS":"FAIL")+" "+path);if(Application.isBatchMode)EditorApplication.Exit(report.passed?0:2);
 }
}
}
