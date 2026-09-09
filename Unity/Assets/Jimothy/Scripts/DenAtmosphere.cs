using UnityEngine;
namespace Jimothy {
/// <summary>Blend to a sheltered room without letting the directional moon wash through its ceiling.</summary>
public sealed class DenAtmosphere:MonoBehaviour {
 RaccoonMotor player;Light moon;float moonPower;Color sky,equator,ground;bool fog;float blend;
 public void Initialize(RaccoonMotor motor,Camera camera){player=motor;moon=RenderSettings.sun;moonPower=moon?moon.intensity:0;sky=RenderSettings.ambientSkyColor;equator=RenderSettings.ambientEquatorColor;ground=RenderSettings.ambientGroundColor;fog=RenderSettings.fog;}
 void LateUpdate(){if(!player)return;var p=player.transform.position;float desired=p.x>-20&&p.x<-8&&p.z>50&&p.z<63.5f?Mathf.InverseLerp(-.1f,-1.7f,p.y):0;blend=Mathf.MoveTowards(blend,desired,Time.unscaledDeltaTime*2.5f);if(moon)moon.intensity=moonPower*(1-blend);RenderSettings.ambientSkyColor=Color.Lerp(sky,new Color(.10f,.085f,.065f),blend);RenderSettings.ambientEquatorColor=Color.Lerp(equator,new Color(.075f,.060f,.05f),blend);RenderSettings.ambientGroundColor=Color.Lerp(ground,new Color(.028f,.023f,.02f),blend);RenderSettings.fog=fog&&blend<.85f;}
 void OnDestroy(){if(moon)moon.intensity=moonPower;RenderSettings.ambientSkyColor=sky;RenderSettings.ambientEquatorColor=equator;RenderSettings.ambientGroundColor=ground;RenderSettings.fog=fog;}
}
}
