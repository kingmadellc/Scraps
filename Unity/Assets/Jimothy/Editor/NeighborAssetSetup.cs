using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
namespace Jimothy.Editor {
public sealed class NeighborModelImport:AssetPostprocessor {
 static bool IsNeighbor(string path)=>path.EndsWith("/BallardHuman.fbx")||path.EndsWith("/BallardDog.fbx");
 void OnPreprocessModel(){if(!IsNeighbor(assetPath))return;var m=(ModelImporter)assetImporter;m.animationType=ModelImporterAnimationType.Generic;m.importAnimation=true;m.avatarSetup=ModelImporterAvatarSetup.CreateFromThisModel;m.useFileScale=true;m.globalScale=1;m.materialImportMode=ModelImporterMaterialImportMode.ImportStandard;}
 void OnPreprocessAnimation(){if(!IsNeighbor(assetPath))return;var m=(ModelImporter)assetImporter;var clips=m.defaultClipAnimations;foreach(var c in clips){c.loopTime=true;c.lockRootPositionXZ=true;c.lockRootHeightY=true;c.lockRootRotation=true;}m.clipAnimations=clips;}
}
public static class NeighborAssetSetup {
 [MenuItem("Jimothy/Prepare rigged neighbors")]
 public static void Prepare(){
  const string folder="Assets/Jimothy/Resources/";
  foreach(string name in new[]{"BallardHuman","BallardDog"}){
   string model=folder+name+".fbx";AssetDatabase.ImportAsset(model,ImportAssetOptions.ForceUpdate);
   var clips=AssetDatabase.LoadAllAssetsAtPath(model).OfType<AnimationClip>().Where(c=>!c.name.StartsWith("__preview__")).ToArray();
   string path=folder+name+"Controller.controller";var controller=AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
   if(!controller)controller=AnimatorController.CreateAnimatorControllerAtPath(path);
   var machine=controller.layers[0].stateMachine;
   foreach(string action in new[]{"Idle","Walk","Run"}){
    var clip=clips.FirstOrDefault(c=>c.name==action||c.name.EndsWith("|"+action)||c.name.EndsWith("_"+action));
    if(!clip)throw new InvalidOperationException(name+" missing "+action+"; imported: "+string.Join(",",clips.Select(c=>c.name)));
    var state=machine.states.Select(s=>s.state).FirstOrDefault(s=>s.name==action)??machine.AddState(action);state.motion=clip;state.writeDefaultValues=true;
    if(action=="Idle")machine.defaultState=state;
   }
   EditorUtility.SetDirty(controller);
   var prefab=AssetDatabase.LoadAssetAtPath<GameObject>(model);Debug.Log(name+" NPC ready: height="+prefab.GetComponentsInChildren<SkinnedMeshRenderer>().Select(r=>r.bounds.size.y).DefaultIfEmpty(0).Max()+" clips="+string.Join(",",clips.Select(c=>c.name)));
  }
  AssetDatabase.SaveAssets();
 }
}
}
