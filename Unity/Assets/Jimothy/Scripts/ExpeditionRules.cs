using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Jimothy {
/// <summary>Pure run settlement. Callers own world rebuilding and one save after each transition.</summary>
public static class ExpeditionRules {
 public const int BagCapacity=8,GoalBonus=12;
 static readonly string[][] Sets={new[]{"item_176","item_178","item_188"},new[]{"item_160","item_162","item_168"},new[]{"item_128","item_136","item_211"}};
 static readonly string[] SetNames={"Music","Greenery","Harbor"},Awards={"vinyl-wall","plant-corner","harbor-shelf"};
 static void Lists(SaveData d){d.bag??=new();d.pantry??=new();d.trophies??=new();d.decor??=new();d.cooldowns??=new();d.bankedDiscoveries??=new();d.collectionClaims??=new();d.neighborStates??=new();}
 public static void Migrate(SaveData d,Vector3 den){Lists(d);foreach(var id in d.pantry.Concat(d.trophies).Distinct())if(!d.bankedDiscoveries.Contains(id))d.bankedDiscoveries.Add(id);if(d.loopRevision>=1)return;
  // Same room extents relative to its spawn; no scene objects or physics required.
  var p=new Vector3(d.x,d.y,d.z)-den;bool inside=Math.Abs(p.x)<=5.7f&&p.z>=-3&&p.z<=4.2f&&p.y>=-.25f&&p.y<=3.75f;
  foreach(var cd in d.cooldowns)if(cd.harvests==0&&cd.readyAt>d.worldSeconds)cd.harvests=1;
  d.loopRevision=1;d.nightGoal=3;d.runActive=d.bag.Count>0||!inside;d.runNumber=d.runActive?1:0;d.selectedGoal=d.nightEvent=0;
 }
 public static bool Start(SaveData d,int seed){Lists(d);if(d.runActive||d.bag.Count>0||d.pendingCoins>0)return false;d.loopRevision=1;d.nightGoal=3;d.runNumber=Add(d.runNumber,1);d.runActive=true;d.pendingCoins=d.runStyle=0;d.health=d.hunger=100;d.survivalSeconds=0;d.cooldowns.Clear();d.neighborStates.Clear();d.forageSeed=seed==0?1:seed;
  d.selectedGoal=(d.runNumber-1)%3;d.nightEvent=EventForRun(d.runNumber);return true;
 }
 public static int EventForRun(int number){int i=Math.Max(0,number-1);return (i%3+i/3)%3;}
 public static string GoalTitle(int goal)=>goal==1?"Bank 2 valuables":goal==2?"Bank a rooftop trophy":"Bank 3 snacks";
 public static string EventTitle(int night)=>night==1?"Harbor night":night==2?"Garden night":"Record-store night";
 static int Count(SaveData d,Dictionary<string,ItemDefinition> items){string category=d.selectedGoal==1?"valuable":d.selectedGoal==2?"trophy":"food";return d.bag.Count(id=>items.TryGetValue(id,out var item)&&item.category==category);}
 static int Target(SaveData d)=>d.selectedGoal==1?2:d.selectedGoal==2?1:3;
 public static string GoalProgress(SaveData d,Dictionary<string,ItemDefinition> items)=>Math.Min(Count(d,items),Target(d))+" / "+Target(d);
 public static bool GoalComplete(SaveData d,Dictionary<string,ItemDefinition> items)=>Count(d,items)>=Target(d);
 public static int Bank(SaveData d,Dictionary<string,ItemDefinition> items){Lists(d);if(!d.runActive||(d.bag.Count==0&&d.pendingCoins==0))return 0;int earned=d.pendingCoins;if(GoalComplete(d,items))earned=Add(earned,GoalBonus);
  foreach(string id in d.bag){if(!items.TryGetValue(id,out var item))continue;if(!d.bankedDiscoveries.Contains(id))d.bankedDiscoveries.Add(id);if(item.category=="trophy"){if(!d.trophies.Contains(id)){d.trophies.Add(id);earned=Add(earned,15);}}else{d.pantry.Add(id);if(item.category!="valuable")earned=Add(earned,1);}}
  d.bestSeconds=Math.Max(d.bestSeconds,(int)Math.Min(int.MaxValue,d.survivalSeconds));d.coins=Add(d.coins,earned);d.lastHaul=earned;d.bankedRuns=Add(d.bankedRuns,1);d.bag.Clear();d.pendingCoins=0;d.runActive=false;d.neighborStates.Clear();ClaimSets(d);return earned;
 }
 public static void Lose(SaveData d){Lists(d);if(!d.runActive)return;d.lastLoss=d.bag.Count;d.bestSeconds=Math.Max(d.bestSeconds,(int)Math.Min(int.MaxValue,d.survivalSeconds));d.health=d.hunger=100;d.failedRuns=Add(d.failedRuns,1);d.bag.Clear();d.pendingCoins=0;d.runStyle=0;d.runActive=false;d.cooldowns.Clear();d.neighborStates.Clear();}
 static void ClaimSets(SaveData d){foreach(int i in Enumerable.Range(0,Sets.Length))if(Sets[i].All(d.bankedDiscoveries.Contains)&&!d.collectionClaims.Contains(Awards[i])){d.collectionClaims.Add(Awards[i]);if(!d.decor.Contains(Awards[i]))d.decor.Add(Awards[i]);}}
 public static string CollectionSummary(SaveData d){Lists(d);var clues=new[]{"pick / record sleeve / cassette unlocks vinyl wall","acorn / pine cone / seeds unlocks plant corner","cork float / lure / compass unlocks harbor shelf"};return string.Join("\n",Enumerable.Range(0,Sets.Length).Select(i=>SetNames[i]+" "+Sets[i].Count(d.bankedDiscoveries.Contains)+"/3"+(d.collectionClaims.Contains(Awards[i])?" ✓ ":" · ")+clues[i]));}
 static int Add(int a,int b)=>(int)Math.Min(int.MaxValue,(long)Math.Max(0,a)+Math.Max(0,b));
 }
}
