using System.Collections.Generic;
using System.Linq;
using OFIS.Roles;
using UnityEngine;

namespace OFIS.Lobby
{
    public sealed class LobbyReadyFlowService
    {
        public LobbyReadyFlowSnapshot BuildSnapshot(IReadOnlyList<MockLobbyPlayer> players)
        {
            if (players == null || players.Count == 0)
                return new LobbyReadyFlowSnapshot(0, 0, 0, 0, false, false);

            int totalPlayers = players.Count;
            int disconnectedPlayers = players.Count(x => x.ConnectionState == LobbyConnectionState.Disconnected);
            int connectedPlayers = players.Count(x => x.IsConnected);
            int readyPlayers = players.Count(x => x.ConnectionState == LobbyConnectionState.Ready);
            bool allConnectedPlayersReady = connectedPlayers > 0 && connectedPlayers == readyPlayers;
            bool supportedCount = totalPlayers is 6 or 8 or 10 or 12;
            bool canStartMatch = supportedCount && disconnectedPlayers == 0 && allConnectedPlayersReady;

            return new LobbyReadyFlowSnapshot(
                totalPlayers,
                connectedPlayers,
                readyPlayers,
                disconnectedPlayers,
                allConnectedPlayersReady,
                canStartMatch);
        }

        public void SetAllConnected(IReadOnlyList<MockLobbyPlayer> players)
        {
            if (players == null)
                return;

            for (int i = 0; i < players.Count; i++)
                players[i]?.SetConnectionState(LobbyConnectionState.Connected);
        }

        public void SetAllReady(IReadOnlyList<MockLobbyPlayer> players)
        {
            if (players == null)
                return;

            for (int i = 0; i < players.Count; i++)
                players[i]?.SetReady(true);
        }

        public void SetAllLoadingMatch(IReadOnlyList<MockLobbyPlayer> players)
        {
            if (players == null)
                return;

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i] == null || players[i].IsDisconnected)
                    continue;

                players[i].SetConnectionState(LobbyConnectionState.LoadingMatch);
            }
        }

        public void SetAllInMatch(IReadOnlyList<MockLobbyPlayer> players)
        {
            if (players == null)
                return;

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i] == null || players[i].IsDisconnected)
                    continue;

                players[i].SetConnectionState(LobbyConnectionState.InMatch);
            }
        }

        public bool TryStartLoadingMatch(IReadOnlyList<MockLobbyPlayer> players)
        {
            LobbyReadyFlowSnapshot snapshot = BuildSnapshot(players);

            if (!snapshot.CanStartMatch)
            {
                Debug.LogWarning($"[LobbyReadyFlow] Cannot start match. {snapshot}");
                return false;
            }

            SetAllLoadingMatch(players);
            Debug.Log($"[LobbyReadyFlow] All players moved to LoadingMatch. {snapshot}");
            return true;
        }
    }
}
