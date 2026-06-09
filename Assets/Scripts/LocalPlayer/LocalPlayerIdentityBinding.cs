using OFIS.Core.Ids;
using OFIS.MatchContext;
using OFIS.Players;
using OFIS.Roles;
using OFIS.Roles.Identity;
using OFIS.Rooms;
using UnityEngine;

namespace OFIS.LocalPlayer
{
    public sealed class LocalPlayerIdentityBinding : MonoBehaviour
    {
        [SerializeField] private int mockPlayerIndex = 0;
        [SerializeField] private bool bindOnStart = true;

        public bool IsBound { get; private set; }

        public PlayerId PlayerId { get; private set; }
        public string DisplayName { get; private set; } = "Unbound";

        public PlayerPublicIdentity PublicIdentity { get; private set; }
        public PlayerPublicState PublicState { get; private set; }
        public PlayerPrivateState PrivateState { get; private set; }
        public PlayerRole OwnRole { get; private set; }
        public PlayerRoomTracker RoomTracker { get; private set; }

        private void Start()
        {
            RoomTracker = GetComponent<PlayerRoomTracker>();

            if (bindOnStart)
                BindToMockContext();
        }

        [ContextMenu("Bind To Mock Context")]
        public void BindToMockContext()
        {
            var builder = new MockMatchContextBuilder(
                new RoleAssignmentService(seed: 12345),
                new IdentityAssignmentService(seed: 54321),
                new PlayerStateBuilder());

            var result = builder.Build(8);

            if (!result.Success)
            {
                Debug.LogWarning($"[LocalPlayerBinding] Failed to build mock context: {result.ErrorMessage}");
                return;
            }

            var context = result.Context;

            if (mockPlayerIndex < 0 || mockPlayerIndex >= context.Registry.Entries.Count)
            {
                Debug.LogWarning($"[LocalPlayerBinding] Invalid mockPlayerIndex: {mockPlayerIndex}");
                return;
            }

            var entry = context.Registry.Entries[mockPlayerIndex];

            PlayerId = entry.LobbyPlayer.PlayerId;
            DisplayName = entry.LobbyPlayer.DisplayName;

            PublicIdentity = entry.PublicIdentity;
            PublicState = entry.PublicState;
            PrivateState = entry.PrivateState;
            OwnRole = entry.PrivateState.OwnRole;

            IsBound = true;

            Debug.Log(
                $"[LocalPlayerBinding] Bound LocalPlayer to {DisplayName}. " +
                $"Role={OwnRole}, " +
                $"Department={PublicIdentity.Department}, " +
                $"KnownTargets={PrivateState.KnownVictimTargets.Count}");
        }

        public string GetDebugSummary()
        {
            if (!IsBound)
                return "LocalPlayer is not bound.";

            string roomText = RoomTracker == null
                ? "No RoomTracker"
                : $"{RoomTracker.CurrentRoomType} ({RoomTracker.CurrentRoomDisplayName})";

            return
                $"LocalPlayer Binding\n" +
                $"Player: {DisplayName}\n" +
                $"Role: {OwnRole}\n" +
                $"Department: {PublicIdentity.Department}\n" +
                $"Job: {PublicIdentity.JobTitle}\n" +
                $"KnownTargets: {PrivateState.KnownVictimTargets.Count}\n" +
                $"CurrentRoom: {roomText}";
        }
    }
}