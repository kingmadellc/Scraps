using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
namespace Jimothy.Editor {
[InitializeOnLoad] public static class EnvironmentArtAudit {
 const string Pending="Jimothy.EnvironmentArtAudit";static int shot=-1;static Camera camera;static GameSession game;static RenderTexture target;static string folder;static double ready;
 static readonly string[] names={"avenue-gameplay","rainier-rooftop","marine-supply","northwest-kitchen","bakery","fishing-pub","fishmonger","copper-tavern","billiards","botanical-cocktails","coffee","records","yoga","pizza-kitchen","lunch-terrace","bell-garden","plane-silhouette"};
 static EnvironmentArtAudit(){EditorApplication.playModeStateChanged+=Mode;if(SessionState.GetBool(Pending,false))EditorApplication.update+=Tick;}
 public static void Run(){EditorSceneManager.OpenScene("Assets/Jimothy/Scenes/Ballard.unity");SessionState.SetBool(Pending,true);EditorApplication.isPlaying=true;}
 static void Mode(PlayModeStateChange mode){if(!SessionState.GetBool(Pending,false))return;if(mode==PlayModeStateChange.EnteredPlayMode){shot=-1;EditorApplication.update-=Tick;EditorApplication.update+=Tick;}}
 static void Tick(){if(!EditorApplication.isPlaying||!SessionState.GetBool(Pending,false))return;try{
  if(shot<0){game=UnityEngine.Object.FindFirstObjectByType<GameSession>();if(!game)return;game.SuppressSaving=true;game.NewGame();game.Player.enabled=false;foreach(var canvas in game.GetComponentsInChildren<Canvas>())canvas.enabled=false;camera=Camera.main;camera.aspect=16f/9;target=new RenderTexture(1600,900,24,RenderTextureFormat.DefaultHDR){antiAliasing=2};target.Create();camera.targetTexture=target;folder=Path.GetFullPath("../PlaytestCaptures/environment-v050");Directory.CreateDirectory(folder);shot=0;Pose();return;}
  if(EditorApplication.timeSinceStartup<ready)return;
  RenderPipeline.SubmitRenderRequest(camera,new RenderPipeline.StandardRequest{destination=target});var ldr=RenderTexture.GetTemporary(1600,900,0,RenderTextureFormat.ARGB32,RenderTextureReadWrite.sRGB);var old=RenderTexture.active;Graphics.Blit(target,ldr);RenderTexture.active=ldr;var tex=new Texture2D(1600,900,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,1600,900),0,0);tex.Apply();File.WriteAllBytes(Path.Combine(folder,shot.ToString("00")+"-"+names[shot]+".png"),tex.EncodeToPNG());RenderTexture.active=old;RenderTexture.ReleaseTemporary(ldr);UnityEngine.Object.Destroy(tex);shot++;if(shot==names.Length){foreach(string shaderName in new[]{"Jimothy/RainierDistance","Jimothy/FoliageVisibility"})if(ShaderUtil.ShaderHasError(Shader.Find(shaderName)))throw new Exception("Shader failed: "+shaderName);File.WriteAllText(Path.Combine(folder,"method.txt"),"Actual Unity URP 1600x900, production2xMSAA. Saving suppressed. Frame00 uses production gameplay camera; remaining frames are directed asset-inspection cameras, with player placed nearby for unchanged local-light distance fading. Final flight frame stages the plane at45% of its normal path; natural timing is18s start,38s crossing,142s repeat. No fake inventory or rendered-background substitutions.");Finish(0);}else Pose();
 }catch(Exception e){Debug.LogException(e);Finish(1);}}
 static void Aim(Vector3 p,Vector3 at,float fov=58){camera.fieldOfView=fov;camera.transform.position=p;camera.transform.LookAt(at);}
 static void Pose(){
  if(shot==0){game.Player.Teleport(ClosingTimeWorld.StreetStart);game.Player.FaceDirection(180);game.Player.RecenterCamera();for(int i=0;i<60;i++)game.Player.UpdateFollowCamera(1f/60);}
  else if(shot==1){game.Player.Teleport(new(-13,7.4f,-30));Aim(new(-13,9.3f,-28),new(8,21,-180),52);}
  else if(shot>=2&&shot<=11){int index=shot-2,side=index<5?-1:1,row=index%5;float z=-30+row*16;game.Player.Teleport(new(side*6,.24f,z));Aim(new(side*3.8f,2.2f,z+4.5f),new(side*9.25f,2.65f,z),64);}
  else if(shot==12){game.Player.Teleport(new(-13,7.4f,-30));Aim(new(-13.5f,10.9f,-24.8f),new(-16.4f,7.8f,-32.2f),61);}
  else if(shot==13){game.Player.Teleport(new(13,7.4f,-30));Aim(new(13.4f,10.8f,-25.4f),new(16.5f,8.1f,-32.4f),61);}
  else if(shot==14){game.Player.Teleport(new(-13,8.3f,-14));Aim(new(-13.3f,11.5f,-8.9f),new(-16.4f,8.8f,-16.5f),61);}
  else if(shot==15){game.Player.Teleport(new(-16,.24f,-51));Aim(new(-15,2.8f,-48),new(-21,.9f,-58),61);}
  else {game.Player.Teleport(new(-13,7.4f,-30));var plane=UnityEngine.Object.FindFirstObjectByType<ShadowSeaplane>();plane.enabled=false;plane.GetComponent<Renderer>().enabled=true;plane.transform.localPosition=ShadowSeaplane.FlightPosition(.45f);plane.transform.localRotation=Quaternion.LookRotation(ShadowSeaplane.FlightPosition(.451f)-plane.transform.localPosition);Aim(new(-13,9.4f,-28),new(8,30,-180),52);}
  ready=EditorApplication.timeSinceStartup+.5;
 }
 static void Finish(int code){if(camera)camera.targetTexture=null;if(target){target.Release();UnityEngine.Object.Destroy(target);}SessionState.SetBool(Pending,false);EditorApplication.update-=Tick;Debug.Log("ENVIRONMENT_ART_AUDIT "+code);EditorApplication.isPlaying=false;EditorApplication.Exit(code);}
}}
