using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
namespace Jimothy {
public static partial class ClosingTimeWorld {
 static readonly Dictionary<string,Font> brandFonts=new();static readonly Dictionary<Font,Material> brandFontMaterials=new();
 struct Brand {public string name,subtitle,font,paint,ink;public int icon,shape;public Brand(string n,string sub,string f,string p,string i,int symbol,int form){name=n;subtitle=sub;font=f;paint=p;ink=i;icon=symbol;shape=form;}}
 static readonly Brand[] brands={
  new("THE NET LOFT","MARINE SUPPLY  ·  BALLARD","BarlowCondensed-SemiBold","navy","linen",0,1),
  new("Cedar & Salt","PACIFIC NORTHWEST KITCHEN","Pacifico-Regular","wood","linen",1,2),
  new("Ballard Bakery","BREAD  ·  PASTRIES  ·  SINCE 1904","BreeSerif-Regular","linen","wine",2,3),
  new("THE LAST CAST","ALES & FISHING TALES","BarlowCondensed-SemiBold","wine","gold",3,4),
  new("SALMON & SONS","FISHMONGERS  •  BALLARD","BreeSerif-Regular","navy","linen",4,1),
  new("Copper Gull","TAVERN  /  EST. 1912","FugazOne-Regular","terracotta","linen",5,2),
  new("Old Avenue","BILLIARDS & GOOD COMPANY","BreeSerif-Regular","iron","gold",6,4),
  new("Juniper Room","BOTANICALS  ·  COCKTAILS","Pacifico-Regular","awning","linen",7,3),
  new("Dockside Coffee","ROASTED HERE. POURED LATE.","Pacifico-Regular","linen","navy",8,0),
  new("Northwest Records","VINYL  ·  NEW & USED","FugazOne-Regular","navy","linen",9,2)
 };
 static Vector3 SignPoint(int side,float front,float z,Vector2 p,float depth)=>new(front-side*depth,p.y,z-side*p.x);
 static void SignPolygon(int side,float front,float z,IList<Vector2> polygon,string material,float depth){var b=GetBatch(material,new Vector3(front,0,z));Vector2 center=Vector2.zero;foreach(var p in polygon)center+=p;center/=polygon.Count;int k=b.v.Count;b.v.Add(SignPoint(side,front,z,center,depth));b.n.Add(Vector3.left*side);b.uv.Add(Vector2.one*.5f);foreach(var p in polygon){b.v.Add(SignPoint(side,front,z,p,depth));b.n.Add(Vector3.left*side);b.uv.Add(p*.2f);}for(int i=0;i<polygon.Count;i++){b.t.AddRange(new[]{k,k+1+i,k+1+(i+1)%polygon.Count});b.t.AddRange(new[]{k,k+1+(i+1)%polygon.Count,k+1+i});}}
 static void SignDisc(int side,float front,float z,Vector2 center,float rx,float ry,string material,float depth){var pts=new List<Vector2>();for(int i=0;i<32;i++){float a=i*Mathf.PI/16;pts.Add(center+new Vector2(Mathf.Cos(a)*rx,Mathf.Sin(a)*ry));}SignPolygon(side,front,z,pts,material,depth);}
 static void SignStroke(int side,float front,float z,Vector2 a,Vector2 b,string material,float depth,float radius=.018f){Cylinder("Painted logo stroke",SignPoint(side,front,z,a,depth),SignPoint(side,front,z,b,depth),radius,material,5);}
 static void BrandType(string value,string fontName,int side,float front,float z,Vector2 p,float height,string ink,float depth){
  if(!brandFonts.TryGetValue(fontName,out var font)){font=Resources.Load<Font>("TitleFonts/"+fontName);if(!font)font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");brandFonts[fontName]=font;string glyphs="ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 &•·/.–'";font.RequestCharactersInTexture(glyphs,72,FontStyle.Normal);}
  var go=new GameObject("Shop wordmark: "+value);go.transform.SetParent(root,false);go.transform.position=SignPoint(side,front,z,p,depth);go.transform.rotation=Quaternion.Euler(0,side>0?90:-90,0);var text=go.AddComponent<TextMesh>();text.font=font;text.fontSize=72;text.characterSize=height;text.text=value;text.anchor=TextAnchor.MiddleCenter;text.alignment=TextAlignment.Center;text.color=mats[ink].GetColor("_BaseColor");
  if(!brandFontMaterials.TryGetValue(font,out var material)){material=new Material(Shader.Find("Jimothy/WorldLettering")){name="Shop lettering "+fontName};brandFontMaterials[font]=material;Font.textureRebuilt+=RebindBrandFont;}
  material.mainTexture=font.material.mainTexture;var renderer=go.GetComponent<MeshRenderer>();renderer.sharedMaterial=material;renderer.shadowCastingMode=ShadowCastingMode.Off;
 }
 static void RebindBrandFont(Font font){if(brandFontMaterials.TryGetValue(font,out var material)&&material)material.mainTexture=font.material.mainTexture;}
 // Original fictional identities. Symbol geometry is original and carries no downloaded brand artwork.
 static void BrandIcon(Brand brand,int side,float front,float z,Vector2 origin,float scale,float depth){
  Vector2 P(float x,float y)=>origin+new Vector2(x,y)*scale;
  void Line(float ax,float ay,float bx,float by,float r=.026f)=>SignStroke(side,front,z,P(ax,ay),P(bx,by),brand.ink,depth,r*scale);
  void Disc(float x,float y,float rx,float ry,string color,float inset=0)=>SignDisc(side,front,z,P(x,y),rx*scale,ry*scale,color,depth+inset);
  switch(brand.icon){
   case 0: // maritime anchor
    Disc(0,.31f,.13f,.13f,brand.ink);Disc(0,.31f,.075f,.075f,brand.paint,.003f);Line(0,.20f,0,-.37f);Line(-.27f,.04f,.27f,.04f);Line(-.35f,-.17f,-.25f,-.35f);Line(-.25f,-.35f,0,-.43f);Line(0,-.43f,.25f,-.35f);Line(.25f,-.35f,.35f,-.17f);break;
   case 1: // cedar sprig
    Line(0,-.43f,0,.43f);for(int k=0;k<4;k++){float y=-.25f+k*.16f;Line(0,y,-.29f+k*.045f,y+.18f);Line(0,y,.29f-k*.045f,y+.18f);}break;
   case 2: // artisan loaf and scoring
    Disc(0,0,.45f,.25f,brand.ink);for(int k=-1;k<=1;k++)SignStroke(side,front,z,P(k*.19f-.035f,-.11f),P(k*.19f+.035f,.11f),brand.paint,depth+.007f,.018f*scale);break;
   case 3: // fish hook
    Line(.15f,.42f,.15f,-.18f);Line(.15f,-.18f,.08f,-.35f);Line(.08f,-.35f,-.13f,-.39f);Line(-.13f,-.39f,-.29f,-.22f);Line(-.29f,-.22f,-.27f,-.06f);Line(-.27f,-.06f,-.13f,-.17f);Disc(.15f,.43f,.09f,.09f,brand.ink);break;
   case 4: // salmon profile
    Disc(.06f,0,.34f,.17f,brand.ink);SignPolygon(side,front,z,new[]{P(-.20f,0),P(-.47f,.23f),P(-.47f,-.23f)},brand.ink,depth);Disc(.28f,.025f,.027f,.027f,brand.paint,.01f);Line(-.02f,.13f,.04f,.26f,.014f);break;
   case 5: // gull spreading wings
    Line(-.52f,.22f,-.28f,.27f);Line(-.28f,.27f,0,-.04f);Line(0,-.04f,.28f,.27f);Line(.28f,.27f,.52f,.22f);Line(0,-.04f,.05f,-.27f);break;
   case 6: // billiard eight ball
    Disc(0,0,.37f,.37f,brand.ink);Disc(0,0,.22f,.22f,brand.paint,.004f);BrandType("8","BreeSerif-Regular",side,front,z,origin,.043f*scale,brand.ink,depth+.008f);break;
   case 7: // juniper branch and berries
    Line(-.26f,-.40f,.27f,.38f);for(int k=0;k<3;k++){float y=-.22f+k*.20f;Line(y*.65f,y,y*.65f-.22f,y+.14f);Disc(y*.65f+.12f,y+.05f,.067f,.067f,brand.ink);}break;
   case 8: // ceramic coffee cup, saucer and steam
    SignPolygon(side,front,z,new[]{P(-.31f,.10f),P(.25f,.10f),P(.18f,-.25f),P(-.24f,-.25f)},brand.ink,depth);Disc(.28f,-.045f,.15f,.12f,brand.ink);Disc(.29f,-.045f,.09f,.068f,brand.paint,.004f);Line(-.38f,-.31f,.35f,-.31f);Line(-.12f,.22f,-.04f,.39f,.014f);Line(.08f,.22f,.16f,.39f,.014f);break;
   case 9: // vinyl grooves and center label
    Disc(0,0,.44f,.44f,"iron");for(int k=0;k<3;k++){float r=.38f-k*.065f;for(int j=0;j<32;j++){float a=j*Mathf.PI/16,b=(j+1)*Mathf.PI/16;SignStroke(side,front,z,P(Mathf.Cos(a)*r,Mathf.Sin(a)*r),P(Mathf.Cos(b)*r,Mathf.Sin(b)*r),"copper",depth+.008f,.007f*scale);}}Disc(0,0,.14f,.14f,"terracotta",.012f);Disc(0,0,.027f,.027f,"linen",.016f);break;
  }
 }
 static void BrandShop(int side,int row,float front,float z,float h){
  var brand=brands[(side<0?0:5)+row];float center=3.66f,half=4.65f,depth=.67f;
  // Project the board beyond the upper-window sill outer edge (.54 m), so masonry cannot cut through lettering.
  Box("Historic painted fascia",new(front-side*.54f,center,z),new(.24f,.85f,11.7f),brand.paint);
  if(brand.shape==0){SignPolygon(side,front,z,new[]{new Vector2(-half,center-.34f),new Vector2(half,center-.34f),new Vector2(half,center+.22f),new Vector2(half-.25f,center+.40f),new Vector2(-half+.25f,center+.40f),new Vector2(-half,center+.22f)},brand.paint,depth);}
  else if(brand.shape==1){SignPolygon(side,front,z,new[]{new Vector2(-half-.25f,center),new Vector2(-half,center-.34f),new Vector2(half,center-.34f),new Vector2(half+.25f,center),new Vector2(half,center+.34f),new Vector2(-half,center+.34f)},brand.ink,depth);SignPolygon(side,front,z,new[]{new Vector2(-half-.14f,center),new Vector2(-half+.03f,center-.30f),new Vector2(half-.03f,center-.30f),new Vector2(half+.14f,center),new Vector2(half-.03f,center+.30f),new Vector2(-half+.03f,center+.30f)},brand.paint,depth+.01f);}
  else if(brand.shape==2){foreach(float yy in new[]{center-.37f,center+.37f})SignStroke(side,front,z,new(-half,yy),new(half,yy),brand.ink,depth,.019f);}
  else if(brand.shape==3){SignDisc(side,front,z,new(0,center),half,.45f,brand.ink,depth);SignDisc(side,front,z,new(0,center),half-.08f,.40f,brand.paint,depth+.008f);}
  else {SignPolygon(side,front,z,new[]{new Vector2(-half,center-.30f),new Vector2(-half+.22f,center-.41f),new Vector2(half-.22f,center-.41f),new Vector2(half,center-.30f),new Vector2(half,center+.30f),new Vector2(half-.22f,center+.41f),new Vector2(-half+.22f,center+.41f),new Vector2(-half,center+.30f)},brand.paint,depth);}
  BrandIcon(brand,side,front,z,new(-3.75f,center),.72f,depth+.04f);
  BrandType(brand.name,brand.font,side,front,z,new(.30f,center+.085f),brand.font=="Pacifico-Regular"?.050f:.055f,brand.ink,depth+.07f);
  BrandType(brand.subtitle,"BarlowCondensed-SemiBold",side,front,z,new(.30f,center-.235f),.018f,brand.ink,depth+.07f);
  // Individual compact badge beside the doorway, visible at raccoon height without a giant uniform OPEN panel.
  float badgeZ=z+4.45f;SignDisc(side,front,badgeZ,new(0,2.02f),.36f,.39f,brand.paint,.47f);BrandIcon(brand,side,front,badgeZ,new(0,2.02f),.57f,.50f);
  // Warm external gooseneck illumination belongs to the sign, not a lightbox billboard.
  for(int i=-1;i<=1;i+=2){float zz=z+i*2.45f;Cylinder("Sign lamp bracket",new(front-side*.30f,4.21f,zz),new(front-side*.81f,4.36f,zz),.024f,"iron",6);Ellipsoid(new(front-side*.83f,4.31f,zz),new(.14f,.07f,.14f),"iron",10,4);Ellipsoid(new(front-side*.83f,4.28f,zz),new(.10f,.017f,.10f),"bulb",8,3);}
 }
 static void FacadeVariation(int side,int row,float front,float z,float h){
  int style=(side<0?0:5)+row;
  // Recessed arched window surrounds, segmented cornices, and modest masonry ornaments.
  if(style==0||style==3||style==6){for(int bay=0;bay<5;bay++){float zz=z-4.7f+bay*2.35f;for(int wedge=0;wedge<13;wedge++){float angle=wedge*Mathf.PI/12;float py=5.60f+Mathf.Sin(angle)*.58f,pz=zz+Mathf.Cos(angle)*.73f;Box("Segmental brick window arch",new(front-side*.27f,py,pz),new(.18f,.19f,.17f),style==6?"stone":"terracotta",false,Quaternion.Euler(angle*Mathf.Rad2Deg,0,0));}Box("Window keystone",new(front-side*.31f,6.22f,zz),new(.21f,.28f,.19f),"stone");}}
  if(style==2||style==8){for(int bay=0;bay<5;bay++){float zz=z-4.7f+bay*2.35f;foreach(int edge in new[]{-1,1})Box("Painted timber window shutter",new(front-side*.34f,4.90f,zz+edge*.90f),new(.10f,1.85f,.28f),style==2?"wine":"navy");}}
  if(style==1||style==5||style==9){for(int bay=-2;bay<=2;bay++){Box("Recessed parapet panel",new(front-side*.18f,h-.67f,z+bay*1.85f),new(.13f,.32f,1.42f),style==9?"terracotta":"brickUmber");Box("Panel frame top",new(front-side*.24f,h-.46f,z+bay*1.85f),new(.17f,.065f,1.50f),"stone");}}
  if(style==4||style==7){for(int bay=-2;bay<=2;bay++){float zz=z+bay*2.0f;Box("Broad masonry pilaster",new(front-side*.16f,4.84f,zz),new(.22f,2.60f,.18f),"stone");Box("Pilaster capital",new(front-side*.21f,6.14f,zz),new(.31f,.17f,.39f),"stone");}}
  // Different metal versus timber cornice paint profiles without altering the roof landing height.
  string cap=style==2?"linen":style==5?"copper":style==7?"awning":style==9?"terracotta":"stone";
  Box("Distinctive cornice paint",new(front-side*.19f,h+.15f,z),new(.41f,.12f,11.85f),cap);
 }
}
}
