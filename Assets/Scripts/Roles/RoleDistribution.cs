using System;

namespace OFIS.Roles
{
    [Serializable]
    public readonly struct RoleDistribution
    {
        public readonly int PlayerCount;
        public readonly int KillerCount;
        public readonly int VictimCount;
        public readonly int DetectiveCount;

        public RoleDistribution(
            int playerCount,
            int killerCount,
            int victimCount,
            int detectiveCount)
        {
            PlayerCount = playerCount;
            KillerCount = killerCount;
            VictimCount = victimCount;
            DetectiveCount = detectiveCount;
        }

        public int TotalRoleCount => KillerCount + VictimCount + DetectiveCount;

        public bool IsValid => PlayerCount > 0 && TotalRoleCount == PlayerCount;

        public override string ToString()
        {
            return $"Players={PlayerCount}, Killers={KillerCount}, Victims={VictimCount}, Detectives={DetectiveCount}";
        }
    }
}