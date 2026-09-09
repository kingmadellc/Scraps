using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
namespace Jimothy.Editor {
[InitializeOnLoad] public static class FurVisualAudit {
 const string Key="Jimothy.FurVisualAudit";
 static GameSession game;static Camera cam;static RenderTexture target;static Renderer fur;static Material skin;static int shot=-1;static double ready;static string folder;static Vector3 origin;static string notes="";
 static FurVisualAudit(){EditorApplication.playModeStateChanged+=m=>{if(m==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool(Key,false)){shot=-1;EditorApplication.update-=Tick;EditorApplication.update+=Tick;}};if(SessionState.GetBool(Key,false))EditorApplication.update+=Tick;}
 public static void Run(){EditorSceneManager.OpenScene("Assets/Jimothy/Scenes/Ballard.unity");SessionState.SetBool(Key,true);EditorApplication.isPlaying=true;}
 static void Tick(){if(!EditorApplication.isPlaying||!SessionState.GetBool(Key,false))return;try{
 if(shot<0){game=UnityEngine.Object.FindFirstObjectByType<GameSession>();if(!game)return;game.SuppressSaving=true;game.NewGame();game.TogglePause();game.Player.enabled=false;origin=game.Player.transform.position;foreach(var c in game.GetComponentsInChildren<Canvas>())c.enabled=false;foreach(var r in game.Player.GetComponentsInChildren<Renderer>()){foreach(var m in r.sharedMaterials){if(m.shader.name=="Jimothy/RootedFur")fur=r;if(m.name.Contains("painted coat"))skin=m;}}
 if(!fur||!skin)throw new Exception("Missing coat renderers");cam=Camera.main;cam.fieldOfView=50;cam.aspect=16f/9;cam.nearClipPlane=.03f;target=new RenderTexture(1280,720,24,RenderTextureFormat.DefaultHDR){antiAliasing=2};target.Create();cam.targetTexture=target;folder=Path.GetFullPath("../PlaytestCaptures/fur-v9-diagnostic");Directory.CreateDirectory(folder);notes="Saving suppressed: "+game.SuppressSaving+"\n0 full close; 1 no geometric fur; 2 full fur no base normal; 3 full play distance; 4 no fur play distance; 5 full fur/no normal play distance; 6–11 run, 12–14 walk, 15–17 stop; actual locomotion with spring logged at six decimals. Target requests2xMSAA; no supersampling.\n";shot=0;Pose();return;}
 if(EditorApplication.timeSinceStartup<ready)return;
 if(shot>=6){cam.transform.position=game.Player.transform.position+new Vector3(.6f,.65f,1.8f);cam.transform.LookAt(game.Player.transform.position+Vector3.up*.35f);}
 RenderPipeline.SubmitRenderRequest(cam,new RenderPipeline.StandardRequest{destination=target});var ldr=RenderTexture.GetTemporary(1280,720,0,RenderTextureFormat.ARGB32,RenderTextureReadWrite.sRGB);Graphics.Blit(target,ldr);var old=RenderTexture.active;RenderTexture.active=ldr;var tex=new Texture2D(1280,720,TextureFormat.RGB24,false);tex.ReadPixels(new Rect(0,0,1280,720),0,0);tex.Apply();File.WriteAllBytes(Path.Combine(folder,$"fur-{shot:00}.png"),tex.EncodeToPNG());UnityEngine.Object.Destroy(tex);RenderTexture.active=old;RenderTexture.ReleaseTemporary(ldr);
 var props=new MaterialPropertyBlock();fur.GetPropertyBlock(props);notes+=$"frame {shot}: position={game.Player.transform.position}, spring={props.GetVector("_FurMotion").ToString("F6")}\n";
 File.WriteAllText(Path.Combine(folder,"evidence.txt"),notes);shot++;if(shot==18){File.WriteAllText(Path.Combine(folder,"evidence.txt"),notes);Finish(0);return;}Pose();
 }catch(Exception e){Debug.LogException(e);Finish(1);}}
 static void Pose(){fur.enabled=shot%3!=1||shot>=6;skin.SetFloat("_BumpScale",shot%3==2?0:.65f);cam.transform.position=origin+new Vector3(.2f,shot<3?.5f:.8f,shot<3?.9f:2.8f);cam.transform.LookAt(origin+Vector3.up*.35f);if(shot==12)game.Player.touchMove=new Vector2(0,.35f);if(shot==15)game.Player.touchMove=Vector2.zero;if(shot==6){skin.SetFloat("_BumpScale",.65f);game.TogglePause();game.Player.enabled=true;game.Player.touchMove=new Vector2(0,1);}ready=EditorApplication.timeSinceStartup+(shot>=6?.13:.2);}
 static void Finish(int code){if(game&&game.Player)game.Player.touchMove=Vector2.zero;if(cam)cam.targetTexture=null;if(target){target.Release();UnityEngine.Object.Destroy(target);}SessionState.SetBool(Key,false);EditorApplication.update-=Tick;EditorApplication.isPlaying=false;if(Application.isBatchMode)EditorApplication.Exit(code);}
}}
