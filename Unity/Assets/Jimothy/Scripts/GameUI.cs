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
 GameSession game; Canvas canvas; RectTransform safe; GameObject page; Text toast,stats,objective,searchText,guidance,denStats; Button searchButton,cushionButton,floatButton; Image health,hunger,toastPanel;static Sprite rounded; Font font; bool menuTypography; float toastUntil,nextGuideUpdate; Rect lastSafe;
 readonly Color cream=new(.95f,.93f,.84f),green=new(.045f,.12f,.105f),mint=new(.63f,.81f,.68f),coral=new(.9f,.52f,.34f);
 public void Build(GameSession session) {
  game=session;font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
  var go=new GameObject("Interface",typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));go.transform.SetParent(transform);
  canvas=go.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceOverlay;var scale=go.GetComponent<CanvasScaler>();scale.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scale.referenceResolution=new Vector2(1440,810);scale.matchWidthOrHeight=1f;
  safe=Rect("Safe area",go.transform,Vector2.zero,Vector2.one);
  if(!FindFirstObjectByType<EventSystem>())new GameObject("Input events",typeof(EventSystem),typeof(InputSystemUIInputModule));
  toastPanel=Panel(safe,new Color(.025f,.075f,.065f,.94f),new Vector2(.26f,.22f),new Vector2(.76f,.34f));toastPanel.sprite=Rounded();toastPanel.type=Image.Type.Sliced;toastPanel.raycastTarget=false;
  toast=Label(toastPanel.transform,"",MobileLayout?30:24,cream);toast.rectTransform.offsetMin=new Vector2(18,10);toast.rectTransform.offsetMax=new Vector2(-18,-10);toast.alignment=TextAnchor.MiddleCenter;toast.resizeTextForBestFit=true;toast.resizeTextMinSize=22;toast.resizeTextMaxSize=MobileLayout?30:24;toastPanel.enabled=false;
  gameObject.AddComponent<MobileTouchSafety>().Initialize(session);
  UpdateSafe();ShowMenu();
 }
 RectTransform Rect(string name,Transform parent,Vector2 min,Vector2 max) {
  var o=new GameObject(name,typeof(RectTransform));o.transform.SetParent(parent,false);var r=(RectTransform)o.transform;Anchor(r,min,max);return r;
 }
 static void Anchor(RectTransform r,Vector2 min,Vector2 max){r.anchorMin=min;r.anchorMax=max;r.offsetMin=Vector2.zero;r.offsetMax=Vector2.zero;}
 Image Panel(Transform parent,Color color,Vector2 min,Vector2 max){var r=Rect("Panel",parent,min,max);var i=r.gameObject.AddComponent<Image>();i.color=color;return i;}
 Text Label(Transform parent,string value,int size,Color color) {
  var r=Rect(value,parent,Vector2.zero,Vector2.one);var t=r.gameObject.AddComponent<Text>();t.text=value;t.font=menuTypography?GameTypography.Secondary:font;t.fontSize=MobileLayout?Mathf.RoundToInt(size*Mathf.Clamp(((float)Screen.width/Mathf.Max(1,Screen.height))/(16f/9f),.74f,1f)):size;t.color=color;t.raycastTarget=false;t.horizontalOverflow=HorizontalWrapMode.Wrap;t.verticalOverflow=VerticalWrapMode.Truncate;return t;
 }
 Text Placed(Transform parent,string text,int size,Color color,Vector2 min,Vector2 max){var t=Label(parent,text,size,color);Anchor(t.rectTransform,min,max);return t;}
 Button Button(Transform parent,string title,Vector2 min,Vector2 max,Action action,bool primary=false) {
  var p=Panel(parent,primary?mint:new Color(.06f,.16f,.14f,.94f),min,max);p.sprite=Rounded();p.type=Image.Type.Sliced;var b=p.gameObject.AddComponent<Button>();var colors=b.colors;colors.highlightedColor=new Color(.8f,.94f,.84f);colors.pressedColor=new Color(.6f,.75f,.67f);b.colors=colors;b.onClick.AddListener(()=>action());
  var t=Label(p.transform,title,MobileLayout?32:24,primary?green:cream);t.alignment=TextAnchor.MiddleCenter;if(menuTypography){t.fontSize=MobileLayout?40:28;t.resizeTextForBestFit=true;t.resizeTextMinSize=24;t.resizeTextMaxSize=t.fontSize;}return b;
 }
 void Clear(Color? background=null) {
  menuTypography=background.HasValue;
  if(page){page.SetActive(false);Destroy(page);}stats=null;denStats=null;cushionButton=null;floatButton=null;health=null;hunger=null;objective=null;searchText=null;guidance=null;searchButton=null;detectionPanel=null;detectionFill=null;landingPanel=null;trickTotal=null;trickButton=null;
  page=Rect("Screen",safe,Vector2.zero,Vector2.one).gameObject;
  if(background.HasValue){var p=page.AddComponent<Image>();p.color=background.Value;}
  Anchor(toastPanel.rectTransform,new Vector2(.26f,.22f),new Vector2(.76f,.34f));toast.fontSize=MobileLayout?30:24;
  toastPanel.transform.SetAsLastSibling();
  Anchor(toastPanel.rectTransform,background.HasValue?new Vector2(.05f,.905f):new Vector2(.26f,.22f),background.HasValue?new Vector2(.95f,.99f):new Vector2(.76f,.34f));
 }
 // Menus share the title identity; gameplay HUD retains its body font.
 Font TitleDisplay=>GameTypography.Display;
 Font TitleSmall=>GameTypography.Secondary;
 Text Heading(Transform parent,string value,int size,Color color,Vector2 min,Vector2 max){
  var t=Placed(parent,value,size,color,min,max);t.font=GameTypography.Display;
  t.resizeTextForBestFit=true;t.resizeTextMinSize=28;t.resizeTextMaxSize=t.fontSize;
  t.alignment=TextAnchor.MiddleLeft;return t;
 }
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
  var text=b.GetComponentInChildren<Text>();text.font=TitleSmall;text.resizeTextForBestFit=false;text.fontSize=MobileLayout?33:29;text.alignment=TextAnchor.MiddleLeft;text.rectTransform.offsetMin=new Vector2(28,0);text.rectTransform.offsetMax=new Vector2(-46,0);text.color=primary?green:cream;
  var mark=TitleText(b.transform,">",28,primary?green:coral,new(.88f,0),new(.98f,1));mark.alignment=TextAnchor.MiddleCenter;
  if(!string.IsNullOrEmpty(detail))TitleText(page.transform,detail,18,new Color(.75f,.80f,.70f,.85f),new(.395f,bottom+.009f),new(.57f,bottom+.073f));
  return b;
 }
 public void ShowMenu() {
  Clear(green);page.name="Scraps · Title";Hero();
  var brass=new Color(.94f,.69f,.36f);var warmCream=new Color(1f,.94f,.79f);
  var wordmark=Rect("Scraps wordmark",page.transform,new(.065f,.555f),new(.49f,.865f));
  wordmark.localRotation=Quaternion.Euler(0,0,3);
  var shadow=TitleText(wordmark,"Scraps",140,new Color(.015f,.035f,.027f,.95f),new(.012f,.225f),new(1.012f,.955f),true);
  var depth=TitleText(wordmark,"Scraps",140,new Color(.66f,.29f,.14f),new(.006f,.252f),new(1.006f,.982f),true);
  var title=TitleText(wordmark,"Scraps",140,warmCream,new(0,.275f),new(1,1.005f),true);
  var outline=title.gameObject.AddComponent<Outline>();outline.effectColor=new Color(.045f,.09f,.066f);outline.effectDistance=new Vector2(1.4f,-1.4f);
  // A compact underline echoes hand-painted travel emblems.
  Panel(wordmark,brass,new(.015f,.245f),new(.22f,.264f)).raycastTarget=false;
  Panel(wordmark,brass,new(.015f,.195f),new(.15f,.211f)).raycastTarget=false;
  bool saved=SaveStore.Exists;
  if(saved){TitleButton("Continue","",.338f,game.LoadGame,true);TitleButton("New game","",.237f,ConfirmNew);}
  else {TitleButton("New game","",.338f,game.NewGame,true);}
  TitleButton("Settings","",saved?.136f:.237f,()=>Settings(false));
  Panel(page.transform,new Color(.94f,.69f,.36f,.45f),new(.077f,.094f),new(.37f,.096f)).raycastTarget=false;
  var location=TitleText(page.transform,"",17,warmCream,new(.60f,.044f),new(.957f,.085f));location.alignment=TextAnchor.MiddleRight;
 }
 void ConfirmNew(){Clear(green);Heading(page.transform,"Start over?",48,cream,new(.15f,.63f),new(.85f,.76f));Placed(page.transform,"This replaces your saved game and den collection.",26,cream,new(.15f,.48f),new(.83f,.6f));Button(page.transform,"Start fresh",new(.15f,.28f),new(.45f,.42f),game.NewGame,true);Button(page.transform,"Keep my save",new(.5f,.28f),new(.8f,.42f),ShowMenu);}
 public void ShowHUD() {
  Clear();bool mobile=MobileLayout;
  var hud=Panel(page.transform,new Color(.025f,.08f,.065f,.88f),new(.025f,.865f),new(mobile?.36f:.30f,.975f));hud.sprite=Rounded();hud.type=Image.Type.Sliced;
  stats=Placed(hud.transform,"",mobile?23:20,cream,new(.045f,.53f),new(.97f,.96f));
  Placed(hud.transform,"HEALTH",mobile?22:14,cream,new(.045f,.18f),new(.42f,.49f));Placed(hud.transform,"FULLNESS",mobile?22:14,cream,new(.51f,.18f),new(.96f,.49f));
  health=Panel(hud.transform,coral,new(.04f,.08f),new(.45f,.16f));hunger=Panel(hud.transform,mint,new(.51f,.08f),new(.94f,.16f));
  objective=HudPill("Objective",new(mobile?.38f:.325f,.975f),cream);
  guidance=HudPill("Direction",new(mobile?.38f:.325f,.900f),mint);
  UpdateHudGuidance();
  if(!mobile){
  Button(page.transform,"II",new(mobile?.90f:.915f,mobile?.875f:.905f),new(.985f,.975f),game.TogglePause);
  Button(page.transform,"Den",new(.79f,mobile?.875f:.905f),new(.89f,.975f),game.FastTravel);
  Button(page.transform,"Eat",mobile?new(.64f,.04f):new(.81f,.055f),mobile?new(.75f,.18f):new(.89f,.12f),game.Eat);
  Button(page.transform,"Stash",mobile?new(.51f,.04f):new(.72f,.055f),mobile?new(.62f,.18f):new(.80f,.12f),()=>{if(game.AtHome)ShowDen();else Toast("Visit your den to unload finds or decorate. Tap Den when safe.");});
  searchButton=Button(page.transform,"Search",mobile?new(.77f,.04f):new(.90f,.055f),mobile?new(.88f,.18f):new(.98f,.12f),game.Search,true);searchText=searchButton.GetComponentInChildren<Text>();
  }
  BuildStealthHUD();
  if(mobile)BuildTouchControls();
 }
 Text HudPill(string name,Vector2 anchor,Color color){
  var back=Panel(page.transform,new Color(.025f,.08f,.065f,.88f),anchor,anchor);back.name=name+" cue";back.sprite=Rounded();back.type=Image.Type.Sliced;back.raycastTarget=false;
  back.rectTransform.pivot=new(0,1);back.rectTransform.sizeDelta=new(180,42);
  var label=Label(back.transform,"",MobileLayout?25:22,color);label.alignment=TextAnchor.MiddleLeft;label.rectTransform.offsetMin=new(12,6);label.rectTransform.offsetMax=new(-12,-6);return label;
 }
 void FitHudPill(Text label,string value){
  if(!label)return;var back=(RectTransform)label.transform.parent;bool visible=!string.IsNullOrWhiteSpace(value);back.gameObject.SetActive(visible);if(!visible)return;label.text=value;
  float maxWidth=safe.rect.width*.43f;float width=Mathf.Clamp(label.preferredWidth+26,100,maxWidth);back.sizeDelta=new(width,42);back.sizeDelta=new(width,Mathf.Clamp(label.preferredHeight+14,38,68));
 }
 void UpdateHudGuidance(){
  if(game.Data==null)return;
  // Stash already has a labeled contextual action: never duplicate a Den tutorial above it.
  FitHudPill(objective,game.AtHome?(game.HasLooseHaul?"Bank your haul":"Den · Safe"):game.Data.runActive?ExpeditionRules.GoalTitle(game.Data.selectedGoal)+" · "+ExpeditionRules.GoalProgress(game.Data,game.Items):"Head out");
  FitHudPill(guidance,game.AtHome?"":game.Guidance.Replace(" · "+ExpeditionRules.GoalTitle(game.Data.selectedGoal),""));
 }
 public void ShowPause(){Clear(new Color(.02f,.075f,.055f,.96f));Heading(page.transform,"PAUSED",62,cream,new(.15f,.72f),new(.85f,.86f));Placed(page.transform,$"{game.Data.coins} banked shinies  ·  Style {game.Data.trickScore:N0}  ·  Best landing {game.Data.bestLandingScore:N0}",24,mint,new(.15f,.63f),new(.90f,.70f));Button(page.transform,"Resume",new(.3f,.48f),new(.7f,.62f),game.Resume,true);Button(page.transform,"Settings",new(.15f,.31f),new(.48f,.45f),()=>Settings(true));Button(page.transform,"Controls",new(.52f,.31f),new(.85f,.45f),ShowControlsFromPause);Button(page.transform,"Save & main menu",new(.3f,.14f),new(.7f,.28f),game.Menu);}
 void Settings(bool inGame) {
  toast.text="";toastUntil=0;toastPanel.enabled=false;
  Clear(GameTypography.Forest);
  var rule=Panel(page.transform,GameTypography.Brass,new(.12f,.705f),new(.88f,.708f));rule.raycastTarget=false;
  var footer=Panel(page.transform,new Color(.88f,.65f,.34f,.35f),new(.12f,.10f),new(.88f,.102f));footer.raycastTarget=false;
  Heading(page.transform,"SETTINGS",48,GameTypography.Cream,new(.12f,.73f),new(.92f,.85f));
  Button(page.transform,(PlayerPrefs.GetInt("fps",30)==60?"✓ Performance · 60 fps":"Performance · 60 fps"),new(.14f,.54f),new(.49f,.68f),()=>{Application.targetFrameRate=60;PlayerPrefs.SetInt("fps",60);Settings(inGame);},PlayerPrefs.GetInt("fps",30)==60);
  Button(page.transform,(PlayerPrefs.GetInt("fps",30)==30?"✓ Battery saver · 30 fps":"Battery saver · 30 fps"),new(.52f,.54f),new(.87f,.68f),()=>{Application.targetFrameRate=30;PlayerPrefs.SetInt("fps",30);Settings(inGame);},PlayerPrefs.GetInt("fps",30)==30);
  Button(page.transform,AudioListener.volume>0?"Sound: on":"Sound: off",new(.14f,.35f),new(.49f,.49f),()=>{AudioListener.volume=AudioListener.volume>0?0:1;PlayerPrefs.SetFloat("volume",AudioListener.volume);Settings(inGame);});
  Button(page.transform,"Controls",new(.52f,.35f),new(.87f,.49f),()=>ShowControlsFromSettings(inGame));
#if UNITY_EDITOR || DEVELOPMENT_BUILD
  Button(page.transform,PlayerPrefs.GetInt("playtestStats",0)==1?"Performance overlay: on":"Performance overlay: off",new(.52f,.15f),new(.87f,.29f),()=>{PlayerPrefs.SetInt("playtestStats",1-PlayerPrefs.GetInt("playtestStats",0));Settings(inGame);});
#endif
  Button(page.transform,"Back",new(.14f,.15f),new(.49f,.29f),()=>{if(inGame)ShowPause();else ShowMenu();});
 }
#if UNITY_EDITOR || DEVELOPMENT_BUILD
 public void OpenDenForPlaytest(){ShowDen();}
#endif
 void ShowDen() {
  if(!game.AtHome)return;game.OpenDen();Clear(new Color(.025f,.09f,.07f,.97f));
  Heading(page.transform,"THE DEN",58,cream,new(.08f,.77f),new(.90f,.9f)).font=TitleDisplay;
  denStats=Placed(page.transform,$"{game.Data.coins} shinies  ·  {game.Data.pantry.Count} finds  ·  {game.Data.trophies.Count} trophies",24,mint,new(.08f,.67f),new(.90f,.75f));
  Button(page.transform,"Unload pockets",new(.08f,.47f),new(.44f,.61f),()=>{game.Deposit();ShowDen();},true);
  Button(page.transform,"Decorate",new(.53f,.47f),new(.91f,.61f),()=>ShowDenShop(0));
  Button(page.transform,"Eat from pantry",new(.08f,.30f),new(.44f,.44f),game.EatPantry);
  Button(page.transform,"Favorite finds",new(.53f,.30f),new(.91f,.44f),()=>ShowDenFavorites(0));
  Button(page.transform,"Back to den",new(.08f,.10f),new(.44f,.24f),game.Resume);
  Button(page.transform,"Next outing",new(.53f,.10f),new(.91f,.24f),ShowNightBoard,true);
 }
 public void ShowRunEnd(int best){Clear(green);Heading(page.transform,"NIGHT OVER",54,cream,new(.1f,.68f),new(.95f,.84f));Placed(page.transform,$"Your den and banked collection are safe.\n{game.Data.lastLoss} pocket finds and loose shinies lost.\nBest night: {best/60}m {best%60}s.",28,mint,new(.1f,.47f),new(.9f,.62f));Button(page.transform,"Next night",new(.1f,.24f),new(.49f,.38f),()=>{game.Resume();ShowNightBoard();},true);Button(page.transform,"Main menu",new(.55f,.24f),new(.90f,.38f),game.Menu);}
 public void Toast(string message){toast.text=message;toastPanel.enabled=true;toastUntil=Time.unscaledTime+4;}
 static Sprite Rounded(){
  if(rounded)return rounded;var texture=new Texture2D(64,64,TextureFormat.RGBA32,false);texture.name="UI rounded corner";texture.wrapMode=TextureWrapMode.Clamp;
  var pixels=new Color32[4096];for(int y=0;y<64;y++)for(int x=0;x<64;x++){float dx=Mathf.Max(16-x,x-47),dy=Mathf.Max(16-y,y-47);float distance=Mathf.Sqrt(Mathf.Max(0,dx)*Mathf.Max(0,dx)+Mathf.Max(0,dy)*Mathf.Max(0,dy));pixels[y*64+x]=new Color(1,1,1,Mathf.Clamp01(16-distance));}
  texture.SetPixels32(pixels);texture.Apply(false,true);rounded=Sprite.Create(texture,new Rect(0,0,64,64),new Vector2(.5f,.5f),100,0,SpriteMeshType.FullRect,new Vector4(16,16,16,16));return rounded;
 }
 void UpdateDecorButton(Button button,string id,string label,int cost){if(!button)return;bool owned=game.Data.decor.Contains(id);button.interactable=!owned;button.GetComponentInChildren<Text>().text=owned?label+" · Owned":label+" · "+cost;}
 void UpdateSafe(){lastSafe=Screen.safeArea;Anchor(safe,new(lastSafe.xMin/Screen.width,lastSafe.yMin/Screen.height),new(lastSafe.xMax/Screen.width,lastSafe.yMax/Screen.height));}
 void Update(){UpdateStealthHUD();UpdateTouchControls();if(safe&&lastSafe!=Screen.safeArea)UpdateSafe();if(toast&&Time.unscaledTime>toastUntil){toast.text="";toastPanel.enabled=false;}
  if(denStats){denStats.text=$"{game.Data.coins} shinies  ·  {game.Data.pantry.Count} finds  ·  {game.Data.trophies.Count} trophies";UpdateDecorButton(cushionButton,"cushion","Patchwork cushion",20);UpdateDecorButton(floatButton,"glass-floats","Glass float collection",35);}
  if(objective&&Time.unscaledTime>=nextGuideUpdate){nextGuideUpdate=Time.unscaledTime+.2f;UpdateHudGuidance();}
  if(searchButton){searchButton.interactable=game.NearbyLoot;searchText.text=game.NearbyLoot?"Search":"Search";}
  if(stats&&game.Data!=null){stats.text=$"POCKETS {game.Data.bag.Count}/{ExpeditionRules.BagCapacity}   ·   {game.Data.pendingCoins} loose shinies";health.rectTransform.anchorMax=new(.04f+.41f*game.Data.health/100,.16f);hunger.rectTransform.anchorMax=new(.51f+.43f*game.Data.hunger/100,.16f);}
 }
}
}
