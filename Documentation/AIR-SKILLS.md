# Air skills and guided stair ascent

RaccoonMotor exposes RequestTrick() for touch UI, plus keyboardT/gamepadEast. Ground requests do nothing. One accepted press starts one .52second flip; a completed flip is only banked on a clean supported landing. Incomplete flips and damaging landings award no trick points. Clean consecutive trick landings within5seconds multiply up to4x; each full flip starts at150points. GameSession.ResolveLanding owns score persistence, health changes, reward UI and economy.

The flip rotates a dedicated body-centered pivot. The visual's local heading remains stable and the camera uses that heading independently, preventing somersault Euler-angle flips from swinging the camera around. The CharacterController and ballistic jump are not rotated or translated by the trick effect. Queued inputs clear on teleport, stopped Running state and focus loss.

Fall height tracks the maximum world root height during an unbroken descent. Wall and ceiling contact do not reset it. A landing needs controller support, an upward-facing static support beneath the feet, and actual downward movement slowed by collision. Damage starts above3.5m and grows14health per extra meter, capped100. A supported intermediate landing breaks the fall into separate descents. Existing void recovery remains separate.

Once the first access jump lands on an authored RoofRoutes tread, pushing toward the next nearby tread can invoke the existing checked mantle. It requires positive direction alignment, safe height, supported foot area and clear capsule rise/crossing. Initial street access still needs jumping. Normal jump input takes priority; no blanket increase to CharacterController stepOffset.

Public audit observables: CompletedTricks, LastLandingPoints, LastLandingClean, LastFallHeight, LastFallDamage, IsTricking. Editor/AirSkillsAudit.Run builds unsaved actual-controller fixtures for ground/no-op, complete and incomplete tricks, long falls, wall brushing, supported intermediate landing, first access and continuous stairs. Source syntax checks pass; physical results require the coordinated Unity run. No simulated playtest ratings or device-performance claims.

A clean rooftop transfer also awards100basepoints, optionally combined with flips. It requires both takeoff and landing at least4.5m high, over2m horizontal travel, and a sampled gap with no supporting ground within2.2m during flight. Routine street jumps, same-roof jumps with close ground, and mantles are excluded. AirSkillsAudit includes a ninth actual-controller roof-gap fixture.

## Physical verification

The actual fixture initially exposed two traversal problems, both fixed: a valid near-edge landing could sit.73m from a tread center and miss the original.65m arming radius; upper service platforms have open undersides, so a generic riser-wall ray could never find them. The authored assist now recognizes near-edge supported landings and probes the next real tread, then runs the unchanged capsule headroom, rise, crossing and foot-support checks. It does not invent an invisible ramp or remove colliders.

GuidedStairAudit passes all10actualworld roof routes with jump requests restricted to the first tread. Subsequent steps use directional movement and checked assistance. Its normal tap-jump and blocked-headroom controls also pass.

AirSkillsAudit now passes11fixtures. A9m fall deals76.02health; brushing a wall preserves that damage; an actual intermediate shelf separates the fall into3.93m and5m descents. A real rooftop gap awards100points without a flip. A completed normal-jump flip awards150, a late unfinished flip awards0, and a second stationary practice flip stays150. Combo multipliers require over2m from the previous scored landing and reset on teleport.

An independent visual-pivot/camera fixture samples300ticks across four headings during actual simulated flips: the visual visibly inverts while the camera rear alignment stays below-.99. This is geometric/physical camera verification, not a rendered character-animation quality judgment. Tests are in unsaved scenes without GameSession/save access.
