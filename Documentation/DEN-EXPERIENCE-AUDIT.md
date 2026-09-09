# Den UI and integration audit

HUD travel is **Den**. The separate inventory/decoration action is **Stash**; away from the den, it explains that the player should visit the den. The desktop shortcut hint is **H den**, matching the actual H binding. Closing Stash says **Back to den** and resumes without teleporting.

`Logs/den-experience-audit.json` reports219 passing assertions. This includes real CharacterController movement from the street through the descending entrance to the interior and back, without jumping or teleporting during those walks. The production follow camera was sampled441 times during ramp/interior movement; a0.07m probe at the camera remained clear of world collision.

An explicit inventory fixture exceeding display capacity exercised Stash/deposit and collection selection. Exactly32 unique owned IDs were chosen, with the trophy first; selection did not mutate inventory. Every selected ID had an actual authored mesh, correct shelf-height placement and a supporting shelf collision ray. JSON serialization followed by the same Begin reconstruction used during load restored identical display order and pantry contents. Because saving was suppressed, the fixture explicitly captured the player's current position before serialization, matching the normal Save behavior. Native main, backup and temporary saves remained byte-for-byte unchanged.

`Logs/scavenge-den.log` confirms the new den still leaves all22 find identities with63 supported, reachable placements. The welcome snack's approach now uses StreetStart, keeping onboarding on the avenue.

This is an Editor integration audit, not a human playtest, physical iPhone/iPad verification, or disk-save reload test. The new den's compiled native/browser presentation still requires the coordinated build review. Root owns the interior/display implementation and user-facing GameSession text; this pass changed GameUI labels and added integration validation.

## Compiled 0.4.3 verification

Native mobile-layout playtest passed: actual pixel Den/Stash/Unload/Back to den navigation, deposited bagel visible on the coffee table, and steering plus no-jump ramp exit to the street. All10 captures reported saving suppression and zero button overlaps; no runtime/shader errors were found. Test process quit. Evidence: `AgentPlaytests/v043-den/report.md` and frames0–9.

WebKit0.4.3 passed7/7 isolated touch/browser assertions. Runtime version was verified, Den/Stash/deposit rendered correctly, and a complete reload/Continue retained the interior location, banked find and currency. No browser runtime errors. Test browser closed. Evidence: `PlaytestCaptures/web-v043-den/report.md`. Physical iPhone/iPad verification remains pending.
