using UnityEngine;
using UnityEngine.EventSystems;
namespace Jimothy {
/// <summary>Each surface owns one finger and only its own input channel.</summary>
public class TouchPad:MonoBehaviour,IPointerDownHandler,IDragHandler,IPointerUpHandler,IInitializePotentialDragHandler {
 public RaccoonMotor motor;public bool orbit;public RectTransform thumb;int owner=int.MinValue;
 public int Owner=>owner;
 public void OnInitializePotentialDrag(PointerEventData e){e.useDragThreshold=false;}
 public void OnPointerDown(PointerEventData e){if(owner!=int.MinValue||!motor||!motor.Running)return;owner=e.pointerId;if(!orbit)Move(e);}
 public void OnDrag(PointerEventData e){if(owner!=e.pointerId||!motor||!motor.Running)return;if(orbit){float height=Mathf.Max(1,Screen.height);motor.touchLook+=Vector2.ClampMagnitude(e.delta,height*.3f)*(810f/height);}else Move(e);}
 void Move(PointerEventData e){var rect=(RectTransform)transform;if(!RectTransformUtility.ScreenPointToLocalPointInRectangle(rect,e.position,e.pressEventCamera,out var point))return;float radius=Mathf.Max(1,Mathf.Min(rect.rect.width,rect.rect.height)*.36f);var value=Vector2.ClampMagnitude((point-rect.rect.center)/radius,1);motor.touchMove=value.magnitude<.10f?Vector2.zero:value;if(thumb)thumb.anchoredPosition=motor.touchMove*radius;}
 public void OnPointerUp(PointerEventData e){if(owner==e.pointerId)Release();}
 public void Release(){owner=int.MinValue;if(thumb)thumb.anchoredPosition=Vector2.zero;if(motor){if(orbit)motor.touchLook=Vector2.zero;else motor.touchMove=Vector2.zero;}}
 void OnDisable(){Release();}
 void OnApplicationFocus(bool focused){if(!focused)Release();}
 void OnApplicationPause(bool paused){if(paused)Release();}
}
public sealed class TouchJump:MonoBehaviour,IPointerDownHandler {
 public RaccoonMotor motor;
 public void OnPointerDown(PointerEventData e){if(motor&&motor.Running)motor.jumpRequested=true;}
 void OnDisable(){if(motor)motor.jumpRequested=false;}
}
}
