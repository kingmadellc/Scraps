using System;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
namespace Jimothy {
/// <summary>Read-only browser accessibility and smoke-test snapshot; never changes game state.</summary>
public sealed class BrowserState : MonoBehaviour {
 [Serializable] class Watcher {public string kind;public float suspicion;public bool visible,chasing;}
 [Serializable] class Control{public string name,label;public bool interactable;public float x,y,w,h;}
 [Serializable] class Snapshot {
  public string version=Application.version;public bool discoveryOpen;public string discoveryItem;
  public string coordinates="Unity metres; +Y up, initial forward -Z",mode,objective,guidance;
  public string[] uiText;public string jumpDiagnostic;public int jumpOwner;public bool jumpPending;public Control[] controls;public string lastTrick;public bool trickActive;public Vector3 position;public Vector2 touchMove,touchLook;public float heading,movementHeading;
  public string outfit;public int coat,wardrobeUnlocked;public bool runActive;public int forageSeed,consumedNodes,runNumber,pendingCoins,goal,nightEvent;public string nightStatus;public int bag,banked,trophies,coins,trickScore,bestLanding;public float health;public bool grounded;public Watcher[] watchers;
 }
 GameSession game;float next;
 public void Initialize(GameSession session){game=session;}
#if UNITY_WEBGL && !UNITY_EDITOR
 [DllImport("__Internal")] static extern void JimothyPublishState(string json);
#endif
 void Update(){
#if UNITY_WEBGL && !UNITY_EDITOR
  if(!game||Time.unscaledTime<next)return;next=Time.unscaledTime+.1f;
  var s=new Snapshot {discoveryOpen=game.DiscoveryOpen,discoveryItem=game.GetComponent<DiscoveryReveal>()?.ItemName,mode=!game.Playing?"menu":game.Paused?"paused":"playing",objective=game.Objective};
  if(game.Data!=null){s.outfit=game.Data.outfit;s.coat=game.Data.coat;s.wardrobeUnlocked=WardrobeRules.Count(game.Data);s.forageSeed=game.Data.forageSeed;s.consumedNodes=game.Data.cooldowns.Count(c=>c.harvests>0);s.runActive=game.Data.runActive;s.runNumber=game.Data.runNumber;s.pendingCoins=game.Data.pendingCoins;s.goal=game.Data.selectedGoal;s.nightEvent=game.Data.nightEvent;s.nightStatus=game.NightStatus;s.bag=game.Data.bag.Count;s.banked=game.Data.pantry.Count;s.trophies=game.Data.trophies.Count;s.coins=game.Data.coins;s.health=game.Data.health;s.trickScore=game.Data.trickScore;s.bestLanding=game.Data.bestLandingScore;}
  if(game.Player){s.position=game.Player.transform.position;s.movementHeading=game.Player.ViewHeading;s.heading=Camera.main?Camera.main.transform.eulerAngles.y:game.Player.transform.eulerAngles.y;s.touchMove=game.Player.touchMove;s.touchLook=game.Player.touchLook;s.guidance=game.Guidance;var cc=game.Player.GetComponent<CharacterController>();s.grounded=cc&&cc.isGrounded;}
  var gesture=game.GetComponentInChildren<TouchTrickGesture>();if(gesture){s.jumpDiagnostic=gesture.Diagnostic;s.jumpOwner=gesture.Owner;s.jumpPending=gesture.Pending;}var controls=new List<Control>();foreach(var rect in game.GetComponentsInChildren<RectTransform>()){if(!rect.gameObject.activeInHierarchy||(!rect.GetComponent<Button>()&&!rect.GetComponent<TouchPad>()))continue;var corners=new Vector3[4];rect.GetWorldCorners(corners);var button=rect.GetComponent<Button>();controls.Add(new Control{name=rect.name,label=button?button.GetComponentInChildren<Text>()?.text:rect.name,interactable=!button||button.interactable,x=(corners[0].x+corners[2].x)*.5f/Screen.width,y=1-(corners[0].y+corners[2].y)*.5f/Screen.height,w=(corners[2].x-corners[0].x)/Screen.width,h=(corners[2].y-corners[0].y)/Screen.height});}s.controls=controls.ToArray();s.uiText=game.GetComponentsInChildren<Text>().Where(t=>t.isActiveAndEnabled&&!string.IsNullOrWhiteSpace(t.text)).Select(t=>t.text).Distinct().ToArray();if(game.Player){s.lastTrick=game.Player.LastTrickName;s.trickActive=game.Player.IsTricking;}
  s.watchers=game.Neighbors.Where(n=>n&&n.Suspicion01>0).Select(n=>new Watcher{kind=n.Kind.ToString(),suspicion=n.Suspicion01,visible=n.HasLineOfSight,chasing=n.IsThreat}).ToArray();
  JimothyPublishState(JsonUtility.ToJson(s));
#endif
 }
}
}
