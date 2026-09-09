using System;
using UnityEditor;
using UnityEngine;
namespace Jimothy.Editor {
/// <summary>Checks actual authored world collision, not just intended coordinates.</summary>
public static class ScavengePlacementAudit {
 public static void Run(){
  var root=new GameObject("Find placement audit");int sites=0,nodes=0;
  try{ClosingTimeWorld.Build(root.transform);NeighborWorldObstacles.Ensure();RooftopConnections.Build(root.transform);Physics.SyncTransforms();
   foreach(var spot in ClosingTimeWorld.Spots){var valid=ScavengeSites.ValidSites(spot);int minimum=spot.id=="welcome_snack"?1:2;
    if(valid.Count<minimum)throw new Exception(spot.id+" only "+valid.Count+" accessible candidate sites; require "+minimum);
    foreach(var site in valid){if(!spot.rooftop&&spot.id!="welcome_snack"&&Mathf.Abs(site.position.x)<7.5f)throw new Exception("Exposed road find: "+spot.id);if(Vector3.Distance(site.position,site.approach)>1.3f)throw new Exception("Search approach out of reach: "+spot.id);}
    Debug.Log("FIND_SITE "+spot.id+" validated="+valid.Count);sites+=valid.Count;nodes++;
   }
   if(nodes!=22)throw new Exception("Expected22 stable find identities, got "+nodes);
   Debug.Log("SCAVENGE_PLACEMENT PASS "+nodes+" stable nodes / "+sites+" supported, clear, searchable sites");
  }finally{UnityEngine.Object.DestroyImmediate(root);}
 }
}
}
