# Validation status

## Completed

- Executed Blender 4.5 in background mode to create `.blend` sources, binary FBX exports, native renders and keyed clips.
- Reopened character and environment sources in Blender and checked the skeleton, clip names/ranges, animated limb matrices and collision mesh inventory. See `blender-validation.json`.
- Parsed all authored C# files with tree-sitter C#; no syntax errors. This is not C# type checking or Unity compilation.
- Validated 256 unique item IDs, 16 legendary trophies and positive nutrition for food.
- Checked authored climb-step rises against the controller's theoretical 1.86 m jump apex. This is not collision or route playtesting.
- Inspected character three-quarter/side renders and the district overview. Earlier character renders were rejected by the user; v4 is another review pass, not an approved final asset.
- Round-tripped the v4 FBX through Blender: one mesh, eleven bones, eight materials, UVs, all vertices weighted and three animation actions. See `character-fbx-roundtrip.json`. This does not validate Unity import.
- Baked a native 2048×2048 character albedo from original vertex-painted markings.

## Not run

Unity package resolution/compilation, EditMode tests, PlayMode/gameplay, touch/controller input, camera collision, save/backup filesystem behavior, animation import, route traversal, NPC behavior, phone frame-time/thermal/memory measurements, cloud sync, iOS export, signing or App Store review.

Unity and full Xcode are not installed. The initial machine had only approximately 5.7 GB free, insufficient for the complete toolchain. No claim of a playable or shipping-validated build is made.

## Included Unity tests (authored, not executed)

Offline timer cap without starvation; backwards clock handling; rejection of invalid/future save data; catalog uniqueness/trophy count; JSON collection round trip. Run through Unity's EditMode Test Runner after editor setup.

## Visual limitations

The Blender groom is an offline render asset. The exported character uses a shaded, baked-color mesh and still needs optimized fur cards/LODs and final motion polish. Fine fur on the offline model should not be mistaken for measured mobile rendering performance. The menu illustration is AI-generated concept art at 1672×941, below the requested minimum, and is not an in-game screenshot. The district, NPCs and pickup geometry remain prototype art.
