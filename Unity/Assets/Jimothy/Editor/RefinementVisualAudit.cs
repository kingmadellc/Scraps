using System;using System.IO;using UnityEditor;using UnityEditor.SceneManagement;using UnityEngine;
namespace Jimothy.Editor {
[InitializeOnLoad] public static class RefinementVisualAudit {
 const string Pending="Jimothy.RefinementVisualPending";static GameSession game;static RenderTexture target;static int stage;static double until;static string output;
 static RefinementVisualAudit(){EditorApplication.playModeStateChanged+=m=>{if(m==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool(Pending,false)){stage=-1;EditorApplication.update+=Tick;}};}
 public static void Run(){ProjectSetup.SetupAssets();AssetDatabase.SaveAssets();EditorSceneManager.OpenScene("Assets/Jimothy/Scenes/Ballard.unity");SessionState.SetBool(Pending,true);EditorApplication.isPlaying=true;}
 static void Tick(){try{
 if(!EditorApplication.isPlaying)return;
 if(stage==-1){game=UnityEngine.Object.FindFirstObjectByType<GameSession>();if(!game)return;game.SuppressSaving=true;game.NewGame();foreach(var c in game.GetComponentsInChildren<Canvas>())c.enabled=false;
 target=new RenderTexture(1440,900,24,RenderTextureFormat.DefaultHDR){antiAliasing=2};target.Create();Camera.main.targetTexture=target;Camera.main.aspect=1.6f;output=Path.GetFullPath("../PlaytestCaptures/v6-refinement");Directory.CreateDirectory(output);stage=0;until=EditorApplication.timeSinceStartup+2;return;}
 if(EditorApplication.timeSinceStartup<until)return;
 Capture(stage==0?"rear-gameplay.png":stage==1?"rear-close.png":"crossing-"+(stage-2)+".png");
 if(stage==5){Finish(0);return;}game.Player.enabled=false;
 if(stage==0){var p=game.Player.transform.position;Camera.main.transform.position=p+new Vector3(0,.85f,1.2f);Camera.main.transform.LookAt(p+Vector3.up*.30f);}
 else {var path=RooftopConnections.Paths[stage-1];var mid=path[path.Length/2];game.Player.Teleport(path[0]);Camera.main.transform.position=mid+new Vector3(3.2f,2.4f,-3.4f);Camera.main.transform.LookAt(mid+Vector3.up*.35f);}
 stage++;until=EditorApplication.timeSinceStartup+1;
 }catch(Exception e){Debug.LogException(e);Finish(1);}}
 static void Capture(string name){var rt=RenderTexture.GetTemporary(1440,900,0,RenderTextureFormat.ARGB32,RenderTextureReadWrite.sRGB);Graphics.Blit(target,rt);var old=RenderTexture.active;RenderTexture.active=rt;var im=new Texture2D(1440,900,TextureFormat.RGB24,false);im.ReadPixels(new Rect(0,0,1440,900),0,0);im.Apply();File.WriteAllBytes(Path.Combine(output,name),im.EncodeToPNG());RenderTexture.active=old;UnityEngine.Object.Destroy(im);RenderTexture.ReleaseTemporary(rt);}
 static void Finish(int code){SessionState.SetBool(Pending,false);EditorApplication.update-=Tick;EditorApplication.isPlaying=false;EditorApplication.Exit(code);}
}}
