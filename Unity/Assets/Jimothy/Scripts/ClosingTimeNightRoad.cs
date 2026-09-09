using UnityEngine;
namespace Jimothy {
public static partial class ClosingTimeWorld {
 static void RoadQuad(Vector3 a,Vector3 b,Vector3 c,Vector3 d){var batch=GetBatch("streetBrick",(a+c)*.5f);int k=batch.v.Count;foreach(var p in new[]{a,b,c,d}){batch.v.Add(p);batch.n.Add(Vector3.up);batch.uv.Add(new Vector2(p.x,p.z));}batch.t.AddRange(new[]{k,k+2,k+1,k,k+3,k+2});}
 static void NightRoadMaterials(){
  if(mats.ContainsKey("streetBrick"))return;
  const int n=1024;var albedo=new Texture2D(n,n,TextureFormat.RGBA32,true,false){name="Ballard fired-clay pavers"};var normal=new Texture2D(n,n,TextureFormat.RGBA32,true,true){name="Paver bevel and worn grain"};var mask=new Texture2D(n,n,TextureFormat.RGBA32,true,true){name="Paver roughness and mortar AO"};
  var heights=new float[n*n];var colors=new Color32[n*n];var masks=new Color32[n*n];var normals=new Color32[n*n];
  float Hash(int x,int y){unchecked{uint h=(uint)(x*374761393+y*668265263);h=(h^(h>>13))*1274126177;return (h^(h>>16))/4294967295f;}}
  for(int y=0;y<n;y++)for(int x=0;x<n;x++){
   int row=y/64,col=((x+(row%2)*64)%n)/128;float xx=(x+(row%2)*64)%128,yy=y%64;
   float edge=Mathf.Min(xx,127-xx,yy,63-yy),bevel=Mathf.SmoothStep(0,1,(edge-1)/5);
   float grain=Hash(x,y),variation=Hash(col,row),wear=Mathf.PerlinNoise(x*.045f,y*.045f);
   float chips=grain<.025f&&edge<9?.16f:0;
   heights[y*n+x]=bevel*.78f+wear*.09f+(grain-.5f)*.035f-chips;
   Color clay=Color.Lerp(new Color(.26f,.105f,.052f),new Color(.55f,.29f,.14f),variation);
   if(variation>.88f)clay=Color.Lerp(clay,new Color(.30f,.255f,.20f),.5f);
   clay*=.84f+wear*.25f+grain*.08f;var mortar=new Color(.135f,.13f,.105f)*(.8f+grain*.35f);
   colors[y*n+x]=Color.Lerp(mortar,clay,bevel);masks[y*n+x]=new Color(0,Mathf.Lerp(.55f,1,bevel),0,Mathf.Lerp(.18f,.38f+wear*.18f,bevel));
  }
  for(int y=0;y<n;y++)for(int x=0;x<n;x++){
   float dx=heights[y*n+(x+n-1)%n]-heights[y*n+(x+1)%n],dy=heights[((y+n-1)%n)*n+x]-heights[((y+1)%n)*n+x];
   var v=new Vector3(dx*2.8f,dy*2.8f,1).normalized;normals[y*n+x]=new Color(v.x*.5f+.5f,v.y*.5f+.5f,v.z*.5f+.5f,1);
  }
  albedo.SetPixels32(colors);normal.SetPixels32(normals);mask.SetPixels32(masks);
  foreach(var t in new[]{albedo,normal,mask}){t.wrapMode=TextureWrapMode.Repeat;t.filterMode=FilterMode.Trilinear;t.anisoLevel=8;t.Apply(true,true);}
  var material=new Material(Shader.Find("Jimothy/MoonlitPavers")){name="streetBrick",enableInstancing=true};material.SetTexture("_BaseMap",albedo);material.SetTexture("_BumpMap",normal);material.SetTexture("_SurfaceMap",mask);mats.Add("streetBrick",material);
 }
}}
