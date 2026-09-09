# Surface texture provenance

Downloaded 2026-09-07 from the official Poly Haven public asset API and asset download host. These are source PBR maps, not website preview renders.

License: **CC0 1.0 Universal**. Poly Haven licenses its texture assets for commercial use and redistribution without attribution requirements. Verified official license: https://polyhaven.com/license ; legal deed: https://creativecommons.org/publicdomain/zero/1.0/

All files are original 2048×2048 JPEG downloads, bytes and MD5 verified against the official API. OpenGL normals (+Y). Roughness is linear scalar data, not Unity smoothness; invert when packing smoothness.

| Set | Source / author | Width per tile | Unity UV scale for metre UVs |
|---|---|---|---|
| Brick | https://polyhaven.com/a/red_brick_03 — Rob Tuytel | 1 m | 1 |
| Asphalt | https://polyhaven.com/a/asphalt_02 — Rob Tuytel | 3 m | 0.333333 |
| Wood | https://polyhaven.com/a/rough_wood — Rob Tuytel | 0.5 m | 2 |

## Exact source downloads

- `Brick_BaseColor.jpg`: https://dl.polyhaven.org/file/ph-assets/Textures/jpg/2k/red_brick_03/red_brick_03_diff_2k.jpg — MD5 `df521ebe6c4eb72c3329832acfecd95d` (2969950 bytes)
- `Brick_Normal.jpg`: https://dl.polyhaven.org/file/ph-assets/Textures/jpg/2k/red_brick_03/red_brick_03_nor_gl_2k.jpg — MD5 `fd2d01b5e0f4de51ca4d7c2b4bcbc22c` (3684630 bytes)
- `Brick_Roughness.jpg`: https://dl.polyhaven.org/file/ph-assets/Textures/jpg/2k/red_brick_03/red_brick_03_rough_2k.jpg — MD5 `18815a721bf7cb485955359204484608` (1329161 bytes)
- `Asphalt_BaseColor.jpg`: https://dl.polyhaven.org/file/ph-assets/Textures/jpg/2k/asphalt_02/asphalt_02_diff_2k.jpg — MD5 `336af399fd98a39ab986d8b3bf73b4ff` (3075676 bytes)
- `Asphalt_Normal.jpg`: https://dl.polyhaven.org/file/ph-assets/Textures/jpg/2k/asphalt_02/asphalt_02_nor_gl_2k.jpg — MD5 `77ebd1cc0b020ccaa1c6b58f103d1f75` (4943950 bytes)
- `Asphalt_Roughness.jpg`: https://dl.polyhaven.org/file/ph-assets/Textures/jpg/2k/asphalt_02/asphalt_02_rough_2k.jpg — MD5 `6fe669ab38640ef2009d6a28d1ad5ee9` (2230457 bytes)
- `Wood_BaseColor.jpg`: https://dl.polyhaven.org/file/ph-assets/Textures/jpg/2k/rough_wood/rough_wood_diff_2k.jpg — MD5 `f5f9c55696f047ceaa49dc97c5aa5526` (2223327 bytes)
- `Wood_Normal.jpg`: https://dl.polyhaven.org/file/ph-assets/Textures/jpg/2k/rough_wood/rough_wood_nor_gl_2k.jpg — MD5 `62594f2d00eb246581b9d4fde30209f1` (2520021 bytes)
- `Wood_Roughness.jpg`: https://dl.polyhaven.org/file/ph-assets/Textures/jpg/2k/rough_wood/rough_wood_rough_2k.jpg — MD5 `832d6d549a8e76c16b9af2586f429d6c` (810338 bytes)

API metadata endpoints: https://api.polyhaven.com/files/red_brick_03 ; https://api.polyhaven.com/files/asphalt_02 ; https://api.polyhaven.com/files/rough_wood

No geometry displacement is required on mobile. Use restrained normal strength (~0.35–0.6) and preserve shared materials.
