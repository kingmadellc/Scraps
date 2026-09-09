# Closing Time revision — September 7, 2026

The v0.01 playtest identified three concrete failures: the city looked sparse and generic, rooftop access was unreliable/unreadable, and road pickups substituted quantity for exploration. The revision changes the first map into a closing-time scavenging loop.

## Implemented priorities

| Opportunity | Implemented change | Intended player benefit |
| --- | --- | --- |
| Reach the fun quickly | Welcome snack and three-stage first-night goal with direction/distance | An obvious first action and reason to explore |
| Make jumps predictable | Full tap jump, coyote time, input buffer, substeps, checked ledge assist | Fewer failed jumps caused by input timing |
| Make vertical routes legible | Ten amber-edged crate/AC/service routes, paw marks and four roof bridges | Visible entry points and exploration after climbing |
| Make finds meaningful | 22 authored restaurant/roof locations; search interaction; distinct bags, bagels and keepsakes | Curiosity and discovery instead of green road dots |
| Close the reward loop | Banked finds pay currency; first-night keepsake reward funds decor | Exploration produces a visible home-base improvement |
| Reduce early frustration | Warning before pursuit, safe home, full pause, slower hunger, fair fast travel | Time to learn movement and recover |
| Establish place and mood | Closing-time sky, linear lighting, warm storefronts/lamps, textured brick, cornices, awnings, planters, patio furniture, rail seams and wet patches | A coherent Ballard-inspired block |
| Add feedback | Discovery/banking chimes, grounded pawsteps, quiet harbor ambience | Actions and locations feel responsive |
| Improve phone usability | Separate desktop/touch HUD, readable objectives, rounded controls, thumb feedback, safe area | Less clutter and clearer controls |

The task used three AI development specialists for traversal, gameplay/economy and mobile/QA, with integration and world art reviewed in the main task. These are implemented design hypotheses, not evidence of improved ratings from real players.

## Verification

- Nine EditMode tests pass, including airborne landing guidance and roof-footprint regressions: catalog uniqueness/counts, save round trips, offline progress limits, backwards clock handling, invalid state and old-map migration.
- Actual CharacterController movement at 60 Hz passes all ten entry routes and all four bridges, requiring supported landings. One-frame jump apex: approximately 1.856 m. Low-ceiling fixture blocks mantle and reports no penetration.
- Actual playmode first-night integration passes 22 assertions, including preserving food when already full: reachable welcome search, objective transitions, rooftop search, fast travel, deposit, decor purchase, pause/safe-home survival, resumed clock and unchanged main/backup/temp save files. Teleports isolate interactions; the separate movement audit tests travel.
- Actual Unity camera output was captured and inspected repeatedly to fix reversed/oversized signs, overbright panes, camera obstruction and clipped UI. `PlaytestCaptures` contains frames and machine-readable reports.
- The new Blender district was exported from native world geometry, saved with packed maps and reopened successfully. 136 static mesh batches, 159,392 vertices and 10 packed base/normal/roughness maps. TextMesh lettering, dynamic actors, colliders and Unity postprocessing are not baked into that Blender scene. Re-run export/import helpers after art changes.

## Mobile cost controls

Shared materials and spatial mesh batches replace thousands of decorative objects. NPC clothing and each independent pickup combine by material. Tiny collectibles cast no shadows and stop drawing beyond 30 m. Local lights disable beyond 22 m; additional lights have no shadows. Main shadows are bounded to 35 m/two cascades. URP uses SRP batching, 2x MSAA, 90% render scale and four additional lights per object. SSAO uses reduced sampling and is disabled by the default mobile 30 fps profile. Imported iPhone textures use ASTC 6x6. Runtime-generated textures are small but do not use that importer compression. Temporary meshes/audio/volume resources are cleaned up with the world.

PBR source textures are CC0 Poly Haven assets; provenance and verified source hashes are included in `Unity/Assets/Jimothy/Resources/Surfaces/provenance.md`.

These choices reduce identifiable costs; they do not prove a phone performance target. Editor draw counters include editor/multiple rendering passes and must not be presented as a phone benchmark. Profile a signed build on an older supported iPhone and a recent iPhone for at least 20 minutes before choosing final quality defaults. Measure CPU/GPU frame time, 95th-percentile stalls, memory, thermal state and battery drain.

## Next playtest protocol

Use 8–12 representative players initially, including touch-first casual players and experienced platformer players. Let each play unaided for 10–15 minutes. Record actual results; do not assign simulated satisfaction scores.

| Question | Measure | Initial target hypothesis |
| --- | --- | --- |
| Is the first action obvious? | Time to first successful search; help requests | Most find the snack within 30 seconds |
| Is verticality usable? | Time to first roof; failed attempts and camera collisions | Most reach a roof within 3 minutes without help |
| Does the loop pay off? | First-night completion and voluntary extra exploration | Most complete it within 8 minutes |
| Is it fun? | 1–7 enjoyment, frustration and desire-to-replay ratings plus comments | Improve from the measured v0.01 baseline |
| Is the map convincing? | Place recognition, lighting/readability and art-quality ratings | Players recognize the intended district/mood |
| Does touch work? | Missed taps, accidental actions, camera corrections | Diagnose per-device rather than average away failures |

Prioritize observed blockers first, then strengthen successful moments. No App Store retention, enjoyment improvement or playtest score has been measured yet. The approved seaplane render remains a visual target; this build does not yet match its asset fidelity. Next art production should focus on a small hero block with bespoke facade meshes, convincing shop interiors, organic foliage, authored lighting and higher-quality character materials before expanding map size.
