# Public browser playtest

https://kingmadellc.github.io/JimothySurvival/?v=0.4.9

The compiled 0.4.9 Web build is committed on `codex/playable-build`, separately from editable source on `main`. GitHub Pages publishes the branch root with `.nojekyll`. Game binaries are the tested local 0.4.9 files; the launcher network-error text now applies to public hosting instead of asking for a local Mac. Each binary is below GitHub's hard per-file size limit. This branch intentionally contains real files rather than LFS pointers so Pages can serve them.

Use Safari in landscape on iPhone/iPad and tap Play in browser. The initial payload is about 124 MB. The Mac and external development drive can be offline. Saves belong to this website origin and browser/device, with no cloud sync. Saves from the old LAN URL or native Mac build do not automatically transfer.

To update: copy the verified Web output to the publishing checkout on the external drive, preserve `.nojekyll`, commit and push to the publishing branch, then verify the Pages build, `build-info.json`, WebAssembly MIME type, and live game startup before sharing. Do not overwrite source `main` with compiled output.

Deployment verification: Pages reports built at `cd16c5045d903153d3dd1c9d44b52560b2968166`; public metadata reports 0.4.9. Public WebKit testing created a save under 0.4.8 and resumed it in Scraps 0.4.9 with the same active outing, seed and run number, and zero browser errors. The title screenshot confirms Scraps without a subtitle. The existing URL is retained to preserve browser saves.
