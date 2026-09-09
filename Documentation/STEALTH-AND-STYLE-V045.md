# Stealth and rooftop style — 0.4.5

This pass adds stealth and optional arcade tricks to the existing survival game. It does not replace persistent exploration with a forced timer.

## Player-facing behavior

- Hostile NPCs accumulate suspicion from proximity, facing, and unobstructed sight. Hiding or putting distance between you and them drains the meter; they investigate the last seen position instead of tracking through walls.
- Roof access is a refuge. Hostile navigation stays at street/curb height and cannot pursue up stairs.
- A blocked pedestrian should turn and take a short, supported wall break, then resume patrol. Dogs use a species-appropriate pause rather than a human lean.
- The first access jump remains deliberate. Subsequent authored stair treads get traversal assistance when moving forward, with collision and landing support checks retained.
- Press **T**, gamepad **B / Circle**, or touch **Trick** in the air to attempt a flip. Ordinary jumps do not automatically spin. A completed trick needs a safe landing to score.
- Each 1,000 accumulated style points awards five shinies for Den furnishings. Total style and best landing persist with the save.
- A high uninterrupted fall causes damage even during the brief immunity after an enemy hit. Landing on a lower supported ledge breaks the descent; grazing a wall does not.

## Verification status

The native agent completed the snack → rooftop → keepsake → Den deposit → purchase/show/hide/favorite loop using real movement and rendered buttons, with saving suppressed. WebKit passed 13 touch, keyboard, movement, collection and save/reload checks with no browser runtime errors. Desktop/WebKit checks do not substitute for device performance measurements or human playtesting.

Fast travel requires Jimothy to be grounded, so it cannot cancel a dangerous fall. Stationary flips remain useful practice but do not build a traveling combo multiplier.

Editor validation: 11 motor fixtures plus 300 camera samples passed; all 10 authored stair routes passed after one initial jump; 14 traversal routes and the rear-camera regression passed. Actual NPC perception/ground navigation and supported-rest rig checks passed, including a re-audit after repairing jacket skin weights. Den save checks cover decoration ownership/visibility, eight favorite displays, style milestones, unsafe landing damage, and persistence. Rendered art review corrected imported instrument orientation. The final polish changes replace repeated stair Jump prompts with Keep moving after the first tread and settle the purchased cushion into the sofa. Den behavior and rendered art checks were rerun successfully.
