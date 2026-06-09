namespace OFIS.Lobby
{
    public readonly struct LobbyReadyFlowSnapshot
    {
        public int TotalPlayers { get; }
        public int ConnectedPlayers { get; }
        public int ReadyPlayers { get; }
        public int DisconnectedPlayers { get; }
        public bool AllConnectedPlayersReady { get; }
        public bool CanStartMatch { get; }

        public LobbyReadyFlowSnapshot(
            int totalPlayers,
            int connectedPlayers,
            int readyPlayers,
            int disconnectedPlayers,
            bool allConnectedPlayersReady,
            bool canStartMatch)
        {
            TotalPlayers = totalPlayers;
            ConnectedPlayers = connectedPlayers;
            ReadyPlayers = readyPlayers;
            DisconnectedPlayers = disconnectedPlayers;
            AllConnectedPlayersReady = allConnectedPlayersReady;
            CanStartMatch = canStartMatch;
        }

        public override string ToString()
        {
            return $"Total={TotalPlayers}, Connected={ConnectedPlayers}, Ready={ReadyPlayers}, Disconnected={DisconnectedPlayers}, AllReady={AllConnectedPlayersReady}, CanStart={CanStartMatch}";
        }
    }
}
