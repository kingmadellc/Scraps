using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
namespace Jimothy.Editor {
[InitializeOnLoad] public static class WardrobeArtAudit {
 const string Key="Jimothy.WardrobeArtAudit";
 static readonly string[] Pages={"den","bare","bandana","crown","roadie","boat","inspector"};
 [Serializable] class TextEvidence {public string text;public Rect bounds;public int size;public bool outsideCapture;}
 [Serializable] class Frame {public string file;public int width,height;public List<TextEvidence> labels=new();public List<string> glyphs=new();}
 [Serializable] class Report {public string method="Actual Unity UI camera renders at phone 1688x780 and 4:3 tablet 1440x1080 canvas sizes. Forced mobile layout, full safe-area preview, no physical touch/device/browser testing. Page methods invoked for design inspection; no preference changes or saved-game loads. Text bounds are diagnostics, not human readability scores.";public List<Frame> frames=new();public string error;}
 static Report report;static GameSession game;static Camera camera;static RenderTexture target;static int shot=-1,width,height;static double ready;static string folder;static float oldTime;
 static WardrobeArtAudit(){EditorApplication.playModeStateChanged+=s=>{if(s==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool(Key,false)){shot=-1;EditorApplication.update+=Tick;}};}
 public static void Run(){EditorSceneManager.OpenScene("Assets/Jimothy/Scenes/Ballard.unity");SessionState.SetBool(Key,true);EditorApplication.isPlaying=true;}
 static void Invoke(string method,params object[] args)=>typeof(GameUI).GetMethod(method,BindingFlags.Instance|BindingFlags.NonPublic).Invoke(game.UI,args);
 static void Pose(){
  width=shot<Pages.Length?1688:1440;height=shot<Pages.Length?780:1080;
  if(target){camera.targetTexture=null;target.Release();UnityEngine.Object.Destroy(target);}target=new RenderTexture(width,height,24,RenderTextureFormat.DefaultHDR){antiAliasing=2};target.Create();camera.targetTexture=target;camera.aspect=(float)width/height;
  game.Resume();switch(Pages[shot%Pages.Length]){case "bare":case "bandana":case "crown":case "roadie":case "boat":case "inspector":game.UI.ShowWardrobe(Array.FindIndex(WardrobeRules.Looks,x=>x.id==Pages[shot%Pages.Length]));break;case "title":game.Menu();break;case "pause":game.TogglePause();break;case "controls":game.TogglePause();Invoke("ShowTouchHelp");break;case "settings":game.TogglePause();Invoke("Settings",true);break;case "outing":game.UI.ShowNightBoard();break;case "rules":game.UI.ShowNightBoard();Invoke("ShowOutingRules");break;case "collections":game.UI.ShowNightBoard();Invoke("ShowCollections");break;case "den":game.UI.OpenDenForPlaytest();break;case "hud":game.UI.ShowHUD();break;}
  foreach(var canvas in game.UI.GetComponentsInChildren<Canvas>()){canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=1;var scaler=canvas.GetComponent<CanvasScaler>();if(scaler)scaler.enabled=false;canvas.scaleFactor=height/810f;}
  Canvas.ForceUpdateCanvases();foreach(var glyph in game.UI.GetComponentsInChildren<TouchGlyph>()){glyph.SetAllDirty();}Canvas.ForceUpdateCanvases();ready=EditorApplication.timeSinceStartup+.2;
 }
 static void Tick(){if(!EditorApplication.isPlaying)return;try{
  if(shot<0){game=UnityEngine.Object.FindFirstObjectByType<GameSession>();if(!game)return;GameUI.ForceMobileLayout=true;game.SuppressSaving=true;game.NewGame();game.Data.bankedDiscoveries.AddRange(new[]{"item_176","item_128","item_000"});game.Data.bankedRuns=4;game.Player.enabled=false;game.Player.UpdateFollowCamera(.1f);camera=Camera.main;oldTime=Time.timeScale;Time.timeScale=0;folder=Path.GetFullPath("../PlaytestCaptures/wardrobe-060");Directory.CreateDirectory(folder);report=new Report();shot=0;Pose();return;}
  if(EditorApplication.timeSinceStartup<ready)return;foreach(var glyph in game.UI.GetComponentsInChildren<TouchGlyph>())glyph.SetAllDirty();Canvas.ForceUpdateCanvases();RenderPipeline.SubmitRenderRequest(camera,new RenderPipeline.StandardRequest{destination=target});
  var frame=new Frame{file=(shot<Pages.Length?"phone-":"tablet-")+Pages[shot%Pages.Length]+".png",width=width,height=height};
  foreach(var glyph in game.UI.GetComponentsInChildren<TouchGlyph>()){var mesh=glyph.canvasRenderer.GetMesh();frame.glyphs.Add(glyph.symbol+" vertices="+mesh.vertexCount+" bounds="+mesh.bounds+" culled="+glyph.canvasRenderer.cull+" shader="+glyph.materialForRendering.shader.name);}
  foreach(var t in game.UI.GetComponentsInChildren<Text>()){if(!t.gameObject.activeInHierarchy||string.IsNullOrWhiteSpace(t.text))continue;var corners=new Vector3[4];t.rectTransform.GetWorldCorners(corners);var a=RectTransformUtility.WorldToScreenPoint(camera,corners[0]);var b=RectTransformUtility.WorldToScreenPoint(camera,corners[2]);frame.labels.Add(new TextEvidence{text=t.text,size=t.fontSize,bounds=Rect.MinMaxRect(a.x,a.y,b.x,b.y),outsideCapture=a.x< -2||a.y< -2||b.x>width+2||b.y>height+2});}
  var ldr=RenderTexture.GetTemporary(width,height,0,RenderTextureFormat.ARGB32,RenderTextureReadWrite.sRGB);var previous=RenderTexture.active;Graphics.Blit(target,ldr);RenderTexture.active=ldr;var image=new Texture2D(width,height,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,width,height),0,0);image.Apply();File.WriteAllBytes(Path.Combine(folder,frame.file),image.EncodeToPNG());RenderTexture.active=previous;RenderTexture.ReleaseTemporary(ldr);UnityEngine.Object.Destroy(image);report.frames.Add(frame);
  if(++shot==Pages.Length*2){Finish(0);return;}Pose();
 }catch(Exception e){if(report==null)report=new Report();report.error=e.ToString();Finish(1);}}
 static void Finish(int code){if(folder==null){folder=Path.GetFullPath("../PlaytestCaptures/wardrobe-060");Directory.CreateDirectory(folder);}File.WriteAllText(Path.Combine(folder,"report.json"),JsonUtility.ToJson(report,true));Time.timeScale=oldTime;GameUI.ForceMobileLayout=false;if(camera)camera.targetTexture=null;if(target){target.Release();UnityEngine.Object.Destroy(target);}EditorApplication.update-=Tick;SessionState.SetBool(Key,false);EditorApplication.isPlaying=false;if(Application.isBatchMode)EditorApplication.Exit(code);}
}
}
