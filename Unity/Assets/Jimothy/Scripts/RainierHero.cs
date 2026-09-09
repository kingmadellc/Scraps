using UnityEngine;
using UnityEngine.Rendering;
namespace Jimothy {
/// <summary>Authored, geographically inspired northwest massif; scenic compression, not surveyed terrain.</summary>
public sealed class RainierHero:MonoBehaviour {
 Mesh mesh;Material material;
 static readonly Vector2[] Profile={new(-1,0),new(-.85f,.055f),new(-.70f,.18f),new(-.55f,.33f),new(-.40f,.51f),new(-.28f,.68f),new(-.18f,.88f),new(-.12f,.967f),new(-.065f,.95f),new(.005f,1),new(.085f,.985f),new(.17f,.93f),new(.27f,.76f),new(.40f,.56f),new(.56f,.35f),new(.75f,.14f),new(1,0)};
 public const int Across=160,Deep=96;
 public static GameObject Build(Transform parent){var go=new GameObject("Mount Rainier • Liberty Cap, Columbia Crest and glacial ridges");go.transform.SetParent(parent,false);go.transform.localPosition=new Vector3(13,-8,-194);go.AddComponent<RainierHero>().Create();return go;}
 static float Summit(float x){for(int i=1;i<Profile.Length;i++)if(x<=Profile[i].x){float t=Mathf.InverseLerp(Profile[i-1].x,Profile[i].x,x);t=t*t*(3-2*t);return Mathf.Lerp(Profile[i-1].y,Profile[i].y,t);}return 0;}
 static float Height(float x,float z){float depth=Mathf.Exp(-Mathf.Pow(Mathf.Abs(z)/44,1.65f));float h=54*Summit(x/120)*depth;float r=Mathf.Sqrt(x*x+z*z*.9f),angle=Mathf.Atan2(x,z+12);
 // Coherent ridge fans and eroded glacier troughs descend from a broad summit, rather than noise cones.
 float foothold=Mathf.SmoothStep(0,1,Mathf.InverseLerp(10,36,r))*Mathf.Clamp01((h-3)/18);float grooves=Mathf.Pow(.5f+.5f*Mathf.Sin(angle*14+r*.038f),4);h-=grooves*9.5f*foothold;
 h+=(Mathf.PerlinNoise(x*.13f+24,z*.12f+11)-.5f)*2.8f*foothold;h+=(Mathf.PerlinNoise(x*.34f+4,z*.26f+17)-.5f)*1.0f*foothold;return Mathf.Max(0,h);}
 void Create(){int count=(Across+1)*(Deep+1);var vertices=new Vector3[count];var normals=new Vector3[count];var colors=new Color[count];var uv=new Vector2[count];var triangles=new int[Across*Deep*6];int ti=0;
 for(int j=0;j<=Deep;j++)for(int i=0;i<=Across;i++){int k=j*(Across+1)+i;float x=(i/(float)Across-.5f)*240,z=(j/(float)Deep-.5f)*140,h=Height(x,z);vertices[k]=new Vector3(x,h,z);var normal=new Vector3(Height(x-.35f,z)-Height(x+.35f,z),.7f,Height(x,z-.35f)-Height(x,z+.35f)).normalized;normals[k]=normal;uv[k]=new Vector2(x,z);
 float angle=Mathf.Atan2(x,z+12),r=Mathf.Sqrt(x*x+z*z*.9f);float channels=Mathf.Pow(.5f+.5f*Mathf.Sin(angle*14+r*.038f),4);float snowline=22-7*channels+Mathf.PerlinNoise(x*.055f+9,z*.055f+8)*6;float snow=Mathf.SmoothStep(0,1,Mathf.InverseLerp(snowline-2,snowline+4,h));snow*=Mathf.Lerp(.16f,1,Mathf.SmoothStep(0,1,Mathf.InverseLerp(.30f,.79f,normal.y)));float cleaver=Mathf.Pow(.5f+.5f*Mathf.Cos(angle*14+r*.038f),8);float exposedBand=Mathf.SmoothStep(0,1,Mathf.InverseLerp(15,27,h))*(1-Mathf.SmoothStep(0,1,Mathf.InverseLerp(45,57,h)));snow*=1-cleaver*.93f*exposedBand;
 float crag=Mathf.PerlinNoise(x*.17f+5,z*.16f+6);snow*=Mathf.Lerp(.43f,1,Mathf.SmoothStep(0,1,Mathf.InverseLerp(.26f,.55f,crag)))*exposedBand+1-exposedBand;if(h>55)snow=Mathf.Max(snow,.92f);
 Color rock=new Color(.027f,.048f,.077f),ice=new Color(.36f,.46f,.58f);Color white=new Color(.78f,.83f,.89f);colors[k]=Color.Lerp(rock,Color.Lerp(ice,white,Mathf.Clamp01((h-24)/30)),snow);
 if(i<Across&&j<Deep){triangles[ti++]=k;triangles[ti++]=k+Across+1;triangles[ti++]=k+1;triangles[ti++]=k+1;triangles[ti++]=k+Across+1;triangles[ti++]=k+Across+2;}}
 mesh=new Mesh{name="Rainier continuous indexed glacier relief",indexFormat=IndexFormat.UInt32};mesh.vertices=vertices;mesh.normals=normals;mesh.colors=colors;mesh.uv=uv;mesh.triangles=triangles;mesh.RecalculateBounds();gameObject.AddComponent<MeshFilter>().sharedMesh=mesh;material=new Material(Shader.Find("Jimothy/RainierDistance")){name="Moonlit Rainier snow, blue ice and volcanic cleavers"};var renderer=gameObject.AddComponent<MeshRenderer>();renderer.sharedMaterial=material;renderer.shadowCastingMode=ShadowCastingMode.Off;renderer.receiveShadows=false;
 Debug.Log($"RAINIER_HERO vertices={count} triangles={triangles.Length/3}");}
 void OnDestroy(){if(mesh)Destroy(mesh);if(material)Destroy(material);}
}
}
