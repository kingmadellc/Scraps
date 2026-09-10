using UnityEngine;
namespace Jimothy {
public partial class GameSession {
 // The motor resolves a flight once, after real ground contact. Falls do not use
 // enemy hit immunity: being bitten must never make a roof drop harmless.
 public void ResolveLanding(int points,string trickName,float damage,bool clean){
  if(!Playing||Paused||Data==null)return;
  if(damage>0){
   Data.health=Mathf.Max(0,Data.health-Mathf.Clamp(damage,0,100));
   UI.LandingCelebration("ROUGH LANDING",$"−{Mathf.CeilToInt(damage)} health · Break your descent on lower ledges",false);
   if(Data.health<=0){EndRun();return;}Save();return;
  }
  if(!clean||points<=0||!Data.runActive||AtHome)return;
  points=Mathf.Clamp(points,0,5000);
  Data.trickScore=(int)System.Math.Min(int.MaxValue,(long)Data.trickScore+points);
  bool record=points>Data.bestLandingScore;Data.bestLandingScore=Mathf.Max(Data.bestLandingScore,points);
  int previous=Mathf.Min(3,Data.runStyle/1000);Data.runStyle=(int)System.Math.Min(int.MaxValue,(long)Data.runStyle+points);
  int shinies=(Mathf.Min(3,Data.runStyle/1000)-previous)*5;
  Data.pendingCoins+=shinies;
  UI.LandingCelebration(trickName.ToUpperInvariant(),$"+{points:N0}  ·  {(record?"PERSONAL BEST!":"STUCK IT!")}"+(shinies>0?$"  +{shinies} loose shinies":""),true);
  feedback.Play(true);Save();
 }
}
}
