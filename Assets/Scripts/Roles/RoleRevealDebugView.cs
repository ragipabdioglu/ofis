using System.Collections.Generic;
using System.Linq;
using OFIS.Core.Ids;

namespace OFIS.Roles
{
    public sealed class RoleRevealDebugView
    {
        public PlayerId OwnerPlayerId { get; }
        public string OwnerDisplayName { get; }
        public PlayerRole OwnRole { get; }
        public IReadOnlyList<MockLobbyPlayer> KnownVictimTargets { get; }

        public RoleRevealDebugView(
            PlayerId ownerPlayerId,
            string ownerDisplayName,
            PlayerRole ownRole,
            IReadOnlyList<MockLobbyPlayer> knownVictimTargets)
        {
            OwnerPlayerId = ownerPlayerId;
            OwnerDisplayName = ownerDisplayName;
            OwnRole = ownRole;
            KnownVictimTargets = knownVictimTargets;
        }

        public string GetTargetSummary()
        {
            if (KnownVictimTargets == null || KnownVictimTargets.Count == 0)
                return "No known targets.";

            return string.Join(", ", KnownVictimTargets.Select(x => x.DisplayName));
        }
    }
}