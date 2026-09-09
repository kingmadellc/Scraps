using System.IO;
using UnityEditor;
using UnityEngine;
namespace Jimothy.Editor {
public static class PrepareSurfaces {
 public static void Run(){
  const string dir="Assets/Jimothy/Resources/Surfaces/";
  foreach(var name in new[]{"Brick","Asphalt","Wood"}){
   var source=new Texture2D(2,2,TextureFormat.RGBA32,false,true);source.LoadImage(File.ReadAllBytes(dir+name+"_Roughness.jpg"));
   var pixels=source.GetPixels32();for(int i=0;i<pixels.Length;i++)pixels[i]=new Color32(0,0,0,(byte)(255-pixels[i].r));
   var mask=new Texture2D(source.width,source.height,TextureFormat.RGBA32,false,true);mask.SetPixels32(pixels);mask.Apply();File.WriteAllBytes(dir+name+"_Mask.png",mask.EncodeToPNG());Object.DestroyImmediate(source);Object.DestroyImmediate(mask);
  }
  AssetDatabase.Refresh();ProjectSetup.SetupAssets();AssetDatabase.SaveAssets();
 }
}
}
