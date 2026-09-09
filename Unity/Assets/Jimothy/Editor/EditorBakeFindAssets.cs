using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor.SceneManagement;
namespace Jimothy.Editor {
/// <summary>Bakes original catalogue recipes, rejects exact geometry duplicates and captures all IDs.</summary>
public static class EditorBakeFindAssets {
 [Serializable] class Entry {public string id,name,recipe,geometryHash;public int triangles;}
 [Serializable] class Report {public int assets,distinctGeometry;public bool passed;public string[] errors;public Entry[] entries;}
 const string AssetDir="Assets/Jimothy/Resources/FindMeshes";
 [MenuItem("Jimothy/Bake all catalogue find meshes")]
 public static void Run(){var errors=new List<string>();var entries=new List<Entry>();try{
  Directory.CreateDirectory(AssetDir);var catalog=JsonUtility.FromJson<ItemCatalog>(Resources.Load<TextAsset>("Items").text);var hashes=new Dictionary<string,string>();
  foreach(var item in catalog.items){if(!ItemVisuals.HasAuthoredRecipe(item))throw new Exception("Missing recipe: "+item.id);var mesh=ItemVisuals.BuildMesh(item);int tris=mesh.triangles.Length/3;if(tris<8||tris>6500)errors.Add(item.id+" triangle budget: "+tris);foreach(var v in mesh.vertices)if(!float.IsFinite(v.x)||!float.IsFinite(v.y)||!float.IsFinite(v.z))errors.Add(item.id+" nonfinite vertex");string hash=Hash(mesh);if(hashes.TryGetValue(hash,out string prior))errors.Add(item.id+" duplicates geometry of "+prior);else hashes.Add(hash,item.id);
   entries.Add(new Entry{id=item.id,name=item.name,recipe=ItemVisuals.ShapeKey(item),triangles=tris,geometryHash=hash});string path=AssetDir+"/"+item.id+".asset";var old=AssetDatabase.LoadAssetAtPath<Mesh>(path);if(old){EditorUtility.CopySerialized(mesh,old);EditorUtility.SetDirty(old);UnityEngine.Object.DestroyImmediate(mesh);}else AssetDatabase.CreateAsset(mesh,path);
  }
  AssetDatabase.SaveAssets();Capture(catalog.items);Directory.CreateDirectory("../Logs");File.WriteAllText("../Logs/find-mesh-coverage.json",JsonUtility.ToJson(new Report{assets=entries.Count,distinctGeometry=hashes.Count,passed=errors.Count==0&&entries.Count==256,errors=errors.ToArray(),entries=entries.ToArray()},true));if(errors.Count>0)throw new Exception(string.Join("\n",errors));Debug.Log("FIND_MESH_BAKE PASS "+entries.Count+" individually baked meshes; "+hashes.Count+" distinct geometry hashes.");if(Application.isBatchMode)EditorApplication.Exit(0);
 }catch(Exception e){Debug.LogException(e);if(Application.isBatchMode)EditorApplication.Exit(1);}}
 static string Hash(Mesh mesh){using var data=new MemoryStream();using(var writer=new BinaryWriter(data,System.Text.Encoding.UTF8,true)){foreach(var v in mesh.vertices){writer.Write(v.x);writer.Write(v.y);writer.Write(v.z);}foreach(int t in mesh.triangles)writer.Write(t);}using var sha=SHA256.Create();return BitConverter.ToString(sha.ComputeHash(data.ToArray())).Replace("-","");}
 static void Capture(ItemDefinition[] items){EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);RenderSettings.fog=false;RenderSettings.ambientMode=AmbientMode.Flat;RenderSettings.ambientLight=new Color(.55f,.55f,.55f);var lamp=new GameObject("Catalogue soft key");var light=lamp.AddComponent<Light>();light.type=LightType.Directional;light.intensity=1.1f;light.transform.rotation=Quaternion.Euler(40,-30,0);const int tile=240,cols=8,rows=4;Directory.CreateDirectory("../PlaytestCaptures/find-catalogue");var go=new GameObject("Catalogue capture");var cam=go.AddComponent<Camera>();cam.cullingMask=1<<30;cam.clearFlags=CameraClearFlags.SolidColor;cam.backgroundColor=new Color(.055f,.075f,.09f);cam.orthographic=true;cam.nearClipPlane=.01f;cam.farClipPlane=5;var rt=new RenderTexture(tile,tile,24);cam.targetTexture=rt;var read=new Texture2D(tile,tile,TextureFormat.RGB24,false);
  for(int page=0;page<(items.Length+31)/32;page++){var sheet=new Texture2D(tile*cols,tile*rows,TextureFormat.RGB24,false);sheet.SetPixels(Enumerable.Repeat(cam.backgroundColor,tile*cols*tile*rows).ToArray());for(int n=0;n<32&&page*32+n<items.Length;n++){var item=items[page*32+n];var root=ItemVisuals.Create(item,null,"catalogue");root.layer=30;root.transform.position=new Vector3(0,-2400,0);var bounds=root.GetComponent<Renderer>().bounds;cam.orthographicSize=Mathf.Max(.23f,bounds.size.magnitude*.64f);cam.transform.position=bounds.center+new Vector3(.6f,.55f,-.8f);cam.transform.LookAt(bounds.center);var label=new GameObject("ID");label.layer=30;label.transform.position=cam.transform.position+cam.transform.forward*.7f-cam.transform.up*cam.orthographicSize*.79f;label.transform.rotation=cam.transform.rotation;var text=label.AddComponent<TextMesh>();text.text=item.id+"\n"+item.name;text.fontSize=32;text.characterSize=cam.orthographicSize*.045f;text.anchor=TextAnchor.MiddleCenter;text.alignment=TextAlignment.Center;text.color=Color.white;RenderPipeline.SubmitRenderRequest(cam,new RenderPipeline.StandardRequest{destination=rt});var previous=RenderTexture.active;RenderTexture.active=rt;read.ReadPixels(new Rect(0,0,tile,tile),0,0);read.Apply();RenderTexture.active=previous;sheet.SetPixels(n%cols*tile,(rows-1-n/cols)*tile,tile,tile,read.GetPixels());UnityEngine.Object.DestroyImmediate(label);UnityEngine.Object.DestroyImmediate(root);}
   sheet.Apply();File.WriteAllBytes("../PlaytestCaptures/find-catalogue/page-"+(page+1).ToString("00")+".png",sheet.EncodeToPNG());UnityEngine.Object.DestroyImmediate(sheet);}
  cam.targetTexture=null;rt.Release();UnityEngine.Object.DestroyImmediate(rt);UnityEngine.Object.DestroyImmediate(read);UnityEngine.Object.DestroyImmediate(go);UnityEngine.Object.DestroyImmediate(lamp);
 }
}
}
