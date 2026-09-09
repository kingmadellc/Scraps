using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEditor.SceneManagement;
namespace Jimothy.Editor {
public static class TouchSchemeAudit {
 [Serializable] class Report{public bool passed;public List<string> checks=new();public string error;}
 public static void Run(){var report=new Report();var old=Physics.simulationMode;int pref=PlayerPrefs.GetInt("touchTapTricks",0);try{EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);Physics.simulationMode=SimulationMode.Script;PlayerPrefs.SetInt("touchTapTricks",0);
  void Check(bool ok,string message){if(!ok)throw new Exception(message);report.checks.Add(message);}
  var ground=new GameObject("ground");ground.transform.position=new(0,-.25f,0);ground.AddComponent<BoxCollider>().size=new(100,.5f,100);var go=new GameObject("motor");var m=go.AddComponent<RaccoonMotor>();m.Initialize(null);m.Running=true;
  var canvas=new GameObject("touch canvas",typeof(Canvas));var pad=new GameObject("jump",typeof(RectTransform)).AddComponent<TouchTrickGesture>();pad.transform.SetParent(canvas.transform,false);pad.motor=m;
  void Tick(bool jump=false){Physics.SyncTransforms();m.SimulateMovement(Vector3.zero,jump,1f/60);Physics.Simulate(1f/60);}
  void Settle(){m.Teleport(new(0,.06f,0));for(int i=0;i<30;i++)Tick();}
  foreach(var d in new[]{Vector2.up,Vector2.down,Vector2.left,Vector2.right}){Settle();int before=m.CompletedTricks;var e=new PointerEventData(null){pointerId=2,position=new(100,100)};pad.OnPointerDown(e);Check(m.jumpRequested,"Tap initiates jump "+d);e.position+=d*80;pad.OnDrag(e);Check(pad.Pending,"Flick buffers across initial grounded frame "+d);pad.OnPointerUp(e);bool jump=m.jumpRequested;m.jumpRequested=false;Tick(jump);pad.SendMessage("Update");for(int i=0;i<80;i++)Tick();Check(m.CompletedTricks==before+1&&m.LastLandingPoints==150&&m.LastLandingClean,"One gesture completes exactly one clean directional trick "+d);pad.Release();}
  Settle();int fastBefore=m.CompletedTricks;var fast=new PointerEventData(null){pointerId=7,position=new(100,100)};pad.OnPointerDown(fast);fast.position+=Vector2.up*80;pad.OnPointerUp(fast);bool fastJump=m.jumpRequested;m.jumpRequested=false;Tick(fastJump);pad.SendMessage("Update");for(int i=0;i<80;i++)Tick();Check(m.CompletedTricks==fastBefore+1,"Fast swipe ending between frames is recognized on release");pad.Release();
  Settle();var owner=new PointerEventData(null){pointerId=4,position=new(100,100)};pad.OnPointerDown(owner);pad.OnPointerDown(new PointerEventData(null){pointerId=5});pad.OnPointerUp(new PointerEventData(null){pointerId=5});Check(pad.Owner==4,"Second finger cannot steal jump gesture");pad.Release();Check(!m.jumpRequested&&!pad.Pending&&pad.Owner==int.MinValue,"Pause/release clears queued jump and trick");
  foreach(var d in new[]{Vector2.up,Vector2.down,Vector2.left,Vector2.right}){Settle();m.touchMoveHeading=0;m.FaceDirection(0);var start=m.transform.position;for(int i=0;i<120;i++){m.SimulateTouchSteering(d,Vector2.zero,false,1f/60);Physics.Simulate(1f/60);}var delta=m.transform.position-start;Check(Vector3.Dot(delta,new Vector3(d.x,0,d.y))>5.8f,"Held cardinal stick moves straight without camera-induced circling "+d);Check(Mathf.Abs(Mathf.DeltaAngle(m.ViewHeading,Mathf.Atan2(d.x,d.y)*Mathf.Rad2Deg))<1,"Rear camera heading follows cardinal travel "+d);}
  Settle();m.touchMoveHeading=0;m.FaceDirection(0);m.SimulateTouchSteering(Vector2.zero,new Vector2(90,0),false,1f/60);Check(Mathf.Abs(Mathf.DeltaAngle(m.ViewHeading,90))<1,"Neutral held stick allows camera rotation");var neutralStart=m.transform.position;for(int i=0;i<120;i++){m.SimulateTouchSteering(Vector2.up,Vector2.zero,false,1f/60);Physics.Simulate(1f/60);}Check(m.transform.position.x-neutralStart.x>5.8f,"Movement after neutral camera drag uses updated heading");
  report.passed=true;
 }catch(Exception e){report.error=e.ToString();Debug.LogException(e);}finally{PlayerPrefs.SetInt("touchTapTricks",pref);Physics.simulationMode=old;var path=Path.GetFullPath("../Logs/touch-scheme-audit.json");Directory.CreateDirectory(Path.GetDirectoryName(path));File.WriteAllText(path,JsonUtility.ToJson(report,true));Debug.Log("TOUCH_SCHEME_AUDIT "+report.passed);EditorApplication.Exit(report.passed?0:1);}}
}
}
