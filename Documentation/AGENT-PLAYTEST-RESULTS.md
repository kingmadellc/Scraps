# Agent playtest results — Closing Time v0.02

The compiled Mac game was tested by two fresh independent agents (onboarding and rooftop exploration) and a developer-assisted mobile UI reviewer. Testers used real game frames and camera-relative movement in isolated unsaved sessions. They did not teleport. These tests observe functionality and friction; they are not human enjoyment ratings or iPhone benchmarks.

## Initial playthroughs

- **Onboarding:** completed the first-night food → roof → keepsake → den loop and purchased a visible cushion. Found airborne guidance advancing before landing and premature rooftop completion, followed by weak recovery guidance after a fall.
- **Rooftops:** reached a roof, found a keepsake, crossed a bridge, deliberately missed a jump, recovered on foot and banked 36 shinies. Found foliage/trees hiding the character during recovery.
- **Mobile UI:** tested the simulated landscape touch layout, pause and survival behavior. Found toast overlap in the den, desktop keyboard wording, low guidance contrast and food consumed while already full. Button rectangles did not overlap.

## Changes made from findings

| Finding | Resolution |
| --- | --- |
| Jumping in place advances landing guidance | Guidance advances on supported landings and resets after falling to a lower tread |
| Roof achievement fires while airborne near roof | Requires grounded position within a roof footprint |
| Failed climb leaves the player without climbing directions | Guidance returns to a reachable amber route after falling |
| Foliage hides Jimothy / camera enters trunk | Sightline foliage fade and trunk collision |
| Den toast covers buttons | Toast moved to a reserved top band |
| Mobile HUD contains keyboard F hint | Touch wording uses Search |
| Objective text gets lost against scenery | Dark backing behind objective and guidance |
| Eating at full health/fullness wastes a snack | Food preserved with feedback |
| Decor ownership unclear | Owned label and disabled purchase button |
| Den counters appear stale in direct test action | Counters now update live; original report used a bridge action bypassing the normal UI refresh, so it did not establish a normal-click bug |

## Verification

The revised build passes nine EditMode tests, actual CharacterController traversal of ten entry routes and four bridges, and 22 playmode first-night/save assertions. The code-signature verification passes. Real rendered screenshots were inspected after the changes. Separate regression reports record the agent follow-up outcomes.

Evidence folders are under `AgentPlaytests`: `onboarding`, `rooftops`, `mobile`, and their `-retest` counterparts. Each contains observed screenshots and command results. The first pass used public action calls for menus/search/deposit; the follow-up mobile test additionally uses uGUI raycast/pointer clicks. Neither method represents physical finger input.

## Regression outcomes

- **Onboarding, 17 commands:** passed. In-place jumping retains landing 1; airborne upper-stair movement does not complete the rooftop objective; supported roof arrival does. Deliberate fall restores amber-route recovery guidance.
- **Mobile UI, 14 commands:** passed. Actual uGUI clicks verify Search, Eat, Den, Unload, Jump, Pause, Settings and Back. Fullness preserves the snack, den counters refresh and toast labels remain clear. All recorded button-overlap counts are zero.
- **Rooftops:** no missing/pink shader; however, trunk collision exposed a camera-closeup regression at the fall location. This was fixed with a collision-checked alternate camera angle, preserving movement alignment. A subsequent rendered playmode regression at the reported location measures 6.08 m camera clearance (minimum 2.4 m) and shows Jimothy and the surroundings. Evidence: `PlaytestCaptures/tree-camera-regression.png` and `camera-regression.json`. This final camera fix was verified by the parent, after the agent report, rather than claiming that report passed.

## Remaining validation

Representative human players must establish whether the changes improve enjoyment, clarity and replay interest. A signed iPhone build needs device profiling and touch playtests. The city remains a stylized prototype below the approved concept-render fidelity. See CLOSING-TIME-REVIEW.md for the next human playtest measures and art priorities.

## Build 0.4.0 refinement pass — September 8

Native isolated v7-motion: forward travel, steering without strafing, rear view, tap jump, grounded recovery and pause passed. Fur stayed colored with no white/pink regression. Fine paw/fur contrast remains limited at gameplay distance; sparse frames do not constitute detailed animation approval.

Native isolated v7-finds: search, objective progression, movement with pockets, den deposit and unchanged user saves passed. Dynamic discovery portrait required a capture harness fix; see final reveal retest. These are assisted Mac development tests, not human fun scores or iPhone performance evidence.

Final v7-reveal retest passed in the rebuilt native app: actual bagel portrait, rarity, full name and pocket text are readable; forward movement continues while the card remains visible, with inventory retained. Two existing save files remained byte-identical and no save files were added. All isolated test processes exited.
