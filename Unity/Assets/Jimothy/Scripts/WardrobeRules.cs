using System;
using System.Linq;
using UnityEngine;
namespace Jimothy {
public static class WardrobeRules {
 public sealed class Look {public string id,name,story,requirement;public Look(string i,string n,string s,string r){id=i;name=n;story=s;requirement=r;}}
 public static readonly Look[] Looks={
  new("bare","Birthday fur","Unlicensed. Unbothered. Unclothed.","Always yours"),
  new("bandana","Dumpster headliner","Your first tour has one stop: the dumpster.","Always yours"),
  new("crown","Trash royalty","A crown assembled from highly questionable deposits.","Bank 3 different finds"),
  new("roadie","Back-alley roadie","Carries the demo. Refuses to carry the band.","Bank a music keepsake"),
  new("boat","Harbor menace","Self-appointed admiral of a very small puddle.","Bank a harbor keepsake"),
  new("inspector","Municipal inspector","Nobody questions the little guy in a safety vest.","Complete 4 banked outings")};
 public static readonly string[] Coats={"Ash","Cedar","Midnight"};
 public static bool Unlocked(SaveData d,string id){if(d==null)return false;var finds=d.bankedDiscoveries;return id=="bare"||id=="bandana"||id=="crown"&&(finds?.Distinct().Count()??0)>=3||id=="roadie"&&ExpeditionLoot.Curios[0].Any(x=>finds?.Contains(x)==true)||id=="boat"&&ExpeditionLoot.Curios[1].Any(x=>finds?.Contains(x)==true)||id=="inspector"&&d.bankedRuns>=4;}
 public static void Normalize(SaveData d){d.coat=Mathf.Clamp(d.coat,0,Coats.Length-1);if(!Looks.Any(x=>x.id==d.outfit)||!Unlocked(d,d.outfit))d.outfit="bare";}
 public static int Count(SaveData d)=>Looks.Count(x=>Unlocked(d,x.id));
 public static Color Tint(int coat)=>coat==1?new Color(1.13f,.89f,.69f):coat==2?new Color(.69f,.76f,.88f):Color.white;
}
public partial class GameSession {
 public bool Wear(string id){if(!Playing||!AtHome||!DenOpen||!WardrobeRules.Unlocked(Data,id))return false;Data.outfit=id;ApplyWardrobe();Save();return true;}
 public bool ChangeCoat(int index){if(!Playing||!AtHome||!DenOpen||index<0||index>=WardrobeRules.Coats.Length)return false;Data.coat=index;ApplyWardrobe();Save();return true;}
 void ApplyWardrobe(){var look=Player.visual.GetComponent<RaccoonWardrobe>();if(!look)look=Player.visual.gameObject.AddComponent<RaccoonWardrobe>();look.Apply(Data.outfit,Data.coat);}
}
}
