using UnityEngine;
using UnityEngine.Rendering;
namespace Jimothy {
public static partial class ClosingTimeWorld {
 static void DenWordmark(Vector3 p){
  var font=Resources.Load<Font>("TitleFonts/FugazOne-Regular");font.RequestCharactersInTexture("THE DEN",96,FontStyle.Normal);
  var mat=new Material(Shader.Find("Jimothy/WorldLettering")){name="Den aviation wordmark"};mat.mainTexture=font.material.mainTexture;
  for(int layer=0;layer<2;layer++){var go=new GameObject("Den aviation wordmark");go.transform.SetParent(root,false);go.transform.position=p+new Vector3(layer==0?.025f:0,layer==0?-.019f:0,layer==0?.006f:0);var text=go.AddComponent<TextMesh>();text.font=font;text.text="THE DEN";text.fontSize=96;text.characterSize=.039f;text.anchor=TextAnchor.MiddleCenter;text.color=layer==0?new Color(.055f,.095f,.085f):new Color(.96f,.78f,.43f);var mr=go.GetComponent<MeshRenderer>();mr.sharedMaterial=mat;mr.shadowCastingMode=ShadowCastingMode.Off;}
  // Owned by one component; both renderers share the atlas material.
  var binding=root.gameObject.AddComponent<DenWordmarkBinding>();binding.font=font;binding.material=mat;
 }
 static void DenArchitecturalDetail(){
  Mat("denTrim",C(.12f,.18f,.155f),.24f);Mat("denLeather",C(.28f,.105f,.056f),.24f);Mat("denSteel",C(.36f,.39f,.38f),.7f,.65f);
  // Tongue-and-groove dado and baseboard frame the room, without shrinking circulation.
  Box("North wall painted wainscot",new(-14,DenFloor+.36f,63.17f),new(11.2f,.70f,.065f),"denTrim");
  for(int k=0;k<54;k++)Box("Wainscot panel seam",new(-19.4f+k*.20f,DenFloor+.36f,63.126f),new(.011f,.66f,.01f),"denInk");
  Box("Dado chair rail",new(-14,DenFloor+.74f,63.1f),new(11.25f,.07f,.10f),"wood");Box("North room skirting",new(-14,DenFloor+.09f,63.07f),new(11.28f,.18f,.11f),"cedar");
  foreach(float x in new[]{-19.64f,-8.36f})Box("Side room skirting",new(x,DenFloor+.08f,59.55f),new(.10f,.16f,7.0f),"wood");
  foreach(float x in new[]{-18.8f,-16.4f,-11.6f,-9.2f})Box("Exposed basement ceiling joist",new(x,1.48f,59.5f),new(.16f,.23f,7.25f),"wood");
  Cylinder("Old copper supply pipe",new(-19.55f,1.20f,56.2f),new(-19.55f,1.20f,63),.045f,"copper",10);Cylinder("Supply return across ceiling",new(-19.55f,1.20f,63),new(-8.5f,1.20f,63),.045f,"copper",10);
  foreach(float z in new[]{57f,59f,61f,62.7f}){Cylinder("Copper pipe union",new(-19.55f,1.20f,z-.06f),new(-19.55f,1.20f,z+.06f),.060f,"denSteel",10);Box("Pipe mounting cleat",new(-19.63f,1.20f,z),new(.13f,.16f,.08f),"iron");}
  // Floor joints are inset between visible boards; thin strips sit below the rug.
  for(int i=0;i<20;i++)Box("Basement floor board joint",new(-19.48f+i*.57f,DenFloor+.003f,59.65f),new(.012f,.005f,6.95f),"denInk");
  for(int i=0;i<12;i++)Box("Staggered floor end joint",new(-19.2f+i*.9f,DenFloor+.005f,57.7f+(i%3)*1.5f),new(.53f,.005f,.012f),"denInk");
  // Foot-level gig clutter sits at the back wall, outside the approach to the shelves.
  for(int i=0;i<5;i++){Box("Stacked independent record sleeve",new(-12.3f+i*.035f,DenFloor+.27f,62.88f),new(.30f,.52f,.036f),i%2==0?"wine":"navy",false,Quaternion.Euler(0,0,-10+i*3));}
  // One starter stompbox; the purchased board adds an unmistakably larger setup nearby.
  var pedal=new Vector3(-18.25f,DenFloor+.045f,61.57f);
  Box("Starter stomp pedal",pedal,new(.13f,.07f,.22f),"copper");
  Ellipsoid(pedal+new Vector3(0,.05f,-.065f),new(.018f,.025f,.018f),"denSteel",8,5);
  for(int k=0;k<3;k++)Ellipsoid(pedal+new Vector3((k-1)*.034f,.041f,.05f),new(.014f,.012f,.014f),"iron",8,4);
  // Patched rehearsal mat groups the guitar and amp without adding a collision lip.
  Box("Rehearsal corner felt mat",new(-18.05f,DenFloor+.009f,62.13f),new(2.65f,.012f,1.72f),"denInk");
  foreach(int side in new[]{-1,1})Box("Rehearsal mat frayed seam",new(-18.05f,DenFloor+.019f,62.13f+side*.81f),new(2.55f,.006f,.028f),"denTape");
  Vector3 previous=new(-18.65f,DenFloor+.025f,62.04f);
  for(int i=1;i<=28;i++){float t=i/28f;var point=new Vector3(-18.65f+t*1.10f,DenFloor+.025f,62.04f-Mathf.Sin(t*Mathf.PI)*.52f+Mathf.Sin(t*13)*.055f);Cylinder("Guitar cable loose floor coil",previous,point,.009f,"iron",5);previous=point;}

 }
 static void DenImportedInstrument(string resource,Vector3 position,float height){
  var asset=Resources.Load<GameObject>("DenAssets/"+resource);if(!asset){Debug.LogWarning("Missing Den instrument: "+resource);return;}var go=Object.Instantiate(asset,root);go.name="Authored Blender "+resource;go.transform.localPosition=Vector3.zero;go.transform.localRotation=Quaternion.Euler(0,180,0)*go.transform.localRotation;
  var renderers=go.GetComponentsInChildren<Renderer>();if(renderers.Length==0)return;var b=renderers[0].bounds;foreach(var r in renderers)b.Encapsulate(r.bounds);float scale=height/Mathf.Max(.01f,b.size.y);go.transform.localScale*=scale;b=renderers[0].bounds;foreach(var r in renderers)b.Encapsulate(r.bounds);go.transform.position+=position-new Vector3(b.center.x,b.min.y,b.center.z);
  DenInstrumentMaterials.Apply(go);
 }
}}
