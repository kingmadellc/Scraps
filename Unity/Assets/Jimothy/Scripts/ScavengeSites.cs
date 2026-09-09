using System;
using System.Collections.Generic;
using UnityEngine;
namespace Jimothy {
/// <summary>Stable node identities, varied prop-side sites. No item or saved-inventory remapping.</summary>
public static class ScavengeSites {
 public struct Site {public Vector3 position,approach;public string context;public Site(Vector3 p,Vector3 a,string c){position=p;approach=a;context=c;}}
 public static int Slot(int seed,int node,int harvest,int count){if(count<1)return 0;uint mix=unchecked((uint)seed*2654435761u+(uint)node*2246822519u);return (int)((mix%(uint)count+(uint)Math.Max(0,harvest)%(uint)count)%(uint)count);}
 public static double RestockSeconds(int seed,int node,int harvest)=>1200+Slot(seed,node,harvest*137,901);
 public static List<Site> Candidates(ClosingTimeWorld.ScavengeSpot spot){
  var sites=new List<Site>();float side=Mathf.Sign(spot.position.x);Vector3 p=spot.position;
  void Add(float x,float y,float z,string context){var at=new Vector3(x,y,z);sites.Add(new Site(at,at+new Vector3(-side*.75f,0,0),context));}
  if(spot.id=="welcome_snack"){sites.Add(new Site(p,ClosingTimeWorld.StreetStart,"Welcome snack"));return sites;}
  if(spot.id.StartsWith("kitchen_")){
   // The short ends of each dumpster conceal finds from a straight walk down the road.
   Add(side*8.48f,.40f,p.z-1.02f,"Beside the back-door bin");
   Add(side*8.48f,.40f,p.z+.86f,"Behind the recycling corner");
   Add(side*8.02f,.40f,p.z-1.25f,"Among the closing-time scraps");
  }else if(spot.id.StartsWith("patio_")){
   Add(side*8.25f,.40f,p.z-.83f,"In the crate's shadow");
   Add(side*8.25f,.40f,p.z+.81f,"Beside the stacked crate");
   Add(side*8.35f,.40f,p.z-3.65f,"At the planter's edge");
  }else if(spot.id.StartsWith("roof_")){
   float roof=p.y-.35f,z=p.z-2.6f,x=p.x+side*2;
   Add(x-side*2.4f,roof+.38f,z-3.08f,"Behind the roof vent");
   Add(x-side*2.4f,roof+.38f,z-.88f,"In the vent's lee");
   Add(x+side*2.0f,roof+.38f,z+2.15f,"Beside the chimney");
  }else {
   // Lookout keepsake remains on its supported deck, tucked along one of its edges.
   var left=p+new Vector3(-.55f,0,.15f);var right=p+new Vector3(.55f,0,-.15f);
   sites.Add(new Site(left,left+Vector3.right*.6f,"Lookout corner"));sites.Add(new Site(right,right+Vector3.left*.6f,"Lookout corner"));
  }
  return sites;
 }
 public static bool Validate(Site site,out Site grounded){
  grounded=site;
  if(!Physics.Raycast(site.position+Vector3.up*.35f,Vector3.down,out var floor,.85f,1<<0,QueryTriggerInteraction.Ignore)||floor.normal.y<.8f)return false;
  // Search must be possible from a supported, unobstructed raccoon-sized approach.
  if(!Physics.Raycast(site.approach+Vector3.up*.35f,Vector3.down,out var access,.9f,1<<0,QueryTriggerInteraction.Ignore)||access.normal.y<.8f||Mathf.Abs(access.point.y-floor.point.y)>.25f)return false;
  var feet=access.point+Vector3.up*.03f;
  if(Physics.CheckCapsule(feet+Vector3.up*.19f,feet+Vector3.up*.38f,.17f,1<<0,QueryTriggerInteraction.Ignore))return false;
  var anchor=floor.point+Vector3.up*.2f;
  if(Physics.Linecast(feet+Vector3.up*.4f,anchor+Vector3.up*.1f,1<<0,QueryTriggerInteraction.Ignore))return false;
  grounded=new Site(anchor,feet,site.context);return true;
 }
 public static List<Site> ValidSites(ClosingTimeWorld.ScavengeSpot spot){var result=new List<Site>();foreach(var candidate in Candidates(spot))if(Validate(candidate,out var valid))result.Add(valid);return result;}
}
}
