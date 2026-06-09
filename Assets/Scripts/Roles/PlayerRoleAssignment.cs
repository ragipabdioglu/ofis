using System.Collections.Generic;
using OFIS.Core.Ids;

namespace OFIS.Roles
{
    public sealed class PlayerRoleAssignment
    {
        public PlayerId PlayerId { get; }
        public string DisplayName { get; }
        public PlayerRole Role { get; }

        private readonly List<PlayerId> _knownVictimTargets = new();

        public IReadOnlyList<PlayerId> KnownVictimTargets => _knownVictimTargets;

        public PlayerRoleAssignment(
            PlayerId playerId,
            string displayName,
            PlayerRole role)
        {
            PlayerId = playerId;
            DisplayName = displayName;
            Role = role;
        }

        public void AddKnownVictimTarget(PlayerId victimId)
        {
            if (_knownVictimTargets.Contains(victimId))
                return;

            _knownVictimTargets.Add(victimId);
        }

        public override string ToString()
        {
            return $"{DisplayName} ({PlayerId}) => {Role}";
        }
    }
}