# Storefront design pass — 0.5.0

## Review findings

Reviewed existing source and the actual bakery/coffee frames in `PlaytestCaptures/environment-v044`. Businesses already had distinctive fascia typography, canopy construction, and basic merchandise. The dominant unfinished quality was repetition: three nearly identical stocked windows per business, the same display rhythm, very large doorway badges, and little everyday entry information. The fish counter also used copper-colored fish despite its silver-salmon description.

## Implemented

Each business retains its main merchandise display. Its middle bay is now a different closing-time work vignette:

- Marine supply: harbor chart, splicing tools, brass bench cleat.
- Kitchen: chopping board, herbs, stacked service plates and towel.
- Bakery: flour sack, covered proofing baskets and rolling pin.
- Fishing pub: drying cloth and inverted glasses.
- Fishmonger: dial scale and an empty raised-edge catch tray; display fish use the existing pale ceramic material instead of copper.
- Copper tavern: covered cellar crocks and ledger.
- Billiards: bead scoring rail and folded table cover.
- Botanical bar: small propagation jars and fresh cuttings.
- Coffee: pour-over stand, servers and a gooseneck kettle.
- Records: listening turntable, tonearm and upright sleeve.

The third stock window now has partly lowered slatted blinds and a pull cord. These let the retained inventory read as a closing shop rather than another identical showroom. Doors now carry distinct street addresses and appropriate next-opening/service information, with mail slots and hinge leaves. Doorway emblems are reduced from roughly .72 m to .44 m wide, leaving the entry information more space.

All changes use existing materials and batching. No new textures, lights, physical colliders, traversable surfaces, or broader facade projections were introduced. Replacing a complete duplicate merchandise bay removes its geometry before adding the vignette. Notice labels replace the former middle-bay slogan; address/opening information replaces the former CLOSED text, so those text-renderer counts do not increase. Exact final triangle/draw counts still require the coordinated build; no native/mobile performance claim is made.

Files changed: `ClosingTimeStorefronts.cs`, `ClosingTimeShopSigns.cs`. No shared world hook or broad geometry change is needed.

## Validation needed

`Jimothy.Editor.StorefrontDesignAudit.Run` builds the real district in an isolated scene, checks ten unique doorway labels, ten distinct working-window notices, label bounds, actual street approach/sightline clearance, missing shaders, and preserved route counts. It writes `Logs/storefront-design-audit.json` and self-exits in batch mode. It has not been run by this agent.

Root should run the audit and existing navigation/scavenge/route checks, then rerender the ten storefront views using `EnvironmentArtAudit`. Review the new notice readability, sleeve/record depth, shelf support, blind/mullion separation, and doorway badge proportions. The older captures establish the initial problem; they do not validate these new changes. No Unity, Blender, browser or Git was launched during this task.

Final image review covered all ten shops. The reviewer caught middle-bay notices bisected by the retained mullion. Root removed that mullion only from the new work-vignette bay, preserving the other windows and all colliders. Final capture verification follows.


Final integration verification: the 0.5.0 Unity art/UI audits and actual rendered captures passed review. The center-bay mullion fix is included in the final storefront captures. Chromium passes 16 end-to-end checks and Safari/WebKit passes 9 checks without runtime errors. See DESIGN-REVIEW-050.md for scope and remaining limitations.
