# Rooftop living spaces

`ClosingTimeRoofLiving.cs` supplies `RoofPersonalization()`, called once before the world batches are baked. It initializes its own material keys. The obsolete generic `RooftopConnections.RoofLife` call is removed; the crossings and northern lookout are preserved.

| Roof | Authored use | Main details |
|---|---|---|
| West 0 / Net Loft | Yoga terrace | Three ribbed mats, cork blocks, reusable bottles, bench, blanket basket |
| East 0 / Copper Gull | Pizza and entertaining | Open-mouth masonry pizza oven, flue, peel, outdoor counter/bar, sink and tap, stools |
| West 1 / Cedar & Salt | Outdoor work and lunch | Six chairs, long slatted table, notebooks, enamel cups |
| East 1 / Old Avenue | Listening lounge | Cushioned bench, angled chair, low table, record cabinet and turntable |
| West 2 / Ballard Bakery | Artist terrace | Two easels with original color studies, worktable, brushes and stool |
| East 2 / Juniper Room | Community kitchen garden | Potting bench, terracotta pots, edible beds, trellises |
| West 3 / Last Cast | Neighbors' supper patio | Two bistro tables, colorful chairs, plates, cups and bottles |
| East 3 / Dockside Coffee | Chess and reading | Authored checkerboard and pieces, chairs and bench |
| West 4 / Salmon & Sons | Lookout field notes | Outer-edge bench, planted pot, field-notes board; existing lookout untouched |
| East 4 / Northwest Records | Slow-morning terrace | Two reclining loungers, coffee table and blanket basket |

Furniture generally occupies the outward/back-half zone: x=side×(14.6+u), u[0,4.2], local z[-4.9,0]. The street-facing vent/loot strip, chimney find, north roof access, and crossover approach ramps are reserved. The western northern roof uses only the outer strip beside the existing lookout.

Substantive objects have collision: table slabs, chair/bench components, cabinetry, planters, easels and oven masonry. Round table surfaces and the open oven dome use their actual low-polygon mesh for collision. Decorative tabletop objects and textile surfaces do not add tall blocking volumes. Each roof has one small shadowless warm lantern with the existing distance fade; no continuously simulated activity actors were added.

Validation status: authored and handed to the parent's environment render and onboarding route/placement audits. Results will be recorded after visual review. No performance benchmark claimed.
