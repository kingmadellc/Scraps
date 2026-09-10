using System;
using UnityEngine;
namespace Jimothy {
public static partial class ClosingTimeWorld {
 // Scenic, original grid interpretation, not a surveyed or explorable extension.
 // Four sectors × eight existing shared materials = at most32 additional renderer batches.
 static readonly string[] NeighborhoodPalette={"brickRust","brickPaint","roof","stone","windowDim","window","leaf","asphalt"};
 static Batch NeighborhoodBatch(int sector,int material){string key="neighborhood_"+sector+"_"+NeighborhoodPalette[material];if(!batches.TryGetValue(key,out var batch)){batch=new Batch{material=mats[NeighborhoodPalette[material]]};batches.Add(key,batch);}return batch;}
 static void NeighborhoodQuad(Batch batch,Vector3 center,Vector3 u,Vector3 v,Vector3 normal){int i=batch.v.Count;batch.v.AddRange(new[]{center-u-v,center+u-v,center+u+v,center-u+v});batch.n.AddRange(new[]{normal,normal,normal,normal});float w=u.magnitude*2,h=v.magnitude*2;batch.uv.AddRange(new[]{Vector2.zero,new Vector2(w,0),new Vector2(w,h),new Vector2(0,h)});batch.t.AddRange(new[]{i,i+1,i+2,i,i+2,i+3});}
 static void NeighborhoodBox(int sector,int material,Vector3 p,Vector3 size){var b=NeighborhoodBatch(sector,material);var h=size*.5f;
  NeighborhoodQuad(b,p+Vector3.right*h.x,Vector3.back*h.z,Vector3.up*h.y,Vector3.right);
  NeighborhoodQuad(b,p+Vector3.left*h.x,Vector3.forward*h.z,Vector3.up*h.y,Vector3.left);
  NeighborhoodQuad(b,p+Vector3.forward*h.z,Vector3.right*h.x,Vector3.up*h.y,Vector3.forward);
  NeighborhoodQuad(b,p+Vector3.back*h.z,Vector3.left*h.x,Vector3.up*h.y,Vector3.back);
  NeighborhoodQuad(b,p+Vector3.up*h.y,Vector3.right*h.x,Vector3.back*h.z,Vector3.up);
 }
 static void NeighborhoodCanopy(int sector,Vector3 p,float radius,float rise){var b=NeighborhoodBatch(sector,6);const int count=7;for(int k=0;k<count;k++){float a=k*Mathf.PI*2/count,a2=(k+1)*Mathf.PI*2/count;var lower=p+new Vector3(Mathf.Cos(a)*radius,0,Mathf.Sin(a)*radius);var upper=p+Vector3.up*rise;var next=p+new Vector3(Mathf.Cos(a2)*radius,0,Mathf.Sin(a2)*radius);int i=b.v.Count;var normal=Vector3.Cross(upper-lower,next-lower).normalized;b.v.AddRange(new[]{lower,upper,next});b.n.AddRange(new[]{normal,normal,normal});b.uv.AddRange(new[]{Vector2.zero,Vector2.right,Vector2.up});b.t.AddRange(new[]{i,i+1,i+2});}
  NeighborhoodBox(sector,2,p-Vector3.up*1.4f,new(.24f,2.8f,.24f));
 }
 static void NeighborhoodBuilding(int sector,Vector3 origin,float width,float depth,float height,int style,System.Random rng){
  int masonry=style%2;NeighborhoodBox(sector,masonry,origin+Vector3.up*height*.5f,new(width,height,depth));
  NeighborhoodBox(sector,3,origin+Vector3.up*(height+.08f),new(width+.38f,.20f,depth+.38f));
  NeighborhoodBox(sector,2,origin+Vector3.up*(height+.20f),new(width-.28f,.12f,depth-.28f));
  if(style%3==0)NeighborhoodBox(sector,3,origin+Vector3.up*(height-.55f),new(width+.16f,.16f,depth+.16f));
  // Continuous roof strips read as parapets, without extra tiny objects or colliders.
  foreach(int sign in new[]{-1,1}){NeighborhoodBox(sector,masonry,origin+new Vector3(sign*(width*.5f-.10f),height+.40f,0),new(.2f,.46f,depth));NeighborhoodBox(sector,masonry,origin+new Vector3(0,height+.40f,sign*(depth*.5f-.10f)),new(width,.46f,.2f));}
  int floors=Mathf.Clamp(Mathf.RoundToInt((height-2)/3.15f),1,5),across=Mathf.Max(2,Mathf.FloorToInt(width/3.3f)),along=Mathf.Max(2,Mathf.FloorToInt(depth/3.4f));
  for(int floor=0;floor<floors;floor++){float y=2.05f+floor*3.05f;foreach(int face in new[]{-1,1}){
   for(int k=0;k<across;k++){float x=(k+.5f)*width/across-width*.5f;var center=origin+new Vector3(x,y,face*(depth*.5f+.015f));int mat=rng.Next(8)==0?5:4;NeighborhoodQuad(NeighborhoodBatch(sector,mat),center,Vector3.right*(face*.47f),Vector3.up*.73f,Vector3.forward*face);}
   for(int k=0;k<along;k++){float z=(k+.5f)*depth/along-depth*.5f;var center=origin+new Vector3(face*(width*.5f+.015f),y,z);int mat=rng.Next(9)==0?5:4;NeighborhoodQuad(NeighborhoodBatch(sector,mat),center,Vector3.back*(face*.47f),Vector3.up*.73f,Vector3.right*face);}
  }}
  // A few readable service shapes produce varied silhouettes from the playable roofs.
  var service=origin+new Vector3(width*.21f,height+.58f,-depth*.23f);NeighborhoodBox(sector,2,service,new(2.0f,.72f,1.3f));
  NeighborhoodBox(sector,masonry,origin+new Vector3(-width*.28f,height+.8f,depth*.25f),new(.75f,1.4f,.8f));
  if(style%5==0){var tank=origin+new Vector3(width*.2f,height+2.0f,depth*.2f);NeighborhoodBox(sector,2,tank,new(1.6f,2.0f,1.6f));NeighborhoodBox(sector,3,tank+Vector3.up*1.05f,new(1.85f,.12f,1.85f));foreach(int x in new[]{-1,1})foreach(int z in new[]{-1,1})NeighborhoodBox(sector,2,tank+new Vector3(x*.62f,-1.35f,z*.62f),new(.08f,.70f,.08f));}
 }
 static void DistantNeighborhood(){
  var rng=new System.Random(440803);int buildings=0,plots=0,trees=0;
  // Orthogonal streets and paired lots suggest Ballard's neighborhood rhythm.
  // Keep scenery well outside den, park, alleys and all playable rooftops.
  for(int iz=-5;iz<=5;iz++)for(int ix=-5;ix<=5;ix++){
   float x=ix*40f,z=iz*40f;float radius=Mathf.Sqrt(x*x+z*z);if(radius<35||radius>225)continue;
   // Transition blocks start behind the existing shop backs; the central north
   // lot begins beyond the basement. Never place scenery on the playable axis.
   if(Mathf.Abs(x)<30 && z<75)continue;
   // Clear the summit/plane corridor from the avenue and north rooftops.
   if(z< -75&&Mathf.Abs(x-13)<=53)continue;
   int sector=Mathf.Abs(x)>Mathf.Abs(z)?(x<0?0:1):(z>0?2:3);plots++;
   // The north scenery tile used to project 3 m through the basement at street height.
   // Clip its south edge beyond the annex, while preserving the distant buildings/streets.
   float groundSouth=Mathf.Abs(x)<30&&z>75?Mathf.Max(64,z-19.5f):z-19.5f;
   float groundNorth=z+19.5f;
   NeighborhoodBox(sector,7,new(x,-.16f,(groundSouth+groundNorth)*.5f),new(39,.18f,groundNorth-groundSouth));
   NeighborhoodBox(sector,3,new(x,-.025f,z),new(29,.12f,29));
   int configuration=rng.Next(3);float width=configuration==0?12.2f:10.8f,depth=12+rng.Next(5);
   for(int lot=0;lot<2;lot++){
    float bx=x+(lot==0?-7.3f:7.3f),bz=z+(configuration==1?(lot==0?-4f:4f):0);float height=6.4f+rng.Next(4)*3.05f+(float)rng.NextDouble()*.8f;
    // Southern silhouettes remain lower than the mountain's broad snowy shoulders.
    if(sector==3)height=Mathf.Min(height,9.8f);
    NeighborhoodBuilding(sector,new(bx,0,bz),width,depth,height,rng.Next(15),rng);buildings++;
   }
   if((ix+iz)%2==0)foreach(int edge in new[]{-1,1}){var p=new Vector3(x+edge*16.3f,3.1f,z+9);NeighborhoodCanopy(sector,p,2.1f+(float)rng.NextDouble()*.8f,1.7f);trees++;}
  }
  Debug.Log("DISTANT_NEIGHBORHOOD plots="+plots+" buildings="+buildings+" canopies="+trees+" maxBatches=32 noColliders=true range30-245m");
 }
}
}
