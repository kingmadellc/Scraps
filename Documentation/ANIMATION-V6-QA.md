# V6 animation QA

Final native blend and FBX exported with the shortened limbs, strengthened jump poses and final thin fur ribbons. Native renders were inspected at takeoff, reach, apex, descent, cushion, recovery and rear/side/front views. Jump is an authored adaptation, not claimed as footage-matched kinematics. The final apex visibly gathers the hind feet more than takeoff; compression is stronger, with no detached joints or inverted limbs visible in inspected poses.

A 121-sample subframe check per gait found that the previous nine-key run action drifted between otherwise correct integer poses. Densifying analytic keys to quarter-frame intervals retained cycle duration and nominal speed. Maximum intended wrist-trajectory error dropped from .130 to .0179 armature units for Run (about29mm to4mm at .22 scale), and .0531 to .00342 for Walk (about12mm to.75mm). Run stance-height deviation fell from9.4mm to.73mm world. Run paw orientation residual fell from4.95 to1.46 degrees. Joint gaps remain below3e-6 armature units. These are bounded numeric improvements, not a claim of zero residual skating.

Actual final FBX reimport with simplification disabled confirms every channel retains quarter-frame keys: Walk65, Run33, Idle241 and Jump241. Durations are unchanged: Walk frames1–17, Run1–9, Idle/Jump1–61. Details: gait-v6-qa.json and fbx-v6-key-qa.json.

The stronger Jump export reports zero reach clamp, knee gap below1.6e-6, solver error below1.3e-6 and zero keyed paw-orientation error. Actual evaluated skin/fur bounds remain finite in six sampled poses (jump-v6.json).

JumpAnimationAudit.Run is supplied for parent Unity integration. It uses the imported FBX and actual Animator to verify non-looping configuration, different baked skin at takeoff/apex/landing, and unchanged skin when JumpPhase is held for two seconds. This agent did not run Unity; phase-controlled runtime behavior is not approved solely from Blender renders.

Unity verification completed: JumpAnimationAudit passed on the imported FBX/controller. Takeoff/apex/landing poses differ, and holding apex phase for two seconds produces zero pose drift. Compression is disabled for this hero import. The native v7-motion playtest separately passed movement, rear steering, tap jump, grounded recovery and pause; sparse screenshots did not isolate the exact apex or short landing cushion.
