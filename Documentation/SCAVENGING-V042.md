# Scavenging placement pass

The previous implementation restored ordinary finds at the same fixed anchor every 600 world seconds. This pass keeps the same 22 item identities and node/cooldown ordering, but chooses positions from 63 validated sites beside existing props. No district geometry was changed.

Kitchen finds use the short ends of back-door bins; patio finds use crate shadows and planter edges; rooftop keepsakes use vent/chimney corners. The lookout has two inward-accessible deck positions. A saved per-adventure seed varies initial placement. Each ordinary harvest advances to a different candidate; loading the same state preserves the choice. Catalog items and inventory are not rerolled.

Ordinary finds replenish after a deterministic, varied 20–35 minutes of world time. Replenishment is deferred while Jimothy is within 14 metres. Additive `harvests` and `stockedHarvests` cooldown fields preserve pending replenishment through reload; existing saves default these fields safely. The first welcome snack stays accessible at home and becomes one-time, like the already one-time rooftop trophies. An old consumed welcome cooldown is also retired; no saved inventory or banked collection is removed.

Ordinary contextual Search requires 1.3 metres, rooftop Search 1.55 metres, and the welcome snack retains 2.4 metres. The existing world-collider line-of-sight requirement remains. Each candidate is checked against actual world collision for a supporting surface, a supported raccoon-sized clear approach, and an unobstructed interaction line. Future geometry changes that invalidate all candidates log a warning and retain the prior authored anchor rather than losing the item.

Validation:

- `Logs/scavenge-v042-retest.log`: 22 stable nodes, 63 supported and searchable positions. Every repeatable find has three valid positions; each rooftop find has at least two. The initial audit caught an off-deck lookout approach, corrected inward before the passing run.
- `Logs/editmode-v042.xml`: 19/19 Editor tests passed, including successive-site variation, deterministic reload, variable replenishment intervals, old-save field compatibility and the welcome anchor.
- `PlaytestCaptures/first-night-audit.json` and `Logs/firstnight-v042.log`: 23/23 actual Editor playmode interaction assertions passed, including first snack, rooftop keepsake, banking and decoration purchase. Main, backup and temporary native save files remained byte-for-byte unchanged.

FirstNightAudit now uses the node's validated SearchApproach for its deliberate interaction-test teleport, rather than assuming an arbitrary offset remains clear beside a concealed prop. This audit tests interactions, not player traversal or subjective discovery difficulty.

No native/Web build was produced by this pass. Native playtesting, physical-device testing, and visual review of the concealed positions remain for the coordinated final build. No claim is made that these checks establish long-term balance or player satisfaction.
