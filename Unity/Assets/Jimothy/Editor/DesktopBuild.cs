using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
namespace Jimothy.Editor {
public static class DesktopBuild {
 [MenuItem("Jimothy/Build Mac playtest")]
 public static void Build() {
  ProjectSetup.SetupAssets();NeighborAssetSetup.Prepare();AssetDatabase.SaveAssets();
  if(!File.Exists("Assets/Jimothy/Scenes/Ballard.unity"))ProjectSetup.CreateScene();
  Directory.CreateDirectory("../Builds");
  PlayerSettings.SetArchitecture(UnityEditor.Build.NamedBuildTarget.Standalone,1);
  PlayerSettings.bundleVersion="0.4.7";
  // Runtime fur shader consumes FBX vertex colors absent from the imported placeholder material.
  PlayerSettings.stripUnusedMeshComponents=false;
  PlayerSettings.defaultScreenWidth=1440;
  PlayerSettings.defaultScreenHeight=900;
  PlayerSettings.fullScreenMode=UnityEngine.FullScreenMode.Windowed;
  var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions {
   scenes=new[]{"Assets/Jimothy/Scenes/Ballard.unity"},
   locationPathName="../Builds/Jimothy.app",
   target=BuildTarget.StandaloneOSX,
   options=BuildOptions.Development
  });
  if(report.summary.result!=BuildResult.Succeeded)throw new Exception("Mac playtest build failed: "+report.summary.result);
 }
}
}
