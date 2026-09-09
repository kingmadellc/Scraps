using UnityEngine;
namespace Jimothy {
public static partial class ClosingTimeWorld {
 // Landscape frames the west-side bell plaza; east edge remains open to the avenue.
 static void ParkEdges(){
  Mat("parkEarth",C(.12f,.105f,.068f),.02f);Mat("parkGrass",C(.105f,.17f,.077f),.03f);Mat("parkGranite",C(.38f,.40f,.37f),.24f);
  // Ground and paths extend beyond the original block slab. Tops align with the .20 m plaza.
  Box("Garden extension ground",new(-18,-.10f,-56),new(13,.4f,34),"parkGrass",true);
  Box("Garden northern ground",new(-10,-.10f,-68.5f),new(16,.4f,13),"parkGrass",true);
  Box("Garden west promenade",new(-16.2f,.115f,-53),new(2.4f,.19f,23),"parkGranite",true);
  Box("Garden connecting walk",new(-18,.12f,-51),new(8,.20f,2.3f),"parkGranite",true);
  Box("Northern garden walk",new(-10,.115f,-64),new(14,.19f,2.5f),"parkGranite",true);
  Box("Pavilion approach",new(-18.5f,.115f,-66.4f),new(2.5f,.19f,7),"parkGranite",true);
  GardenBed(new(-21,.20f,-44.5f),new(5,0,7));
  GardenBed(new(-21,.20f,-58),new(5,0,8));
  GardenBed(new(-10.5f,.20f,-71),new(12,0,5));
  GardenBed(new(-12.5f,.20f,-39.5f),new(8,0,2));
  foreach(var p in new[]{new Vector3(-23,.24f,-43),new Vector3(-22.5f,.24f,-57),new Vector3(-23,.24f,-62),new Vector3(-15,.24f,-73),new Vector3(-7,.24f,-73)})GardenCedar(p,4.7f);
  foreach(var p in new[]{new Vector3(-19,.24f,-42),new Vector3(-20,.24f,-47),new Vector3(-19,.24f,-56),new Vector3(-20,.24f,-60),new Vector3(-13,.24f,-70),new Vector3(-9,.24f,-71),new Vector3(-5,.24f,-70),new Vector3(-14,.24f,-39.5f)})GardenCedar(p,1.15f);
  foreach(var p in new[]{new Vector3(-19,.24f,-45),new Vector3(-21,.24f,-55),new Vector3(-20,.24f,-62),new Vector3(-11,.24f,-69.4f),new Vector3(-6,.24f,-72),new Vector3(-10,.24f,-39.5f)})Fern(p,.90f);
  GardenPavilion(new(-21,.21f,-69));
  ParkBench(new(-13.4f,.21f,-64),180);ParkBench(new(-16.2f,.21f,-57),90);
  // Shielded warm path lights illuminate the approach, with distance fading.
  foreach(var p in new[]{new Vector3(-15,.21f,-49.5f),new Vector3(-15,.21f,-60.5f),new Vector3(-17,.21f,-65)}){Box("Garden path lamp stem",p+Vector3.up*.35f,new(.11f,.70f,.11f),"iron");Box("Shielded garden lamp",p+Vector3.up*.70f,new(.20f,.08f,.20f),"bulb");Box("Garden lamp cap",p+Vector3.up*.77f,new(.28f,.06f,.28f),"iron");Light(p+Vector3.up*.64f,C(1,.77f,.5f),2.6f,1.1f);}
 }
 static void GardenBed(Vector3 p,Vector3 size){
  // A solid earth volume reaches below the .10 m terrain and meets plant anchors at .24 m.
  Box("Raised garden earth",p+Vector3.up*(-.07f),new(size.x,.22f,size.z),"parkEarth",true);
  // Low segmented granite edging is visibly supported, with no hidden tall collision walls.
  for(int side=-1;side<=1;side+=2){Box("Garden granite border",p+new Vector3(side*size.x*.5f,-.005f,0),new(.18f,.39f,size.z+.18f),"parkGranite",true);Box("Garden granite border",p+new Vector3(0,-.005f,side*size.z*.5f),new(size.x,.39f,.18f),"parkGranite",true);}
 }
 static void GardenCedar(Vector3 p,float height){
  for(int j=0;j<5;j++)Cylinder("Cedar tapered leader",p+Vector3.up*(j*height*.19f),p+Vector3.up*((j+1)*height*.19f),height*(.022f-j*.004f),"wood",7);
  if(height>2){var go=new GameObject("Garden cedar trunk collision");go.transform.SetParent(root,false);go.transform.position=p;var cc=go.AddComponent<CapsuleCollider>();cc.height=height*.94f;cc.radius=.022f*height;cc.center=Vector3.up*height*.47f;}
  int tiers=height>2?8:5;
  for(int tier=0;tier<tiers;tier++){float t=tier/(float)(tiers-1),y=height*(.20f+t*.75f),span=height*(.28f-t*.25f);
   for(int branch=0;branch<7;branch++){var q=Quaternion.Euler(0,branch*51.43f+tier*29,0);var origin=p+Vector3.up*y;var tip=origin+q*new Vector3(0,-height*.05f,span);Cylinder("Drooping cedar spray stem",origin,tip,height*.006f*(1-t*.85f),"wood",5);
    for(int sprig=0;sprig<4;sprig++){float u=.28f+sprig*.22f;var c=Vector3.Lerp(origin,tip,u);for(int leaflet=0;leaflet<6;leaflet++){int sign=leaflet%2==0?-1:1;float off=(leaflet/2)*height*.025f;Leaf(c+q*new Vector3(sign*off,-off*.3f,off),q*Quaternion.Euler(12,sign*48,-sign*12),height*(.084f-t*.057f),height*(.026f-t*.017f),(sprig+tier)%3==0?"leafLight":"leaf");}}
   }
  }
 }
 static void GardenPavilion(Vector3 p){
  Box("Pavilion granite plinth",p+new Vector3(0,.015f,0),new(4.9f,.29f,4.1f),"parkGranite",true);
  foreach(int x in new[]{-1,1})foreach(int z in new[]{-1,1}){Box("Pavilion cedar post",p+new Vector3(x*2.03f,1.55f,z*1.6f),new(.18f,3.0f,.18f),"cedar",true);Box("Pavilion post stone foot",p+new Vector3(x*2.03f,.20f,z*1.6f),new(.32f,.35f,.32f),"parkGranite",true);}
  foreach(int z in new[]{-1,1})Box("Pavilion cross beam",p+new Vector3(0,2.91f,z*1.6f),new(4.6f,.25f,.18f),"cedar");
  foreach(int side in new[]{-1,1}){Box("Pavilion pitched copper roof",p+new Vector3(side*1.23f,3.32f,0),new(2.67f,.11f,4.4f),"copper",false,Quaternion.Euler(0,0,-side*16));for(int rib=-3;rib<=3;rib++)Box("Pavilion standing roof seam",p+new Vector3(side*1.23f,3.40f,rib*.63f),new(2.67f,.035f,.025f),"iron",false,Quaternion.Euler(0,0,-side*16));}
  ParkBench(p+new Vector3(0,.17f,-1.2f),0);
 }
}}
