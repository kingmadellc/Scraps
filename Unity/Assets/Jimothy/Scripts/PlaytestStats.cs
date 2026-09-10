using UnityEngine;
using Unity.Profiling;
namespace Jimothy {
/// <summary>Opt-in measurements, not a claim that the device meets a shipping budget.</summary>
public sealed class PlaytestStats:MonoBehaviour {
 ProfilerRecorder batches,triangles;readonly float[] samples=new float[120],sorted=new float[120];int count,index;float next,average,p95;GUIStyle style;
 void OnEnable(){batches=ProfilerRecorder.StartNew(ProfilerCategory.Render,"Batches Count",1);triangles=ProfilerRecorder.StartNew(ProfilerCategory.Render,"Triangles Count",1);}
 void OnDisable(){batches.Dispose();triangles.Dispose();}
 void Update(){if(PlayerPrefs.GetInt("playtestStats",0)==0)return;float dt=Time.unscaledDeltaTime;if(dt<=0||dt>1)return;samples[index]=dt;index=(index+1)%samples.Length;count=Mathf.Min(count+1,samples.Length);if(Time.unscaledTime<next)return;next=Time.unscaledTime+.5f;float sum=0;for(int i=0;i<count;i++){sum+=samples[i];sorted[i]=samples[i];}System.Array.Sort(sorted,0,count);average=sum/count;p95=sorted[Mathf.Clamp(Mathf.CeilToInt(count*.95f)-1,0,count-1)];}
 void OnGUI(){
#if !UNITY_EDITOR && !DEVELOPMENT_BUILD
  return;
#else
if(PlayerPrefs.GetInt("playtestStats",0)==0)return;style??=new GUIStyle(GUI.skin.box){fontSize=17,alignment=TextAnchor.MiddleLeft,padding=new RectOffset(12,8,7,7)};float scale=Mathf.Max(.75f,Screen.width/1440f);var previous=GUI.matrix;GUI.matrix=Matrix4x4.Scale(Vector3.one*scale);string draws=batches.Valid&&batches.LastValue>0?batches.LastValue.ToString():"unavailable",tris=triangles.Valid&&triangles.LastValue>0?triangles.LastValue.ToString("N0"):"unavailable";GUI.Box(new Rect(12,150,335,94),$"PLAYTEST · {Application.version}\n{(average>0?1/average:0):0} fps · frame {average*1000:0.0} ms · p95 {p95*1000:0.0} ms\nBatches {draws} · visible tris {tris}",style);GUI.matrix=previous;
#endif
 }
}}
