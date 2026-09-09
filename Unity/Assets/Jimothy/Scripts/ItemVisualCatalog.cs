using System;
using UnityEngine;
namespace Jimothy {
public static partial class ItemVisuals {
 static readonly string[] CatalogNames={"salmon","sesame","poppy-seed","everything","onion","rye","blueberry","cinnamon","cardamom-knot","bear-claw","cinnamon-roll","kringle","apple-danish","lemon-scone","blackberry-scone","almond-croissant","apple","pear","blackberries","raspberries","strawberries","carrot","peach","plum","fish-taco","salmon-skin","fries","pizza-crust","rice-ball","cheese-cube","sandwich-corner","clam-chowder","quarter","dime","nickel","arcade-token","bus-token","laundry-token","dollar-coin","foreign-coin","brass-key","copper-washer","silver-button","bronze-nut","enamel-pin","bottle-opener","tiny-bell","watch-back","amber-marble","blue-marble","green-marble","sea-glass","red-bead","crystal-bead","prism-shard","glass-stopper","gift-voucher","market-coupon","record-voucher","coffee-card","book-coupon","arcade-ticket","ferry-token","trade-stamp","cork-float","net-float","net-needle","rope-coil","lure","bobber","shell","boat-tag","bottle-cap","lost-mitten","shoelace","tiny-umbrella","bike-reflector","sunglass-lens","rubber-duck","toy-wheel","acorn","pine-cone","fern-print","flowerpot","seed-packet","garden-gnome","wind-chime","smooth-stone","guitar-pick","record-sleeve","drum-key","ticket-stub","harmonica","kazoo","cassette","tambourine-bell","wooden-horse","knit-patch","rune-bead","mini-oar","wool-tassel","carved-fish","painted-tile","small-pennant","mini-crab-pot","ferry-schedule","dock-cleat","ship-compass","signal-flag","rope-fender","lobster-patch","salmon-scale-charm","coffee-spoon","espresso-cup","tea-strainer","picnic-fork","tiny-thermos","biscuit-tin","jam-jar","honey-dipper","postcard-of-rainier","ballard-map","library-bookmark","old-street-number","bicycle-bell","brass-door-knocker","ceramic-raccoon","wooden-seaplane","mini-sailboat","tin-lighthouse","pocket-telescope","cedar-box","quilt-square","garden-lantern","den-welcome-mat","patchwork-cushion","the-first-salmon","marvins-lost-clapper","rainier-at-dawn","the-golden-bagel","captains-last-compass","the-tiny-troll","the-ballard-crown","ghost-of-the-old-trolley","the-nordic-star","midnight-market-medal","the-uncatchable-floatplane","king-of-the-alley","the-glass-kraken","the-lockkeepers-key","the-perfect-pinecone","the-mayor-of-trash"};
 static int Recipe(ItemDefinition item){if(item.id.StartsWith("trophy_")&&int.TryParse(item.id.Substring(7),out int trophy)&&trophy>=0&&trophy<16)return 136+trophy;if(item.id.StartsWith("item_")&&int.TryParse(item.id.Substring(5),out int id)&&id>=0&&id<240)return id<208?id/2:id-104;return -1;}
 static string CatalogKey(ItemDefinition item){int recipe=Recipe(item);return recipe>=0?CatalogNames[recipe]:"uncatalogued";}
 public static bool HasAuthoredRecipe(ItemDefinition item)=>Recipe(item)>=0;
 public static Mesh BuildMesh(ItemDefinition item){int r=Recipe(item);if(r<0){var fallback=new Builder();fallback.Box(new(0,.06f,0),new(.18f,.12f,.16f),bread);return fallback.Build();}bool alt=item.id.StartsWith("item_")&&int.Parse(item.id.Substring(5))<208&&int.Parse(item.id.Substring(5))%2==1;var b=new Builder();AuthorCatalog(b,r,alt);if(alt)AuthorCondition(b,r);var mesh=b.Build();mesh.name=item.id+" · "+CatalogNames[r]+(alt?" · alternate condition":"");return mesh;}
 static readonly Color red=new(.67f,.09f,.045f),blue=new(.08f,.27f,.48f),purple=new(.28f,.08f,.32f),wood=new(.39f,.17f,.055f),white=new(.86f,.89f,.86f);
 static void Disc(Builder b,Vector3 p,float radius,float height,Color c){b.Lathe(p,new[]{new Vector2(0,0),new Vector2(radius,0),new Vector2(radius,height),new Vector2(0,height)},c);}
 static void Stem(Builder b,Vector3 p,float height=.09f){b.Tube(p,p+new Vector3(.025f,height,0),.009f,wood);}
 static void LeafShape(Builder b,Vector3 p,float size,Color c,float angle=0){b.Sphere(p,new(size*.45f,.008f,size),c);b.Tube(p-Vector3.forward*size,p+Vector3.forward*size,.003f,cream);}
 static void Paper(Builder b,int kind,bool alt){float width=kind==61?.10f:.25f,depth=kind==61?.40f:.19f;b.Box(new(0,.016f,0),new(width,.018f,depth),cream);b.Box(new(0,.028f,-depth*.32f),new(width*.88f,.005f,.025f),kind%2==0?blue:red);for(int i=0;i<3;i++)b.Box(new(-width*.06f,.029f,-.015f+i*.032f),new(width*(.60f-i*.12f),.004f,.008f),dark);if(alt)b.Wedge(new(width*.22f,.035f,depth*.18f),new(width*.26f,.025f,depth*.26f),white);}
 static void Coin(Builder b,int kind,bool alt){float radius=kind==33?.077f:kind==34?.09f:.115f;Color c=kind==32||kind==33||kind==34?silver:gold;Disc(b,Vector3.zero,radius,.022f,c);b.Ring(new(0,.026f,0),radius*.86f,.004f,c,Quaternion.identity);int marks=kind==33?10:kind==34?5:kind==38?12:kind==39?7:8;for(int i=0;i<marks;i++){float a=i*Mathf.PI*2/marks;b.Box(new(Mathf.Cos(a)*radius*.67f,.03f,Mathf.Sin(a)*radius*.67f),new(.01f,.006f,.015f),cream,Quaternion.Euler(0,-a*Mathf.Rad2Deg,0));}if(kind==36){b.Box(new(0,.033f,0),new(.09f,.007f,.055f),dark);for(int i=-1;i<=1;i+=2)b.Sphere(new(i*.032f,.04f,.028f),Vector3.one*.013f,silver);}else if(kind==37)b.Ring(new(0,.035f,0),.035f,.009f,silver,Quaternion.identity);else {b.Box(new(0,.032f,0),new(.028f,.009f,.07f),c);b.Box(new(.02f,.034f,-.03f),new(.035f,.007f,.014f),cream);}if(alt)b.Wedge(new(.02f,.038f,.015f),new(.035f,.012f,.05f),c);}
 static void Cup(Builder b,bool handle,Color c){b.Lathe(Vector3.zero,new[]{new Vector2(0,0),new Vector2(.072f,0),new Vector2(.095f,.14f),new Vector2(.077f,.15f),new Vector2(.06f,.03f),new Vector2(0,.03f)},c);Disc(b,new(0,.115f,0),.073f,.006f,wood);if(handle)b.Ring(new(.10f,.085f,0),.052f,.011f,c,Quaternion.Euler(90,0,0));}
 static void Key(Builder b,bool ornate=false){b.Ring(new(0,.035f,-.11f),ornate?.075f:.055f,.017f,gold,Quaternion.identity);b.Box(new(0,.035f,.075f),new(.027f,.025f,.26f),gold);for(int i=0;i<(ornate?4:2);i++)b.Box(new(.025f,.035f,.12f-i*.035f),new(.07f,.025f,.018f),gold);if(ornate)b.Ring(new(0,.04f,-.11f),.036f,.01f,silver,Quaternion.identity);}
 static void Plant(Builder b,bool pot=true){if(pot)b.Lathe(Vector3.zero,new[]{new Vector2(0,0),new Vector2(.085f,0),new Vector2(.12f,.15f),new Vector2(.10f,.17f),new Vector2(.08f,.04f)},red);float y=pot?.16f:.02f;for(int i=0;i<5;i++){float a=i*Mathf.PI*2/5;var tip=new Vector3(Mathf.Cos(a)*.09f,y+.09f,Mathf.Sin(a)*.09f);b.Tube(new(0,y,0),tip,.004f,green);b.Sphere(tip,new(.045f,.015f,.075f),green);}}
 static void Pinecone(Builder b,bool trophy=false){float size=trophy?1.35f:1;for(int row=0;row<5;row++)for(int k=0;k<7;k++){float a=(k+row*.5f)*Mathf.PI*2/7,rad=(.07f-row*.007f)*size;b.Sphere(new(Mathf.Sin(a)*rad,(.04f+row*.043f)*size,Mathf.Cos(a)*rad),new Vector3(.034f,.028f,.028f)*size,wood);}Stem(b,new(0,.24f*size,0),.025f);}
 static void Seaplane(Builder b){b.Sphere(new(0,.13f,0),new(.055f,.06f,.22f),cream);b.Box(new(0,.18f,-.025f),new(.52f,.027f,.105f),blue);b.Box(new(0,.15f,.17f),new(.20f,.017f,.06f),blue);b.Wedge(new(0,.16f,.16f),new(.014f,.09f,.09f),red);for(int s=-1;s<=1;s+=2){b.Sphere(new(s*.08f,.035f,0),new(.026f,.028f,.18f),silver);b.Tube(new(s*.075f,.05f,0),new(0,.12f,0),.006f,silver);}b.Box(new(0,.13f,-.235f),new(.014f,.16f,.015f),dark);b.Sphere(new(0,.185f,-.085f),new(.045f,.025f,.065f),blue);}
 static void Boat(Builder b){b.Sphere(new(0,.06f,0),new(.10f,.06f,.23f),wood);b.Tube(new(0,.09f,0),new(0,.41f,0),.009f,wood);b.Wedge(new(0,.13f,-.11f),new(.008f,.24f,.23f),cream);b.Wedge(new(.01f,.13f,.03f),new(.008f,.19f,.14f),blue);}
 static void Animal(Builder b,int kind){Color c=kind==0?gold:kind==1?white:wood;b.Sphere(new(0,.13f,0),new(.08f,.075f,.13f),c);b.Sphere(new(0,.22f,-.12f),new(.07f,.065f,.06f),c);for(int s=-1;s<=1;s+=2){b.Cone(new(s*.045f,.25f,-.12f),.025f,.05f,c);for(int z=-1;z<=1;z+=2)b.Tube(new(s*.055f,.02f,z*.07f),new(s*.055f,.13f,z*.07f),.017f,c);b.Sphere(new(s*.042f,.225f,-.169f),Vector3.one*.012f,dark);}b.Tube(new(0,.16f,.10f),new(.08f,.16f,.22f),.025f,c);}
 static void AuthorCondition(Builder b,int r){if(r<32){for(int i=0;i<5;i++)b.Sphere(new(.14f+i*.019f,.01f,-.10f+i*.04f),Vector3.one*(.008f+i*.001f),bread);return;}if(r<40){if(r==35)b.Box(new(0,.042f,0),new(.07f,.016f,.022f),blue);return;}if(r<48){b.Box(new(-.025f,.045f,0),new(.018f,.006f,.07f),dark,Quaternion.Euler(0,30,0));return;}if(r<56){b.Ring(new(0,.095f,0),.097f,.006f,cream,Quaternion.Euler(40,35,0),Mathf.PI*1.4f);return;}if(r<64)return;if(r<72){for(int i=0;i<3;i++)b.Tube(new(.10f,.025f,.06f),new(.14f+i*.015f,.009f,.10f+i*.02f),.003f,cream);return;}if(r<80){b.Ring(new(0,.015f,0),.038f,.005f,silver,Quaternion.Euler(0,0,0),Mathf.PI*1.4f);return;}if(r<88){b.Sphere(new(.05f,.03f,.10f),new(.023f,.012f,.04f),green);return;}if(r<96){b.Box(new(.055f,.015f,.10f),new(.09f,.012f,.035f),red,Quaternion.Euler(0,18,0));return;}for(int i=0;i<3;i++)b.Tube(new(-.065f+i*.03f,.018f,-.1f),new(-.045f+i*.03f,.04f,-.13f),.004f,cream);}
 static void AuthorCatalog(Builder b,int r,bool alt){
  // Every case is an explicit catalogue object; shared helpers represent real object families.
  switch(r){
   case 0:b.Append(LegacyMesh(new ItemDefinition{id="item_096",category="food"},"salmon"));break;
   case 1:b.Append(LegacyMesh(new ItemDefinition{id="item_096",category="food"},"sesame-half"));break;
   case 3:b.Append(LegacyMesh(new ItemDefinition{id="item_096",category="food"},"everything-bagel"));break;
   case 5:b.Append(LegacyMesh(new ItemDefinition{id="item_096",category="food"},"rye-sandwich"));break;
   case 8:b.Append(LegacyMesh(new ItemDefinition{id="item_096",category="food"},"cardamom-knot"));break;
   case 15:b.Append(LegacyMesh(new ItemDefinition{id="item_096",category="food"},"croissant"));break;
   case 18:b.Append(LegacyMesh(new ItemDefinition{id="item_096",category="food"},"berries"));break;
   case 24:b.Append(LegacyMesh(new ItemDefinition{id="item_096",category="food"},"taco"));break;
   case 26:b.Append(LegacyMesh(new ItemDefinition{id="item_096",category="food"},"fries"));break;
   case 27:b.Append(LegacyMesh(new ItemDefinition{id="item_096",category="food"},"pizza"));break;
   case 29:b.Box(new(0,.07f,0),new(.17f,.14f,.16f),gold);for(int i=0;i<3;i++)b.Sphere(new((i-1)*.047f,.143f,0),new(.016f,.003f,.02f),crust);break;
   case 35:b.Append(LegacyMesh(new ItemDefinition{id="item_096",category="food"},"arcade-token"));break;
   case 45:b.Append(LegacyMesh(new ItemDefinition{id="item_096",category="food"},"bottle-opener"));break;
   case 48:b.Append(LegacyMesh(new ItemDefinition{id="item_096",category="food"},"glass-float"));break;
   case 68:b.Append(LegacyMesh(new ItemDefinition{id="item_096",category="food"},"fishing-lure"));break;
   case 94:b.Append(LegacyMesh(new ItemDefinition{id="item_096",category="food"},"cassette"));break;
   case 116:b.Append(LegacyMesh(new ItemDefinition{id="item_096",category="food"},"thermos"));break;
   case 136:b.Append(LegacyMesh(new ItemDefinition{id="item_096",category="trophy"},"salmon-sculpture"));break;
   case 137:b.Append(LegacyMesh(new ItemDefinition{id="item_096",category="trophy"},"bell-clapper"));break;
   case 138:b.Append(LegacyMesh(new ItemDefinition{id="item_096",category="trophy"},"rainier-diorama"));break;
   case 139:b.Append(LegacyMesh(new ItemDefinition{id="item_096",category="trophy"},"golden-bagel"));break;
   case 140:b.Append(LegacyMesh(new ItemDefinition{id="item_096",category="trophy"},"compass"));break;
   case 141:b.Append(LegacyMesh(new ItemDefinition{id="item_096",category="trophy"},"tiny-troll"));break;
   case 142:b.Append(LegacyMesh(new ItemDefinition{id="item_096",category="trophy"},"crown"));break;
   case 2:Bagel(b,false,false,bread);for(int i=0;i<40;i++){float a=i*2.4f;b.Box(new(Mathf.Sin(a)*.12f,.108f,Mathf.Cos(a)*.12f),Vector3.one*.006f,dark);}break;
   case 4:Bagel(b,false,false,bread);for(int i=0;i<9;i++){float a=i*.7f;b.Box(new(Mathf.Sin(a)*.12f,.112f,Mathf.Cos(a)*.12f),new(.017f,.007f,.025f),cream,Quaternion.Euler(0,i*30,0));}break;
   case 6:Bagel(b,false,false,bread);for(int i=0;i<8;i++){float a=i*.8f;b.Sphere(new(Mathf.Sin(a)*.12f,.105f,Mathf.Cos(a)*.12f),Vector3.one*.014f,purple);}break;
   case 7:Bagel(b,false,false,crust);for(int i=0;i<4;i++)b.Ring(new(0,.108f,0),.09f+i*.015f,.004f,bread,Quaternion.identity,Mathf.PI*1.8f);break;
   case 9:b.Sphere(new(0,.05f,-.035f),new(.16f,.05f,.085f),bread);for(int i=0;i<5;i++)b.Sphere(new(-.12f+i*.06f,.045f,.075f),new(.024f,.04f,.06f),bread);break;
   case 10:Disc(b,Vector3.zero,.15f,.05f,bread);for(int i=0;i<5;i++)b.Ring(new(0,.06f,0),.025f+i*.025f,.009f,i%2==0?cream:crust,Quaternion.identity,Mathf.PI*1.8f);break;
   case 11:b.Ring(new(0,.05f,0),.14f,.04f,bread,Quaternion.identity);for(int i=-2;i<=2;i++)b.Box(new(i*.045f,.09f,0),new(.012f,.006f,.25f),cream,Quaternion.Euler(0,25,0));break;
   case 12:b.Box(new(0,.035f,0),new(.25f,.07f,.25f),bread);b.Sphere(new(0,.08f,0),new(.085f,.016f,.08f),gold);for(int i=0;i<4;i++)b.Sphere(new((i-1.5f)*.03f,.095f,0),new(.02f,.009f,.064f),red);break;
   case 13:case 14:b.Wedge(new(0,.015f,0),new(.28f,.08f,.26f),bread);for(int i=0;i<6;i++)b.Sphere(new(-.10f+(i%3)*.055f,.102f,-.09f+(i/3)*.07f),Vector3.one*.012f,r==13?gold:purple);if(r==13)b.Wedge(new(.06f,.095f,-.07f),new(.06f,.009f,.05f),gold);break;
   case 16:b.Sphere(new(0,.105f,0),new(.11f,.10f,.10f),red);Stem(b,new(0,.19f,0));LeafShape(b,new(.04f,.24f,0),.04f,green);break;
   case 17:b.Sphere(new(0,.08f,0),new(.095f,.08f,.09f),green);b.Sphere(new(0,.17f,0),new(.05f,.085f,.05f),green);Stem(b,new(0,.245f,0),.045f);break;
   case 19:for(int berry=0;berry<3;berry++){Vector3 at=new((berry-1)*.075f,.025f,berry%2*.055f);for(int i=0;i<10;i++){float a=i*2.4f;b.Sphere(at+new Vector3(Mathf.Sin(a)*.024f,.018f+i*.002f,Mathf.Cos(a)*.024f),Vector3.one*.018f,red);}}break;
   case 20:for(int i=-1;i<=1;i++){var p=new Vector3(i*.075f,.035f,0);b.Sphere(p,new(.05f,.035f,.075f),red);for(int k=0;k<6;k++)b.Sphere(p+new Vector3(Mathf.Sin(k)*.034f,.028f,Mathf.Cos(k)*.045f),Vector3.one*.003f,cream);LeafShape(b,p+Vector3.forward*.065f,.03f,green);}break;
   case 21:b.Cone(new(0,.025f,0),.05f,.25f,new(.9f,.31f,.035f));for(int i=0;i<5;i++)b.Tube(new(0,.025f,0),new((i-2)*.025f,.025f,-.10f-i*.012f),.009f,green);break;
   case 22:b.Sphere(new(0,.09f,0),new(.105f,.09f,.10f),new(.89f,.39f,.20f));b.Ring(new(0,.09f,0),.091f,.004f,crust,Quaternion.Euler(90,0,0),Mathf.PI);LeafShape(b,new(.04f,.18f,0),.05f,green);break;
   case 23:b.Sphere(new(0,.08f,0),new(.075f,.08f,.075f),purple);Stem(b,new(0,.16f,0),.04f);break;
   case 25:b.Box(new(0,.016f,0),new(.29f,.025f,.14f),silver,Quaternion.Euler(0,8,0));for(int i=0;i<7;i++)b.Box(new((i-3)*.038f,.033f,0),new(.009f,.007f,.13f),dark,Quaternion.Euler(0,12,0));break;
   case 28:b.Sphere(new(0,.08f,0),new(.11f,.08f,.105f),cream);b.Box(new(0,.04f,-.085f),new(.105f,.075f,.018f),dark);for(int i=0;i<6;i++)b.Sphere(new((i-2.5f)*.025f,.155f,0),new(.008f,.004f,.012f),white);break;
   case 30:b.Wedge(new(0,.025f,0),new(.27f,.026f,.26f),bread);b.Wedge(new(0,.052f,0),new(.27f,.022f,.26f),green);b.Wedge(new(0,.078f,0),new(.27f,.026f,.26f),cream);break;
   case 31:Cup(b,false,cream);for(int i=0;i<5;i++)b.Box(new((i%3-1)*.04f,.13f,(i/3-.5f)*.05f),Vector3.one*.025f,bread);b.Tube(new(.05f,.10f,0),new(.15f,.22f,0),.008f,silver);break;
   case 32:case 33:case 34:case 36:case 37:case 38:case 39:Coin(b,r,alt);break;
   case 40:Key(b,alt);break;
   case 41:b.Ring(new(0,.025f,0),alt?.085f:.065f,.025f,crust,Quaternion.identity);break;
   case 42:Disc(b,Vector3.zero,.10f,.025f,silver);for(int i=-1;i<=1;i+=2)for(int j=-1;j<=1;j+=2)Disc(b,new(i*.025f,.027f,j*.025f),.012f,.002f,dark);break;
   case 43:for(int i=0;i<6;i++){float a=i*Mathf.PI/3;b.Box(new(Mathf.Sin(a)*.062f,.04f,Mathf.Cos(a)*.062f),new(.066f,.08f,.028f),gold,Quaternion.Euler(0,a*Mathf.Rad2Deg,0));}break;
   case 44:b.Wedge(new(0,.03f,0),new(.20f,.015f,.23f),blue);b.Ring(new(0,.02f,0),.05f,.007f,gold,Quaternion.identity);b.Tube(new(-.1f,.025f,.04f),new(.09f,.025f,.04f),.005f,silver);break;
   case 46:b.Append(LegacyMesh(new ItemDefinition{category="curio"},"bell-clapper"));break;
   case 47:Disc(b,Vector3.zero,.12f,.024f,silver);b.Ring(new(0,.027f,0),.10f,.007f,dark,Quaternion.identity);for(int i=0;i<4;i++){float a=i*Mathf.PI*.5f;b.Box(new(Mathf.Sin(a)*.115f,.03f,Mathf.Cos(a)*.115f),new(.02f,.008f,.02f),dark);}break;
   case 49:case 50:b.Sphere(new(0,.095f,0),Vector3.one*.095f,r==49?blue:green);for(int i=0;i<(r==49?3:4);i++)b.Ring(new(0,.095f,0),.095f,.006f,cream,Quaternion.Euler(i*(r==49?40:25)+25,r==49?20:65,0),Mathf.PI*1.4f);break;
   case 51:b.Wedge(new(0,.01f,0),new(.20f,.045f,.17f),new(.12f,.48f,.36f));b.Wedge(new(.025f,.025f,.03f),new(.11f,.027f,.13f),blue,Quaternion.Euler(0,45,0));break;
   case 52:b.Ring(new(0,.065f,0),.046f,.026f,red,Quaternion.Euler(90,0,0));break;
   case 53:b.Lathe(Vector3.zero,new[]{new Vector2(.035f,0),new Vector2(.085f,.08f),new Vector2(.035f,.16f)},white);Disc(b,new(0,.16f,0),.025f,.005f,dark);break;
   case 54:b.Wedge(new(0,.03f,0),new(.12f,.18f,.18f),blue);b.Wedge(new(-.025f,.03f,0),new(.08f,.16f,.14f),white,Quaternion.Euler(0,160,0));break;
   case 55:b.Lathe(Vector3.zero,new[]{new Vector2(.04f,0),new Vector2(.05f,.11f),new Vector2(.09f,.13f),new Vector2(.065f,.20f),new Vector2(0,.22f)},new(.17f,.45f,.37f));break;
   case 56:case 57:case 58:case 59:case 60:case 61:case 63:Paper(b,r,alt);if(r==56)b.Ring(new(0,.035f,.025f),.035f,.006f,gold,Quaternion.identity);if(r==57)for(int i=0;i<4;i++)b.Box(new((i-1.5f)*.04f,.033f,.07f),new(.01f,.004f,.02f),dark);if(r==58)Disc(b,new(0,.03f,.02f),.055f,.006f,dark);if(r==59){for(int i=0;i<5;i++)Disc(b,new((i-2)*.035f,.03f,.05f),.009f,.003f,wood);}if(r==60)b.Box(new(.06f,.033f,.04f),new(.04f,.008f,.06f),blue);if(r==63)for(int i=0;i<6;i++)b.Box(new((i-2.5f)*.043f,.026f,.095f),new(.016f,.006f,.015f),white);break;
   case 62:Coin(b,36,alt);b.Wedge(new(0,.04f,0),new(.08f,.008f,.045f),blue);break;
   case 64:b.Lathe(Vector3.zero,new[]{new Vector2(.045f,0),new Vector2(.08f,.04f),new Vector2(.08f,.19f),new Vector2(.045f,.23f)},bread);b.Tube(new(0,-.01f,0),new(0,.26f,0),.009f,cream);break;
   case 65:b.Sphere(new(0,.10f,0),Vector3.one*.10f,green);for(int i=0;i<3;i++)b.Ring(new(0,.10f,0),.103f,.005f,cream,Quaternion.Euler(90,i*60,0));break;
   case 66:b.Box(new(0,.02f,0),new(.075f,.027f,.31f),wood);b.Wedge(new(0,.02f,-.18f),new(.075f,.027f,.08f),wood);b.Box(new(0,.037f,0),new(.022f,.008f,.23f),dark);b.Box(new(0,.045f,.04f),new(.012f,.009f,.11f),wood);break;
   case 67:for(int i=0;i<4;i++)b.Ring(new(0,.019f+i*.012f,0),.075f+i*.011f,.009f,cream,Quaternion.identity);b.Tube(new(.07f,.025f,.02f),new(.17f,.01f,.09f),.009f,cream);break;
   case 69:b.Sphere(new(0,.12f,0),new(.07f,.10f,.07f),red);Disc(b,new(0,.12f,0),.071f,.015f,white);b.Tube(new(0,.02f,0),new(0,.28f,0),.009f,dark);break;
   case 70:for(int i=0;i<9;i++){float a=(i-4)*.19f;b.Tube(new(0,.018f,-.075f),new(Mathf.Sin(a)*.15f,.07f,Mathf.Cos(a)*.15f-.07f),.018f,cream);}break;
   case 71:b.Box(new(0,.025f,0),new(.23f,.035f,.12f),blue);b.Ring(new(-.09f,.05f,0),.024f,.005f,silver,Quaternion.identity);for(int i=0;i<3;i++)b.Box(new(i*.04f-.025f,.046f,0),new(.018f,.006f,.065f),white);break;
   case 72:Disc(b,Vector3.zero,.09f,.026f,red);for(int i=0;i<16;i++){float a=i*Mathf.PI/8;b.Box(new(Mathf.Sin(a)*.091f,.01f,Mathf.Cos(a)*.091f),new(.018f,.03f,.016f),silver,Quaternion.Euler(0,a*Mathf.Rad2Deg,0));}break;
   case 73:b.Sphere(new(0,.035f,0),new(.085f,.035f,.12f),blue);b.Sphere(new(.085f,.033f,0),new(.06f,.026f,.035f),blue);b.Box(new(0,.032f,.13f),new(.115f,.06f,.06f),cream);break;
   case 74:for(int i=0;i<8;i++)b.Tube(new((i-4)*.03f,.013f,Mathf.Sin(i)*.045f),new((i-3)*.03f,.014f,Mathf.Sin(i+1)*.045f),.007f,cream);b.Tube(new(-.12f,.01f,-.025f),new(-.17f,.01f,-.025f),.009f,silver);break;
   case 75:b.Cone(new(0,.14f,0),.19f,.08f,blue);b.Tube(new(0,.015f,0),new(0,.21f,0),.007f,silver);b.Ring(new(.018f,.023f,0),.022f,.006f,dark,Quaternion.Euler(90,0,0),Mathf.PI*1.3f);break;
   case 76:b.Box(new(0,.025f,0),new(.16f,.045f,.10f),red);for(int i=0;i<5;i++)for(int j=0;j<3;j++)b.Cone(new((i-2)*.029f,.05f,(j-1)*.028f),.011f,.013f,cream);break;
   case 77:b.Sphere(new(0,.024f,0),new(.13f,.018f,.07f),dark);b.Ring(new(0,.022f,0),.082f,.006f,gold,Quaternion.identity);break;
   case 78:b.Sphere(new(0,.065f,0),new(.10f,.065f,.12f),gold);b.Sphere(new(0,.16f,-.065f),Vector3.one*.061f,gold);b.Wedge(new(0,.14f,-.135f),new(.065f,.018f,.07f),red);b.Sphere(new(.045f,.18f,-.092f),Vector3.one*.009f,dark);break;
   case 79:b.Ring(new(0,.08f,0),.075f,.022f,dark,Quaternion.Euler(90,0,0));for(int i=0;i<6;i++){float a=i*Mathf.PI/3;b.Tube(new(0,.08f,0),new(Mathf.Sin(a)*.07f,.08f+Mathf.Cos(a)*.07f,0),.006f,silver);}break;
   case 80:b.Sphere(new(0,.075f,0),new(.065f,.075f,.065f),wood);Disc(b,new(0,.12f,0),.077f,.035f,crust);Stem(b,new(0,.15f,0),.055f);break;
   case 81:Pinecone(b);break;
   case 82:Paper(b,60,alt);for(int i=0;i<6;i++){b.Tube(new(0,.03f,-.07f),new(0,.03f,.07f),.003f,green);b.Tube(new(0,.032f,-.07f+i*.023f),new(.05f,.032f,-.04f+i*.023f),.003f,green);b.Tube(new(0,.032f,-.07f+i*.023f),new(-.05f,.032f,-.04f+i*.023f),.003f,green);}break;
   case 83:Plant(b);break;
   case 84:Paper(b,57,alt);b.Sphere(new(0,.035f,.02f),Vector3.one*.032f,gold);for(int i=0;i<5;i++){float a=i*1.25f;b.Sphere(new(Mathf.Sin(a)*.045f,.035f,.02f+Mathf.Cos(a)*.045f),new(.025f,.006f,.025f),red);}break;
   case 85:b.Sphere(new(0,.095f,0),new(.06f,.085f,.05f),blue);b.Sphere(new(0,.205f,0),Vector3.one*.047f,cream);b.Cone(new(0,.225f,0),.06f,.14f,red);b.Cone(new(0,.19f,-.04f),.035f,-.07f,white);break;
   case 86:b.Box(new(0,.27f,0),new(.25f,.018f,.07f),wood);for(int i=0;i<5;i++){float h=.12f+i*.023f;b.Tube(new((i-2)*.048f,.26f,0),new((i-2)*.048f,.24f-h,0),.011f,silver);}b.Ring(new(0,.31f,0),.03f,.005f,cream,Quaternion.Euler(90,0,0));break;
   case 87:b.Sphere(new(0,.036f,0),new(.13f,.036f,.085f),silver);b.Ring(new(0,.06f,0),.058f,.003f,white,Quaternion.identity,Mathf.PI*1.2f);break;
   case 88:b.Wedge(new(0,.012f,0),new(.16f,.013f,.19f),purple);b.Sphere(new(-.035f,.028f,-.045f),new(.055f,.005f,.05f),purple);break;
   case 89:b.Box(new(0,.014f,0),new(.28f,.025f,.28f),red);Disc(b,new(.018f,.028f,.015f),.105f,.005f,dark);Disc(b,new(.018f,.035f,.015f),.035f,.004f,cream);break;
   case 90:b.Tube(new(0,.03f,0),new(0,.19f,0),.019f,silver);b.Tube(new(-.11f,.19f,0),new(.11f,.19f,0),.017f,silver);b.Box(new(0,.025f,0),new(.045f,.04f,.045f),dark);break;
   case 91:Paper(b,61,alt);for(int i=0;i<5;i++)b.Box(new(-.047f,.03f,-.10f+i*.04f),new(.017f,.007f,.017f),dark);break;
   case 92:b.Box(new(0,.04f,0),new(.30f,.065f,.085f),silver);for(int i=0;i<10;i++)b.Box(new((i-4.5f)*.026f,.04f,-.045f),new(.016f,.027f,.012f),dark);break;
   case 93:b.Tube(new(-.12f,.035f,0),new(.12f,.035f,0),.027f,red);Disc(b,new(.035f,.055f,0),.05f,.022f,silver);break;
   case 95:b.Ring(new(0,.04f,0),.11f,.017f,wood,Quaternion.identity);for(int i=0;i<6;i++){float a=i*Mathf.PI/3;Disc(b,new(Mathf.Sin(a)*.11f,.065f,Mathf.Cos(a)*.11f),.033f,.006f,silver);}break;
   case 96:Animal(b,2);b.Box(new(0,.145f,0),new(.15f,.018f,.075f),red);break;
   case 97:b.Box(new(0,.014f,0),new(.24f,.025f,.19f),blue);for(int i=0;i<12;i++)b.Tube(new((i-5.5f)*.018f,.03f,-.085f),new((i-5.5f)*.018f,.03f,.085f),.005f,cream);break;
   case 98:b.Lathe(Vector3.zero,new[]{new Vector2(.045f,0),new Vector2(.068f,.065f),new Vector2(.045f,.13f)},wood);b.Box(new(0,.07f,-.06f),new(.012f,.07f,.006f),cream);b.Box(new(.017f,.08f,-.062f),new(.04f,.008f,.008f),cream,Quaternion.Euler(0,0,40));break;
   case 99:b.Tube(new(0,.025f,-.19f),new(0,.025f,.16f),.01f,wood);b.Sphere(new(0,.025f,.19f),new(.047f,.012f,.09f),bread);break;
   case 100:b.Ring(new(0,.20f,0),.035f,.009f,cream,Quaternion.Euler(90,0,0));for(int i=0;i<12;i++){float a=i*Mathf.PI/6;b.Tube(new(Mathf.Sin(a)*.02f,.17f,Mathf.Cos(a)*.02f),new(Mathf.Sin(a)*.06f,.025f,Mathf.Cos(a)*.06f),.008f,red);}break;
   case 101:b.Sphere(new(0,.06f,0),new(.16f,.05f,.065f),wood);b.Wedge(new(-.17f,.02f,0),new(.11f,.035f,.15f),bread);b.Sphere(new(.11f,.075f,-.052f),Vector3.one*.009f,dark);for(int i=0;i<5;i++)b.Tube(new(-.09f+i*.04f,.12f,-.055f),new(-.07f+i*.04f,.06f,-.067f),.004f,wood);break;
   case 102:b.Box(new(0,.024f,0),new(.23f,.045f,.23f),white);for(int i=0;i<4;i++)b.Wedge(new(0,.05f,0),new(.09f,.006f,.09f),blue,Quaternion.Euler(0,i*90,0));break;
   case 103:b.Tube(new(-.11f,.015f,-.13f),new(-.11f,.015f,.16f),.006f,wood);b.Wedge(new(-.1f,.02f,-.10f),new(.22f,.009f,.22f),red);break;
   case 104: // Wire crab pot: open lattice and funnel throat.
    for(int i=0;i<7;i++){float t=(i-3)*.043f;b.Tube(new(t,.025f,-.13f),new(t,.19f,-.13f),.004f,silver);b.Tube(new(t,.025f,.13f),new(t,.19f,.13f),.004f,silver);b.Tube(new(-.13f,.19f,t),new(.13f,.19f,t),.004f,silver);}for(int i=0;i<4;i++){float y=.025f+i*.055f;b.Box(new(0,y,-.13f),new(.28f,.007f,.007f),silver);b.Box(new(0,y,.13f),new(.28f,.007f,.007f),silver);b.Box(new(-.13f,y,0),new(.007f,.007f,.26f),silver);b.Box(new(.13f,y,0),new(.007f,.007f,.26f),silver);}b.Ring(new(0,.105f,-.132f),.045f,.007f,red,Quaternion.Euler(90,0,0));break;
   case 105:Paper(b,105,true);for(int i=0;i<5;i++)b.Box(new((i-2)*.036f,.035f,.035f),new(.003f,.004f,.11f),blue);break;
   case 106:b.Box(new(0,.018f,0),new(.22f,.035f,.095f),silver);for(int s=-1;s<=1;s+=2)b.Tube(new(s*.045f,.025f,0),new(s*.045f,.09f,0),.018f,silver);b.Tube(new(-.15f,.11f,0),new(.15f,.11f,0),.025f,silver);break;
   case 107:b.Append(LegacyMesh(new ItemDefinition{category="curio"},"compass"));b.Box(new(0,.025f,0),new(.39f,.04f,.39f),wood);break;
   case 108:b.Tube(new(-.12f,0,0),new(-.12f,.31f,0),.008f,wood);b.Box(new(.005f,.23f,0),new(.25f,.16f,.009f),white);b.Box(new(.005f,.23f,-.006f),new(.055f,.16f,.005f),blue);b.Box(new(.005f,.23f,-.009f),new(.25f,.045f,.005f),blue);break;
   case 109:for(int i=0;i<9;i++)b.Ring(new(0,.025f+i*.024f,0),.056f,.016f,cream,Quaternion.identity);b.Ring(new(0,.28f,0),.04f,.009f,wood,Quaternion.Euler(90,0,0));break;
   case 110:b.Sphere(new(0,.023f,0),new(.13f,.014f,.16f),blue);b.Sphere(new(0,.045f,0),new(.035f,.01f,.085f),red);for(int s=-1;s<=1;s+=2){b.Sphere(new(s*.064f,.045f,-.08f),new(.027f,.013f,.04f),red);for(int i=0;i<4;i++)b.Tube(new(0,.044f,-.02f+i*.035f),new(s*.075f,.044f,i*.034f),.006f,red);}break;
   case 111:b.Ring(new(0,.04f,-.08f),.025f,.006f,gold,Quaternion.identity);b.Sphere(new(0,.025f,.035f),new(.07f,.016f,.085f),silver);for(int i=0;i<4;i++)b.Ring(new(0,.043f,.065f-i*.025f),.045f,.003f,blue,Quaternion.identity,Mathf.PI);break;
   case 112:b.Sphere(new(0,.03f,-.10f),new(.045f,.009f,.065f),silver);b.Tube(new(0,.027f,-.05f),new(0,.02f,.19f),.009f,silver);break;
   case 113:Cup(b,true,white);Disc(b,Vector3.zero,.14f,.013f,white);break;
   case 114:b.Ring(new(0,.07f,-.045f),.07f,.006f,silver,Quaternion.identity);for(int i=0;i<5;i++){float x=(i-2)*.023f;b.Tube(new(x,.04f,-.095f),new(x,.04f,.005f),.0025f,silver);b.Tube(new(-.05f,.041f,-.045f+x),new(.05f,.041f,-.045f+x),.0025f,silver);}b.Tube(new(0,.07f,.02f),new(0,.07f,.20f),.008f,silver);break;
   case 115:b.Box(new(0,.022f,.075f),new(.026f,.014f,.25f),wood);b.Box(new(0,.022f,-.07f),new(.085f,.014f,.035f),wood);for(int i=0;i<4;i++)b.Box(new((i-1.5f)*.023f,.022f,-.125f),new(.014f,.014f,.09f),wood);break;
   case 117:b.Box(new(0,.065f,0),new(.26f,.13f,.19f),blue);b.Box(new(0,.14f,0),new(.275f,.022f,.205f),silver);Disc(b,new(0,.153f,0),.068f,.004f,gold);break;
   case 118:b.Lathe(Vector3.zero,new[]{new Vector2(0,0),new Vector2(.075f,0),new Vector2(.085f,.16f),new Vector2(.065f,.19f)},purple);Disc(b,new(0,.19f,0),.075f,.025f,silver);b.Box(new(0,.10f,-.082f),new(.11f,.065f,.008f),cream);b.Sphere(new(0,.10f,-.09f),new(.027f,.027f,.009f),red);break;
   case 119:b.Tube(new(0,.035f,-.17f),new(0,.035f,.12f),.012f,wood);for(int i=0;i<6;i++)b.Ring(new(0,.035f,.08f+i*.018f),.038f,.006f,bread,Quaternion.Euler(90,0,0));break;
   case 120:Paper(b,120,false);b.Wedge(new(0,.032f,-.015f),new(.16f,.04f,.10f),blue);b.Wedge(new(0,.055f,-.015f),new(.06f,.023f,.04f),white);break;
   case 121:Paper(b,121,true);for(int i=0;i<4;i++){b.Box(new((i-1.5f)*.04f,.035f,0),new(.006f,.004f,.14f),green);b.Box(new(0,.036f,(i-1.5f)*.035f),new(.22f,.004f,.005f),blue);}break;
   case 122:b.Box(new(0,.013f,0),new(.065f,.012f,.31f),red);for(int i=0;i<3;i++)b.Tube(new(0,.02f,.155f),new((i-1)*.02f,.025f,.21f),.003f,gold);b.Wedge(new(0,.021f,-.05f),new(.04f,.004f,.07f),cream);break;
   case 123:b.Box(new(0,.021f,0),new(.28f,.035f,.15f),blue);for(int k=0;k<2;k++){float x=(k-.5f)*.11f;b.Box(new(x,.043f,0),new(.014f,.008f,.09f),white);b.Box(new(x+.018f,.043f,-.04f),new(.047f,.008f,.012f),white);}break;
   case 124:b.Sphere(new(0,.055f,0),new(.09f,.055f,.09f),silver);b.Ring(new(0,.018f,0),.083f,.01f,dark,Quaternion.identity);b.Box(new(.10f,.028f,0),new(.09f,.022f,.025f),dark);break;
   case 125:b.Box(new(0,.02f,0),new(.12f,.035f,.23f),gold);b.Ring(new(0,.05f,.035f),.10f,.018f,gold,Quaternion.identity);b.Sphere(new(0,.06f,-.055f),new(.038f,.025f,.035f),gold);break;
   case 126:Animal(b,1);for(int s=-1;s<=1;s+=2)b.Sphere(new(s*.039f,.225f,-.176f),new(.031f,.018f,.009f),dark);for(int i=0;i<3;i++)b.Ring(new(.02f+i*.02f,.16f,.13f+i*.03f),.027f,.006f,dark,Quaternion.Euler(60,30,0));break;
   case 127:Seaplane(b);b.Box(new(0,.015f,0),new(.16f,.025f,.12f),wood);break;
   case 128:Boat(b);break;
   case 129:b.Lathe(Vector3.zero,new[]{new Vector2(0,0),new Vector2(.095f,0),new Vector2(.052f,.30f)},white);b.Ring(new(0,.12f,0),.077f,.013f,red,Quaternion.identity);b.Ring(new(0,.24f,0),.061f,.012f,red,Quaternion.identity);Disc(b,new(0,.3f,0),.073f,.015f,dark);b.Box(new(0,.345f,0),new(.08f,.07f,.08f),gold);b.Cone(new(0,.38f,0),.09f,.065f,red);break;
   case 130:b.Tube(new(0,.065f,-.18f),new(0,.065f,.09f),.045f,gold);b.Tube(new(0,.065f,.09f),new(0,.065f,.19f),.032f,wood);b.Ring(new(0,.065f,-.18f),.048f,.01f,dark,Quaternion.Euler(90,0,0));break;
   case 131:b.Box(new(0,.06f,0),new(.26f,.12f,.18f),wood);b.Box(new(0,.13f,0),new(.28f,.025f,.20f),bread);b.Box(new(0,.10f,-.095f),new(.035f,.03f,.009f),gold);for(int i=0;i<3;i++)b.Box(new((i-1)*.065f,.145f,0),new(.006f,.003f,.18f),wood);break;
   case 132:for(int x=0;x<3;x++)for(int z=0;z<3;z++)b.Box(new((x-1)*.08f,.017f,(z-1)*.08f),new(.079f,.023f,.079f),(x+z)%2==0?blue:red);for(int i=0;i<9;i++)b.Tube(new((i-4)*.026f,.012f,-.125f),new((i-4)*.026f,.012f,-.15f),.003f,cream);break;
   case 133:Disc(b,Vector3.zero,.09f,.025f,dark);for(int x=-1;x<=1;x+=2)for(int z=-1;z<=1;z+=2)b.Tube(new(x*.06f,.025f,z*.06f),new(x*.06f,.24f,z*.06f),.006f,dark);b.Box(new(0,.12f,0),new(.035f,.15f,.035f),gold);b.Cone(new(0,.24f,0),.10f,.065f,dark);b.Ring(new(0,.33f,0),.035f,.007f,dark,Quaternion.Euler(90,0,0));break;
   case 134:b.Box(new(0,.014f,0),new(.36f,.025f,.22f),wood);for(int i=0;i<12;i++)b.Box(new((i-5.5f)*.027f,.03f,0),new(.013f,.005f,.18f),bread);for(int s=-1;s<=1;s+=2){b.Sphere(new(s*.055f,.039f,0),new(.025f,.005f,.032f),dark);for(int k=0;k<3;k++)b.Sphere(new(s*.055f+(k-1)*.018f,.04f,-.038f),new(.008f,.005f,.01f),dark);}break;
   case 135:b.Sphere(new(0,.065f,0),new(.19f,.065f,.15f),purple);b.Box(new(-.06f,.118f,0),new(.11f,.008f,.16f),red);b.Box(new(.065f,.12f,.03f),new(.09f,.008f,.09f),blue);b.Sphere(new(0,.132f,0),new(.014f,.008f,.014f),cream);break;
   case 143: // Trolley roof, windows, wheels and overhead collector.
    b.Box(new(0,.15f,0),new(.23f,.19f,.40f),red);b.Box(new(0,.26f,0),new(.25f,.04f,.43f),cream);for(int s=-1;s<=1;s+=2){for(int i=0;i<4;i++)b.Box(new(s*.117f,.20f,(i-1.5f)*.088f),new(.008f,.07f,.058f),blue);for(int z=-1;z<=1;z+=2)b.Sphere(new(s*.10f,.052f,z*.13f),new(.018f,.04f,.04f),dark);}b.Tube(new(0,.28f,.10f),new(0,.39f,-.10f),.007f,silver);break;
   case 144:Disc(b,Vector3.zero,.09f,.035f,wood);b.Star(new(0,.05f,0),.19f,.07f,.024f,8,gold);b.Sphere(new(0,.085f,0),new(.035f,.016f,.035f),blue);break;
   case 145:Disc(b,Vector3.zero,.15f,.028f,gold);b.Ring(new(0,.033f,0),.125f,.008f,silver,Quaternion.identity);b.Box(new(0,.048f,0),new(.15f,.02f,.10f),wood);b.Wedge(new(0,.06f,-.06f),new(.19f,.025f,.06f),red);for(int s=-1;s<=1;s+=2)b.Box(new(s*.075f,.04f,.15f),new(.075f,.018f,.13f),blue);break;
   case 146:Seaplane(b);b.Tube(new(0,-.06f,0),new(0,.09f,0),.015f,gold);b.Box(new(0,-.075f,0),new(.30f,.035f,.23f),dark);for(int i=0;i<3;i++)b.Ring(new(0,-.04f,0),.11f+i*.026f,.004f,blue,Quaternion.identity,Mathf.PI*1.5f);break;
   case 147:Animal(b,0);b.Ring(new(0,.285f,-.12f),.055f,.011f,gold,Quaternion.identity);for(int i=0;i<5;i++)b.Cone(new(Mathf.Sin(i*1.256f)*.055f,.29f,-.12f+Mathf.Cos(i*1.256f)*.055f),.014f,.055f,gold);for(int s=-1;s<=1;s+=2)for(int i=0;i<3;i++)b.Tube(new(s*.05f,.21f,-.18f),new(s*.11f,.205f+i*.008f,-.18f),.002f,cream);break;
   case 148:b.Sphere(new(0,.19f,0),new(.085f,.12f,.075f),blue);for(int i=0;i<8;i++){float a=i*Mathf.PI/4;var p=new Vector3(Mathf.Sin(a)*.13f,.05f,Mathf.Cos(a)*.13f);b.Tube(new(0,.12f,0),p,.023f,blue);b.Ring(p,.045f,.013f,silver,Quaternion.Euler(0,i*45,0),Mathf.PI*1.6f);}for(int s=-1;s<=1;s+=2)b.Sphere(new(s*.033f,.20f,-.071f),Vector3.one*.014f,gold);break;
   case 149:Key(b,true);for(int s=-1;s<=1;s+=2)b.Wedge(new(s*.058f,.055f,-.11f),new(.035f,.02f,.045f),gold,Quaternion.Euler(0,s*40,0));break;
   case 150:Pinecone(b,true);Disc(b,new(0,-.025f,0),.13f,.025f,gold);break;
   case 151:b.Lathe(Vector3.zero,new[]{new Vector2(0,0),new Vector2(.10f,0),new Vector2(.13f,.21f)},silver);for(int i=0;i<10;i++){float a=i*Mathf.PI/5;b.Tube(new(Mathf.Sin(a)*.105f,.025f,Mathf.Cos(a)*.105f),new(Mathf.Sin(a)*.127f,.195f,Mathf.Cos(a)*.127f),.005f,dark);}b.Sphere(new(0,.245f,0),new(.085f,.055f,.07f),white);for(int s=-1;s<=1;s+=2){b.Cone(new(s*.05f,.28f,0),.028f,.04f,white);b.Sphere(new(s*.035f,.25f,-.06f),new(.028f,.018f,.012f),dark);}b.Box(new(0,.32f,0),new(.12f,.018f,.10f),dark);b.Box(new(0,.355f,0),new(.075f,.06f,.065f),dark);break;
   default:throw new InvalidOperationException("Missing authored find recipe "+r);
  }
 }
}
}
