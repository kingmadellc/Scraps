using NUnit.Framework;
using UnityEngine;
namespace Jimothy.Tests {
public class ScavengeSiteTests {
 [Test] public void ConsecutiveHarvestsChangeSiteAndReloadKeepsIt(){for(int seed=1;seed<30;seed++)for(int node=0;node<22;node++)for(int count=2;count<=4;count++){int last=ScavengeSites.Slot(seed,node,0,count);for(int harvest=1;harvest<12;harvest++){int next=ScavengeSites.Slot(seed,node,harvest,count);Assert.AreNotEqual(last,next);Assert.AreEqual(next,ScavengeSites.Slot(seed,node,harvest,count));last=next;}}}
 [Test] public void ReplenishmentIsVariedAndNotTenMinuteClockwork(){var times=new System.Collections.Generic.HashSet<double>();for(int node=0;node<22;node++){double t=ScavengeSites.RestockSeconds(456,node,1);Assert.That(t,Is.InRange(1200d,2100d));times.Add(t);}Assert.Greater(times.Count,10);}
 [Test] public void AdditiveSaveFieldsPreserveInventoryAndExistingCooldownIds(){var old=JsonUtility.FromJson<SaveData>("{\"version\":1,\"bag\":[\"item_002\"],\"pantry\":[\"item_000\"],\"trophies\":[\"trophy_01\"],\"cooldowns\":[{\"node\":17,\"readyAt\":912.0}]}");Assert.AreEqual("item_002",old.bag[0]);Assert.AreEqual("item_000",old.pantry[0]);Assert.AreEqual("trophy_01",old.trophies[0]);Assert.AreEqual(17,old.cooldowns[0].node);Assert.AreEqual(912d,old.cooldowns[0].readyAt);Assert.AreEqual(0,old.cooldowns[0].harvests);old.forageSeed=987;old.cooldowns[0].harvests=3;var reload=JsonUtility.FromJson<SaveData>(JsonUtility.ToJson(old));Assert.AreEqual(987,reload.forageSeed);Assert.AreEqual(3,reload.cooldowns[0].harvests);Assert.AreEqual(old.bag,reload.bag);}
 [Test] public void WelcomeSnackRetainsItsAccessibleAuthoredAnchor(){var spot=new ClosingTimeWorld.ScavengeSpot("welcome_snack","item_002",new Vector3(-3,.61f,43));var candidates=ScavengeSites.Candidates(spot);Assert.AreEqual(1,candidates.Count);Assert.AreEqual(spot.position,candidates[0].position);}
}
}
