using System;using System.IO;using System.Linq;using UnityEditor;using UnityEditor.SceneManagement;using UnityEngine;
namespace Jimothy.Editor {
[InitializeOnLoad] public static class NightLightingAudit {
 const string Pending="Jimothy.NightLightingPending";static GameSession game;static RenderTexture target;static int stage;static double until;static string output;static float[] brightness=new float[8];static Light[] practicals;
 static NightLightingAudit(){EditorApplication.playModeStateChanged+=m=>{if(m==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool(Pending,false)){stage=-1;EditorApplication.update+=Tick;}};}
 public static void Run(){ProjectSetup.SetupAssets();AssetDatabase.SaveAssets();EditorSceneManager.OpenScene("Assets/Jimothy/Scenes/Ballard.unity");SessionState.SetBool(Pending,true);EditorApplication.isPlaying=true;}
 static void Tick(){try{
  if(!EditorApplication.isPlaying)return;
  if(stage==-1){game=UnityEngine.Object.FindFirstObjectByType<GameSession>();if(!game)return;game.SuppressSaving=true;game.NewGame();foreach(var c in game.GetComponentsInChildren<Canvas>())c.enabled=false;
   foreach(var ai in UnityEngine.Object.FindObjectsByType<NeighborAI>(FindObjectsSortMode.None))ai.enabled=false;
   target=new RenderTexture(1440,900,24,RenderTextureFormat.DefaultHDR){antiAliasing=2};target.Create();Camera.main.targetTexture=target;Camera.main.aspect=1.6f;output=Path.GetFullPath("../PlaytestCaptures/v042-lighting");Directory.CreateDirectory(output);stage=0;until=EditorApplication.timeSinceStartup+3;return;}
  if(EditorApplication.timeSinceStartup<until)return;
  string[] names={"rear-gameplay","street-all-lights","street-moon-only","street-practicals-only","street-ambient-only","roof-moonlight","full-moon","garden"};brightness[stage]=Capture(names[stage]+".png");
  if(stage==7){if(brightness[1]<=brightness[2]+.001f||brightness[1]<=brightness[3]+.001f)throw new Exception("Direct-light image contribution failed");File.WriteAllText(Path.Combine(output,"light-contribution.json"),"{\"all\":"+brightness[1]+",\"moonOnly\":"+brightness[2]+",\"practicalsOnly\":"+brightness[3]+",\"ambientOnly\":"+brightness[4]+",\"passed\":true}");Finish(0);return;}
  game.Player.enabled=false;
  if(stage==0){game.Player.Teleport(new Vector3(0,.05f,20));Camera.main.transform.position=new Vector3(2,.85f,23);Camera.main.transform.LookAt(new Vector3(-2,1.6f,-4));}
  if(stage==1){practicals=UnityEngine.Object.FindObjectsByType<Light>(FindObjectsSortMode.None).Where(l=>l.type==LightType.Point).ToArray();foreach(var f in UnityEngine.Object.FindObjectsByType<ClosingLight>(FindObjectsSortMode.None))f.enabled=false;foreach(var l in practicals)l.enabled=false;}
  if(stage==2){foreach(var l in practicals)l.enabled=true;RenderSettings.sun.enabled=false;}
  if(stage==3)foreach(var l in practicals)l.enabled=false;
  if(stage==4){RenderSettings.sun.enabled=true;foreach(var f in UnityEngine.Object.FindObjectsByType<ClosingLight>(FindObjectsSortMode.None))f.enabled=true;var p=RooftopConnections.Paths[0][1];game.Player.Teleport(p);Camera.main.transform.position=p+new Vector3(2,1.7f,2);Camera.main.transform.LookAt(p+new Vector3(-4,0,-6));}
  if(stage==5){Camera.main.transform.position=new Vector3(0,12,8);Camera.main.transform.forward=-RenderSettings.sun.transform.forward;}
  if(stage==6){game.Player.Teleport(new Vector3(-12,.2f,-47));Camera.main.transform.position=new Vector3(-5,2.4f,-43);Camera.main.transform.LookAt(new Vector3(-12,3,-52));}
  stage++;until=EditorApplication.timeSinceStartup+1.2;
 }catch(Exception e){Debug.LogException(e);Finish(1);}}
 static float Capture(string name){var rt=RenderTexture.GetTemporary(1440,900,0,RenderTextureFormat.ARGB32,RenderTextureReadWrite.sRGB);Graphics.Blit(target,rt);var old=RenderTexture.active;RenderTexture.active=rt;var im=new Texture2D(1440,900,TextureFormat.RGB24,false);im.ReadPixels(new Rect(0,0,1440,900),0,0);im.Apply();File.WriteAllBytes(Path.Combine(output,name),im.EncodeToPNG());var pixels=im.GetPixels32();double sum=0;for(int y=100;y<450;y++)for(int x=100;x<1340;x++){var p=pixels[y*1440+x];sum+=(p.r*.2126+p.g*.7152+p.b*.0722)/255;}RenderTexture.active=old;UnityEngine.Object.Destroy(im);RenderTexture.ReleaseTemporary(rt);return (float)(sum/(350*1240));}
 static void Finish(int code){SessionState.SetBool(Pending,false);EditorApplication.update-=Tick;EditorApplication.isPlaying=false;EditorApplication.Exit(code);}
}}
