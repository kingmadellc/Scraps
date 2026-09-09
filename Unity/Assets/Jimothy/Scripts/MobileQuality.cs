using UnityEngine;
using UnityEngine.Rendering.Universal;
namespace Jimothy {
/// <summary>Conservative baseline. Actual thermal/frame budgets require device profiling.</summary>
public static class MobileQuality {
 public static void Apply() {
  int requested=PlayerPrefs.GetInt("fps",Application.isMobilePlatform?30:60);
  Application.targetFrameRate=requested<=30?30:60;QualitySettings.vSyncCount=0;
  QualitySettings.skinWeights=SkinWeights.FourBones;
  var renderer=Resources.Load<UniversalRendererData>("BallardRenderer");if(renderer)foreach(var feature in renderer.rendererFeatures)if(feature is ScreenSpaceAmbientOcclusion)feature.SetActive(!Application.isMobilePlatform || requested>30);
 }
 public static void ConfigureCamera(Camera camera) {
  camera.allowMSAA=true;camera.allowHDR=true;camera.nearClipPlane=.12f;
  // The skyline remains visible; detailed street props can be culled much nearer.
  camera.farClipPlane=450;
 }
}
}
