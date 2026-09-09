using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
namespace Jimothy.Editor {
/// <summary>Isolated art review camera set. No player save is loaded or written.</summary>
[InitializeOnLoad] public static class DistrictArtAudit {
 const string Pending="Jimothy.DistrictArtAudit";static GameSession game;static Camera camera;static RenderTexture target;static int shot=-1;static double readyAt;static string folder;
 static DistrictArtAudit(){EditorApplication.playModeStateChanged+=OnMode;if(SessionState.GetBool(Pending,false))EditorApplication.update+=Tick;}
 [MenuItem("Jimothy/Capture shop identities and bell park")]
 public static void Run(){if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop play mode before isolated art audit.");EditorSceneManager.OpenScene("Assets/Jimothy/Scenes/Ballard.unity");SessionState.SetBool(Pending,true);EditorApplication.isPlaying=true;}
 static void OnMode(PlayModeStateChange mode){if(!SessionState.GetBool(Pending,false))return;if(mode==PlayModeStateChange.EnteredPlayMode){shot=-1;EditorApplication.update-=Tick;EditorApplication.update+=Tick;}}
 static void Tick(){
  if(!SessionState.GetBool(Pending,false)||!EditorApplication.isPlaying)return;
  try {
   if(shot<0){game=UnityEngine.Object.FindFirstObjectByType<GameSession>();if(!game)return;game.SuppressSaving=true;game.NewGame();game.Player.enabled=false;foreach(var canvas in game.GetComponentsInChildren<Canvas>())canvas.enabled=false;camera=Camera.main;target=new RenderTexture(1920,1080,24,RenderTextureFormat.DefaultHDR){name="District art review",antiAliasing=1};target.Create();camera.targetTexture=target;camera.aspect=16f/9;camera.fieldOfView=56;folder=Path.GetFullPath("../PlaytestCaptures/District");Directory.CreateDirectory(folder);shot=0;Pose();return;}
   if(EditorApplication.timeSinceStartup<readyAt)return;
   string filename=shot<10?"shop-"+(shot<5?"west-":"east-")+(shot%5)+".png":shot==10?"bell-park-overview.png":"brick-road-bend.png";
   // Explicit URP request supports both normal Editor and graphics-enabled batchmode.
   RenderPipeline.SubmitRenderRequest(camera,new RenderPipeline.StandardRequest{destination=target});Capture(Path.Combine(folder,filename));
   shot++;if(shot>=12){File.WriteAllText(Path.Combine(folder,"capture-note.txt"),"Twelve actual Unity camera renders. User save suppressed; player disabled solely to pose review camera. Captures are art inspection views, not traversal or playability proof. Visual legibility requires review of images.");Finish(0);return;}Pose();
  }catch(Exception ex){Debug.LogException(ex);folder=Path.GetFullPath("../PlaytestCaptures/District");Directory.CreateDirectory(folder);File.WriteAllText(Path.Combine(folder,"capture-error.txt"),ex.ToString());Finish(1);}
 }
 static void Pose(){
  if(shot<10){int side=shot<5?-1:1;float z=-30+(shot%5)*16;camera.transform.position=new Vector3(side*1.0f,3.8f,z+.35f);camera.transform.LookAt(new Vector3(side*9.4f,3.7f,z));game.Player.Teleport(new Vector3(side*6.9f,.4f,z));}
  else if(shot==10){camera.transform.position=new Vector3(-25,14,-32);camera.transform.LookAt(new Vector3(-12,3,-52));game.Player.Teleport(new Vector3(-12,.5f,-52));}
  else {camera.transform.position=new Vector3(25,11,-42);camera.transform.LookAt(new Vector3(1,1.4f,-57));game.Player.Teleport(new Vector3(4,.5f,-51));}
  readyAt=EditorApplication.timeSinceStartup+1.4;
 }
 static void Capture(string path){var ldr=RenderTexture.GetTemporary(1920,1080,0,RenderTextureFormat.ARGB32,RenderTextureReadWrite.sRGB);var previous=RenderTexture.active;Texture2D image=null;try{Graphics.Blit(target,ldr);RenderTexture.active=ldr;image=new Texture2D(1920,1080,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,1920,1080),0,0);image.Apply();File.WriteAllBytes(path,image.EncodeToPNG());}finally{RenderTexture.active=previous;if(image)UnityEngine.Object.Destroy(image);RenderTexture.ReleaseTemporary(ldr);}}
 static void Finish(int code){if(camera)camera.targetTexture=null;if(target){target.Release();UnityEngine.Object.Destroy(target);target=null;}Debug.Log("DISTRICT_ART_AUDIT_COMPLETE "+code+" "+folder);SessionState.SetBool(Pending,false);EditorApplication.update-=Tick;EditorApplication.isPlaying=false;if(Application.isBatchMode)EditorApplication.Exit(code);}
}
}
