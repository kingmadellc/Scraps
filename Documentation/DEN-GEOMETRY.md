# Physical den annex

`ClosingTimeDen.cs` builds a connected basement annex north of the final west-side building. The only district edits are the Home alias, four ground slabs around the excavation, and Den method delegation. Existing buildings and sidewalks are intact.

- StreetStart: `(-3, .05, 44)`; welcome snack `(-3, .20, 43)`.
- DenEntrance: `(-14, .05, 49)`, immediately outside the hole at z50.
- Ramp: 3 m wide, top plane from `(-14,0,50)` to `(-14,-2.4,56)`, approximately 21.8°.
- DenInterior / Home: `(-14,-2.35,59)`.
- Floor: y=-2.4; ceiling underside y=1.6.
- DenContains: x[-19.7,-8.3], z[56,63.2], y[-2.6,1.4].
- 32 DenDisplaySlots: 28 wall shelf surfaces, three coffee-table surfaces, one amplifier top. Anchors are the surface tops; item fitting is owned by DenCollection.

Original fictional gig flyers, a worn couch with flannel, guitar, amplifier, cassettes, geometric rug, and warm bare bulbs establish the basement music-room theme. The “Smells Like Dinner” flyer is an original playful nod. No existing album artwork, band logo, or song recording was added.

The obsolete street platform and its two separate post proxies are removed; the other 124 street furniture proxies remain.

## Actual Unity verification

`Jimothy.Editor.DenGeometryAudit.Run` passed 42/42 checks: ten continuous actual-motor waypoints from street to entrance, down the ramp, across the room/shelf approaches, and back to street; all 32 display support raycasts. Movement used no jump or teleport after initial test placement. This isolated unsaved world did not instantiate GameSession or access saves. Results: `Logs/den-geometry-audit.json`.

`Jimothy.Editor.DenArtAudit.Run` produced three actual URP renders through a fresh GameSession with saving suppressed: the rear gameplay camera inside the room, a separate room overview, and the rear gameplay camera outside the entrance. Images were visually reviewed. Art inspection uses explicit fixture placements; it is not a continuous gameplay test. Results: `PlaytestCaptures/den-room/`.

The gameplay rear view shows the central movement space and furniture; high posters extend above its top framing. The separate overview shows their full arrangement. Lighting is warm and intentionally dim. Integrated den UI, collection fitting, audio, save migration, and camera behavior throughout ramp transitions are handled by the parent and DenExperienceAudit.
