# Scraps naming cleanup — 0.5.1

The player-facing title remains Scraps without a subtitle. Removed the extra location line from the title composition. Browser title, launcher heading, iOS home-screen app title and canvas accessibility label explicitly use Scraps. Unity authoring menus and character inspection labels no longer present the former name. The hero illustration has no embedded name.

Legacy asset filenames/namespaces, native bundle identifiers, save paths and the existing public GitHub Pages URL remain for compatibility. This avoids stranding existing saves or breaking resource references; this pass does not erase historical source or rename the repository.

## Legal scope

This is a branding cleanup, not trademark or likeness clearance. The Copyright Office distinguishes a protected photograph from its subject; that does not grant permission to reproduce protected artwork. USPTO guidance addresses confusion about source or sponsorship. No claim is made that a subtle likeness cannot lead to a lawsuit, or that Scraps itself has been cleared. Obtain an IP attorney's assessment of the final character, reference-derived artwork, title and marketing before commercial launch.

Sources checked September 11, 2026:
- https://www.copyright.gov/help/faq/faq-protect.html
- https://www.uspto.gov/page/about-trademark-infringement

## Validation

Web build succeeds. Safari/WebKit passes 11 checks covering browser/home-screen branding, visible UI name screening, settings, outing, touch jump, save/resume and orientation. Title and settings screenshots inspected. One initial browser process closed during the test; a fresh run passed without errors. Native Mac build succeeds and passes strict code-signature verification. Supplied Chromium skill client completed in headless Metal mode; actual gameplay capture and 0.5.1 state inspected. The first headed Chromium capture timed out, so that run is not counted as a pass. Public Pages build ffe9098557f001ac7fd04c9c63ae245a826b1d3f and 0.5.1 metadata verified.

Build-machine maintenance: copied and hash-verified the Puppeteer browser cache and uv download/environment cache to the external development drive, then preserved their original paths using symlinks. No project files or saves were removed.

Final public Safari/WebKit verification: all 11 checks pass with no runtime errors, including the Scraps browser/home-screen title, visible-name checks and save/resume.
