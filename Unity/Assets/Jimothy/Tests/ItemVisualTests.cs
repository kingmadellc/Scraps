using System.Linq;
using NUnit.Framework;
using UnityEngine;
namespace Jimothy.Tests {
public class ItemVisualTests {
 static readonly string[] exposed={"item_136","item_048","item_016","item_054","item_000","item_052","item_070","item_090","item_030","item_188","item_036","item_058","item_096","item_220","item_002","trophy_00","trophy_01","trophy_02","trophy_03","trophy_04","trophy_05","trophy_06"};
 [Test] public void EveryExposedRewardHasItsOwnSilhouette(){var items=JsonUtility.FromJson<ItemCatalog>(Resources.Load<TextAsset>("Items").text).items.ToDictionary(i=>i.id);Assert.AreEqual(22,exposed.Select(id=>ItemVisuals.ShapeKey(items[id])).Distinct().Count());}
 [Test] public void CatalogFallbacksRemainVaried(){var items=JsonUtility.FromJson<ItemCatalog>(Resources.Load<TextAsset>("Items").text).items;Assert.GreaterOrEqual(items.Select(ItemVisuals.ShapeKey).Distinct().Count(),25);Assert.IsTrue(items.All(i=>!string.IsNullOrEmpty(ItemVisuals.ShapeKey(i))));}
 [Test] public void RewardMeshesAreSolidSingleDrawAndBounded(){var items=JsonUtility.FromJson<ItemCatalog>(Resources.Load<TextAsset>("Items").text).items.ToDictionary(i=>i.id);foreach(string id in exposed){var root=ItemVisuals.Create(items[id],null,id);var mesh=root.GetComponent<MeshFilter>().sharedMesh;try{Assert.AreEqual(1,root.GetComponentsInChildren<Renderer>().Length,id);Assert.AreEqual(1,mesh.subMeshCount,id);Assert.AreEqual(mesh.vertexCount,mesh.colors.Length,id);Assert.Greater(mesh.bounds.size.y,.015f,id);Assert.Less(mesh.vertexCount,12000,id);Assert.AreEqual(0,root.GetComponentsInChildren<Collider>().Length,id);Assert.IsNull(root.GetComponent<ClosingWorldCleanup>(),id+" must not own or destroy its shared baked mesh");}finally{Object.DestroyImmediate(root);Assert.IsNotNull(mesh,id+" shared asset survives instance destruction");Assert.AreSame(Resources.Load<Mesh>("FindMeshes/"+id),mesh,id+" uses its baked mesh asset");}}}
}
}
