# Authored find catalogue

The source defines 152 named object recipes: 104 base objects with two condition variants, 32 standalone finds, and 16 trophies. All 256 IDs resolve explicitly. Shared helpers describe actual related objects (cups, keys, coins, paper goods, boats); they are not category fallbacks. Alternate conditions add geometry such as folds, embossing, wear, crumbs, or loose fibres. These remain 104 paired base designs, not 208 unrelated objects.

`ItemVisuals.BuildMesh(item)` authors the mesh. Runtime `Create` loads `Resources/FindMeshes/<id>` first and generates only when a baked asset is missing. Only generated meshes receive an ownership cleanup component; shared imported meshes are not destroyed when a discovery card or display closes.

Run `Jimothy.Editor.EditorBakeFindAssets.Run` in an exclusive Unity editor slot. The baker creates or updates individual `.asset` meshes, preserving existing asset GUIDs. It rejects missing recipes, nonfinite vertices, geometry outside the broad 8–6500 triangle guard, and exact geometry duplicates using SHA256 of vertex positions and indices (excluding colours). Most recipes are compact; intricate seeded bread, foliage and hero objects may exceed the typical 500–3000 triangle target. The resulting report records actual counts for every item.

Outputs:

- `Unity/Assets/Jimothy/Resources/FindMeshes/`: individual shared mesh assets.
- `Logs/find-mesh-coverage.json`: IDs, names, recipes, triangle counts, geometry hashes and errors.
- `PlaytestCaptures/find-catalogue/page-01.png` through `page-08.png`: 32 labelled objects per sheet, every ID shown.

All geometry is original procedural authoring, with no downloaded model or texture dependencies. A different geometry hash proves geometric difference, not artistic recognizability. Contact sheets require visual review; native discovery cards and den displays require runtime checks. The graphics-enabled Unity bake passed: 256 assets, 256 distinct geometry hashes, no missing recipes, nonfinite vertices or budget failures. Meshes range from 16 to 4,920 triangles (median 528; total 211,916 across the complete catalogue); 17 exceed the typical 3,000-triangle target. All eight contact sheets were inspected. The first run exposed one identical arcade-token variant and hidden marble/scone detail; the final run corrected them and improved the cheese cube, carved fish and Nordic Star silhouettes.

The sheets show actual URP renders with neutral directional and ambient inspection light. Each base-object family has authored geometry; paired variants remain visually related, and their smaller wear/fold/loose-detail differences are sometimes subtle at thumbnail scale. Coins and printed goods intentionally share their physical object families. This is not a claim that an unlabeled human recognition test passed. Native discovery-card rendering and revised den displays still require the integration checks coordinated by the parent agent. Unity exited successfully and the slot was released.
