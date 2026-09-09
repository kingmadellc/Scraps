using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
namespace Jimothy.Editor {
/// <summary>Actual camera-motion roof evidence, with frozen simulation and unchanged lights.</summary>
[InitializeOnLoad] public static class DenCollectionArtAudit {
 const string Pending="Jimothy.DenCollectionArtAudit";const int Width=1280,Height=720,Count=3;
 [Serializable] class Frame {public string image;public Vector3 cameraPosition,target;}
 [Serializable] class Report {public string method="Actual Unity URP: rear gameplay view, wide room view, and shelf detail. Isolated NewGame with saving suppressed; 10 catalog trophies and12 rare curios explicitly seeded into bag then deposited through GameSession.Deposit. This is test inventory, not user inventory. Runtime collection meshes and room lights; HUD hidden for art review.";public bool savingSuppressed;public string fixture="Seeded test collection, not user inventory";public List<string> seededIds=new();public int displayed;public float maxSupportError;public List<Frame> frames=new();}
 static GameSession game;static Camera camera;static RenderTexture target;static Texture2D sheet;static int shot=-1;static double readyAt;static float previousScale=1;static string folder;static Report report;static Vector3 aim;
 static DenCollectionArtAudit(){EditorApplication.playModeStateChanged+=OnMode;if(SessionState.GetBool(Pending,false))EditorApplication.update+=Tick;}
 [MenuItem("Jimothy/Capture seeded den collection evidence")]
 public static void Run(){if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop play mode before isolated art audit.");EditorSceneManager.OpenScene("Assets/Jimothy/Scenes/Ballard.unity");SessionState.SetBool(Pending,true);EditorApplication.isPlaying=true;}
 static void OnMode(PlayModeStateChange mode){if(!SessionState.GetBool(Pending,false))return;if(mode==PlayModeStateChange.EnteredPlayMode){shot=-1;EditorApplication.update-=Tick;EditorApplication.update+=Tick;}}
 static void Tick(){if(!SessionState.GetBool(Pending,false)||!EditorApplication.isPlaying)return;try{
  if(shot<0){game=UnityEngine.Object.FindFirstObjectByType<GameSession>();if(!game)return;game.SuppressSaving=true;game.NewGame();game.Player.Teleport(ClosingTimeWorld.DenInterior);SeedCollection();game.Player.FaceDirection(0);game.Player.RecenterCamera();game.Player.enabled=false;foreach(var canvas in game.GetComponentsInChildren<Canvas>())canvas.enabled=false;
   previousScale=Time.timeScale;camera=Camera.main;camera.fieldOfView=58;camera.aspect=16f/9;target=new RenderTexture(Width,Height,24,RenderTextureFormat.DefaultHDR){name="Roof motion evidence",antiAliasing=2};target.Create();camera.targetTexture=target;folder=Path.GetFullPath("../PlaytestCaptures/den-populated");Directory.CreateDirectory(folder);sheet=new Texture2D(1440,270,TextureFormat.RGB24,false);report=new Report{savingSuppressed=game.SuppressSaving,seededIds=new List<string>(seedIds)};shot=0;Pose();return;}
  if(EditorApplication.timeSinceStartup<readyAt)return;
  CheckSupport();RenderPipeline.SubmitRenderRequest(camera,new RenderPipeline.StandardRequest{destination=target});string filename="den-"+shot.ToString("00")+".png";Capture(filename);report.frames.Add(new Frame{image=filename,cameraPosition=camera.transform.position,target=aim});shot++;
  if(shot==Count){sheet.Apply();File.WriteAllBytes(Path.Combine(folder,"contact-sheet.png"),sheet.EncodeToPNG());File.WriteAllText(Path.Combine(folder,"capture.json"),JsonUtility.ToJson(report,true));Finish(0);return;}Pose();
 }catch(Exception e){Debug.LogException(e);folder=Path.GetFullPath("../PlaytestCaptures/den-populated");Directory.CreateDirectory(folder);File.WriteAllText(Path.Combine(folder,"error.txt"),e.ToString());Finish(1);}}
 static readonly string[] seedIds={"trophy_00","trophy_01","trophy_02","trophy_03","trophy_04","trophy_05","trophy_06","trophy_07","trophy_08","trophy_09","item_208","item_211","item_217","item_220","item_224","item_228","item_230","item_231","item_232","item_233","item_237","item_239"};
 static void SeedCollection(){game.Data.bag.Clear();foreach(var id in seedIds){if(!game.Items.ContainsKey(id))throw new Exception("Fixture catalog ID missing: "+id);game.Data.bag.Add(id);}game.Deposit();if(game.Data.bag.Count!=0)throw new Exception("Deposit did not accept fixture bag");}
 static void CheckSupport(){var finds=UnityEngine.Object.FindObjectsByType<DenDisplayedFind>(FindObjectsSortMode.None);report.displayed=finds.Length;if(finds.Length!=seedIds.Length)throw new Exception("Expected 22 seeded displays; got "+finds.Length);foreach(var find in finds){var b=find.GetComponent<Renderer>().bounds;float nearest=float.MaxValue;foreach(var at in ClosingTimeWorld.DenDisplaySlots){if(Vector2.Distance(new(at.x,at.z),new(b.center.x,b.center.z))<.45f)nearest=Mathf.Min(nearest,Mathf.Abs(b.min.y-at.y-.012f));}report.maxSupportError=Mathf.Max(report.maxSupportError,nearest);}if(report.maxSupportError>.015f)throw new Exception("Displayed item support gap "+report.maxSupportError);}
 static void Pose(){
  if(shot==0){game.Player.Teleport(ClosingTimeWorld.DenInterior);game.Player.FaceDirection(0);game.Player.RecenterCamera();for(int i=0;i<60;i++)game.Player.UpdateFollowCamera(1f/60);aim=game.Player.transform.position+Vector3.up*.3f;}
  else if(shot==1){camera.transform.position=new Vector3(-18,-.1f,56.6f);aim=new Vector3(-13.8f,-1.4f,60.7f);camera.transform.LookAt(aim);}
  else{camera.transform.position=new Vector3(-12.2f,-.95f,58.2f);aim=new Vector3(-8.65f,-1.5f,59.5f);camera.transform.LookAt(aim);}
  readyAt=EditorApplication.timeSinceStartup+.8;
 }

 static void Capture(string filename){var ldr=RenderTexture.GetTemporary(Width,Height,0,RenderTextureFormat.ARGB32,RenderTextureReadWrite.sRGB);var thumbnail=RenderTexture.GetTemporary(480,270,0,RenderTextureFormat.ARGB32,RenderTextureReadWrite.sRGB);var previous=RenderTexture.active;Texture2D image=null;try{Graphics.Blit(target,ldr);RenderTexture.active=ldr;image=new Texture2D(Width,Height,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,Width,Height),0,0);image.Apply();File.WriteAllBytes(Path.Combine(folder,filename),image.EncodeToPNG());Graphics.Blit(ldr,thumbnail);RenderTexture.active=thumbnail;sheet.ReadPixels(new Rect(0,0,480,270),(shot%4)*480,0);}finally{RenderTexture.active=previous;if(image)UnityEngine.Object.Destroy(image);RenderTexture.ReleaseTemporary(ldr);RenderTexture.ReleaseTemporary(thumbnail);}}
 static void Finish(int code){Time.timeScale=previousScale;if(camera)camera.targetTexture=null;if(target){target.Release();UnityEngine.Object.Destroy(target);target=null;}if(sheet)UnityEngine.Object.Destroy(sheet);SessionState.SetBool(Pending,false);EditorApplication.update-=Tick;Debug.Log("DEN_COLLECTION_ART_CAPTURE "+code+" "+folder);EditorApplication.isPlaying=false;if(Application.isBatchMode)EditorApplication.Exit(code);}
}
}
