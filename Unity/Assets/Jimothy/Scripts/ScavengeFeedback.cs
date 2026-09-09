using UnityEngine;
namespace Jimothy {
/// <summary>Short, quiet success tones generated once; no downloaded audio or per-pickup allocation.</summary>
public sealed class ScavengeFeedback:MonoBehaviour {
 AudioSource source;AudioClip pickup,bank;
 void Awake(){source=gameObject.AddComponent<AudioSource>();source.playOnAwake=false;source.spatialBlend=0;source.volume=.19f;pickup=Make("Parcel discovery",false);bank=Make("Keepsake banked",true);}
 static AudioClip Make(string name,bool reward){const int rate=22050;float duration=reward?.55f:.22f;var data=new float[(int)(rate*duration)];for(int i=0;i<data.Length;i++){float t=(float)i/rate;float note=reward?(t<.16f?523.25f:t<.32f?659.25f:783.99f):(t<.09f?659.25f:880);float local=reward?t% .16f:t% .09f;float envelope=Mathf.Min(1,local/.006f)*Mathf.Exp(-local*16)*(1-t/duration);data[i]=(Mathf.Sin(t*note*Mathf.PI*2)*.7f+Mathf.Sin(t*note*Mathf.PI*4)*.12f)*envelope;}var clip=AudioClip.Create(name,data.Length,1,rate,false);clip.SetData(data,0);return clip;}
 public void Play(bool reward){if(source)source.PlayOneShot(reward?bank:pickup);}
 void OnDestroy(){if(pickup)Destroy(pickup);if(bank)Destroy(bank);}
}
}
