using UnityEngine;
namespace Jimothy {
public static partial class ClosingTimeWorld {
 static void BotanicalMaterials(){
  Mat("flowerViolet",C(.47f,.36f,.64f),.22f);Mat("flowerPink",C(.76f,.38f,.47f),.22f);Mat("flowerCream",C(.90f,.82f,.60f),.19f);
 }
 // Sword fern: each curved rachis carries paired, tapering pinnae, with uneven tips.
 static void SwordFern(Vector3 p,float scale){
  for(int arm=0;arm<9;arm++){
   var q=Quaternion.Euler(0,arm*137.508f,0);float reach=scale*(.56f+(arm%3)*.065f),rise=scale*(.34f+(arm%4)*.045f);
   Vector3 At(float t)=>p+q*new Vector3(0,Mathf.Sin(t*Mathf.PI*.85f)*rise,t*reach);
   for(int k=0;k<9;k++)Cylinder("Sword fern curved rachis",At(k/9f),At((k+1)/9f),.004f*scale,"moss",4);
   for(int k=1;k<11;k++){float t=k/11f;float length=scale*(.19f*Mathf.Sin(t*Mathf.PI)*(.95f+(arm%3)*.06f)+.025f);foreach(int side in new[]{-1,1}){
    var orientation=q*Quaternion.Euler(-19+t*28,side*(64-t*15),side*13);var leafAt=At(t)+q*new Vector3(side*length*.35f,0,length*.11f);
    Leaf(leafAt,orientation,length,.048f*scale*(1-t*.5f),k%4==0?"leafLight":"leaf");
   }}
   Leaf(At(.94f),q*Quaternion.Euler(24,0,0),scale*.15f,scale*.027f,"leafLight");
  }
 }
 static void Hosta(Vector3 p,float scale){
  for(int k=0;k<11;k++){var q=Quaternion.Euler(-25+(k%4)*12,k*137.5f,0);var tip=p+Quaternion.Euler(0,k*137.5f,0)*new Vector3(0,.20f*scale,.19f*scale);Cylinder("Hosta petiole",p,tip,.009f*scale,"moss",4);Leaf(tip,q,.49f*scale,.23f*scale,k%3==0?"leafLight":"leaf");}
 }
 static void Hydrangea(Vector3 p,float scale,int variant=0){
  BotanicalMaterials();Hosta(p,scale*.9f);
  for(int j=0;j<5;j++){float angle=j*2.4f;var c=p+new Vector3(Mathf.Cos(angle)*.20f,.35f+(j%2)*.12f,Mathf.Sin(angle)*.20f)*scale;Cylinder("Hydrangea woody stem",p,c,.012f*scale,"wood",5);
   for(int k=0;k<13;k++){float a=k*2.39996f,r=.16f*Mathf.Sqrt(k/13f);var flower=c+new Vector3(Mathf.Cos(a)*r,Mathf.Sqrt(Mathf.Max(0,.032f-r*r))*.65f,Mathf.Sin(a)*r)*scale;
    for(int petal=0;petal<4;petal++)Leaf(flower+Quaternion.Euler(0,petal*90,0)*new Vector3(0,0,.022f*scale),Quaternion.Euler(-10,petal*90,0),.063f*scale,.041f*scale,variant%3==0?"flowerViolet":variant%3==1?"flowerPink":"flowerCream");
   }
  }
 }
 static void PlantPot(Vector3 p,float scale,int kind=0){
  // Stand on a real shelf/ground surface; ellipsoid bottom is at p.y.
  Ellipsoid(p+Vector3.up*.15f*scale,new Vector3(.18f,.15f,.18f)*scale,"terracotta",12,6);Cylinder("Plant pot rolled lip",p+Vector3.up*.27f*scale,p+Vector3.up*.30f*scale,.19f*scale,"terracotta",14);Ellipsoid(p+Vector3.up*.285f*scale,new Vector3(.165f,.012f,.165f)*scale,"moss",12,3);
  if(kind==0)SwordFern(p+Vector3.up*.30f*scale,scale*.62f);else if(kind==1)Hosta(p+Vector3.up*.30f*scale,scale*.65f);else Hydrangea(p+Vector3.up*.30f*scale,scale*.65f,kind);
 }
 static void TrailingVine(Vector3 start,float length,int seed){
  Vector3 At(float t)=>start+new Vector3(Mathf.Sin(t*8+seed)*.055f,-t*length,Mathf.Cos(t*6+seed)*.06f);
  for(int k=0;k<10;k++){float t=k/10f;Cylinder("Hanging vine",At(t),At(t+.1f),.005f,"moss",4);foreach(int side in new[]{-1,1})Leaf(At(t)+new Vector3(side*.055f,0,.02f),Quaternion.Euler(60,seed*45+k*31+side*30,side*20),.13f,.09f,k%4==0?"leafLight":"leaf");}
 }
 static void FacadeGarden(int side,int row,float front,float z){
  // Window boxes sit on visible corbels well above walking and rooftop routes.
  if(row==1||row==2||row==3){foreach(float dz in new[]{-4.7f,4.7f}){
   var p=new Vector3(front-side*.43f,4.28f,z+dz);Box("Painted window flower box",p,new(.46f,.25f,1.48f),row==2?"wine":"awning");Box("Window box soil",p+Vector3.up*.13f,new(.39f,.024f,1.37f),"moss");
   foreach(float d in new[]{-.53f,.53f})Box("Window box iron corbel",p+new Vector3(side*.07f,-.20f,d),new(.32f,.17f,.045f),"iron");
   for(int k=-1;k<=1;k++){var b=p+new Vector3(0,.14f,k*.45f);if(k==0)Hydrangea(b,.63f,row);else Hosta(b,.54f);TrailingVine(b+new Vector3(-side*.19f,0,0),.47f,k+row);}
  }}
  // Baskets hang at the jambs, above heads, on attached iron brackets.
  if(row==1||row==3){var p=new Vector3(front-side*.76f,2.33f,z-5.85f);Cylinder("Basket wall bracket",new(front-side*.14f,3.1f,z-5.85f),p+Vector3.up*.58f,.021f,"iron",6);PlantPot(p,.8f,2+row);for(int k=0;k<3;k++){float a=k*Mathf.PI*2/3;Cylinder("Basket suspension chain",p+new Vector3(Mathf.Cos(a)*.14f,.22f,Mathf.Sin(a)*.14f),p+Vector3.up*.58f,.006f,"iron",4);TrailingVine(p+new Vector3(Mathf.Cos(a)*.12f,.22f,Mathf.Sin(a)*.12f),.42f,k+row);}}
 }
 static void LivingGardens(){
  BotanicalMaterials();
  // Existing planted beds only: no new hard obstacles on paths, spawn sites or sidewalks.
  Vector3[] groups={new(-21,.245f,-44.5f),new(-21,.245f,-58),new(-10.5f,.245f,-71),new(-12.5f,.245f,-39.5f)};
  for(int g=0;g<groups.Length;g++)for(int k=0;k<14;k++){
   var p=groups[g]+new Vector3((k%7-3)*(g==2?1.5f:g==3?1.05f:.60f),0,(k/7-.5f)*(g==3?.55f:2.1f));
   if(k%3==0)Hydrangea(p,.85f,g);else if(k%3==1)Hosta(p,1.0f);else SwordFern(p,.90f);
  }
 }
}}
