# Jimothy Survival — v5 polish / 0.3.0

September 7, 2026. This record distinguishes implemented work, observed evidence, and checks still pending. It does not assign an improvement multiplier, enjoyment score, or realism percentage.

## User direction and scope

This pass responds to requests for a distinctive premium title, more convincing raccoon proportions and paw motion, slower deliberate walking, fur with visible movement, a better sense of scale beside people and animals, and richer Ballard surroundings. The requested vintage Super Cub direction informed the title's slant and emblem treatment; the game does not incorporate Honda artwork or branding. Existing progress must remain usable.

Version is 0.3.0. The displayed title is **Jimothy Survival**, while the legacy application product name remains in place to preserve the existing local save path. Do not rename the product identity merely to match the new wordmark.

## Implemented

| Area | Change | Evidence / limit |
| --- | --- | --- |
| Title | Original cream/copper slanted Jimothy wordmark, spaced SURVIVAL, Fugaz One + Barlow Condensed fonts, feathered art scrim, aspect-fill hero, compact menu and save-aware Continue action | `PlaytestCaptures/mobile-menu.png` captured and visually inspected by parent and documentation agent: title, subtitle, menu and raccoon remain readable. This is a mobile-layout capture, not an iPhone test. |
| Character | `Art/Blender/Jimothy-v5.blend`, 15-bone rig including four paw bones; updated Idle, Walk, Waddle and Jump actions | `Documentation/GAIT-STUDY.md` records source observations, all-frame paw/weight/contact checks, and inspected poses. Runtime animation blending is a separate check. |
| Geometry / fur | 46,196 base triangles plus 7,781 fur cards / 46,686 fur triangles | `Documentation/character-v5.json`. Combined base+fur is 92,882 triangles; the old base was approximately 75,000. Lower base cost does not mean lower total geometry cost. |
| Scale | Model scale 0.22; Jimothy approximately 0.485 m tall, compared with human ~1.8 m, dog ~0.78 m, cat ~0.4 m | Human and dog assets integrated; `PlaytestCaptures/animal-scale-lineup.png` inspected by parent and documentation agent and shows the intended relative scale. These are design sizes, not biological measurements or a performance result. |
| Movement | 3 m/s full movement; left Shift or a partial thumbstick permits slow walking; playback follows actual horizontal velocity | Helps gait match travel. Walking reduces enemy detection by 30%; dogs move at 3.4 m/s and use a 0.8 m attack range. These are tuning values, not a measured balance improvement. |
| Environment | Ten fictional shop identities and facade variations; dark feathered wet patches, smaller den plaque, curved fern leaflets and revised Rainier; exposed-brick avenue with a left bend and walk-through bell park | Shop details and font licenses are recorded in `Documentation/SHOP-IDENTITIES.md`. Physical district audit passes after correcting patio overlap and curb-drop access. Native onboarding smoke checks passed; final camera regression passes. |

## Fur: implementation and visual verification

Fur is card geometry with damped spring motion driving GPU tip displacement. It is **not** a full individual-strand simulation: no claim of per-strand collision, self-collision, grooming dynamics, or a physically complete fur solver. It has a real rendering/geometry cost that needs on-device profiling.

The user identified excessive white/overlit fur in an earlier rendered result. After calibration, `PlaytestCaptures/jimothy-color-neutral.png` was inspected by parent and documentation agent: the coat reads charcoal with silver detail rather than the earlier white overexposure. `JimothyFur.shader` includes `AlphaToMask On`. The first native candidate still showed a pale coat; after preserving mesh vertex colors in the replacement build, native den captures in `AgentPlaytests/v5-onboarding-retest/frame-0.png` and `AgentPlaytests/v5-ui-final/frame-0.png` show the dark coat. Fine fur remains stippled at this screen scale. This is a visual correction verified in the captured lighting, not user approval, a guarantee under every lighting condition, or a device-performance result.

## Verification status

- Title composition: observed in the native capture noted above; passes visual readability inspection.
- Gait source research and Blender paw checks: documented in `GAIT-STUDY.md`; distinguish frame-sequence observation from continuous video viewing and distinguish baked pose checks from final native animation.
- Earlier first-night loop: completed through actual compiled play in `AgentPlaytests/onboarding/report.md`, including a rooftop keepsake, banking, and cushion purchase. Found issues were reported rather than scored.
- Earlier targeted guidance retest: `AgentPlaytests/onboarding-retest/report.md` confirms in-place jumps do not advance landings, roof progress waits for supported arrival, and a fall restores climb guidance. These are prior build results, not a rerun of the entire v5 candidate.
- Current v5 EditMode checks: `PlaytestCaptures/v5-editmode.xml` reports 9 passed, 0 failed.
- Current v5 district traversal: `Unity/Logs/traversal-audit.json` reports all 14 physical routes passed, tap-jump rise ~1.856 m, and blocked-overhead regression passed with no penetration. This uses actual CharacterController motion at 60 Hz.
- Current v5 first-night integration: `PlaytestCaptures/first-night-audit.json` reports PASS across 22 assertions, including banking/decor, pause behavior and unchanged user save files. Interaction fixtures use teleports; traversal is covered separately.
- District geometry: `Unity/Logs/district-geometry-audit.json` reports PASS: 183 road-support samples and four physical walks (spawn to bend endpoint, return, avenue to garden through bell, reverse bell passage). No penetration was reported. Parent corrected the patio/sidewalk overlap and curb-drop access before this pass. Walks use no jumps; teleports only reset independent starts.
- Fur color and character scale: actual captures noted above visually inspected; human/dog assets are integrated.
- 0.3.0 Mac build succeeded and launched in isolated native processes. `AgentPlaytests/v5-onboarding-retest/report.md` records pixel-click Search, Jump, Home, Den, banking, and menu navigation passes. The whole roof/first-night loop was not repeated in this bounded smoke test. The final native camera/sign regression passes (AgentPlaytests/v5-camera-verified/report.md): the repeated tree-side jump sequence keeps the camera clear and Jimothy visible.
- `AgentPlaytests/v5-ui-final/report.md` found guidance crossing the Fullness bar despite zero button-overlap pairs. Build4 moved guidance to a separate column: `AgentPlaytests/v5-ui-verified/frame-0.png` visually confirms the bar is clear and labels unclipped; frame 2 confirms title readability. Pixel pause/menu navigation passed and process exited 0.
- No physical iPhone deployment or sustained CPU/GPU, memory, thermal, battery, touch-comfort, or frame-rate result is established here.

## Sources and licenses

Title references and full font provenance: `Art/TitleDesign/README.md`. Fugaz One and Barlow Condensed are unmodified SIL OFL 1.1 fonts from Google Fonts; the original license texts are retained beside the font binaries in `Unity/Assets/Jimothy/Resources/TitleFonts` and included with resources. No referenced game or Honda artwork was imported.

Gait sources, viewing limitations, and non-bundling decisions: `Documentation/GAIT-STUDY.md`. The ground footage informed shape/contact observations; the faster bound remains an authored platformer adaptation, not motion capture.

District heritage references: Seattle's [Ballard Avenue Landmark District](https://www.cityofseattle.org/neighborhoods/historic-preservation/historic-districts/ballard-avenue-landmark-district) and [existing-conditions report](https://www.seattle.gov/documents/departments/seattleplanningcommission/minutesandagendas/ballardexistingconditionsreport.pdf) describe historic brick paving beneath the street's asphalt. The exposed-brick road is the user's requested heritage interpretation, not a claim that today's street is fully exposed brick. Seattle's [Marvin's Garden page](https://www.cityofseattle.org/parks/parks/marvins-garden) identifies the Centennial Bell Tower and former city-hall bell. The game park/road geometry is an authored playable interpretation, not a surveyed reconstruction. New shop marks are original fictional identities; Pacifico and Bree Serif join the existing title fonts with their OFL licenses retained.

Environment PBR maps: original CC0 Poly Haven brick, asphalt and wood maps. URLs, authors and verified hashes are retained in `Unity/Assets/Jimothy/Resources/Surfaces/provenance.md`. The existing title hero is `MenuHero-v2`; it is separate from runtime character fidelity.

## Play the Mac build without replacing progress

See `PLAYTEST.md`. Open `Builds/Jimothy.app` from the Jimothy Dev drive and choose **Continue adventure** when an existing save is present. **New adventure** replaces progress after confirmation; do not use it simply to inspect the new model/title. Preserve the legacy product name and local save location. Agent playtests use saving suppression and are not a substitute for validating normal save/load behavior.

## Final handover

Build4 retains version 0.3.0. Native vertex-color stripping was disabled after the first compiled test exposed pale fur absent from Editor captures. The next native build verified charcoal coloring. Camera probes were reduced from .24 m to .10 m to fit within the small player’s .17 m clearance; elevated fallback angles and safe-orbit snapping avoid an obstructed interpolated view. A route sign was moved off the character silhouette. The exact native failure sequence now passes in AgentPlaytests/v5-camera-verified. These targeted results do not prove all camera positions are clear. Mobile guidance overlap was separately fixed and verified in v5-ui-verified.

Final Mac signature verification passed. The Unity-exported environment was repacked into Old-Ballard-Closing-Time.blend. All automated/agent gameplay sessions suppressed user-save writes. These are developer-assisted AI tests on Mac, not independent human enjoyment ratings or iPhone profiling.
