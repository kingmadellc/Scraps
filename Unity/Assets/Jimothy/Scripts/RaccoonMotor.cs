using UnityEngine;
using UnityEngine.InputSystem;
namespace Jimothy {
[RequireComponent(typeof(CharacterController))]
public class RaccoonMotor : MonoBehaviour {
 public Vector2 touchMove, touchLook;
 public bool touchDirectional; public float touchMoveHeading;
 public float ViewHeading=>yaw;
 public enum AirTrick { FrontFlip, BackFlip, LeftRoll, RightRoll }
 AirTrick requestedAirTrick,activeAirTrick,lastAirTrick;
 public string LastTrickName=>TrickName(lastAirTrick);
 static string TrickName(AirTrick t)=>t==AirTrick.BackFlip?"Backflip":t==AirTrick.LeftRoll?"Left barrel roll":t==AirTrick.RightRoll?"Right barrel roll":"Frontflip";
 public bool jumpRequested, Running;
 public Transform visual;
 public Animator animator;
 public const float MoveSpeed=3.0f, JumpSpeed=8.4f, Gravity=19f;
 public bool Grounded => cc && cc.isGrounded;
 public bool IsMantling => mantling;
 public float VerticalSpeed => vertical;
 public float HorizontalSpeed {get;private set;}
 public int CompletedMantles {get;private set;}
 public int CompletedTricks {get;private set;}
 public float LastFallHeight {get;private set;}
 public float LastFallDamage {get;private set;}
 public bool IsTricking=>trickActive;
 public int LastLandingPoints {get;private set;}
 public bool LastLandingClean {get;private set;}
 Transform trickPivot;bool trickRequested,trickActive,airTracking,gapCrossed,flightMantled;Vector3 airOrigin,lastScoredLanding;float trickElapsed,fallApex,airTime,motorClock,lastCleanLanding=-100;int flightTricks,landingCombo;
 const float TrickDuration=.38f;
 public void RequestTrick(){RequestDirectionalTrick(AirTrick.FrontFlip);}
 public bool RequestDirectionalTrick(AirTrick kind){if(!Running||Grounded||mantling||trickActive||trickRequested||vertical<=-7)return false;requestedAirTrick=kind;trickRequested=true;return true;}
 void EnsureTrickPivot(){if(!visual||trickPivot)return;trickPivot=new GameObject("Jimothy aerial trick pivot").transform;trickPivot.SetParent(transform,false);trickPivot.localPosition=Vector3.up*.28f;visual.SetParent(trickPivot,true);}
 void SetVisualFacing(Quaternion rotation){if(!visual)return;if(trickPivot)visual.localRotation=rotation;else visual.rotation=rotation;}
 Quaternion VisualFacing=>!visual?Quaternion.Euler(0,yaw,0):trickPivot?visual.localRotation:visual.rotation;
 void ApplyTrickVisual(){if(!trickPivot)return;float t=Mathf.Clamp01(trickElapsed/TrickDuration);float smooth=t*t*(3-2*t);trickPivot.localRotation=trickActive?Quaternion.AngleAxis(360*smooth,VisualFacing*(activeAirTrick==AirTrick.BackFlip?Vector3.left:activeAirTrick==AirTrick.LeftRoll?Vector3.forward:activeAirTrick==AirTrick.RightRoll?Vector3.back:Vector3.right)):Quaternion.identity;}
 void ResetAirState(){airTracking=trickRequested=trickActive=false;airTime=trickElapsed=0;flightTricks=0;gapCrossed=flightMantled=false;airOrigin=transform.position;fallApex=transform.position.y;ApplyTrickVisual();}
 CharacterController cc; InputAction move, jump, look, eat, home, pause, trick;
 float vertical, coyote, buffered, yaw=180, pitch=12, mantleIntent, mantleCooldown, mantleElapsed, mantleStalled;
 Vector3 cameraVelocity, mantleRaisedStart, mantleRaisedEnd, mantleLanding;
 bool mantling, cameraSnap=true; int mantleStage;
 Camera cam; int animationState=-1;
 bool animationAirborne; float landingAnimationRemaining;
 readonly Collider[] overlaps=new Collider[24];
 readonly RaycastHit[] castHits=new RaycastHit[24];
 const int WorldMask=~(1<<2);
 public void Initialize(Camera camera) {
  cc=GetComponent<CharacterController>();cc.height=.52f;cc.radius=.17f;cc.center=new Vector3(0,.27f,0);cc.stepOffset=.14f;cc.slopeLimit=48;cc.minMoveDistance=0;cam=camera;
  move=new InputAction("Move",InputActionType.Value);move.AddCompositeBinding("2DVector").With("Up","<Keyboard>/w").With("Down","<Keyboard>/s").With("Left","<Keyboard>/a").With("Right","<Keyboard>/d");move.AddBinding("<Gamepad>/leftStick");
  jump=new InputAction("Jump",InputActionType.Button,"<Keyboard>/space");jump.AddBinding("<Gamepad>/buttonSouth");
  look=new InputAction("Orbit",InputActionType.Value,"<Gamepad>/rightStick");
  eat=new InputAction("Eat",InputActionType.Button,"<Keyboard>/e");home=new InputAction("Home",InputActionType.Button,"<Keyboard>/h");pause=new InputAction("Pause",InputActionType.Button,"<Keyboard>/escape");
  trick=new InputAction("Aerial trick",InputActionType.Button,"<Keyboard>/t");trick.AddBinding("<Gamepad>/buttonEast");
  foreach(var a in new[]{move,jump,look,eat,home,pause,trick}) a.Enable();EnsureTrickPivot();ResetAirState();
 }
 public void FaceDirection(float degrees){yaw=degrees;SetVisualFacing(Quaternion.Euler(0,degrees,0));cameraSnap=true;}
 public void RecenterCamera(){pitch=12;cameraSnap=true;}
 public void Teleport(Vector3 position) {
  if(!cc)cc=GetComponent<CharacterController>();
  cc.enabled=false;transform.position=position;cc.enabled=true;
  vertical=0;coyote=buffered=mantleIntent=0;mantling=false;
  jumpRequested=false;touchMove=touchLook=Vector2.zero;cameraSnap=true;cameraVelocity=Vector3.zero;ResetAirState();landingCombo=0;lastCleanLanding=-100;stairRoute=stairStep=-1;stairAssistTime=0;
 }
 void Update() {
  if(pause!=null && pause.WasPressedThisFrame() && GameSession.Instance)GameSession.Instance.TogglePause();
  if(!Running || move==null){trickRequested=false;jumpRequested=false;touchLook=Vector2.zero;return;}
  float dt=Time.deltaTime;if(dt<=0)return;
  if(eat.WasPressedThisFrame() && GameSession.Instance)GameSession.Instance.Eat();
  if(trick.WasPressedThisFrame())RequestTrick();
  if(home.WasPressedThisFrame() && GameSession.Instance)GameSession.Instance.FastTravel();
  Vector2 orbit=look.ReadValue<Vector2>()*110*dt+touchLook*.16f;touchLook=Vector2.zero;
  if(Mouse.current!=null && Mouse.current.rightButton.isPressed)orbit+=Mouse.current.delta.ReadValue()*.13f;
  Vector2 input=Vector2.ClampMagnitude(move.ReadValue<Vector2>()+touchMove,1);
  float travel=Keyboard.current!=null&&Keyboard.current.leftShiftKey.isPressed?.32f:1f;
  bool pressed=jump.WasPressedThisFrame()||jumpRequested;jumpRequested=false;
  if(touchDirectional)SimulateTouchSteering(touchMove,orbit,pressed,dt);else SimulateSteering(input,orbit,pressed,dt,travel);
 }
 // Keep the movement basis stable for this finger gesture, so camera follow cannot bend a held path into a circle.
 public void SimulateTouchSteering(Vector2 input,Vector2 lookDelta,bool pressed,float dt){
  input=Vector2.ClampMagnitude(input,1);pitch=Mathf.Clamp(pitch-lookDelta.y,-5,40);touchMoveHeading+=lookDelta.x;
  Vector3 direction=Quaternion.Euler(0,touchMoveHeading,0)*new Vector3(input.x,0,input.y);
  float facing=direction.sqrMagnitude>.001f?Mathf.Atan2(direction.x,direction.z)*Mathf.Rad2Deg:yaw+lookDelta.x;
  yaw=direction.sqrMagnitude>.001f?Mathf.MoveTowardsAngle(yaw,facing,220*dt):facing;SimulateMovement(direction,pressed,dt);SetVisualFacing(Quaternion.Euler(0,facing,0));ApplyTrickVisual();
 }
 public void SimulateSteering(Vector2 input,Vector2 lookDelta,bool pressed,float dt,float speedScale=1){
  input=Vector2.ClampMagnitude(input,1);
  pitch=Mathf.Clamp(pitch-lookDelta.y,-5,40);
  yaw+=lookDelta.x+input.x*125f*dt;
  Vector3 direction=Quaternion.Euler(0,yaw,0)*Vector3.forward*(input.y*speedScale);
  SimulateMovement(direction,pressed,dt);
  SetVisualFacing(Quaternion.Euler(0,yaw,0));ApplyTrickVisual();
 }
 // Physics-driven diagnostics call the same movement path as keyboard, gamepad and touch.
 // worldDirection is horizontal, magnitude 0..1. Input release never shortens a tap jump.
 public void SimulateMovement(Vector3 worldDirection,bool pressedJump,float deltaTime) {
  if(!cc)cc=GetComponent<CharacterController>();
  if(deltaTime<=0 || !cc.enabled)return;
  Vector3 direction=Vector3.ClampMagnitude(new Vector3(worldDirection.x,0,worldDirection.z),1);
  // Small integration steps keep low frame rates from changing the jump arc substantially.
  Vector3 priorPosition=transform.position;
  float remaining=Mathf.Min(deltaTime,.12f);bool first=true;
  while(remaining>0){float dt=Mathf.Min(remaining,1f/60f);Step(direction,pressedJump&&first,dt);remaining-=dt;first=false;}
  if(direction.sqrMagnitude>.02f && visual)SetVisualFacing(Quaternion.Slerp(VisualFacing,Quaternion.LookRotation(direction),deltaTime*13));
  Vector3 travelled=transform.position-priorPosition;travelled.y=0;HorizontalSpeed=travelled.magnitude/deltaTime;
  // Jump is a pose sequence sampled by flight phase, never an elapsed-time loop.
  bool airborne=mantling||!cc.isGrounded;
  if(animationAirborne&&!airborne)landingAnimationRemaining=.16f;
  if(airborne)landingAnimationRemaining=0;
  float jumpPhase=airborne?(mantling?.48f:vertical>=0?Mathf.Lerp(.04f,.48f,1-Mathf.Clamp01(vertical/JumpSpeed)):Mathf.Lerp(.48f,.76f,Mathf.Clamp01(-vertical/JumpSpeed))):Mathf.Lerp(.76f,1,1-Mathf.Clamp01(landingAnimationRemaining/.16f));
  int state=airborne||landingAnimationRemaining>0?2:HorizontalSpeed>.08f?(HorizontalSpeed<1.75f?3:1):0;
  if(animator){
   animator.SetFloat("JumpPhase",jumpPhase);
   if(animationState!=state){animator.CrossFade(new[]{"Idle","Waddle","Jump","Walk"}[state],state==2?.055f:.10f);animationState=state;}
   animator.speed=state==1?Mathf.Clamp(HorizontalSpeed/2.145f,.65f,1.6f):state==3?Mathf.Clamp(HorizontalSpeed/.7425f,.35f,1.4f):1;
  }
  ApplyTrickVisual();animationAirborne=airborne;landingAnimationRemaining=Mathf.Max(0,landingAnimationRemaining-deltaTime);
  if(transform.position.y < -12 && GameSession.Instance)GameSession.Instance.Fell();
 }
 void Step(Vector3 direction,bool pressed,float dt) {
  motorClock+=dt;UpdateAirTrick(dt);UpdateStairAssist(dt);
  mantleCooldown=Mathf.Max(0,mantleCooldown-dt);mantleIntent=Mathf.Max(0,mantleIntent-dt);
  if(pressed){buffered=.18f;mantleIntent=.7f;}else buffered=Mathf.Max(0,buffered-dt);
  if(mantling){Vector3 before=transform.position;AdvanceMantle(dt);TrackAirLanding(before,dt,cc.isGrounded&&vertical<=0);return;}
  bool onGround=cc.isGrounded && vertical<=0;
  coyote=onGround?.14f:Mathf.Max(0,coyote-dt);
  if(onGround)vertical=-2;
  if(coyote>0 && buffered>0){vertical=JumpSpeed;coyote=buffered=0;}
  if(onGround&&vertical<=0&&direction.sqrMagnitude>.15f&&stairAssistTime>0&&CanAssistNextTread(direction.normalized)&&TryStairMantle(direction.normalized))return;
  if(!onGround && mantleIntent>0 && mantleCooldown<=0 && direction.sqrMagnitude>.15f && vertical<4.5f && TryMantle(direction.normalized))return;
  Vector3 beforeMove=transform.position;float oldVertical=vertical;vertical-=Gravity*dt;
  var flags=cc.Move((direction*MoveSpeed+Vector3.up*((oldVertical+vertical)*.5f))*dt);
  if((flags&CollisionFlags.Above)!=0 && vertical>0)vertical=0;
  bool landed=(flags&CollisionFlags.Below)!=0&&vertical<0;TrackAirLanding(beforeMove,dt,landed);
  if(landed)vertical=-2;
 }
 // A ray alone cannot break a fall: the controller must have landed and lost downward velocity.
 bool SupportedFeet(){int count=Physics.RaycastNonAlloc(transform.position+Vector3.up*.14f,Vector3.down,castHits,.27f,WorldMask,QueryTriggerInteraction.Ignore);for(int i=0;i<count;i++)if(!IsSelf(castHits[i].collider)&&!(castHits[i].collider is CharacterController)&&castHits[i].normal.y>.67f)return true;return false;}
 void UpdateAirTrick(float dt){if(trickRequested){trickRequested=false;if(!cc.isGrounded&&!mantling&&!trickActive&&vertical>-7){EnsureTrickPivot();trickActive=true;activeAirTrick=requestedAirTrick;trickElapsed=0;}}if(trickActive){trickElapsed+=dt;if(trickElapsed>=TrickDuration){trickActive=false;trickElapsed=0;flightTricks++;CompletedTricks++;lastAirTrick=activeAirTrick;}}}
 void TrackAirLanding(Vector3 before,float dt,bool controllerContact){
  float actualVertical=(transform.position.y-before.y)/dt;
  bool supported=controllerContact&&actualVertical>Mathf.Min(-.35f,vertical*.5f)&&SupportedFeet();
  if(!supported){if(!airTracking){airTracking=true;airOrigin=before;fallApex=before.y;airTime=0;}fallApex=Mathf.Max(fallApex,Mathf.Max(before.y,transform.position.y));airTime+=dt;if(airOrigin.y>=4.5f&&!flightMantled&&!HasCloseGround())gapCrossed=true;return;}
  if(!airTracking)return;
  LastFallHeight=Mathf.Max(0,fallApex-transform.position.y);LastFallDamage=DamageForFall(LastFallHeight);
  bool clean=LastFallDamage<=0&&!trickActive&&!mantling;int points=0;string name="";
  Vector3 span=transform.position-airOrigin;span.y=0;bool transfer=clean&&gapCrossed&&!flightMantled&&airOrigin.y>=4.5f&&transform.position.y>=4.5f&&span.magnitude>2;
  if(clean&&(flightTricks>0||transfer)){landingCombo=motorClock-lastCleanLanding<5&&Vector3.Distance(transform.position,lastScoredLanding)>2?Mathf.Min(landingCombo+1,4):1;lastCleanLanding=motorClock;lastScoredLanding=transform.position;points=(150*flightTricks+(transfer?100:0))*landingCombo;name=flightTricks==0?"Rooftop transfer":flightTricks==1?TrickName(lastAirTrick):flightTricks+" flips";if(transfer&&flightTricks>0)name+=" + rooftop transfer";if(landingCombo>1)name+=" · "+landingCombo+"× combo";}else if(LastFallDamage>0||trickActive)landingCombo=0;
  LastLandingPoints=points;LastLandingClean=clean;
  if(airTime>.08f||LastFallDamage>0){if(GameSession.Instance)GameSession.Instance.ResolveLanding(points,name,LastFallDamage,clean);}
  if(mantleIntent>0||airTime>.08f)FindSupportedStair();ResetAirState();
 }
 bool HasCloseGround(){int count=Physics.RaycastNonAlloc(transform.position+Vector3.up*.10f,Vector3.down,castHits,2.2f,WorldMask,QueryTriggerInteraction.Ignore);for(int i=0;i<count;i++)if(!IsSelf(castHits[i].collider)&&!(castHits[i].collider is CharacterController)&&castHits[i].normal.y>.67f)return true;return false;}
 public static float DamageForFall(float meters)=>Mathf.Clamp((meters-3.5f)*14,0,100);
 int stairRoute=-1,stairStep=-1;float stairAssistTime;
 void FindSupportedStair(){var routes=ClosingTimeWorld.RoofRoutes;for(int r=0;r<routes.Count;r++)for(int i=0;i<routes[r].Length;i++){Vector3 delta=transform.position-routes[r][i];if(Mathf.Abs(delta.y)<.23f&&new Vector2(delta.x,delta.z).sqrMagnitude<.85f*.85f){stairRoute=r;stairStep=i;stairAssistTime=6;return;}}}
 void UpdateStairAssist(float dt){stairAssistTime=Mathf.Max(0,stairAssistTime-dt);if(stairAssistTime<=0||!cc.isGrounded)return;var routes=ClosingTimeWorld.RoofRoutes;if(stairRoute<0||stairRoute>=routes.Count)return;var route=routes[stairRoute];for(int i=stairStep;i<=Mathf.Min(stairStep+1,route.Length-1);i++){Vector3 d=transform.position-route[i];if(Mathf.Abs(d.y)<.23f&&new Vector2(d.x,d.z).sqrMagnitude<.75f*.75f){stairStep=i;stairAssistTime=6;}}}
 bool CanAssistNextTread(Vector3 direction){var routes=ClosingTimeWorld.RoofRoutes;if(stairRoute<0||stairRoute>=routes.Count)return false;var route=routes[stairRoute];if(stairStep<0||stairStep+1>=route.Length)return false;Vector3 d=route[stairStep+1]-transform.position;float rise=d.y;d.y=0;return rise>.2f&&rise<1.15f&&d.magnitude<1.5f&&Vector3.Dot(d.normalized,direction)>.6f;}
 bool TryStairMantle(Vector3 direction){
  // Later authored service steps are open underneath: probe their real tread, not a nonexistent riser wall.
  var target=ClosingTimeWorld.RoofRoutes[stairRoute][stairStep+1]-direction*.38f;
  if(!Physics.Raycast(target+Vector3.up*.25f,Vector3.down,out var hit,.5f,WorldMask,QueryTriggerInteraction.Ignore)||hit.normal.y<.85f||IsSelf(hit.collider)||Mathf.Abs(hit.point.y-target.y)>.15f)return MantleBlocked("authored tread unsupported");
  return BeginCheckedMantle(transform.position,hit.point+Vector3.up*.04f,direction);
 }
 bool IsSelf(Collider collider) => collider==cc || collider.transform.IsChildOf(transform);
 void Capsule(Vector3 at,out Vector3 a,out Vector3 b,out float radius) {
  radius=cc.radius-.025f;Vector3 center=at+cc.center;
  float half=cc.height*.5f-cc.radius;
  a=center+Vector3.up*half;b=center-Vector3.up*half;
 }
 bool ClearAt(Vector3 at) {
  Capsule(at,out var a,out var b,out var radius);
  int count=Physics.OverlapCapsuleNonAlloc(a,b,radius,overlaps,WorldMask,QueryTriggerInteraction.Ignore);
  if(count==overlaps.Length)return false;
  for(int i=0;i<count;i++)if(!IsSelf(overlaps[i]))return false;
  return true;
 }
 bool ClearSweep(Vector3 from,Vector3 to) {
  Vector3 delta=to-from;if(delta.sqrMagnitude<.00001f)return true;
  Capsule(from,out var a,out var b,out var radius);
  int count=Physics.CapsuleCastNonAlloc(a,b,radius,delta.normalized,castHits,delta.magnitude,WorldMask,QueryTriggerInteraction.Ignore);
  if(count==castHits.Length)return false;
  for(int i=0;i<count;i++)if(!IsSelf(castHits[i].collider))return false;
  return true;
 }
 public string LastMantleBlockReason {get;private set;}
 bool MantleBlocked(string reason){LastMantleBlockReason=reason;return false;}
 bool TryMantle(Vector3 direction) {
  Vector3 origin=transform.position;
  if(!Physics.Raycast(origin+Vector3.up*.38f,direction,out var wall,cc.radius+.4f,WorldMask,QueryTriggerInteraction.Ignore))return MantleBlocked("surface probe");
  if(IsSelf(wall.collider) || Mathf.Abs(wall.normal.y)>.25f)return MantleBlocked("wall angle or self");
  Vector3 probe=wall.point+direction*(cc.radius+.18f);probe.y=origin.y+1.3f;
  if(!Physics.Raycast(probe,Vector3.down,out var top,1.1f,WorldMask,QueryTriggerInteraction.Ignore) || top.normal.y<.85f || IsSelf(top.collider))return MantleBlocked("top probe");
  float rise=top.point.y-origin.y;if(rise<.22f || rise>1.15f)return MantleBlocked("rise limit");
  return BeginCheckedMantle(origin,top.point+Vector3.up*.04f,direction);
 }
 bool BeginCheckedMantle(Vector3 origin,Vector3 landing,Vector3 direction){
  // All four edges of the landing footprint must have support on the same level.
  Vector3 side=Vector3.Cross(Vector3.up,direction);
  for(int i=0;i<4;i++){
   Vector3 edge=(i<2?direction:side)*((i%2==0?1:-1)*cc.radius*.85f);
   if(!Physics.Raycast(landing+edge+Vector3.up*.12f,Vector3.down,out var support,.24f,WorldMask,QueryTriggerInteraction.Ignore) || support.normal.y<.85f)return MantleBlocked("footprint support");
  }
  Vector3 raisedStart=new Vector3(origin.x,landing.y+.06f,origin.z);
  Vector3 raisedEnd=landing+Vector3.up*.06f;
  if(!ClearAt(landing))return MantleBlocked("landing occupied");if(!ClearAt(raisedStart)||!ClearAt(raisedEnd))return MantleBlocked("headroom occupied");if(!ClearSweep(origin,raisedStart))return MantleBlocked("rise sweep blocked");if(!ClearSweep(raisedStart,raisedEnd))return MantleBlocked("cross sweep blocked");
  mantleRaisedStart=raisedStart;mantleRaisedEnd=raisedEnd;mantleLanding=landing;
  flightMantled=true;mantling=true;mantleStage=0;mantleElapsed=mantleStalled=0;vertical=0;buffered=coyote=mantleIntent=0;return true;
 }
 void AdvanceMantle(float dt) {
  mantleElapsed+=dt;
  // Collision skin keeps a grounded capsule slightly above the authored landing.
  Vector3 landingOffset=transform.position-mantleLanding;Vector3 horizontal=landingOffset;horizontal.y=0;
  if(mantleStage==2 && cc.isGrounded && horizontal.magnitude<.035f && Mathf.Abs(landingOffset.y)<=cc.skinWidth+.02f){FinishMantle(true);return;}
  if(mantleElapsed>1.5f || mantleStalled>.20f){FinishMantle(false);return;}
  Vector3 goal=mantleStage==0?mantleRaisedStart:mantleStage==1?mantleRaisedEnd:mantleLanding;
  Vector3 before=transform.position;Vector3 next=Vector3.MoveTowards(before,goal,5.5f*dt);
  cc.Move(next-before);
  mantleStalled=Vector3.Distance(before,transform.position)<.001f?mantleStalled+dt:0;
  // A newly arriving obstacle cancels the assist instead of forcing Jimothy through it.
  if(Vector3.Distance(transform.position,next)>.035f){FinishMantle(false);return;}
  if(Vector3.Distance(transform.position,goal)<.015f){
   mantleStage++;
   if(mantleStage>2)FinishMantle(true);
  }
 }
 void FinishMantle(bool completed){mantling=false;mantleCooldown=.3f;vertical=-2;mantleElapsed=mantleStalled=0;if(completed)CompletedMantles++;}
 void LateUpdate() { UpdateFollowCamera(Time.deltaTime); }
 // Heading is shared by character and camera; collision handling never changes its azimuth.
 public void UpdateFollowCamera(float dt) {
  if(!cam)return;
  var target=transform.position+Vector3.up*.32f;
  float heading=VisualFacing.eulerAngles.y;
  float followDistance=ClosingTimeWorld.DenContains(transform.position)?2.35f:3f;
  Vector3 offset=Quaternion.Euler(pitch,heading,0)*new Vector3(0,0,-followDistance);
  float distance=CameraClearance(target,offset);
  if(distance<1.3f){
   for(float elevation=Mathf.Max(pitch+10,35);elevation<=85;elevation+=10){
    var candidate=Quaternion.Euler(elevation,heading,0)*new Vector3(0,0,-followDistance);
    float available=CameraClearance(target,candidate);
    if(available>distance+.005f){offset=candidate;distance=available;}
    if(distance>=2.4f)break;
   }
  }
  // Smooth distance only: smoothing position around a turn produces an unwanted side profile.
  float current=Vector3.Distance(cam.transform.position,target);
  float follow=cameraSnap||distance<current||(current<.65f&&distance>=.65f)?distance:Mathf.Lerp(current,distance,1-Mathf.Exp(-Mathf.Max(0,dt)*12));
  Vector3 position=target+offset.normalized*follow;
  var forward=Quaternion.Euler(0,heading,0)*Vector3.forward;
  cam.transform.position=position;cam.transform.LookAt(target+forward*.75f+Vector3.up*.05f);cameraSnap=false;
  Shader.SetGlobalVector("_JimothySightTarget",new Vector4(target.x,target.y,target.z,1));Shader.SetGlobalVector("_JimothySightCamera",new Vector4(position.x,position.y,position.z,1));
 }
 float CameraClearance(Vector3 target,Vector3 offset){
  float distance=offset.magnitude;int count=Physics.SphereCastNonAlloc(target,.10f,offset.normalized,castHits,distance,WorldMask,QueryTriggerInteraction.Ignore);
  for(int i=0;i<count;i++)if(!IsSelf(castHits[i].collider)&&!castHits[i].collider.GetComponentInParent<NeighborAI>())distance=Mathf.Min(distance,Mathf.Max(.35f,castHits[i].distance-.12f));
  return distance;
 }
 void OnApplicationFocus(bool focused){if(!focused){trickRequested=false;jumpRequested=false;touchMove=touchLook=Vector2.zero;}}
 void OnDestroy(){foreach(var a in new[]{move,jump,look,eat,home,pause,trick})a?.Dispose();}
}
}
