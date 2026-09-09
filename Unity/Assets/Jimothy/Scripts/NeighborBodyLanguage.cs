using System.Collections.Generic;
using UnityEngine;
namespace Jimothy {
/// <summary>Additive joint-only rest poses. Feet/root stay in the locomotion animation.
/// A human reaches a measured wall with two-link arm IK; no unsupported whole-body tilt.</summary>
[DefaultExecutionOrder(100)] public sealed class NeighborBodyLanguage:MonoBehaviour {
 sealed class Joint{public Transform t;public Quaternion baseline;public bool applied;}
 readonly Dictionary<string,Joint> joints=new();NeighborKind kind;bool resting,supported,alert;Vector3 contact,normal;float weight;
 public float LeanWeight=>weight;
 public void Initialize(Transform rig,NeighborKind type){kind=type;foreach(var t in rig.GetComponentsInChildren<Transform>())if(t.name=="spine"||t.name=="head"||t.name.StartsWith("arm_")||t.name.StartsWith("hand_"))joints[t.name]=new Joint{t=t};}
 public void Set(bool rest,bool hasSupport,RaycastHit hit,bool watching){resting=rest;supported=hasSupport;contact=hit.point;normal=hit.normal;alert=watching;}
 void Restore(){foreach(var j in joints.Values)if(j.applied&&j.t){j.t.localRotation=j.baseline;j.applied=false;}}
 public void ResetPose(){Restore();}
 void Update(){Restore();}
 void OnDisable(){Restore();}
 Transform Bone(string name)=>joints.TryGetValue(name,out var j)?j.t:null;
 void LateUpdate(){SamplePose(Time.deltaTime,Time.time);}
 public void SamplePose(float dt,float time){
  foreach(var j in joints.Values)if(j.t){j.baseline=j.t.localRotation;j.applied=true;}
  bool human=kind!=NeighborKind.Dog&&kind!=NeighborKind.Cat&&kind!=NeighborKind.Gull;
  weight=Mathf.MoveTowards(weight,resting&&supported&&human?1:0,dt*2);
  if(human&&weight>.001f){var spine=Bone("spine");var head=Bone("head");Vector3 axis=Vector3.Cross(Vector3.up,-normal).normalized;if(spine)spine.rotation=Quaternion.AngleAxis(weight*7,axis)*spine.rotation;if(head)head.rotation=Quaternion.AngleAxis(-weight*4,axis)*head.rotation;
   var left=Bone("arm_L");var right=Bone("arm_R");if(left&&right){string side=Vector3.Distance(left.position,contact)<Vector3.Distance(right.position,contact)?"L":"R";Reach(Bone("arm_"+side),Bone("arm_"+side+"_fore"),Bone("hand_"+side),contact+normal*.085f,weight);}
  }else if(!human&&resting){var head=Bone("head");if(head){float nod=alert?-3:8+Mathf.Sin(time*1.5f)*3;head.rotation=Quaternion.AngleAxis(nod,transform.right)*head.rotation;head.rotation=Quaternion.AngleAxis(Mathf.Sin(time*.7f)*8,Vector3.up)*head.rotation;}}
 }
 static void Reach(Transform upper,Transform lower,Transform hand,Vector3 destination,float blend){if(!upper||!lower||!hand)return;Vector3 a=upper.position;float l1=Vector3.Distance(a,lower.position),l2=Vector3.Distance(lower.position,hand.position);if(l1<.02f||l2<.02f||Vector3.Distance(a,destination)>l1+l2-.01f)return;var target=Vector3.Lerp(hand.position,destination,blend);Vector3 direction=(target-a).normalized;float distance=Mathf.Clamp(Vector3.Distance(a,target),Mathf.Abs(l1-l2)+.005f,l1+l2-.005f);Vector3 bend=Vector3.ProjectOnPlane(Vector3.down,direction).normalized;if(bend.sqrMagnitude<.01f)bend=Vector3.ProjectOnPlane(Vector3.forward,direction).normalized;float along=(l1*l1-l2*l2+distance*distance)/(2*distance);var elbow=a+direction*along+bend*Mathf.Sqrt(Mathf.Max(0,l1*l1-along*along));upper.rotation=Quaternion.FromToRotation(lower.position-a,elbow-a)*upper.rotation;lower.rotation=Quaternion.FromToRotation(hand.position-lower.position,target-lower.position)*lower.rotation;}
}
}
