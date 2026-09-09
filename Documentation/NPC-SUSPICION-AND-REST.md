# NPC suspicion and movement revision

Logic and district navigation audits passed in the coordinated Unity run. The first supported-rest render exposed a jacket-weight defect; repaired asset is staged for runtime visual re-audit.

Hostile neighbors expose `Suspicion01`, `HasLineOfSight`, `IsThreat`, `DetectionLabel`, and `LastSeenPosition`. Detection accumulates over sustained visible exposure, rises faster nearby and while Jimothy moves quickly, and decays continuously behind static geometry or outside detection range. Chase starts at full suspicion and uses a short last-seen search memory. Entering the Den ends danger immediately while the meter decays. Leaving the old 28 m update radius no longer wipes accumulated suspicion.

Perception and reachability are separate: an ordinary airborne Jimothy remains visible above a clear street location, but the remembered chase destination is that ground position. Raised navigation targets clear any stale path immediately. Ground navigation remains limited to the avenue/curbs; roof and basement routes are rejected.

Animated NPC navigation pivots before moving when its direction differs by more than 55 degrees, then accelerates as facing aligns. Existing Walk/Run clips have speed hysteresis and crossfades; pivoting samples slow Walk. This is not a newly authored foot-locked turning clip. Proactive clearance and pathfinding remain primary; dynamic obstructions cause yielding and replanning rather than sideways translation while facing elsewhere.

Blocked humans may briefly pause alongside a measured tall wall. An additive spine/head adjustment and two-link arm solve place the nearer wrist toward that wall without translating the root or feet. Unreachable hand targets are rejected. Dogs use a head sniff/look idle. These are procedural poses on existing rigs, not newly captured or authored animations.

Validation entry: `Jimothy.Editor.NeighborSuspicionAudit.Run` (graphics enabled, self-exits). The audit uses isolated physical fixtures and actual imported rigs; it does not access a GameSession or user saves. It covers gradual accumulation, a 1 m jump, wall occlusion, far-distance decay, Den safety, roof contact loss, stale path rejection, ground movement/facing agreement, supported wrist reach, and root/foot stability. Supported-rest images are explicitly arranged test fixtures, not evidence of a naturally encountered gameplay event. Existing whole-world navigation audits remain necessary to check district geometry.

## Read-only existing clip check

`Tools/audit_neighbor_motion.py` sampled the left human foot at 241 phases in both the source blend and a fresh import of the production FBX. Mid-stance nominal-translation-compensated drift was 0.88 mm Walk / 5.07 mm Run in the blend and 1.85 mm / 5.07 mm after FBX import; cycle-end position mismatch was zero. This checks one foot and excludes contact transitions; it is not full-body validation. It found no major stance defect requiring an emergency clip replacement. Axial hip/shoulder counterrotation and dedicated turning clips remain absent. The original gait actions and bones are retained. A subsequent supported-rest visual audit exposed jacket torso vertices incorrectly following the forearm; see the repair below.

## Jacket skinning repair

The first actual Unity supported-rest capture showed a sharp triangular torso flap, despite passing wrist/contact metrics. `Tools/fix_neighbor_jacket_weights.py` rebinds 2,099 jacket vertices by torso/upper sleeve/fore sleeve regions on the existing mesh. Mesh topology (7,253 vertices), UVs, 15 bones, and gait actions are unchanged. Staged FBX and blend are in `Art/Staged/NPC`; the reviewed `supported-jacket.png` shows a clean torso and bent sleeve. Blender wrist target error is approximately 1e-7 m. This is an approximate matching pose fixture; the actual Unity supported-rest audit and render must be repeated before accepting the fix. No claim of new realistic motion capture or a new rig is made.
