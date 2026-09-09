# Seeded den collection art fixture

This audit constructs an isolated NewGame with saving suppressed. It places Jimothy in the den, explicitly adds the following 22 catalog IDs to the test bag, and calls the production Deposit handler. These are seeded test finds, not the user’s inventory.

The intended views are the actual rear gameplay camera, a separate wide room overview, and a shelf detail. Collection models, placement, and lighting come from the running game. The fixture also checks displayed item count and vertical support alignment.

| ID | Catalog find | Rarity |
|---|---|---|
| trophy_00 | The First Salmon | legendary |
| trophy_01 | Marvin’s Lost Clapper | legendary |
| trophy_02 | Rainier at Dawn | legendary |
| trophy_03 | The Golden Bagel | legendary |
| trophy_04 | Captain’s Last Compass | legendary |
| trophy_05 | The Tiny Troll | legendary |
| trophy_06 | The Ballard Crown | legendary |
| trophy_07 | Ghost of the Old Trolley | legendary |
| trophy_08 | The Nordic Star | legendary |
| trophy_09 | Midnight Market Medal | legendary |
| item_208 | Mini Crab Pot | rare |
| item_211 | Ship Compass | rare |
| item_217 | Espresso Cup | rare |
| item_220 | Tiny Thermos | rare |
| item_224 | Postcard of Rainier | rare |
| item_228 | Bicycle Bell | rare |
| item_230 | Ceramic Raccoon | rare |
| item_231 | Wooden Seaplane | rare |
| item_232 | Mini Sailboat | rare |
| item_233 | Tin Lighthouse | rare |
| item_237 | Garden Lantern | rare |
| item_239 | Patchwork Cushion | rare |

## Captured and reviewed

`Jimothy.Editor.DenCollectionArtAudit.Run` completed successfully with production-style 2× MSAA. All 22 seeded items were displayed. Maximum measured vertical support alignment error was 0.000000155 m (floating-point noise); saving suppression was true. These checks concern rendered item bounds against their allocated shelf/table surface anchors, not an exhaustive inter-object collision audit.

Evidence in `PlaytestCaptures/den-populated/`:

- `den-00.png`: actual rear gameplay camera inside the populated room.
- `den-01.png`: separate wide room overview.
- `den-02.png`: shelf detail.
- `contact-sheet.png`: combined review image.
- `capture.json`: fixture IDs, camera positions, item count, and support error.

The textured couch/rug, reading lamp, posters, coffee-table trophies, and shelves create a coherent warm basement scene. All visible finds sit on their supports; none appeared suspended above or sunken into shelving. The wide view communicates a collection, but individual identities are less clear at gameplay distance. Several existing catalog models reuse token/ring shapes, which limits visual distinction even in the detail view. No item models or collection behavior were changed by this art audit.

Unity exited and the exclusive slot was released after review. Log: `Logs/den-populated-art.log`.
