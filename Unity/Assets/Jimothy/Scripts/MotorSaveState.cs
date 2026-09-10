using System;
using UnityEngine;
namespace Jimothy {
[Serializable] public sealed class MotorSaveState {
 public int version=1;
 public float yaw,pitch,visualYaw,touchHeading,vertical,fallApex,airTime,trickElapsed,comboRemaining,mantleElapsed,mantleStalled,mantleCooldown,stairAssist;
 public bool airTracking,gapCrossed,flightMantled,trickActive,mantling;
 public Vector3 airOrigin,lastScoredLanding,raisedStart,raisedEnd,landing;
 public int flightTricks,activeTrick,lastTrick,combo,mantleStage,stairRoute=-1,stairStep=-1;
 public bool Valid(){
  foreach(float v in new[]{yaw,pitch,visualYaw,touchHeading,vertical,fallApex,airTime,trickElapsed,comboRemaining,mantleElapsed,mantleStalled,mantleCooldown,stairAssist})if(!float.IsFinite(v))return false;
  foreach(var v in new[]{airOrigin,lastScoredLanding,raisedStart,raisedEnd,landing})if(!float.IsFinite(v.x)||!float.IsFinite(v.y)||!float.IsFinite(v.z)||v.sqrMagnitude>1000000)return false;
  return version==1&&vertical>=-200&&vertical<=RaccoonMotor.JumpSpeed+.1f&&fallApex>=-100&&fallApex<=1000&&airTime>=0&&airTime<=120&&flightTricks>=0&&flightTricks<=100&&activeTrick>=0&&activeTrick<=3&&lastTrick>=0&&lastTrick<=3&&trickElapsed>=0&&trickElapsed<=.38f&&combo>=0&&combo<=4;
 }
}
}
