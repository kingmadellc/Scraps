using UnityEngine;
namespace Jimothy {
/// <summary>A continuous, interruptible warning before contact damage. No queued strikes.</summary>
public sealed class NeighborAttack {
 const float Windup=.42f;float held;
 public float Warning01=>Mathf.Clamp01(held/Windup);
 public void Reset(){held=0;}
 public bool Step(float dt,bool eligible){
  if(!eligible){Reset();return false;}
  held+=Mathf.Clamp(dt,0,.1f);
  if(held<Windup)return false;
  Reset();return true;
 }
}
}
