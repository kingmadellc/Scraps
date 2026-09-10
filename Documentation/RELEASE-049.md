# Scraps — 0.4.9

Selected by the user on September 10, 2026: **Scraps**, with no subtitle. The title wordmark, gameplay HUD, browser launcher/tab title and rotation prompt use the new name. The old Survival line and title/launcher tagline are removed. The Mac application is now `Builds/Scraps.app`; native product metadata uses Scraps.

Save compatibility is deliberate: the established Mac save directory and filename, Mac application identifier, iOS bundle identifier, company name, and public website URL remain stable. The Mac save-store directory is explicitly pinned to the old folder rather than following the new display name. Web assets carry version query parameters so old browser caches do not combine the new launcher with old binaries. Internal asset paths and code namespaces remain unchanged.

Public play: https://kingmadellc.github.io/JimothySurvival/?v=0.4.9

Verification: read-only editor compatibility audit passes for the product name, Mac save path and both native bundle identifiers. Public WebKit testing created a save in 0.4.8, resumed the same active outing in 0.4.9, and verified the seed and run number persisted with zero browser errors. The supplied browser client completed New → Head out → jump in 0.4.9; gameplay state and screenshots were inspected. Mac and Web builds succeeded; Mac code signing verification passed. No trademark clearance is implied by the naming decision.
