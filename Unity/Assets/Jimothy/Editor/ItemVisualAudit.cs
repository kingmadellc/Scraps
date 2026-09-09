using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace Jimothy.Editor {
[InitializeOnLoad] public static class ItemVisualAudit {
 static readonly string[] ids={"item_136","item_048","item_016","item_054","item_000","item_052","item_070","item_090","item_030","item_188","item_036","item_058","item_096","item_220","item_002","trophy_00","trophy_01","trophy_02","trophy_03","trophy_04","trophy_05","trophy_06"};
 const string Pending="Jimothy.ItemVisualAudit";static int stage,frame;static GameSession game;static double started;
 static ItemVisualAudit(){EditorApplication.playModeStateChanged+=Mode;if(SessionState.GetBool(Pending,false))EditorApplication.update+=Tick;}
 public static void Run(){EditorSceneManager.OpenScene("Assets/Jimothy/Scenes/Ballard.unity");SessionState.SetBool(Pending,true);EditorApplication.isPlaying=true;}
 static void Mode(PlayModeStateChange mode){if(SessionState.GetBool(Pending,false)&&mode==PlayModeStateChange.EnteredPlayMode){stage=0;started=EditorApplication.timeSinceStartup;EditorApplication.update-=Tick;EditorApplication.update+=Tick;}}
 static string Dir=>Path.GetFullPath("../PlaytestCaptures");
 static void Tick(){if(!SessionState.GetBool(Pending,false)||!EditorApplication.isPlaying)return;try{
  if(EditorApplication.timeSinceStartup-started>100)throw new TimeoutException("Reward capture timed out");
  if(stage==0){game=UnityEngine.Object.FindFirstObjectByType<GameSession>();if(!game)return;game.SuppressSaving=true;game.NewGame();GameUI.ForceMobileLayout=true;game.UI.ShowHUD();frame=Time.frameCount;stage=1;return;}
  if(Time.frameCount-frame<12)return;
  if(stage==1){Directory.CreateDirectory(Dir);ContactSheet();game.Search();frame=Time.frameCount;stage=2;return;}
  if(stage==2){CaptureCard("discovery-bagel.png");var reveal=game.GetComponent<DiscoveryReveal>();reveal.Show(game.Items["trophy_05"]);frame=Time.frameCount;stage=3;return;}
  CaptureCard("discovery-trophy.png");Finish(true,null);
 }catch(Exception e){Debug.LogException(e);Finish(false,e.ToString());}}
 static void ContactSheet(){const int tile=320;var sheet=new Texture2D(tile*6,tile*4,TextureFormat.RGB24,false);var blank=Enumerable.Repeat(new Color(.055f,.075f,.09f),tile*6*tile*4).ToArray();sheet.SetPixels(blank);
  var cameraObject=new GameObject("Item audit camera");var camera=cameraObject.AddComponent<Camera>();camera.cullingMask=1<<30;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.055f,.075f,.09f);camera.orthographic=true;camera.orthographicSize=.34f;camera.nearClipPlane=.01f;camera.farClipPlane=3;
  var rt=new RenderTexture(tile,tile,24){antiAliasing=2};camera.targetTexture=rt;var staging=new Texture2D(tile,tile,TextureFormat.RGB24,false);
  for(int i=0;i<ids.Length;i++){var root=ItemVisuals.Create(game.Items[ids[i]],null,ids[i]);root.transform.position=new Vector3(0,-2200,0);root.layer=30;camera.transform.position=root.transform.position+new Vector3(.55f,.50f,-.7f);camera.transform.LookAt(root.transform.position+Vector3.up*.15f);
   var label=new GameObject("Label");label.layer=30;label.transform.position=camera.transform.position+camera.transform.forward*.9f-camera.transform.up*.25f;label.transform.rotation=camera.transform.rotation;var text=label.AddComponent<TextMesh>();text.text=ids[i]+"\n"+ItemVisuals.ShapeKey(game.Items[ids[i]]);text.anchor=TextAnchor.MiddleCenter;text.alignment=TextAlignment.Center;text.fontSize=32;text.characterSize=.023f;text.color=Color.white;
   camera.Render();var previous=RenderTexture.active;RenderTexture.active=rt;staging.ReadPixels(new Rect(0,0,tile,tile),0,0);staging.Apply();RenderTexture.active=previous;sheet.SetPixels((i%6)*tile,(3-i/6)*tile,tile,tile,staging.GetPixels());root.SetActive(false);label.SetActive(false);UnityEngine.Object.Destroy(root);UnityEngine.Object.Destroy(label);
  }
  sheet.Apply();File.WriteAllBytes(Path.Combine(Dir,"finds-22-contact-sheet.png"),sheet.EncodeToPNG());camera.targetTexture=null;rt.Release();UnityEngine.Object.Destroy(rt);UnityEngine.Object.Destroy(sheet);UnityEngine.Object.Destroy(staging);UnityEngine.Object.Destroy(cameraObject);
 }
 static void CaptureCard(string file){var camera=Camera.main;var rt=new RenderTexture(1688,780,24){antiAliasing=2};camera.targetTexture=rt;camera.aspect=1688f/780;var canvases=UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);foreach(var c in canvases){c.renderMode=RenderMode.ScreenSpaceCamera;c.worldCamera=camera;c.planeDistance=.5f;}
  foreach(var c in UnityEngine.Object.FindObjectsByType<Camera>(FindObjectsSortMode.None))if(c!=camera&&c.targetTexture)c.Render();Canvas.ForceUpdateCanvases();camera.Render();var previous=RenderTexture.active;RenderTexture.active=rt;var image=new Texture2D(1688,780,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,1688,780),0,0);image.Apply();File.WriteAllBytes(Path.Combine(Dir,file),image.EncodeToPNG());RenderTexture.active=previous;camera.targetTexture=null;foreach(var c in canvases)c.renderMode=RenderMode.ScreenSpaceOverlay;rt.Release();UnityEngine.Object.Destroy(rt);UnityEngine.Object.Destroy(image);
 }
 static void Finish(bool pass,string error){SessionState.SetBool(Pending,false);EditorApplication.update-=Tick;GameUI.ForceMobileLayout=false;Directory.CreateDirectory(Dir);File.WriteAllText(Path.Combine(Dir,"item-visual-audit.json"),JsonUtility.ToJson(new Report{passed=pass,error=error},true));Debug.Log("ITEM_VISUAL_AUDIT "+(pass?"PASS":"FAIL"));EditorApplication.isPlaying=false;if(Application.isBatchMode)EditorApplication.Exit(pass?0:1);}
 [Serializable] class Report {public bool passed;public string error;public string note="22 original reward meshes rendered; actual successful snack collection card and directly invoked trophy preview captured. Saving suppressed. Captures require visual inspection; not native or performance proof.";}
}
}
