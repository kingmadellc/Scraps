using UnityEngine;
using UnityEngine.UI;
namespace Jimothy {
public partial class GameUI {
 System.Action helpReturn;
 void ShowControlsFromPause(){helpReturn=ShowPause;ShowTouchHelp();}
 void ShowControlsFromSettings(bool inGame){helpReturn=()=>Settings(inGame);ShowTouchHelp();}
 bool shownMoveHint;Text moveHint;float moveHintUntil;
 static Sprite disc;Button touchContext,touchFood;Text touchContextLabel;
 static Sprite Disc(){if(disc)return disc;var tex=new Texture2D(128,128,TextureFormat.RGBA32,false);tex.wrapMode=TextureWrapMode.Clamp;var pixels=new Color32[128*128];for(int y=0;y<128;y++)for(int x=0;x<128;x++)pixels[y*128+x]=new Color(1,1,1,Mathf.Clamp01(63-Vector2.Distance(new(x,y),new(63.5f,63.5f))));tex.SetPixels32(pixels);tex.Apply(false,true);disc=Sprite.Create(tex,new Rect(0,0,128,128),new(.5f,.5f));return disc;}
 RectTransform TouchRect(string name,Transform parent,Vector2 anchor,Vector2 offset,Vector2 size){var r=Rect(name,parent,anchor,anchor);r.anchoredPosition=offset;r.sizeDelta=size;return r;}
 Button TouchAction(string name,string glyph,Vector2 anchor,Vector2 offset,float diameter,UnityEngine.Events.UnityAction action){var r=TouchRect(name,page.transform,anchor,offset,Vector2.one*diameter);var image=r.gameObject.AddComponent<Image>();image.sprite=Disc();image.color=new(.13f,.27f,.23f,.88f);var button=r.gameObject.AddComponent<Button>();button.targetGraphic=image;button.onClick.AddListener(action);var colors=button.colors;colors.pressedColor=new(1,.78f,.43f);colors.highlightedColor=Color.white;colors.disabledColor=new(.4f,.4f,.4f,.5f);button.colors=colors;var icon=Rect(glyph,r,new(.06f,.06f),new(.94f,.94f)).gameObject.AddComponent<TouchGlyph>();icon.symbol=glyph;icon.color=cream;icon.raycastTarget=false;return button;}
 Text TouchCaption(Transform parent,string text,float y,float width=230){var r=TouchRect(text,parent,new(.5f,.5f),new(0,y),new(width,28));var t=r.gameObject.AddComponent<Text>();t.text=text;t.font=font;t.fontSize=16;t.color=cream;t.alignment=TextAnchor.MiddleCenter;t.raycastTarget=false;return t;}
 void BuildTouchControls(){
  // Large transparent surfaces own camera/movement fingers, behind the visible actions.
  var look=Panel(page.transform,Color.clear,new(.40f,.05f),new(.99f,.76f));look.name="Camera gesture surface";var cameraPad=look.gameObject.AddComponent<TouchPad>();cameraPad.motor=game.Player;cameraPad.orbit=true;look.transform.SetAsFirstSibling();
  var movement=Panel(page.transform,Color.clear,new(.02f,.035f),new(.38f,.39f));movement.name="Floating movement surface";movement.transform.SetAsFirstSibling();var pad=movement.gameObject.AddComponent<TouchPad>();pad.motor=game.Player;pad.floating=true;
  var baseRect=TouchRect("Movement ring",movement.transform,new(.5f,.5f),Vector2.zero,new(166,166));var baseImage=baseRect.gameObject.AddComponent<Image>();baseImage.sprite=Disc();baseImage.color=new(.06f,.13f,.12f,.45f);baseImage.raycastTarget=false;var ring=Rect("Movement outline",baseRect,Vector2.zero,Vector2.one).gameObject.AddComponent<TouchGlyph>();ring.symbol="ring";ring.color=new(.8f,.86f,.78f,.55f);ring.raycastTarget=false;pad.ring=baseRect;
  var thumb=TouchRect("Movement thumb",movement.transform,new(.5f,.5f),Vector2.zero,new(64,64));var thumbImage=thumb.gameObject.AddComponent<Image>();thumbImage.sprite=Disc();thumbImage.color=new(.77f,.85f,.71f,.85f);thumbImage.raycastTarget=false;pad.thumb=thumb;
  var jump=TouchAction("Jump and trick","jump",new(1,0),new(-116,120),148,()=>{});var gesture=jump.gameObject.AddComponent<TouchTrickGesture>();gesture.motor=game.Player;gesture.fill=jump.GetComponent<Image>();gesture.feedback=TouchCaption(jump.transform,"JUMP",-102,290);gesture.feedback.rectTransform.anchoredPosition+=Vector2.left*38;
  touchContext=TouchAction("Nearby interaction","search",new(1,0),new(-286,113),100,()=>{if(game.AtHome)ShowDen();else game.Search();});touchContextLabel=TouchCaption(touchContext.transform,"SEARCH",-65,140);
  touchFood=TouchAction("Eat food","eat",new(1,0),new(-250,252),92,game.Eat);TouchCaption(touchFood.transform,"EAT",-58,100);
  TouchAction("Recenter camera","camera",new(1,0),new(-108,310),88,()=>game.Player.RecenterCamera());
  TouchAction("Travel to Den","den",Vector2.one,new(-150,-54),88,game.FastTravel);TouchAction("Pause","pause",Vector2.one,new(-48,-54),88,game.TogglePause);
  if(!shownMoveHint){shownMoveHint=true;moveHint=TouchCaption(page.transform,"LEFT THUMB MOVE · RIGHT SIDE LOOK",0,500);moveHint.rectTransform.anchorMin=moveHint.rectTransform.anchorMax=new(.5f,0);moveHint.rectTransform.anchoredPosition=new(0,30);moveHintUntil=Time.unscaledTime+10;}
 }
 void UpdateTouchControls(){if(moveHint&&Time.unscaledTime>moveHintUntil)moveHint.gameObject.SetActive(false);if(!MobileLayout||!game.Player)return;if(touchContext){bool available=game.AtHome||game.NearbyLoot;touchContext.gameObject.SetActive(available);if(available){touchContextLabel.text=game.AtHome?"STASH":"SEARCH";var glyph=touchContext.GetComponentInChildren<TouchGlyph>();string icon=game.AtHome?"stash":"search";if(glyph.symbol!=icon){glyph.symbol=icon;glyph.SetVerticesDirty();}}}if(touchFood&&game.Data!=null)touchFood.gameObject.SetActive((game.Data.health<99||game.Data.hunger<99)&&(game.Data.bag.Exists(id=>game.Items.TryGetValue(id,out var item)&&item.category=="food")||game.AtHome&&game.Data.pantry.Exists(id=>game.Items.TryGetValue(id,out var item)&&item.category=="food")));}
 void ShowTouchHelp(){Clear(green);Heading(page.transform,"CONTROLS",48,cream,new(.10f,.77f),new(.92f,.89f)).font=TitleDisplay;
  Placed(page.transform,MobileLayout?"MOVE  Left thumb. Small drags walk; longer drags run.\nLOOK  Drag open space on the right.\nJUMP  Tap the disc. Swipe up/down to flip, left/right to roll.\nLAND CLEAN  Finish your trick before touching down.":"MOVE  W/S · A/D steer. Shift walks.\nLOOK  Right-drag · JUMP  Space · TRICK  T\nSEARCH  F · EAT  E · DEN  H\nCONTROLLER  Left stick moves · Right stick looks\nSouth jumps · East flips · West searches",36,cream,new(.10f,.29f),new(.91f,.75f));
  Button(page.transform,PlayerPrefs.GetInt("touchTapTricks",0)==1?"Tricks: tap in air":"Tricks: directional swipe",new(.10f,.14f),new(.58f,.25f),()=>{PlayerPrefs.SetInt("touchTapTricks",1-PlayerPrefs.GetInt("touchTapTricks",0));PlayerPrefs.Save();ShowTouchHelp();},true);
  Button(page.transform,"Back",new(.65f,.14f),new(.9f,.25f),()=>{if(helpReturn!=null)helpReturn();else ShowPause();});
 }
}
}
