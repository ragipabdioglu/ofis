using OFIS.Core.Ids;
using OFIS.Lobby;

namespace OFIS.Roles
{
    public sealed class MockLobbyPlayer
    {
        public PlayerId PlayerId { get; }
        public int PlayerIndex { get; }
        public string DisplayName { get; }
        public LobbyConnectionState ConnectionState { get; private set; }

        public bool IsConnected => ConnectionState is LobbyConnectionState.Connected or LobbyConnectionState.Ready or LobbyConnectionState.LoadingMatch or LobbyConnectionState.InMatch;
        public bool IsReady => ConnectionState == LobbyConnectionState.Ready;
        public bool IsDisconnected => ConnectionState == LobbyConnectionState.Disconnected;

        public MockLobbyPlayer(PlayerId playerId, int playerIndex, string displayName)
        {
            PlayerId = playerId;
            PlayerIndex = playerIndex;
            DisplayName = displayName;
            ConnectionState = LobbyConnectionState.Connected;
        }

        public void SetConnectionState(LobbyConnectionState state)
        {
            ConnectionState = state;
        }

        public void SetReady(bool isReady)
        {
            if (ConnectionState == LobbyConnectionState.Disconnected)
                return;

            ConnectionState = isReady
                ? LobbyConnectionState.Ready
                : LobbyConnectionState.Connected;
        }

        public override string ToString()
        {
            return $"{DisplayName} ({PlayerId}) [{ConnectionState}]";
        }
    }
}
