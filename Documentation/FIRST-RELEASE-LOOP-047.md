# First-release loop — 0.4.7

One compact Ballard map, with two marked north-roof round trips. The goal is a repeatable scavenging outing: leave the Den, risk a small haul, and bring it back to improve the room.

## Player rules

- Eight pocket slots. Every find occupies a slot, including valuables. Existing oversized legacy bags are preserved; no more finds fit until space is made.
- Currency from valuables and up to15 shinies from landing style remain loose. Death or falling outside the playable world loses the loose haul. Ordinary supported falls still apply height-based health damage.
- Returning physically to the Den and choosing **Unload pockets** banks the haul. Empty banking cannot finish a night. Repeating the action cannot pay twice.
- Fast travel remains available with empty pockets and no nearby suspicion; a carried haul must return by a physical route. The Den button then gives return guidance.
- **Head out** starts the next outing. Leaving the Den after settlement also starts it. A new outing restores health/fullness and seeds a fresh arrangement; no find replenishes during an active outing.
- Saving, pausing, or leaving the browser preserves the current outing, consumed finds and loose rewards. Time does not advance while away. Banked finds, decor and shinies survive a loss.

## Goals and events

Three rotating banked-haul goals: three snacks, two valuables, or one rooftop trophy. A completed goal adds12 shinies. Eating a required snack before banking reduces the carried proof.

Three reusable events: record-store music, harbor unloading, garden watering. They feature three different collectible sets. From20–65seconds in each100second interval, music and watering reduce human sight range; harbor unloading slows the fisherman while increasing his sight range. Intervals and event identity survive reload. The sequence covers all nine goal/event pairings over nine outings, with the next event shown before departure.

## Curated collection

27 item types are available in this map's rotation: six foods, six valuables, nine themed curios, and six trophies. They use existing authored 3D find assets. The wider256-item catalogue is retained for save compatibility and later map expansion.

Three banked sets award furnishings once, even if the player already bought the furnishing:

- Guitar pick, record sleeve, cassette → vinyl wall.
- Acorn, pine cone, seed packet → plant corner.
- Cork float, lure, ship compass → harbor shelf.

The night board shows set progress and item clues. Valuables are also retained as displayable finds after their currency is banked.

## Validation and release limits

The Mac/Web builds and all required loop, route, NPC, movement-save and browser checks passed. See [verification and limitations](RELEASE-047.md) for counts and methods.

These are automated functional checks. Physical iPhone/iPad frame rates, battery/thermal behavior, and human fun/readability scores still require device playtesting. This build is a focused playable iteration, not an App Store readiness certification.
