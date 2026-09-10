using System.Linq;
using UnityEngine;
namespace Jimothy {
public partial class GameSession {
 bool returnToDen;int outingRoute=-1,outingWaypoint;bool wasReturning,eventWasActive;
 public bool HasLooseHaul=>Data!=null&&(Data.bag.Count>0||Data.pendingCoins>0);
 public string NightStatus=>Data==null?"":ExpeditionRules.EventTitle(Data.nightEvent)+(NightEventActive?" · ACTIVE":" · quiet interval");
 public bool NightEventActive=>Data!=null&&Data.runActive&&Data.survivalSeconds%100>=20&&Data.survivalSeconds%100<65;
 public void StartNight(){
  if(Data==null||Data.runActive)return;
  // Crossing the Den threshold starts the same next outing as the board button.
  if(!ExpeditionRules.Start(Data,Random.Range(1,int.MaxValue)))return;
  returnToDen=false;outingRoute=-1;outingWaypoint=0;wasReturning=false;
  foreach(var n in nodes)if(n){n.gameObject.SetActive(false);Destroy(n.gameObject);}nodes.Clear();SpawnLoot();
  foreach(var n in Neighbors)if(n){n.gameObject.SetActive(false);Destroy(n.gameObject);}Neighbors.Clear();Data.neighborStates.Clear();SpawnNeighbors();
  hasSession=true;Playing=true;Paused=false;DenOpen=false;Player.Running=true;autosave=0;invulnerable=2;
  if(AtHome){Player.FaceDirection(180);Player.RecenterCamera();}
  ApplyNightEvent();Save();UI.ShowHUD();UI.Toast($"Night {Data.runNumber}: {ExpeditionRules.GoalTitle(Data.selectedGoal)}. Bank at the Den; death loses loose finds.");
 }
 void UpdateNightEvent(){bool active=NightEventActive;if(active==eventWasActive)return;ApplyNightEvent();if(Data.runActive)UI.Toast(active?NightStatus+" · "+ExpeditionLoot.EventDescription(Data.nightEvent):"The street settles down. Watchers are attentive again.");}
 void ApplyNightEvent(){
  eventWasActive=NightEventActive;
  foreach(var n in Neighbors){float speed=1,sight=1;
   if(eventWasActive){if(Data.nightEvent==1&&n.Kind==NeighborKind.Fisherman){speed=.75f;sight=1.2f;}
    else if((Data.nightEvent==0||Data.nightEvent==2)&&(n.Kind==NeighborKind.AngryHuman||n.Kind==NeighborKind.Fisherman))sight=.65f;}
   n.ConfigureChallenge(speed,sight);
  }
 }
 Vector3[] ExpeditionRoutePath(int index,bool returning){
  var trip=ClosingTimeWorld.DenRoundTrips[index];var cache=nodes.FirstOrDefault(n=>n&&n.name==trip.CacheId);
  var path=cache?ClosingTimeWorld.AppendFindApproach(trip,cache):trip.Outbound;
  return returning?path.Reverse().ToArray():path;
 }
 void GetExpeditionTarget(out Vector3 target,out string label){
  target=ClosingTimeWorld.DenEntrance;label="Den entrance · Bank your haul";
  bool returning=returnToDen||Data.bag.Count>=ExpeditionRules.BagCapacity||ExpeditionRules.GoalComplete(Data,Items)||!Data.runActive;
  bool useRoof=returning&&Player.transform.position.y>1.5f||!returning&&Data.selectedGoal==2;
  if(useRoof&&ClosingTimeWorld.DenRoundTrips.Count>0){
   int route=Player.transform.position.x<0?0:1;
   bool fellBelow=outingRoute>=0&&Player.Grounded&&ExpeditionRoutePath(route,returning)[Mathf.Clamp(outingWaypoint,0,ExpeditionRoutePath(route,returning).Length-1)].y>Player.transform.position.y+1.4f;
   if(outingRoute<0||route!=outingRoute||returning!=wasReturning||fellBelow){outingRoute=route;wasReturning=returning;outingWaypoint=0;var initial=ExpeditionRoutePath(route,returning);
    // Rejoin only at a physically matching height; never route through an upper wall.
    float best=float.MaxValue;for(int i=0;i<initial.Length;i++){if(Mathf.Abs(initial[i].y-Player.transform.position.y)>1.2f)continue;float d=(initial[i]-Player.transform.position).sqrMagnitude;if(d<best){best=d;outingWaypoint=i;}}
   }
   var trip=ClosingTimeWorld.DenRoundTrips[outingRoute];var path=ExpeditionRoutePath(outingRoute,returning);
   outingWaypoint=Mathf.Min(path.Length-1,ClosingTimeWorld.NextWaypoint(Player.transform.position,Player.Grounded,path,outingWaypoint));
   target=path[outingWaypoint];label=$"Route {trip.Id} · {(returning?"Den ↓ same steps":trip.Name)} · {outingWaypoint+1}/{path.Length}";
   if(!returning&&outingWaypoint==path.Length-1){var cache=nodes.FirstOrDefault(n=>n&&n.name==trip.CacheId&&n.IsAvailable);if(cache){label="Search rooftop keepsake";}}
   return;
  }
  if(returning){target=ClosingTimeWorld.ReturnStreetWaypoint(Player.transform.position);label=target.z>=55?"Down the Den ramp → Stash to bank":"Follow the open north street → Den";return;}
  string category=Data.selectedGoal==1?"valuable":"food";
  var next=nodes.Where(n=>n&&n.IsAvailable&&n.Category==category&&!n.IsRooftop).OrderBy(n=>(n.transform.position-Player.transform.position).sqrMagnitude).FirstOrDefault();
  if(next){target=next.SearchApproach;label=category=="food"?"Sheltered restaurant find · Search":"Patio lost property · Search";}
 }
}
}
