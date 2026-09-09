using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
namespace Jimothy {
public partial class GameUI:MonoBehaviour {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
 public static bool ForceMobileLayout;
#endif
 public static bool MobileLayout {
  get {
#if UNITY_WEBGL && !UNITY_EDITOR
   return true; // Web builds expose touch controls even with iPad desktop-style user agents.
#elif UNITY_EDITOR || DEVELOPMENT_BUILD
   return Application.isMobilePlatform||ForceMobileLayout;
#else
   return Application.isMobilePlatform;
#endif
  }
 }
 GameSession game; Canvas canvas; RectTransform safe; GameObject page; Text toast,stats,objective,searchText,guidance,denStats; Button searchButton,cushionButton,floatButton; Image health,hunger,toastPanel;static Sprite rounded; Font font; float toastUntil,nextGuideUpdate; Rect lastSafe;
 readonly Color cream=new(.95f,.93f,.84f),green=new(.045f,.12f,.105f),mint=new(.63f,.81f,.68f),coral=new(.9f,.52f,.34f);
 public void Build(GameSession session) {
  game=session;font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
  var go=new GameObject("Interface",typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));go.transform.SetParent(transform);
  canvas=go.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;var scale=go.GetComponent<CanvasScaler>();scale.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scale.referenceResolution=new Vector2(1440,810);scale.matchWidthOrHeight=1f;
  safe=Rect("Safe area",go.transform,Vector2.zero,Vector2.one);
  if(!FindFirstObjectByType<EventSystem>())new GameObject("Input events",typeof(EventSystem),typeof(InputSystemUIInputModule));
  toastPanel=Panel(safe,new Color(.025f,.075f,.065f,.94f),MobileLayout?new Vector2(.22f,.25f):new Vector2(.24f,.19f),MobileLayout?new Vector2(.83f,.42f):new Vector2(.76f,.31f));toastPanel.sprite=Rounded();toastPanel.type=Image.Type.Sliced;toastPanel.raycastTarget=false;
  toast=Label(toastPanel.transform,"",MobileLayout?30:24,cream);toast.rectTransform.offsetMin=new Vector2(18,10);toast.rectTransform.offsetMax=new Vector2(-18,-10);toast.alignment=TextAnchor.MiddleCenter;toastPanel.enabled=false;
  gameObject.AddComponent<MobileTouchSafety>().Initialize(session);
  UpdateSafe();ShowMenu();
 }
 RectTransform Rect(string name,Transform parent,Vector2 min,Vector2 max) {
  var o=new GameObject(name,typeof(RectTransform));o.transform.SetParent(parent,false);var r=(RectTransform)o.transform;Anchor(r,min,max);return r;
 }
 static void Anchor(RectTransform r,Vector2 min,Vector2 max){r.anchorMin=min;r.anchorMax=max;r.offsetMin=Vector2.zero;r.offsetMax=Vector2.zero;}
 Image Panel(Transform parent,Color color,Vector2 min,Vector2 max){var r=Rect("Panel",parent,min,max);var i=r.gameObject.AddComponent<Image>();i.color=color;return i;}
 Text Label(Transform parent,string value,int size,Color color) {
  var r=Rect(value,parent,Vector2.zero,Vector2.one);var t=r.gameObject.AddComponent<Text>();t.text=value;t.font=font;t.fontSize=MobileLayout?Mathf.RoundToInt(size*Mathf.Clamp(((float)Screen.width/Mathf.Max(1,Screen.height))/(16f/9f),.74f,1f)):size;t.color=color;t.raycastTarget=false;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;return t;
 }
 Text Placed(Transform parent,string text,int size,Color color,Vector2 min,Vector2 max){var t=Label(parent,text,size,color);Anchor(t.rectTransform,min,max);return t;}
 Button Button(Transform parent,string title,Vector2 min,Vector2 max,Action action,bool primary=false) {
  var p=Panel(parent,primary?mint:new Color(.06f,.16f,.14f,.94f),min,max);p.sprite=Rounded();p.type=Image.Type.Sliced;var b=p.gameObject.AddComponent<Button>();var colors=b.colors;colors.highlightedColor=new Color(.8f,.94f,.84f);colors.pressedColor=new Color(.6f,.75f,.67f);b.colors=colors;b.onClick.AddListener(()=>action());
  var t=Label(p.transform,title,MobileLayout?32:24,primary?green:cream);t.alignment=TextAnchor.MiddleCenter;return b;
 }
 void Clear(Color? background=null) {
  if(page){page.SetActive(false);Destroy(page);}stats=null;denStats=null;cushionButton=null;floatButton=null;health=null;hunger=null;objective=null;searchText=null;guidance=null;searchButton=null;detectionPanel=null;detectionFill=null;landingPanel=null;trickTotal=null;trickButton=null;
  page=Rect("Screen",safe,Vector2.zero,Vector2.one).gameObject;
  if(background.HasValue){var p=page.AddComponent<Image>();p.color=background.Value;}
  Anchor(toastPanel.rectTransform,MobileLayout?new Vector2(.22f,.25f):new Vector2(.24f,.19f),MobileLayout?new Vector2(.83f,.42f):new Vector2(.76f,.31f));toast.fontSize=MobileLayout?30:24;
  toastPanel.transform.SetAsLastSibling();
  Anchor(toastPanel.rectTransform,background.HasValue?new Vector2(.05f,.905f):MobileLayout?new Vector2(.22f,.25f):new Vector2(.24f,.19f),background.HasValue?new Vector2(.95f,.99f):MobileLayout?new Vector2(.83f,.42f):new Vector2(.76f,.31f));
 }
 // Title-specific typography never changes the gameplay/HUD font.
 Font titleDisplay,titleSmall;
 Font TitleDisplay=>titleDisplay?titleDisplay:(titleDisplay=Resources.Load<Font>("TitleFonts/FugazOne-Regular")??font);
 Font TitleSmall=>titleSmall?titleSmall:(titleSmall=Resources.Load<Font>("TitleFonts/BarlowCondensed-SemiBold")??font);
 Text TitleText(Transform parent,string value,int size,Color color,Vector2 min,Vector2 max,bool display=false) {
  var t=Placed(parent,value,size,color,min,max);t.font=display?TitleDisplay:TitleSmall;t.alignment=TextAnchor.MiddleLeft;
  t.horizontalOverflow=HorizontalWrapMode.Overflow;t.verticalOverflow=VerticalWrapMode.Overflow;return t;
 }
 void Hero() {
  var r=Rect("Menu illustration",page.transform,Vector2.zero,Vector2.one);var image=r.gameObject.AddComponent<RawImage>();image.texture=Resources.Load<Texture2D>("MenuHero-v2");image.raycastTarget=false;
  r.gameObject.AddComponent<TitleArtFit>();
  // A continuous feathered scrim keeps the image intact instead of splitting it into columns.
  var shade=Rect("Title contrast",page.transform,Vector2.zero,Vector2.one).gameObject.AddComponent<TitleScrim>();shade.raycastTarget=false;
 }
 Button TitleButton(string label,string detail,float bottom,Action action,bool primary=false,bool enabled=true) {
  var b=Button(page.transform,label,new(.077f,bottom),new(.37f,bottom+.082f),action,primary);
  b.gameObject.name="Title action · "+label;var panel=b.GetComponent<Image>();panel.color=primary?new Color(.94f,.69f,.36f):new Color(.035f,.09f,.075f,.48f);
  var colors=b.colors;colors.normalColor=Color.white;colors.highlightedColor=new Color(1,.89f,.65f);colors.selectedColor=colors.highlightedColor;colors.pressedColor=new Color(.72f,.73f,.60f);colors.disabledColor=new Color(.6f,.65f,.59f,.45f);colors.fadeDuration=.12f;b.colors=colors;b.interactable=enabled;
  var text=b.GetComponentInChildren<Text>();text.font=TitleSmall;text.fontSize=MobileLayout?33:29;text.alignment=TextAnchor.MiddleLeft;text.rectTransform.offsetMin=new Vector2(28,0);text.rectTransform.offsetMax=new Vector2(-46,0);text.color=primary?green:cream;
  var mark=TitleText(b.transform,">",28,primary?green:coral,new(.88f,0),new(.98f,1));mark.alignment=TextAnchor.MiddleCenter;
  if(!string.IsNullOrEmpty(detail))TitleText(page.transform,detail,18,new Color(.75f,.80f,.70f,.85f),new(.395f,bottom+.009f),new(.57f,bottom+.073f));
  return b;
 }
 public void ShowMenu() {
  Clear(green);page.name="Jimothy Survival · Title";Hero();
  var brass=new Color(.94f,.69f,.36f);var warmCream=new Color(1f,.94f,.79f);
  TitleText(page.transform,"B A L L A R D   /   S E A T T L E",21,brass,new(.077f,.855f),new(.46f,.897f));
  var wordmark=Rect("Jimothy Survival wordmark",page.transform,new(.065f,.555f),new(.49f,.865f));
  wordmark.localRotation=Quaternion.Euler(0,0,3);
  var shadow=TitleText(wordmark,"Jimothy",126,new Color(.015f,.035f,.027f,.95f),new(.012f,.225f),new(1.012f,.955f),true);
  var depth=TitleText(wordmark,"Jimothy",126,new Color(.66f,.29f,.14f),new(.006f,.252f),new(1.006f,.982f),true);
  var title=TitleText(wordmark,"Jimothy",126,warmCream,new(0,.275f),new(1,1.005f),true);
  var outline=title.gameObject.AddComponent<Outline>();outline.effectColor=new Color(.045f,.09f,.066f);outline.effectDistance=new Vector2(1.4f,-1.4f);
  // A compact underline and condensed subtitle echo hand-painted travel emblems.
  Panel(wordmark,brass,new(.015f,.245f),new(.22f,.264f)).raycastTarget=false;
  Panel(wordmark,brass,new(.015f,.195f),new(.15f,.211f)).raycastTarget=false;
  var sub=TitleText(wordmark,"S U R V I V A L",40,warmCream,new(.26f,.11f),new(.93f,.32f));sub.alignment=TextAnchor.MiddleCenter;
  TitleText(page.transform,"SMALL PAWS.  BIG APPETITE.",23,brass,new(.079f,.535f),new(.47f,.582f));
  TitleText(page.transform,"The city closes. Your night begins.",24,warmCream,new(.079f,.465f),new(.49f,.514f));
  bool saved=SaveStore.Exists;
  if(saved){TitleButton("Continue adventure","Your den is waiting",.338f,game.LoadGame,true);TitleButton("New adventure","",.237f,ConfirmNew);}
  else {TitleButton("New adventure","",.338f,game.NewGame,true);TitleButton("Load adventure","No saved adventure",.237f,game.LoadGame,false,false);}
  TitleButton("Settings","",.136f,()=>Settings(false));
  Panel(page.transform,new Color(.94f,.69f,.36f,.45f),new(.077f,.094f),new(.37f,.096f)).raycastTarget=false;
  TitleText(page.transform,"A LITTLE WILDER AFTER DARK",17,new Color(.72f,.79f,.68f),new(.077f,.043f),new(.45f,.085f));
  var location=TitleText(page.transform,"BALLARD AVENUE  /  LAST CALL  /  "+Application.version,17,warmCream,new(.60f,.044f),new(.957f,.085f));location.alignment=TextAnchor.MiddleRight;
 }
 void ConfirmNew(){Clear(green);Placed(page.transform,"A fresh set of pawprints?",48,cream,new(.15f,.63f),new(.85f,.76f));Placed(page.transform,"Starting a new adventure replaces your current save and den collection.",26,cream,new(.15f,.48f),new(.83f,.6f));Button(page.transform,"Start fresh",new(.15f,.28f),new(.45f,.42f),game.NewGame,true);Button(page.transform,"Keep my adventure",new(.5f,.28f),new(.8f,.42f),ShowMenu);}
 public void ShowHUD() {
  Clear();bool mobile=MobileLayout;
  var hud=Panel(page.transform,new Color(.025f,.08f,.065f,.88f),new(.025f,mobile?.835f:.845f),new(mobile?.36f:.30f,.975f));hud.sprite=Rounded();hud.type=Image.Type.Sliced;
  stats=Placed(hud.transform,"",mobile?23:20,cream,new(.045f,.43f),new(.97f,.96f));
  Placed(hud.transform,"HEALTH",mobile?17:13,cream,new(.045f,.20f),new(.42f,.40f));Placed(hud.transform,"FULLNESS",mobile?17:13,cream,new(.51f,.20f),new(.96f,.40f));
  health=Panel(hud.transform,coral,new(.04f,.08f),new(.45f,.16f));hunger=Panel(hud.transform,mint,new(.51f,.08f),new(.94f,.16f));
  var objectiveBack=Panel(page.transform,new Color(.025f,.08f,.065f,.90f),new(mobile?.38f:.325f,mobile?.875f:.905f),new(.765f,.975f));objectiveBack.sprite=Rounded();objectiveBack.type=Image.Type.Sliced;objectiveBack.raycastTarget=false;
  var guidanceBack=Panel(page.transform,new Color(.025f,.08f,.065f,.90f),new(mobile?.38f:.325f,mobile?.79f:.85f),new(.905f,mobile?.865f:.90f));guidanceBack.sprite=Rounded();guidanceBack.type=Image.Type.Sliced;guidanceBack.raycastTarget=false;
  objective=Placed(page.transform,game.Objective,mobile?26:22,cream,new(mobile?.39f:.33f,mobile?.875f:.905f),new(.76f,.975f));
  guidance=Placed(page.transform,"",mobile?26:22,mint,new(mobile?.38f:.33f,mobile?.79f:.85f),new(.90f,mobile?.865f:.90f));
  Button(page.transform,"II",new(mobile?.90f:.915f,mobile?.875f:.905f),new(.985f,.975f),game.TogglePause);
  Button(page.transform,"Den",new(.79f,mobile?.875f:.905f),new(.89f,.975f),game.FastTravel);
  Button(page.transform,"Eat",mobile?new(.64f,.04f):new(.81f,.055f),mobile?new(.75f,.18f):new(.89f,.12f),game.Eat);
  Button(page.transform,"Stash",mobile?new(.51f,.04f):new(.72f,.055f),mobile?new(.62f,.18f):new(.80f,.12f),()=>{if(game.AtHome)ShowDen();else Toast("Visit your den to unload finds or decorate. Tap Den when safe.");});
  searchButton=Button(page.transform,"Search",mobile?new(.77f,.04f):new(.90f,.055f),mobile?new(.88f,.18f):new(.98f,.12f),game.Search,true);searchText=searchButton.GetComponentInChildren<Text>();
  BuildStealthHUD();
  if(mobile){
   trickButton=Button(page.transform,"Trick",new(.89f,.225f),new(.985f,.35f),()=>game.Player.RequestTrick());
   Button(page.transform,"Jump",new(.89f,.04f),new(.985f,.20f),()=>{},true).gameObject.AddComponent<TouchJump>().motor=game.Player;
   var zone=Panel(page.transform,new Color(0,0,0,0),new(.40f,.22f),new(.98f,.71f));var orbit=zone.gameObject.AddComponent<TouchPad>();orbit.motor=game.Player;orbit.orbit=true;zone.transform.SetAsFirstSibling();
   var stick=Panel(page.transform,new Color(.04f,.14f,.11f,.5f),new(.025f,.04f),new(.23f,.31f));stick.sprite=Rounded();stick.type=Image.Type.Sliced;var pad=stick.gameObject.AddComponent<TouchPad>();pad.motor=game.Player;
   Placed(stick.transform,"MOVE / STEER",14,mint,new(.04f,.85f),new(.96f,.98f)).alignment=TextAnchor.MiddleCenter;
   Placed(page.transform,"Drag to turn view",15,mint,new(.56f,.24f),new(.79f,.29f));
   Button(page.transform,"Center view",new(.25f,.04f),new(.39f,.14f),()=>game.Player.RecenterCamera());
   var thumb=Panel(stick.transform,new Color(.63f,.81f,.68f,.85f),new(.35f,.35f),new(.65f,.65f));thumb.sprite=Rounded();thumb.type=Image.Type.Sliced;thumb.raycastTarget=false;pad.thumb=thumb.rectTransform;
  }else Placed(page.transform,"W/S move · A/D steer · Space jump · T flip · F search · H den · Right-drag turn",18,cream,new(.025f,.018f),new(.83f,.050f));
 }
 public void ShowPause(){Clear(new Color(.02f,.075f,.055f,.96f));Placed(page.transform,"TAKE A BREATHER.",62,cream,new(.2f,.68f),new(.85f,.8f));Button(page.transform,"Back to Ballard",new(.3f,.48f),new(.7f,.62f),game.Resume,true);Button(page.transform,"Settings",new(.3f,.31f),new(.7f,.45f),()=>Settings(true));Button(page.transform,"Save & main menu",new(.3f,.14f),new(.7f,.28f),game.Menu);}
 void Settings(bool inGame) {
  Clear(green);Placed(page.transform,"MAKE YOURSELF COMFORTABLE.",48,cream,new(.12f,.73f),new(.92f,.85f));
  Button(page.transform,(PlayerPrefs.GetInt("fps",30)==60?"✓ Performance · 60 fps":"Performance · 60 fps"),new(.14f,.54f),new(.49f,.68f),()=>{Application.targetFrameRate=60;PlayerPrefs.SetInt("fps",60);Settings(inGame);},PlayerPrefs.GetInt("fps",30)==60);
  Button(page.transform,(PlayerPrefs.GetInt("fps",30)==30?"✓ Battery saver · 30 fps":"Battery saver · 30 fps"),new(.52f,.54f),new(.87f,.68f),()=>{Application.targetFrameRate=30;PlayerPrefs.SetInt("fps",30);Settings(inGame);},PlayerPrefs.GetInt("fps",30)==30);
  Button(page.transform,AudioListener.volume>0?"Sound: on":"Sound: off",new(.14f,.35f),new(.49f,.49f),()=>{AudioListener.volume=AudioListener.volume>0?0:1;PlayerPrefs.SetFloat("volume",AudioListener.volume);Settings(inGame);});
  Placed(page.transform,"Landscape play  /  Touch, keyboard & gamepad\nPickup / reward chimes enabled. Full soundscape in development.",22,mint,new(.52f,.35f),new(.89f,.47f));
  Button(page.transform,PlayerPrefs.GetInt("playtestStats",0)==1?"Playtest stats: on":"Playtest stats: off",new(.52f,.15f),new(.87f,.29f),()=>{PlayerPrefs.SetInt("playtestStats",1-PlayerPrefs.GetInt("playtestStats",0));Settings(inGame);});
  Button(page.transform,"Back",new(.14f,.15f),new(.49f,.29f),()=>{if(inGame)ShowPause();else ShowMenu();});
 }
#if UNITY_EDITOR || DEVELOPMENT_BUILD
 public void OpenDenForPlaytest(){ShowDen();}
#endif
 void ShowDen() {
  if(!game.AtHome)return;game.OpenDen();Clear(new Color(.025f,.09f,.07f,.97f));
  Placed(page.transform,"THE DEN",58,cream,new(.08f,.77f),new(.90f,.9f)).font=TitleDisplay;
  denStats=Placed(page.transform,$"{game.Data.pantry.Count} banked finds    /    {game.Data.trophies.Count} trophies banked    /    {game.Data.coins} shinies",24,mint,new(.08f,.67f),new(.90f,.75f));
  Button(page.transform,"Unload pockets",new(.08f,.47f),new(.44f,.61f),()=>{game.Deposit();ShowDen();},true);
  Button(page.transform,"Decorate your den",new(.53f,.47f),new(.91f,.61f),()=>ShowDenShop(0));
  Button(page.transform,"Eat from pantry",new(.08f,.30f),new(.44f,.44f),game.Eat);
  Button(page.transform,"Choose favorite displays",new(.53f,.30f),new(.91f,.44f),()=>ShowDenFavorites(0));
  Button(page.transform,"Back to den",new(.08f,.10f),new(.44f,.24f),game.Resume);
 }
 public void ShowRunEnd(int best){Clear(green);Placed(page.transform,"OUTFOXED. STILL ADORABLE.",54,cream,new(.1f,.68f),new(.95f,.84f));Placed(page.transform,$"Your den and banked collection are safe.\nBest survival: {best/60}m {best%60}s. Loose pocket items were lost.",28,mint,new(.1f,.47f),new(.9f,.62f));Button(page.transform,"Another night in Ballard",new(.1f,.24f),new(.49f,.38f),game.Resume,true);Button(page.transform,"Main menu",new(.55f,.24f),new(.90f,.38f),game.Menu);}
 public void Toast(string message){toast.text=message;toastPanel.enabled=true;toastUntil=Time.unscaledTime+4;}
 static Sprite Rounded(){
  if(rounded)return rounded;var texture=new Texture2D(64,64,TextureFormat.RGBA32,false);texture.name="UI rounded corner";texture.wrapMode=TextureWrapMode.Clamp;
  var pixels=new Color32[4096];for(int y=0;y<64;y++)for(int x=0;x<64;x++){float dx=Mathf.Max(16-x,x-47),dy=Mathf.Max(16-y,y-47);float distance=Mathf.Sqrt(Mathf.Max(0,dx)*Mathf.Max(0,dx)+Mathf.Max(0,dy)*Mathf.Max(0,dy));pixels[y*64+x]=new Color(1,1,1,Mathf.Clamp01(16-distance));}
  texture.SetPixels32(pixels);texture.Apply(false,true);rounded=Sprite.Create(texture,new Rect(0,0,64,64),new Vector2(.5f,.5f),100,0,SpriteMeshType.FullRect,new Vector4(16,16,16,16));return rounded;
 }
 void UpdateDecorButton(Button button,string id,string label,int cost){if(!button)return;bool owned=game.Data.decor.Contains(id);button.interactable=!owned;button.GetComponentInChildren<Text>().text=owned?label+" · Owned":label+" · "+cost;}
 void UpdateSafe(){lastSafe=Screen.safeArea;Anchor(safe,new(lastSafe.xMin/Screen.width,lastSafe.yMin/Screen.height),new(lastSafe.xMax/Screen.width,lastSafe.yMax/Screen.height));}
 void Update(){UpdateStealthHUD();if(safe&&lastSafe!=Screen.safeArea)UpdateSafe();if(toast&&Time.unscaledTime>toastUntil){toast.text="";toastPanel.enabled=false;}
  if(denStats){denStats.text=$"{game.Data.pantry.Count} banked finds    /    {game.Data.trophies.Count} trophies banked    /    {game.Data.coins} shinies";UpdateDecorButton(cushionButton,"cushion","Patchwork cushion",20);UpdateDecorButton(floatButton,"glass-floats","Glass float collection",35);}
  if(objective)objective.text=game.Objective;
  if(guidance&&Time.unscaledTime>=nextGuideUpdate){nextGuideUpdate=Time.unscaledTime+.2f;guidance.text=game.Guidance;}
  if(searchButton){searchButton.interactable=game.NearbyLoot;searchText.text=game.NearbyLoot?"Search":"Search";}
  if(stats&&game.Data!=null){stats.text=$"JIMOTHY   ·   {(int)game.Data.survivalSeconds/60:D2}:{(int)game.Data.survivalSeconds%60:D2}\n{game.Data.coins} shinies  ·  Bag {game.Data.bag.Count}/24";health.rectTransform.anchorMax=new(.04f+.41f*game.Data.health/100,.16f);hunger.rectTransform.anchorMax=new(.51f+.43f*game.Data.hunger/100,.16f);}
 }
}
}
