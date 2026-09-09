using UnityEngine;
namespace Jimothy {
/// <summary>Continuous suspicion with hysteresis and a short last-seen search memory.</summary>
public sealed class NeighborAwareness {
 public float Value {get;private set;}
 public bool Chasing {get;private set;}
 public float UnseenSeconds {get;private set;}
 public void Step(float dt,bool visible,float proximity,float movement,bool hostile,bool safe){
  dt=Mathf.Clamp(dt,0,.1f);proximity=Mathf.Clamp01(proximity);movement=Mathf.Clamp01(movement);
  if(visible&&!safe){UnseenSeconds=0;Value=Mathf.Min(1,Value+dt*(.20f+.47f*proximity)*Mathf.Lerp(.65f,1,movement));}
  else{UnseenSeconds+=dt;Value=Mathf.Max(0,Value-dt*(safe?.25f:.16f));}
  if(!hostile||safe)Chasing=false;
  else if(!Chasing&&Value>=.999f&&visible)Chasing=true;
  else if(Chasing&&(UnseenSeconds>=3||Value<.30f))Chasing=false;
 }
}
}
