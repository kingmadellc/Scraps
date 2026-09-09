using UnityEngine;
using UnityEngine.UI;
namespace Jimothy {
/// <summary>Resolution-independent original control glyphs; no font symbols or external icon fonts.</summary>
public sealed class TouchGlyph:MaskableGraphic {
 public string symbol="jump";VertexHelper mesh;
 protected override void OnPopulateMesh(VertexHelper vh){vh.Clear();mesh=vh;Ring(Vector2.zero,.47f,.013f);switch(symbol){
 case "jump": Line(new(-.22f,-.16f),new(0,.10f));Line(new(0,.10f),new(.22f,-.16f));Line(new(0,.10f),new(0,.26f));Line(new(-.10f,.16f),new(0,.26f));Line(new(0,.26f),new(.10f,.16f));Ring(new(0,-.20f),.075f,.025f);break;
 case "search":Ring(new(-.06f,.065f),.19f,.033f);Line(new(.08f,-.08f),new(.25f,-.25f),.045f);break;
 case "den":Line(new(-.25f,.0f),new(0,.25f));Line(new(0,.25f),new(.25f,0));Line(new(-.19f,0),new(-.19f,-.23f));Line(new(.19f,0),new(.19f,-.23f));Line(new(-.19f,-.23f),new(.19f,-.23f));Line(new(-.07f,-.23f),new(-.07f,-.07f));Line(new(.07f,-.23f),new(.07f,-.07f));Line(new(-.07f,-.07f),new(.07f,-.07f));break;
 case "eat":Ring(Vector2.zero,.235f,.04f);Ring(new(.20f,.14f),.09f,.035f);Line(new(-.1f,.1f),new(-.07f,.07f));Line(new(.05f,-.12f),new(.1f,-.15f));break;
 case "stash":Line(new(-.24f,.12f),new(.24f,.12f));Line(new(-.24f,-.22f),new(.24f,-.22f));Line(new(-.24f,-.22f),new(-.24f,.12f));Line(new(.24f,-.22f),new(.24f,.12f));Line(new(-.09f,.12f),new(-.09f,.26f));Line(new(.09f,.12f),new(.09f,.26f));Line(new(-.09f,.26f),new(.09f,.26f));break;
 case "pause":Line(new(-.09f,-.22f),new(-.09f,.22f),.07f);Line(new(.09f,-.22f),new(.09f,.22f),.07f);break;
 case "camera":Ring(Vector2.zero,.18f,.025f);Line(new(-.30f,0),new(-.11f,0));Line(new(.11f,0),new(.30f,0));Line(new(0,-.3f),new(0,-.11f));Line(new(0,.11f),new(0,.3f));break;
 case "stick":Ring(Vector2.zero,.12f,.035f);break;
 }
 }
 void Ring(Vector2 c,float r,float width){for(int i=0;i<56;i++){float a=i*Mathf.PI*2/56,b=(i+1)*Mathf.PI*2/56;Line(c+new Vector2(Mathf.Cos(a),Mathf.Sin(a))*r,c+new Vector2(Mathf.Cos(b),Mathf.Sin(b))*r,width);}}
 void Line(Vector2 a,Vector2 b,float width=.032f){Rect r=rectTransform.rect;float size=Mathf.Min(r.width,r.height);Vector2 d=(b-a).normalized,n=new(-d.y,d.x);int k=mesh.currentVertCount;foreach(var p in new[]{a+n*width*.5f,a-n*width*.5f,b-n*width*.5f,b+n*width*.5f}){var v=UIVertex.simpleVert;v.position=r.center+p*size;v.color=color;mesh.AddVert(v);}mesh.AddTriangle(k,k+1,k+2);mesh.AddTriangle(k,k+2,k+3);}
}
}
