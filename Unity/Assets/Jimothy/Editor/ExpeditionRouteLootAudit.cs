using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace Jimothy.Editor {
[InitializeOnLoad] public static class ExpeditionRouteLootAudit {
 const string Pending="Jimothy.ExpeditionRouteLootAudit";static readonly int[] Seeds={107,108,109,110};static GameSession game;static int seed=-1,frame;static Report report;
 [Serializable] class Check {public int seed;public string route,item,error;public Vector3 actualNode,approach,end;public bool reached,searchable,returned;}
 [Serializable] class Report {public bool passed;public string error;public string method="Fresh production Begin with four explicit forage seeds, saving suppressed. Actual LootNode.SearchApproach and SearchRadius; actual motor walks the authored roof connector there and back. Initial roof-checkpoint placement only per case; main Den round trip is checked separately. No user saves read or written.";public List<Check> checks=new();}
 static ExpeditionRouteLootAudit(){EditorApplication.playModeStateChanged+=Changed;if(SessionState.GetBool(Pending,false))EditorApplication.update+=Tick;}
 public static void Run(){EditorSceneManager.OpenScene("Assets/Jimothy/Scenes/Ballard.unity");SessionState.SetBool(Pending,true);EditorApplication.isPlaying=true;}
 static void Changed(PlayModeStateChange mode){if(mode!=PlayModeStateChange.EnteredPlayMode||!SessionState.GetBool(Pending,false))return;seed=-1;report=new Report();EditorApplication.update-=Tick;EditorApplication.update+=Tick;}
 static void BeginSeed(){seed++;if(seed>=Seeds.Length){Finish();return;}game.SuppressSaving=true;typeof(GameSession).GetMethod("Begin",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(game,new object[]{new SaveData{forageSeed=Seeds[seed]}});frame=Time.frameCount;}
 static void Tick(){if(!SessionState.GetBool(Pending,false)||!EditorApplication.isPlaying)return;try{
  if(!game){game=UnityEngine.Object.FindFirstObjectByType<GameSession>();if(!game)return;BeginSeed();return;}if(Time.frameCount-frame<20)return;
  var old=Physics.simulationMode;Physics.simulationMode=SimulationMode.Script;try{
   game.Player.enabled=false;game.Player.Running=false;foreach(var neighbor in UnityEngine.Object.FindObjectsByType<NeighborAI>(FindObjectsSortMode.None))neighbor.enabled=false;
   foreach(var route in ClosingTimeWorld.DenRoundTrips){var check=new Check{seed=Seeds[seed],route=route.Id};report.checks.Add(check);var node=UnityEngine.Object.FindObjectsByType<LootNode>(FindObjectsSortMode.None).Single(n=>n.name==route.CacheId);check.actualNode=node.transform.position;check.approach=node.SearchApproach;check.item=node.DisplayName;var points=ClosingTimeWorld.CacheApproachPath(route,node.SearchApproach);game.Player.Teleport(points[0]);
    void Step(Vector3 d){Physics.SyncTransforms();game.Player.SimulateMovement(d,false,1f/60);Physics.Simulate(1f/60);}
    for(int i=0;i<45;i++)Step(Vector3.zero);
    bool Follow(Vector3[] path){for(int i=1;i<path.Length;i++){bool reached=false;for(int f=0;f<480;f++){var d=path[i]-game.Player.transform.position;d.y=0;Step(d.magnitude>.055f?d.normalized*Mathf.Clamp01(d.magnitude/.32f):Vector3.zero);if(d.magnitude<.18f&&Mathf.Abs(game.Player.transform.position.y-path[i].y)<.22f&&game.Player.Grounded){reached=true;break;}}if(!reached){check.error="Blocked at connector point "+i;return false;}}return true;}
    check.reached=Follow(points);check.searchable=check.reached&&Vector3.Distance(game.Player.transform.position+Vector3.up*.5f,node.transform.position)<node.SearchRadius&&!Physics.Linecast(game.Player.transform.position+Vector3.up*.7f,node.transform.position+Vector3.up*.2f,1,QueryTriggerInteraction.Ignore);Array.Reverse(points);check.returned=check.reached&&Follow(points);check.end=game.Player.transform.position;
   }
  }finally{Physics.simulationMode=old;}BeginSeed();
 }catch(Exception e){report.error=e.ToString();Debug.LogException(e);Finish();}}
 static void Finish(){report.passed=report.error==null&&report.checks.Count==8&&report.checks.TrueForAll(c=>c.reached&&c.searchable&&c.returned);var path=Path.GetFullPath("../Logs/expedition-route-loot-audit.json");Directory.CreateDirectory(Path.GetDirectoryName(path));File.WriteAllText(path,JsonUtility.ToJson(report,true));SessionState.SetBool(Pending,false);EditorApplication.update-=Tick;Debug.Log("EXPEDITION_ROUTE_LOOT_AUDIT "+report.passed);EditorApplication.isPlaying=false;if(Application.isBatchMode)EditorApplication.Exit(report.passed?0:2);}
}
}
