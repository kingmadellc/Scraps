# Public browser playtest

https://kingmadellc.github.io/JimothySurvival/?v=0.5.0

The compiled 0.5.0 Web build is committed on `codex/playable-build`, separately from editable source on `main`. GitHub Pages publishes the branch root with `.nojekyll`. Game binaries are the tested local 0.5.0 files; the launcher network-error text now applies to public hosting instead of asking for a local Mac. Each binary is below GitHub's hard per-file size limit. This branch intentionally contains real files rather than LFS pointers so Pages can serve them.

Use Safari in landscape on iPhone/iPad and tap Play in browser. The initial payload is about 122 MB. The Mac and external development drive can be offline. Saves belong to this website origin and browser/device, with no cloud sync. Saves from the old LAN URL or native Mac build do not automatically transfer.

To update: copy the verified Web output to the publishing checkout on the external drive, preserve `.nojekyll`, commit and push to the publishing branch, then verify the Pages build, `build-info.json`, WebAssembly MIME type, and live game startup before sharing. Do not overwrite source `main` with compiled output.

Deployment verification: Pages reports built at `66d7aea5c78eb8d9f1fe92de0cc048085023ba0c`; public metadata reports 0.5.0 and WebAssembly is served with application/wasm. The existing URL and save formats are retained. Browser saves remain specific to the browser and device.

Published 0.5.0: public Safari/WebKit verification passes all 9 checks with no browser errors, including touch jump, save/resume and orientation pause. Public gameplay/outing captures inspected. Pages build: 66d7aea5c78eb8d9f1fe92de0cc048085023ba0c. Play: https://kingmadellc.github.io/JimothySurvival/?v=0.5.0
