using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace Jimothy.Editor {
[InitializeOnLoad] public static class ExpeditionIntegrationAudit {
 const string Pending="Jimothy.ExpeditionAudit";static int stage;static double started;static GameSession game;
 [Serializable] class Report {public bool passed;public string error;public List<string> checks=new();}
 static Report report;
 static ExpeditionIntegrationAudit(){EditorApplication.playModeStateChanged+=Mode;}
 public static void Run(){EditorSceneManager.OpenScene("Assets/Jimothy/Scenes/Ballard.unity");SessionState.SetBool(Pending,true);EditorApplication.isPlaying=true;}
 static void Mode(PlayModeStateChange s){if(SessionState.GetBool(Pending,false)&&s==PlayModeStateChange.EnteredPlayMode){stage=0;report=new();EditorApplication.update+=Tick;started=EditorApplication.timeSinceStartup;}}
 static void Check(bool value,string label){if(!value)throw new Exception(label);report.checks.Add(label);}
 static void Tick(){if(!EditorApplication.isPlaying)return;try{
  if(stage==0){game=UnityEngine.Object.FindFirstObjectByType<GameSession>();if(!game)return;game.SuppressSaving=true;game.NewGame();Check(game.AtHome&&!game.Data.runActive&&game.Paused,"New adventure starts on Den night board, save isolated");game.StartNight();started=EditorApplication.timeSinceStartup;stage=1;return;}
  if(EditorApplication.timeSinceStartup-started<1)return;
  if(stage==1){
   var d=game.Data;Check(d.runActive&&game.Playing&&!game.Paused&&d.runNumber==1,"Head out opens exactly one outing");
   int seed=d.forageSeed;game.StartNight();Check(seed==d.forageSeed&&d.runNumber==1,"Repeated Start cannot reroll active outing");
   var nodes=UnityEngine.Object.FindObjectsByType<LootNode>(FindObjectsSortMode.None).Where(n=>n.IsAvailable).ToArray();
   Check(nodes.Count(n=>n.Category=="food")>=3&&nodes.Count(n=>n.Category=="valuable")>=2&&nodes.Any(n=>n.Category=="trophy"),"Generated outing supplies every goal category");
   Check(nodes.Where(n=>n.Category=="curio").Select(n=>n.ItemId).Distinct().Count()==3,"Featured set has all three discoverable pieces");
   foreach(var n in nodes.Where(n=>n.Category=="valuable").Take(2))Check(game.Collect(n.NodeId,game.Items[n.ItemId]),"Collect valuable into loose haul");
   Check(d.coins==0&&d.pendingCoins==8&&d.bag.Count==2,"Valuables remain at risk until banked");
   var first=nodes.First(n=>n.Category=="valuable");Check(!game.Collect(first.NodeId,game.Items[first.ItemId]),"Consumed node cannot pay twice");
   var outside=ClosingTimeWorld.StreetStart;game.Player.Teleport(outside);game.FastTravel();Check(Vector3.Distance(game.Player.transform.position,outside)<.05f,"Loose haul blocks instant extraction");
   foreach(var n in nodes.Where(n=>n.Category=="food").Take(3))Check(game.Collect(n.NodeId,game.Items[n.ItemId]),"Collect goal snack");
   game.Player.Teleport(game.Home);game.Deposit();Check(!d.runActive&&d.coins==23&&d.bag.Count==0&&d.pendingCoins==0,"Bank settles pending valuables, food and twelve-shiny goal once");
   game.Deposit();Check(d.coins==23&&d.bankedRuns==1,"Repeated bank is idempotent");
   d.health=80;d.bag.Add("item_002");int pantry=d.pantry.Count;game.EatPantry();Check(d.bag.Count==1&&d.pantry.Count==pantry-1,"Pantry action preserves carried goal proof");d.bag.Clear();game.StartNight();started=EditorApplication.timeSinceStartup;stage=2;return;
  }
  if(stage==2){var d=game.Data;Check(d.runNumber==2&&d.cooldowns.Count==0,"Next outing resets harvested nodes");
   var trophy=UnityEngine.Object.FindObjectsByType<LootNode>(FindObjectsSortMode.None).First(n=>n.IsAvailable&&n.Category=="trophy");Check(game.Collect(trophy.NodeId,game.Items[trophy.ItemId]),"New outing trophy can be carried");
   d.pendingCoins=5;game.Fell();Check(!game.Playing&&!d.runActive&&d.bag.Count==0&&d.pendingCoins==0&&d.coins==23&&d.health>0,"Out-of-world loss preserves bank and clears all loose rewards");
   game.Resume();game.StartNight();started=EditorApplication.timeSinceStartup;stage=3;return;}
  if(stage==3){var d=game.Data;Check(d.runNumber==3&&d.health==100&&d.hunger==100,"Death can start a healthy next outing");
   Check(UnityEngine.Object.FindObjectsByType<LootNode>(FindObjectsSortMode.None).Any(n=>n.IsAvailable&&n.Category=="trophy"),"Trophies remain obtainable after a loss");
   var serialized=JsonUtility.ToJson(d);var copy=JsonUtility.FromJson<SaveData>(serialized);copy.ApplyOffline(DateTimeOffset.UtcNow.ToUnixTimeSeconds()+86400);Check(copy.forageSeed==d.forageSeed&&copy.runNumber==d.runNumber&&copy.nightEvent==d.nightEvent&&Math.Abs(copy.worldSeconds-d.worldSeconds)<.000001,"JSON and offline resume preserve active outing identity and clock");
   game.Menu();game.Resume();Check(d.runNumber==3&&d.runActive,"Menu resume does not start or reroll an outing");report.passed=true;Finish();}
 }catch(Exception e){report??=new();report.error=e.ToString();Debug.LogException(e);Finish();}}
 static void Finish(){SessionState.SetBool(Pending,false);EditorApplication.update-=Tick;var p=Path.GetFullPath("../Logs/expedition-integration-audit.json");Directory.CreateDirectory(Path.GetDirectoryName(p));File.WriteAllText(p,JsonUtility.ToJson(report,true));Debug.Log("EXPEDITION_INTEGRATION_AUDIT "+report.passed);EditorApplication.Exit(report.passed?0:2);}
}
}
