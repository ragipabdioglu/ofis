using OFIS.Players;
using OFIS.Roles;
using OFIS.Roles.Identity;
using UnityEngine;

namespace OFIS.MatchContext
{
    public sealed class MockMatchContextDebugTester : MonoBehaviour
    {
        [SerializeField] private int playerCount = 8;
        [SerializeField] private bool buildOnStart = false;

        private MockMatchContext _lastContext;

        private void Start()
        {
            if (buildOnStart)
                BuildAndLog();
        }

        [ContextMenu("Build And Log Mock Match Context")]
        public void BuildAndLog()
        {
            var builder = new MockMatchContextBuilder(
                new RoleAssignmentService(seed: 12345),
                new IdentityAssignmentService(seed: 54321),
                new PlayerStateBuilder());

            var result = builder.Build(playerCount);

            if (!result.Success)
            {
                Debug.LogWarning($"[MockMatchContext] Build failed: {result.ErrorMessage}");
                return;
            }

            _lastContext = result.Context;

            LogContext(_lastContext);
        }

        private static void LogContext(MockMatchContext context)
        {
            Debug.Log($"[MockMatchContext] Build success. MatchId={context.MatchId}");
            Debug.Log($"[MockMatchContext] Players={context.Registry.Count}");
            Debug.Log($"[MockMatchContext] Killers={context.Registry.GetKillers().Count}");
            Debug.Log($"[MockMatchContext] Victims={context.Registry.GetVictims().Count}");
            Debug.Log($"[MockMatchContext] Detectives={context.Registry.GetDetectives().Count}");

            foreach (var entry in context.Registry.Entries)
            {
                Debug.Log($"[MockMatchContext] RegistryEntry => {entry}");

                Debug.Log($"[MockMatchContext] PUBLIC => {entry.PublicState}");
                Debug.Log("[MockMatchContext] PUBLIC => Secret role is not present.");

                Debug.Log(
                    $"[MockMatchContext] PRIVATE => Owner={entry.LobbyPlayer.DisplayName}, " +
                    $"OwnRole={entry.PrivateState.OwnRole}, " +
                    $"KnownTargets={entry.PrivateState.KnownVictimTargets.Count}");
            }
        }
    }
}