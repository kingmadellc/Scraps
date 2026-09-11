#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
namespace Jimothy {
/// <summary>Development-only observed-frame input bridge. Never loads or writes the player's save.</summary>
[DefaultExecutionOrder(-100)] public sealed class AgentPlaytestDriver:MonoBehaviour {
 [Serializable] public class Command {public int id;public string action="wait";public int frames=1;public float inputX,inputY,lookX,lookY,clickX,clickY;public bool jump;}
 [Serializable] public class ButtonBounds {public string label;public bool interactable;public Rect pixels;}
 [Serializable] public class Result {
  public ButtonBounds[] buttons;public int buttonOverlapPairs,captureWidth,captureHeight;public bool mobileLayout;
  public int id,frames;public int captureSettleFrames=2;public string action,error,screenshot,objective,guidance,nearbyLoot;
  public bool playing,paused,atHome,denOpen,savingSuppressed;public Vector3 position;
  public float health,hunger,stepElapsedSeconds;public int coins;public string[] bag,trophies,decor;
  public string inputMethod="Steering input X turns the raccoon; Y moves forward/backward. Movement/jump is applied before the motor Update. The click command dispatches a uGUI raycast/pointer click at rendered pixels; other actions invoke public game methods. No teleport or hidden route information is exposed.";
 }
 GameSession game;string folder,commandPath;int lastId=-1,remaining,completedFrames;double nextPoll;DateTime stamp;
 Command active;float started;bool initialized,capturing;string commandError;
 int width=1280,height=720;bool mobile;Transform cameraWorld;
 Camera camera;RenderTexture frameTarget;Canvas[] canvases;bool canvasDirty=true;
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
 static void ConfigureMobileBeforeUI(){var args=Environment.GetCommandLineArgs();bool driver=false,mobile=false;foreach(var arg in args){driver|=arg=="--agent-playtest-dir";mobile|=arg=="--agent-mobile-ui";}if(driver&&mobile){GameUI.ForceMobileLayout=true;Screen.SetResolution(1688,780,FullScreenMode.Windowed);}}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
 static void Install(){
  var args=Environment.GetCommandLineArgs();for(int i=0;i<args.Length-1;i++)if(args[i]=="--agent-playtest-dir"){
   string dir=args[i+1];if(!Path.IsPathRooted(dir)){Debug.LogError("Agent playtest folder must be absolute.");return;}
   try{dir=Path.GetFullPath(dir);Directory.CreateDirectory(dir);}catch(Exception ex){Debug.LogError("Cannot create isolated playtest folder: "+ex.Message);return;}
   var go=new GameObject("Development agent playtest driver");DontDestroyOnLoad(go);var driver=go.AddComponent<AgentPlaytestDriver>();driver.mobile=Array.IndexOf(args,"--agent-mobile-ui")>=0;if(driver.mobile){driver.width=1688;driver.height=780;}else Screen.SetResolution(1280,720,FullScreenMode.Windowed);driver.folder=dir;driver.commandPath=Path.Combine(dir,"cmd.json");Application.runInBackground=true;Application.targetFrameRate=30;QualitySettings.vSyncCount=0;return;
  }
 }
 void Update(){
  if(string.IsNullOrEmpty(folder))return;
  if(!game)game=GameSession.Instance;
  if(!game)return;
  // Also reassert after any public command: no external tester can enable saving.
  game.SuppressSaving=true;
  if(!initialized){initialized=true;game.NewGame();Application.targetFrameRate=30;canvasDirty=true;WriteReady();}
  EnsureCaptureCamera();
  if(game.Player){game.Player.touchMove=Vector2.zero;game.Player.touchLook=Vector2.zero;}
  if(capturing)return;
  if(active==null){Poll();if(active==null)return;}
  EnsureCaptureCamera();
  if(active.action=="move"&&game.Player){
   game.Player.touchMove=Vector2.ClampMagnitude(new Vector2(active.inputX,active.inputY),1);
   game.Player.touchLook=new Vector2(Mathf.Clamp(active.lookX,-80,80),Mathf.Clamp(active.lookY,-80,80));
   if(active.jump&&completedFrames==0)game.Player.jumpRequested=true;
  }
  completedFrames++;remaining--;
  if(remaining<=0){capturing=true;StartCoroutine(FinishCapture());}
 }
 void WriteReady(){File.WriteAllText(Path.Combine(folder,"ready.json"),"{\"ready\":true,\"savingSuppressed\":true,\"width\":"+width+",\"height\":"+height+",\"protocol\":1}");}
 void Poll(){
  if(Time.realtimeSinceStartupAsDouble<nextPoll)return;nextPoll=Time.realtimeSinceStartupAsDouble+.1;
  try{
   if(!File.Exists(commandPath))return;var changed=File.GetLastWriteTimeUtc(commandPath);if(changed==stamp)return;
   var command=JsonUtility.FromJson<Command>(File.ReadAllText(commandPath));
   if(command==null||command.id<=lastId){stamp=changed;return;}
   if(!float.IsFinite(command.inputX)||!float.IsFinite(command.inputY)||!float.IsFinite(command.lookX)||!float.IsFinite(command.lookY))throw new ArgumentException("Input values must be finite.");
   stamp=changed;lastId=command.id;active=command;active.frames=Mathf.Clamp(active.frames,1,180);remaining=active.frames;completedFrames=0;started=Time.realtimeSinceStartup;commandError=null;
   ExecuteAction(command.action);canvasDirty=true;
  }catch(Exception ex){if(active!=null){commandError=ex.Message;remaining=1;}else{Debug.LogWarning("Playtest command read failed: "+ex.Message);}}
 }
 void ExecuteAction(string action){
  switch(action){
   case "click":ClickUI(active.clickX,active.clickY);break;
   case "new":game.SuppressSaving=true;game.NewGame();Application.targetFrameRate=30;break;
   case "move":case "wait":case "quit":break;
   case "search":game.Search();break;
   case "eat":game.Eat();break;
   case "home":game.FastTravel();break;
   case "den":if(game.AtHome)game.UI.OpenDenForPlaytest();break;
   case "deposit":game.Deposit();break;
   case "buy_cushion":game.BuyDecor("cushion",20);break;
   case "resume":game.Resume();break;
   case "pause":game.TogglePause();break;
   case "menu":game.Menu();break;
   default:throw new ArgumentException("Unknown action: "+action);
  }
 }
 void ClickUI(float x,float y){var events=EventSystem.current;if(!events)throw new InvalidOperationException("No UI event system");var data=new PointerEventData(events){position=new Vector2(x,y),button=PointerEventData.InputButton.Left};var hits=new List<RaycastResult>();events.RaycastAll(data,hits);if(hits.Count==0)throw new InvalidOperationException("No UI target at supplied pixel");var target=hits[0].gameObject;ExecuteEvents.ExecuteHierarchy(target,data,ExecuteEvents.pointerDownHandler);ExecuteEvents.ExecuteHierarchy(target,data,ExecuteEvents.pointerUpHandler);ExecuteEvents.ExecuteHierarchy(target,data,ExecuteEvents.pointerClickHandler);}
 void EnsureCaptureCamera(){
  var world=game.Player?game.Player.transform.parent:null;if(world!=cameraWorld||!camera){cameraWorld=world;var current=world?world.GetComponentInChildren<Camera>():Camera.main;if(!current)return;if(camera&&camera!=current)camera.targetTexture=null;camera=current;canvasDirty=true;}
  var currentCamera=camera;if(!currentCamera)return;
  if(!frameTarget){frameTarget=new RenderTexture(width,height,24,RenderTextureFormat.DefaultHDR){name="Agent observed gameplay frame",antiAliasing=2};frameTarget.Create();}
  camera.targetTexture=frameTarget;camera.aspect=(float)width/height;
  // Discovery cards are created after the HUD and may have their own canvas root.
  canvases=UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);canvasDirty=false;
  foreach(var canvas in canvases){if(!canvas.isActiveAndEnabled||canvas.renderMode==RenderMode.WorldSpace)continue;canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=camera;canvas.planeDistance=1;}
  Canvas.ForceUpdateCanvases();
 }
 IEnumerator FinishCapture(){
  // WaitForEndOfFrame can stall in batchmode. Two regular frames let URP finish the target.
  yield return null;yield return null;
  try {
   EnsureCaptureCamera();string screenshot=null;
   if(frameTarget&&camera){screenshot=Path.Combine(folder,"frame-"+active.id+".png");Capture(screenshot);}
   var state=game.Data;var result=new Result{id=active.id,action=active.action,frames=completedFrames,error=commandError,screenshot=screenshot,playing=game.Playing,paused=game.Paused,atHome=game.AtHome,denOpen=game.DenOpen,savingSuppressed=game.SuppressSaving,position=game.Player?game.Player.transform.position:Vector3.zero,health=state.health,hunger=state.hunger,coins=state.coins,objective=game.Objective,guidance=game.Guidance,nearbyLoot=game.NearbyLoot?game.NearbyLoot.name:null,bag=Names(state.bag),trophies=Names(state.trophies),decor=state.decor.ToArray(),stepElapsedSeconds=Time.realtimeSinceStartup-started};
   result.captureWidth=width;result.captureHeight=height;result.mobileLayout=mobile;result.buttons=ButtonRects(out result.buttonOverlapPairs);
   string json=JsonUtility.ToJson(result,true);string tmp=Path.Combine(folder,"result.tmp");File.WriteAllText(tmp,json);File.Copy(tmp,Path.Combine(folder,"result.json"),true);File.Delete(tmp);
  }catch(Exception ex){File.WriteAllText(Path.Combine(folder,"result.json"),JsonUtility.ToJson(new Result{id=active.id,action=active.action,error=ex.ToString(),savingSuppressed=game.SuppressSaving},true));}
  bool quit=active.action=="quit";active=null;capturing=false;if(game.Player){game.Player.touchMove=Vector2.zero;game.Player.touchLook=Vector2.zero;}if(quit)Application.Quit();
 }
 string[] Names(System.Collections.Generic.List<string> ids){var names=new string[ids.Count];for(int i=0;i<names.Length;i++)names[i]=game.Items.TryGetValue(ids[i],out var item)?item.name:ids[i];return names;}
 ButtonBounds[] ButtonRects(out int overlaps){
  var buttons=game.GetComponentsInChildren<Button>();var result=new ButtonBounds[buttons.Length];var corners=new Vector3[4];
  for(int i=0;i<buttons.Length;i++){var button=buttons[i];((RectTransform)button.transform).GetWorldCorners(corners);var low=RectTransformUtility.WorldToScreenPoint(camera,corners[0]);var high=RectTransformUtility.WorldToScreenPoint(camera,corners[2]);var text=button.GetComponentInChildren<Text>();result[i]=new ButtonBounds{label=text?text.text:button.name,interactable=button.interactable,pixels=Rect.MinMaxRect(low.x,low.y,high.x,high.y)};}
  overlaps=0;for(int i=0;i<result.Length;i++)for(int j=i+1;j<result.Length;j++)if(result[i].pixels.Overlaps(result[j].pixels))overlaps++;
  return result;
 }
 void Capture(string path){
  if(Application.isBatchMode)foreach(var portrait in UnityEngine.Object.FindObjectsByType<Camera>(FindObjectsSortMode.None)){if(portrait!=camera&&portrait.isActiveAndEnabled&&portrait.targetTexture)UnityEngine.Rendering.RenderPipeline.SubmitRenderRequest(portrait,new UnityEngine.Rendering.RenderPipeline.StandardRequest{destination=portrait.targetTexture});}
  if(Application.isBatchMode)UnityEngine.Rendering.RenderPipeline.SubmitRenderRequest(camera,new UnityEngine.Rendering.RenderPipeline.StandardRequest{destination=frameTarget});
  var ldr=RenderTexture.GetTemporary(width,height,0,RenderTextureFormat.ARGB32,RenderTextureReadWrite.sRGB);var previous=RenderTexture.active;Texture2D image=null;
  try{Graphics.Blit(frameTarget,ldr);RenderTexture.active=ldr;image=new Texture2D(width,height,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,width,height),0,0);image.Apply();File.WriteAllBytes(path,image.EncodeToPNG());}
  finally{RenderTexture.active=previous;if(image)Destroy(image);RenderTexture.ReleaseTemporary(ldr);}
 }
 void OnDestroy(){if(camera)camera.targetTexture=null;if(frameTarget){frameTarget.Release();Destroy(frameTarget);}}
}
}
#endif
