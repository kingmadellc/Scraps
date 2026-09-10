# 0.4.8 — Treasure celebrations and a shared menu identity

Rare and legendary finds, including trophies, now receive a dismissible concert-poster reveal. The original title artwork, Fugaz One headline, Barlow Condensed supporting text, cream/rust dimensional lettering, and brass/evergreen palette carry the start-screen identity into the reward moment. The portrait uses the actual collectible mesh in a 768px transparent render target with an isolated warm studio shader. Ordinary finds retain a short 256px portrait card and do not pause movement.

A full reveal freezes the session and motor, blocks world touches, clears held move/look/jump gestures, and requires a deliberate dismissal after a short 0.45-second input guard. Enter, touch and gamepad south dismiss; Escape transfers to the pause menu. Orientation, app suspension, and focus loss transfer to a safe pause instead of unexpectedly resuming. Temporary portrait resources are released on exit. The reveal never banks or duplicates loot; its copy explicitly tells the player to return to the Den.

Major title/menu headings now share Fugaz One. Menu controls and supporting text use Barlow Condensed, including settings, pause, run end, new-game confirmation, Den and night-board pages. A visual review caught small phone labels; menu button type was enlarged and settings copy simplified before the final build. The gameplay HUD retains its separate legible face. Existing open-font license files remain included.

The game has not been permanently renamed. See [ten replacement names and evidence](NAMING-RESEARCH-048.md). Bundle identifiers and save paths remain unchanged.

## Verification

- Production Unity reveal audit: 111 assertions passed across desktop and mobile layouts, including pause/health/movement freeze, single collection, input shielding and dismissal, common-card behavior, pause/menu transfer, portrait alpha and cleanup. Eight screen captures were generated and the relevant layouts visually inspected.
- Chromium: 13/13 touch-loop checks passed with zero console/page errors, including physical Den departure/return, loose loot, save/reload, single banking, next-night reset and portrait pause. Tablet and phone settings were captured; gameplay exercised at 932×430.
- WebKit: 9/9 phone checks passed, including touch jump, save/reload and portrait pause, with zero engine errors.
- Supplied develop-web-game client: New → Head out → jump completed using the Metal adapter; final screenshot and read-only state reviewed, no console errors.
- Mac and Web builds report 0.4.8; Mac code signature verified. Web payload: 124,356,618 bytes.

Chrome's default software graphics path caused contention/timeouts during initial concurrent runs. The focused Chromium loop uses Metal on this Mac. `Tools/run_skill_client_metal.cjs` selects the same backend for the supplied develop-web-game client without changing its action/state/screenshot logic. These are test-launch options, not shipped game settings. Editor visual captures are in `PlaytestCaptures/discovery-reveal/`. These are functional and visual checks, not physical iPhone/iPad performance measurements or human preference research.
