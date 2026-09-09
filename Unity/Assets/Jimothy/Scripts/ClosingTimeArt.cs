using UnityEngine;
namespace Jimothy {
public static partial class ClosingTimeWorld {
 static void ArtMaterials(){
  Mat("terracotta",C(.45f,.19f,.09f),.18f);Mat("linen",C(.75f,.68f,.48f),.08f);Mat("wine",C(.25f,.075f,.065f),.3f);Mat("navy",C(.055f,.14f,.20f),.28f);Mat("ceramic",C(.62f,.69f,.61f),.6f);Mat("interior",C(.16f,.105f,.062f),.1f,0,C(.08f,.046f,.021f));Mat("interiorCool",C(.095f,.145f,.16f),.1f,0,C(.023f,.036f,.043f));Mat("bread",C(.67f,.38f,.13f),.08f);Mat("bottleAmber",C(.26f,.13f,.035f),.75f,.15f);Mat("bottleGreen",C(.055f,.24f,.13f),.72f,.2f);
  InteriorWindowMaterials();
  Mat("rainierSnow",C(.67f,.76f,.84f),.2f,0,C(.12f,.165f,.23f));Mat("rainierIce",C(.29f,.40f,.52f),.32f,0,C(.037f,.065f,.11f));Mat("rainierRock",C(.15f,.21f,.29f),.1f,0,C(.017f,.034f,.061f));Mat("foothill",C(.11f,.17f,.23f),.04f,0,C(.017f,.03f,.05f));
 }
 static void InteriorWindowMaterials(){
  const int n=128;var texture=new Texture2D(n,n,TextureFormat.RGBA32,true,false){name="Soft interior glazing light",wrapMode=TextureWrapMode.Clamp,filterMode=FilterMode.Trilinear};var pixels=new Color32[n*n];
  for(int y=0;y<n;y++)for(int x=0;x<n;x++){float u=x/(float)(n-1),v=y/(float)(n-1);float lamp=Mathf.Exp(-((u-.70f)*(u-.70f)*18+(v-.60f)*(v-.60f)*9));float shade=.20f+lamp*.72f;float curtain=Mathf.Sin(u*54)*.5f+.5f;if(u<.22f||u>.91f)shade*=.36f+curtain*.20f;if(v<.11f)shade*=.25f;pixels[y*n+x]=new Color(shade,shade*.85f,shade*.68f,1);}
  texture.SetPixels32(pixels);texture.Apply(true,true);foreach(var key in new[]{"window","windowDim"}){mats[key].SetTexture("_BaseMap",texture);mats[key].SetTexture("_EmissionMap",texture);mats[key].SetFloat("_Smoothness",.46f);}
 }
 // Six-point folded leaf: a raised midrib and pointed tip make a lit organic silhouette.
 static void Leaf(Vector3 p,Quaternion rotation,float length,float width,string material){
  var b=GetBatch(material,p);Vector3[] points={new(0,0,-length*.5f),new(-width*.25f,length*.06f,-length*.31f),new(-width*.49f,length*.09f,-length*.06f),new(-width*.35f,0,length*.25f),new(0,-length*.10f,length*.5f),new(width*.35f,0,length*.25f),new(width*.49f,length*.09f,-length*.06f),new(width*.25f,length*.06f,-length*.31f),new(0,length*.11f,0)};
  int first=b.v.Count;foreach(var v in points){b.v.Add(p+rotation*v);b.n.Add(rotation*new Vector3(v.x<0?-.22f:.22f,1,.10f).normalized);b.uv.Add(new Vector2(v.x/width+.5f,v.z/length+.5f));}for(int i=0;i<8;i++)b.t.AddRange(new[]{first+8,first+(i+1)%8,first+i});
 }
 static void Fern(Vector3 p,float scale=1){SwordFern(p,scale);}
 static void TreeOrganic(Vector3 p){
  var trunk=new GameObject("Tree trunk collision");trunk.transform.SetParent(root,false);trunk.transform.localPosition=p;var collider=trunk.AddComponent<CapsuleCollider>();collider.center=Vector3.up*1.8f;collider.height=3.6f;collider.radius=.15f;
  var bend=p+new Vector3(.13f,2.25f,.07f);Cylinder("Textured tree trunk",p,bend,.115f,"wood",10);Cylinder("Upper trunk",bend,p+new Vector3(.21f,4.2f,.13f),.068f,"wood",8);
  // Deliberately airy crown: overlapping leaf sprays, not opaque spheres.
  for(int branch=0;branch<7;branch++){float angle=branch*2.39996f;var basePoint=p+new Vector3(.13f,2.45f+branch*.14f,.07f);var end=p+new Vector3(Mathf.Cos(angle)*(1.08f+branch*.045f),3.65f+(branch%3)*.44f,Mathf.Sin(angle)*1.20f);var elbow=Vector3.Lerp(basePoint,end,.65f)+Vector3.up*.23f;Cylinder("Organic branch",basePoint,elbow,.047f,"wood",6);Cylinder("Branch taper",elbow,end,.022f,"wood",5);
   for(int twig=0;twig<5;twig++){float phase=angle+twig*1.9f;var tip=Vector3.Lerp(elbow,end,.3f+twig*.14f)+new Vector3(Mathf.Cos(phase)*.58f,.12f+Mathf.Sin(twig)*.2f,Mathf.Sin(phase)*.58f);Cylinder("Fine twig",elbow,tip,.008f,"wood",4);
    for(int leaf=0;leaf<10;leaf++){float a=leaf*2.39996f+phase;float radius=.12f+leaf*.043f;var lp=tip+new Vector3(Mathf.Cos(a)*radius,Mathf.Sin(a*1.3f)*.18f,Mathf.Sin(a)*radius);Leaf(lp,Quaternion.Euler(random.Next(-45,40),random.Next(0,360),random.Next(-35,35)),.30f+(float)random.NextDouble()*.17f,.18f+(float)random.NextDouble()*.10f,(leaf+twig)%3==0?"leafLight":"leaf");}
   }
  }
  Box("Tree grate",p+Vector3.up*.009f,new(.94f,.018f,.94f),"iron");for(int i=-3;i<=3;i++)Box("Tree grate slot",p+new Vector3(i*.12f,.021f,0),new(.045f,.01f,.80f),"moss");
 }
 static void DeepShop(int side,int row,int bay,float front,float z){CraftedShop(side,row,bay,front,z);}
 static void BuildingCharacter(int side,int row,float front,float z,float h){
  FacadeVariation(side,row,front,z,h);
  string brick=row%2==0?"brickRust":"brickUmber";
  for(int edge=-1;edge<=1;edge+=2){for(int q=0;q<8;q++)Box("Corner masonry quoin",new(front-side*.10f,.7f+q*.86f,z+edge*5.97f),new(.24f,.26f,q%2==0?.52f:.31f),"stone");}
  // Small cornice pediments vary the skyline, away from the alley landings.
  if(row%2==0){Box("Masonry parapet center",new(front+.12f*side,h+.48f,z),new(.36f,.62f,3.8f),brick);Box("Pediment cap",new(front,h+.83f,z),new(.49f,.13f,4.06f),"stone");}
  int facadeStyle=(side<0?0:5)+row;var facing=Quaternion.Euler(0,side>0?90:-90,0);if(facadeStyle!=1&&facadeStyle!=5&&facadeStyle!=9)Text(row==2?"EST. 1904":row==0?"BALLARD • SEATTLE":"EST. 1912",new(front-side*.24f,h-.68f,z),.025f,"stone",facing);
  // Handbill board and a lower street-level secondary name give buildings a human scale.
  Box("Community handbill board",new(front-side*.42f,1.52f,z+5.8f),new(.07f,.88f,.43f),"wood");for(int k=0;k<3;k++)Box("Weathered concert bill",new(front-side*.465f,1.28f+k*.23f,z+5.8f),new(.012f,.19f,.34f),k==0?"terracotta":"linen");
  if(row%2==1){for(int stem=0;stem<4;stem++){float zz=z-5.7f+stem*.17f;Cylinder("Climbing ivy stem",new(front-side*.19f,.5f,zz),new(front-side*.19f,3.10f,zz+.35f),.009f,"moss",4);for(int leaf=0;leaf<12;leaf++)Leaf(new(front-side*.24f,.65f+leaf*.20f,zz+(leaf%2)*.17f),Quaternion.Euler(0,side*90,65),.18f,.17f,leaf%3==0?"leafLight":"leaf");}}
 }
 static void RainierScenery(){RainierHero.Build(root);ShadowSeaplane.Build(root);}
}
}
