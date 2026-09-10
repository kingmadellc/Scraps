using System.Collections.Generic;
using UnityEngine;
namespace Jimothy {
public static partial class ClosingTimeWorld {
 public sealed class DenRoundTrip {
  public string Id,Name,CacheId;public Vector3[] Outbound,Return;
 }
 public static readonly List<DenRoundTrip> DenRoundTrips=new();
 // Progress only through the next checkpoint after a supported landing; never choose a future stair through a wall.
 public static int NextWaypoint(Vector3 position,bool grounded,Vector3[] waypoints,int current){
  if(waypoints==null||waypoints.Length==0)return 0;current=Mathf.Clamp(current,0,waypoints.Length);if(current==waypoints.Length||!grounded)return current;var d=position-waypoints[current];return Mathf.Abs(d.y)<.25f&&new Vector2(d.x,d.z).sqrMagnitude<.35f*.35f?current+1:current;
 }
 // Append this to Outbound using the live LootNode.SearchApproach. Reverse it before Return.
 public static Vector3[] CacheApproachPath(DenRoundTrip route,Vector3 approach){
  float side=route.Id=="A"?-1:1,y=route.Outbound[route.Outbound.Length-1].y;var start=route.Outbound[route.Outbound.Length-1];
  return Mathf.Abs(approach.x)>14?new[]{start,new Vector3(side*14.9f,y,38.6f),new Vector3(side*14.9f,y,approach.z),approach}:new[]{start,new Vector3(side*10.65f,y,39.1f),new Vector3(side*10.65f,y,approach.z),approach};
 }
 public static Vector3[] AppendFindApproach(DenRoundTrip route,LootNode node){if(!node)return route.Outbound;var path=new List<Vector3>(route.Outbound);var connector=CacheApproachPath(route,node.SearchApproach);for(int i=1;i<connector.Length;i++)path.Add(connector[i]);return path.ToArray();}
 public static Vector3 ReturnStreetWaypoint(Vector3 position){
  if(position.z<43)return new Vector3(position.x<0?-7:7,.24f,44);
  if(position.x> -9)return new Vector3(-10,.05f,44);
  if(position.z>=48&&Mathf.Abs(position.x+14)<2)return position.z>=54.5f?DenInterior:new Vector3(-14,-1.95f,55);
  return DenEntrance;
 }
 // Two named routes reuse existing north access landings and roof finds.
 static void RoundTripWayfinding(){
  DenRoundTrips.Clear();Mat("routeMint",new Color(.35f,.89f,.77f),.08f);Mat("routeGold",new Color(1f,.73f,.29f),.08f);
  Physics.SyncTransforms();
  foreach(int side in new[]{-1,1}){
   int index=side<0?4:9;string letter=side<0?"A":"B",name=side<0?"SALMON LOOKOUT":"RECORDS ROOFTOP",ink=side<0?"routeMint":"routeGold";
   var path=new List<Vector3>{DenInterior,new(-14,-1.95f,55),DenEntrance,new(-10,.05f,44),StreetStart,new(side*7,.24f,44),RouteStarts[index]};
   path.AddRange(RoofRoutes[index]);path.Add(new(side*12.6f,8.24f,39.1f));path.Add(new(side*12.6f,8.24f,38.6f));
   var reverse=new List<Vector3>(path);reverse.Reverse();DenRoundTrips.Add(new DenRoundTrip{Id=letter,Name=name,CacheId="roof_"+side+"_4",Outbound=path.ToArray(),Return=reverse.ToArray()});
   // Retitle existing sign; no new post or collision is placed in the approach.
   Vector3 sign=new(side*6.35f,.80f,41.85f);foreach(var label in root.GetComponentsInChildren<TextMesh>())if(Vector3.Distance(label.transform.position,sign)<.08f){label.text=letter+" / "+name+"\nJUMP UP THE STEPS";label.characterSize=.0105f;label.gameObject.name="Route "+letter+" start sign";}
   // Roof boards sit flush against the outer edge of an existing service landing.
   Vector3 board=new(side*16.34f,8.78f,41.62f);Box("Route "+letter+" return plate",board,new(1.3f,.40f,.025f),"awning");
   foreach(int edge in new[]{-1,1})Box("Return plate mounting bracket",board+new Vector3(edge*.5f,-.29f,0),new(.025f,.3f,.025f),"iron");
   Text(letter+"  /  DEN\nDOWN THE SAME STEPS",board+Vector3.back*.016f,.0115f,"cream",Quaternion.identity);
   Text(letter+"  /  "+name,board+Vector3.forward*.016f,.011f,ink,Quaternion.Euler(0,180,0));
   // Small paired letter/return chevrons are paint, never invisible physical barriers.
   for(int i=5;i<path.Count;i++)if(i<7||i%2==0||i==path.Count-1){var toward=path[i-1]-path[i];toward.y=0;if(toward.sqrMagnitude<.01f)continue;var p=path[i];if(Physics.Raycast(p+Vector3.up*.25f,Vector3.down,out var hit,.65f,1,QueryTriggerInteraction.Ignore))p.y=hit.point.y+.015f;else p.y-=.02f;ReturnPaint(letter,p,toward.normalized,ink);}
  }
  // Shared street decision point uses an explicit destination instead of an ambiguous arrow.
  Text("A  LOOKOUT     B  RECORDS\nRETURN: THE DEN  /  NORTHWEST",new(-3,.025f,44.8f),.016f,"cream",Quaternion.Euler(90,0,0));
  foreach(var p in new[]{new Vector3(-7,.22f,44),new Vector3(-10,.02f,46),new Vector3(-13,.02f,48)})ReturnPaint("DEN",p,(DenEntrance-p).normalized,"cream");
 }
 static void ReturnPaint(string label,Vector3 p,Vector3 direction,string material){
  var q=Quaternion.LookRotation(direction,Vector3.up);var offset=q*Vector3.right*.35f;
  foreach(int side in new[]{-1,1})Box("Painted Den return arrow",p+offset+q*new Vector3(side*.07f,0,0),new(.025f,.008f,.20f),material,false,q*Quaternion.Euler(0,side*-40,0));
  Text(label,p+offset-q*Vector3.forward*.22f,.010f,material,q*Quaternion.Euler(90,0,0));
 }
}
}
