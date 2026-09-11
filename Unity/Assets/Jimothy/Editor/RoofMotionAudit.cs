using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
namespace Jimothy.Editor {
/// <summary>Actual camera-motion roof evidence, with frozen simulation and unchanged lights.</summary>
[InitializeOnLoad] public static class RoofMotionAudit {
 const string Pending="Jimothy.RoofMotionAudit";const int Width=1280,Height=720,Count=8;
 [Serializable] class Frame {public string image;public Vector3 cameraPosition,target;}
 [Serializable] class Report {public string method="Eight actual Unity URP renders from a camera sliding 0.40 m across the first crossing/access coping. Player placed once, disabled; game paused, Time.timeScale = 0 keeps simulation/shader time fixed. Lighting settings unchanged. Contact sheet is a montage of those renders. No automatic temporal-quality or flicker-pass claim.";public bool savingSuppressed;public List<Frame> frames=new();}
 static GameSession game;static Camera camera;static RenderTexture target;static Texture2D sheet;static int shot=-1;static double readyAt;static float previousScale=1;static string folder;static Report report;static Vector3 aim;
 static RoofMotionAudit(){EditorApplication.playModeStateChanged+=OnMode;if(SessionState.GetBool(Pending,false))EditorApplication.update+=Tick;}
 [MenuItem("Scraps/Capture roof camera-motion evidence")]
 public static void Run(){if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop play mode before isolated art audit.");EditorSceneManager.OpenScene("Assets/Jimothy/Scenes/Ballard.unity");SessionState.SetBool(Pending,true);EditorApplication.isPlaying=true;}
 static void OnMode(PlayModeStateChange mode){if(!SessionState.GetBool(Pending,false))return;if(mode==PlayModeStateChange.EnteredPlayMode){shot=-1;EditorApplication.update-=Tick;EditorApplication.update+=Tick;}}
 static void Tick(){if(!SessionState.GetBool(Pending,false)||!EditorApplication.isPlaying)return;try{
  if(shot<0){game=UnityEngine.Object.FindFirstObjectByType<GameSession>();if(!game)return;game.SuppressSaving=true;game.NewGame();game.Player.Teleport(new Vector3(-12,8.24f,-24.7f));game.TogglePause();game.Player.enabled=false;foreach(var canvas in game.GetComponentsInChildren<Canvas>())canvas.enabled=false;
   previousScale=Time.timeScale;Time.timeScale=0;camera=Camera.main;camera.fieldOfView=58;camera.aspect=16f/9;target=new RenderTexture(Width,Height,24,RenderTextureFormat.DefaultHDR){name="Roof motion evidence",antiAliasing=1};target.Create();camera.targetTexture=target;folder=Path.GetFullPath("../PlaytestCaptures/roof-motion");Directory.CreateDirectory(folder);sheet=new Texture2D(1920,540,TextureFormat.RGB24,false);report=new Report{savingSuppressed=game.SuppressSaving};shot=0;Pose();return;}
  if(EditorApplication.timeSinceStartup<readyAt)return;
  RenderPipeline.SubmitRenderRequest(camera,new RenderPipeline.StandardRequest{destination=target});string filename="roof-"+shot.ToString("00")+".png";Capture(filename);report.frames.Add(new Frame{image=filename,cameraPosition=camera.transform.position,target=aim});shot++;
  if(shot==Count){sheet.Apply();File.WriteAllBytes(Path.Combine(folder,"contact-sheet.png"),sheet.EncodeToPNG());File.WriteAllText(Path.Combine(folder,"capture.json"),JsonUtility.ToJson(report,true));Finish(0);return;}Pose();
 }catch(Exception e){Debug.LogException(e);folder=Path.GetFullPath("../PlaytestCaptures/roof-motion");Directory.CreateDirectory(folder);File.WriteAllText(Path.Combine(folder,"error.txt"),e.ToString());Finish(1);}}
 static void Pose(){float slide=(shot/(float)(Count-1)-.5f)*.4f;camera.transform.position=new Vector3(-8.5f+slide,10.1f,-20.5f);aim=new Vector3(-13.3f,7.95f,-24.4f);camera.transform.LookAt(aim);readyAt=EditorApplication.timeSinceStartup+.25;}
 static void Capture(string filename){var ldr=RenderTexture.GetTemporary(Width,Height,0,RenderTextureFormat.ARGB32,RenderTextureReadWrite.sRGB);var thumbnail=RenderTexture.GetTemporary(480,270,0,RenderTextureFormat.ARGB32,RenderTextureReadWrite.sRGB);var previous=RenderTexture.active;Texture2D image=null;try{Graphics.Blit(target,ldr);RenderTexture.active=ldr;image=new Texture2D(Width,Height,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,Width,Height),0,0);image.Apply();File.WriteAllBytes(Path.Combine(folder,filename),image.EncodeToPNG());Graphics.Blit(ldr,thumbnail);RenderTexture.active=thumbnail;sheet.ReadPixels(new Rect(0,0,480,270),(shot%4)*480,(1-shot/4)*270);}finally{RenderTexture.active=previous;if(image)UnityEngine.Object.Destroy(image);RenderTexture.ReleaseTemporary(ldr);RenderTexture.ReleaseTemporary(thumbnail);}}
 static void Finish(int code){Time.timeScale=previousScale;if(camera)camera.targetTexture=null;if(target){target.Release();UnityEngine.Object.Destroy(target);target=null;}if(sheet)UnityEngine.Object.Destroy(sheet);SessionState.SetBool(Pending,false);EditorApplication.update-=Tick;Debug.Log("ROOF_MOTION_CAPTURE "+code+" "+folder);EditorApplication.isPlaying=false;if(Application.isBatchMode)EditorApplication.Exit(code);}
}
}
