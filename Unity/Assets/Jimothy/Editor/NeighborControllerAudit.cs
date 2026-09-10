using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace Jimothy.Editor {
[InitializeOnLoad] public static class NeighborControllerAudit {
 const string Key="Jimothy.NeighborControllerAudit";
 static NeighborControllerAudit(){EditorApplication.playModeStateChanged+=s=>{if(s==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool(Key,false))new GameObject("Scaled controller audit").AddComponent<NeighborControllerAuditRunner>();};}
 public static void Run(){EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);SessionState.SetBool(Key,true);EditorApplication.isPlaying=true;}
 public static void Finish(bool pass,string json){var p=Path.GetFullPath("../PlaytestCaptures/neighbor-controller-audit.json");Directory.CreateDirectory(Path.GetDirectoryName(p));File.WriteAllText(p,json);SessionState.SetBool(Key,false);EditorApplication.isPlaying=false;if(Application.isBatchMode)EditorApplication.Exit(pass?0:1);}
}
public class NeighborControllerAuditRunner:MonoBehaviour {
 [Serializable] class Check {public string name;public bool passed;public string evidence;}
 [Serializable] class Report {public bool passed;public string method="Play mode: actual NeighborAI.Initialize at production species scale, CharacterController ground/curb movement, capture/restore and deactivate/renew. No loaded player saves. Editor physics results require separate browser confirmation.";public List<Check> checks=new();public List<string> engineErrors=new();}
 readonly Report report=new();
 void Log(string condition,string stack,LogType type){if(type==LogType.Error||type==LogType.Exception||type==LogType.Assert)report.engineErrors.Add(condition);}
 void CheckThat(string name,bool pass,string evidence=""){report.checks.Add(new Check{name=name,passed=pass,evidence=evidence});}
 void Box(Vector3 p,Vector3 size){var g=new GameObject("Curb audit solid");g.transform.position=p;g.AddComponent<BoxCollider>().size=size;}
 NeighborAI Create(NeighborKind kind,Vector3 at){var go=new GameObject("Production initialized "+kind);go.transform.position=at;var n=go.AddComponent<NeighborAI>();n.Initialize(kind);return n;}
 void Start(){Application.logMessageReceived+=Log;var old=Physics.simulationMode;Physics.simulationMode=SimulationMode.Script;
 try{
  var session=new GameObject("Suppressed audit session").AddComponent<GameSession>();session.SuppressSaving=true;typeof(GameSession).GetField("world",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).SetValue(session,new GameObject("Isolated primitive owner"));
  Box(new(0,-.25f,0),new(40,.5f,40));Box(new(5,.10f,0),new(10,.20f,20));Physics.SyncTransforms();
  foreach(NeighborKind kind in Enum.GetValues(typeof(NeighborKind))){
   var n=Create(kind,new(-4,.04f,0));var cc=n.GetComponent<CharacterController>();float scale=n.transform.lossyScale.y;float limit=cc.height*scale;
   CheckThat(kind+" production scaled capsule accepts world step",cc.enabled&&cc.stepOffset<=limit&&Mathf.Abs(cc.stepOffset-.22f)<.001f,$"scale={scale} step={cc.stepOffset} effectiveHeight={limit}");
   var nav=n.GetComponent<NeighborNavigation>();var before=n.transform.position;for(int i=0;i<30;i++){nav.Step(new(-2,0,0),1,1f/60);Physics.Simulate(1f/60);}
   CheckThat(kind+" live controller actually moves",Vector3.Distance(before,n.transform.position)>.25f);
   var state=JsonUtility.FromJson<NeighborSaveState>(JsonUtility.ToJson(n.CaptureState()));CheckThat(kind+" persistent restore accepted",n.RestoreState(state));
   n.gameObject.SetActive(false);before=n.transform.position;nav.Step(new(4,.2f,0),3,1f/60);CheckThat(kind+" inactive controller safely ignores movement",n.transform.position==before);
   var renewed=Create(kind,state.position);CheckThat(kind+" replacement restores snapshot",renewed.RestoreState(state));DestroyImmediate(n.gameObject);n=renewed;nav=n.GetComponent<NeighborNavigation>();
   if(kind==NeighborKind.Cat||kind==NeighborKind.Gull){
    bool planned=nav.Plan(new(4,.2f,0));for(int i=0;i<600;i++){nav.Step(new(4,.2f,0),3,1f/60);Physics.Simulate(1f/60);}
    CheckThat(kind+" ascends .20m curb",planned&&n.transform.position.x>3.5f&&n.transform.position.y>.17f,n.transform.position.ToString());
    planned=nav.Plan(new(-4,0,0));for(int i=0;i<600;i++){nav.Step(new(-4,0,0),3,1f/60);Physics.Simulate(1f/60);}
    CheckThat(kind+" descends .20m curb",planned&&n.transform.position.x< -3.5f&&n.transform.position.y<.1f,n.transform.position.ToString());
   }
   DestroyImmediate(n.gameObject);
  }
 }catch(Exception e){report.engineErrors.Add(e.ToString());}
 finally{Physics.simulationMode=old;Application.logMessageReceived-=Log;CheckThat("No engine errors during initialization/movement/renewal",report.engineErrors.Count==0);report.passed=report.checks.Count>=50&&report.checks.TrueForAll(c=>c.passed);NeighborControllerAudit.Finish(report.passed,JsonUtility.ToJson(report,true));}
 }
}
}
