# Scraps — playtest 0.5.0

On this Mac, open `/Volumes/Jimothy Dev/Projects/Jimothy/Builds/Scraps.app`. Keep the **Jimothy Dev** drive connected. Unity Hub is not required to play the built app.

**Public play link:** https://kingmadellc.github.io/JimothySurvival/?v=0.5.0 — Safari in landscape, then Play. This works away from the development Mac. The public site has its own browser save; local-link saves do not automatically transfer.

For LAN development only, use Safari on the same Wi-Fi as the Mac: **http://192.168.4.23:8765/?v=0.5.0**. Keep the Mac awake and the development drive connected. Turn the device sideways, tap the browser launch button, then choose **Continue** or **New game**. This is a local browser playtest, not a TestFlight installation. Saves are local to each browser/device and are separate from the Mac app; cloud synchronization is not implemented.

## The outing loop

1. At the Den, open **Stash → Next outing → Head out**. A fresh adventure opens this board automatically. Leaving after a banked outing also starts the next one.
2. Carry up to eight finds. Follow the current goal: three snacks, two valuables, or a rooftop trophy. The goal pays12 bonus shinies only when its required items are banked.
3. Walk back down the marked steps and into the Den ramp. Choose **Stash → Unload pockets** to make your haul safe. Fast travel works only with empty pockets and no nearby suspicion.
4. Decorate, choose favorite displays, and complete the music, garden, and harbor sets for free furnishings. Open Collections on the outing board to see required items and furnishing rewards.
5. Head out again for new loot positions, the next goal, and the next event. Nothing replenishes within the same outing. Death or falling outside the map loses all loose finds and loose shinies; your banked collection stays safe.

**Continue** keeps your existing collection. Older saves migrate without discarding oversized bags or banked progress. Starting a new adventure with an existing save asks before replacing it. Save/resume preserves the outing, consumed finds, loose rewards, NPC awareness and movement/fall state. Nothing advances while away.

## Controls

| Action | Mac | Touch |
| --- | --- | --- |
| Move | W/S move; A/D steer | Left floating stick; small drags walk |
| Look | Hold right mouse button and drag | Drag open space on the right |
| Jump | Space | Tap the jump disc |
| Aerial trick | T while airborne | Swipe from jump disc up/down/left/right |
| Search | F or Search | Nearby search glyph |
| Eat carried food | E | Food glyph when useful |
| Den travel / return hint | H or Den | House glyph |
| Bank / decorate / night board | Stash inside Den | Bag glyph inside Den |
| Pause / save / settings | Escape | Pause glyph |

Touch settings include an optional tap-in-air trick scheme. A clean full trick earns style; up to15 loose shinies per outing can come from style milestones. Large uninterrupted falls damage health; use supported intermediate landings to break the descent. **Eat from pantry** in the Den uses banked food, leaving carried goal snacks intact.

## Two marked roof routes

Both begin at the open north end of the avenue near the Den. **A / Salmon Lookout** climbs the west staircase; **B / Records Rooftop** climbs the east staircase. Jump onto the first step and keep moving along the landings. Follow the lettered paint and **DEN / DOWN THE SAME STEPS** signs to return. Hostile NPCs cannot follow onto the roofs.

## Rebuild / restart the local server

The working project is `/Volumes/Jimothy Dev/Projects/Jimothy/Unity`, using Unity **6000.3.0f1**. Open `Assets/Jimothy/Scenes/Ballard.unity` in the Editor. The build commands are `Jimothy.Editor.DesktopBuild.Build` and `Jimothy.Editor.MobileWebBuild.Build`.

If the Safari link stops responding, run `python3 Tools/start_ipad_server.py` from the external project. A changed Wi-Fi address may require a new link; use the Mac's current LAN address. Refresh Safari after a new build. Actual iPhone/iPad performance, heat, battery use, and human playtest scores still need device testing.

## Treasure celebrations (0.4.8)

Rare finds and trophies open a full-screen reveal using the title artwork and fonts. The actual item rotates in its portrait. The action pauses until you tap **Back to the night**, press Enter, or press the gamepad south button. Escape transfers to the pause menu. Finds are still loose and can be lost until banked at the Den. Ordinary finds show a short card without interrupting movement.

The selected name is **Scraps**, with no subtitle. The old internal save identifiers and public URL are intentionally retained so existing saves continue to load. A title choice does not constitute trademark clearance.
