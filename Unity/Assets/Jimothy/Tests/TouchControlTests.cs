using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
namespace Jimothy.Tests {
public class TouchControlTests {
 [Test] public void ReleasingLookFingerDoesNotCancelHeldMovement(){var g=new GameObject("motor");var m=g.AddComponent<RaccoonMotor>();m.Running=true;var p=new GameObject("look",typeof(RectTransform)).AddComponent<TouchPad>();p.motor=m;p.orbit=true;try{p.OnPointerDown(new PointerEventData(null){pointerId=2});m.touchMove=Vector2.up;m.touchLook=Vector2.one;p.OnPointerUp(new PointerEventData(null){pointerId=2});Assert.AreEqual(Vector2.up,m.touchMove);Assert.AreEqual(Vector2.zero,m.touchLook);}finally{Object.DestroyImmediate(p.gameObject);Object.DestroyImmediate(g);}}
 [Test] public void ForeignFingerCannotReleaseOrStealLook(){var g=new GameObject("motor");var m=g.AddComponent<RaccoonMotor>();m.Running=true;var p=new GameObject("look",typeof(RectTransform)).AddComponent<TouchPad>();p.motor=m;p.orbit=true;try{p.OnPointerDown(new PointerEventData(null){pointerId=2});p.OnPointerDown(new PointerEventData(null){pointerId=3});m.touchLook=Vector2.one;p.OnPointerUp(new PointerEventData(null){pointerId=3});Assert.AreEqual(2,p.Owner);Assert.AreEqual(Vector2.one,m.touchLook);p.Release();Assert.AreEqual(int.MinValue,p.Owner);Assert.AreEqual(Vector2.zero,m.touchLook);}finally{Object.DestroyImmediate(p.gameObject);Object.DestroyImmediate(g);}}
 [Test] public void JumpPressWorksWhileMovingAndPausedPressDoesNotQueue(){var g=new GameObject("motor");var m=g.AddComponent<RaccoonMotor>();var j=new GameObject("jump").AddComponent<TouchJump>();j.motor=m;try{m.Running=true;m.touchMove=Vector2.up;j.OnPointerDown(new PointerEventData(null){pointerId=3});Assert.IsTrue(m.jumpRequested);Assert.AreEqual(Vector2.up,m.touchMove);m.Running=false;m.jumpRequested=false;j.OnPointerDown(new PointerEventData(null){pointerId=3});Assert.IsFalse(m.jumpRequested);}finally{Object.DestroyImmediate(j.gameObject);Object.DestroyImmediate(g);}}
}
}
