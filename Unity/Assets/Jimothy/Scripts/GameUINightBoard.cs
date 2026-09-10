using UnityEngine;
namespace Jimothy {
public partial class GameUI {
 public void ShowNightBoard(){
  if(!game.AtHome)return;game.OpenDen();toastUntil=0;toast.text="";toastPanel.enabled=false;Clear(new Color(.025f,.07f,.065f,.98f));var d=game.Data;
  Placed(page.transform,"ONE MORE NIGHT",52,cream,new(.07f,.83f),new(.94f,.94f)).font=TitleDisplay;
  int goal=d.runActive?d.selectedGoal:d.runNumber%3,eventId=d.runActive?d.nightEvent:ExpeditionRules.EventForRun(d.runNumber+1);
  Placed(page.transform,(d.runActive?$"OUTING {d.runNumber} IN PROGRESS":"NEXT OUTING")+"  /  "+ExpeditionRules.GoalTitle(goal),27,cream,new(.07f,.72f),new(.94f,.82f));
  Placed(page.transform,ExpeditionRules.EventTitle(eventId)+"\n"+ExpeditionLoot.EventDescription(eventId),26,mint,new(.07f,.55f),new(.94f,.71f));
  Placed(page.transform,"8 pocket slots. Bank at the Den for shinies and a +12 goal bonus.\nDeath or falling out of the world loses loose finds and loose shinies.\nSave & resume keeps this outing. Finds reset only for the next outing.",26,cream,new(.07f,.37f),new(.94f,.54f));
  Placed(page.transform,ExpeditionRules.CollectionSummary(d),24,mint,new(.07f,.20f),new(.94f,.36f));
  Button(page.transform,d.runActive?"Return to this outing":"Head out",new(.07f,.065f),new(.48f,.17f),()=>{if(d.runActive)game.Resume();else game.StartNight();},true);
  Button(page.transform,"Den & displays",new(.54f,.065f),new(.94f,.17f),ShowDen);
 }
}
}
