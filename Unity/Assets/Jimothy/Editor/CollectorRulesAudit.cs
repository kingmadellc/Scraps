using System;
using System.Linq;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace Jimothy.Editor {
public static class CollectorRulesAudit {
 public static void Run(){int n=0;try{void Check(bool ok,string why){n++;if(!ok)throw new Exception(why);}
  var items=JsonUtility.FromJson<ItemCatalog>(Resources.Load<TextAsset>("Items").text).items.ToDictionary(i=>i.id);var d=new SaveData();CollectorRules.Normalize(d);Check(!CollectorRules.Ready(d,items),"No trade before first bank");
  d.bag.AddRange(new[]{"item_000","item_002"});d.bankedRuns=1;Check(!CollectorRules.Ready(d,items),"Pockets cannot pay");string before=JsonUtility.ToJson(d);Check(!CollectorRules.Trade(d,items,1,0,out _)&&before==JsonUtility.ToJson(d),"Insufficient trade is atomic");
  d.pantry.AddRange(new[]{"item_000","item_002"});d.bankedDiscoveries.AddRange(d.pantry);d.favoriteFinds.Add("item_000");Check(CollectorRules.Ready(d,items),"Exact banked snacks qualify");Check(CollectorRules.Trade(d,items,1,0,out _)&&d.coins==8&&d.pantry.Count==0,"Exact payment and reward");Check(d.bag.Count==2&&d.bankedDiscoveries.Count==2,"Trade leaves pocket loot and historical unlocks intact");
  d.pantry.Add("item_064");before=JsonUtility.ToJson(d);Check(!CollectorRules.Trade(d,items,1,0,out _)&&before==JsonUtility.ToJson(d),"Double callback rejected without mutation");Check(!CollectorRules.Ready(d,items),"One deal per banked outing");
  d.bankedRuns=2;Check(CollectorRules.Trade(d,items,2,1,out _)&&d.coins==20,"Second offer consumes a valuable");d.bankedRuns=3;d.trophies.Add("trophy_00");Check(!CollectorRules.Ready(d,items),"Trophies cannot substitute for curios");d.pantry.Add("item_176");Check(CollectorRules.Trade(d,items,3,2,out var message)&&message.Contains("Golden Sardine"),"Third deal earns trophy");Check(d.collectorTrades==3&&d.trophies.Contains("trophy_00"),"Trophy inventory untouched");
  var loaded=JsonUtility.FromJson<SaveData>(JsonUtility.ToJson(d));CollectorRules.Normalize(loaded);Check(loaded.collectorTrades==3&&loaded.collectorLastRun==3&&!CollectorRules.Ready(loaded,items),"Save reload cannot repeat trade");loaded.runActive=true;ExpeditionRules.Lose(loaded);Check(loaded.collectorTrades==3,"Death preserves reputation");loaded.bankedRuns=4;loaded.pantry.AddRange(new[]{"item_000","item_002"});Check(CollectorRules.Ready(loaded,items),"Offers repeat after next bank");before=JsonUtility.ToJson(loaded);Check(!CollectorRules.Trade(loaded,items,3,2,out _)&&before==JsonUtility.ToJson(loaded),"Stale offer rejected atomically");
  var old=JsonUtility.FromJson<SaveData>("{\"version\":1}");CollectorRules.Normalize(old);Check(old.collectorTrades==0&&old.collectorLastRun==0,"Legacy ledger defaults safe");File.WriteAllText("../Logs/collector-rules.json","{\"passed\":true,\"assertions\":"+n+"}");EditorApplication.Exit(0);
 }catch(Exception e){Debug.LogException(e);EditorApplication.Exit(1);}}
}
}
