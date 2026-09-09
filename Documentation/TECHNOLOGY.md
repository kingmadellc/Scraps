# Technology decision — 7 September 2026

Use **Unity 6.3 LTS, C#, Universal Render Pipeline (URP), and Metal**, with Blender as the asset and animation source. This is an engineering recommendation for this particular iOS 3D platformer, not a claim that one engine is universally best.

| Choice | Fit for Jimothy |
|---|---|
| Unity + URP | Recommended. Native iOS export, mature character animation and mobile graphics workflow, gamepad/touch input, profiling, and an established asset pipeline. |
| Unreal | Capable of exceptional visuals, but its high-end rendering features do not eliminate mobile budgets. More engine complexity than this small team's initial slice needs. |
| Godot | Attractive open-source option with native iOS export. Worth choosing for a lighter scope; less compelling here than Unity's integrated 3D/mobile production workflow. |
| Web / Three.js | Useful for lightweight previews, but not the recommended shipping architecture for this native 3D game. |

Unity documents Metal support on iOS and URP compatibility in [Metal requirements](https://docs.unity3d.com/6000.0/Documentation/Manual/metal-requirements-and-compatibility.html). The current baseline is [Unity 6.3 LTS](https://unity.com/releases/unity-6/support); the repository pins the initial 6000.3.0f1 release as a reproducible starting point, not the latest patch. Select a current stable 6.3 patch and lock it after the first successful device build. Package resolution and editor compilation still need to be tested locally.

Blender provides mesh authoring, UVs, material baking, skeletons, animation and offline promotional rendering. FBX is the handoff to Unity; `.blend` sources stay outside Assets so importing the game does not require Blender. The delivered fur groom belongs to Blender renders. It is not exported as runtime hair.

## Rendering direction and proposed budgets

Target a stylized, tactile Pacific Northwest world with realistic proportions and lighting. The reference is the movement freedom of classic 3D Mario platformers, with original characters, layouts and art. Start with camera-relative movement and forgiving jumps; add ledge grabs, wall kicks, rail traversal and more elaborate movement after the current controller is proven on touch.

Initial device target: iPhone 13 and newer, iOS 16+, landscape. This is a proposed product minimum rather than the engine minimum. Aim for 60 fps, with a 30 fps battery option, then measure on physical hardware. The current prototype has not been profiled.

Production budgets to validate: approximately 25–40k triangles for Jimothy's nearest LOD, 12k / 4k secondary LODs, 2K character texture sets, shared environment atlases, baked indirect lighting, short shadow distance, instanced foliage, and distance-culling. Use a true 4K master for menu art, downsampled/compressed appropriately in the app. Texture resolution alone will not make the game beautiful; material quality, silhouette, lighting and composition matter.

## Persistence

Implemented source: one local save slot with a versioned JSON envelope, SHA-256 corruption detection, a flushed temporary write, replacement plus backup, inventory, stash, trophy unlocks, decor, player location, survival record and per-node refill timers. SHA-256 is an integrity check, not protection against cheating. An unreadable save is preserved. Offline world progression is capped at eight hours and never drains hunger or health. Death loses carried items and keeps the den collection.

Planned production service: Unity Authentication plus Cloud Save, with a platform identity linking flow, local guest mode, a revisioned server record, conflict resolution and retries. This is not implemented. Do not imply that local loading is sign-in or cloud sync. If trade or purchasable currency is introduced, economy mutations need server-side validation rather than trusting this local prototype.

## iOS path

1. Free enough disk space for Unity, iOS Build Support, full Xcode and build artifacts. The Mac had about 5.7 GB available at the start; full Xcode is absent.
2. Open `Unity/` in Unity Hub using Unity 6.3 LTS. Resolve packages.
3. Run **Jimothy → Create or refresh playable scene**, then Play.
4. Run the EditMode tests. Verify touch controls and the core loop on a device.
5. Export with **Jimothy → Export iOS Xcode project**. Set the actual bundle ID and Apple team; the current bundle ID is a placeholder.
6. Add authentication/cloud sync, finish device profiling, App Store metadata, privacy declarations, final audio/art, and TestFlight testing before submission.

No Unity compilation, iOS export, signing, TestFlight upload or App Store submission has been performed in this workspace.
