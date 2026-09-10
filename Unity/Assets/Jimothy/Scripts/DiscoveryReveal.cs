using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
namespace Jimothy {
/// <summary>Brief common-find cards and safe, dismissible concert-poster reveals for rare treasures.</summary>
public sealed class DiscoveryReveal:MonoBehaviour {
 GameObject reward,overlay;Camera portrait;RenderTexture target;Material portraitMaterial;float elapsed;Transform model;CanvasGroup group;RectTransform rays;bool modal;Button continueButton;
 public bool IsOpen=>overlay; public bool IsCelebrating=>overlay&&modal;public string ItemName{get;private set;}
 public static bool DeservesCelebration(ItemDefinition item)=>item.category=="trophy"||item.rarity=="rare"||item.rarity=="legendary";
 static Color Brass=>new(.94f,.69f,.36f);static Color Cream=>new(1,.94f,.79f);
 public void Show(ItemDefinition item){
  if(IsCelebrating)return;Close(false);var game=GameSession.Instance;if(!game||!game.Playing||game.Paused)return;
  modal=DeservesCelebration(item);if(modal&&!game.BeginDiscovery())return;
  elapsed=0;ItemName=item.name;
  reward=new GameObject("Discovery portrait scene");reward.transform.position=new Vector3(0,-2000,0);
  model=new GameObject("Treasure turntable").transform;model.SetParent(reward.transform,false);
  var mesh=ItemVisuals.Create(item,model);mesh.transform.localRotation=Quaternion.identity;
  var bounds=mesh.GetComponent<MeshFilter>().sharedMesh.bounds;mesh.transform.localPosition=-bounds.center;
  portraitMaterial=new Material(Resources.Load<Shader>("Shaders/FindPortrait"));mesh.GetComponent<Renderer>().sharedMaterial=portraitMaterial;
  foreach(var t in reward.GetComponentsInChildren<Transform>())t.gameObject.layer=30;
  var cameraObject=new GameObject("Reward portrait camera");cameraObject.transform.SetParent(reward.transform,false);portrait=cameraObject.AddComponent<Camera>();portrait.cullingMask=1<<30;portrait.clearFlags=CameraClearFlags.SolidColor;portrait.backgroundColor=Color.clear;portrait.orthographic=true;portrait.orthographicSize=Mathf.Max(.12f,bounds.extents.magnitude*1.15f);portrait.nearClipPlane=.01f;portrait.farClipPlane=3;portrait.transform.localPosition=new Vector3(.45f,.62f,-.8f);portrait.transform.LookAt(model.position);
  target=new RenderTexture(modal?768:256,modal?768:256,16){antiAliasing=2};portrait.targetTexture=target;
  overlay=new GameObject(modal?"Treasure celebration":"Discovery card",typeof(Canvas),typeof(CanvasScaler),typeof(CanvasGroup),typeof(GraphicRaycaster));overlay.transform.SetParent(transform,false);var canvas=overlay.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.sortingOrder=80;var scaler=overlay.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1440,810);scaler.matchWidthOrHeight=1;group=overlay.GetComponent<CanvasGroup>();group.blocksRaycasts=modal;group.interactable=modal;
  if(modal)BuildCelebration(item);else BuildCard(item);
 }
 RectTransform Area(string name,Transform parent,Vector2 min,Vector2 max){var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);r.anchorMin=min;r.anchorMax=max;r.offsetMin=r.offsetMax=Vector2.zero;return r;}
 Image Block(string name,Transform parent,Vector2 min,Vector2 max,Color color){var r=Area(name,parent,min,max);var i=r.gameObject.AddComponent<Image>();i.color=color;i.raycastTarget=false;return i;}
 Text Label(Transform parent,string value,int size,Color color,Vector2 min,Vector2 max,bool display=false,TextAnchor align=TextAnchor.MiddleLeft){var r=Area(value,parent,min,max);var t=r.gameObject.AddComponent<Text>();t.text=value;t.font=display?GameTypography.Display:GameTypography.Secondary;t.fontSize=size;t.resizeTextForBestFit=true;t.resizeTextMinSize=Mathf.Max(16,size/2);t.resizeTextMaxSize=size;t.alignment=align;t.color=color;t.raycastTarget=false;return t;}
 void Icon(Transform parent,Vector2 min,Vector2 max){var r=Area("Actual treasure model",parent,min,max);var raw=r.gameObject.AddComponent<RawImage>();raw.texture=target;raw.raycastTarget=false;var fit=r.gameObject.AddComponent<AspectRatioFitter>();fit.aspectMode=AspectRatioFitter.AspectMode.FitInParent;fit.aspectRatio=1;}
 void BuildCelebration(ItemDefinition item){
  var background=Block("Input shield",overlay.transform,Vector2.zero,Vector2.one,new(.025f,.065f,.05f,1));background.raycastTarget=true;
  var art=Area("Ballard title artwork",background.transform,Vector2.zero,Vector2.one).gameObject.AddComponent<RawImage>();art.texture=Resources.Load<Texture2D>("MenuHero-v2");art.color=new(1,1,1,.32f);art.raycastTarget=false;art.gameObject.AddComponent<TitleArtFit>();
  Block("Evergreen ink",background.transform,Vector2.zero,Vector2.one,new(.02f,.055f,.045f,.64f));
  var safe=Area("Safe poster",overlay.transform,new(.04f,.05f),new(.96f,.95f));var screen=Screen.safeArea;safe.anchorMin=new Vector2(Mathf.Max(.04f,screen.xMin/Screen.width),Mathf.Max(.05f,screen.yMin/Screen.height));safe.anchorMax=new Vector2(Mathf.Min(.96f,screen.xMax/Screen.width),Mathf.Min(.95f,screen.yMax/Screen.height));
  Block("Top brass rule",safe,new(.03f,.91f),new(.97f,.914f),Brass);
  Label(safe,"B A L L A R D   /   A F T E R   H O U R S",22,Brass,new(.03f,.925f),new(.75f,.985f));
  Label(safe,"THE NIGHT HAS A NEW HEADLINER",22,Brass,new(.04f,.77f),new(.60f,.84f));
  var word=Area("Angled concert headline",safe,new(.028f,.55f),new(.64f,.77f));word.localRotation=Quaternion.Euler(0,0,3);
  string headline=item.category=="trophy"?"Den legend!":"What a find!";
  Label(word,headline,80,new(.60f,.25f,.12f),new(.005f,-.035f),new(1.005f,.965f),true);
  Label(word,headline,80,Cream,Vector2.zero,Vector2.one,true);
  Label(safe,item.name,42,Cream,new(.04f,.37f),new(.57f,.52f),true);
  Label(safe,(item.rarity??"rare").ToUpperInvariant()+"  /  "+(item.category=="trophy"?"DEN DISPLAY TREASURE":item.category=="valuable"?item.value+" LOOSE SHINIES":"POCKET TREASURE"),23,Brass,new(.04f,.29f),new(.57f,.36f));
  Label(safe,"In your pockets. Get it back to the Den to keep it.",25,new(.72f,.83f,.74f),new(.04f,.19f),new(.60f,.27f));
  var stage=Area("Treasure spotlight",safe,new(.62f,.23f),new(.98f,.86f));rays=Area("Brass sunburst",stage,Vector2.zero,Vector2.one);rays.gameObject.AddComponent<DiscoverySunburst>().color=new(.94f,.69f,.36f,.19f);
  Icon(stage,new(.05f,.05f),new(.95f,.95f));
  var button=Block("Continue scavenging",safe,new(.04f,.045f),new(.48f,.15f),Brass);button.raycastTarget=true;continueButton=button.gameObject.AddComponent<Button>();continueButton.targetGraphic=button;continueButton.interactable=false;continueButton.onClick.AddListener(Dismiss);Label(button.transform,"Back to the night  >",30,new(.04f,.11f,.08f),new(.07f,0),new(.94f,1),true);
  Label(safe,"ACTION PAUSED  /  TREASURE NOT YET BANKED",19,Brass,new(.55f,.055f),new(.97f,.14f),false,TextAnchor.MiddleRight);
 }
 void BuildCard(ItemDefinition item){var panel=Block("Pocket find",overlay.transform,new(.33f,.60f),new(.67f,.80f),new(.025f,.065f,.05f,.96f));Block("Brass edge",panel.transform,new(0,.96f),Vector2.one,Brass);Icon(Area("Portrait slot",panel.transform,new(.02f,.06f),new(.30f,.90f)),Vector2.zero,Vector2.one);Label(panel.transform,item.name,26,Cream,new(.32f,.37f),new(.97f,.91f),true);Label(panel.transform,"POCKETED  /  BANK IT AT THE DEN",18,Brass,new(.32f,.08f),new(.98f,.39f));}
 public void Dismiss(){if(!modal||elapsed<.45f||Screen.height>Screen.width)return;Close(true);}
 public void Close(bool resume){bool wasModal=modal;modal=false;if(portrait)portrait.targetTexture=null;if(target){target.Release();Destroy(target);}if(reward){reward.SetActive(false);Destroy(reward);}if(overlay){overlay.SetActive(false);Destroy(overlay);}if(portraitMaterial)Destroy(portraitMaterial);model=null;portrait=null;target=null;reward=null;overlay=null;portraitMaterial=null;ItemName=null;if(wasModal&&GameSession.Instance)GameSession.Instance.EndDiscovery(resume);}
 void Update(){if(!overlay)return;var g=GameSession.Instance;if(!g||!g.Playing||(!modal&&g.Paused)){Close(false);return;}elapsed+=Time.unscaledDeltaTime;model.localRotation=Quaternion.Euler(0,Mathf.Sin(elapsed*.55f)*32,0);model.localScale=Vector3.one*Mathf.Lerp(.75f,1,Mathf.SmoothStep(0,1,elapsed/.3f));group.alpha=Mathf.Clamp01(elapsed/.14f);if(modal){if(rays)rays.localRotation=Quaternion.Euler(0,0,elapsed*3);continueButton.interactable=elapsed>=.45f;if(Keyboard.current?.escapeKey.wasPressedThisFrame==true){g.TogglePause();return;}if(Keyboard.current?.enterKey.wasPressedThisFrame==true||Gamepad.current?.buttonSouth.wasPressedThisFrame==true)Dismiss();}else{group.alpha*=Mathf.Clamp01((2.3f-elapsed)/.35f);if(elapsed>=2.3f)Close(false);}}
 void OnDestroy(){Close(false);}
}
/// <summary>One batched UI mesh; no particle objects, textures or flashing effects.</summary>
public sealed class DiscoverySunburst:MaskableGraphic {
 protected override void OnPopulateMesh(VertexHelper vh){vh.Clear();var r=rectTransform.rect;Vector2 c=r.center;float radius=Mathf.Min(r.width,r.height)*.48f;for(int i=0;i<18;i++){float a=i*Mathf.PI*2/18;int n=vh.currentVertCount;vh.AddVert(c,color,Vector2.zero);vh.AddVert(c+new Vector2(Mathf.Cos(a-.035f),Mathf.Sin(a-.035f))*radius,color,Vector2.zero);vh.AddVert(c+new Vector2(Mathf.Cos(a+.035f),Mathf.Sin(a+.035f))*radius,color,Vector2.zero);vh.AddTriangle(n,n+1,n+2);}}
 protected override void Awake(){base.Awake();raycastTarget=false;}
}
}
