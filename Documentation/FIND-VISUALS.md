# Find visuals and discovery reveal

Original authored procedural geometry, September 8, 2026. No downloaded assets, AI images, or additional third-party licenses. All 22 exposed reward IDs have distinct shape recipes in ItemVisuals.cs; old inventory IDs retain category/name fallbacks.

The exposed set: lure, taco, cardamom knot, pizza, salmon scraps, fries, arcade token, bottle opener, croissant, cassette, berries, cheese wedge, amber marble, thermos, sesame bagel half; seven trophies: salmon sculpture, bell/clapper, Rainier diorama, golden bagel, compass, tiny troll, crown. Seven trophies are supplied even though the initial request mentioned four, matching the expanded placements coordinated with the world owner.

Each find is one generated mesh with vertex colours, one renderer/material, no collider, and an owned-mesh cleanup component. Parts are directly assembled into vertex/triangle arrays, avoiding primitive GameObjects and per-part draw calls. FindSurface is a lightweight opaque URP colour/light shader, not a physically accurate glass or metallic shader. Geometry is kept available for validation and can be rendered by ItemVisuals.Create(item,parent,spot). Repeated placements receive deterministic presentation yaw.

Successful GameSession.Collect invokes a 2.3-second nonblocking DiscoveryReveal. It shows the actual item's mesh in a 256px portrait, its rarity and full name, and pocket/shiny outcome. The card has no raycast target or input capture, and closes on pause/menu. The existing quiet generated pickup tone remains. There is no road-wide particle or marker cloud. The contextual SEARCH marker is smaller and scales down near the camera. Keyboard F and touch Search continue to use the same existing collection path. Inventory/cooldown/save/objective mutations were preserved.

Tests supplied: distinct mapping for all 22 exposed rewards, varied valid fallback mappings, and instantiated mesh assertions for single renderer/submesh, vertex colours, three-dimensional bounds, bounded vertex count, and absence of colliders. Root owns Unity execution and first-night regression. At this handoff compile, tests, and visual capture are pending; implementation alone is not a visual-quality or performance verification.

## Verification

Editor compilation and initial ItemVisualAudit capture passed. `PlaytestCaptures/finds-22-contact-sheet.png`, `discovery-bagel.png`, and `discovery-trophy.png` were visually inspected. All silhouettes differed and the actual snack collection card appeared. Inspection prompted improved taco-shell geometry, upright golden-bagel display, removal of rope from amber marble, and extra portrait headroom; those refinements still need final capture/native inspection. The initial sheet predates these small refinements.

FirstNightAudit passed 23 assertions including authored trophy mesh displayed in the den, inventory/banking/objective completion/cushion purchase, and unchanged user save/main/bak/tmp. Added den geometry assertion uses actual generated vertex colours. ItemVisualTests enforce 1.5cm minimum 3D thickness, allowing intentionally thin metal finds; the initial 3.5cm threshold incorrectly rejected the bottle opener and was corrected.

Final EditMode result: 12/12 passed; 0 failed (`PlaytestCaptures/items-editmode.xml`).

Final native 0.4.0 reveal retest passed: `AgentPlaytests/v7-reveal/frame-1.png` shows the actual bagel portrait/name/rarity, frame 2 shows three metres of forward movement while the card remains visible. Initial native Search/Den/deposit passed; existing save fingerprints unchanged across both runs. World finds use .55 common/.75 trophy scale and ground the visual independently of interaction anchors. The capture driver was updated to include dynamically created overlays; normal game UI behavior was not changed by that hook.
