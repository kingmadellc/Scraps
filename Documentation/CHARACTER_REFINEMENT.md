# Jimothy character refinement — v4

The approved seaplane concept remains the visual target. The supplied real animal and sculpture photographs define the unusual anatomy. This pass changes the actual Blender asset; the rooftop image is a native Blender render, not a substituted AI illustration.

## Changes

- Broader, rounded upper torso over long narrow limbs; low, forward head and short tail.
- Tapered muzzle, recessed eyes with eyelids, cupped ears and finer mask transitions.
- Modeled fingers, thumbs, small claws and whiskers attached to the skeleton.
- Denser directional undercoat and longer guard hairs with varied clumping.
- Localized leg weights keep the belly from following the nearest leg. Each fur strand inherits its root weights. Paw tips use the corresponding lower-leg bone so toes and claws stay together.
- Separate editable rooftop presentation scene with a removable bagel and warm rim lighting.

## Outputs

`Art/Blender/Jimothy.blend` is the groomed source. `Art/Blender/Jimothy-Presentation.blend` contains the posed rooftop scene. The gameplay FBX is one skinned mesh with 75,362 triangles, eight materials and eleven bones. Its 2K albedo and three clips round-trip through Blender's FBX importer; see `character-fbx-roundtrip.json`.

The actual source animation is shown in `Art/Renders/jimothy-gait-v4.mp4`. The 24-frame in-place loop repeats five times. Timing is an interpretation of the supplied stills, not motion capture of Jimothy. Foot contact and movement speed still need gameplay tuning.

## Remaining difference from the target

The face and fur remain more stylized and uniform than the approved illustration. The rooftop background is prototype scenery. The dense groom is for offline Blender renders and is excluded from the mobile FBX; real-time fur cards, LODs and device performance checks remain necessary. This refinement is not a claim of final visual approval or an iOS performance result.
