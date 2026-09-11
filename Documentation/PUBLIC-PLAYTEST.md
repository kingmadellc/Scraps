# Public browser playtest

https://kingmadellc.github.io/JimothySurvival/?v=0.5.1

The compiled 0.5.1 Web build is committed on `codex/playable-build`, separately from editable source on `main`. GitHub Pages publishes the branch root with `.nojekyll`. Game binaries are the tested local 0.5.1 files; the launcher network-error text now applies to public hosting instead of asking for a local Mac. Each binary is below GitHub's hard per-file size limit. This branch intentionally contains real files rather than LFS pointers so Pages can serve them.

Use Safari in landscape on iPhone/iPad and tap Play in browser. The initial payload is about 122 MB. The Mac and external development drive can be offline. Saves belong to this website origin and browser/device, with no cloud sync. Saves from the old LAN URL or native Mac build do not automatically transfer.

To update: copy the verified Web output to the publishing checkout on the external drive, preserve `.nojekyll`, commit and push to the publishing branch, then verify the Pages build, `build-info.json`, WebAssembly MIME type, and live game startup before sharing. Do not overwrite source `main` with compiled output.

Deployment verification: Pages reports built at `ffe9098557f001ac7fd04c9c63ae245a826b1d3f`; public metadata reports 0.5.1 and WebAssembly is served with application/wasm. The existing URL and save formats are retained. Browser saves remain specific to the browser and device.

Naming cleanup: browser and iOS home-screen titles are Scraps. The public path remains stable to preserve saved progress. Local Safari/WebKit passes 11 naming and gameplay checks; public metadata and Pages deployment verified.

Final public Safari/WebKit verification: all 11 checks pass with no runtime errors, including the Scraps browser/home-screen title, visible-name checks and save/resume.
