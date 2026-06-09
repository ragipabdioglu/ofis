using System.Collections.Generic;
using OFIS.Core.Ids;

namespace OFIS.Roles
{
    public static class MockPlayerFactory
    {
        public static List<PlayerId> CreateMockPlayers(int count)
        {
            var players = new List<PlayerId>();

            for (int i = 0; i < count; i++)
                players.Add(PlayerId.New());

            return players;
        }

        public static List<MockLobbyPlayer> CreateMockLobbyPlayers(int count)
        {
            var players = new List<MockLobbyPlayer>();

            for (int i = 0; i < count; i++)
            {
                int index = i + 1;

                players.Add(new MockLobbyPlayer(
                    PlayerId.New(),
                    index,
                    $"Player {index}"));
            }

            return players;
        }

        public static List<PlayerId> ExtractPlayerIds(IReadOnlyList<MockLobbyPlayer> lobbyPlayers)
        {
            var ids = new List<PlayerId>();

            if (lobbyPlayers == null)
                return ids;

            for (int i = 0; i < lobbyPlayers.Count; i++)
                ids.Add(lobbyPlayers[i].PlayerId);

            return ids;
        }
    }
}