# Mobile browser build and touch verification

Target devices requested: iPhone 17 Pro Max and a recent iPad Pro, on the Mac's Wi-Fi. Physical-device verification remains pending; desktop Chromium and WebKit emulation are not substitutes for testing those devices.

## Access

Keep the Mac awake with the server running. On either device, open Safari in landscape and visit http://192.168.4.23:8765/ (fallback http://mac-mini-2.local:8765/). Tap **Play in browser**, then **New adventure**. The first download is approximately 114 MB uncompressed. Use the same address consistently: browser saves belong to that origin and device. The native Mac save is separate. Clearing website data removes browser progress.

Left pad moves and steers; drag the right side to turn the view. Jump responds when pressed and supports simultaneous movement. Search, Eat, Den, Home, pause and Center view have touch buttons. Rotating to portrait or losing application focus stops held input and pauses gameplay. Rotate back and select Back to Ballard.

## Automated controls

Editor tests: 15/15 passed, including independent pointer ownership, foreign-pointer rejection and jump-on-press while moving. Evidence: `PlaytestCaptures/mobile-editmode.xml`.

## Initial technical build evidence

Unity 6000.3.0f1 WebGL IL2CPP build succeeded, output `Builds/Web/index.html`, 105,992,102 bytes. Current first-build art is not final. Chromium at 1194 × 834 with touch enabled loaded the actual Unity title and accepted New adventure, Search, movement plus right-view drag, Jump, Home, Den and Unload pockets. The bagel discovery card rendered. Deposit changed the den to one banked find and one shiny. A full page reload showed Continue adventure, confirming a persisted browser save. Evidence: `PlaytestCaptures/web-touch/01-title.png` through `09-reload-continue.png`.

Final 0.4.1 WebGL build succeeded: 114,102,819 bytes, including final character assets. Targeted `link.xml` preservation removed the runtime SphereCollider errors; final Chromium console and page error lists are empty. The menu's narrow-aspect crop now retains the raccoon's head; inspected at 1194 × 834. Actual browser touch Search advanced the objective and bag count; Jump while moving made the controller airborne. Home → Den → Unload yielded one banked find and one shiny. A full reload and Continue recovered those exact values. Portrait resize paused and cleared inputs, and returning to landscape allowed resume. The 932 × 430 iPhone-shaped HUD was inspected: controls and text remain within screen bounds. Evidence: `PlaytestCaptures/web-final-touch/`, including read-only `states.json` and `errors.json`.

Final two-finger release retest passed: releasing the camera finger retained forward input at 0.765 and Jimothy continued moving. Jump also worked with movement held, and releasing all fingers cleared input. All nine browser assertions passed (`PlaytestCaptures/web-final-touch/assertions.json`). Physical iPhone/iPad performance, touch feel and Safari safe-area behavior remain unverified until the user tests the actual devices.

The browser wrapper sets a maximum 1.25 device pixel ratio to limit Retina fill cost. It uses landscape, safe-area CSS and disables browser gesture handling over the canvas. Unity UI also respects its safe area. Web-only save writes preserve a backup and Unity's `autoSyncPersistentDataPath` queues IndexedDB persistence. This is local browser persistence, not cloud sync or an instant-write guarantee.

## Installation provenance and build

The official Unity release API reported editor hash `d1870ce95baf`, matching the installed editor. Downloaded the ARM64 Web Support package from `https://download.unity3d.com/download_unity/d1870ce95baf/MacEditorTargetInstaller/UnitySetup-WebGL-Support-for-Editor-Arm64-6000.3.0f1.pkg`. Package MD5 matched the release manifest (`81fd8dc82830d0fb868a5884e6f96b4a`); macOS verified Unity Technologies' trusted Developer ID Installer signature. Extracted support files were installed into the external editor's PlaybackEngines/WebGLSupport. No Xcode installation was required.

Run Unity's `Jimothy.Editor.MobileWebBuild.Build` method to export to `Builds/Web`. Run `python3 Tools/serve_ipad.py --directory Builds/Web --port 8765` from the project root. Do not run a second server if one already owns that port.

Official documentation: [Unity 6.3 browser compatibility](https://docs.unity3d.com/6000.3/Documentation/Manual/webgl-browsercompatibility.html) lists iOS Safari 15+ and recommends current Safari; this build is served directly, avoiding Safari iframe IndexedDB restrictions. [Unity native iOS build process](https://docs.unity3d.com/6000.3/Documentation/Manual/iphone-BuildProcess.html) requires an Xcode build for native delivery. This delivery is the browser route, not an App Store or TestFlight installation.

Final desktop WebKit (26.5, touch enabled, 1366×1024 iPad-shaped viewport) passed loading, native canvas touch New adventure/Search, collected bagel count1, full reload and Continue recovering bag1, with zero console/page errors. Evidence BrowserPlaytests/webkit-final/verification.json and captures. This is not a claim of a physical iPad or iPhone run.

The local server is detached from the terminal. Restart if needed with `python3 Tools/start_ipad_server.py`; it detects an already-running game server. The Mac must remain awake and the external drive connected.


0.4.2 update: reload the same origin (http://192.168.4.23:8765/) to retain browser progress and load the new lighting/navigation build. The menu footer now shows the version. Chromium11/11 checks and an independent WebKit touch/Search/reload check passed. Physical-device performance still needs testing.
