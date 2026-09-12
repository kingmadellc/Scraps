using System.Linq;
using UnityEngine;
using UnityEngine.UI;
namespace Jimothy {
public partial class GameUI {
 public void ShowCollector(){
  if(!game.AtHome)return;game.OpenDen();DenPageTitle("CRIMP'S COUNTER","");
  var d=game.Data;int run=d.bankedRuns,trades=d.collectorTrades;bool fresh=d.bankedRuns>0&&d.collectorLastRun<d.bankedRuns;
  var portrait=Rect("Crimp portrait",page.transform,new(.065f,.23f),new(.43f,.77f));var imageRect=Rect("Portrait image",portrait,Vector2.zero,Vector2.one);var raw=imageRect.gameObject.AddComponent<RawImage>();raw.raycastTarget=false;var fit=imageRect.gameObject.AddComponent<AspectRatioFitter>();fit.aspectMode=AspectRatioFitter.AspectMode.FitInParent;fit.aspectRatio=1;portrait.gameObject.AddComponent<WardrobePreview>().Initialize(raw,"boat",2,true);
  Heading(page.transform,CollectorRules.Request(d),36,cream,new(.48f,.64f),new(.93f,.77f));Placed(page.transform,CollectorRules.Remark(d),24,mint,new(.48f,.53f),new(.92f,.63f));
  var payment=CollectorRules.Payment(d,game.Items);string pay=payment.Count==0?"No matching pantry finds":string.Join("\n",payment.GroupBy(id=>id).Select(g=>g.Count()+" × "+game.Items[g.Key].name));
  Placed(page.transform,"GIVE  "+pay,23,cream,new(.48f,.35f),new(.93f,.51f));Placed(page.transform,"GET  "+CollectorRules.Reward(d)+" shinies",25,coral,new(.48f,.27f),new(.93f,.35f));
  Placed(page.transform,trades>=3?"Golden Sardine earned":$"Golden Sardine · {trades}/3 trades",24,mint,new(.07f,.16f),new(.93f,.24f));
  Button(page.transform,"Back",new(.07f,.045f),new(.40f,.14f),ShowDen);
  var deal=Button(page.transform,!fresh?"Bank a new haul":CollectorRules.Ready(d,game.Items)?"Make the trade":"Find the requested loot",new(.48f,.045f),new(.93f,.16f),()=>{game.TradeWithCollector(run,trades);ShowCollector();},true);deal.interactable=CollectorRules.Ready(d,game.Items);
 }
}
}
