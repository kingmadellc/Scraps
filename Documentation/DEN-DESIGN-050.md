# Den design pass — 0.5.0

September 10, 2026. Source implementation and review by the Den design agent. No human testing is claimed. No Unity, Blender, browser, or Git process was launched by this agent; runtime validation is assigned to the coordinating agent.

## Observed problem

Reviewed actual previous captures `PlaytestCaptures/den-polish-v045/contact-sheet.png` and `PlaytestCaptures/den-populated/contact-sheet.png`, together with the current Den source. The imported guitar and amplifier are already detailed assets worth keeping. The amplifier's display crate was positioned directly in front of the cabinet; the populated close view showed the crate and collectible taking foreground space from the grille. The guitar, floor lamp, mushroom lamp, and pedalboard crowded one small area. The room had useful grunge details, but the music equipment needed clearer separation from collection storage.

## Implemented composition

- Kept the sofa, coffee table, main rug, wall shelves, original gig posters, entrance ramp, room footprint, and collectible selection/order unchanged.
- Moved the amplifier slightly west to (-18.65, floor, 62.20) and lifted the imported cabinet 0.18 m onto a low road case. The case has restrained metal corner protectors and a crooked gaffer label. Its collider supports the raised cabinet; the cabinet collision moved up by the same amount.
- Moved the former amplifier-front display crate to the east lounge edge at (-11.65, floor, 61.20). It remains the same supported crate and the final collection slot; all 32 display slots remain present. This deliberately removes a competing collectible from the amplifier's foreground without deleting a display.
- Pulled the guitar forward to (-17.55, floor + 0.06, 61.95), preserving the authored FBX and its stand. A dark rehearsal mat groups the guitar and amplifier visually. Fine seams and a short cable remain noncolliding surface detail.
- Reduced baseline pedals to one starter stompbox. The purchased pedalboard now occupies its own position at (-17.55, -2.37, 61.12), so the upgrade changes the room visibly without overlapping that starter pedal or the guitar stand.
- Moved the tall reading lamp behind the sofa's west edge. Repositioned the existing west ceiling practical toward the music corner at (-18.3, 0.9, 61.0), illuminating it from the front. No extra light was added; imported instrument materials and texture assets remain unchanged.
- Moved the optional mushroom lamp to the east sofa end at (-12.6, floor, 62.4), away from the instrument silhouettes. The record crate, reading nook, vinyl wall, collection furnishings, and other purchases retain their saved IDs and unlock requirements.

Changes are in `ClosingTimeDen.cs`, `ClosingTimeDenPolish.cs`, and `DenFurnishings.cs`. `DenCollection`, `DenAtmosphere`, instrument materials, reward mappings, inventory, save data, UI interactions, decoration prices, and ownership rules were not changed.

## Capture preparation

Updated `Jimothy.Editor.DenPolishArtAudit.Run` to produce six actual URP frames in `PlaytestCaptures/den-design-v050/` and a 3-by-2 contact sheet:

1. Empty Den, rear gameplay camera at the normal interior position.
2. Empty Den, low music-corner inspection camera.
3. Empty Den, room overview.
4. Fully furnished/populated Den, same rear camera.
5. Fully furnished/populated Den, same music inspection camera.
6. Fully furnished/populated Den, same room overview.

The audit creates an isolated saving-suppressed fixture, then seeds ownership only for the second row. It is an art inspection fixture, not evidence that a player earned the collection or completed a continuous traversal. It uses the gameplay camera's configured FOV and `RenderPipeline.SubmitRenderRequest`; it hides the HUD and freezes simulation for consistent comparisons. Previous 0.4.5 captures are not overwritten. The menu entry is now uniquely named `Jimothy/Capture Den design 050 evidence`.

## Validation status and required checks

Source reviewed for placement bounds and preservation of IDs; runtime compile, geometry and visual checks are **pending**. Do not describe this pass as visually verified until the new captures have been inspected.

- Run `Jimothy.Editor.DenGeometryAudit.Run`: continuous street/ramp/interior/exit movement and all 32 support raycasts should still pass. The new road case is colliding geometry in the back music corner, and the moved collection crate is the main collision regression to check.
- Run `Jimothy.Editor.DenExperienceAudit.Run` and the existing Den collection/persistence checks: deposits, favorite selection, hide/show, owned decorations, and reload must preserve inventory and display mapping.
- Run the updated DenPolishArtAudit and inspect all six frames. Confirm the amplifier grille is recognizable from the normal rear view, the guitar's body/neck stay distinct, and no optional furnishing covers either in the populated row.
- Verify the imported amplifier's visible bounds fit the cabinet collider and sit on the road case, the guitar stand remains grounded, and the relocated crate has a supported collectible.
- Inspect the ceiling bulb's new location and lamp illumination for excessive highlights on the guitar or raccoon's fur. No new brightness/performance claim is made from source alone.
- Walk close to the music corner in a native or browser build and inspect rear-camera collision behavior. The automated geometry route covers central circulation, not every possible approach to the amp.

The goal is a deliberately arranged 1990s basement music hangout that becomes more personal through earned collections. This implementation does not establish human preference, measured performance, or physical-device usability.

## Follow-up after first 0.5.0 captures

Inspected all six new den-design-v050 images. Frames 01/04 confirm the amplifier and guitar are individually recognizable and the cabinet grille is no longer covered by the collection crate. The baseline sofa and large rug still looked excessively plain. A second source refinement adds rounded back pads/arm rolls, small upholstery repairs, a folded wool throw, and sparse woven rug lozenges. The three original flyers now have distinct wave, burst, and record artwork rather than repeating one symbol. All added details are batched with existing materials, add no colliders/lights/downloads, and preserve the existing circulation and collection supports. These refinements require new captures before visual verification.

A dark horizontal slab/band is visible in **rear gameplay frames 00/03 as well as overview frames 02/05**. This is not being dismissed as an inspection-camera artifact or hidden by moving the camera. The coordinating agent was asked to identify its actual intersecting geometry: the documented ceiling and joists are higher than the apparent band. Camera poses remain unchanged for an honest before/after comparison.

The slab cause was subsequently identified in `ClosingTimeNeighborhood.cs`: the decorative asphalt tile centered at (0, -0.16, 80), size (39, 0.18, 39), reaches south to z=60.5 and crosses the back of the Den. It has no collider, explaining why collision-only traversal checks missed it. Root owns clipping its south edge to z=64 and adding a rendered-geometry regression. That fix and refreshed captures remain pending at this note's timestamp.

## Refreshed capture review and audit compatibility

All six refreshed images were inspected after root's scenery fix. The slab/band is absent from both rear gameplay and overview cameras. Amp/guitar remain distinct; sofa pads, repaired throw, and rug pattern improve the unpurchased room, while the populated row visibly adds collections and decor. Root reports DenGeometryAudit and DenSceneryClearance passing. The dinner flyer's intended record art currently reads nearly blank: the shared Cylinder helper produces an uncapped tube rather than a filled disk. This is a minor poster-art defect, reported to root separately, not a traversal or instrument-visibility failure.

Updated DenExperienceAudit's obsolete avenue-spawn expectation to the production Den/night-board start. It checks inactive empty state, paused management, an enabled Head out button, invokes that actual UI handler, verifies run activation/movement with no relocation, then physically walks out and back through the ramp before the existing banking/display/save/reload assertions. Its final continuous exit, camera collision sampling, and save-file byte comparisons remain intact. Runtime rerun is pending; no Unity was launched by this agent.


Final integration verification: the 0.5.0 Unity art/UI audits and actual rendered captures passed review. The center-bay mullion fix is included in the final storefront captures. Chromium passes 16 end-to-end checks and Safari/WebKit passes 9 checks without runtime errors. See DESIGN-REVIEW-050.md for scope and remaining limitations.
