using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Animations;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace Jimothy.Editor {
public class AssetImport : AssetPostprocessor {
 void OnPreprocessModel() {
  if(!assetPath.Contains("/Jimothy/Resources/"))return;
  var importer=(ModelImporter)assetImporter;importer.useFileScale=true;importer.globalScale=1;importer.materialImportMode=ModelImporterMaterialImportMode.ImportStandard;
  if(assetPath.EndsWith("Jimothy.fbx")){importer.animationCompression=ModelImporterAnimationCompression.Off;importer.animationType=ModelImporterAnimationType.Generic;importer.importAnimation=true;importer.avatarSetup=ModelImporterAvatarSetup.CreateFromThisModel;}
 }
 void OnPostprocessMaterial(Material material) {
  if(!assetPath.Contains("/Jimothy/Resources/"))return;
  var color=material.HasProperty("_Color")?material.GetColor("_Color"):material.color;
  var texture=material.mainTexture;
  material.shader=Shader.Find("Universal Render Pipeline/Lit");material.SetColor("_BaseColor",texture?Color.white:color);if(texture)material.SetTexture("_BaseMap",texture);material.SetFloat("_Smoothness",.24f);
 }
 void OnPreprocessAnimation() {
  if(!assetPath.EndsWith("Jimothy.fbx"))return;var importer=(ModelImporter)assetImporter;var clips=importer.defaultClipAnimations;
  foreach(var clip in clips){clip.loopTime=!clip.name.Contains("Jump");clip.lockRootPositionXZ=true;clip.lockRootHeightY=true;clip.keepOriginalPositionY=true;}
  importer.clipAnimations=clips;
 }
 void OnPreprocessTexture(){
  if(!assetPath.StartsWith("Assets/Jimothy/"))return;
  var importer=(TextureImporter)assetImporter;bool hero=assetPath.Contains("MenuHero");
  if(assetPath.Contains("/Surfaces/")){importer.wrapMode=TextureWrapMode.Repeat;if(assetPath.Contains("_Normal")){importer.textureType=TextureImporterType.NormalMap;importer.sRGBTexture=false;}else importer.sRGBTexture=!(assetPath.Contains("_Roughness")||assetPath.Contains("_Mask"));}
  importer.maxTextureSize=hero?4096:2048;importer.textureCompression=TextureImporterCompression.CompressedHQ;
  if(assetPath.EndsWith("FurStrands.png")){importer.alphaIsTransparency=true;importer.wrapMode=TextureWrapMode.Clamp;importer.mipMapsPreserveCoverage=true;importer.alphaTestReferenceValue=.10f;}
  importer.mipmapEnabled=!hero;importer.isReadable=false;importer.anisoLevel=hero?1:4;
  importer.SetPlatformTextureSettings(new TextureImporterPlatformSettings{name="iPhone",overridden=true,maxTextureSize=2048,format=TextureImporterFormat.ASTC_6x6,compressionQuality=100});
 }
}
public static class ProjectSetup {
 const string ResourcesPath="Assets/Jimothy/Resources/";
 [MenuItem("Scraps/Create or refresh playable scene")]
 public static void CreateScene() {
  if(!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())return;
  SetupAssets();
  var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
  new GameObject("Jimothy game").AddComponent<GameSession>();
  Directory.CreateDirectory("Assets/Jimothy/Scenes");EditorSceneManager.SaveScene(scene,"Assets/Jimothy/Scenes/Ballard.unity");
  EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/Jimothy/Scenes/Ballard.unity",true)};AssetDatabase.SaveAssets();
  Debug.Log("Jimothy scene created. Press Play. iOS export requires the iOS Build Support module and full Xcode.");
 }
 public static void SetupAssets() {
  var heroImporter=AssetImporter.GetAtPath(ResourcesPath+"Jimothy.fbx") as ModelImporter;
  if(heroImporter&&heroImporter.animationCompression!=ModelImporterAnimationCompression.Off){heroImporter.animationCompression=ModelImporterAnimationCompression.Off;heroImporter.SaveAndReimport();}
  var furImporter=AssetImporter.GetAtPath(ResourcesPath+"FurStrands.png") as TextureImporter;
  if(furImporter&&!furImporter.mipMapsPreserveCoverage){furImporter.mipMapsPreserveCoverage=true;furImporter.alphaTestReferenceValue=.10f;furImporter.alphaIsTransparency=true;furImporter.wrapMode=TextureWrapMode.Clamp;furImporter.SaveAndReimport();}
  string pipelinePath=ResourcesPath+"BallardURP.asset";
  var pipeline=AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(pipelinePath);
  if(!pipeline) {
   var renderer=ScriptableObject.CreateInstance<UniversalRendererData>();AssetDatabase.CreateAsset(renderer,ResourcesPath+"BallardRenderer.asset");
   pipeline=UniversalRenderPipelineAsset.Create(renderer);pipeline.msaaSampleCount=2;pipeline.renderScale=.9f;pipeline.shadowDistance=45;pipeline.supportsHDR=false;AssetDatabase.CreateAsset(pipeline,pipelinePath);
  }
  var worldRenderer=AssetDatabase.LoadAssetAtPath<UniversalRendererData>(ResourcesPath+"BallardRenderer.asset");
  var ao=worldRenderer.rendererFeatures.OfType<ScreenSpaceAmbientOcclusion>().FirstOrDefault();
  if(!ao){ao=ScriptableObject.CreateInstance<ScreenSpaceAmbientOcclusion>();ao.name="Soft contact shading";AssetDatabase.AddObjectToAsset(ao,worldRenderer);worldRenderer.rendererFeatures.Add(ao);}
  var aoSettings=new SerializedObject(ao);var config=aoSettings.FindProperty("m_Settings");
  config.FindPropertyRelative("Downsample").boolValue=true;
  config.FindPropertyRelative("Source").enumValueIndex=0;
  config.FindPropertyRelative("Samples").enumValueIndex=2;
  config.FindPropertyRelative("BlurQuality").enumValueIndex=1;
  config.FindPropertyRelative("Intensity").floatValue=.65f;
  config.FindPropertyRelative("Radius").floatValue=.18f;
  config.FindPropertyRelative("Falloff").floatValue=35;
  config.FindPropertyRelative("DirectLightingStrength").floatValue=.12f;
  aoSettings.ApplyModifiedPropertiesWithoutUndo();ao.SetActive(true);EditorUtility.SetDirty(ao);EditorUtility.SetDirty(worldRenderer);
  // Runtime-created PBR/emissive materials need matching assets to retain shader_feature variants in players.
  foreach(string surface in new[]{"Brick","Asphalt","Wood"}){
   var baseMap=AssetDatabase.LoadAssetAtPath<Texture2D>(ResourcesPath+"Surfaces/"+surface+"_BaseColor.jpg");if(!baseMap)continue;
   string path=ResourcesPath+"Surfaces/"+surface+"_Runtime.mat";var material=AssetDatabase.LoadAssetAtPath<Material>(path);
   if(!material){material=new Material(Shader.Find("Universal Render Pipeline/Lit"));AssetDatabase.CreateAsset(material,path);}
   material.SetTexture("_BaseMap",baseMap);material.SetTexture("_BumpMap",AssetDatabase.LoadAssetAtPath<Texture2D>(ResourcesPath+"Surfaces/"+surface+"_Normal.jpg"));material.SetTexture("_MetallicGlossMap",AssetDatabase.LoadAssetAtPath<Texture2D>(ResourcesPath+"Surfaces/"+surface+"_Mask.png"));material.EnableKeyword("_NORMALMAP");material.EnableKeyword("_METALLICSPECGLOSSMAP");EditorUtility.SetDirty(material);
  }
  string emissivePath=ResourcesPath+"ClosingTimeEmissive.mat";var emissive=AssetDatabase.LoadAssetAtPath<Material>(emissivePath);if(!emissive){emissive=new Material(Shader.Find("Universal Render Pipeline/Lit"));AssetDatabase.CreateAsset(emissive,emissivePath);}emissive.EnableKeyword("_EMISSION");emissive.SetColor("_EmissionColor",new Color(.5f,.3f,.12f));EditorUtility.SetDirty(emissive);
  PlayerSettings.colorSpace=ColorSpace.Linear;
  // Shared baseline prioritizes readable night lighting with bounded shadow cost.
  pipeline.msaaSampleCount=2;pipeline.renderScale=.9f;pipeline.shadowDistance=35;
  pipeline.shadowCascadeCount=2;pipeline.mainLightShadowmapResolution=2048;
  pipeline.maxAdditionalLightsCount=8;pipeline.useSRPBatcher=true;
  pipeline.supportsHDR=true;pipeline.supportsCameraDepthTexture=false;pipeline.supportsCameraOpaqueTexture=false;
  var settings=new SerializedObject(pipeline);
  settings.FindProperty("m_AdditionalLightShadowsSupported").boolValue=false;
  settings.FindProperty("m_SoftShadowsSupported").boolValue=true;
  settings.FindProperty("m_SoftShadowQuality").intValue=1;
  settings.ApplyModifiedPropertiesWithoutUndo();EditorUtility.SetDirty(pipeline);
  GraphicsSettings.defaultRenderPipeline=pipeline;QualitySettings.renderPipeline=pipeline;
  QualitySettings.skinWeights=SkinWeights.FourBones;QualitySettings.vSyncCount=0;
  PlayerSettings.stripUnusedMeshComponents=true;
  string controllerPath=ResourcesPath+"JimothyMotion.controller";
  var controller=AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
  if(!controller)controller=AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
  var clips=AssetDatabase.LoadAllAssetsAtPath(ResourcesPath+"Jimothy.fbx").OfType<AnimationClip>().Where(c=>!c.name.StartsWith("__preview__")).ToArray();
  if(!controller.parameters.Any(p=>p.name=="JumpPhase"))controller.AddParameter("JumpPhase",AnimatorControllerParameterType.Float);
  foreach(string name in new[]{"Idle","Waddle","Jump","Walk"}){
   var clip=clips.FirstOrDefault(c=>c.name.Contains(name));if(!clip)throw new System.InvalidOperationException("Missing Blender animation: "+name);
   var state=controller.layers[0].stateMachine.states.Select(s=>s.state).FirstOrDefault(s=>s.name==name)??controller.layers[0].stateMachine.AddState(name);state.motion=clip;
   state.timeParameterActive=name=="Jump";if(name=="Jump")state.timeParameter="JumpPhase";
   if(name=="Idle")controller.layers[0].stateMachine.defaultState=state;
  }
  EditorUtility.SetDirty(controller);
  PlayerSettings.companyName="Ballard Stories";PlayerSettings.productName="Scraps";
  PlayerSettings.SetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.Standalone,"com.Ballard-Stories.Jimothy--Small-Paws--Big-Appetite");
  PlayerSettings.defaultInterfaceOrientation=UIOrientation.LandscapeLeft;
  PlayerSettings.SetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.iOS,"com.ballardstories.jimothy");
  PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.iOS,ScriptingImplementation.IL2CPP);
  PlayerSettings.iOS.targetOSVersionString="16.0";
  PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.iOS,false);PlayerSettings.SetGraphicsAPIs(BuildTarget.iOS,new[]{GraphicsDeviceType.Metal});
  // Avoid old Input Manager dependency. iOS signing/team identity is configured by the owner.
  var playerSettings=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);
  var input=playerSettings.FindProperty("activeInputHandler");if(input!=null){input.intValue=1;playerSettings.ApplyModifiedPropertiesWithoutUndo();}
 }
 [MenuItem("Scraps/Export iOS Xcode project")]
 public static void ExportIOS() {
  SetupAssets();AssetDatabase.SaveAssets();
  if(!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.iOS,BuildTarget.iOS))throw new System.InvalidOperationException("Install Unity iOS Build Support first.");
  if(!File.Exists("Assets/Jimothy/Scenes/Ballard.unity"))CreateScene();
  var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{"Assets/Jimothy/Scenes/Ballard.unity"},locationPathName="Builds/iOS",target=BuildTarget.iOS,options=BuildOptions.Development});
  if(report.summary.result!=UnityEditor.Build.Reporting.BuildResult.Succeeded)throw new System.Exception("iOS export failed: "+report.summary.result);
 }
}
}
