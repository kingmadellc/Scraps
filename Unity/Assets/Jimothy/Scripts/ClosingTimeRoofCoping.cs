using UnityEngine;
namespace Jimothy {
public static partial class ClosingTimeWorld {
 // Upper cornices frame the roof; they must not be roof-sized slabs covering its membrane.
 static void RoofCoping(int side,float x,float z,float h){
  const float edge=.22f,innerHalf=5.205f;
  foreach(int sign in new[]{-1,1})Box("Roof coping side",new(x+sign*5.315f,h+.12f,z),new(edge,.16f,12.55f),"stone");
  Box("Roof coping south",new(x,h+.12f,z-6.165f),new(innerHalf*2,.16f,edge),"stone");
  float accessX=side*(8.1f+(Mathf.RoundToInt(h/.9f)-1)*1.03f);
  // Bridge top and coping top are both h+.20: split the coping to avoid exact depth overlap.
  float west=x-innerHalf,east=x+innerHalf,cutWest=Mathf.Clamp(accessX-.86f,west,east),cutEast=Mathf.Clamp(accessX+.86f,west,east);
  if(cutWest>west)Box("Roof coping north west",new((west+cutWest)*.5f,h+.12f,z+6.165f),new(cutWest-west,.16f,edge),"stone");
  if(east>cutEast)Box("Roof coping north east",new((cutEast+east)*.5f,h+.12f,z+6.165f),new(east-cutEast,.16f,edge),"stone");
  Box("Roof cornice shadow edge",new(x,h-.02f,z),new(10.67f,.07f,12.38f),"iron");
 }
}}
