# Jimothy Survival title direction

Implemented September 7, 2026. Scope: the main title page only. Gameplay HUD and its typography are unchanged. Native build/visual capture is coordinated by the parent agent; this agent did not launch Unity or claim native visual verification.

## Design

The wordmark combines a substantial slanted Fugaz One “Jimothy” with a widely spaced, condensed SURVIVAL subtitle. Cream faces, copper extrusion, a dark keyline, and short parallel strokes suggest a vintage hand-painted travel emblem without copying a vehicle or game's logo. The title is intentionally distinct from the game's system font. Ballard/Seattle and the small-paws tagline ground the identity in place and character.

The full-bleed existing MenuHero-v2 remains the primary character portrait. A continuous feathered dark scrim replaces the abrupt half-screen panel; aspect-fill UV cropping avoids stretching the hero on wider landscape displays. The menu has a smaller amber primary button, restrained secondary buttons, and readable hover/selected/pressed/disabled states. Save files get Continue adventure as the primary action; New adventure still asks before replacing existing progress. Without a save, Load adventure is visibly disabled with a short explanation.

Fonts only load through title helpers. Main menu settings navigation retains the game's existing Settings page. No new page/interstitial or external link obstructs play.

## Reference research

- Honda's official Super Cub emblem gallery: https://global.honda/jp/Cub/history/emblem/2017_super-cub-50_2/ — vintage slant/emblem treatment. Used only as stylistic reference; no Honda graphic, logo, typeface, or trademark embedded.
- Celeste official site: https://www.celestegame.com/ — strong title identity with a clear place/character mood. Official branding/reference research, not a claim to have captured a live game menu.
- Stray official publisher site and logo: https://stray.happinet-games.com/index.html and https://stray.happinet-games.com/00_index/images/stray_logo.webp — character-first key art and a distinct simple wordmark. No artwork copied.
- Nintendo's official Super Mario Odyssey site: https://supermario.nintendo.com/ and https://www.nintendo.com/en-gb/Games/Nintendo-Switch-games/Super-Mario-Odyssey-1173332.html — browsed for title/key-art hierarchy.
- Studio MDHR official about page: https://studiomdhr.com/about-us/ — primary reference for Cuphead's stated vintage-cartoon roots. Did not copy a Cuphead asset or claim direct title-screen inspection.

Browser control was unavailable. Research used web primary pages/official linked imagery. References informed design choices; none were imported into this project.

## Font provenance / redistribution

Unmodified font files downloaded from Google Fonts' official repository:

- Fugaz One Regular: https://github.com/google/fonts/tree/main/ofl/fugazone — copyright 2011 LatinoType Limitada. SIL Open Font License 1.1, reserved name “Fugaz One.”
- Barlow Condensed SemiBold: https://github.com/google/fonts/tree/main/ofl/barlowcondensed — copyright 2017 The Barlow Project Authors. SIL Open Font License 1.1.

Both original OFL texts sit beside the fonts under Unity/Assets/Jimothy/Resources/TitleFonts and are included as Resources text assets in builds. Font binaries are unmodified; only Unity importer metadata was added. No additional font purchase or attribution screen is required by the OFL. Retain the licenses when redistributing the fonts.

## Validation

Measured Fugaz One's “Jimothy” at 126px with font metrics: 526px advance; wordmark container is approximately 612px wide at reference 1440×810. Title text uses overflow rather than clipping italic extents. Buttons remain native uGUI Button components, preserve actions and save-replacement confirmation, and maintain desktop/mobile readable type sizes. Parent must inspect final native rendering and pointer/navigation behavior after Unity imports new resources.

## Direct menu screenshot comparison (parent follow-up)

The parent subsequently inspected actual Celeste and Stray menu screenshots in the browser:
- https://interfaceingame.com/screenshots/celeste-main-menu/ — Celeste groups four short actions at the left, uses a bright selection color, and lets the mountain dominate the remaining composition. Applied here: a compact action column and strong active color, leaving Jimothy unobstructed.
- https://www.mobygames.com/game/188150/stray/screenshots/ — Stray uses a distinctive character-related wordmark, abundant dark space and three restrained actions. Applied here: a custom title identity and limited options rather than HUD-like panels. Jimothy uses warmer Ballard colors and more humorous hero imagery to fit this game.

These images were viewed for comparison only; no reference screenshot or logo was downloaded into the game.
