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
            OwnRole = entry.PrivateState != null ? entry.PrivateState.OwnRole : PlayerRole.None;

            IsBound = true;

            string department = PublicIdentity == null ? "Unknown" : PublicIdentity.Department.ToString();
            int knownTargetCount = PrivateState == null || PrivateState.KnownVictimTargets == null
                ? 0
                : PrivateState.KnownVictimTargets.Count;

            Debug.Log(
                $"[LocalPlayerBinding] Bound LocalPlayer to {DisplayName}. " +
                $"Role={OwnRole}, " +
                $"Department={department}, " +
                $"KnownTargets={knownTargetCount}");
        }

        public string GetDebugSummary()
        {
            if (!IsBound)
                return "LocalPlayer is not bound.";

            string roomText = GetRoomDebugText();
            string departmentText = PublicIdentity == null ? "Unknown" : PublicIdentity.Department.ToString();
            string jobTitleText = PublicIdentity == null || string.IsNullOrWhiteSpace(PublicIdentity.JobTitle)
                ? "Unknown"
                : PublicIdentity.JobTitle;
            int knownTargetCount = PrivateState == null || PrivateState.KnownVictimTargets == null
                ? 0
                : PrivateState.KnownVictimTargets.Count;

            return
                $"LocalPlayer Binding\n" +
                $"Player: {DisplayName}\n" +
                $"Role: {OwnRole}\n" +
                $"Department: {departmentText}\n" +
                $"Job: {jobTitleText}\n" +
                $"KnownTargets: {knownTargetCount}\n" +
                $"CurrentRoom: {roomText}";
        }

        private string GetRoomDebugText()
        {
            if (RoomTracker == null)
                return "No RoomTracker";

            string roomName = string.IsNullOrWhiteSpace(RoomTracker.CurrentRoomDisplayName)
                ? "Unknown Room"
                : RoomTracker.CurrentRoomDisplayName;

            return $"{RoomTracker.CurrentRoomType} ({roomName})";
        }
    }
}
