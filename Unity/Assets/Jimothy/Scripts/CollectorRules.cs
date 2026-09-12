using System;
using System.Collections.Generic;
using System.Linq;
namespace Jimothy {
public static class CollectorRules {
 public static int Offer(SaveData d)=>Math.Max(0,d.collectorTrades)%3;
 public static string Request(SaveData d)=>Offer(d)==0?"Two pantry snacks":Offer(d)==1?"One banked valuable":"One banked curio";
 public static string Remark(SaveData d)=>Offer(d)==0?"For inventory. Definitely not lunch.":Offer(d)==1?"I accept shiny. I do not accept questions.":"If nobody knows what it does, I want it.";
 public static int Reward(SaveData d)=>Offer(d)==0?8:Offer(d)==1?12:10;
 public static List<string> Payment(SaveData d,Dictionary<string,ItemDefinition> items){string category=Offer(d)==0?"food":Offer(d)==1?"valuable":"curio";return d.pantry.Where(id=>items.TryGetValue(id,out var item)&&item.category==category).OrderBy(id=>items[id].value).ThenBy(id=>id,StringComparer.Ordinal).Take(Offer(d)==0?2:1).ToList();}
 public static bool Ready(SaveData d,Dictionary<string,ItemDefinition> items)=>d.bankedRuns>0&&d.collectorLastRun<d.bankedRuns&&Payment(d,items).Count==(Offer(d)==0?2:1);
 public static void Normalize(SaveData d){d.collectorTrades=Math.Max(0,d.collectorTrades);d.collectorLastRun=Math.Max(0,Math.Min(d.bankedRuns,d.collectorLastRun));}
 public static bool Trade(SaveData d,Dictionary<string,ItemDefinition> items,int expectedRun,int expectedTrades,out string message){
  message="Bring back another banked haul for a fresh deal.";
  if(d.bankedRuns!=expectedRun||d.collectorTrades!=expectedTrades||!Ready(d,items))return false;
  var payment=Payment(d,items);int reward=Reward(d);foreach(string id in payment)d.pantry.Remove(id);
  d.coins=(int)Math.Min(int.MaxValue,(long)d.coins+reward);d.collectorLastRun=d.bankedRuns;d.collectorTrades=(int)Math.Min(int.MaxValue,(long)d.collectorTrades+1);
  message="Fair market nonsense. +"+reward+" shinies";
  if(d.collectorTrades==3)message+=" · Golden Sardine earned!";
  return true;
 }
}
public partial class GameSession {
 public bool NearCollector=>AtHome&&UnityEngine.Vector2.Distance(new(Player.transform.position.x,Player.transform.position.z),new(DenNeighbors.CollectorPosition.x,DenNeighbors.CollectorPosition.z))<1.8f;
 public bool NearClothesRail=>AtHome&&UnityEngine.Vector2.Distance(new(Player.transform.position.x,Player.transform.position.z),new(-18.9f,60))<1.6f;
 public bool TradeWithCollector(int run,int trades){if(!Playing||!AtHome||!DenOpen)return false;if(!CollectorRules.Trade(Data,Items,run,trades,out string message)){UI.Toast(message);return false;}RefreshDen();Save();UI.Toast(message);return true;}
}
}
