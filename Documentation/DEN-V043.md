# 0.4.3 — Den and movement-responsive fur

Home is now Den. Walk through the marked brick opening at the north end of the avenue and down the ramp, or use H / the Den travel button when safe. Stash opens collection management inside; Unload pockets banks finds and refreshes physical displays. Follow the ramp south to return to Ballard.

The basement has worn brick, textured rug and upholstery, a couch, coffee table, guitar, amplifier, cassettes, flannel and original 1990s-style gig flyers, including “Smells Like Dinner.” Warm practical lamps replace the outdoor moon wash underground. The follow camera shortens to fit the room while preserving the rear view.

Up to 32 unique banked finds appear on shelves, the coffee table and amplifier. Selection prioritizes trophies, non-food collectibles, rarity and value. Displays rebuild from existing saved inventory; collecting more than 32 types does not remove anything from storage. Purchased cushions and hanging glass floats also decorate the room.

V8 keeps the existing character rig and animation keys and adds longer layered fur, wider overlapping strands, corrected ambient response and subtle acceleration-driven tip motion. Roots remain fixed. Unity run/walk/stop captures at production 2× MSAA verified millimeter-scale lag and settling. Some fine silhouette aliasing remains; physical iPhone/iPad performance has not been measured.

Validation: 19/19 EditMode tests; Den geometry 42/42; Den experience 219 assertions and 441 camera samples; 22 find nodes / 63 supported placements. A populated render fixture verified 22 displayed objects resting on their support surfaces. Fixture images use synthetic inventory and never modify the player save. See DEN-EXPERIENCE-AUDIT.md, DEN-COLLECTION-ART.md and CHARACTER-V8.md for evidence and limits.

Build verification: native 0.4.3 completed and passed strict deep codesign verification. Web build passed, 117,043,267 bytes uncompressed, served on the existing LAN origin. Native pre-build save backup: LocalBackups/before-v043.

Compiled playtests: native agent successfully collected, traveled to Den, deposited, saw the item on the table, and walked back up the ramp. WebKit touch smoke passed 7/7 checks, including runtime version 0.4.3, Den/Stash/deposit and full-page reload restoring banked collection and interior position; no runtime errors. This uses desktop WebKit touch emulation, not physical-device profiling.
