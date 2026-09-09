# 0.4.6 — touch movement and aerial tricks

The gameplay HUD now uses original circular icon controls, with fewer actions visible at once. The design adapts Sky's two-thumb exploration controls and Alto's simple trick vocabulary; see [research and source limitations](TOUCH-CONTROLS-RESEARCH.md).

## Play

- Left thumb: drag the floating stick in the direction you want to travel. Short displacement walks; farther displacement runs.
- Right thumb: drag open space to look around. The camera area never triggers tricks.
- Jump disc: tap to jump immediately. Flick from the disc up/down for a frontflip/backflip, or left/right for a barrel roll. Finish the rotation before landing to earn style points. Late tricks can fail; flips do not prevent fall damage.
- Search appears near a find. Stash replaces it at the Den. Eat appears only when you can use carried food or food stored at the Den.
- Pause → Touch controls: see the mapping or switch to tap-in-air frontflips. The preference persists locally.

Movement captures its reference direction at finger-down, so a held stick does not cause circles as the rear camera follows. Camera drags deliberately rotate that reference, including while the movement thumb rests at neutral. Separate fingers own separate actions. Rotation, focus loss and pause release held input.

Keyboard/gamepad controls remain available. This release changes input and trick timing; it does not claim additional environment or character art improvements. Physical iPhone/iPad comfort, performance and preference still need user playtesting.

## Validation

- TouchSchemeAudit25 checks; AirSkillsAudit, RearCameraAudit and all10 guided stair routes passed. Existing19 EditMode tests passed.
- Chromium19/19 combined touch checks and WebKit7/7 smoke checks passed with zero console/page errors. The supplied web-game smoke client also completed New/Search/Jump; screenshots and state were inspected.
- Web and Mac builds report0.4.6. Automated browser timing on this host is not a physical-device performance benchmark.
