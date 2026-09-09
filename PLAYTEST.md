# Jimothy Survival — Mac and mobile-browser playtest 0.4.1

Open `/Volumes/Jimothy Dev/Projects/Jimothy/Builds/Jimothy.app`. Keep the Jimothy Dev drive connected while playing. Unity Hub is not needed to play the Mac app. The folder name remains Jimothy.app; the game's title screen now says **Jimothy Survival**.

The working Unity project is `/Volumes/Jimothy Dev/Projects/Jimothy/Unity`, using Unity 6000.3.0f1 ARM64. In the Editor, open `Assets/Jimothy/Scenes/Ballard.unity` and press Play.

Choose **Continue adventure** to retain your existing collection. With no save, the menu offers **New adventure** and disables Load adventure. Starting a new adventure with an existing save asks before replacing it. Do not start fresh merely to see the new art.

The legacy application product name is intentionally retained so the new title does not move your local save path. Older-map migration moves Jimothy safely to the new den and resets obsolete pickup positions/cooldowns while preserving bag, pantry, currency, trophies and decor. Save data is local with a backup; cloud account sync is not implemented.

## Controls

| Action | Mac | Touch / stick |
| --- | --- | --- |
| Move forward / backward | W / S | Push thumb pad up / down |
| Steer Jimothy | A / D | Push thumb pad left / right |
| Walk slowly | Hold left Shift while moving | Partially deflect movement stick/pad |
| Turn Jimothy and rear camera / adjust pitch | Hold right mouse button and drag | Drag the right side |
| Jump | Space (tap; no need to hold) | Jump |
| Search nearby find/keepsake | F | Search |
| Eat food | E / Eat button | Eat |
| Return home while safe | H / Home button | Home |
| Pause | Escape | II |
| Bank finds / buy decor | Den button while home | Den |

The camera stays behind Jimothy. Steering and right-drag turn him and the view together; obstacles raise or retract the camera without swinging it sideways.

Full movement is 3 m/s; deliberate walking is slower and reduces enemy detection by 30%. Dogs can outrun full movement, so watch their warning and pursuit cues. Animation playback follows actual travel speed; slow walking has its own gait.

## First night

1. Search the snack at your feet at the Trash Palace.
2. Follow the direction/distance to an amber-edged rooftop route. Jump toward marked landings. Paw marks indicate usable tread centers. Ten routes lead up; four bridges connect neighboring roofs.
3. Search a rooftop keepsake. Try the northern lookout for a special find.
4. Return home, open Den, and unload your pockets. First-night rewards fund a cushion.

Rooftop progress requires a supported arrival. If you fall before finding a keepsake, recovery guidance points back toward the amber climb. The Home action returns you while safe.

Collectibles are distinct 3D food, trinket and trophy models beside restaurant props or on rooftops; searching reveals the actual object, its name and rarity. The first map contains 22 authored search locations; the catalog contains 256 definitions, but this map does not expose all of them yet.

## What to inspect in this pass

The title has custom licensed typography and a revised layout. Jimothy uses a smaller 15-bone character with separately controlled paws, a Walk gait, and moving fur cards. Compare his scale with people, dogs and cats. Look for foot sliding, tilted soles, coat overexposure, camera obstruction, unreadable landing edges, and missed control inputs. Record where an issue occurs and what you were doing; no rating or improvement claim is assumed from these changes.

The earlier overbright white coat has been recalibrated. A native-only color-stripping issue was then found and corrected; both neutral Editor and replacement native den captures now read dark charcoal/silver and have been visually checked. The human/dog assets and lineup have also been checked for relative scale. This is capture-based verification, not a performance claim or user approval.

Ten fictional shop identities and varied facades now lead toward an exposed-brick left bend and a bell park. The brick surface is a requested heritage interpretation: official Seattle sources describe historic brick beneath asphalt, not a fully exposed-brick present-day avenue. The park is a playable interpretation of Marvin’s Garden rather than an exact reconstruction.

Current checks pass: 9 EditMode tests, 14 physical traversal routes, first-night integration, and a district audit with 183 road samples and four walks including the bell passage both ways. The 0.3.0 Mac build succeeded; isolated native checks passed Search, Jump, Home, banking, and title navigation. The final native camera/sign regression passes (AgentPlaytests/v5-camera-verified/report.md): the repeated tree-side jump sequence keeps the camera clear and Jimothy visible. A compact mobile HUD capture found guidance crossing the Fullness bar; the final Build4 native capture verifies this is corrected, with readable HUD and title. Evidence and limits are tracked in `Documentation/POLISH-V5.md`.

## Current limits

This is a playable prototype, not a finished App Store release. The title illustration is an art target; it does not establish equivalent runtime fidelity. Some NPCs, props and scenery remain procedural/stylized, and the audio is a minimal procedural soundscape. Capture review has verified the revised fur color and character scale; further lighting/device conditions remain to be tested.

Fur uses cards with damped spring-driven GPU tip movement, not full individual-strand physics or per-strand collisions. Its performance has not been established on an iPhone.

Mac builds, Editor checks and screenshots using a mobile layout do not establish iPhone frame rate, thermal behavior, battery use or touch comfort. Actual iOS deployment requires iOS Build Support, full Xcode, signing and a physical device. No physical iPhone validation is claimed for this pass.

## This build

Shorter legs, a tapered rear silhouette, visible layered fur and a closer rear camera; phase-driven jump/landing poses and denser run animation. Four varied roof crossings include festoon lights, laundry, ductwork and cedar. All 22 exposed finds have distinct models, and banked trophies retain their appearance at home. See Documentation/REFINEMENT-V6.md for implementation and validation limits.

## iPhone and iPad Safari test

On the same Wi-Fi as the Mac, open http://192.168.4.23:8765/ in Safari, turn landscape and tap Play in browser. Tap New adventure on first use, Continue thereafter. Left pad moves/steers; right-side drag turns view; touch buttons provide Jump, Search, Eat, Home, Den, Pause and Center view. Browser progress is per device and URL, separate from the Mac save. Keep the Mac awake and the external drive attached.

0.4.1 lowers the camera and aims ahead, adds a solid camera obstruction to the den beam, replaces fur cards with dense directional geometric fibers, and reauthors Waddle from the observed original Seattle Times running footage. See GAIT-STUDY-V7.md, CHARACTER-V7.md and MOBILE-WEB.md.
