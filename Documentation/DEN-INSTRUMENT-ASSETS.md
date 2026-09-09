# Custom den instruments

Original, unbranded Blender assets authored by `Tools/build_den_instruments.py`. No downloaded mesh, logo, or texture source was used.

| Asset | Width × height × depth | Triangles | Meshes | Material slots |
|---|---|---:|---:|---:|
| Guitar | .357 × 1.100 × .087 m | 16,528 | 1 | 5 |
| Amp | .655 × .582 × .317 m | 7,332 | 1 | 7 |

The single-cut guitar has a beveled butterscotch body, tapered maple neck/fingerboard, 22 equal-tempered frets, six strings continuing to six inline tuners, position markers, black pickguard, neck and slanted bridge pickups, six bridge saddles, control plate/knobs/selector, strap button, and side jack. The amp has a rounded open-front cabinet, recessed woven grille, piping, controls/input sockets, raised carrying handle, folded corner guards, rubber feet, and rear ventilation slots.

Both exports use a ground origin and metre dimensions. Blender fronts face -Y; FBX settings are forward=-Z, up=Y. Runtime integration composes a 180-degree room-facing rotation with the imported FBX axis conversion. Unity bounds, material bindings and rendered front-facing instruments were verified in DenPolishAudit and DenPolishArtAudit.

Materials shared across assets: `Den_BlackPlastic`, `Den_BrushedNickel`, `Den_AgedIvory`. Guitar adds `Den_Butterscotch` and `Den_Maple`. Amp adds `Den_CharcoalTolex`, `Den_WovenGrille`, `Den_PatinaBrass`, and `Den_Rubber`. The four authored 512px BaseColor textures are stored alongside FBXs and packed in both FBXs and the blend. Preserve texture assignments when converting imported materials to URP; nickel/brass are metallic materials.

Source: `Art/Blender/DenInstruments.blend` (arranged review scene; individual exports retain ground origins).
Exports: `Unity/Assets/Jimothy/Resources/DenAssets/Guitar.fbx`, `Amp.fbx`.

## Verification

Final Blender renders were inspected after correcting an over-curved early neck and replacing ball-like amp corners with folded guards. `Art/Renders/den-instruments.png` shows the complete pair; `den-guitar-close.png` and `den-amp-close.png` show hardware and surfaces.

Both final FBXs were copied to a temporary folder and reimported into fresh Blender scenes. Each produced exactly one mesh, recovered its embedded texture images, retained its dimensions, and had ground-height error below 0.00000002 m. Machine reports: `den-instruments.json` and `den-instruments-fbx-check.json`. This verifies FBX structure and texture embedding, not Unity shader appearance.
