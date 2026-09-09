using System;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
namespace Jimothy {
/// <summary>Read-only browser accessibility and smoke-test snapshot; never changes game state.</summary>
public sealed class BrowserState : MonoBehaviour {
 [Serializable] class Watcher {public string kind;public float suspicion;public bool visible,chasing;}
 [Serializable] class Snapshot {
  public string version=Application.version;
  public string coordinates="Unity metres; +Y up, initial forward -Z",mode,objective,guidance;
  public Vector3 position;public Vector2 touchMove,touchLook;public float heading;
  public int bag,banked,trophies,coins,trickScore,bestLanding;public float health;public bool grounded;public Watcher[] watchers;
 }
 GameSession game;float next;
 public void Initialize(GameSession session){game=session;}
#if UNITY_WEBGL && !UNITY_EDITOR
 [DllImport("__Internal")] static extern void JimothyPublishState(string json);
#endif
 void Update(){
#if UNITY_WEBGL && !UNITY_EDITOR
  if(!game||Time.unscaledTime<next)return;next=Time.unscaledTime+.1f;
  var s=new Snapshot {mode=!game.Playing?"menu":game.Paused?"paused":"playing",objective=game.Objective};
  if(game.Data!=null){s.bag=game.Data.bag.Count;s.banked=game.Data.pantry.Count;s.trophies=game.Data.trophies.Count;s.coins=game.Data.coins;s.health=game.Data.health;s.trickScore=game.Data.trickScore;s.bestLanding=game.Data.bestLandingScore;}
  if(game.Player){s.position=game.Player.transform.position;s.heading=Camera.main?Camera.main.transform.eulerAngles.y:game.Player.transform.eulerAngles.y;s.touchMove=game.Player.touchMove;s.touchLook=game.Player.touchLook;s.guidance=game.Guidance;var cc=game.Player.GetComponent<CharacterController>();s.grounded=cc&&cc.isGrounded;}
  s.watchers=game.Neighbors.Where(n=>n&&n.Suspicion01>0).Select(n=>new Watcher{kind=n.Kind.ToString(),suspicion=n.Suspicion01,visible=n.HasLineOfSight,chasing=n.IsThreat}).ToArray();
  JimothyPublishState(JsonUtility.ToJson(s));
#endif
 }
}
}
