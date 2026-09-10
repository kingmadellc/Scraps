# Two north-roof round trips — 0.4.7

Coordinated Unity validation passed: DenRoundTripAudit passed all 80 checks, and ExpeditionRouteLootAudit passed all 8 live-find approach/search/return cases across seeds107–110. These results validate scripted physical traversal and interaction reach, not human readability.

A / Salmon Lookout uses the existing west north access staircase and stable `roof_-1_4` cache identity. B / Records Rooftop uses the existing east north access staircase and `roof_1_4`. Both follow the physical Den ramp, the previously tested west street approach, and the open north street at z44. Their return retraces existing landings; there are no new bridges, shortcut teleports, or map extensions.

Existing street signs now identify A/B and say to jump up the steps. The roof landing has an explicit “DEN / DOWN THE SAME STEPS” plate. Lettered paint and return chevrons mark the route; signs and paint do not add invisible collision obstacles. Return plates are supported at the outer landing edge.

## Integration

- `ClosingTimeWorld.DenRoundTrips` exposes Id, Name, CacheId, Outbound, Return.
- `AppendFindApproach(route, liveLootNode)` appends a connector to the actual `SearchApproach`, because the cache moves among seeded vent/chimney sites. Reverse this full array to return from that find.
- `NextWaypoint(position, grounded, waypoints, current)` advances only the current checkpoint after a supported landing, within .35 m horizontal and .25 m vertical. Completion is index == array length. It never selects a future waypoint by nearest distance through a wall.
- The avenue-side roof connector runs at x=±10.65 to bypass the vent; the chimney connector uses x=±14.9 to avoid the planter. Neither changes collision geometry.

## Audits

`Jimothy.Editor.DenRoundTripAudit.Run` runs in an isolated scene with actual motor/CharacterController movement at60Hz. It starts inside the Den and continuously traverses each base route out and back, allowing jump taps only for ascent. No teleport recovery. It also rejects NPC roof destinations and airborne guidance advancement. Output: `Logs/den-round-trip-audit.json`. Batch mode; no graphics required; self-exits.

`Jimothy.Editor.ExpeditionRouteLootAudit.Run` uses production Begin with suppressed saving and forage seeds107–110. For each actual cache node it moves from the roof checkpoint to `SearchApproach` and back, tests the actual SearchRadius and search line, and records seed/item/node/approach. Initial placement is explicitly at the roof checkpoint per case; the main round trip is independently tested above. Output: `Logs/expedition-route-loot-audit.json`. Play-mode batch audit; self-exits. No user save access.

These are scripted physical tests. Native player-facing readability and input feel require separate visual/playtest review; no human playtest scores are claimed.

## Guidance integration review

Read-only review of `GameSessionExpeditions.GetExpeditionTarget` confirmed use of the live-find connector and grounded progression helper. One mismatch was reported to the integration owner: the fallback return target stopped at DenEntrance after leaving the rooftop, so it did not guide the final entrance→ramp→DenInterior segment validated by the route audit. The root owner is responsible for correcting and checking that HUD integration.
