# Jimothy V8 longer groom

V8 rebuilds only the groom from the finalized V7 Blender asset. The skin coordinates, bind skeleton and all animation key/handle data are hash-identical, verified by `character-v8-preservation.json`. Original V7 remains intact.

The shoulder and flank fibers are longer, layered and gently curved, with shorter face/leg fur. There are 75,702 fibers. Surface-derived custom normals prevent each narrow triangle lighting as a separate bright facet. Silver guard contrast is reduced. The inherited baked undercoat remains unchanged.

The FBX export explicitly uses LINEAR vertex colors. Blender's default SRGB export wrote brighter encoded colors that the custom runtime shader consumed directly as linear values. This is a concrete encoding mismatch; its visual correction still needs Unity/native verification. The shader also removes additive grazing sheen and respects outward surface normals on both strand sides.

A single damped spring drives GPU tip offsets, weighted by strand length and squared root-to-tip coordinate. Long shoulder hairs bend more; face and leg hairs remain restrained. Gentle coherent breeze is spatially low frequency. The motion offset is capped at 2.2 cm world space and resets on teleport. Roots remain fixed. No per-fiber CPU update, geometry shader or tessellation is used.

## Geometry costs

Actual staged FBX roundtrip: 152,146 triangles, 280,642 vertices total across two skinned meshes and 15 bones. Fur alone: 105,950 triangles / 257,354 vertices. V7 was 145,890 triangles. This is a quality-first asset with substantial skinned vertex cost; it is not a device performance claim or a completed LOD solution.

## Verification

- Blender rear/profile/front renders inspected: longer directional dark pelt and shoulder/flank silhouette. Images: `Art/Renders/jimothy-v8-{rear,side,front}.png`.
- Staged FBX imported independently: two meshes, weighted vertices, UV and color layers, 15 bones, all four expected actions. Waddle frames 1–13, Walk 1–17, Idle/Jump 1–61 unchanged.
- Blender V7/V8 skin, bone and every action-key signature identical.
- Staged export passed before replacing Resources/Jimothy.fbx. Prior runtime FBX archived at Art/Exports/Jimothy-v7-runtime.fbx.
- No Unity run by this agent. Native/Web shader compilation, temporal motion and device performance remain integration checks.

## In-game coverage correction and motion evidence

Actual Unity A/B captures showed that disabling geometric hairs removed the pale flecks; disabling the base normal map did not. Linear export alone was insufficient. The final groom therefore widens the tapered fiber ribbons to 0.0045–0.0065 model units, improving overlapping coverage without adding vertices or triangles. The shader no longer forces a bright .26–.36 ambient floor over a dark undercoat; it uses scene SH with a small .025–.033 minimum instead.

Final `FurVisualAudit.Run` completed with exit0. It captures full/no-hair/no-normal pairs at close and game distance, followed by actual input run, walk and stop. Primary evidence in `PlaytestCaptures/fur-v8-diagnostic` requests production 2x MSAA, 1280×720; the separate `fur-v8-4x-diagnostic` is diagnostic only. Native agent-capture target was corrected from1x to2x to match production. This does not change production quality. Earlier sparse coverage comparison is in `fur-v8-before-coverage`.

Final close image fur-00 shows connected dark directional strands; fur-03 shows the gameplay-distance silhouette. Fine edge aliasing remains. These are actual Unity URP renders, not conceptual art; a fresh standalone/native/Web build and physical-device performance remain separate integration checks.

Six-decimal runtime material-property evidence records run tip spring settling near +4.20 mm, slower walk near +1.47 mm, and stop recoil -2.88 mm then -1.00 mm then -.237 mm. Those are measured world-space shader spring inputs, not a claim of independently measured individual GPU vertices. Root-to-tip weighting is zero at roots and strongest for the longer fibers. The snapshots also show changing real locomotion poses. All test sessions suppressed saving.
