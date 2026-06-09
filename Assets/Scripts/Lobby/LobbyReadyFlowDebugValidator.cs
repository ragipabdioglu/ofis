using System.Collections.Generic;
using OFIS.Roles;
using UnityEngine;

namespace OFIS.Lobby
{
    public sealed class LobbyReadyFlowDebugValidator : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;
        [SerializeField] private int mockPlayerCount = 8;

        private readonly LobbyReadyFlowService _readyFlowService = new();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateReadyFlow();
        }

        [ContextMenu("Validate Ready Flow")]
        public void ValidateReadyFlow()
        {
            List<MockLobbyPlayer> players = MockPlayerFactory.CreateMockLobbyPlayers(mockPlayerCount);
            _readyFlowService.SetAllConnected(players);

            LobbyReadyFlowSnapshot connectedSnapshot = _readyFlowService.BuildSnapshot(players);
            Debug.Log($"[LobbyReadyFlowValidator] Connected snapshot: {connectedSnapshot}");

            _readyFlowService.SetAllReady(players);
            LobbyReadyFlowSnapshot readySnapshot = _readyFlowService.BuildSnapshot(players);
            Debug.Log($"[LobbyReadyFlowValidator] Ready snapshot: {readySnapshot}");

            bool loadingStarted = _readyFlowService.TryStartLoadingMatch(players);
            Debug.Log($"[LobbyReadyFlowValidator] TryStartLoadingMatch={loadingStarted}");

            LobbyReadyFlowSnapshot loadingSnapshot = _readyFlowService.BuildSnapshot(players);
            Debug.Log($"[LobbyReadyFlowValidator] Loading snapshot: {loadingSnapshot}");

            _readyFlowService.SetAllInMatch(players);
            LobbyReadyFlowSnapshot inMatchSnapshot = _readyFlowService.BuildSnapshot(players);
            Debug.Log($"[LobbyReadyFlowValidator] InMatch snapshot: {inMatchSnapshot}");

            LogPlayers(players);
        }

        private static void LogPlayers(IReadOnlyList<MockLobbyPlayer> players)
        {
            for (int i = 0; i < players.Count; i++)
                Debug.Log($"[LobbyReadyFlowValidator] {players[i].DisplayName}: {players[i].ConnectionState}");
        }
    }
}
