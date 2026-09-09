using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;
namespace Jimothy.Editor {public static class WorldAudit {
public static void Run(){var o=Object.Instantiate(Resources.Load<GameObject>("OldBallard"));var s=new StringBuilder();foreach(var t in o.GetComponentsInChildren<Transform>()){if(t.name.StartsWith("COL_Den")||t.name.StartsWith("COL_Street")||t.name.StartsWith("COL_Climb_-1_0")||t.name.StartsWith("COL_Building_-1_0")){var r=t.GetComponent<Renderer>();s.AppendLine(t.name+" pos="+t.position+" bounds="+(r?r.bounds.ToString():"none"));}}File.WriteAllText("../world-audit.txt",s.ToString());Object.DestroyImmediate(o);}
}}
