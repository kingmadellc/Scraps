using UnityEngine;
namespace Jimothy {
public sealed class DenWordmarkBinding:MonoBehaviour {
 public Font font;public Material material;
 void OnEnable(){Font.textureRebuilt+=Rebind;}
 void Rebind(Font rebuilt){if(rebuilt==font&&material)material.mainTexture=font.material.mainTexture;}
 void OnDestroy(){Font.textureRebuilt-=Rebind;if(material)Destroy(material);}
}}
