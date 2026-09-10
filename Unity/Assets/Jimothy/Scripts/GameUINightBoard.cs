using UnityEngine;
using System.Linq;
namespace Jimothy {
public partial class GameUI {
 public void ShowNightBoard(){
  if(!game.AtHome)return;game.OpenDen();toastUntil=0;toast.text="";toastPanel.enabled=false;Clear(new Color(.025f,.07f,.065f,.98f));var d=game.Data;
  Heading(page.transform,d.runActive?"TONIGHT":"NEXT OUTING",52,cream,new(.07f,.79f),new(.72f,.91f));
  Button(page.transform,"How it works",new(.74f,.80f),new(.94f,.90f),ShowOutingRules);
  int goal=d.runActive?d.selectedGoal:d.runNumber%3,eventId=d.runActive?d.nightEvent:ExpeditionRules.EventForRun(d.runNumber+1);
  Placed(page.transform,ExpeditionRules.GoalTitle(goal),40,cream,new(.07f,.63f),new(.94f,.75f));
  Placed(page.transform,ExpeditionRules.EventTitle(eventId)+"  ·  +12 shinies for your goal",27,mint,new(.07f,.53f),new(.94f,.62f));
  Placed(page.transform,"Fill your pockets. Bank at the Den.\nLose your life, lose your unbanked haul.",30,cream,new(.07f,.34f),new(.94f,.49f));
  Placed(page.transform,$"{d.collectionClaims.Count}/3 collections complete",25,mint,new(.07f,.22f),new(.59f,.31f));
  Button(page.transform,"Collections",new(.65f,.215f),new(.94f,.315f),ShowCollections);
  Button(page.transform,d.runActive?"Return to outing":"Head out",new(.07f,.065f),new(.48f,.17f),()=>{if(d.runActive)game.Resume();else game.StartNight();},true);
  Button(page.transform,"Back to Den",new(.54f,.065f),new(.94f,.17f),ShowDen);
 }
 void ShowCollections(){
  Clear(GameTypography.Forest);Heading(page.transform,"COLLECTIONS",48,cream,new(.08f,.80f),new(.92f,.92f));
  Placed(page.transform,"Bank each set to unlock a furnishing for your Den.",26,mint,new(.08f,.72f),new(.92f,.79f));
  // Mirrors the three launch sets settled by ExpeditionRules; names come from the item catalog.
  string[][] sets={new[]{"item_176","item_178","item_188"},new[]{"item_160","item_162","item_168"},new[]{"item_128","item_136","item_211"}};
  string[] names={"Music","Greenery","Harbor"},awards={"vinyl-wall","plant-corner","harbor-shelf"};
  for(int i=0;i<sets.Length;i++){
   float top=.69f-i*.17f;int count=sets[i].Count(game.Data.bankedDiscoveries.Contains);bool claimed=game.Data.collectionClaims.Contains(awards[i]);
   string reward=DenFurnishings.Catalog.TryGetValue(awards[i],out var furnishing)?furnishing.name:awards[i];
   Placed(page.transform,$"{names[i]}  {count}/3   ·   {reward}"+(claimed?" · Unlocked":""),29,claimed?mint:cream,new(.08f,top-.06f),new(.92f,top));
   string items=string.Join("   /   ",sets[i].Select(id=>(game.Data.bankedDiscoveries.Contains(id)?"✓ ":"")+(game.Items.TryGetValue(id,out var item)?item.name:id)));
   Placed(page.transform,items,25,mint,new(.08f,top-.13f),new(.92f,top-.065f));
  }
  Button(page.transform,"Back",new(.08f,.055f),new(.40f,.16f),ShowNightBoard,true);
 }
 void ShowOutingRules(){
  Clear(GameTypography.Forest);Heading(page.transform,"A GOOD NIGHT'S HAUL",46,cream,new(.08f,.77f),new(.94f,.90f));
  var d=game.Data;int eventId=d.runActive?d.nightEvent:ExpeditionRules.EventForRun(d.runNumber+1);
  Placed(page.transform,"8 pocket slots. Eat food to recover or bank it for your goal.\nBank at the Den to keep finds and loose shinies. Death loses both.\nA completed goal earns +12 shinies. Collections unlock furnishings.\nSave & resume keeps this outing; only a new outing resets finds.",28,cream,new(.08f,.36f),new(.93f,.74f));
  Placed(page.transform,ExpeditionLoot.EventDescription(eventId),26,mint,new(.08f,.19f),new(.93f,.34f));
  Button(page.transform,"Back",new(.08f,.055f),new(.40f,.16f),ShowNightBoard,true);
 }
}
}
