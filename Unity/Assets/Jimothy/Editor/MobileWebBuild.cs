using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
namespace Jimothy.Editor {
public static class MobileWebBuild {
 public static void Build(){
  ProjectSetup.SetupAssets();PlayerSettings.bundleVersion="0.5.0";
  if(!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.WebGL,BuildTarget.WebGL))throw new InvalidOperationException("Install Web Build Support for this exact Unity Editor first.");
  PlayerSettings.stripUnusedMeshComponents=false;PlayerSettings.WebGL.template="PROJECT:JimothyMobile";PlayerSettings.WebGL.compressionFormat=WebGLCompressionFormat.Disabled;PlayerSettings.WebGL.dataCaching=true;PlayerSettings.WebGL.decompressionFallback=false;
  PlayerSettings.runInBackground=false;PlayerSettings.defaultScreenWidth=1280;PlayerSettings.defaultScreenHeight=720;
  PlayerSettings.SetIl2CppCompilerConfiguration(NamedBuildTarget.WebGL,Il2CppCompilerConfiguration.Release);
  Directory.CreateDirectory("../Builds/Web");
  var result=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{"Assets/Jimothy/Scenes/Ballard.unity"},locationPathName="../Builds/Web",target=BuildTarget.WebGL,options=BuildOptions.None});
  if(result.summary.result!=BuildResult.Succeeded)throw new Exception("Mobile web build failed: "+result.summary.result);
  File.WriteAllText("../Builds/Web/build-info.json",JsonUtility.ToJson(new Info{version=Application.version,unity=Application.unityVersion,bytes=(long)result.summary.totalSize},true));Debug.Log("MOBILE_WEB_BUILD PASS "+result.summary.totalSize+" bytes");
 }
 [Serializable] class Info{public string version,unity;public long bytes;}
}
}
