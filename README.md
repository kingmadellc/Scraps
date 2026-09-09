# Jimothy Survival

Unity 6000.3.0f1 / URP survival platformer set around Old Ballard Avenue, with editable Blender character and environment assets. Current prototype: **0.4.5**.

## Get started

Install Git LFS before cloning this repository:

```sh
git lfs install
git clone https://github.com/kingmadellc/JimothySurvival.git
cd JimothySurvival
git lfs pull
```

In Unity Hub, install **6000.3.0f1** and open the `Unity` folder. Let Unity restore packages and import assets, then open `Assets/Jimothy/Scenes/Ballard.unity` and press Play. Install Web Build Support for Safari builds or iOS Build Support and Xcode for native iOS work. Unity caches and compiled players are intentionally not in Git.

Current editable models include `Art/Blender/Jimothy-v9.blend`, `Art/Blender/BallardHuman-v6.blend`, and `Art/Blender/DenInstruments.blend`. Runtime meshes and textures are under `Unity/Assets/Jimothy/Resources`. Open Blender sources with Blender 4.5 or newer. Older model versions remain for reference.

See [0.4.5 release notes](Documentation/RELEASE-045.md), [stealth and tricks](Documentation/STEALTH-AND-STYLE-V045.md), and [design priorities](Documentation/FEEDBACK-REVIEW-V045.md). This is a playable prototype, not a finished App Store release. Physical iPhone/iPad performance and human fun ratings remain to be validated.

## Working from mobile

Use ChatGPT Remote to continue tasks on the connected development Mac. A GitHub copy provides source backup and collaboration; it does not run Unity or Blender on the phone. Keep the Mac awake and its external development drive connected. See [official Remote setup](https://learn.chatgpt.com/docs/remote-connections).

The original development machine uses `/Volumes/Jimothy Dev/Projects/Jimothy` for builds and heavy authoring. Some helper scripts still contain machine-specific paths; review them before running on another computer. The Unity project can be opened directly from a clone. Large binary assets use Git LFS; always fetch them before opening Unity or Blender.

Generated previews, reference photographs, and superseded presentation/blockout archives remain local. Editable current game assets and their Unity exports are included.
