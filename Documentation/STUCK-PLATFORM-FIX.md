# 0.3.1 — stuck climb recovery

User report: WASD rotates Jimothy, but movement and Space do nothing on the second access block. Reported saved position: (8.683109, 2.0699999, -22.909088).

The mantle completion target was top + .04 m, while the CharacterController rested at top + .07 m because of collision skin. The .03 m gap exceeded the .015 m completion tolerance but fell below the .035 m blocked-movement cancellation threshold, keeping the motor in mantle mode indefinitely.

Fix: a grounded final approach accepts the collision-skin tolerance with a strict horizontal check. A .20-second no-progress watchdog and 1.5-second total limit cancel a stalled assist. Movement and jump inputs resume through the normal motor.

Validation: MantleRecoveryAudit injects that interrupted state into a fresh unsaved scene using the real colliders and motor. Settlement cleared after two 60Hz frames; the blocked intermediate stage cancelled after 14 frames. The same motor then jumped 1.85625 m and moved .60 m. All 14 rooftop routes/connectors passed the traversal regression. Tests never load or write the user save. The existing user save and backup were copied byte-for-byte into LocalBackups/stuck-platform-2026-09-07 before restart.
