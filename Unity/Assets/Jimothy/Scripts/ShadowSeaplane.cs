using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
namespace Jimothy {
/// <summary>A distant high-wing floatplane. No collider, real-time lights, audio or per-frame allocation.</summary>
public sealed class ShadowSeaplane:MonoBehaviour {
 readonly List<Vector3> vertices=new();readonly List<int> triangles=new();Mesh mesh;Material material;Renderer body;float elapsed;
 public const float FirstPass=18,Period=142,PassDuration=38;
 public static ShadowSeaplane Build(Transform parent){var go=new GameObject("Distant harbor floatplane");go.transform.SetParent(parent,false);var flight=go.AddComponent<ShadowSeaplane>();flight.Create();return flight;}
 void Loft(Vector3 center,float length,float width,float height,int rings=9,int sides=12){int start=vertices.Count;for(int ring=0;ring<=rings;ring++){float t=ring/(float)rings;float shape=Mathf.Pow(Mathf.Max(0,Mathf.Sin(t*Mathf.PI)),.45f);for(int i=0;i<sides;i++){float angle=i*Mathf.PI*2/sides;vertices.Add(center+new Vector3(Mathf.Cos(angle)*width*.5f*shape,Mathf.Sin(angle)*height*.5f*shape,(t-.5f)*length));}}for(int r=0;r<rings;r++)for(int s=0;s<sides;s++){int a=start+r*sides+s,b=start+r*sides+(s+1)%sides,c=a+sides,d=b+sides;triangles.AddRange(new[]{a,b,c,b,d,c});}}
 void Box(Vector3 center,Vector3 size,Quaternion rotation){int start=vertices.Count;for(int z=-1;z<=1;z+=2)for(int y=-1;y<=1;y+=2)for(int x=-1;x<=1;x+=2)vertices.Add(center+rotation*Vector3.Scale(new Vector3(x,y,z),size*.5f));int[] index={0,2,3,0,3,1,4,5,7,4,7,6,0,4,6,0,6,2,1,3,7,1,7,5,2,6,7,2,7,3,0,1,5,0,5,4};foreach(int i in index)triangles.Add(start+i);}
 void Strut(Vector3 a,Vector3 b,float diameter){Box((a+b)*.5f,new Vector3(diameter,diameter,Vector3.Distance(a,b)),Quaternion.LookRotation(b-a));}
 void Create(){
 Loft(Vector3.zero,6.6f,1.05f,1.25f,12,16);Loft(new Vector3(0,.31f,.8f),2.5f,1.13f,1.20f,8,12);
 // Broad high wing, narrower tailplane and vertical fin produce the recognizable floatplane silhouette.
 Box(new Vector3(0,.9f,.55f),new Vector3(9,.13f,1.43f),Quaternion.identity);Box(new Vector3(0,.30f,-2.45f),new Vector3(3.45f,.10f,.78f),Quaternion.identity);Box(new Vector3(0,.90f,-2.40f),new Vector3(.12f,1.40f,.90f),Quaternion.Euler(-15,0,0));
 foreach(int side in new[]{-1,1}){Loft(new Vector3(side*.98f,-1.35f,.23f),5.55f,.54f,.63f,10,10);Strut(new Vector3(side*.42f,-.32f,1.0f),new Vector3(side*.98f,-1.09f,1.12f),.08f);Strut(new Vector3(side*.35f,-.33f,-1.05f),new Vector3(side*.98f,-1.09f,-.80f),.08f);Strut(new Vector3(side*.46f,-.26f,.2f),new Vector3(side*3.15f,.82f,.35f),.065f);}
 Strut(new Vector3(-.98f,-1.12f,.85f),new Vector3(.98f,-1.12f,.85f),.065f);Box(new Vector3(0,0,3.22f),new Vector3(.13f,2.2f,.055f),Quaternion.Euler(0,0,28));
 mesh=new Mesh{name="High wing harbor floatplane with rounded pontoons and struts"};mesh.SetVertices(vertices);mesh.SetTriangles(triangles,0);mesh.RecalculateNormals();mesh.RecalculateBounds();gameObject.AddComponent<MeshFilter>().sharedMesh=mesh;material=new Material(Shader.Find("Universal Render Pipeline/Lit")){name="Distant charcoal aircraft"};material.SetColor("_BaseColor",new Color(.043f,.054f,.067f));material.SetFloat("_Smoothness",.22f);body=gameObject.AddComponent<MeshRenderer>();body.sharedMaterial=material;body.shadowCastingMode=ShadowCastingMode.Off;body.receiveShadows=false;body.enabled=false;
 Debug.Log($"SHADOW_SEAPLANE vertices={vertices.Count} triangles={triangles.Count/3} firstPass={FirstPass}s period={Period}s");}
 public static Vector3 FlightPosition(float t){return new Vector3(Mathf.Lerp(-108,112,t),43+Mathf.Sin(t*Mathf.PI)*4,Mathf.Lerp(-117,-161,t));}
 void Update(){var game=GameSession.Instance;if(!game||!game.Playing||game.Paused||game.DenOpen)return;elapsed+=Time.deltaTime;float phase=elapsed-FirstPass;bool visible=phase>=0&&phase%Period<PassDuration;body.enabled=visible;if(!visible)return;float t=(phase%Period)/PassDuration;transform.localPosition=FlightPosition(t);var direction=FlightPosition(Mathf.Min(t+.001f,1.001f))-transform.localPosition;transform.localRotation=Quaternion.LookRotation(direction)*Quaternion.Euler(0,0,Mathf.Sin(t*Mathf.PI*2)*4);}
 void OnDestroy(){if(mesh)Destroy(mesh);if(material)Destroy(material);}
}
}
