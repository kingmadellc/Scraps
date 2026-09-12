# Scraps 0.7.0: a collector moves in

Crimp occupies a lamp-lit counter inside the Den. Walk up and use Trade, or choose Crimp from the Den menu. This short, stocky raccoon trades apparently worthless banked goods for shinies. The adjacent clothes rail displays unlocked looks on hooks and opens the wardrobe when approached.

## The outing-to-trade loop

Bank a haul, then make one trade. Requests rotate after each successful trade:

| Request | Reward |
|---|---|
| Two pantry snacks | 8 shinies |
| One banked valuable | 12 shinies |
| One banked curio | 10 shinies |

The trade page shows exact payment items before purchase. Matching pantry items are selected by lowest value, then item ID. Pocket loot and trophies cannot pay. Another banked outing refreshes eligibility; waiting or reloading does not. Three completed trades permanently display the Golden Sardine on its own shelf. Later trades continue the three-request cycle.

Spending a banked item removes that owned copy and refreshes Den displays, but preserves discovery history and earned wardrobe unlocks. Favorite status does not protect a pantry item from the displayed payment. Trade reputation survives death and save reload. Legacy saves start at zero trades and retain existing inventory and appearance. Old saves with previous banked outings can make their first trade immediately if they have payment.

## Implementation and validation

The counter, lamp, rail and reward use material-batched procedural meshes. Crimp reuses the raccoon rig with distinct proportions and a newspaper hat. Accessories on hooks reuse wardrobe mesh construction without active character controllers. New scenery does not change player abilities. The counter has a solid collider; Crimp is stationary and is not an enemy.

17 pure trade-rule assertions pass, including insufficient payment, stale offers, duplicate callbacks, category restrictions, serialization, legacy defaults and permanent reputation. The Den integration audit passes 248 assertions including real movement to both stations, actual trade/equip APIs, exact payment, save reconstruction and native-save preservation. Eight phone/tablet UI views and six Den views were rendered; the rail was raised to clear shelves, signs were corrected to face the room, and counter lighting added after visual review.

Browser, native build and public deployment verification will be recorded below. These automated checks do not measure physical-device thermals or human enjoyment.

## Next slices

Test the request pacing with human players before increasing trade requirements. Add authored night-event props/objectives and give each a dependable street and rooftop escape. Separate wearable slots require clipping checks across combinations. Shared events, cloud saves and visiting friends' Dens remain later networking work.

Local0.7.0 browser validation: Safari/WebKit21/21 and Chromium21/21 pass with zero runtime errors. Chromium used trusted touch input for two physical Den/avenue roundtrips, searched fresh loot, banked twice, paid two snacks for8shinies, blocked repeat trade and reloaded the actual browser save with ledger/currency preserved. Final Web payload122075711bytes. Phone/tablet counter screenshots inspected.

Final local0.7.0: supplied Chromium skill client completed New→Head out→jump; gameplay screenshot and read-only state inspected, no browser errors. Mac build succeeded and strict codesign verification passed. Publishing final compiled Web output.

Published0.7.0 Pages build3a8bce833f5d3241c6038e141bfa465278c709cc verified live. Public Safari/WebKit passes21/21 with zero runtime errors, including collector eligibility, wardrobe, touch jump and save/resume; actual public screenshots inspected. Play: https://kingmadellc.github.io/JimothySurvival/?v=0.7.0. Physical-device performance and human enjoyment remain unmeasured.
