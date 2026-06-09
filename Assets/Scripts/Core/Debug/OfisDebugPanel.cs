using System.Collections.Generic;
using System.Linq;
using OFIS.Core.Config;
using OFIS.Core.Ids;
using OFIS.MatchFlow;
using OFIS.Players;
using OFIS.Roles;
using OFIS.Roles.Identity;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace OFIS.Core.Debugging
{
    public sealed class OfisDebugPanel : MonoBehaviour
    {
        private OfisCoreConfig _config;
        private bool _isVisible;
        private MatchFlowRunner _matchFlowRunner;
        private Vector2 _scrollPosition;

        private readonly RoleAssignmentService _roleAssignmentService = new(seed: 12345);
        private readonly IdentityAssignmentService _identityAssignmentService = new(seed: 54321);
        private readonly PlayerStateBuilder _playerStateBuilder = new();

        private RoleAssignmentResult _lastRoleAssignmentResult;
        private IdentityAssignmentResult _lastIdentityAssignmentResult;
        private PlayerStateBuildResult _lastPlayerStateBuildResult;

        private List<MockLobbyPlayer> _mockLobbyPlayers = new();
        private RoleRevealDebugView _selectedRevealView;
        private PlayerPublicState _selectedPublicState;
        private PlayerPrivateState _selectedPrivateState;

        private int _mockPlayerCount = 8;
        private int _selectedRevealIndex;

        public void Initialize(OfisCoreConfig config)
        {
            _config = config;
            _isVisible = false;
            _matchFlowRunner = FindFirstObjectByType<MatchFlowRunner>();

            RebuildMockLobbyPlayers();
        }

        private void Update()
        {
            if (_config == null)
                return;

            if (WasDebugKeyPressed())
            {
                _isVisible = !_isVisible;

                if (_matchFlowRunner == null)
                    _matchFlowRunner = FindFirstObjectByType<MatchFlowRunner>();
            }
        }

        private bool WasDebugKeyPressed()
        {
#if ENABLE_INPUT_SYSTEM
            return Keyboard.current != null && Keyboard.current.f1Key.wasPressedThisFrame;
#else
            return Input.GetKeyDown(_config.debugPanelKey);
#endif
        }

        private void OnGUI()
        {
            if (!_isVisible || _config == null)
                return;

            float width = Mathf.Min(680f, Screen.width - 32f);
            float height = Mathf.Min(860f, Screen.height - 32f);

            GUILayout.BeginArea(new Rect(16, 16, width, height), GUI.skin.box);
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

            GUILayout.Label("OFIS Debug Panel");
            GUILayout.Space(8);

            DrawCoreInfo();
            GUILayout.Space(10);

            DrawMatchFlowInfo();
            GUILayout.Space(10);

            DrawMatchFlowButtons();
            GUILayout.Space(16);

            DrawRoleIdentityAndStateDebug();

            GUILayout.Space(20);
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void DrawCoreInfo()
        {
            GUILayout.Label($"Project: {_config.projectName}");
            GUILayout.Label($"Version: {_config.projectVersion}");
            GUILayout.Label($"Default Players: {_config.defaultPlayerCount}");
            GUILayout.Label($"Default Match Duration: {_config.defaultMatchDurationSeconds}s");
        }

        private void DrawMatchFlowInfo()
        {
            if (_matchFlowRunner == null)
            {
                GUILayout.Label("MatchFlowRunner: Not found in scene.");
                return;
            }

            var service = _matchFlowRunner.Service;

            if (service == null)
            {
                GUILayout.Label("MatchFlowService: Not initialized yet.");
                return;
            }

            GUILayout.Label("Match Flow");
            GUILayout.Label($"State: {service.CurrentState}");
            GUILayout.Label($"Running: {service.IsRunning}");
            GUILayout.Label($"Fast Test: {service.IsFastTest}");
            GUILayout.Label($"Match Time: {service.MatchTimeSeconds:0.0}s");
            GUILayout.Label($"Match Remaining: {service.MatchRemainingSeconds:0.0}s");
            GUILayout.Label($"Phase Elapsed: {service.CurrentStateElapsedSeconds:0.0}s");
            GUILayout.Label($"Phase Remaining: {service.CurrentStateRemainingSeconds:0.0}s");
        }

        private void DrawMatchFlowButtons()
        {
            GUILayout.Label("Match Flow Controls");

            if (_matchFlowRunner == null)
            {
                if (GUILayout.Button("Find MatchFlowRunner", GUILayout.Height(32)))
                    _matchFlowRunner = FindFirstObjectByType<MatchFlowRunner>();

                return;
            }

            if (GUILayout.Button("Start Normal Match", GUILayout.Height(32)))
                _matchFlowRunner.StartNormalMatch();

            if (GUILayout.Button("Start Fast Test Match", GUILayout.Height(32)))
                _matchFlowRunner.StartFastTestMatch();

            if (GUILayout.Button("Stop Match", GUILayout.Height(32)))
                _matchFlowRunner.StopMatch();

            GUILayout.Label("F1: Toggle Debug Panel");
        }

        private void DrawRoleIdentityAndStateDebug()
        {
            GUILayout.Label("Role + Identity + Player State Debug");

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("-"))
            {
                _mockPlayerCount = Mathf.Max(1, _mockPlayerCount - 1);
                RebuildMockLobbyPlayers();
            }

            GUILayout.Label($"Mock Players: {_mockPlayerCount}", GUILayout.Width(160));

            if (GUILayout.Button("+"))
            {
                _mockPlayerCount = Mathf.Min(12, _mockPlayerCount + 1);
                RebuildMockLobbyPlayers();
            }

            GUILayout.EndHorizontal();

            if (GUILayout.Button("Rebuild Mock Lobby Players", GUILayout.Height(28)))
                RebuildMockLobbyPlayers();

            if (GUILayout.Button("Assign Public Identities", GUILayout.Height(32)))
                AssignPublicIdentities();

            if (GUILayout.Button("Assign Roles", GUILayout.Height(32)))
                AssignRolesForMockLobbyPlayers();

            if (GUILayout.Button("Build Public/Private Player States", GUILayout.Height(32)))
                BuildPublicPrivatePlayerStates();

            if (GUILayout.Button("Full Build: Identities + Roles + States", GUILayout.Height(36)))
            {
                AssignPublicIdentities();
                AssignRolesForMockLobbyPlayers();
                BuildPublicPrivatePlayerStates();
            }

            GUILayout.Space(8);

            DrawMockLobbyPlayersWithIdentityAndRole();
            GUILayout.Space(8);

            DrawPlayerStates();
            GUILayout.Space(8);

            DrawOwnerPrivateStateInspector();
        }

        private void RebuildMockLobbyPlayers()
        {
            _mockLobbyPlayers = MockPlayerFactory.CreateMockLobbyPlayers(_mockPlayerCount);
            _lastRoleAssignmentResult = null;
            _lastIdentityAssignmentResult = null;
            _lastPlayerStateBuildResult = null;
            _selectedRevealView = null;
            _selectedPublicState = null;
            _selectedPrivateState = null;
            _selectedRevealIndex = 0;

            Debug.Log($"[MockLobby] Rebuilt mock lobby players. Count={_mockPlayerCount}");
        }

        private void AssignPublicIdentities()
        {
            var playerIds = MockPlayerFactory.ExtractPlayerIds(_mockLobbyPlayers);
            _lastIdentityAssignmentResult = _identityAssignmentService.AssignIdentities(playerIds);

            if (!_lastIdentityAssignmentResult.Success)
            {
                Debug.LogWarning($"[IdentityAssignment] Failed: {_lastIdentityAssignmentResult.ErrorMessage}");
                return;
            }

            Debug.Log($"[IdentityAssignment] Success. Count={_lastIdentityAssignmentResult.Identities.Count}");

            foreach (var identity in _lastIdentityAssignmentResult.Identities)
                Debug.Log($"[IdentityAssignment] {GetMockLobbyName(identity.PlayerId)} => {identity}");
        }

        private void AssignRolesForMockLobbyPlayers()
        {
            _lastRoleAssignmentResult = _roleAssignmentService.AssignRolesToLobbyPlayers(_mockLobbyPlayers);
            _selectedRevealView = null;
            _selectedPublicState = null;
            _selectedPrivateState = null;
            _selectedRevealIndex = 0;

            if (!_lastRoleAssignmentResult.Success)
            {
                Debug.LogWarning($"[RoleAssignment] Failed: {_lastRoleAssignmentResult.ErrorMessage}");
                return;
            }

            var distribution = _lastRoleAssignmentResult.Distribution;
            Debug.Log($"[RoleAssignment] Success: {distribution}");

            foreach (var assignment in _lastRoleAssignmentResult.Assignments)
            {
                Debug.Log($"[RoleAssignment] {assignment.DisplayName}: {assignment.PlayerId} => {assignment.Role}");

                if (assignment.Role == PlayerRole.Killer)
                {
                    var targets = string.Join(", ", assignment.KnownVictimTargets.Select(GetDisplayNameById));
                    Debug.Log($"[RoleAssignment] Killer known victims for {assignment.DisplayName}: {targets}");
                }
            }
        }

        private void BuildPublicPrivatePlayerStates()
        {
            if (_lastIdentityAssignmentResult == null || !_lastIdentityAssignmentResult.Success)
            {
                Debug.LogWarning("[PlayerStateBuilder] Cannot build states. Public identities missing.");
                return;
            }

            if (_lastRoleAssignmentResult == null || !_lastRoleAssignmentResult.Success)
            {
                Debug.LogWarning("[PlayerStateBuilder] Cannot build states. Role assignments missing.");
                return;
            }

            _lastPlayerStateBuildResult = _playerStateBuilder.BuildStates(
                _mockLobbyPlayers,
                _lastIdentityAssignmentResult.Identities,
                _lastRoleAssignmentResult.Assignments);

            if (!_lastPlayerStateBuildResult.Success)
            {
                Debug.LogWarning($"[PlayerStateBuilder] Failed: {_lastPlayerStateBuildResult.ErrorMessage}");
                return;
            }

            Debug.Log($"[PlayerStateBuilder] Success. PublicStates={_lastPlayerStateBuildResult.PublicStates.Count}, PrivateStates={_lastPlayerStateBuildResult.PrivateStates.Count}");
        }

        private void DrawMockLobbyPlayersWithIdentityAndRole()
        {
            GUILayout.Label("Mock Lobby Players");

            if (_mockLobbyPlayers == null || _mockLobbyPlayers.Count == 0)
            {
                GUILayout.Label("No mock lobby players.");
                return;
            }

            for (int i = 0; i < _mockLobbyPlayers.Count; i++)
            {
                var player = _mockLobbyPlayers[i];
                var identity = GetIdentity(player.PlayerId);
                var role = GetRole(player.PlayerId);

                string identityText = identity != null
                    ? identity.ToString()
                    : "No public identity";

                string roleText = role != PlayerRole.None
                    ? role.ToString()
                    : "No secret role";

                GUILayout.Label($"{player.PlayerIndex}. {player.DisplayName}");
                GUILayout.Label($"   Public Identity: {identityText}");
                GUILayout.Label($"   Secret Role(Debug Only): {roleText}");
            }
        }

        private void DrawPlayerStates()
        {
            GUILayout.Label("Public / Private Player States");

            if (_lastPlayerStateBuildResult == null)
            {
                GUILayout.Label("No player states built yet.");
                return;
            }

            if (!_lastPlayerStateBuildResult.Success)
            {
                GUILayout.Label($"Failed: {_lastPlayerStateBuildResult.ErrorMessage}");
                return;
            }

            GUILayout.Label("PUBLIC STATES — no secret role here");

            foreach (var publicState in _lastPlayerStateBuildResult.PublicStates)
                GUILayout.Label($"  {publicState}");

            GUILayout.Space(6);
            GUILayout.Label("PRIVATE STATES — owner-only data");

            foreach (var privateState in _lastPlayerStateBuildResult.PrivateStates)
            {
                string ownerName = GetDisplayNameById(privateState.OwnerPlayerId);
                GUILayout.Label($"  Owner={ownerName}, OwnRole={privateState.OwnRole}, KnownTargets={privateState.KnownVictimTargets.Count}");
            }
        }

        private void DrawOwnerPrivateStateInspector()
        {
            GUILayout.Label("Selected Owner State Inspector");

            if (_lastPlayerStateBuildResult == null || !_lastPlayerStateBuildResult.Success)
            {
                GUILayout.Label("Build player states first.");
                return;
            }

            if (_mockLobbyPlayers == null || _mockLobbyPlayers.Count == 0)
            {
                GUILayout.Label("No mock lobby players.");
                return;
            }

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("<"))
                _selectedRevealIndex = Mathf.Max(0, _selectedRevealIndex - 1);

            var selectedPlayer = _mockLobbyPlayers[_selectedRevealIndex];
            GUILayout.Label($"Selected: {selectedPlayer.DisplayName}", GUILayout.Width(220));

            if (GUILayout.Button(">"))
                _selectedRevealIndex = Mathf.Min(_mockLobbyPlayers.Count - 1, _selectedRevealIndex + 1);

            GUILayout.EndHorizontal();

            if (GUILayout.Button("Inspect Selected Public + Private State", GUILayout.Height(32)))
            {
                _selectedPublicState = _playerStateBuilder.GetPublicState(
                    selectedPlayer.PlayerId,
                    _lastPlayerStateBuildResult.PublicStates);

                _selectedPrivateState = _playerStateBuilder.GetPrivateState(
                    selectedPlayer.PlayerId,
                    _lastPlayerStateBuildResult.PrivateStates);

                _selectedRevealView = _roleAssignmentService.BuildDebugRevealViewForOwner(
                    selectedPlayer,
                    _mockLobbyPlayers,
                    _lastRoleAssignmentResult.Assignments);

                LogSelectedStateInspection(selectedPlayer);
            }

            if (_selectedPublicState == null || _selectedPrivateState == null)
            {
                GUILayout.Label("No selected state inspected yet.");
                return;
            }

            GUILayout.Space(4);
            GUILayout.Label("SELECTED PUBLIC STATE");
            GUILayout.Label($"Owner: {_selectedPublicState.DisplayName}");
            GUILayout.Label($"Public Identity: {_selectedPublicState.PublicIdentity}");
            GUILayout.Label($"Life State: {_selectedPublicState.LifeState}");
            GUILayout.Label("Secret Role: NOT PRESENT IN PUBLIC STATE");

            GUILayout.Space(4);
            GUILayout.Label("SELECTED PRIVATE STATE");
            GUILayout.Label($"Owner: {GetDisplayNameById(_selectedPrivateState.OwnerPlayerId)}");
            GUILayout.Label($"Own Role: {_selectedPrivateState.OwnRole}");
            GUILayout.Label($"Known Victim Targets: {GetPrivateTargetsSummary(_selectedPrivateState)}");

            if (_selectedPrivateState.OwnRole != PlayerRole.Killer && _selectedPrivateState.KnownVictimTargets.Count > 0)
                GUILayout.Label("ERROR: Non-killer should not know victim targets.");
        }

        private void LogSelectedStateInspection(MockLobbyPlayer selectedPlayer)
        {
            if (_selectedPublicState == null || _selectedPrivateState == null)
            {
                Debug.LogWarning("[PlayerStateInspector] Selected state missing.");
                return;
            }

            Debug.Log($"[PlayerStateInspector] Selected={selectedPlayer.DisplayName}");
            Debug.Log($"[PlayerStateInspector] PUBLIC => {_selectedPublicState}");
            Debug.Log("[PlayerStateInspector] PUBLIC => Secret role is not present.");
            Debug.Log($"[PlayerStateInspector] PRIVATE => OwnRole={_selectedPrivateState.OwnRole}, KnownTargets={GetPrivateTargetsSummary(_selectedPrivateState)}");
        }

        private PlayerPublicIdentity GetIdentity(PlayerId playerId)
        {
            if (_lastIdentityAssignmentResult == null || !_lastIdentityAssignmentResult.Success)
                return null;

            return _identityAssignmentService.GetIdentityForPlayer(playerId, _lastIdentityAssignmentResult.Identities);
        }

        private PlayerRole GetRole(PlayerId playerId)
        {
            if (_lastRoleAssignmentResult == null || !_lastRoleAssignmentResult.Success)
                return PlayerRole.None;

            var assignment = _lastRoleAssignmentResult.Assignments.FirstOrDefault(x => x.PlayerId == playerId);
            return assignment?.Role ?? PlayerRole.None;
        }

        private string GetPrivateTargetsSummary(PlayerPrivateState privateState)
        {
            if (privateState == null || privateState.KnownVictimTargets == null || privateState.KnownVictimTargets.Count == 0)
                return "No known targets.";

            return string.Join(", ", privateState.KnownVictimTargets.Select(GetDisplayNameById));
        }

        private string GetDisplayNameById(PlayerId playerId)
        {
            var player = _mockLobbyPlayers.FirstOrDefault(x => x.PlayerId == playerId);
            return player != null ? player.DisplayName : playerId.ToString();
        }

        private string GetMockLobbyName(PlayerId playerId)
        {
            var player = _mockLobbyPlayers.FirstOrDefault(x => x.PlayerId == playerId);
            return player != null ? player.DisplayName : playerId.ToString();
        }
    }
}