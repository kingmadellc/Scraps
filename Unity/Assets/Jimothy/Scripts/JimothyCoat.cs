using UnityEngine;
using UnityEngine.Rendering;
namespace Jimothy {
/// <summary>Root-anchored fur: one damped motion spring, GPU tip bending, no per-strand CPU simulation.</summary>
public sealed class JimothyCoat:MonoBehaviour {
 Material coat,baseCoat;Renderer fur;Vector3 previous,velocity,lag,lagVelocity;MaterialPropertyBlock properties;
 public void Initialize(){
  previous=transform.position;properties=new MaterialPropertyBlock();
  foreach(var r in GetComponentsInChildren<Renderer>()){
   var materials=r.sharedMaterials;bool changed=false;
   for(int i=0;i<materials.Length;i++){
    if(materials[i]&&materials[i].name.Contains("painted coat")){
     baseCoat=new Material(materials[i]);baseCoat.SetTexture("_BaseMap",Resources.Load<Texture2D>("Jimothy_Albedo_2K"));baseCoat.SetColor("_BaseColor",new Color(1f,.98f,.96f));baseCoat.SetFloat("_Smoothness",.08f);baseCoat.SetTexture("_BumpMap",Resources.Load<Texture2D>("Surfaces/Jimothy_Undercoat_Normal"));baseCoat.SetFloat("_BumpScale",.65f);baseCoat.EnableKeyword("_NORMALMAP");baseCoat.SetTextureScale("_BaseMap",Vector2.one);baseCoat.SetTextureOffset("_BaseMap",Vector2.zero);baseCoat.SetFloat("_EnvironmentReflections",0);baseCoat.EnableKeyword("_ENVIRONMENTREFLECTIONS_OFF");materials[i]=baseCoat;changed=true;
    }
    if(materials[i]&&materials[i].name.Contains("realtime fur")){
    coat=new Material(Shader.Find("Jimothy/RootedFur"));coat.SetColor("_BaseColor",Color.white);coat.SetFloat("_FiberSheen",.06f);materials[i]=coat;fur=r;changed=true;r.shadowCastingMode=ShadowCastingMode.Off;
   }
   }
   if(changed)r.sharedMaterials=materials;
  }
 }
 public void SetTint(Color tint){if(coat)coat.SetColor("_BaseColor",tint);if(baseCoat)baseCoat.SetColor("_BaseColor",tint*new Color(1f,.98f,.96f));}
 void LateUpdate(){
  float dt=Mathf.Min(Time.deltaTime,.05f);if(dt<=0||!fur)return;
  Vector3 displacement=transform.position-previous;
  if(displacement.sqrMagnitude>1){previous=transform.position;velocity=lag=lagVelocity=Vector3.zero;properties.SetVector("_FurMotion",Vector4.zero);fur.SetPropertyBlock(properties);return;}
  Vector3 next=displacement/Mathf.Max(dt,.001f);Vector3 acceleration=(next-velocity)/Mathf.Max(dt,.001f);previous=transform.position;velocity=next;
  Vector3 target=Vector3.ClampMagnitude(-acceleration*.0018f-next*.0014f,.022f);
  lag=Vector3.SmoothDamp(lag,target,ref lagVelocity,.18f,1,dt);properties.SetVector("_FurMotion",lag);fur.SetPropertyBlock(properties);
 }
 void OnDestroy(){if(coat)Destroy(coat);if(baseCoat)Destroy(baseCoat);}
}}
