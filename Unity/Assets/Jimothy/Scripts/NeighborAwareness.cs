using UnityEngine;
namespace Jimothy {
/// <summary>Continuous suspicion with hysteresis and a short last-seen search memory.</summary>
public sealed class NeighborAwareness {
 public float Value {get;private set;}
 public bool Chasing {get;private set;}
 public float UnseenSeconds {get;private set;}
 public float ReacquireGrace {get;private set;}
 public void Restore(float value,bool chasing,float unseen,float grace){
  Value=FiniteClamp(value,0,1);UnseenSeconds=FiniteClamp(unseen,0,60);ReacquireGrace=FiniteClamp(grace,0,.85f);
  Chasing=chasing&&Value>=.30f&&UnseenSeconds<3;
 }
 static float FiniteClamp(float n,float low,float high)=>float.IsNaN(n)||float.IsInfinity(n)?low:Mathf.Clamp(n,low,high);
 public void Step(float dt,bool visible,float proximity,float movement,bool hostile,bool safe){
  dt=Mathf.Clamp(dt,0,.1f);proximity=Mathf.Clamp01(proximity);movement=Mathf.Clamp01(movement);
  ReacquireGrace=safe?.85f:Mathf.Max(0,ReacquireGrace-dt);
  if(visible&&!safe){UnseenSeconds=0;Value=Mathf.Min(1,Value+dt*(.20f+.47f*proximity)*Mathf.Lerp(.65f,1,movement));}
  else{UnseenSeconds+=dt;Value=Mathf.Max(0,Value-dt*(safe?.25f:.16f));}
  if(!hostile||safe)Chasing=false;
  else if(!Chasing&&Value>=.999f&&visible&&ReacquireGrace<=0)Chasing=true;
  else if(Chasing&&(UnseenSeconds>=3||Value<.30f)){Chasing=false;ReacquireGrace=.85f;}
 }
}
}
