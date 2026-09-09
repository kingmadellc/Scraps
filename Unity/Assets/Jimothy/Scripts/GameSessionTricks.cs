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
  if(!clean||points<=0)return;
  points=Mathf.Clamp(points,0,5000);
  Data.trickScore=(int)System.Math.Min(int.MaxValue,(long)Data.trickScore+points);
  bool record=points>Data.bestLandingScore;Data.bestLandingScore=Mathf.Max(Data.bestLandingScore,points);
  int milestone=Data.trickScore/1000;
  int shinies=Mathf.Max(0,milestone-Data.trickRewardMilestones)*5;
  Data.trickRewardMilestones=Mathf.Max(Data.trickRewardMilestones,milestone);
  Data.coins+=shinies;
  UI.LandingCelebration(trickName.ToUpperInvariant(),$"+{points:N0}  ·  {(record?"PERSONAL BEST!":"STUCK IT!")}"+(shinies>0?$"  +{shinies} shinies":""),true);
  feedback.Play(true);Save();
 }
}
}
