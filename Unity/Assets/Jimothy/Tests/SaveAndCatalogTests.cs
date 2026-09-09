using NUnit.Framework;
using UnityEngine;
using System.Linq;
namespace Jimothy.Tests {
public class SaveAndCatalogTests {
 [Test] public void JumpDoesNotAdvanceLandingGuidanceBeforeTouchdown(){var route=new[]{new Vector3(0,1,0),new Vector3(1,2,0),new Vector3(2,3,0)};Assert.AreEqual(0,GameSession.StableLandingStep(new Vector3(1,2,0),false,route,0));Assert.AreEqual(2,GameSession.StableLandingStep(new Vector3(1,2.04f,0),true,route,0));Assert.AreEqual(0,GameSession.StableLandingStep(Vector3.zero,true,route,2));}
 [Test] public void AlleyStairsDoNotCountAsTheRoof(){Assert.IsFalse(GameSession.OnRoofFootprint(new Vector3(-15.3f,9,40.95f)));Assert.IsTrue(GameSession.OnRoofFootprint(new Vector3(-15.3f,8.36f,39.5f)));Assert.IsFalse(GameSession.OnRoofFootprint(new Vector3(-15.3f,0,39.5f)));}

 [Test] public void OldMapMovesHomeWithoutLosingCollection(){var d=new SaveData{mapRevision=2,x=100,y=20,z=-30,coins=51};d.bag.Add("item_002");d.trophies.Add("trophy_03");d.cooldowns.Add(new NodeCooldown{node=3,readyAt=900});d.MigrateToClosingTime(new Vector3(-3,.45f,44));Assert.AreEqual(3,d.mapRevision);Assert.AreEqual(44,d.z);Assert.AreEqual(51,d.coins);Assert.AreEqual("item_002",d.bag[0]);Assert.AreEqual("trophy_03",d.trophies[0]);Assert.IsEmpty(d.cooldowns);d.x=8;d.MigrateToClosingTime(Vector3.zero);Assert.AreEqual(8,d.x);}
 [Test] public void NightObjectiveSurvivesSaveRoundTrip(){var d=new SaveData{mapRevision=2,nightGoal=2};var r=JsonUtility.FromJson<SaveData>(JsonUtility.ToJson(d));Assert.AreEqual(2,r.nightGoal);Assert.AreEqual(2,r.mapRevision);}
 [Test] public void OfflineProgressIsCappedWithoutStarvation(){var d=new SaveData{savedUtc=100,hunger=23,health=40};d.ApplyOffline(100+86400);Assert.AreEqual(28800,d.worldSeconds);Assert.AreEqual(23,d.hunger);Assert.AreEqual(40,d.health);}
 [Test] public void BackwardsClockCannotSubtractProgress(){var d=new SaveData{savedUtc=100,worldSeconds=90};d.ApplyOffline(50);Assert.AreEqual(90,d.worldSeconds);}
 [Test] public void RejectsNonfiniteAndFutureSave(){var d=new SaveData{x=float.NaN};Assert.IsFalse(d.Valid());d.x=0;d.version=99;Assert.IsFalse(d.Valid());}
 [Test] public void CatalogHasUniqueIdsAndSixteenTrophies(){var c=JsonUtility.FromJson<ItemCatalog>(Resources.Load<TextAsset>("Items").text);Assert.AreEqual(256,c.items.Length);Assert.AreEqual(256,c.items.Select(i=>i.id).Distinct().Count());Assert.AreEqual(16,c.items.Count(i=>i.category=="trophy"));Assert.IsTrue(c.items.Where(i=>i.category=="food").All(i=>i.nutrition>0));}
 [Test] public void StateRoundTripPreservesCollection(){var d=new SaveData{coins=71};d.bag.Add("item_001");d.trophies.Add("trophy_03");d.decor.Add("cushion");d.cooldowns.Add(new NodeCooldown{node=8,readyAt=900});var restored=JsonUtility.FromJson<SaveData>(JsonUtility.ToJson(d));Assert.AreEqual(71,restored.coins);CollectionAssert.AreEqual(d.bag,restored.bag);CollectionAssert.AreEqual(d.trophies,restored.trophies);Assert.AreEqual(900,restored.cooldowns[0].readyAt);}
}
}
