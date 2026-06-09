using OFIS.Core.Ids;

namespace OFIS.Roles
{
    public sealed class MockLobbyPlayer
    {
        public PlayerId PlayerId { get; }
        public int PlayerIndex { get; }
        public string DisplayName { get; }

        public MockLobbyPlayer(PlayerId playerId, int playerIndex, string displayName)
        {
            PlayerId = playerId;
            PlayerIndex = playerIndex;
            DisplayName = displayName;
        }

        public override string ToString()
        {
            return $"{DisplayName} ({PlayerId})";
        }
    }
}