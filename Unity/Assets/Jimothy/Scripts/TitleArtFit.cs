using UnityEngine;
using UnityEngine.UI;
namespace Jimothy {
// Menu-only aspect-fill: retain the hero's proportions on wider mobile safe areas.
[RequireComponent(typeof(RawImage))]
public sealed class TitleArtFit : MonoBehaviour {
 RawImage image;RectTransform rect;
 void Awake(){image=GetComponent<RawImage>();rect=(RectTransform)transform;}
 void LateUpdate(){
  if(!image.texture||rect.rect.height<=0)return;
  float target=rect.rect.width/rect.rect.height,source=(float)image.texture.width/image.texture.height;
  if(target>source){float height=source/target;image.uvRect=new Rect(0,(1-height)*.5f,1,height);}
  else {float width=target/source;image.uvRect=new Rect((1-width)*(GameUI.MobileLayout?.90f:.5f),0,width,1);}
 }
}
// A feathered left-to-right treatment preserves the hero and avoids a visible panel seam.
public sealed class TitleScrim : MaskableGraphic {
 protected override void OnPopulateMesh(VertexHelper vh){
  vh.Clear();Rect r=rectTransform.rect;
  float[] stops={0,.30f,.49f,.67f,1};float[] alpha={.55f,.39f,.17f,.025f,.025f};
  for(int i=0;i<stops.Length;i++){
   float x=Mathf.Lerp(r.xMin,r.xMax,stops[i]);Color c=new Color(.015f,.055f,.04f,alpha[i]);
   vh.AddVert(new Vector3(x,r.yMin),c,Vector2.zero);vh.AddVert(new Vector3(x,r.yMax),c,Vector2.one);
   if(i>0){int j=(i-1)*2;vh.AddTriangle(j,j+1,j+2);vh.AddTriangle(j+2,j+1,j+3);}
  }
 }
}
}
