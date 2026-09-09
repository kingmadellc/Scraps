# Jimothy gait study and v5 clip refinement

## Direct observation and limits

[ABC News: Jimothy wanders through Seattle backyard](https://abcnews.com/video/135166641/), credited on screen to Anna Miner via Storyful, was accessed through the public player manifest exposed by that page. Inspected decoded sequential frames covering 0–24 seconds at 2fps, then 4–7 seconds at 8fps and 14–18 seconds at 6fps. This is frame-sequence inspection, not continuous real-time playback. The public stream emitted several NAL warnings during retrieval; the inspected frames decoded visibly. Media remained temporary research material, not a game asset.

Observed: a large rounded torso over thin exposed lower limbs; head/forequarters initiate the turn around 4 seconds, with forepaws changing contact separately. The torso follows, and the trailing hind foot extends behind before being brought underneath. The rear-view turn around 14–18 seconds shows separate rear placements and modest body sway; the tail stays close to the rump. Grass, the torso and camera cropping hide some contacts. The footage does not establish an exact four-foot phase chart or a clean full-speed running cycle. No claim of measured gait frequency, stride distance or biological joint angles.

[Commons climbing clip](https://commons.wikimedia.org/wiki/File:Jimothy_climbing.webm) was inspected as 4fps sequential frames, with a closer 8fps study of its first 3 seconds. The animal moves up the trunk; foliage and trunk hide much limb detail. It is not a ground-gait reference. The page lists CC BY-SA 4.0 but also pending permission/license review; no source footage or derived images are bundled.

[Seattle Times original YouTube link](https://www.youtube.com/watch?v=RqlQYJUz-j8) could not be watched: web fetch was throttled, and this agent's CUA browser request returned no available browser. No observations are attributed to that video.

## Implemented adaptation

`Tools/refine_gait_v5.py` exposes `refine(rig)` and changes only actions in the already-open scene. It never saves or exports. The optional paw setup adds four deform bones to the original 11, for 15 bones total; no helper objects or constraints. Existing action references are remapped when replacing Idle, Walk, Waddle and Jump.

- Walk uses staggered individual contacts, a small torso roll and a smooth lifted recovery.
- Waddle remains the legacy run name: staggered forepair followed by staggered hindpair. This faster bound and its timings are a platformer adaptation, not motion capture from the clips.
- Wrist targets travel at constant velocity during stance. Analytic two-link IK preserves limb lengths; the rest elbow's perpendicular offset fixes the bend plane to avoid knee flipping. Linear baked keys avoid spline contact overshoot.
- Idle compensates small breathing motion at the wrists. Jump gathers limbs while the game motor supplies ballistic translation.

`Tools/add_paw_rig_v5.py` now exposes `add_paws(rig, skin, meshes=None)`. Call after rest-shape edits/mesh reduction and before fur generation. It adds four connected `_paw` bones at the shin endpoints, directed 0.18 units forward and 0.04 down. Existing low shin weights transfer smoothly: full paw influence below wrist height +0.005, fading out by wrist height +0.095. Other bound meshes, including claws, are discovered automatically; callers can supply an explicit mesh list. Total weights are preserved. The gait solver holds each paw in its authored rest orientation independently of shin rotation, eliminating the previous tilted-sole limitation. This is flat-ground baked contact, not terrain-adaptive runtime IK.

## Integration and validation

At the current 24fps scene and model scale 0.22:

| Clip | Frame range | Period | Nominal world speed |
|---|---|---|---|
| Walk | 1–17 | 0.667s | 0.7425 m/s |
| Waddle | 1–9 | 0.333s | 2.145 m/s |
| Idle | 1–61 | 2.5s | zero |
| Jump | 1–31 | 1.25s | motor driven |

For a 3m/s motor use Waddle playback approximately 1.3986; for 0.96m/s slow movement use Walk playback approximately 1.293. Blend/playback must follow actual velocity: fixed playback at arbitrary input speed still slides. Nominal speed is stored on each action and in the returned report; changed FPS or model scale requires recomputation. Ground targets are centered below current rest hips; torso is lowered 0.29/0.32 armature units for attainable long strides.

Executed in separate Blender 4.5 background processes against original Jimothy.blend and compact Jimothy-v5.blend without saving either. Original rig stride test stayed below 0.000005 armature units wrist error. Final compact-rig test: Walk <0.00000042, Waddle <0.00000051, Jump <0.000505. Rendered and inspected compact Walk frame5; corrected unstable elbow pole and rerendered successfully. These validate bake execution/reach and one visible pose, not the final game's complete animation blending. No FBX or .blend was written by this agent.

Paw refinement was separately validated over every integer frame of all four clips on compact Jimothy-v5.blend. Four paw bones added; a second call added none and changed no weights. Weight sum error <0.00000003. Each foot has over 2,000 fully transferred mesh vertices. All clips had zero measured paw/shin joint gap and zero measured paw orientation error; rigid paw vertex deviation <0.00000033 units. Minimum evaluated fully weighted paw vertex height stayed above 0.0252 units, so no floor penetration was observed in that checked geometry. Inspected Walk renders at frames 1, 5, 9, and 13: toes retain ground-facing orientation as the legs alternate reach/recovery. Fur generation after transfer inherits the ankle blend naturally. Runtime import/blending remains parent verification.
