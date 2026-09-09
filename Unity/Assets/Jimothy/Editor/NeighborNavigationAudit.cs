using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace Jimothy.Editor {
[InitializeOnLoad] public static class NeighborNavigationAudit {
 const string Key="Jimothy.NeighborNavigationAudit";
 static NeighborNavigationAudit(){EditorApplication.playModeStateChanged+=Changed;}
 public static void Run(){if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop play mode first.");EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);SessionState.SetBool(Key,true);EditorApplication.isPlaying=true;}
 static void Changed(PlayModeStateChange state){if(state==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool(Key,false))new GameObject("Isolated neighbor navigation audit").AddComponent<NeighborNavigationAuditRunner>();}
 public static void Finish(bool passed,string json){var folder=Path.GetFullPath("../PlaytestCaptures");Directory.CreateDirectory(folder);File.WriteAllText(Path.Combine(folder,"neighbor-navigation-audit.json"),json);SessionState.SetBool(Key,false);EditorApplication.isPlaying=false;if(Application.isBatchMode)EditorApplication.Exit(passed?0:1);}
}
public class NeighborNavigationAuditRunner:MonoBehaviour {
 [Serializable] class Result {public string test;public bool passed;public int frames;public float traveled,simulatedSeconds;public bool planned,pathRemaining;public Vector3 end;}
 [Serializable] class Report {public string method="Actual CharacterController.Move through NeighborNavigation.Step with explicit 1/60s input steps in isolated play mode. No GameSession/save data. Synthetic fixtures, not whole-district coverage.";public List<Result> checks=new();}
 readonly Report report=new();GameObject fixture,actor;NeighborNavigation nav;Vector3 target,start,last;int stage=-1,frames;float traveled;bool planned;
 void Box(Vector3 p,Vector3 size){var g=new GameObject("Audit static obstruction");g.transform.SetParent(fixture.transform);g.transform.position=p;g.AddComponent<BoxCollider>().size=size;}
 void Setup(){stage++;frames=0;traveled=0;fixture=new GameObject("Navigation fixture "+stage);Box(new(0,-.25f,0),new(20,.5f,20));start=new(-4,.04f,0);target=new(4,0,0);
  if(stage==0)Box(new(0,1.5f,0),new(2,3,6));
  if(stage==1){start=new(0,.04f,-4);target=new(0,0,4);Box(new(-1,1.5f,0),new(.25f,3,12));Box(new(1,1.5f,0),new(.25f,3,12));}
  if(stage==2)Box(new(4,1.5f,0),new(5,3,5));
  if(stage>=3){Box(new(5,.10f,0),new(10,.20f,20));if(stage==4){start=new(4,.24f,0);target=new(-4,0,0);}else target=new(4,.20f,0);}
  actor=new GameObject("Actual audit controller");actor.transform.position=start;actor.transform.SetParent(fixture.transform);var cc=actor.AddComponent<CharacterController>();cc.radius=.26f;cc.height=1.78f;cc.center=Vector3.up*.89f;cc.stepOffset=.2f;nav=actor.AddComponent<NeighborNavigation>();nav.Initialize(cc);Physics.SyncTransforms();planned=nav.Plan(target);last=start;
 }
 void Update(){if(stage<0){Setup();return;}frames++;nav.Step(target,3,1f/60);traveled+=Vector3.Distance(actor.transform.position,last);last=actor.transform.position;bool arrived=Vector2.Distance(new(last.x,last.z),new(target.x,target.z))<.8f;
  if((stage!=2&&arrived)||frames>=600||(stage==2&&frames>=90)){bool passed=stage!=2?arrived&&planned:!planned&&Vector2.Distance(new(last.x,last.z),new(start.x,start.z))<.1f;report.checks.Add(new Result{test=stage==0?"Route around solid wall corner":stage==1?"Traverse 1.75m clear corridor":stage==2?"Unreachable enclosed target stays stationary":stage==3?"Ascend 0.20m curb with actual CC":"Descend 0.20m curb with actual CC",passed=passed,frames=frames,traveled=traveled,simulatedSeconds=frames/60f,planned=planned,pathRemaining=nav.HasPath,end=last});DestroyImmediate(fixture);if(stage==4){bool all=report.checks.TrueForAll(r=>r.passed);NeighborNavigationAudit.Finish(all,JsonUtility.ToJson(report,true));Destroy(this);}else Setup();}
 }
}
}
