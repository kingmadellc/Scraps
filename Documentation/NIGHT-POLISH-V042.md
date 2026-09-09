# Night lighting, navigation and finds — 0.4.2

The street lighting now illuminates the world rather than only making bulbs glow. Street lanterns cast larger warm pools, avenue festoons have nine grouped light sources, rooftop festoons have five, and the garden path has three low shielded lights. One cool directional moon light supplies overhead shape and soft shadows. The visible full moon follows that light's direction. Local lights fade between20–30m instead of switching abruptly at22m; only the moon casts realtime shadows.

The heritage street uses a new physically shaded paver surface: varied fired-clay color, bevel/mortar relief, fine worn grain, occluded joints and irregular world-space dampness. Dampness is in the same opaque surface, replacing asphalt-colored transparent puddle overlays. Normal filtering, mipmaps and8×anisotropy limit distant shimmer. Eight-meter street sections and16m building batches keep up to eight forward local lights relevant to nearby geometry. This increases draws over the old32m batches; actual phone thermal/FPS performance remains to be measured.

Roof-wide stone slabs were masking the roof membrane and overlapping access landings. Perimeter coping replaces the slab, with notches at all ten access bridges. Crossing visuals meet at their endpoints while collision volumes retain their safety overlap. Exposed roof membranes now use textured rough aggregate.

Find locations retain their22 stable identities and item types, preserving inventory and cooldown compatibility. Concealed, validated alternative sites replace fixed refill locations; longer varied refill intervals and player-distance gating prevent nearby pop-in. The welcome snack is an initial onboarding find. Search still requires reach and line of sight.

Verification is recorded in Logs and PlaytestCaptures, including separate moon/practical-light captures to measure each light system's contribution. These are actual Unity renders, not generated concept images. Device-specific performance is not inferred from desktop screenshots.


## Completed checks

- Actual-district navigation: all nine production NPC configurations plus an embedded dog spawn; reachable patrol goals, no measured clearance-volume penetration, unreachable building targets rejected. Synthetic wall/corridor/curb checks also passed.
- Roof surfaces:20 rendered-mesh samples passed, all eight crossing directions passed, and eight moving-camera roof frames were reviewed without the former coincident roof/access surfaces.
- Finds after new café/post colliders:22 identities /63 supported searchable positions passed. First-night interaction assertions and rear-camera regression passed.
-19 Editor tests passed. Moon/practical-light contribution images passed, with both systems independently contributing to the street.

Native and Web0.4.2 builds succeeded. Native code-sign verification passed. An isolated native playtest verified movement, jump, forward rear camera and rendered road/lighting with no logged runtime errors. Chromium passed11 touch/steering/jump/orientation/save assertions; WebKit loaded0.4.2, collected by touch, and restored the snack after full reload/Continue, without captured runtime errors. The camera audit was rerun with the new prop colliders and passed.

Evidence: `AgentPlaytests/v042-native-check/`, `PlaytestCaptures/web-v042-touch/`, `BrowserPlaytests/webkit-v042/`, and `Logs/rear-camera-props-v042.log`. Physical iPhone/iPad performance remains unmeasured. Agent art review still finds the runtime fur conspicuously speckled and the repeated street-light pools noticeable; this pass does not claim concept-render parity.
