# Public browser playtest

https://kingmadellc.github.io/JimothySurvival/?v=0.4.8

The compiled 0.4.8 Web build is committed on `codex/playable-build`, separately from editable source on `main`. GitHub Pages publishes the branch root with `.nojekyll`. Game binaries are the tested local 0.4.8 files; the launcher network-error text now applies to public hosting instead of asking for a local Mac. Each binary is below GitHub's hard per-file size limit. This branch intentionally contains real files rather than LFS pointers so Pages can serve them.

Use Safari in landscape on iPhone/iPad and tap Play in browser. The initial payload is about 124 MB. The Mac and external development drive can be offline. Saves belong to this website origin and browser/device, with no cloud sync. Saves from the old LAN URL or native Mac build do not automatically transfer.

To update: copy the verified Web output to the publishing checkout on the external drive, preserve `.nojekyll`, commit and push to the publishing branch, then verify the Pages build, `build-info.json`, WebAssembly MIME type, and live game startup before sharing. Do not overwrite source `main` with compiled output.

Deployment verification: Pages reports built at670fa0966df20c122a4a61080419c4d0ad9d4469; public metadata reports0.4.8 and wasm returns application/wasm. The live public WebKit phone smoke passed9/9with no runtime errors, including touch jump and save/reload. Actual public gameplay screenshot reviewed.
