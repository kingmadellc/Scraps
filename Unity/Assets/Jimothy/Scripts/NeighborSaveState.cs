using System;
using UnityEngine;
namespace Jimothy {
[Serializable] public sealed class NeighborSaveState {
 public int version=1;public NeighborKind kind;
 public Vector3 position,lastSeen,patrolTarget;public float yaw,suspicion,unseenSeconds,reacquireGrace,cooldown,patrolTimer,restTimer;
 public bool chasing;public int patrolRound;
}
}
