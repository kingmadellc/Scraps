# Scraps UI design pass — 0.5.0

This is a heuristic design review of actual 0.4.9 screenshots and runtime source, not measured human research or a playtest score. Reviewed `PlaytestCaptures/scraps-049/phone-title.png` and `phone-resumed.png`. Changes below await actual build/capture review.

## Findings

The resumed HUD repeated the current goal, showed a persistent title/timer and style totals, and used a large toast across the character. Multiple permanent instruction strips competed with the street. Title microcopy repeated location/mood around an already expressive hero image. The outing board delivered rules, events, objectives and collections with similar visual weight.

## Implementation

- Title keeps Scraps, Ballard/Seattle, hero art and clear actions. Continue/New game labels are shorter. Removed decorative footer slogan/location/version and the disabled load row when no save exists. Save replacement still requires explicit confirmation with clear loss copy.
- HUD keeps labeled health/fullness bars, pocket capacity, loose currency, current objective and route guidance. Upper left panel is shorter. Removed repeated goal suffix, perpetual style/best line and desktop instruction strip. Banked balance/style records remain in pause/Den; landing feedback remains contextual.
- Toast uses a smaller lower-center band, with bounded font fitting. Modal toasts retain the separate top band. No input targets changed.
- Movement/look hint appears once per app session for ten seconds. Jump rests on a short JUMP label; swipe/tap results remain contextual. Pause → Controls contains touch and desktop/controller instructions; touch trick preference callback remains available.
- Pause, settings, Den and death copy is shorter. Settings no longer advertises unfinished music or labels a setting as a playtest; the same performance-overlay callback is preserved.
- Outing board leads with the goal and always states banking/loss. How it works exposes capacity, healing, goal bonus, saved-run behavior and full current-event effects. Collection progress remains on the board. Main gameplay/save callbacks are unchanged.
- Rare reveal retains item identity, rarity/value, banking requirement and paused status; removed repeated poster slogans. Common finds retain a short pocketed/banking cue. Discovery pause/dismiss logic unchanged.
- Browser entry removes duplicate controls tutorial. Retains device-local save warning and orientation guidance. Loader/test element IDs and callbacks are unchanged.

## Files

`GameUI.cs`, `GameUIDen.cs`, `GameUINightBoard.cs`, `GameUIStealth.cs`, `GameUITouch.cs`, `DiscoveryReveal.cs`, authorized single resting-label change in `TouchTrickGesture.cs`, and `Assets/WebGLTemplates/JimothyMobile/index.html`.

## Verification / next review

C# syntax parsing passed. No Unity or browser was launched by this agent. New `Jimothy.Editor.UIDesignAudit.Run` prepares sixteen actual Unity UI camera renders: title, pause, controls, settings, outing, rules, Den and HUD at 1688×780 phone and 1440×1080 tablet canvas sizes. Outputs `PlaytestCaptures/ui-design-050/` with text rectangles in report.json. Saving is suppressed, page methods are invoked directly, no preferences are changed and no saved game is loaded. These are full-safe-area forced-mobile previews, not physical-device touch/notch testing; runtime Screen-dependent font scaling also needs browser/device review.

Review text wrapping, touch target visibility, modal toast overlap, controls discoverability and high-suspicion/low-health readability. Use actual browser page flow for callback acceptance. Audit selectors based on visible strings need updates: Continue adventure→Continue; New adventure→New game; Touch controls→Controls; Plan another outing→Next night; Night board & collections→Next outing; Playtest stats→Performance overlay. Browser start remains `#start` with new Play label. Internal GameObjects/callback APIs are preserved; title action object names derive from the shorter labels.

## First actual render review / follow-up

Reviewed phone HUD, phone/tablet controls, phone settings and tablet rules in the first Unity capture set. Controls/settings/rules fit their bounds in these renders; no text clipping observed in the reviewed pages. The HUD still had repeated Den state and large unused-width background bands. Replaced the objective/guidance bands with content-sized pills; Den shows only “Den · Safe” or “Bank your haul,” while its labeled Stash action supplies the interaction. Guidance is hidden in the Den. Outside, goal/progress and directional guidance remain visible in bounded, wrapping pills.

All procedural touch glyphs were absent in the editor camera captures, while disc backgrounds and labels remained. Prior browser screenshots showed the same glyphs, so this is not yet evidence of a runtime regression. Audit now explicitly dirties glyph meshes after switching canvas projection/size and records per-glyph mesh vertex count, bounds, culling state and shader. Browser confirmation is still required; no unsupported claim that this rebuild resolves the issue.


Final integration verification: the 0.5.0 Unity art/UI audits and actual rendered captures passed review. The center-bay mullion fix is included in the final storefront captures. Chromium passes 16 end-to-end checks and Safari/WebKit passes 9 checks without runtime errors. See DESIGN-REVIEW-050.md for scope and remaining limitations.
