using UnityEngine;
namespace Jimothy {
public static partial class ClosingTimeWorld {
 static void CraftedShop(int side,int row,int bay,float front,float z){
  int style=(side<0?0:5)+row;var brand=brands[style];string trim=style==2?"wine":style==4?"navy":style==8?"iron":style==9?"terracotta":brand.paint;
  Vector3 P(float depth,float y,float along=0)=>new(front-side*depth,y,z+along);
  void B(string name,float d,float y,float along,float depth,float height,float width,string mat)=>Box(name,P(d,y,along),new(depth,height,width),mat);
  B("Warm recessed display backing",.016f,1.68f,0,.028f,2.4f,2.3f,style==7?"interiorCool":"interior");
  foreach(int e in new[]{-1,1}){B("Painted storefront jamb",.22f,1.66f,e*1.18f,.45f,2.7f,.14f,trim);B("Jamb inset molding",.465f,1.67f,e*1.18f,.035f,2.3f,.055f,style==0||style==6?"gold":trim);}
  B("Storefront lintel",.23f,2.98f,0,.48f,.16f,2.5f,trim);B("Display sill",.24f,.56f,0,.51f,.13f,2.49f,trim);
  B("Shopfront plinth",.33f,.29f,0,.23f,.48f,2.37f,style==4?"ceramic":trim);
  if(style==1||style==4||style==8){for(int x=0;x<12;x++)for(int y=0;y<2;y++)B("Glazed ceramic dado tile",.458f,.15f+y*.21f,-1.07f+x*.195f,.027f,.195f,.183f,style==4?(x%4==0?"navy":"ceramic"):style==8?(x%2==0?"cream":"iron"):"awning");}
  else if(style==0||style==5||style==9){for(int k=0;k<4;k++)B("Tongue and groove frontage",.455f,.115f+k*.115f,0,.03f,.09f,2.28f,style==5?"cedar":trim);}
  else foreach(int k in new[]{-1,1}){B("Recessed wood frontage panel",.46f,.29f,k*.58f,.025f,.30f,1.02f,"wood");B("Raised panel rail",.477f,.43f,k*.58f,.02f,.035f,1.06f,trim);}
  B("Transom rail",.405f,2.47f,0,.055f,.065f,2.32f,style==0||style==6?"gold":trim);
  int panes=style==0?5:style==3||style==6?7:3;
  for(int k=1;k<panes;k++)B("Individual transom light mullion",.40f,2.71f,-1.15f+k*2.3f/panes,.05f,.42f,.032f,trim);
  if(style==3||style==6)for(int k=0;k<panes;k++)B("Amber pub transom glass",.055f,2.69f,-1.0f+k*2f/(panes-1),.025f,.30f,.17f,k%2==0?"bottleAmber":"bottleGreen");
  if(bay==3){
   B("Recessed shop entrance door",.07f,1.56f,0,.08f,2.02f,1.12f,trim);B("Door inset glazing",.122f,1.97f,0,.022f,.92f,.88f,"glass");B("Door brass kickplate",.126f,.72f,0,.025f,.19f,.87f,"gold");Cylinder("Vertical brass door pull",P(.18f,1.08f,.39f),P(.18f,1.38f,.39f),.02f,"gold",8);
   BrandType("CLOSED","BarlowCondensed-SemiBold",side,front,z,new(0,1.97f),.026f,"linen",.148f);
  }else{
   // Merchandise varies by business, rather than repeating bottle shelves on every façade.
   ShopMerchandise(side,style,bay,front,z);
   if(style!=7){B("Fine display mullion",.40f,1.53f,0,.046f,1.8f,.034f,trim);}
   foreach(float along in new[]{-.65f,.65f}){Cylinder("Display pendant cable",P(.16f,2.91f,along),P(.16f,2.27f,along),.009f,"iron",4);Ellipsoid(P(.16f,2.25f,along),new(.115f,.066f,.15f),style==1||style==5?"copper":"iron",10,5);Ellipsoid(P(.16f,2.218f,along),new(.08f,.018f,.115f),"bulb",8,4);}
  }
  // Three distinct canopy constructions, with visible support rods and properly separated surfaces.
  if(style==8||style==9||style==0){
   string metal=style==9?"copper":"iron";B("Industrial shallow canopy",.68f,3.065f,0,1.06f,.07f,2.51f,metal);
   for(int k=0;k<10;k++)B("Standing canopy seam",.68f,3.115f,-1.13f+k*.25f,1.05f,.035f,.025f,metal);
   foreach(float along in new[]{-1f,1f})Cylinder("Canopy tension rod",P(.12f,3.38f,along),P(1.17f,3.08f,along),.013f,"iron",5);
  }else if(style==4||style==6){
   B("Compact cast metal rain hood",.52f,3.05f,0,.71f,.095f,2.5f,trim);foreach(float along in new[]{-1.04f,1.04f}){Cylinder("Scrolled hood support",P(.16f,2.74f,along),P(.72f,3.0f,along),.025f,"iron",6);}
  }else{
   float projection=style==2?1.12f:style==7?.78f:.92f;
   Box("Tailored canvas canopy",P(.38f+projection*.5f,3.05f),new(projection,.055f,2.52f),trim,false,Quaternion.Euler(0,0,side*10));
   B("Canvas valance",.38f+projection,2.83f,0,.045f,.26f,2.52f,trim);
   if(style==2||style==5){for(int k=0;k<5;k++){float along=-1.06f+k*.53f;Box("Woven canvas stripe",P(.38f+projection*.5f,3.087f,along),new(projection,.012f,.13f),"linen",false,Quaternion.Euler(0,0,side*10));B("Valance stripe",.414f+projection,2.83f,along,.012f,.25f,.13f,"linen");}}
   if(style==1||style==2||style==7)for(int k=0;k<10;k++)Ellipsoid(P(.39f+projection,2.70f,-1.13f+k*.25f),new(.025f,.045f,.123f),trim,6,3);
  }
 }
 static void ShopMerchandise(int side,int style,int bay,float front,float z){
  Vector3 P(float depth,float y,float along=0)=>new(front-side*depth,y,z+along);
  void B(string name,float d,float y,float along,float depth,float height,float width,string mat)=>Box(name,P(d,y,along),new(depth,height,width),mat);
  void Shelf(float y,string mat="cedar")=>B("Shop display shelf",.20f,y,0,.32f,.055f,2.10f,mat);
  void Bottle(float along,float y,string mat){Cylinder("Bottle body",P(.20f,y+.04f,along),P(.20f,y+.22f,along),.043f,mat,8);Cylinder("Bottle neck",P(.20f,y+.22f,along),P(.20f,y+.32f,along),.022f,mat,8);B("Paper bottle label",.247f,y+.14f,along,.012f,.08f,.06f,"linen");}
  void Disk(float along,float y,float radius,string mat,float depth=.25f){SignDisc(side,front,z+along,new(0,y),radius,radius,mat,depth);}
  switch(style){
   case 0: // Chandlery: rope coils, cork floats, nets and enamel navigation plaques.
    Shelf(.83f);Shelf(1.47f);
    for(int k=0;k<3;k++){float along=-.73f+k*.72f;for(int loop=0;loop<3;loop++){float r=.16f+loop*.029f;for(int n=0;n<20;n++){float a=n*Mathf.PI/10,b=(n+1)*Mathf.PI/10;Cylinder("Coiled marine rope",P(.21f,1.12f+Mathf.Sin(a)*r,along+Mathf.Cos(a)*r),P(.21f,1.12f+Mathf.Sin(b)*r,along+Mathf.Cos(b)*r),.014f,"linen",5);}}Ellipsoid(P(.20f,1.66f,along),new(.105f,.15f,.10f),k==1?"terracotta":"ceramic",10,6);Cylinder("Buoy eye",P(.20f,1.76f,along),P(.20f,1.85f,along),.012f,"gold",6);}
    for(int k=0;k<8;k++){float along=-1+k*.27f;Cylinder("Fishing net vertical cord",P(.11f,1.84f,along),P(.11f,2.39f,along),.006f,"linen",4);}for(int k=0;k<4;k++)Cylinder("Fishing net cross cord",P(.11f,1.90f+k*.14f,-1),P(.11f,1.90f+k*.14f,1),.006f,"linen",4);
    break;
   case 1: // Pacific Northwest kitchen: crockery, copper stockpots and fresh herbs.
    Shelf(.87f);Shelf(1.54f);
    for(int k=0;k<3;k++){float along=-.72f+k*.72f;Ellipsoid(P(.19f,1.02f,along),new(.13f,.13f,.20f),"copper",12,6);Ellipsoid(P(.19f,1.15f,along),new(.135f,.025f,.21f),"iron",12,4);for(int j=-1;j<=1;j+=2)Cylinder("Stockpot handle",P(.19f,1.08f,along+j*.19f),P(.19f,1.09f,along+j*.26f),.018f,"iron",6);PlantPot(P(.19f,1.58f,along),.42f,1);}
    break;
   case 2:
    foreach(float y in new[]{.86f,1.40f}){Shelf(y);for(int k=0;k<4;k++){float along=-.78f+k*.52f;B("Bakery blackened baking sheet",.2f,y+.05f,along,.28f,.03f,.44f,"iron");Ellipsoid(P(.2f,y+.145f,along),new(.115f,.09f,.19f),"bread",12,6);for(int score=0;score<3;score++)Cylinder("Scored artisan loaf",P(.29f,y+.16f,along-.12f+score*.10f),P(.26f,y+.20f,along-.10f+score*.10f),.009f,"linen",4);}}
    break;
   case 3: // Tavern bottles, hand-pulled taps, leaded dark amber glass.
    Shelf(.90f);Shelf(1.58f);for(int k=0;k<7;k++)Bottle(-.87f+k*.29f,1.61f,k%2==0?"bottleAmber":"bottleGreen");for(int k=0;k<3;k++){float along=-.65f+k*.65f;Cylinder("Beer engine brass neck",P(.22f,.95f,along),P(.22f,1.23f,along),.033f,"gold",8);Ellipsoid(P(.22f,1.32f,along),new(.045f,.11f,.045f),"wood",8,6);}
    break;
   case 4: // Fish counter with individual silver salmon on chipped ice.
    Shelf(.89f,"ceramic");B("Fishmonger's blue counter band",.35f,.82f,0,.025f,.12f,2.12f,"navy");
    for(int k=0;k<8;k++)Ellipsoid(P(.2f,.97f,-.94f+k*.26f),new(.12f,.05f,.15f),"ceramic",6,3);
    for(int k=0;k<3;k++){float along=-.70f+k*.70f;Ellipsoid(P(.23f,1.04f,along),new(.08f,.09f,.25f),"copper",12,6);SignPolygon(side,front,z+along,new[]{new Vector2(-.19f,1.04f),new Vector2(-.35f,1.15f),new Vector2(-.35f,.93f)},"ceramic",.32f);Disk(along+.17f,1.065f,.014f,"iron",.321f);}
    for(int k=0;k<3;k++){float along=-.72f+k*.72f;Cylinder("Hanging fishmonger tag",P(.19f,2.35f,along),P(.19f,1.88f,along),.007f,"linen",4);B("Enamel catch label",.22f,1.80f,along,.028f,.19f,.47f,"linen");}
    break;
   case 5:
    Shelf(.93f,"wood");Shelf(1.62f,"copper");for(int k=0;k<5;k++){float along=-.83f+k*.42f;Bottle(along,1.65f,"bottleAmber");Cylinder("Copper tavern tankard",P(.2f,.97f,along),P(.2f,1.17f,along),.067f,"copper",10);Cylinder("Tankard rim",P(.2f,1.17f,along),P(.2f,1.19f,along),.072f,"gold",10);}
    break;
   case 6:
    B("Billiard table polished apron",.20f,.98f,0,.31f,.17f,1.88f,"wood");B("Billiard green felt",.20f,1.075f,0,.29f,.025f,1.8f,"awning");for(int k=0;k<6;k++)Ellipsoid(P(.24f,1.12f,-.61f+k*.21f),Vector3.one*.038f,k%3==0?"cream":k%3==1?"wine":"gold",8,5);
    B("Cue rack",.14f,1.65f,0,.06f,.07f,1.94f,"wood");for(int k=0;k<7;k++)Cylinder("Individually racked billiard cue",P(.20f,1.23f,-.84f+k*.28f),P(.20f,2.32f,-.76f+k*.26f),.014f,"cedar",6);
    break;
   case 7:
    Shelf(.85f);Shelf(1.48f);for(int k=0;k<3;k++){PlantPot(P(.21f,.89f,-.69f+k*.69f),.57f,k);PlantPot(P(.21f,1.52f,-.69f+k*.69f),.47f,k+1);}TrailingVine(P(.20f,2.37f,-.95f),.80f,4);TrailingVine(P(.20f,2.37f,.95f),.66f,7);break;
   case 8:
    Shelf(.88f);B("Espresso machine stainless body",.20f,1.14f,-.39f,.28f,.45f,.81f,"copper");B("Espresso machine black face",.36f,1.19f,-.39f,.025f,.22f,.72f,"iron");for(int k=0;k<2;k++){float along=-.62f+k*.4f;Cylinder("Espresso group head",P(.30f,1.13f,along),P(.30f,1.03f,along),.045f,"gold",8);B("Portafilter handle",.38f,1.08f,along,.20f,.025f,.03f,"iron");}
    Ellipsoid(P(.19f,1.49f,.48f),new(.10f,.17f,.12f),"bottleAmber",10,6);B("Coffee grinder base",.20f,1.13f,.48f,.22f,.4f,.24f,"iron");for(int k=0;k<3;k++){Cylinder("Stacked ceramic espresso cup",P(.22f,1.39f+k*.065f,-.46f),P(.22f,1.44f+k*.065f,-.46f),.048f,"ceramic",10);}
    Shelf(1.83f);for(int k=0;k<5;k++){B("Kraft coffee bean bag",.20f,2.01f,-.80f+k*.40f,.16f,.29f,.25f,"linen");B("Bean bag roast label",.289f,2.02f,-.80f+k*.40f,.012f,.12f,.14f,"navy");}
    break;
   case 9:
    Shelf(.89f);Shelf(1.58f);for(int k=0;k<5;k++){float along=-.81f+k*.4f;B("Indie album sleeve",.18f,1.13f,along,.065f,.37f,.34f,k%3==0?"wine":k%3==1?"navy":"linen");Disk(along,1.13f,.125f,"iron",.225f);Disk(along,1.13f,.038f,k%2==0?"terracotta":"ceramic",.23f);}
    for(int k=0;k<3;k++){float along=-.69f+k*.69f;Disk(along,1.91f,.25f,"iron",.22f);Disk(along,1.91f,.085f,k%2==0?"terracotta":"linen",.228f);Disk(along,1.91f,.012f,"iron",.234f);}
    break;
  }
  string[] labels={"ROPE · TACKLE · CHARTS","SEASONAL / LOCAL","BAKED BEFORE DAWN","LAST CALL  /  GOOD ALE","WILD PACIFIC SALMON","SMALL BATCH / ON TAP","BILLIARDS SINCE 1912","PLANTS & APERITIFS","ESPRESSO / HOUSE ROAST","LISTEN LOCAL"};
  if(bay==1)BrandType(labels[style],"BarlowCondensed-SemiBold",side,front,z,new(0,.65f),.021f,"linen",.41f);
 }
}}
