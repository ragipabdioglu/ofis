using System.Collections.Generic;
using OFIS.Core.Ids;
using OFIS.Roles;

namespace OFIS.Players
{
    public sealed class PlayerPrivateState
    {
        public PlayerId OwnerPlayerId { get; }
        public PlayerRole OwnRole { get; }
        public IReadOnlyList<PlayerId> KnownVictimTargets { get; }

        public PlayerPrivateState(
            PlayerId ownerPlayerId,
            PlayerRole ownRole,
            IReadOnlyList<PlayerId> knownVictimTargets)
        {
            OwnerPlayerId = ownerPlayerId;
            OwnRole = ownRole;
            KnownVictimTargets = knownVictimTargets;
        }

        public bool HasKnownTargets => KnownVictimTargets != null && KnownVictimTargets.Count > 0;
    }
}