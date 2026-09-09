# Fun-first review of the external feedback

The supplied review describes0.4.3; current baseline is0.4.4. It is design input, not an instruction to execute every proposed system. Existing user goals establish native iOS as the shipping target and persistent survival/exploration as the game structure; mobile WebGL is the current testing bridge.

## Proceed first

1. **Movement identity and control feel.** Jimothy needs readable head/ears, a tapered body under the hunch, and forepaw catches followed by a hindquarter push. Preserve quirky asymmetry without four legs windmilling outward. Jump anticipation, tucked flight, and a short grounded recovery matter more than increasing jump height. Device playtests remain essential; desktop WebKit cannot prove iPhone/iPad feel.
2. **Make rewards specific and decorating personal.** Fix the generic fallback meshes across the catalogue; give each find an authored asset and meaningful identity. Let users select display priorities and buy/toggle individual room upgrades. A short excursion should end with a visible improvement to their Den.
3. **Extend goals after the tutorial.** Offer small optional collection pursuits (music finds, harbor curios, food for the pantry), with progress and a visible cosmetic reward. Keep broad exploration free. Do not force a dawn deadline into an endless-survival premise.
4. **Measure real performance, then optimize the bottleneck.** A rolling frame-time display is useful during device testing. Check visible rendering counters in native profiling;251generated material/chunk meshes are not necessarily251draw calls in every view.120draws/500ktriangles are provisional design budgets, not universal pass/fail limits. Keep instancing, texture memory, skinning, overdraw and local-light cost in view.
5. **Cheap control/settings clarity.** Selected frame-rate mode must be unmistakable, with separated touch targets. First-run touch guidance should be dismissible and should not block gameplay.

## Already present / refine instead of duplicating

- Hunger already decays outdoors and drains health at zero. The Den is safe. The default full meter lasts about24minutes, so short sessions barely feel food pressure. Tune only after a device playtest and make low-fullness feedback clearer; don't silently turn it into a punitive timer.
- Eating consumes a real item; banked food can be eaten in the Den. Death clears loose bag contents and preserves banked collections, shinies and decor. The bank-versus-eat choice needs presentation and food selection, not a second inventory system.
- Dogs, angry humans, fishermen and other neighbors already exist. Adding another enemy before fixing readable pursuit and escape routes has low value.
- Rarity and16named trophies already exist. The gap is distinctive physical rewards, discovery variety and goals after the three-step tutorial.
- Seeded placement alternatives and persistent restock state already exist. Randomizing the entire world each night would discard learned routes and conflicts with the stable home base.

## Verify, don't apply blindly

- The three named unsupported shaders are internal/debug paths. Reproduce with target-device logs and visible rendering evidence before changing shipping HDR or stripping variants. Never enable blanket stripping that removes the fur's required vertex-color shader data.
- Texture compression and Brotli need browser/server support and correct response headers; neither should be enabled as a checkbox without testing the resulting build on Safari.
- Jobs/multithreaded physics and a full data-driven world rewrite are later scaling work. Current single-threaded physics is not itself evidence of a bottleneck.
- A tappable objective may gently orient the camera, but must not unexpectedly spin it while steering or jumping.

## Defer

Forced dawn endings, roguelite world resets, new antagonists and large route-unlock systems. First make existing traversal, discoveries and the Den rewarding. Keep authored audio content with the user; do not add generated music or samples as part of this art pass. Event/mixer integration can be a separate small task when those recordings are ready.

## How to judge the next playtest

On the actual iPad/iPhone: time to first food; attempts to climb one rooftop; camera corrections per minute; successful versus failed jumps; number of meaningful new finds in10minutes; whether the player chooses to bank a find without being told; whether they want another excursion after decorating. Record frame-time spikes during those same moments. Scores from agent checks are not substitutes for human fun ratings.

## Follow-up: requested stealth and style

The subsequent stealth/trick request fits the first priority well: it adds a readable risk decision on the street and a skill expression/reward loop on rooftops. It should be evaluated with explicit playtest tasks: evade one suspicious NPC using an occluding object, reach the first roof without repeated fall-offs, land an intentional flip, and spend a style reward in the Den. Track successful completion, confusion, unintended damage, and whether players voluntarily repeat a route. Do not equate a functioning score counter with demonstrated fun or invent playtest scores.
