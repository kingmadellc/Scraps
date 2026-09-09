# V9 silhouette and compact gait

Source: finalized Art/Blender/Jimothy-v8.blend, kept untouched. V9 coherently warps skin, groom and bind skeleton: tapered narrower rump, modestly lowered rear contour, narrower stance at the paws, and a forward/marginally raised head with more visible ears. Eyes are modestly enlarged and moved forward. Mask-region geometric hairs were removed because they obscured the face with dark dots; the existing fine baked undercoat supplies that region. Long shoulder/flank fur and runtime tip-flow shader remain.

The original footage contact sheet referenced by GAIT-STUDY-V7.md was inspected again. This is an authored game adaptation, not motion capture. The distinctive hunch, long legs and asymmetrical fore catch/hind recovery remain. Footage cannot establish precise biological speed or left/right duty factors.

A concrete spider-motion source was the old analytic solver's nearly straight rest-pole vectors, which could bend knees/elbows laterally. The V9 solver bends in the sagittal plane: fore elbows backward, hind knees forward. Recovery side offset is drastically reduced. Fore lift decreases .28→.18 model units, hind .49→.29; fore duty .28→.22 shortens reach while preserving the analytic stance speed. Waddle remains13 frames at24fps, nominal2.145world m/s at.22 scale. Runtime speed matching remains the motor's responsibility.

Jump still uses61 frames and the existing motor phase contract: launch, apex tuck, descent extension, contact cushion and recovery. Apex hind lift and landing compression are reduced. Jump has no root-flight translation: the motor supplies the actual ballistic arc. Blender pose-only jump images must not be presented as a physical in-game jump test.

## Validation and cost

- Blender audit passes:15 bones, identical loop endpoints, maximum joint separation<1e-6 model units, stance-height interpolation variation<.0006 model units, landing-to-Idle matrix error<1e-8.
- Generation checks: no reach clamping in run or jump; all sampled jump vertices finite.
- Independent staged FBX roundtrip passes: two skinned meshes; vertex weights, UV/color layers; Idle/Jump1–61, Walk1–17, Waddle1–13. Linear vertex-color export and.125frame animation baking retained.
- Actual FBX totals:140,706 triangles /253,530 vertices. Fur94,510 triangles /230,242 vertices. PreviousV8 totals152,146 /280,642. This remains a substantial skinned mesh; no physical-device performance claim.
- Resources/Jimothy.fbx replaced only after both gait and stagedFBX audits passed. Previous runtime archived Art/Exports/Jimothy-v8-runtime.fbx.

## Visual evidence and limits

Studio front/profile/rear: Art/Renders/jimothy-v9-{front,side,rear}.png. Actual13-frame run render sequence: Art/Renders/v9-run. Run preview Art/Renders/jimothy-v9-run.mp4 loops12 unique frames at34fps (approximate existing3m/s playback cadence, not measured biological timing). Six jump poses in Art/Renders/v9-jump show the authored tuck and landing cushion without physical root movement.

Native/Unity import, actual running speed, jump-phase timing, collision-grounding and gameplay-camera appeal still require the parent integration audit. No Unity launch or gameplay C# changes by this agent.

## Staged face follow-up

The first V9 face review found protruding eyes and an overly bare mask. The follow-up reduces the eyes to1.08× original and recesses them instead of pushing them forward. Sparse very short facial guards now use the underlying coat color rather than dark contrasting dots; fine procedural normal detail is restored in Blender. Current staged mesh costs145,838triangles/261,718vertices. Updated independent gait and FBX roundtrip checks pass. Staged FBX is Art/Exports/Jimothy-v9.fbx; integration must wait for the coordinated Assets freeze to lift. Runtime facial normal detail continues to use the existing undercoat normal map, so the Blender procedural bump alone is not proof of native appearance.

The coordinated Assets freeze subsequently lifted; the recessed-eye/fine-face follow-up passed both audits and replaced Resources/Jimothy.fbx before the next Unity integration run. Latest authoritative geometry totals are145,838triangles/261,718vertices, overriding the earlier first-pass counts above.
