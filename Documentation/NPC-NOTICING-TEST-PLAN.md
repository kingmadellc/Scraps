# Early noticing behavior regression plan

Integrated in `NeighborAI.cs` after the source freeze was released; awaiting coordinated Unity validation.

- `noticing` above 0.06 drives the existing question/exclamation cue.
- `IsInvestigating` starts at 0.55 only while there is sight and no active chase. This gates patrol interruption and turning toward Jimothy. Before that threshold, navigation and patrol heading continue normally.
- DetectionLabel distinguishes Noticing, Investigating, Chasing, and Searching.

Required checks after integration:

1. On an unobstructed street patrol, expose Jimothy briefly until suspicion is 0.10–0.30. Confirm the NPC continues toward its existing waypoint, with visible question cue/meter, rather than stopping or snapping to face Jimothy.
2. Keep Jimothy plainly visible until suspicion exceeds 0.55. Confirm investigation pauses the patrol and turns toward him, while chase remains false until full suspicion.
3. At a low accumulated value, let the NPC pass so Jimothy leaves its forward view beyond the near-awareness distance. Confirm patrol continues, line of sight becomes false, and suspicion decays rather than resuming pursuit or instantly zeroing.
4. Break sight behind a static barrier during investigation. Confirm the pause ends and patrol resumes as suspicion decays. A lost full chase retains only the existing short last-seen search behavior.
5. Repeat with dog and angry human. Preserve ordinary-jump detection, ground-only pursuit, and roof/Den contact-loss checks from NeighborSuspicionAudit.

The staged change does not alter detection accumulation/decay rates, near awareness, or chase threshold. Continuous close exposure can still reach investigation before a slow pedestrian has passed; passing unnoticed requires enough separation, breaking sight, or waiting outside the forward view.
