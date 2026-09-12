# Scraps: the first wardrobe milestone

The first slice of the approved raccoon-RPG direction is a Den wardrobe. Choose a look, turn the real 3D portrait, choose Ash/Cedar/Midnight fur, and wear unlocked outfits. Appearance saves immediately and returns after a reload. One outfit is equipped at a time; cosmetics never alter collision, speed, detection or jump distance.

| Look | Earn it | Character |
|---|---|---|
| Birthday fur | Starter | Unlicensed, unbothered, unclothed |
| Dumpster headliner | Starter | A tied bandana for a one-dumpster world tour |
| Trash royalty | Bank three different finds | Crown made from crimped bottle caps and bent tabs |
| Back-alley roadie | Bank a music keepsake | Cassette satchel with reels, label, handle and straps |
| Harbor menace | Bank a harbor keepsake | Folded newspaper boat hat |
| Municipal inspector | Complete four banked outings | Scavenged high-visibility gear and a questionable badge |

The wardrobe previews locked looks without granting them. Pocket finds do not unlock outfits; banked discoveries do. Death keeps banked cosmetic unlocks and the selected appearance. Existing saves gain a default coat and no outfit. Unknown/locked saved outfit IDs fall back safely. No items or currency are consumed for equipping a look.

The existing three local night-event rotations now carry the names Unauthorized album launch, The Great Fish Incident, and Guerrilla gardening. Their existing watcher/loot rules remain readable in the outing rules. These are local run events, not a server-managed live service.

## Art and implementation

Each accessory has its own authored procedural mesh construction, attached to the existing head/body bone. Parts batch by material. No accessory physics or new transparent fur layers. The wardrobe owns one 512×512 portrait target, a temporary model and two portrait-only lights; it releases them when closed. The gameplay camera excludes the portrait layer. The base character and animation rig are preserved.

This milestone does not include independent mask/ear/tail editing, freeform naming, multiple simultaneous clothing slots, multiplayer, account sync or a live-operations backend. Physical-device thermal performance is not yet measured. Continue character face/coat polish alongside wearable art; avoid hiding those remaining needs with accessories.

## Next slices

1. Add a physical clothes rail to the Den, show collected outfits on hooks, and expand into separate hat/neck/back slots after testing clipping combinations.
2. Introduce an NPC raccoon collector with a short, repeatable trade loop. Give it a distinct silhouette and a fixation on apparently worthless junk. A possum acting as an unqualified accountant is a possible later character.
3. Add authored event props and changing objectives: a tipped fish delivery, a cleanup after a rooftop gig, a mysteriously migrating garden gnome. Keep two dependable escape routes through every event.
4. Test whether players pursue specific wardrobe/Den rewards over several outings. Observe real player choices before increasing grind or catalog size.
5. Build shared event scheduling and account/cloud saves before community objectives. Friend Den visits and cooperative outings are later networking milestones.

## Validation

Twelve rules assertions cover banking-only unlocks, distinct finds, loss, repeat banking, old-save defaults, invalid selections and appearance serialization. The extended Den integration test exercises actual equip APIs, locked-item rejection, no active accessory colliders, physical routes and reconstructed appearance. Fourteen phone/tablet renders cover all six looks and the Den menu. The extended Den integration suite passes 232 assertions. Safari/WebKit passes 18 touch/branding/wardrobe/gameplay checks, including selecting a coat, equipping the bandana, previewing a locked crown without unlocking it, resuming after a browser reload, jumping and orientation safety, with no runtime errors. Actual wardrobe images were inspected. Physical-device performance remains unmeasured.

Build recovery: WebAssembly link initially failed with ENOSPC in the system temporary directory. MobileWebBuild now puts Emscripten scratch output in project-adjacent Temp/Emscripten (ignored by Git). Test browser binaries were copied and hash-verified on the external drive; their original cache path is a symlink.

Final local checks: supplied Chromium skill client completed and its gameplay screenshot/state were inspected. Native Mac build succeeded and passed strict code-signature verification. The public Pages build is e96fbc9a08928140a22261a5955c50171c95d4b4. This milestone leaves the larger city/trader/live-event expansion on the roadmap above.

Public0.6.0 Safari/WebKit verification passes18/18, including wardrobe equip,coat,locked look,save/reload and jump.
