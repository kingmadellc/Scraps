using UnityEngine;
namespace Jimothy {
public static partial class ClosingTimeWorld {
 // A playable interpretation of historic paving and Marvin's Garden, not a surveyed reconstruction.
 static void BrickAvenue(){
  NightRoadMaterials();
  // Short spatial batches keep the forward renderer's local-light selection local to each block.
  for(float z=-38;z<56;z+=8){float end=Mathf.Min(z+8,56);RoadQuad(new(-5.45f,.005f,z),new(5.45f,.005f,z),new(5.45f,.005f,end),new(-5.45f,.005f,end));}
  // Tangent-continuous bend away from the straight retail block. Overlapping short sections prevent cracks.
  Vector3 last=new(0,0,-38);float distance=0;for(int i=1;i<=30;i++){float t=i/30f;var next=new Vector3(10*t*t,0,-38-30*t);var delta=next-last;var q=Quaternion.LookRotation(delta);var middle=(last+next)*.5f;
   float previous=(i-1)/30f;var tangent0=new Vector3(20*previous,0,-30).normalized;var tangent1=new Vector3(20*t,0,-30).normalized;var right0=Vector3.Cross(Vector3.up,tangent0)*5.45f;var right1=Vector3.Cross(Vector3.up,tangent1)*5.45f;var batch=GetBatch("streetBrick",middle);int index=batch.v.Count;
   batch.v.AddRange(new[]{last-right0+Vector3.up*.005f,last+right0+Vector3.up*.005f,next+right1+Vector3.up*.005f,next-right1+Vector3.up*.005f});batch.n.AddRange(new[]{Vector3.up,Vector3.up,Vector3.up,Vector3.up});foreach(var vertex in new[]{last-right0,last+right0,next+right1,next-right1})batch.uv.Add(new Vector2(vertex.x,vertex.z));batch.t.AddRange(new[]{index,index+2,index+1,index,index+3,index+2});distance+=delta.magnitude;
   foreach(int side in new[]{-1,1}){var edge=middle+q*(Vector3.right*side*6.65f);bool entrance=side==1 && i>=8 && i<=12;float level=entrance?.045f:.1f;var walk=GetBatch("paving",middle);int k=walk.v.Count;walk.v.AddRange(new[]{last+right0.normalized*side*5.45f+Vector3.up*level*2,last+right0.normalized*side*7.85f+Vector3.up*level*2,next+right1.normalized*side*7.85f+Vector3.up*level*2,next+right1.normalized*side*5.45f+Vector3.up*level*2});walk.n.AddRange(new[]{Vector3.up,Vector3.up,Vector3.up,Vector3.up});walk.uv.AddRange(new[]{new Vector2(0,distance),new Vector2(2.4f,distance),new Vector2(2.4f,distance+delta.magnitude),new Vector2(0,distance+delta.magnitude)});walk.t.AddRange(side==1?new[]{k,k+2,k+1,k,k+3,k+2}:new[]{k,k+1,k+2,k,k+2,k+3});var support=new GameObject("Bend sidewalk support");support.transform.SetParent(root,false);support.transform.position=edge+Vector3.up*level;support.transform.rotation=q;support.AddComponent<BoxCollider>().size=new Vector3(2.4f,level*2,delta.magnitude+.08f);if(!entrance)Box("Bend granite curb",middle+q*(Vector3.right*side*5.55f)+Vector3.up*.12f,new(.20f,.26f,delta.magnitude+.08f),"stone",false,q);}
   last=next;
  }
  Box("Garden accessible threshold",new(-7,.045f,-48),new(2,.09f,3.2f),"paving",true);
  foreach(var p in new[]{new Vector3(8,.22f,-46),new Vector3(13,.22f,-58),new Vector3(0,.22f,-65)})PoleLamp(p);
 }
 static void RingProfile(Vector3 p,float[] ys,float[] rs,string material,int sides=40){var b=GetBatch(material,p);for(int j=0;j<ys.Length-1;j++)for(int i=0;i<sides;i++){int k=b.v.Count;float a=i*2*Mathf.PI/sides,c=(i+1)*2*Mathf.PI/sides;float slope=(rs[j]-rs[j+1])/(ys[j+1]-ys[j]);foreach(var v in new[]{new Vector3(Mathf.Cos(a)*rs[j],ys[j],Mathf.Sin(a)*rs[j]),new Vector3(Mathf.Cos(a)*rs[j+1],ys[j+1],Mathf.Sin(a)*rs[j+1]),new Vector3(Mathf.Cos(c)*rs[j+1],ys[j+1],Mathf.Sin(c)*rs[j+1]),new Vector3(Mathf.Cos(c)*rs[j],ys[j],Mathf.Sin(c)*rs[j])}){b.v.Add(p+v);b.n.Add(new Vector3(v.x,Mathf.Sqrt(v.x*v.x+v.z*v.z)*slope,v.z).normalized);b.uv.Add(new Vector2(Mathf.Atan2(v.z,v.x)*1.2f,v.y));}b.t.AddRange(new[]{k,k+1,k+2,k,k+2,k+3});}}
 static void ParkBench(Vector3 p,float yaw){var q=Quaternion.Euler(0,yaw,0);void Part(string name,Vector3 off,Vector3 size,string mat,bool solid=false)=>Box(name,p+q*off,size,mat,solid,q);for(int i=0;i<4;i++)Part("Park bench seat slat",new(0,.46f,-.22f+i*.14f),new(1.65f,.07f,.12f),"cedar",true);for(int i=0;i<3;i++)Part("Park bench back slat",new(0,.65f+i*.14f,.27f),new(1.65f,.105f,.065f),"cedar");foreach(int side in new[]{-1,1}){Part("Bench feet",new(side*.64f,.23f,0),new(.075f,.46f,.48f),"iron");Part("Bench back upright",new(side*.64f,.65f,.28f),new(.06f,.65f,.06f),"iron");}}
 static void BellGarden(){var p=new Vector3(-12,.2f,-52);
  Box("Marvin garden stone patio",new(-12,.095f,-54),new(10,.19f,16),"paving",true);
  foreach(var off in new[]{new Vector3(0,0,6.8f),new Vector3(-4.7f,0,4),new Vector3(4.7f,0,4),new Vector3(-4.7f,0,-4),new Vector3(4.7f,0,-4)})ParkBench(p+off,off.x<0?90:off.x>0?-90:180);
  // Two piers plus a true open semicircular arch retain a walk-through passage.
  foreach(int side in new[]{-1,1}){Box("Bell tower brick pier",p+new Vector3(side*1.22f,1.3f,0),new(.72f,2.6f,2.1f),"brickRust",true);Box("Tower granite footing",p+new Vector3(side*1.22f,.18f,0),new(.90f,.36f,2.28f),"stone",true);}
  for(int i=0;i<20;i++){float a=(i+.5f)*Mathf.PI/20;var at=p+new Vector3(Mathf.Cos(a)*1.20f,2.6f+Mathf.Sin(a)*1.20f,0);Box("Arch radial brick voussoir",at,new(.38f,.24f,2.1f),"brickRust",true,Quaternion.Euler(0,0,a*Mathf.Rad2Deg));}
  RingProfile(p,new[]{3.8f,3.95f,4.02f,5.1f,5.18f,5.28f},new[]{1.58f,1.65f,1.48f,1.48f,1.65f,1.65f},"brickRust");
  foreach(float y in new[]{3.9f,5.22f})RingProfile(p,new[]{y,y+.10f},new[]{1.68f,1.68f},"stone");
  foreach(int x in new[]{-1,1})foreach(int z in new[]{-1,1}){var at=p+new Vector3(x*.94f,5.25f,z*.94f);Cylinder("White bell pavilion column",at,at+Vector3.up*2.05f,.11f,"cream",16);Box("Column capital",at+Vector3.up*1.99f,new(.32f,.16f,.32f),"cream");}
  RingProfile(p,new[]{7.27f,7.38f,7.55f,7.72f,7.86f,7.93f},new[]{1.74f,1.77f,1.49f,1.12f,.60f,.02f},"copper");
  Cylinder("Bell suspension",p+Vector3.up*6.25f,p+Vector3.up*7.32f,.045f,"iron");RingProfile(p,new[]{5.52f,5.60f,5.70f,5.95f,6.20f,6.27f},new[]{.62f,.65f,.49f,.32f,.25f,.03f},"gold");Ellipsoid(p+new Vector3(0,5.52f,0),new(.10f,.13f,.10f),"iron");
  Box("Bell park plaque",p+new Vector3(1.23f,1.45f,1.07f),new(.51f,.58f,.035f),"copper");Text("BALLARD\n1889",p+new Vector3(1.23f,1.46f,1.096f),.012f,"cream",Quaternion.Euler(0,180,0));
  Text("MARVIN'S GARDEN",p+new Vector3(0,.035f,4.4f),.028f,"iron",Quaternion.Euler(90,180,0));
  for(int i=0;i<8;i++){float a=i*Mathf.PI/4;var at=p+new Vector3(Mathf.Sin(a)*.72f,.012f,Mathf.Cos(a)*.72f+3);Box("Inlaid compass ray",at,new(.06f,.012f,1.1f),i%2==0?"copper":"stone",false,Quaternion.Euler(0,i*45,0));}
  foreach(var off in new[]{new Vector3(-5,0,-8),new Vector3(5,0,-8),new Vector3(5,0,8)}){Tree(p+off);Planter(p+off+Vector3.forward*1.8f);}
  PoleLamp(p+new Vector3(-4,0,7));PoleLamp(p+new Vector3(4,0,-6));Light(p+new Vector3(0,6.3f,0),C(1,.73f,.43f),9,3);
 }
}}
