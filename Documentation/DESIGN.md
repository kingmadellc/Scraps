# Jimothy — Small Paws, Big Appetite

An original third-person survival platformer through Old Ballard's storefronts, alleys and roofs. Scavenge to stay fed, evade trouble, and turn a fixed den into an increasingly absurd collection of neighborhood treasures.

## Character contract

The user's supplied photograph at `Art/References/jimothy-user-primary.png` is authoritative. Jimothy has an unusually high domed back, a compressed torso, a relatively small head carried low in front, long skinny limbs, little dark hands, and a short fluffy tail. The original short-legged spherical design and first hero image are superseded.

The third modeling pass uses a continuous voxel-remeshed surface, blended joint weights, embedded facial markings baked into a 2048×2048 albedo, and a directional groom bound to the skeleton for Blender rendering. The revised rig uses 11 bones including separate shin joints. The current running loop offsets hindlimb timing, varies front/rear extension, bends raised forelegs, and sways the torso. It is an authored interpretation: the still photograph does not establish exact real-life timing. A source video is needed for a faithful gait study. Final approval should compare the side silhouette and moving footage, not just a flattering front render.

## Core loop

Leave the Trash Palace → find food → spend fullness while exploring → identify risky humans and animals → take a climbable route to roof rewards → return or fast travel when safe → bank supplies and trophies → spend shinies on den decor → push for a longer survival record.

Touch: left movement pad, right camera drag, Jump, Eat, Home, Den and Pause. Keyboard: WASD, Space, right mouse drag, E, H, Escape. Gamepad movement/jump/orbit bindings are authored. All require runtime validation.

24 carried objects creates a reason to return. Valuables immediately become shinies. Food restores fullness and some health. Street resources replenish after 600 simulated seconds; trophies do not replenish. Deposit is required to permanently bank a trophy. Death loses loose pocket contents and ends the run while keeping the den, shinies and banked collection.

## World

Delivered Blender block: ten mixed-height brick buildings, sandstone cornices, storefront windows, awnings, balconies, AC units, chimney stacks, alley climb steps, crossing rails/clotheslines, trees, planters, street lamps, a fixed den and a Marvin's Garden bell-inspired landmark. The map is a designed approximation, not a surveyed recreation of Ballard Avenue. Geometry is prototype quality, especially side/back facades and foliage.

Runtime scenic stand-ins: changing warm daylight, a primitive Rainier silhouette and an occasional floatplane. The current sky is not a physically accurate sunrise/sunset simulation. The distant mountain in the concept art is an art-directed background; real sightlines and orientation need a location pass.

Planned landmark expansion: detailed Marvin's Garden and historic bell, Ballard Carnegie Library, Ballard Avenue's historic hotel/corner facades, Salmon Bay docks and fishermen, with the Locks as a later connected district. Do not place all of these at literal walking distance inside one short block.

Planned traversal polish: ledge grabs, climbable drainpipes, fire escapes, AC hops, balcony rails, awning rebounds, roofs with distinct silhouettes, alternate return routes, secret alcoves and traversal skill challenges. Current source supports jumping and collision geometry, not the entire future move set.

## Neighborhood population

Dogs see and chase. Cats stop and arch their backs with a hiss cue. Property owners defend a limited area; fishermen swing net stand-ins. Kind humans share food. Passersby, unhoused neighbors and visibly impaired pedestrians remain in the setting, including a slumped/swaying pose prototype. They are not automatically part of an enemy faction. Individual hostile or unpredictable characters can occur across the population. Neighborhood props, specific human models, substance-use scenes, richer routines and audio are still production content, not delivered finished assets.

Readable behavior should preserve uncertainty without making hits arbitrary: gaze, posture, vocal cue, a wind-up, then action. A dog should lose interest if Jimothy breaks sight or climbs out of reach. The current AI is simple line-of-sight and direct movement, without a navigation mesh or robust obstacle avoidance.

## Collection

`Items.json` contains 256 IDs: 64 food variants, 64 valuables, 112 curios and 16 named trophies. Some are meaningful variants of a base item, not 256 entirely different silhouettes. Most use shared geometric pickup placeholders. Named trophies include The Golden Bagel, Marvin's Lost Clapper, The Lockkeeper's Key, and The Mayor of Trash.

Production trophy loop: each legendary object gets a specific environmental clue, a bespoke route or challenge, a distinct model and icon, a discovery celebration, and a permanent display slot. Current trophies sit on roof locations and bank into simple den pedestals; they do not yet satisfy the intended rarity and discovery depth.

## Premium UI

Evergreen backgrounds, cream typography, sea-glass accents, salmon highlights. New adventure, Load adventure, Settings. A new game confirms replacing an existing slot. The generated corrected hero is wired into the menu; it is 1672×941, below the requested minimum, and remains a concept-resolution asset. A genuine ≥1920×1080 or 4K production hero remains required. The delivered Blender character renders are 1920×1920 and the district overview is 2560×1440.

## Next acceptance milestones

1. Approve revised silhouette and obtain running footage; refine foot contact, fur cards and animation polish on the continuous textured skin.
2. Install Unity/Xcode; compile, test, play and fix the initial street-to-roof-to-den loop.
3. Replace one street section with a detailed, location-grounded environment and validate performance on an iPhone.
4. Add full traversal, unique NPC art and behavior/audio, bespoke trophy routes, and richer den decoration.
5. Add linked player identity, cloud persistence and conflict recovery; exercise cold launch and interrupted-save cases.
6. Produce final resolution menu art, finish accessibility, audio, device QA and TestFlight.
