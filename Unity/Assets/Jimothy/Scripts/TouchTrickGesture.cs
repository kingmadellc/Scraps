using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
namespace Jimothy {
/// <summary>A jump press and optional flick share one finger. Camera and move fingers never enter this recognizer.</summary>
public sealed class TouchTrickGesture:MonoBehaviour,IPointerDownHandler,IDragHandler,IPointerUpHandler,IInitializePotentialDragHandler {
 public RaccoonMotor motor;public Text feedback;public Image fill;int owner=int.MinValue,touchId=-1;Vector2 start;float began,feedbackUntil;bool pending,spent;RaccoonMotor.AirTrick choice;
 public string Diagnostic {get;private set;}="idle";
 public int Owner=>owner;public bool Pending=>pending;
 public static bool TryDirection(Vector2 delta,float scale,out RaccoonMotor.AirTrick kind){kind=RaccoonMotor.AirTrick.FrontFlip;if(delta.magnitude<Mathf.Max(1,scale)*34)return false;kind=Mathf.Abs(delta.x)>Mathf.Abs(delta.y)?(delta.x<0?RaccoonMotor.AirTrick.LeftRoll:RaccoonMotor.AirTrick.RightRoll):(delta.y<0?RaccoonMotor.AirTrick.BackFlip:RaccoonMotor.AirTrick.FrontFlip);return true;}
 public void OnInitializePotentialDrag(PointerEventData e){e.useDragThreshold=false;}
 public void OnPointerDown(PointerEventData e){if(owner!=int.MinValue||!motor||!motor.Running)return;Diagnostic="down";owner=e.pointerId;touchId=e is ExtendedPointerEventData extended?extended.touchId:-1;start=e.position;began=Time.unscaledTime;pending=spent=false;if(motor.Grounded)motor.jumpRequested=true;else if(PlayerPrefs.GetInt("touchTapTricks",0)==1){choice=RaccoonMotor.AirTrick.FrontFlip;pending=true;TryCommit();}if(fill)fill.color=new Color(.83f,.68f,.38f,.92f);}
 public void OnDrag(PointerEventData e){Diagnostic="drag "+(e.position-start)+" owner "+owner+" event "+e.pointerId+" spent "+spent;if(owner!=e.pointerId||spent||pending||!motor||!motor.Running)return;float scale=GetComponentInParent<Canvas>().scaleFactor;if(TryDirection(e.position-start,scale,out choice)){pending=true;TryCommit();}}
 void TryCommit(){if(!pending||!motor||!motor.Running)return;if(motor.RequestDirectionalTrick(choice)){Diagnostic="accepted";pending=false;spent=true;Show(choice==RaccoonMotor.AirTrick.FrontFlip?"FRONTFLIP":choice==RaccoonMotor.AirTrick.BackFlip?"BACKFLIP":choice==RaccoonMotor.AirTrick.LeftRoll?"LEFT ROLL":"RIGHT ROLL");}else if(!motor.Grounded&&(motor.IsTricking||motor.VerticalSpeed<=-7)){pending=false;spent=true;Show("LAND FIRST");}}
 void Show(string text){if(feedback)feedback.text=text;feedbackUntil=Time.unscaledTime+1.3f;}
 void Update(){if(owner!=int.MinValue&&touchId>=0&&Touchscreen.current!=null){bool held=false;foreach(var touch in Touchscreen.current.touches)if(touch.touchId.ReadValue()==touchId&&touch.press.isPressed){held=true;break;}if(!held)EndPointer();}if(!motor||!motor.Running){Release();return;}if(pending){if(Time.unscaledTime-began>.45f){pending=false;Show("TRY ON TAKEOFF");}else TryCommit();}if(feedback&&Time.unscaledTime>feedbackUntil)feedback.text=PlayerPrefs.GetInt("touchTapTricks",0)==1?"TAP JUMP · TAP IN AIR TO FLIP":"TAP JUMP · SWIPE TO TRICK";}
 public void OnPointerUp(PointerEventData e){if(owner!=e.pointerId)return;if(!spent&&!pending)OnDrag(e);EndPointer();/* A flick released just before takeoff retains its short, bounded request. */}
 void EndPointer(){owner=int.MinValue;touchId=-1;if(fill)fill.color=new Color(.13f,.27f,.23f,.88f);}
 public void Release(){owner=int.MinValue;pending=spent=false;if(motor)motor.jumpRequested=false;if(fill)fill.color=new Color(.13f,.27f,.23f,.88f);}
 void OnDisable(){Release();}void OnApplicationFocus(bool focused){if(!focused)Release();}void OnApplicationPause(bool paused){if(paused)Release();}
}
}
