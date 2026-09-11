using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Jimothy.Editor {
public static class ClosingWorldExport {
 static string F(float n)=>n.ToString("R",CultureInfo.InvariantCulture);
 static string V(Vector3 v)=>F(v.x)+" "+F(v.z)+" "+F(v.y);
 [MenuItem("Scraps/Export editable closing-time world")]
 public static void Run(){
  if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play Mode before exporting the authored world.");
  string path=Path.GetFullPath("../Art/Exports/ClosingTime");Directory.CreateDirectory(path);
  var holder=new GameObject("Temporary Blender export");
  try{
   var world=ClosingTimeWorld.Build(holder.transform);RooftopConnections.Build(world.transform);
   var filters=world.GetComponentsInChildren<MeshFilter>();
   var materials=filters.SelectMany(f=>f.GetComponent<Renderer>().sharedMaterials).Distinct().ToArray();
   var names=new Dictionary<Material,string>();var mtl=new StringBuilder();var metadata=new List<SurfaceMetadata>();
   for(int i=0;i<materials.Length;i++){
    var m=materials[i];string name="material_"+i+"_"+m.name.Replace(' ','_');names[m]=name;
    Color c=m.HasProperty("_BaseColor")?m.GetColor("_BaseColor"):Color.white;
    float smooth=m.HasProperty("_Smoothness")?m.GetFloat("_Smoothness"):0;
    mtl.AppendLine("newmtl "+name).AppendLine("Kd "+F(c.r)+" "+F(c.g)+" "+F(c.b));
    mtl.AppendLine("Pr "+F(1-smooth));mtl.AppendLine("Pm "+F(m.IsKeywordEnabled("_METALLICSPECGLOSSMAP")?0:m.HasProperty("_Metallic")?m.GetFloat("_Metallic"):0));
    if(m.HasProperty("_EmissionColor")){var e=m.GetColor("_EmissionColor");mtl.AppendLine("Ke "+F(e.r)+" "+F(e.g)+" "+F(e.b));}
    var texture=m.HasProperty("_BaseMap")?m.GetTexture("_BaseMap"):null;
    var scale=m.HasProperty("_BaseMap")?m.GetTextureScale("_BaseMap"):Vector2.one;var uvOffset=m.HasProperty("_BaseMap")?m.GetTextureOffset("_BaseMap"):Vector2.zero;
    var meta=new SurfaceMetadata{name=name,scaleX=scale.x,scaleY=scale.y,offsetX=uvOffset.x,offsetY=uvOffset.y,smoothnessMultiplier=smooth};
    if(texture){
     string source=AssetDatabase.GetAssetPath(texture);string filename;
     if(!string.IsNullOrEmpty(source)&&File.Exists(source)){filename=Path.GetFileName(source);File.Copy(source,Path.Combine(path,filename),true);}
     else{filename="surface_"+texture.GetInstanceID()+".png";SaveTexture(texture,Path.Combine(path,filename));}
     mtl.AppendLine("map_Kd -s "+F(scale.x)+" "+F(scale.y)+" 1 -o "+F(uvOffset.x)+" "+F(uvOffset.y)+" 0 "+filename);
     string rough=source.Replace("_BaseColor","_Roughness");
     if(rough!=source&&File.Exists(rough)){meta.roughnessFile=Path.GetFileName(rough);File.Copy(rough,Path.Combine(path,meta.roughnessFile),true);mtl.AppendLine("map_Pr "+meta.roughnessFile);}
    }
    var normal=m.HasProperty("_BumpMap")?m.GetTexture("_BumpMap"):null;
    if(normal&&m.IsKeywordEnabled("_NORMALMAP")){string source=AssetDatabase.GetAssetPath(normal);if(File.Exists(source)){meta.normalFile=Path.GetFileName(source);File.Copy(source,Path.Combine(path,meta.normalFile),true);meta.normalStrength=m.GetFloat("_BumpScale");mtl.AppendLine("map_Bump -bm "+F(meta.normalStrength)+" "+meta.normalFile);}}
    metadata.Add(meta);
    mtl.AppendLine();
   }
   File.WriteAllText(Path.Combine(path,"ClosingTime.mtl"),mtl.ToString());File.WriteAllText(Path.Combine(path,"ClosingTime.materials.json"),JsonUtility.ToJson(new SurfaceList{materials=metadata.ToArray()},true));
   var obj=new StringBuilder("# Jimothy exact native world; coordinates converted Unity XYZ to Blender XZY\nmtllib ClosingTime.mtl\n");int offset=1;
   foreach(var filter in filters){var mesh=filter.sharedMesh;if(!mesh.isReadable)throw new InvalidOperationException("Mesh must remain readable for export: "+mesh.name);
    obj.AppendLine("o "+filter.name.Replace(' ','_'));var vertices=mesh.vertices;var normals=mesh.normals;var uv=mesh.uv;
    foreach(var p in vertices)obj.AppendLine("v "+V(filter.transform.TransformPoint(p)));
    for(int i=0;i<vertices.Length;i++){var t=i<uv.Length?uv[i]:Vector2.zero;obj.AppendLine("vt "+F(t.x)+" "+F(t.y));}
    foreach(var n in normals)obj.AppendLine("vn "+V(filter.transform.TransformDirection(n).normalized));
    var slots=filter.GetComponent<Renderer>().sharedMaterials;
    for(int sub=0;sub<mesh.subMeshCount;sub++){obj.AppendLine("usemtl "+names[slots[sub]]);var tris=mesh.GetTriangles(sub);for(int t=0;t<tris.Length;t+=3){int a=offset+tris[t],b=offset+tris[t+2],c=offset+tris[t+1];obj.AppendLine($"f {a}/{a}/{a} {b}/{b}/{b} {c}/{c}/{c}");}}
    offset+=vertices.Length;
   }
   File.WriteAllText(Path.Combine(path,"ClosingTime.obj"),obj.ToString());
   Debug.Log("CLOSING_WORLD_EXPORTED "+path+" meshes="+filters.Length+" materials="+materials.Length);
  }finally{
   // Export owns these editor-only copies. Avoid runtime Destroy callbacks in edit mode.
   foreach(var cleanup in holder.GetComponentsInChildren<ClosingWorldCleanup>()){
    var field=typeof(ClosingWorldCleanup).GetField("ownedMeshes");if(field!=null){var owned=field.GetValue(cleanup) as Mesh[];field.SetValue(cleanup,null);if(owned!=null)foreach(var mesh in owned)if(mesh)UnityEngine.Object.DestroyImmediate(mesh);}
   }
   UnityEngine.Object.DestroyImmediate(holder);
  }
 }
 [Serializable] class SurfaceMetadata{public string name,normalFile,roughnessFile;public float scaleX=1,scaleY=1,offsetX,offsetY,normalStrength=1,smoothnessMultiplier=1;}
 [Serializable] class SurfaceList{public SurfaceMetadata[] materials;}
 static void SaveTexture(Texture source,string path){
  var rt=RenderTexture.GetTemporary(source.width,source.height,0,RenderTextureFormat.ARGB32,RenderTextureReadWrite.sRGB);
  var previous=RenderTexture.active;Texture2D image=null;
  try{Graphics.Blit(source,rt);RenderTexture.active=rt;image=new Texture2D(source.width,source.height,TextureFormat.RGBA32,false);image.ReadPixels(new Rect(0,0,source.width,source.height),0,0);image.Apply();File.WriteAllBytes(path,image.EncodeToPNG());}
  finally{RenderTexture.active=previous;RenderTexture.ReleaseTemporary(rt);if(image)UnityEngine.Object.DestroyImmediate(image);}
 }
}
}
