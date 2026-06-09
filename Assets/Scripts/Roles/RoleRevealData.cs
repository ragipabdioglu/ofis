using System.Collections.Generic;
using OFIS.Core.Ids;

namespace OFIS.Roles
{
    public sealed class RoleRevealData
    {
        public PlayerId OwnerPlayerId { get; }
        public PlayerRole OwnRole { get; }
        public IReadOnlyList<PlayerId> KnownVictimTargets { get; }

        public RoleRevealData(
            PlayerId ownerPlayerId,
            PlayerRole ownRole,
            IReadOnlyList<PlayerId> knownVictimTargets)
        {
            OwnerPlayerId = ownerPlayerId;
            OwnRole = ownRole;
            KnownVictimTargets = knownVictimTargets;
        }
    }
}