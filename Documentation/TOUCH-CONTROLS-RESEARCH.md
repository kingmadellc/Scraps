# Touch controls research and implementation — 0.4.6

Research snapshot: September 8, 2026, US App Store. Ratings are whole-game averages, not measurements of input usability. There is no supported claim that one control scheme has the highest control-specific rating across mobile games.

| Comparable game | Store rating observed | Fit for Jimothy |
|---|---:|---|
| [Sky: Children of the Light](https://apps.apple.com/us/app/sky-children-of-the-light/id1462117269) | 4.8 / 267K ratings | Strongest overall-rated comparator in this shortlist and closest 3D exploration/jumping fit. Adopt two-thumb separation, proportional movement and compact action controls. |
| [Alto's Odyssey](https://apps.apple.com/us/app/altos-odyssey/id1182456409) | 4.4 / 3K ratings | Strong fit for readable aerial tricks and immediate landing rewards; one-touch simplicity is useful, but auto-running 2D movement cannot replace freely explored 3D streets. |
| [Genshin Impact](https://apps.apple.com/us/app/genshin-impact/id1517783697) | 4.2 / 578K ratings | Useful 3D exploration comparator. Its combat-heavy interaction needs exceed Jimothy's; do not duplicate a dense combat HUD. No control-specific rating inferred. |

## Primary-source basis

[Sky's developer control guide](https://thatgamecompany.helpshift.com/hc/en/17-sky-children-of-the-light/faq/489-how-do-i-get-my-character-to-move/) documents a left movement area, right camera controls, proportional walking/running, and a right-thumb jump; it also describes directional flick jumping. The guide is older, though still served by the developer; precise current tuning is not public. [Alto's official press kit](https://altosodyssey.com/press/) describes its one-touch trick/combo system. Neither source provides source code or establishes Jimothy's optimal thresholds.

## Adaptation

- Original vector icons and translucent circular controls replace the rectangular gameplay action row. No copied artwork or proprietary code.
- Floating left stick starts near the finger. A small drag walks; farther displacement runs. All cardinal directions produce travel. The movement reference is captured when the finger lands, preventing rear-camera follow from bending a held direction into a circle; camera dragging deliberately rotates that reference.
- Right-side open space exclusively controls the camera. It never requests a trick.
- The jump disc responds on press, without waiting for gesture recognition. An optional swipe from that same disc adds a frontflip (up), backflip (down), or left/right barrel roll. A short request buffer covers the grounded-to-airborne transition. Very fast swipes are also checked on release; the flip takes0.38 seconds to leave a more forgiving input window in a normal jump. Each gesture can request only one trick; taps and minor drift remain ordinary jumps.
- Search appears near loot; Stash replaces it in the Den. Food is contextual. Pause, Den travel and camera recenter remain explicit controls.
- Pause → Touch controls explains the mapping and offers tap-in-air frontflips as a lower-dexterity alternative.
- Existing safe landing, style rewards and fall damage remain authoritative; requesting a trick cannot grant points or erase a fall.

## Evaluation

Automated tests must cover four-direction movement without circling, four trick directions, multi-finger ownership, tap versus swipe, camera isolation, pause/reset and actual phone/tablet render sizes. Physical iPhone/iPad playtests should measure accidental tricks, missed jumps, camera corrections and player preference against0.4.5. Do not label the redesign proven more fun before those trials.
