# Neighbor navigation repair — September 8, 2026

Implemented in `NeighborNavigation.cs`, `NeighborAI.cs`, and `NeighborWorldObstacles.cs`.

## Findings and changes

The previous shared human-sized navigation capsule did not represent the long dog or scaled cat/gull bodies. Initial overlaps were retained, start-to-grid connectors were omitted, and moving circular patrol goals could pass through solid props. All neighbors now use production species collider dimensions and conservative navigation body envelopes. Initialization repairs embedded spawns before establishing their patrol home. Paths validate both connectors, vertical support, body clearance, and every movement segment. Invalid roof/building targets produce a stationary neighbor. Patrol destinations remain fixed until arrival or replanning; animation still follows actual movement.

The first actual-district run found two curb-descent stalls. A body clearance box behind a descending controller intersected the sidewalk's low top. Walkable obstacles within the controller step height are now allowed while the actual CharacterController resolves the step. Tall obstacles remain blocking.

A targeted café crossing exposed a valid narrow passage with no point on the original 0.75 m grid. A 0.375 m grid provides samples inside that passage without reducing the dog envelope. A clear exact destination is appended to the final grid connector so a valid destination is not silently shortened to a nearby cell.

126 individual colliders now match visible café table tops/pedestals, chair seats/backs/legs, street lamps/feet, route signs/posts, and den posts. The helper leaves the real under-table gaps open. Crates, dumpster bodies, and planters already have colliders. GameSession initializes the helper before loot placement and neighbor spawning.

## Verification

- `NeighborWorldNavigationAudit.Run`: **13/13 passed** against actual district geometry. All nine production species/spawn configurations, one intentionally embedded dog, and dog/cat/human café crossings. Every case reached two destinations and rejected an interior-building target. **Zero measured body-envelope penetration** above permitted step-height geometry. Exactly **126 new prop colliders** active. Report: `Logs/neighbor-world-navigation-audit.json`.
- `NeighborNavigationAudit.Run`: **5/5 passed** using actual CharacterController movement around a wall, through a 1.75 m corridor, staying outside an enclosed target, and ascending/descending a 0.20 m curb. Report: `PlaytestCaptures/neighbor-navigation-audit.json`.
- `RoofSurfaceAudit.Run`: passed with prop helper active; 20 exposed membrane/coping checks.
- `RooftopCrossingAudit.Run`: all four crossings in both directions passed with prop helper active.
- `ScavengePlacementAudit.Run`: **22 stable nodes / 63 supported, clear, searchable sites**, with prop helper active.
- `FirstNightAudit.Run`: passed through production GameSession initialization with new colliders active and saving suppressed.
- `RoofMotionAudit.Run`: eight actual rendered frames completed with new colliders active; contact sheet visually reviewed. Roof/coping surfaces remained coherent across the sampled camera offsets. This is not a frame-rate or exhaustive flicker measurement.
- `RearCameraAudit.Run`: passed, **355 samples / zero failures**. Scope correction: this isolated audit creates no neighbors and did not explicitly initialize the prop helper, so these camera results do **not** cover newly added prop colliders. FirstNight and RoofMotion did use production initialization.

Logs for the latter checks are `Logs/<AuditName>-nav-regression.log`. All Unity processes exited before releasing the editor slot to the parent for builds.

## Limits

The district navigation audit uses production collider configuration and conservative body boxes, not every animated mesh vertex. It advances CharacterController movement at 60 Hz and verifies routes/clearance; it does not constitute a human gameplay test or an exhaustive NPC crowd simulation. Neighbor meshes, player movement, camera behavior, and save format were not changed by this navigation repair. Spawn correction occurs only during initialization; movement does not teleport through obstacles. Fine-grid clearance is cached per body profile; no performance benchmark was run for this change.
