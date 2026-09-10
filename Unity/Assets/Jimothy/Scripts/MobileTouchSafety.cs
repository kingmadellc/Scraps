using UnityEngine;
using UnityEngine.UI;
namespace Jimothy {
/// <summary>Portrait and browser focus changes stop held inputs before pausing the session.</summary>
public sealed class MobileTouchSafety:MonoBehaviour {
 GameSession game;GameObject prompt;int lastWidth,lastHeight;
 public void Initialize(GameSession session){game=session;
#if UNITY_WEBGL && !UNITY_EDITOR
 gameObject.AddComponent<BrowserState>().Initialize(session);
#endif
 if(Application.isMobilePlatform)Screen.orientation=ScreenOrientation.LandscapeLeft;}
 void Update(){if(!game||!GameUI.MobileLayout)return;bool portrait=Screen.height>Screen.width;if(portrait){StopInputs();if(game.Playing&&(!game.Paused||game.DiscoveryOpen))game.TogglePause();}if(Screen.width!=lastWidth||Screen.height!=lastHeight){lastWidth=Screen.width;lastHeight=Screen.height;StopInputs();}
  if(portrait&&!prompt){prompt=new GameObject("Turn device",typeof(Canvas),typeof(CanvasScaler));var c=prompt.GetComponent<Canvas>();c.renderMode=RenderMode.ScreenSpaceOverlay;c.sortingOrder=200;var scaler=prompt.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(810,1440);
   var panel=new GameObject("Landscape message",typeof(RectTransform),typeof(Image));panel.transform.SetParent(prompt.transform,false);var r=panel.GetComponent<RectTransform>();r.anchorMin=Vector2.zero;r.anchorMax=Vector2.one;r.offsetMin=r.offsetMax=Vector2.zero;panel.GetComponent<Image>().color=new Color(.025f,.07f,.055f,.99f);
   var label=new GameObject("Rotate device",typeof(RectTransform),typeof(Text));label.transform.SetParent(panel.transform,false);r=label.GetComponent<RectTransform>();r.anchorMin=new Vector2(.08f,.36f);r.anchorMax=new Vector2(.92f,.64f);r.offsetMin=r.offsetMax=Vector2.zero;var t=label.GetComponent<Text>();t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.fontSize=42;t.alignment=TextAnchor.MiddleCenter;t.color=new Color(1,.93f,.78f);t.text="Turn your device sideways\n\nScraps plays in landscape.\nThen tap Back to Ballard.";
  }if(prompt)prompt.SetActive(portrait);
 }
 void StopInputs(){if(game.Player){game.Player.touchMove=game.Player.touchLook=Vector2.zero;game.Player.jumpRequested=false;}foreach(var pad in GetComponentsInChildren<TouchPad>())pad.Release();foreach(var action in GetComponentsInChildren<TouchTrickGesture>())action.Release();}
 void OnApplicationFocus(bool focused){if(!focused&&game){StopInputs();if(game.Playing&&(!game.Paused||game.DiscoveryOpen))game.TogglePause();}}
 void OnDestroy(){if(prompt)Destroy(prompt);}
}
}
