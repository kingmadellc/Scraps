# Isolated agent playtest protocol

This bridge exists only in Editor or **Development Build** players. It starts a fresh unsaved session, never loads the user's saved game, and sets `GameSession.SuppressSaving` before creating it and on every driver update. Each tester must use a separate process and an empty, unique absolute session folder.

Launch the actual Mac executable (substitute the built application path):

```sh
"/absolute/Jimothy.app/Contents/MacOS/Jimothy: Small Paws, Big Appetite" --agent-playtest-dir "/absolute/tester-one" -logFile "/absolute/tester-one/player.log"
```

For simulated mobile HUD layout, add `--agent-mobile-ui`. That switches the layout before the scene's UI initializes and captures at **1688 × 780**. Ordinary captures are **1280 × 720**. These remain Mac player tests, not iPhone performance or physical touch tests. The process runs in the background with a 30 FPS target. `-batchmode` may be used with graphics enabled; do not use `-nographics` because actual rendered frames are required.

Wait for `ready.json`. A new session starts automatically at home. Write one `cmd.json` and wait for a matching ID in `result.json` before writing another command. IDs must strictly increase. Use a temporary file and rename it to `cmd.json` so the player cannot read a partial write. The player checks for changes approximately every 0.1 seconds.

```json
{"id":1,"action":"wait","frames":30}
```

```json
{"id":2,"action":"move","inputX":0,"inputY":1,"lookX":0,"lookY":0,"jump":false,"frames":30}
```

```json
{"id":3,"action":"move","inputX":-1,"inputY":0.4,"jump":true,"frames":24}
```

Movement uses **rear-follow steering**: positive Y forward, negative Y back, positive X turns right and negative X turns left. X alone rotates Jimothy; it does not strafe. The vector is limited to unit length. `jump:true` requests a jump only on the command's first frame. `lookX`/`lookY` apply camera-look pixels per frame (limited to ±80). Commands run for 1–180 actual game frames, then allow two neutral-input sampling frames before reading the most recently rendered camera target. The result identifies these as `captureSettleFrames: 2`. Movement clears between commands. Physics and survival continue while the process is running; a wait or screenshot does not freeze the world. Use pause when taking a long break.

Actions:

| Action | Behavior |
|---|---|
| `new` | Fresh isolated session; existing save stays untouched |
| `move` | Movement/look and optional first-frame jump |
| `wait` | Observe for the requested frames |
| `search` | Search the currently nearby, available find |
| `eat` | Eat available food |
| `home` | Fast travel if gameplay permits |
| `den` | Open the den screen if at home |
| `deposit` | Bank pockets if at home |
| `buy_cushion` | Buy a cushion if the normal conditions are met |
| `resume` | Resume gameplay |
| `pause` | Toggle pause |
| `menu` | Open the main menu |
| `click` | Dispatch actual uGUI pointer events at `clickX`, `clickY` in capture pixels, bottom-left origin |
| `quit` | Write the final result and quit |

Search/eat/home/menu/den and purchase commands invoke the public game actions. **They do not click UI buttons.** Use `click` at a reported button center to test actual uGUI raycast/pointer hit testing. This is synthetic UI input, not physical touch. Button rectangles and screenshots establish layout, labels and overlap. Do not click Load or change persistent settings in isolated tests. Movement commands drive the real motor input; there is no teleport command and the results expose no hidden routes or waypoints.

Every completed command produces:

- `frame-ID.png`: real game camera and UI, including URP postprocessing.
- `result.json`: matching ID/action, actual position, health/fullness, objective and guidance visible in HUD, nearby find name, inventory display names, coins, collection/decor, session state, elapsed seconds, screenshot path and any error.
- Button bounds in capture pixels, with a **bottom-left origin**, enabled state, and count of pairwise rectangle overlaps. Bounds are measured from rendered RectTransforms. No DPI/physical-size or successful-touch claim is implied.

An unavailable action can legitimately do nothing under the game's normal rules; inspect the resulting state and screenshot. A nonempty `error` indicates a driver error. Failed writes or unavailable graphics should be reported, not treated as a successful playtest.

Testers should explore from visible cues, record concrete friction and reproducible command IDs, and distinguish observations from proposed improvements. Do not infer player ratings or iPhone frame rates from this bridge.
