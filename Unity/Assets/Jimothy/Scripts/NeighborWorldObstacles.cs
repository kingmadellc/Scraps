using UnityEngine;
namespace Jimothy {
/// <summary>Colliders for ground props whose visible meshes are batched without physics.
/// Dimensions match ClosingTimeWorld; individual pieces leave the real under-table gaps open.</summary>
public sealed class NeighborWorldObstacles:MonoBehaviour {
 public int ColliderCount {get;private set;}
 public static void Ensure(){
  var world=GameObject.Find("Ballard Avenue — closing time");if(!world||world.GetComponent<NeighborWorldObstacles>())return;
  var owner=world.AddComponent<NeighborWorldObstacles>();owner.Build();Physics.SyncTransforms();
 }
 void Box(string label,Vector3 center,Vector3 size){ColliderCount++;var go=new GameObject(label+" collision");go.transform.SetParent(transform,false);go.transform.localPosition=center;go.AddComponent<BoxCollider>().size=size;}
 void Post(string label,Vector3 bottom,float height,float radius){ColliderCount++;var go=new GameObject(label+" collision");go.transform.SetParent(transform,false);go.transform.localPosition=bottom+Vector3.up*height*.5f;var c=go.AddComponent<CapsuleCollider>();c.radius=radius;c.height=height;c.direction=1;}
 void Build(){
  foreach(int side in new[]{-1,1}){
   foreach(float z in new[]{-24f,7f,25f}){
    var p=new Vector3(side*7.55f,.22f,z);Box("Cafe table",p+Vector3.up*.74f,new(.86f,.08f,.74f));Post("Table pedestal",p,.7f,.06f);
    foreach(int j in new[]{-1,1}){var chair=p+Vector3.forward*j*.8f;Box("Chair seat",chair+Vector3.up*.42f,new(.43f,.06f,.43f));Box("Chair back",chair+new Vector3(0,.7f,j*.2f),new(.43f,.5f,.05f));foreach(int k in new[]{-1,1})foreach(int leg in new[]{-1,1})Post("Chair leg",chair+new Vector3(k*.17f,0,leg*.17f),.43f,.025f);}
   }
   for(int row=0;row<5;row++){
    float z=-31+row*16;Post("Street lamp",new(side*5.95f,.22f,z),4.2f,.065f);Box("Lamp foot",new(side*5.95f,.37f,z),new(.28f,.30f,.28f));
    float signZ=-30+row*16+6.95f+.85f;Box("Route sign",new(side*6.35f,.78f,signZ),new(.85f,.58f,.07f));Post("Route sign post",new(side*6.35f,.2f,signZ),.34f,.045f);
   }
  }

 }
}
}
