using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace Jimothy.Editor {
/// <summary>Actual playmode interaction integration test. Teleports deliberately isolate interaction from traversal.</summary>
[InitializeOnLoad] public static class FirstNightAudit {
 const string Pending="Jimothy.FirstNightAuditPending";
 static GameSession game;static int stage,frame;static double deadline,worldBefore;static float hungerBefore,healthBefore;static LootNode roof;
 static Report report;static readonly Dictionary<string,byte[]> original=new();
 [Serializable] class Report {public bool passed;public List<string> assertions=new();public string error,note="Actual Editor playmode integration. Teleports test interactions and state transitions, not route traversal or player fun. Existing user saves are read for byte comparison only; all session saving is suppressed.";}
 static FirstNightAudit(){EditorApplication.playModeStateChanged+=OnMode;if(SessionState.GetBool(Pending,false))EditorApplication.update+=Tick;}
 [MenuItem("Scraps/Verify first night interactions safely")]
 public static void Run(){if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop current playmode before isolated audit.");EditorSceneManager.OpenScene("Assets/Jimothy/Scenes/Ballard.unity");SessionState.SetBool(Pending,true);EditorApplication.isPlaying=true;}
 static void OnMode(PlayModeStateChange mode){if(!SessionState.GetBool(Pending,false))return;if(mode==PlayModeStateChange.EnteredPlayMode){stage=0;deadline=EditorApplication.timeSinceStartup+120;EditorApplication.update-=Tick;EditorApplication.update+=Tick;}}
 static void Check(bool pass,string message){if(!pass)throw new InvalidOperationException(message);report.assertions.Add(message);}
 static void WaitStage(int next){stage=next;frame=Time.frameCount;}
 static bool FramesReady=>Time.frameCount-frame>=20;
 static void SnapshotSaves(){original.Clear();foreach(string suffix in new[]{"",".bak",".tmp"}){string p=SaveStore.PathName+suffix;original[p]=File.Exists(p)?File.ReadAllBytes(p):null;}}
 static void VerifySaves(){foreach(var file in original){bool exists=File.Exists(file.Key);Check(file.Value==null?!exists:exists&&file.Value.SequenceEqual(File.ReadAllBytes(file.Key)),"User save unchanged: "+Path.GetFileName(file.Key));}}
 static void Tick(){
  if(!SessionState.GetBool(Pending,false)||!EditorApplication.isPlaying)return;
  try {
   if(EditorApplication.timeSinceStartup>deadline)throw new TimeoutException("First-night audit timed out waiting for game frames.");
   if(stage==0){game=UnityEngine.Object.FindFirstObjectByType<GameSession>();if(!game)return;report=new Report();SnapshotSaves();game.SuppressSaving=true;Check(game.SuppressSaving,"Saving suppressed before NewGame");game.NewGame();WaitStage(1);return;}
   if(!FramesReady)return;
   if(stage==1){
    Check(game.NearbyLoot&&game.NearbyLoot.name=="welcome_snack","Welcome snack is reachable through contextual search at spawn");game.Search();Check(game.Data.nightGoal==1,"Searching welcome snack advances food objective");Check(game.Data.bag.Contains("item_002"),"Welcome bagel enters inventory");game.Eat();Check(game.Data.bag.Contains("item_002"),"Eating while fully healthy and full preserves food");
    roof=UnityEngine.Object.FindObjectsByType<LootNode>(FindObjectsSortMode.None).Where(n=>n.IsRooftop&&n.IsAvailable).OrderBy(n=>(n.transform.position-game.Home).sqrMagnitude).First();
    game.Player.Teleport(roof.SearchApproach);Physics.SyncTransforms();WaitStage(2);return;
   }
   if(stage==2){
    Check(game.Data.nightGoal==2,"Actual rooftop position triggers reached-roof objective");Check(game.NearbyLoot==roof,"Rooftop keepsake is reachable through contextual search");game.Search();Check(game.Data.bag.Any(id=>id.StartsWith("trophy_")),"Searching rooftop keepsake enters inventory");
    game.FastTravel();Check(game.AtHome,"Unpursued fast travel returns to den");game.OpenDen();Check(game.Paused&&game.DenOpen&&!game.Player.Running,"Den freezes gameplay controls and session");game.Deposit();Check(game.Data.nightGoal==3,"Banking keepsake completes first night");Check(game.Data.trophies.Count==1&&game.Data.bag.Count==0,"Banking stores trophy and clears pockets");Check(game.Data.coins>=20,"First-night earnings make cushion affordable");int coins=game.Data.coins;game.BuyDecor("cushion",20);Check(game.Data.decor.Contains("cushion")&&game.Data.coins==coins-20,"Cushion purchase consumes currency and persists in session");
    Check(UnityEngine.Object.FindObjectsByType<MeshFilter>(FindObjectsSortMode.None).Any(m=>m.gameObject.name==game.Data.trophies[0]&&m.transform.parent&&m.transform.parent.name=="Persistent den collection"&&m.sharedMesh&&m.sharedMesh.colors.Length==m.sharedMesh.vertexCount),"Banked trophy displays its authored coloured reward mesh in den");
    hungerBefore=game.Data.hunger;healthBefore=game.Data.health;worldBefore=game.Data.worldSeconds;WaitStage(3);return;
   }
   if(stage==3){
    Check(game.Data.hunger==hungerBefore&&game.Data.health==healthBefore&&game.Data.worldSeconds==worldBefore,"Den pauses hunger, health and world clock across 20+ frames");game.Resume();game.Player.Teleport(new Vector3(0,.45f,15));game.TogglePause();hungerBefore=game.Data.hunger;worldBefore=game.Data.worldSeconds;WaitStage(4);return;
   }
   if(stage==4){
    Check(game.Paused&&game.Data.hunger==hungerBefore&&game.Data.worldSeconds==worldBefore,"Pause freezes survival outside den across 20+ frames");game.Resume();game.Player.Teleport(game.Home);game.Data.hunger=0;game.Data.health=45;worldBefore=game.Data.worldSeconds;WaitStage(5);return;
   }
   Check(game.AtHome&&game.Data.hunger==0&&game.Data.health==45,"Safe home prevents starvation damage across 20+ unpaused frames");Check(game.Data.worldSeconds>worldBefore,"Unpaused world resumes ticking");game.Save();Check(game.SuppressSaving,"Saving remained suppressed through all interactions");VerifySaves();report.passed=true;Finish();
  }catch(Exception ex){if(report==null)report=new Report();report.error=ex.ToString();Debug.LogException(ex);Finish();}
 }
 static void Finish(){string dir=Path.GetFullPath("../PlaytestCaptures");Directory.CreateDirectory(dir);File.WriteAllText(Path.Combine(dir,"first-night-audit.json"),JsonUtility.ToJson(report,true));Debug.Log("FIRST_NIGHT_AUDIT "+(report.passed?"PASS":"FAIL"));SessionState.SetBool(Pending,false);EditorApplication.update-=Tick;EditorApplication.isPlaying=false;if(Application.isBatchMode)EditorApplication.Exit(report.passed?0:1);}
}
}
