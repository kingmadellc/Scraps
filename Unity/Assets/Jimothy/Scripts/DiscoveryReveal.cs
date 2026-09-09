using UnityEngine;
using UnityEngine.UI;
namespace Jimothy {
/// <summary>Brief nonblocking reward portrait. Actual mesh and warm colour; no world markers.</summary>
public sealed class DiscoveryReveal:MonoBehaviour {
 GameObject reward,overlay;Camera portrait;RenderTexture target;float elapsed;Transform model;CanvasGroup group;
 public void Show(ItemDefinition item){Clear();elapsed=0;
  reward=new GameObject("Discovery portrait scene");reward.transform.position=new Vector3(0,-2000,0);model=ItemVisuals.Create(item,reward.transform).transform;
  foreach(var t in reward.GetComponentsInChildren<Transform>())t.gameObject.layer=30;
  var cameraObject=new GameObject("Reward portrait camera");cameraObject.transform.SetParent(reward.transform,false);portrait=cameraObject.AddComponent<Camera>();portrait.cullingMask=1<<30;portrait.clearFlags=CameraClearFlags.SolidColor;portrait.backgroundColor=new Color(0,0,0,0);portrait.orthographic=true;portrait.orthographicSize=.32f;portrait.nearClipPlane=.01f;portrait.farClipPlane=3;portrait.transform.localPosition=new Vector3(.55f,.5f,-.7f);portrait.transform.LookAt(model.GetComponent<Renderer>().bounds.center);target=new RenderTexture(256,256,16);portrait.targetTexture=target;
  overlay=new GameObject("Discovery card",typeof(Canvas),typeof(CanvasScaler),typeof(CanvasGroup));var canvas=overlay.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.sortingOrder=80;var scaler=overlay.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1688,780);group=overlay.GetComponent<CanvasGroup>();group.blocksRaycasts=false;group.interactable=false;
  var panel=new GameObject("Warm reveal",typeof(RectTransform),typeof(Image));panel.transform.SetParent(overlay.transform,false);var rect=panel.GetComponent<RectTransform>();rect.anchorMin=rect.anchorMax=new Vector2(.5f,.56f);rect.sizeDelta=new Vector2(420,250);panel.GetComponent<Image>().color=new Color(.025f,.055f,.05f,.94f);panel.GetComponent<Image>().raycastTarget=false;
  var icon=new GameObject("Actual find",typeof(RectTransform),typeof(RawImage));icon.transform.SetParent(panel.transform,false);var ir=icon.GetComponent<RectTransform>();ir.sizeDelta=new Vector2(140,140);ir.anchoredPosition=new Vector2(0,20);icon.GetComponent<RawImage>().texture=target;icon.GetComponent<RawImage>().raycastTarget=false;
  Label(panel.transform,(item.rarity??"common").ToUpperInvariant()+" FIND",new Vector2(0,108),17,new Color(1,.73f,.32f));Label(panel.transform,item.name,new Vector2(0,-60),25,new Color(1,.91f,.72f));
  Label(panel.transform,item.category=="valuable"?"+"+item.value+" shinies":"Tucked into your pockets",new Vector2(0,-100),17,new Color(.68f,.79f,.72f));
 }
 void Label(Transform parent,string text,Vector2 position,int size,Color color){var o=new GameObject(text,typeof(RectTransform),typeof(Text));o.transform.SetParent(parent,false);var r=o.GetComponent<RectTransform>();r.sizeDelta=new Vector2(405,45);r.anchoredPosition=position;var t=o.GetComponent<Text>();t.text=text;t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.fontSize=size;t.alignment=TextAnchor.MiddleCenter;t.color=color;t.raycastTarget=false;}
 void Update(){if(!overlay)return;var g=GameSession.Instance;if(!g||!g.Playing||g.Paused){Clear();return;}elapsed+=Time.unscaledDeltaTime;model.localRotation=Quaternion.Euler(0,elapsed*35,0);float pop=Mathf.Lerp(.75f,1,Mathf.Clamp01(elapsed/.16f));model.localScale=Vector3.one*pop;group.alpha=Mathf.Clamp01(elapsed/.10f)*Mathf.Clamp01((2.3f-elapsed)/.35f);if(elapsed>=2.3f)Clear();}
 void Clear(){if(portrait)portrait.targetTexture=null;if(target){target.Release();Destroy(target);}if(reward)Destroy(reward);if(overlay)Destroy(overlay);model=null;portrait=null;target=null;reward=null;overlay=null;}
 void OnDestroy(){Clear();}
}
}
