# Character, rooftops and finds — build 0.4.0

September 8, 2026. This remains a Mac development playtest, not a measured iPhone release or a claim of concept-render parity.

Jimothy uses the authored v6 Blender source and exported FBX: lower legs shortened by up to .14 model units, rear haunch width tapered by up to 14%, and head/ear position raised while retaining the rounded hunch. The rear camera now starts at 32 degrees and 2.65m, keeping the character larger on screen without changing the rear-only heading rule.

The coat uses 5,887 rooted, irregular-length hair cards, sparse longer guard hairs, a split-strand alpha texture with fading roots and mip coverage preservation, and a normal map on the undercoat. Tip motion remains a lightweight acceleration-driven spring plus GPU bending; this is not individual strand simulation. Geometry totals 81,518 triangles versus 92,882 in v5, about 12% fewer; that count alone is not a frame-rate benchmark. Charcoal/silver coloration receives the world's warm lamps. Close-up and gameplay readability were separately reviewed.

Jump is now a non-looping, velocity-phase-driven sequence: compressed takeoff, forepaw reach, hind-leg tuck, descent preparation, and a brief grounded landing cushion. It does not delay input or alter the ballistic controller. The walk/run bake uses quarter-frame samples and exports without key simplification; Unity animation compression is disabled for Jimothy to retain them. See ANIMATION-V6-QA.md and the numeric reports for residual contact error and test limitations.

Four roof crossing types replace identical bridges: festoon-lit utility footing, laundry balance rail, ventilation service duct, and cedar catwalk. Their visible approaches rise over the parapets, with continuous colliders matched to the walking surface. These are reinforced traversable routes; decorative light wires are not invisible walkways. Rooftops gain herb planters, benches, storage and laundry clusters while preserving alley access lanes.

The 22 currently exposed item types now have distinct solid meshes, including seven trophies. Restaurant finds differ by shop, and both roof rows hold different rewards. Pickup models are smaller and grounded near their interaction anchors. Search shows a short rotating actual-mesh portrait, name and rarity without taking control; banked trophies display the same identifiable model. The catalog still contains 256 definitions, with category fallbacks for items not authored/exposed here.

Map revision 3 migrates old positions safely home and clears obsolete pickup cooldowns. Bag, pantry, currency, trophies and decor are preserved. Current-map loads retain position normally.

Verification reports: rooftop-crossing-audit.json (8/8 bidirectional, no jump), traversal-audit.json (14/14 routes), items-editmode.xml (12/12), first-night-audit.json (23 assertions, user saves unchanged), jump-animation-audit.json (actual imported Animator poses differ and held phase stays stable). Native final visual and control observations are recorded in the agent playtest results; these are developer-assisted Mac checks, not blind human scores or physical iOS testing.

Final native reveal passed after updating the capture harness to include dynamically created canvases. The game has been reopened at its normal Continue menu. Updated Unity source and v6 Blender files are also mirrored into the original Documents workspace.
