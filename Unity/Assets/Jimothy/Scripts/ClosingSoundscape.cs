using UnityEngine;
namespace Jimothy {
/// <summary>Small procedural placeholder soundscape, generated once per world. No per-frame sample generation.</summary>
public sealed class ClosingSoundscape:MonoBehaviour {
 const int Rate=22050;
 RaccoonMotor player;AudioSource breeze,harbor,paws;AudioClip wind,horn;readonly AudioClip[] steps=new AudioClip[3];
 Vector3 previous;float walked,nextHorn=36;int stepIndex;bool suspended;
 public void Initialize(RaccoonMotor motor){
  player=motor;previous=player.transform.position;
  wind=CreateWind();horn=CreateHorn();for(int i=0;i<steps.Length;i++)steps[i]=CreateStep(i);
  breeze=Source("Soft harbor air",.045f);breeze.clip=wind;breeze.loop=true;breeze.Play();
  harbor=Source("Distant boat horn",.085f);paws=Source("Raccoon pawsteps",.22f);
 }
 AudioSource Source(string name,float volume){var go=new GameObject(name);go.transform.SetParent(transform,false);var source=go.AddComponent<AudioSource>();source.playOnAwake=false;source.spatialBlend=0;source.volume=volume;source.priority=name=="Raccoon pawsteps"?110:200;return source;}
 static AudioClip Clip(string name,float[] samples){var clip=AudioClip.Create(name,samples.Length,1,Rate,false);clip.SetData(samples,0);return clip;}
 static float Noise(ref uint seed){seed=1664525u*seed+1013904223u;return ((seed>>8)/16777215f)*2-1;}
 static AudioClip CreateWind(){
  var samples=new float[Rate*8];uint seed=81;float low=0,slow=0;
  for(int i=0;i<samples.Length;i++){float t=(float)i/Rate;low=Mathf.Lerp(low,Noise(ref seed),.025f);slow=Mathf.Lerp(slow,low,.025f);float envelope=.55f+.15f*Mathf.Sin(t*Mathf.PI*.5f);samples[i]=slow*envelope;}
  // Crossfade both ends to silence to prevent a click at the loop seam.
  int fade=Rate/3;for(int i=0;i<fade;i++){float gain=(float)i/fade;samples[i]*=gain;samples[samples.Length-1-i]*=gain;}
  return Clip("Procedural low harbor breeze",samples);
 }
 static AudioClip CreateHorn(){
  var samples=new float[(int)(Rate*2.8f)];for(int i=0;i<samples.Length;i++){float t=(float)i/Rate;float envelope=Mathf.SmoothStep(0,1,t/.7f)*(1-Mathf.SmoothStep(0,1,(t-1.4f)/1.4f));float phase=t*98*Mathf.PI*2;samples[i]=(Mathf.Sin(phase)*.36f+Mathf.Sin(phase*1.5f)*.14f+Mathf.Sin(phase*2)*.035f)*envelope;}
  return Clip("Procedural distant harbor horn",samples);
 }
 static AudioClip CreateStep(int variant){
  var samples=new float[(int)(Rate*.105f)];uint seed=(uint)(200+variant*93);float soft=0;for(int i=0;i<samples.Length;i++){float t=(float)i/Rate;soft=Mathf.Lerp(soft,Noise(ref seed),.32f);float envelope=Mathf.Min(1,t/.003f)*Mathf.Exp(-t*65);samples[i]=(Mathf.Sin(t*(130+variant*19)*Mathf.PI*2)*.27f+soft*.2f)*envelope;}
  return Clip("Soft pawstep "+variant,samples);
 }
 void Update(){
  var session=GameSession.Instance;if(!player||!session||!breeze)return;
  bool pause=!session.Playing||session.Paused;
  if(pause!=suspended){suspended=pause;if(pause){breeze.Pause();harbor.Stop();paws.Stop();}else breeze.UnPause();previous=player.transform.position;walked=0;}
  if(suspended)return;
  float indoors=ClosingTimeWorld.DenContains(player.transform.position)?1:0;breeze.volume=Mathf.Lerp(.045f,.008f,indoors);harbor.volume=Mathf.Lerp(.085f,.012f,indoors);
  Vector3 delta=player.transform.position-previous;previous=player.transform.position;float distance=new Vector2(delta.x,delta.z).magnitude;
  // Teleports and airborne movement never produce a burst of footsteps.
  if(distance<.7f&&player.Grounded&&!player.IsMantling){walked+=distance;if(walked>1.1f){walked%=1.1f;paws.pitch=.94f+(stepIndex%3)*.055f;paws.PlayOneShot(steps[stepIndex%steps.Length]);stepIndex++;}}
  else walked=0;
  nextHorn-=Time.deltaTime;if(nextHorn<=0){harbor.PlayOneShot(horn);nextHorn=75+(stepIndex%4)*13;}
 }
 void OnDestroy(){if(wind)Destroy(wind);if(horn)Destroy(horn);foreach(var clip in steps)if(clip)Destroy(clip);}
}
}
