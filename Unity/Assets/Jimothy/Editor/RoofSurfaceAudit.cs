using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
namespace Jimothy.Editor {
/// <summary>Checks rendered roof triangles, not only physics colliders.</summary>
public static class RoofSurfaceAudit {
 struct Face {public Vector3 a,b,c;public string material;}
 [Serializable] class Check {public string name;public bool passed;public float top;public string[] materials;}
 [Serializable] class Report {public string method="Sample actual horizontal rendered mesh triangles at10 roof centers and10 access bridge/coping intersections; reject hidden membrane and coincident stone/iron top surfaces. Not a temporal-render or lighting test.";public bool passed;public List<Check> checks=new();}
 static bool Contains(Face f,Vector3 p){float Cross(Vector3 a,Vector3 b,Vector3 c)=>(b.x-a.x)*(c.z-a.z)-(b.z-a.z)*(c.x-a.x);float a=Cross(f.a,f.b,p),b=Cross(f.b,f.c,p),c=Cross(f.c,f.a,p);return(a>=-.0001f&&b>=-.0001f&&c>=-.0001f)||(a<=.0001f&&b<=.0001f&&c<=.0001f);}
 public static void Run(){EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);ClosingTimeWorld.Build(null);NeighborWorldObstacles.Ensure();var faces=new List<Face>();
  foreach(var filter in UnityEngine.Object.FindObjectsByType<MeshFilter>(FindObjectsSortMode.None)){var mesh=filter.sharedMesh;var renderer=filter.GetComponent<MeshRenderer>();if(!mesh||!renderer)continue;var vertices=mesh.vertices;for(int sub=0;sub<mesh.subMeshCount;sub++){var indices=mesh.GetTriangles(sub);for(int i=0;i<indices.Length;i+=3){var a=filter.transform.TransformPoint(vertices[indices[i]]);var b=filter.transform.TransformPoint(vertices[indices[i+1]]);var c=filter.transform.TransformPoint(vertices[indices[i+2]]);if(Mathf.Abs(a.y-b.y)>.0005f||Mathf.Abs(a.y-c.y)>.0005f||Vector3.Cross(b-a,c-a).y<=0)continue;faces.Add(new Face{a=a,b=b,c=c,material=renderer.sharedMaterials[Mathf.Min(sub,renderer.sharedMaterials.Length-1)].name});}}}
  var report=new Report();foreach(int side in new[]{-1,1})for(int row=0;row<5;row++){float h=new[]{7.2f,8.1f,9f,7.2f,8.1f}[row],z=-30+row*16,x=side*14.6f,access=side*(8.1f+(Mathf.RoundToInt(h/.9f)-1)*1.03f);foreach(bool landing in new[]{false,true}){var sample=new Vector3(landing?access:x,h,landing?z+6.165f:z);float top=float.NegativeInfinity;var materials=new HashSet<string>();foreach(var face in faces){if(face.a.y<h+.14f||face.a.y>h+.22f||!Contains(face,sample))continue;if(face.a.y>top+.0005f){top=face.a.y;materials.Clear();}if(Mathf.Abs(face.a.y-top)<.0005f)materials.Add(face.material);}bool expected=materials.Count==1&&(landing?materials.Contains("iron"):materials.Contains("roof"));report.checks.Add(new Check{name=(landing?"Access coping notch ":"Exposed roof membrane ")+side+"/"+row,passed=expected,top=top,materials=new List<string>(materials).ToArray()});}}
  report.passed=report.checks.TrueForAll(c=>c.passed);var path=Path.GetFullPath("../PlaytestCaptures/roof-surface-audit.json");Directory.CreateDirectory(Path.GetDirectoryName(path));File.WriteAllText(path,JsonUtility.ToJson(report,true));Debug.Log("ROOF_SURFACE_AUDIT "+report.passed);if(Application.isBatchMode)EditorApplication.Exit(report.passed?0:2);
 }
}}
