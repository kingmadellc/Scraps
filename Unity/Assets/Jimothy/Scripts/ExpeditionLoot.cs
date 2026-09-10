using System;
using UnityEngine;
namespace Jimothy {
// Deliberately small launch collection. Existing authored find meshes supply every item.
public static class ExpeditionLoot {
 public static readonly string[] Foods={"item_000","item_002","item_006","item_016","item_030","item_048"};
 public static readonly string[] Valuables={"item_064","item_070","item_080","item_090","item_098","item_102"};
 public static readonly string[][] Curios={new[]{"item_176","item_178","item_188"},new[]{"item_128","item_136","item_211"},new[]{"item_160","item_162","item_168"}};
 public static readonly string[] Trophies={"trophy_00","trophy_02","trophy_03","trophy_04","trophy_06","trophy_10"};
 public static string ItemFor(ClosingTimeWorld.ScavengeSpot spot,SaveData d){
  uint hash=2166136261;foreach(char c in spot.id)hash=(hash^c)*16777619;
  int choice=(int)((hash+(uint)d.forageSeed)%2147483647);
  if(spot.rooftop)return Trophies[choice%Trophies.Length];
  if(spot.id=="welcome_snack")return Foods[(d.nightEvent+1)%Foods.Length];
  // Four sheltered patios always offer valuables; roof and kitchen finds cover all goals.
  if(spot.id.StartsWith("patio"))return Valuables[choice%Valuables.Length];
  // Each night presents all three pieces of its featured collection, among the kitchens.
  int row=0;var bits=spot.id.Split('_');int.TryParse(bits[bits.Length-1],out row);
  if(spot.id.Contains("kitchen")&&row<3&&spot.id.Contains("_-1_"))return Curios[d.nightEvent%3][row];
  return Foods[choice%Foods.Length];
 }
 public static string EventDescription(int e)=>e==0?"Last-call music: human watchers are distracted during the music. Search west kitchens for music keepsakes.":e==1?"Harbor unloading: the fisherman walks slower but watches farther. West kitchens hide maritime keepsakes.":"Garden watering: human watchers notice you from a shorter distance during the watering. West kitchens hide garden keepsakes.";
}
}
