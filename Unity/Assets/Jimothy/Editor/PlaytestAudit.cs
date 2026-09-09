using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
namespace Jimothy.Editor {
/// <summary>Editor integration smoke test. Run without -nographics to capture URP output.</summary>
[InitializeOnLoad] public static class PlaytestAudit {
 const string Pending="Jimothy.AuditPending";static double started;static int stage;
 static GameSession game;static string output;static RenderTexture captureTarget;static Canvas[] canvases;
 static PlaytestAudit(){EditorApplication.playModeStateChanged+=OnMode;if(SessionState.GetBool(Pending,false))EditorApplication.update+=Tick;}
 [MenuItem("Jimothy/Verify closing time and capture")]
 public static void Run(){
  if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop the existing playtest before running the isolated audit.");
  ProjectSetup.SetupAssets();NeighborAssetSetup.Prepare();AssetDatabase.SaveAssets();
  EditorSceneManager.OpenScene("Assets/Jimothy/Scenes/Ballard.unity");
  SessionState.SetBool(Pending,true);EditorApplication.isPlaying=true;
 }
 static void OnMode(PlayModeStateChange state){
  if(!SessionState.GetBool(Pending,false))return;
  if(state==PlayModeStateChange.EnteredPlayMode){started=EditorApplication.timeSinceStartup;stage=0;EditorApplication.update-=Tick;EditorApplication.update+=Tick;}
 }
 static void Tick(){
  if(!SessionState.GetBool(Pending,false)||!EditorApplication.isPlaying)return;
  try{
   if(stage==0){
    game=UnityEngine.Object.FindFirstObjectByType<GameSession>();if(!game)return;
    // Fail closed if save isolation has not been wired; never touch existing save files.
    var flag=typeof(GameSession).GetProperty("SuppressSaving",BindingFlags.Instance|BindingFlags.Public);
    if(flag==null)throw new InvalidOperationException("GameSession.SuppressSaving is required for isolated audit.");
    flag.SetValue(game,true);game.NewGame();MobileQuality.Apply();
    captureTarget=new RenderTexture(1600,900,24,RenderTextureFormat.DefaultHDR){antiAliasing=2,name="Actual playtest camera output"};captureTarget.Create();
    Camera.main.targetTexture=captureTarget;Camera.main.aspect=1600f/900;
    canvases=game.GetComponentsInChildren<Canvas>();foreach(var canvas in canvases){canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=Camera.main;canvas.planeDistance=1;}
    output=Path.GetFullPath("../PlaytestCaptures");Directory.CreateDirectory(output);
    started=EditorApplication.timeSinceStartup;stage=1;return;
   }
   if(EditorApplication.timeSinceStartup-started<3)return;
   var camera=Camera.main;if(!camera)throw new InvalidOperationException("No main camera in running session.");
   if(stage==1){
    Capture(camera,"gameplay.png");foreach(var canvas in canvases)canvas.renderMode=RenderMode.ScreenSpaceOverlay;
    var report=new Report();var renderers=UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None);
    report.rendererCount=renderers.Length;report.materialCount=renderers.SelectMany(r=>r.sharedMaterials).Where(m=>m).Distinct().Count();
    report.triangles=UnityEngine.Object.FindObjectsByType<MeshFilter>(FindObjectsSortMode.None).Where(m=>m.sharedMesh).Sum(m=>(long)IndexCount(m.sharedMesh)/3);
    report.skinnedTriangles=UnityEngine.Object.FindObjectsByType<SkinnedMeshRenderer>(FindObjectsSortMode.None).Where(m=>m.sharedMesh).Sum(m=>(long)IndexCount(m.sharedMesh)/3);
    var lights=UnityEngine.Object.FindObjectsByType<Light>(FindObjectsSortMode.None);report.lights=lights.Length;report.enabledLights=lights.Count(l=>l.enabled);report.shadowLights=lights.Count(l=>l.shadows!=LightShadows.None);
    report.colliders=UnityEngine.Object.FindObjectsByType<Collider>(FindObjectsSortMode.None).Length;
    report.lootNodes=UnityEngine.Object.FindObjectsByType<LootNode>(FindObjectsSortMode.None).Length;
    report.roofRoutes=ClosingTimeWorld.RoofRoutes.Count;
    Physics.SyncTransforms();
    foreach(var route in ClosingTimeWorld.RoofRoutes)for(int i=0;i<route.Length;i++){
     if(!Physics.Raycast(route[i]+Vector3.up*.20f,Vector3.down,.75f,~(1<<2)))report.unsupportedRouteLandings++;
     if(i>0){var step=route[i]-route[i-1];report.maxRouteRise=Mathf.Max(report.maxRouteRise,step.y);step.y=0;report.maxRouteStride=Mathf.Max(report.maxRouteStride,step.magnitude);}
    }
    report.groundBelowPlayer=Physics.Raycast(game.Player.transform.position+Vector3.up,Vector3.down,5,~(1<<2));
    report.drawCalls=UnityEditor.UnityStats.drawCalls;report.batches=UnityEditor.UnityStats.batches;if(report.drawCalls==0)report.drawCalls=-1;if(report.batches==0)report.batches=-1;
    report.platform=Application.platform.ToString();report.graphics=SystemInfo.graphicsDeviceName;
    report.note="Editor smoke test only. Counts are scene totals; draw calls are a single editor frame, not a phone benchmark; -1 means editor counters unavailable. User save suppressed.";
    File.WriteAllText(Path.Combine(output,"runtime-audit.json"),JsonUtility.ToJson(report,true));
    game.Player.enabled=false;camera.transform.position=new Vector3(20,27,57);camera.transform.LookAt(new Vector3(0,3,10));
    started=EditorApplication.timeSinceStartup;stage=2;return;
   }
   if(stage==2){
    Capture(camera,"roof-overview.png");
    game.Player.Teleport(new Vector3(0,.45f,20));
    camera.transform.position=new Vector3(0,2,22);camera.transform.LookAt(new Vector3(-8,3,-5));
    started=EditorApplication.timeSinceStartup;stage=3;return;
   }
   if(stage==3){
    Capture(camera,"street-corner.png");
    camera.targetTexture=null;captureTarget.Release();UnityEngine.Object.Destroy(captureTarget);
    captureTarget=new RenderTexture(1688,780,24,RenderTextureFormat.DefaultHDR){antiAliasing=2,name="Mobile layout preview"};captureTarget.Create();camera.targetTexture=captureTarget;camera.aspect=1688f/780;
    GameUI.ForceMobileLayout=true;game.Player.enabled=true;game.Player.Teleport(game.Home);game.UI.ShowHUD();
    foreach(var canvas in canvases){canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=1;}
    started=EditorApplication.timeSinceStartup;stage=4;return;
   }
   if(stage==4){Capture(camera,"mobile-hud.png");AuditMobileUI("mobile-hud-ui.json");game.Player.enabled=false;game.UI.ShowMenu();started=EditorApplication.timeSinceStartup;stage=5;return;}
   if(stage==5){Capture(camera,"mobile-menu.png");AuditMobileUI("mobile-menu-ui.json");GameUI.ForceMobileLayout=false;game.UI.ShowHUD();game.Player.enabled=true;game.Player.Teleport(new Vector3(-6.459043f,.26f,22.737537f));typeof(RaccoonMotor).GetField("yaw",BindingFlags.NonPublic|BindingFlags.Instance).SetValue(game.Player,180f);started=EditorApplication.timeSinceStartup;stage=6;return;}
   if(stage==6){Capture(camera,"tree-camera-regression.png");float cameraDistance=Vector3.Distance(camera.transform.position,game.Player.transform.position+Vector3.up*.37f);File.WriteAllText(Path.Combine(output,"camera-regression.json"),"{\"distance\":"+cameraDistance.ToString(System.Globalization.CultureInfo.InvariantCulture)+",\"minimum\":1.3}");if(cameraDistance<1.3f)throw new InvalidOperationException("Camera remains too close at reported tree recovery position.");game.Player.Teleport(game.Home);game.Player.enabled=false;foreach(var canvas in canvases)canvas.enabled=false;camera.transform.position=game.Home+new Vector3(-1.1f,.63f,1.35f);camera.transform.LookAt(game.Home+Vector3.up*.32f);started=EditorApplication.timeSinceStartup;stage=7;return;}
   if(stage==7){Capture(camera,"jimothy-runtime-closeup.png");
    foreach(var rangeLight in UnityEngine.Object.FindObjectsByType<ClosingLight>(FindObjectsSortMode.None))rangeLight.enabled=false;foreach(var lamp in UnityEngine.Object.FindObjectsByType<Light>(FindObjectsSortMode.None)){if(lamp.type==LightType.Directional){lamp.color=Color.white;lamp.intensity=1.0f;}else lamp.enabled=false;}RenderSettings.ambientSkyColor=new Color(.35f,.35f,.35f);RenderSettings.ambientEquatorColor=new Color(.25f,.25f,.25f);RenderSettings.ambientGroundColor=new Color(.15f,.15f,.15f);camera.transform.position=game.Home+new Vector3(-.70f,.45f,.85f);camera.transform.LookAt(game.Home+Vector3.up*.26f);started=EditorApplication.timeSinceStartup;stage=8;return;}
   if(stage==8){Capture(camera,"jimothy-color-neutral.png");Vector3 lineup=new Vector3(0,.04f,35);game.Player.Teleport(lineup);
    int index=0;foreach(var kind in new[]{NeighborKind.Cat,NeighborKind.Dog,NeighborKind.KindHuman}){var source=game.Neighbors.First(n=>n.Kind==kind);var clone=UnityEngine.Object.Instantiate(source.gameObject,game.Player.transform.parent);foreach(var behaviour in clone.GetComponentsInChildren<MonoBehaviour>())behaviour.enabled=false;clone.transform.position=lineup+new Vector3(new[]{-1.8f,-.9f,1.1f}[index++],0,0);clone.transform.rotation=Quaternion.identity;}
    camera.transform.position=lineup+new Vector3(-.2f,1.2f,4.6f);camera.transform.LookAt(lineup+new Vector3(-.2f,.8f,0));started=EditorApplication.timeSinceStartup;stage=9;return;}
   Capture(camera,"animal-scale-lineup.png");
   var skinReport=game.Player.GetComponentsInChildren<SkinnedMeshRenderer>().Select(r=>new {name=r.name,materials=r.sharedMaterials.Select(m=>m.name+":"+m.shader.name+":"+(m.mainTexture?m.mainTexture.name:"no-map")).ToArray(),height=r.bounds.size.y,vertices=r.sharedMesh.vertexCount,colors=r.sharedMesh.colors.Length}).ToArray();File.WriteAllText(Path.Combine(output,"character-runtime.txt"),string.Join("\n",skinReport.Select(r=>r.name+" height="+r.height+" colors="+r.colors+" vertices="+r.vertices+" materials="+string.Join(",",r.materials))));
   Debug.Log("PLAYTEST_AUDIT_COMPLETE "+output);
   CleanupCapture();SessionState.SetBool(Pending,false);EditorApplication.update-=Tick;EditorApplication.isPlaying=false;if(Application.isBatchMode)EditorApplication.Exit(0);
  }catch(Exception ex){Directory.CreateDirectory(Path.GetFullPath("../PlaytestCaptures"));File.WriteAllText(Path.GetFullPath("../PlaytestCaptures/audit-error.txt"),ex.ToString());Debug.LogException(ex);CleanupCapture();SessionState.SetBool(Pending,false);EditorApplication.update-=Tick;EditorApplication.isPlaying=false;if(Application.isBatchMode)EditorApplication.Exit(1);}
 }
 [MenuItem("Jimothy/Preview mobile HUD in Play Mode")]
 public static void PreviewMobileHUD(){if(!EditorApplication.isPlaying||!GameSession.Instance)throw new InvalidOperationException("Start a play session first.");GameUI.ForceMobileLayout=true;GameSession.Instance.UI.ShowHUD();}
 [MenuItem("Jimothy/Preview desktop HUD in Play Mode")]
 public static void PreviewDesktopHUD(){if(!EditorApplication.isPlaying||!GameSession.Instance)throw new InvalidOperationException("Start a play session first.");GameUI.ForceMobileLayout=false;GameSession.Instance.UI.ShowHUD();}
 static void CleanupCapture(){GameUI.ForceMobileLayout=false;if(canvases!=null)foreach(var canvas in canvases)if(canvas)canvas.renderMode=RenderMode.ScreenSpaceOverlay;if(Camera.main)Camera.main.targetTexture=null;if(captureTarget){captureTarget.Release();UnityEngine.Object.Destroy(captureTarget);captureTarget=null;}}
 static void Capture(Camera camera,string file){
  // Read several actual game frames rendered by URP, including postprocessing and gameplay canvas.
  int width=captureTarget.width,height=captureTarget.height;var target=RenderTexture.GetTemporary(width,height,0,RenderTextureFormat.ARGB32,RenderTextureReadWrite.sRGB);
  Graphics.Blit(captureTarget,target);var previous=RenderTexture.active;RenderTexture.active=target;
  var image=new Texture2D(width,height,TextureFormat.RGB24,false);
  image.ReadPixels(new Rect(0,0,width,height),0,0);image.Apply();File.WriteAllBytes(Path.Combine(output,file),image.EncodeToPNG());
  RenderTexture.active=previous;UnityEngine.Object.Destroy(image);RenderTexture.ReleaseTemporary(target);
 }
 static void AuditMobileUI(string filename){
  var buttons=game.UI.GetComponentsInChildren<UnityEngine.UI.Button>();var errors=new System.Collections.Generic.List<string>();float minimumHeight=float.MaxValue;
  var boxes=new System.Collections.Generic.List<Rect>();var labels=new System.Collections.Generic.List<string>();
  foreach(var button in buttons){var corners=new Vector3[4];((RectTransform)button.transform).GetWorldCorners(corners);var a=RectTransformUtility.WorldToScreenPoint(Camera.main,corners[0]);var b=RectTransformUtility.WorldToScreenPoint(Camera.main,corners[2]);var box=Rect.MinMaxRect(a.x,a.y,b.x,b.y);boxes.Add(box);labels.Add(button.GetComponentInChildren<UnityEngine.UI.Text>().text);minimumHeight=Mathf.Min(minimumHeight,box.height*.5f);}
  for(int i=0;i<boxes.Count;i++)for(int j=i+1;j<boxes.Count;j++)if(boxes[i].Overlaps(boxes[j]))errors.Add(labels[i]+" overlaps "+labels[j]);
  foreach(var text in game.UI.GetComponentsInChildren<UnityEngine.UI.Text>())if(!string.IsNullOrEmpty(text.text)&&text.preferredHeight>text.rectTransform.rect.height+2)errors.Add("Text may clip: "+text.text);
  File.WriteAllText(Path.Combine(output,filename),JsonUtility.ToJson(new MobileUIReport{buttonCount=buttons.Length,minimumTargetHeightAt2x=minimumHeight,layoutIssues=errors.ToArray(),note="Simulated844x390 logical layout at2x; actual device touch validation remains required."},true));
 }
 [Serializable] class MobileUIReport{public int buttonCount;public float minimumTargetHeightAt2x;public string[] layoutIssues;public string note;}
 static long IndexCount(Mesh mesh){long total=0;for(int i=0;i<mesh.subMeshCount;i++)total+=mesh.GetIndexCount(i);return total;}
 [Serializable] class Report{public int rendererCount,materialCount,lights,enabledLights,shadowLights,colliders,lootNodes,drawCalls,batches,roofRoutes,unsupportedRouteLandings;public float maxRouteRise,maxRouteStride;public long triangles,skinnedTriangles;public bool groundBelowPlayer;public string platform,graphics,note;}
}
}
