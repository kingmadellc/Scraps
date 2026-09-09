# Rainier and harbor floatplane

This is authored scenic geometry, not a surveyed geographic reconstruction. The summit silhouette and northwestern glacier/ridge character were informed by:

- National Park Service, [Liberty Ridge climbing guide](https://www.nps.gov/mora/planyourvisit/upload/Liberty-Ridge-Routebrief.pdf), especially north-face overview and ridge descriptions. Liberty Ridge divides steep glacier walls below Liberty Cap; surrounding ridges and broad summit determine the distant outline.
- NPS, [Mount Rainier glaciers](https://www.nps.gov/mora/learn/nature/mount-rainier-glaciers.htm), including the North Mowich Glacier visibility from the Seattle/Tacoma area.
- [Mount Rainier summit photo](https://commons.wikimedia.org/wiki/File:Mount_Rainier_summit.jpg), showing the three summit masses from the northwest.

No reference photography is redistributed or mapped onto the game. All geometry and shader code are original procedural authoring.

`RainierHero.cs` builds one indexed relief mesh at (13,-8,-194), with a broad uneven summit profile and coherent descending ridge/glacier fans. Finite-difference normals follow the actual carved relief. Vertex colors blend snow, blue ice and volcanic rock based on elevation, slope and glacier channels. The dedicated `RainierDistance.shader` applies restrained moonlight, fine strata modulation and atmospheric perspective, avoiding the old distant fog flattening everything into one blue silhouette. Lower foothill strips that previously obscured the shoulders are removed.

Rainier: 15,617 vertices / 30,720 triangles / one renderer and one material. Previous mountain: 13,200 triangles / 39,600 separately emitted vertices, plus foothill strips. The new geometry increases triangles while reducing stored vertices and material batches. No collider or shadow casting. Runtime mesh/material explicitly destroyed with world.

`ShadowSeaplane.cs` creates one combined high-wing aircraft with rounded fuselage, cockpit, two long floats, float braces, wing braces, tailplane, fin and a small propeller silhouette. It is deliberately unbranded and shadowy. It has no colliders, real-time lights, or audio. Mesh count from construction: 624 vertices / 1,108 triangles / one renderer/material. Runtime creation logs authoritative counts.

First flyover begins after18 seconds of active outdoor gameplay, lasts38 seconds, and subsequent passes begin142 seconds apart. Track (-108,43,-117) to (112,43,-161), with a4m gentle vertical arc, is in front of the mountain and above the rooftops. Movement pauses in menus, pause and the den. Actual visibility from player cameras needs the coordinated Unity art audit; no automatic success claim is made from geometry.

Only integration change: existing `ClosingTimeArt.RainierScenery()` now calls `RainierHero.Build(root)` and `ShadowSeaplane.Build(root)`. Shared world street/building/camera methods are untouched.
