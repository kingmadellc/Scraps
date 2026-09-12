using UnityEngine;
using UnityEngine.UI;
namespace Jimothy {
// One small render target, created only while the wardrobe is open. No gameplay camera changes.
public sealed class WardrobePreview:MonoBehaviour {
 GameObject stage,model;Camera camera;RenderTexture image;float angle=48;
 public void Initialize(RawImage target,string look,int coat,bool collector=false){
  angle=look=="roadie"||look=="inspector"?145:48;stage=new GameObject("Wardrobe photo booth");stage.transform.position=new Vector3(0,-600,0);
  model=Instantiate(Resources.Load<GameObject>("Jimothy"),stage.transform);model.transform.localPosition=Vector3.zero;model.transform.localScale=(collector?new Vector3(1.22f,.78f,1.05f):Vector3.one)*.22f;
  foreach(var t in model.GetComponentsInChildren<Transform>())t.gameObject.layer=31;
  model.AddComponent<JimothyCoat>().Initialize();model.AddComponent<RaccoonWardrobe>().Apply(look,coat);
  var cg=new GameObject("Wardrobe portrait camera");cg.transform.SetParent(stage.transform,false);camera=cg.AddComponent<Camera>();camera.cullingMask=1<<31;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.045f,.085f,.072f);camera.nearClipPlane=.01f;camera.farClipPlane=8;camera.fieldOfView=35;
  image=new RenderTexture(512,512,24,RenderTextureFormat.ARGB32);image.antiAliasing=2;image.Create();camera.targetTexture=image;target.texture=image;
  var lg=new GameObject("Wardrobe softbox");lg.transform.SetParent(stage.transform,false);lg.transform.localPosition=new Vector3(-1,1.2f,1.3f);var light=lg.AddComponent<Light>();light.type=LightType.Point;light.range=5;light.intensity=9f;light.color=new Color(1,.85f,.68f);light.cullingMask=1<<31;light.shadows=LightShadows.None;
  var fillObject=new GameObject("Wardrobe fill");fillObject.transform.SetParent(stage.transform,false);fillObject.transform.localPosition=new Vector3(1,.65f,-.8f);var fill=fillObject.AddComponent<Light>();fill.type=LightType.Point;fill.range=4;fill.intensity=1.5f;fill.color=new Color(1f,.96f,.9f);fill.cullingMask=1<<31;
  Frame();
 }
 void Frame(){var rs=model.GetComponentsInChildren<SkinnedMeshRenderer>();var b=rs[0].bounds;foreach(var r in rs)b.Encapsulate(r.bounds);float d=Mathf.Max(b.size.y,b.size.x,b.size.z)*1.8f;camera.transform.position=b.center+Quaternion.Euler(0,angle,0)*new Vector3(0,d*.22f,d);camera.transform.LookAt(b.center+Vector3.up*b.size.y*.05f);}
 public void Turn(){angle+=90;Frame();}
 void OnDisable(){if(stage)stage.SetActive(false);}
 void OnDestroy(){if(camera)camera.targetTexture=null;if(stage){stage.SetActive(false);Destroy(stage);}if(image){image.Release();Destroy(image);}}
}
}
