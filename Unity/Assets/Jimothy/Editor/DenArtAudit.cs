using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
namespace Jimothy.Editor {
/// <summary>Actual camera-motion roof evidence, with frozen simulation and unchanged lights.</summary>
[InitializeOnLoad] public static class DenArtAudit {
 const string Pending="Jimothy.DenArtAudit";const int Width=1280,Height=720,Count=3;
 [Serializable] class Frame {public string image;public Vector3 cameraPosition,target;}
 [Serializable] class Report {public string method="Actual Unity URP: rear gameplay camera inside den, separate beauty overview, rear gameplay camera at entrance. Isolated new game with saving suppressed; initial placements for art inspection, not a continuous gameplay test. Runtime interior lighting active. HUD hidden for geometry review.";public bool savingSuppressed;public List<Frame> frames=new();}
 static GameSession game;static Camera camera;static RenderTexture target;static Texture2D sheet;static int shot=-1;static double readyAt;static float previousScale=1;static string folder;static Report report;static Vector3 aim;
 static DenArtAudit(){EditorApplication.playModeStateChanged+=OnMode;if(SessionState.GetBool(Pending,false))EditorApplication.update+=Tick;}
 [MenuItem("Scraps/Capture den geometry evidence")]
 public static void Run(){if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop play mode before isolated art audit.");EditorSceneManager.OpenScene("Assets/Jimothy/Scenes/Ballard.unity");SessionState.SetBool(Pending,true);EditorApplication.isPlaying=true;}
 static void OnMode(PlayModeStateChange mode){if(!SessionState.GetBool(Pending,false))return;if(mode==PlayModeStateChange.EnteredPlayMode){shot=-1;EditorApplication.update-=Tick;EditorApplication.update+=Tick;}}
 static void Tick(){if(!SessionState.GetBool(Pending,false)||!EditorApplication.isPlaying)return;try{
  if(shot<0){game=UnityEngine.Object.FindFirstObjectByType<GameSession>();if(!game)return;game.SuppressSaving=true;game.NewGame();game.Player.Teleport(ClosingTimeWorld.DenInterior);game.Player.FaceDirection(0);game.Player.RecenterCamera();game.Player.enabled=false;foreach(var canvas in game.GetComponentsInChildren<Canvas>())canvas.enabled=false;
   previousScale=Time.timeScale;camera=Camera.main;camera.fieldOfView=58;camera.aspect=16f/9;target=new RenderTexture(Width,Height,24,RenderTextureFormat.DefaultHDR){name="Roof motion evidence",antiAliasing=1};target.Create();camera.targetTexture=target;folder=Path.GetFullPath("../PlaytestCaptures/den-room");Directory.CreateDirectory(folder);sheet=new Texture2D(1440,270,TextureFormat.RGB24,false);report=new Report{savingSuppressed=game.SuppressSaving};shot=0;Pose();return;}
  if(EditorApplication.timeSinceStartup<readyAt)return;
  RenderPipeline.SubmitRenderRequest(camera,new RenderPipeline.StandardRequest{destination=target});string filename="den-"+shot.ToString("00")+".png";Capture(filename);report.frames.Add(new Frame{image=filename,cameraPosition=camera.transform.position,target=aim});shot++;
  if(shot==Count){sheet.Apply();File.WriteAllBytes(Path.Combine(folder,"contact-sheet.png"),sheet.EncodeToPNG());File.WriteAllText(Path.Combine(folder,"capture.json"),JsonUtility.ToJson(report,true));Finish(0);return;}Pose();
 }catch(Exception e){Debug.LogException(e);folder=Path.GetFullPath("../PlaytestCaptures/den-room");Directory.CreateDirectory(folder);File.WriteAllText(Path.Combine(folder,"error.txt"),e.ToString());Finish(1);}}
 static void Pose(){
  if(shot==0){game.Player.Teleport(ClosingTimeWorld.DenInterior);game.Player.FaceDirection(0);game.Player.RecenterCamera();for(int i=0;i<60;i++)game.Player.UpdateFollowCamera(1f/60);aim=game.Player.transform.position+Vector3.up*.3f;}
  else if(shot==1){camera.transform.position=new Vector3(-18,-.1f,56.6f);aim=new Vector3(-13.8f,-1.4f,60.7f);camera.transform.LookAt(aim);}
  else{game.Player.Teleport(ClosingTimeWorld.DenEntrance);game.Player.FaceDirection(0);game.Player.RecenterCamera();for(int i=0;i<60;i++)game.Player.UpdateFollowCamera(1f/60);aim=game.Player.transform.position+Vector3.up*.3f;}
  readyAt=EditorApplication.timeSinceStartup+.8;
 }

 static void Capture(string filename){var ldr=RenderTexture.GetTemporary(Width,Height,0,RenderTextureFormat.ARGB32,RenderTextureReadWrite.sRGB);var thumbnail=RenderTexture.GetTemporary(480,270,0,RenderTextureFormat.ARGB32,RenderTextureReadWrite.sRGB);var previous=RenderTexture.active;Texture2D image=null;try{Graphics.Blit(target,ldr);RenderTexture.active=ldr;image=new Texture2D(Width,Height,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,Width,Height),0,0);image.Apply();File.WriteAllBytes(Path.Combine(folder,filename),image.EncodeToPNG());Graphics.Blit(ldr,thumbnail);RenderTexture.active=thumbnail;sheet.ReadPixels(new Rect(0,0,480,270),(shot%4)*480,0);}finally{RenderTexture.active=previous;if(image)UnityEngine.Object.Destroy(image);RenderTexture.ReleaseTemporary(ldr);RenderTexture.ReleaseTemporary(thumbnail);}}
 static void Finish(int code){Time.timeScale=previousScale;if(camera)camera.targetTexture=null;if(target){target.Release();UnityEngine.Object.Destroy(target);target=null;}if(sheet)UnityEngine.Object.Destroy(sheet);SessionState.SetBool(Pending,false);EditorApplication.update-=Tick;Debug.Log("DEN_ART_CAPTURE "+code+" "+folder);EditorApplication.isPlaying=false;if(Application.isBatchMode)EditorApplication.Exit(code);}
}
}
