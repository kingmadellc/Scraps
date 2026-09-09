using System.Collections.Generic;
using UnityEngine;
namespace Jimothy {
/// <summary>Shared static ground grid with per-body clearance and collision-tested connectors.
/// CharacterControllers remain dynamic obstacles, never baked into occupancy.</summary>
public sealed class NeighborNavigation:MonoBehaviour {
 const float Cell=.375f;const int W=161,H=357,N=W*H;const float X0=-30,Z0=-77;const int Mask=1;
 sealed class Clearance {public readonly sbyte[] valid=new sbyte[N];public readonly Vector3[] points=new Vector3[N];}
 static readonly Dictionary<int,Clearance> caches=new();static int active;
 static readonly float[] cost=new float[N];static readonly int[] parent=new int[N],stamp=new int[N],closed=new int[N],heap=new int[N*3];
 static readonly Collider[] overlaps=new Collider[64];static readonly RaycastHit[] casts=new RaycastHit[64];
 static int generation,heapCount;static Vector3 goal;static Clearance planning;
 readonly List<Vector3> path=new();int cursor;float replan,blocked;Vector3 lastTarget;CharacterController cc;Clearance cells;float radius,height,stepHeight;
 float travelSpeed;public Vector3 DesiredHeading {get;private set;}
 public bool HasPath=>cursor<path.Count;
 public bool IsBlocked=>blocked>.45f;
 public float BlockedSeconds=>blocked;
 public bool TargetUnreachable {get;private set;}
 public int SpawnRepairs {get;private set;}
 public float ClearanceRadius=>radius;
 public float ClearanceHeight=>height;
 public void Initialize(CharacterController controller,float bodyRadius=0,float bodyHeight=0){
  NeighborWorldObstacles.Ensure();cc=controller;Vector3 scale=transform.lossyScale;
  radius=Mathf.Ceil(Mathf.Max(bodyRadius,cc.radius*Mathf.Max(scale.x,scale.z)+.045f)*100)/100;
  height=Mathf.Ceil(Mathf.Max(bodyHeight,cc.height*scale.y+.04f)*100)/100;stepHeight=Mathf.Max(.05f,cc.stepOffset*scale.y+.025f);
  int key=Mathf.RoundToInt(radius*100)*1000+Mathf.RoundToInt(height*100);
  if(!caches.TryGetValue(key,out cells)){cells=new Clearance();caches.Add(key,cells);}
  Physics.SyncTransforms();EnsureSafeSpawn();
 }
 void OnEnable(){if(active++==0)caches.Clear();}
 void OnDisable(){active=Mathf.Max(0,active-1);if(active==0)caches.Clear();}
 static bool StaticObstacle(Collider c)=>c&&!c.isTrigger&&!(c is CharacterController)&&!c.GetComponentInParent<NeighborAI>();
 bool Self(Collider c)=>c==cc||c.transform.IsChildOf(transform);
 Vector3 Half=>new(radius,Mathf.Max(.025f,height*.5f-.035f),radius);
 Vector3 Center(Vector3 p)=>p+Vector3.up*(height*.5f+.035f);
 bool Ground(Vector3 at,out Vector3 p){
  p=default;var origin=new Vector3(at.x,1.2f,at.z);int count=Physics.RaycastNonAlloc(origin,Vector3.down,casts,1.8f,Mask,QueryTriggerInteraction.Ignore);float nearest=float.MaxValue;
  for(int j=0;j<count;j++){var hit=casts[j];if(!StaticObstacle(hit.collider)||hit.normal.y<.85f||hit.point.y>.48f||hit.point.y<-.4f||hit.distance>=nearest)continue;nearest=hit.distance;p=hit.point;}
  return nearest<float.MaxValue;
 }
 // Pursuit can use the avenue and curbs, never a crate, staircase, roof, or basement.
 public bool IsGroundTarget(Vector3 at)=>GroundTarget(at,out _);
 // Low jumps stay visible while the chase destination is projected onto the street.
 public bool TryPursuitGround(Vector3 at,out Vector3 floor)=>Ground(at,out floor)&&at.y>=floor.y-.25f&&at.y-floor.y<=2.2f;
 bool GroundTarget(Vector3 at,out Vector3 floor)=>Ground(at,out floor)&&at.y<=.65f&&floor.y<=.48f&&Mathf.Abs(at.y-floor.y)<=.25f;
 public bool TryRestSurface(out RaycastHit surface){surface=default;float nearest=.76f;var eye=transform.position+Vector3.up*1.15f;for(int i=0;i<8;i++){float a=i*Mathf.PI*.25f;int count=Physics.RaycastNonAlloc(eye,new Vector3(Mathf.Sin(a),0,Mathf.Cos(a)),casts,.75f,Mask,QueryTriggerInteraction.Ignore);for(int j=0;j<count;j++){var hit=casts[j];if(!StaticObstacle(hit.collider)||Self(hit.collider)||Mathf.Abs(hit.normal.y)>.15f||hit.distance>=nearest||hit.collider.bounds.max.y<eye.y+.20f)continue;surface=hit;nearest=hit.distance;}}return nearest<.76f;}
 bool VolumeClear(Vector3 p,bool dynamic=false,Vector3 escape=default){
  int count=Physics.OverlapBoxNonAlloc(Center(p),Half,overlaps,Quaternion.identity,Mask,QueryTriggerInteraction.Ignore);if(count==overlaps.Length)return false;
  for(int j=0;j<count;j++){var c=overlaps[j];if(Self(c))continue;if(StaticObstacle(c)&&c.bounds.max.y>p.y+stepHeight)return false;
   if(dynamic&&c is CharacterController&&Vector3.Dot(escape,p-c.transform.position)<=0)return false;
  }return true;
 }
 bool CellPoint(int i,out Vector3 p){p=default;if(i<0||i>=N)return false;if(cells.valid[i]!=0){p=cells.points[i];return cells.valid[i]>0;}
  cells.valid[i]=-1;if(!Ground(new Vector3(X0+i%W*Cell,0,Z0+i/W*Cell),out p)||!VolumeClear(p))return false;cells.points[i]=p;cells.valid[i]=1;return true;
 }
 static int Index(Vector3 p){int x=Mathf.RoundToInt((p.x-X0)/Cell),z=Mathf.RoundToInt((p.z-Z0)/Cell);return x<0||x>=W||z<0||z>=H?-1:z*W+x;}
 int Nearest(Vector3 p,bool connector,int rings=6){int center=Index(p);if(center<0)return -1;int cx=center%W,cz=center/W,best=-1;float distance=float.MaxValue;
  for(int z=-rings;z<=rings;z++)for(int x=-rings;x<=rings;x++){int xx=cx+x,zz=cz+z;if(xx<0||xx>=W||zz<0||zz>=H)continue;int i=zz*W+xx;if(!CellPoint(i,out var at))continue;float d=(new Vector2(at.x-p.x,at.z-p.z)).sqrMagnitude;if(d>=distance||connector&&!ClearSegment(p,at,false))continue;best=i;distance=d;}return best;
 }
 public bool EnsureSafeSpawn(){
  if(!cc||cells==null)return false;if(Ground(transform.position,out var floor)&&VolumeClear(floor)){cc.Move(Vector3.down*.3f);return true;}
  // Placement repair happens once during construction, before the neighbor is shown.
  // Do not route out through a wall or leave its patrol home inside the original overlap.
  int cell=Nearest(transform.position,false,12);if(cell<0){TargetUnreachable=true;return false;}
  cc.enabled=false;transform.position=cells.points[cell]+Vector3.up*.04f;cc.enabled=true;Physics.SyncTransforms();cc.Move(Vector3.down*.1f);SpawnRepairs++;return true;
 }
 bool ClearSegment(Vector3 a,Vector3 b,bool dynamic){
  Vector3 delta=b-a;delta.y=0;float length=delta.magnitude;Vector3 probe=a;probe.y=Mathf.Max(a.y,b.y);
  if(Mathf.Abs(a.y-b.y)>stepHeight+.09f||!VolumeClear(probe,dynamic,delta))return false;if(length<.001f)return VolumeClear(b,dynamic);
  int count=Physics.BoxCastNonAlloc(Center(probe),Half,delta/length,casts,Quaternion.identity,length,Mask,QueryTriggerInteraction.Ignore);if(count==casts.Length)return false;
  for(int j=0;j<count;j++){var c=casts[j].collider;if(Self(c))continue;if(StaticObstacle(c)&&c.bounds.max.y>probe.y+stepHeight)return false;
   if(dynamic&&c is CharacterController&&Vector3.Dot(delta,a-c.transform.position)<=0)return false;
  }return VolumeClear(new Vector3(b.x,probe.y,b.z),dynamic,delta);
 }
 static float Rank(int i)=>cost[i]+Vector3.Distance(planning.points[i],goal);
 static void Push(int i){if(heapCount>=heap.Length)return;int k=heapCount++;while(k>0){int p=(k-1)/2;if(Rank(heap[p])<=Rank(i))break;heap[k]=heap[p];k=p;}heap[k]=i;}
 static int Pop(){int result=heap[0],last=heap[--heapCount],k=0;while(k*2+1<heapCount){int child=k*2+1;if(child+1<heapCount&&Rank(heap[child+1])<Rank(heap[child]))child++;if(Rank(last)<=Rank(heap[child]))break;heap[k]=heap[child];k=child;}heap[k]=last;return result;}
 public bool Plan(Vector3 target){
  path.Clear();cursor=0;TargetUnreachable=true;if(cells==null)return false;
  // A roof target cannot be reached by ground-only neighbors. Do not pace below it.
  if(!GroundTarget(target,out var exact))return false;
  if(VolumeClear(exact)&&ClearSegment(transform.position,exact,false)){path.Add(exact);TargetUnreachable=false;return true;}
  int start=Nearest(transform.position,true),end=Nearest(target,false,4);if(start<0||end<0)return false;
  // Only snap close to a blocked target; avoid chasing an unrelated cell across a building.
  if(Vector2.Distance(new(target.x,target.z),new(cells.points[end].x,cells.points[end].z))>1.2f)return false;
  planning=cells;goal=cells.points[end];generation++;heapCount=0;cost[start]=0;stamp[start]=generation;parent[start]=-1;Push(start);int iterations=0;
  while(heapCount>0&&iterations++<2500){int node=Pop();if(closed[node]==generation)continue;closed[node]=generation;
   if(node==end){for(int i=end;i>=0;i=parent[i])path.Add(cells.points[i]);path.Reverse();if(VolumeClear(exact)&&ClearSegment(path[path.Count-1],exact,false))path.Add(exact);TargetUnreachable=false;return true;}
   int nx=node%W,nz=node/W;
   for(int dz=-1;dz<=1;dz++)for(int dx=-1;dx<=1;dx++){if(dx==0&&dz==0)continue;int x=nx+dx,z=nz+dz;if(x<0||x>=W||z<0||z>=H)continue;int next=z*W+x;
    if(closed[next]==generation||!CellPoint(next,out var p)||Mathf.Abs(p.y-cells.points[node].y)>stepHeight)continue;
    if(dx!=0&&dz!=0&&(!CellPoint(nz*W+x,out _)||!CellPoint(z*W+nx,out _)))continue;
    if(!ClearSegment(cells.points[node],p,false))continue;float nextCost=cost[node]+Vector3.Distance(cells.points[node],p);if(stamp[next]==generation&&nextCost>=cost[next])continue;
    stamp[next]=generation;cost[next]=nextCost;parent[next]=node;Push(next);
   }
  }return false;
 }
 public bool TryPatrolTarget(Vector3 home,float phase,out Vector3 target){
  for(int i=0;i<12;i++){float a=phase+i*2.39996f;Vector3 candidate=home+new Vector3(Mathf.Sin(a),0,Mathf.Cos(a))*(1.7f+(i%3)*.55f);
   if(!Ground(candidate,out candidate)||!VolumeClear(candidate)||Vector2.Distance(new(candidate.x,candidate.z),new(transform.position.x,transform.position.z))<.7f)continue;
   if(Plan(candidate)){target=path[path.Count-1];lastTarget=target;replan=1;return true;}
  }target=transform.position;path.Clear();return false;
 }
 public Vector3 Step(Vector3 target,float speed,float dt,bool stop=false,float turnRate=0){
  if(!cc||dt<=0)return Vector3.zero;dt=Mathf.Min(dt,.1f);Vector3 before=transform.position;replan-=dt;
  if(!GroundTarget(target,out _)||transform.position.y>.70f){path.Clear();cursor=0;TargetUnreachable=true;cc.Move(Vector3.down*5*dt);return Vector3.zero;}
  if(stop){travelSpeed=0;DesiredHeading=Vector3.zero;cc.Move(Vector3.down*5*dt);return Vector3.zero;}
  if(replan<=0&&(Vector3.Distance(lastTarget,target)>.65f||!HasPath||blocked>.4f)){Plan(target);lastTarget=target;replan=TargetUnreachable?1.2f:.65f+(GetInstanceID()&7)*.035f;blocked=0;}
  // Small waypoint tolerance prevents rounding a valid grid corner into its obstacle.
  while(HasPath&&Vector2.Distance(new(transform.position.x,transform.position.z),new(path[cursor].x,path[cursor].z))<.035f)cursor++;
  Vector3 direction=HasPath?path[cursor]-transform.position:Vector3.zero;direction.y=0;DesiredHeading=direction.normalized;bool pivot=false;float effectiveSpeed=speed;
  if(turnRate>0){if(direction.sqrMagnitude>.00001f)transform.rotation=Quaternion.RotateTowards(transform.rotation,Quaternion.LookRotation(direction),turnRate*dt);float angle=direction.sqrMagnitude>.00001f?Vector3.Angle(transform.forward,direction):0;pivot=angle>55;float aligned=pivot?0:Mathf.Pow(Mathf.Max(0,Mathf.Cos(angle*Mathf.Deg2Rad)),2);travelSpeed=Mathf.MoveTowards(travelSpeed,speed*aligned,dt*(aligned<.5f?10:4));effectiveSpeed=pivot?0:travelSpeed;}
  float step=Mathf.Min(effectiveSpeed*dt,direction.magnitude);Vector3 moved=Vector3.zero;
  if(step>.001f){var probe=transform.position+direction.normalized*Mathf.Min(direction.magnitude,step+.12f);probe.y=path[cursor].y;
   if(ClearSegment(transform.position,probe,true))moved=direction.normalized*step;
   else if(turnRate<=0&&ClearSegment(transform.position,probe,false)){
    // Yield only on supported ground and with clearance, including escape from a
    // dynamic overlap. A static obstacle is never sidestepped without a new path.
    var side=Vector3.Cross(Vector3.up,direction.normalized);for(int i=0;i<2;i++){Vector3 candidate=transform.position+side*(i==0?.4f:-.4f);
     if(Ground(candidate,out candidate)&&VolumeClear(candidate)&&ClearSegment(transform.position,candidate,true)){moved=(candidate-transform.position);moved.y=0;moved=moved.normalized*Mathf.Min(speed*.5f*dt,.06f);break;}
    }
   }
  }
  cc.Move(moved+Vector3.down*5*dt);moved=transform.position-before;moved.y=0;
  if(HasPath&&!pivot&&effectiveSpeed>.1f&&moved.magnitude<effectiveSpeed*dt*.12f)blocked+=dt;else blocked=0;
  return moved;
 }
}
}
