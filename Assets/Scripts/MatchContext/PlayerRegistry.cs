using System.Collections.Generic;
using System.Linq;
using OFIS.Core.Ids;
using OFIS.Roles;

namespace OFIS.MatchContext
{
    public sealed class PlayerRegistry
    {
        private readonly List<PlayerRegistryEntry> _entries = new();

        public IReadOnlyList<PlayerRegistryEntry> Entries => _entries;

        public int Count => _entries.Count;

        public PlayerRegistryEntry GetByIndex(int index)
    {
        if (index < 0 || index >= _entries.Count)
            return null;
    
        return _entries[index];
    }

        public void Add(PlayerRegistryEntry entry)
        {
            if (entry == null)
                return;

            if (_entries.Any(x => x.LobbyPlayer.PlayerId == entry.LobbyPlayer.PlayerId))
                return;

            _entries.Add(entry);
        }

        public PlayerRegistryEntry GetByPlayerId(PlayerId playerId)
        {
            return _entries.FirstOrDefault(x => x.LobbyPlayer.PlayerId == playerId);
        }

        public IReadOnlyList<PlayerRegistryEntry> GetByRole(PlayerRole role)
        {
            return _entries.Where(x => x.RoleAssignment.Role == role).ToList();
        }

        public IReadOnlyList<PlayerRegistryEntry> GetKillers()
        {
            return GetByRole(PlayerRole.Killer);
        }

        public IReadOnlyList<PlayerRegistryEntry> GetVictims()
        {
            return GetByRole(PlayerRole.Victim);
        }

        public IReadOnlyList<PlayerRegistryEntry> GetDetectives()
        {
            return GetByRole(PlayerRole.Detective);
        }
    }
}