# Scraps design review — 0.5.0

The September 10 review used three specialist agents for UI/UX, the Den, and storefront/environment design, with the coordinating agent handling fur, integration and verification. Findings come from source inspection, actual Unity/browser images and interaction checks. They are heuristic findings, not a human study, preference ranking or playtest score.

## Decisions implemented

| Journey | Problem | Change |
|---|---|---|
| Start and settings | Repeated location/slogan/version copy, verbose actions and prototype terminology | Keep the Scraps wordmark and hero art; short Continue/New game/Settings actions; consistent display typography; controls live in pause |
| Explore | Large permanent panels and tutorials compete with the avenue | Compact goal/direction pills, pockets and labeled survival bars; one initial movement hint; contextual search, suspicion and landing feedback |
| Plan an outing | Rule explanations and collection recipes compete with the goal | Goal and banking/loss rule stay visible; detailed rules and collection requirements use secondary screens |
| Find treasure | Celebration repeated poster slogans and status copy | Keep the actual rotating item, rarity/value and the instruction to bank; shorten the rest without changing modal safety |
| Come home | Amp blocked by a collection crate; upgrades crowd instruments | Separate music corner, raised amp, grounded road case, distinct guitar/pedal positions, moved display crate and lamp |
| Grow the Den | Baseline furniture and flyers lack detail; populated room needs readable rewards | Rounded upholstery, repair stitching, throw, worn rug pattern, different gig-flyer artwork; retain saved decorations, 32 display positions and collection rewards |
| See Ballard | Three identical stock windows per business | Ten business-specific work vignettes, partial closing blinds, scaled door emblems and compact entry details |
| Read the character | Uniform dark shell and uneven facial stubble | New Blender groom with different face/leg/body/tail lengths, dark roots and sparse silver guard tips, anchored motion and restrained directional sheen |

## Problems found by rendering, then fixed

A north background-city road tile began at z=60.5 and crossed the back of the below-street Den. It had no collider, so a collision-only test could not catch it. Its southern edge now starts at z=64, beyond the Den. The regression check inspects actual rendered scenery triangle footprints, and the same gameplay/overview cameras verify the missing band.

Custom touch graphics lacked an explicit CanvasRenderer requirement in editor captures. TouchGlyph now declares that dependency; the rerun renders movement, jump, stash, camera, Den and pause symbols instead of blank discs.

The first UI iteration still repeated Den guidance and left large empty panels. A second pass replaces those with content-sized cues and one Den status. A further outing-screen pass separates detailed collection recipes.

## Character budget and limits

The final groom has 52,277 geometric fibers / 83,523 triangles, down from the previous 99,642 fur triangles (about 16% fewer). The complete character has 129,719 triangles. This is a geometry reduction, not a measured frame-rate improvement. It uses one rooted motion spring and per-vertex tip bending rather than CPU simulation per hair, with no new transparent fur layers. Blender skin, bind skeleton and animation keys are preserved; FBX skin weights and all four clips are checked.

The in-game character still does not match the illustration's fur fidelity or facial appeal. This pass improves the groom and lighting while retaining the recognizable hunch and current gait. A future focused face/silhouette pass should be judged at the actual follow-camera distance, not only in a beauty render.

## Release checks

Unity checks pass for storefront clearance/identity, rendered Den scenery clearance, physical Den geometry, 227 Den experience assertions and 111 discovery-modal assertions. Eighteen phone/tablet UI pages, 22 fur inspection/motion frames, ten storefronts and six empty/populated Den views were captured and inspected. The Chromium browser flow passes 16 checks, including controls from settings, collections, scavenge, save/resume, physical return, banking, next outing and orientation safety, with no runtime browser errors.

The first browser return attempt failed after the test harness queued dependent touch events concurrently. The harness now sends them in order and uses the exposed movement basis for route input; the full physical route completes with full health. Browser review also caught clipped survival labels, which now have enough line height. Release settings replace redundant orientation copy with Controls and omit the development performance overlay. The Safari/WebKit flow also passes 9 checks with no browser errors. The supplied web-game skill client completed successfully and its actual gameplay screenshot was inspected. The native Mac build succeeds and passes strict code-signature verification. Physical iPhone/iPad performance and human preference/fun testing remain separate work.

## Next priorities toward 1.0

1. Test the first outing with unfamiliar players: can they find food, identify danger, return and bank without prompts from us? Observe where they hesitate before adding more UI.
2. Test Den motivation: do players understand which find earns the decoration they want, and choose to make another run? Show progress through actual collection rewards.
3. Continue the character's face and shoulder/hip silhouette refinement; check animation and coat together in motion under street and Den lighting.
4. Profile sustained play on the user's actual iPhone and iPad, including warm-device frame pacing, startup time, memory and battery use. Browser automation is not a substitute.
5. Tighten the scene's remaining material/lighting inconsistencies against the hero art. Preserve clear escape routes and control readability as detail increases.

Published 0.5.0: public Safari/WebKit verification passes all 9 checks with no browser errors, including touch jump, save/resume and orientation pause. Public gameplay/outing captures inspected. Pages build: 66d7aea5c78eb8d9f1fe92de0cc048085023ba0c. Play: https://kingmadellc.github.io/JimothySurvival/?v=0.5.0
