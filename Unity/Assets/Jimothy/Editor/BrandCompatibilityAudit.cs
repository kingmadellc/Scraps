using System;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace Jimothy.Editor {
public static class BrandCompatibilityAudit {
 public static void Run(){
  ProjectSetup.SetupAssets();AssetDatabase.SaveAssets();
  if(PlayerSettings.productName!="Scraps")throw new Exception("Wrong display name");
  string expected=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal),"Library","Application Support","Ballard Stories","Jimothy_ Small Paws, Big Appetite","jimothy-v1.json");
  if(SaveStore.PathName!=expected)throw new Exception("Mac save location changed: "+SaveStore.PathName);
  if(PlayerSettings.GetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.Standalone)!="com.Ballard-Stories.Jimothy--Small-Paws--Big-Appetite")throw new Exception("Mac preference identity changed");
  if(PlayerSettings.GetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.iOS)!="com.ballardstories.jimothy")throw new Exception("iOS identity changed");
  Debug.Log("SCRAPS_BRAND_COMPATIBILITY PASS: display name, Mac save path, Mac and iOS identifiers. No user save read or written.");
  if(Application.isBatchMode)EditorApplication.Exit(0);
 }
}
}
