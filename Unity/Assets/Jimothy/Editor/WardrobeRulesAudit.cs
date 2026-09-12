using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace Jimothy.Editor {
public static class WardrobeRulesAudit {
 public static void Run(){int n=0;try{
  void Check(bool ok,string why){n++;if(!ok)throw new Exception(why);}
  var items=JsonUtility.FromJson<ItemCatalog>(Resources.Load<TextAsset>("Items").text).items.ToDictionary(i=>i.id);
  var d=new SaveData();WardrobeRules.Normalize(d);Check(d.outfit=="bare"&&WardrobeRules.Count(d)==2,"Starter looks");
  d.bag.AddRange(new[]{"item_176","item_128","item_000"});Check(!WardrobeRules.Unlocked(d,"crown")&&!WardrobeRules.Unlocked(d,"roadie")&&!WardrobeRules.Unlocked(d,"boat"),"Pocket loot must not unlock outfits");
  d.runActive=true;ExpeditionRules.Lose(d);Check(WardrobeRules.Count(d)==2,"Death cannot unlock loose outfits");
  d.runActive=true;d.bag.AddRange(new[]{"item_176","item_128","item_000"});ExpeditionRules.Bank(d,items);Check(WardrobeRules.Unlocked(d,"crown")&&WardrobeRules.Unlocked(d,"roadie")&&WardrobeRules.Unlocked(d,"boat"),"Banked finds unlock exact looks");
  int c=d.coins;ExpeditionRules.Bank(d,items);Check(d.coins==c&&WardrobeRules.Count(d)==5,"Repeated bank does not duplicate rewards");
  d.outfit="roadie";d.coat=2;var loaded=JsonUtility.FromJson<SaveData>(JsonUtility.ToJson(d));WardrobeRules.Normalize(loaded);Check(loaded.outfit=="roadie"&&loaded.coat==2&&loaded.Valid(),"Appearance survives serialization");
  loaded.runActive=true;loaded.bag.Add("item_176");ExpeditionRules.Lose(loaded);Check(loaded.outfit=="roadie"&&WardrobeRules.Unlocked(loaded,"roadie"),"Banked cosmetic survives death");
  loaded.bankedRuns=4;Check(WardrobeRules.Unlocked(loaded,"inspector"),"Four banked outings unlock inspector");
  var old=JsonUtility.FromJson<SaveData>("{\"version\":1}");WardrobeRules.Normalize(old);Check(old.outfit=="bare"&&old.coat==0,"Old saves default safely");
  old.outfit="hacked";old.coat=500;WardrobeRules.Normalize(old);Check(old.outfit=="bare"&&old.coat==2,"Unknown appearance clamped");
  old.bankedDiscoveries.AddRange(new[]{"item_000","item_000","item_000"});Check(!WardrobeRules.Unlocked(old,"crown"),"Duplicates do not count as unique");
  old.outfit="inspector";WardrobeRules.Normalize(old);Check(old.outfit=="bare","Locked equipped look rejected");
  File.WriteAllText("../Logs/wardrobe-rules.json","{\"passed\":true,\"assertions\":"+n+"}");EditorApplication.Exit(0);
 }catch(Exception e){Debug.LogException(e);EditorApplication.Exit(1);}}
}
}
