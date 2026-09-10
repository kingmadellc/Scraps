# 0.4.7 — One more night

Scavenging now has a complete outing loop: carry a limited haul, return physically to the Den, bank it, and choose another night. Death loses loose items and loose shinies; banked collections survive. Repeated banking cannot pay twice, lost trophies return in later outings, and active outings do not restock while playing or away.

## Playable changes

- Eight pocket slots; valuables and landing earnings stay loose until banking. Existing oversized legacy bags and banked progress are preserved.
- Three rotating banked-haul goals, three recurring night events, and all nine goal/event pairings over nine outings. Goal completion adds12 shinies.
- A curated27-item rotation using existing 3D finds. Three themed collections unlock a vinyl wall, plant corner and harbor shelf.
- A Den night board with clear rules, next-night information and collection clues. Pantry eating preserves carried goal snacks.
- Two marked north-roof round trips, actual-find connectors, grounded waypoint advancement, missed-stair recovery and directions through the Den ramp.
- Interruptible NPC attack warnings and a short reacquisition grace. Pursuit, suspicion and patrol state survive saves.
- Movement, camera, falls and mantle/flip state survive saves. A settled landing cannot replay damage or rewards. Fixed scaled cat controller step height and guarded inactive movement during replacement.

## Verification

- 31/31 EditMode tests.
- 23 runtime outing checks, including bank idempotence, loss/reset, goal payout, migration/resume and pantry protection.
- 86 route/guidance checks and8 seeded actual-roof-find approach/return cases.
- 50 production NPC controller checks with no engine errors; perception, world navigation, assisted stairs, rear camera and touch/air-skill audits passed.
- 12 actual-motor persistence checks, including identical damage for uninterrupted versus reloaded nine-metre falls, flip continuation and obstructed mantle recovery.
- Chromium13/13: trusted touch departure, collection, save/reload, physical return, banking, next outing, phone controls and portrait pause. Zero console/page errors.
- WebKit9/9: phone board, departure guidance, touch jump, save/resume and portrait pause. Zero console/page errors.
- The supplied web-game smoke client completed New → Head out → jump; gameplay image and state inspected, no console errors.
- Mac and Web builds both report0.4.7. Mac signature verification passed. Web build124,339,968bytes.

The initial browser run exposed the scaled-cat controller error; it was fixed and the full touch loop rerun successfully. An early WebKit assertion ran before the new scene snapshot arrived; the harness now waits for observed state transitions. The runtime integration's JSON clock comparison uses microsecond tolerance for serialization precision. No player save was read or changed by editor audits.

Screenshots were inspected on tablet and phone layouts, with agent review of onboarding and route guidance. These checks do not establish physical-device frame rates, thermals, battery life or human fun scores. This remains a playable prototype rather than an App Store readiness certification. Cloud save synchronization is not implemented.
