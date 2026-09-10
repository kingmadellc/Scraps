using UnityEngine;
using UnityEngine.UI;
namespace Jimothy {
public partial class GameUI {
 UnityEngine.UI.Button trickButton;
 Image detectionPanel,detectionFill,landingPanel;Text detectionText,landingTitle,landingDetail,trickTotal;float landingUntil;
 void BuildStealthHUD(){
  detectionPanel=Panel(page.transform,new Color(.025f,.065f,.06f,.92f),new(.38f,.715f),new(.765f,.777f));
  detectionPanel.sprite=Rounded();detectionPanel.type=Image.Type.Sliced;detectionPanel.raycastTarget=false;
  detectionText=Placed(detectionPanel.transform,"",22,cream,new(.04f,.28f),new(.96f,.96f));
  var track=Panel(detectionPanel.transform,new Color(.12f,.20f,.17f),new(.04f,.10f),new(.96f,.23f));track.raycastTarget=false;
  detectionFill=Panel(track.transform,coral,Vector2.zero,Vector2.one);detectionFill.raycastTarget=false;
  detectionPanel.gameObject.SetActive(false);
  trickTotal=Placed(page.transform,"",MobileLayout?20:17,mint,new(.025f,.775f),new(.35f,.831f));
  landingPanel=Panel(page.transform,new Color(.025f,.07f,.06f,.87f),new(.28f,.50f),new(.82f,.66f));
  landingPanel.sprite=Rounded();landingPanel.type=Image.Type.Sliced;landingPanel.raycastTarget=false;
  landingTitle=Placed(landingPanel.transform,"",40,cream,new(.04f,.40f),new(.96f,.98f));landingTitle.font=TitleDisplay;landingTitle.alignment=TextAnchor.MiddleCenter;
  landingDetail=Placed(landingPanel.transform,"",23,mint,new(.04f,.03f),new(.96f,.44f));landingDetail.alignment=TextAnchor.MiddleCenter;
  landingPanel.gameObject.SetActive(false);
 }
 public void LandingCelebration(string title,string detail,bool success){
  if(!landingPanel){Toast(title+" · "+detail);return;}
  landingTitle.text=title;landingTitle.color=success?new Color(1,.77f,.32f):coral;
  landingDetail.text=detail;landingUntil=Time.unscaledTime+3;landingPanel.gameObject.SetActive(true);
 }
 void UpdateStealthHUD(){
  if(trickButton&&game.Player)trickButton.interactable=game.Playing&&!game.Paused&&!game.Player.Grounded&&!game.Player.IsMantling&&!game.Player.IsTricking;
  if(trickTotal)trickTotal.text="";
  if(landingPanel&&landingPanel.gameObject.activeSelf){
   float left=landingUntil-Time.unscaledTime;
   if(left<=0)landingPanel.gameObject.SetActive(false);
   else {float intro=Mathf.Clamp01((3-left)/.20f);float bump=1+Mathf.Sin(intro*Mathf.PI)*.08f;landingPanel.rectTransform.localScale=Vector3.one*bump;}
  }
  if(!detectionPanel)return;
  NeighborAI watching=null;float highest=.06f;
  foreach(var neighbor in game.Neighbors)if(neighbor&&neighbor.Suspicion01>highest){highest=neighbor.Suspicion01;watching=neighbor;}
  detectionPanel.gameObject.SetActive(watching!=null);if(!watching)return;
  string who=watching.Kind==NeighborKind.AngryHuman?"NEIGHBOR":watching.Kind==NeighborKind.Fisherman?"FISHERMAN":watching.Kind.ToString().ToUpperInvariant();
  detectionText.text=who+" · "+(watching.IsThreat?"HIDE — BREAK SIGHT":watching.HasLineOfSight?"NOTICING YOU":"LOSING INTEREST");
  detectionFill.rectTransform.anchorMax=new Vector2(Mathf.Clamp01(highest),1);
  detectionFill.color=watching.IsThreat?coral:watching.HasLineOfSight?new Color(.98f,.72f,.25f):mint;
 }
}
}
