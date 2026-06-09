using OFIS.Rooms;
using UnityEngine;

namespace OFIS.Rules
{
    public sealed class RoomBasedRuleGuard : MonoBehaviour
    {
        [SerializeField] private PlayerRoomTracker roomTracker;

        private void Awake()
        {
            if (roomTracker == null)
                roomTracker = GetComponent<PlayerRoomTracker>();
        }

        public PlayerActionRuleResult CanPerform(PlayerActionType actionType)
        {
            if (roomTracker == null)
                return PlayerActionRuleResult.Deny("RoomTracker missing.");

            return actionType switch
            {
                PlayerActionType.Kill => CanKill(),
                PlayerActionType.CarryCorpse => CanCarryCorpse(),
                PlayerActionType.HideCorpse => CanHideCorpse(),
                PlayerActionType.DoTask => PlayerActionRuleResult.Allow(),
                PlayerActionType.ReportFinding => PlayerActionRuleResult.Allow(),
                PlayerActionType.JoinMeeting => PlayerActionRuleResult.Allow(),
                _ => PlayerActionRuleResult.Deny("Unknown action type.")
            };
        }

        private PlayerActionRuleResult CanKill()
        {
            if (roomTracker.CurrentRoomType == OfficeRoomType.MeetingRoom)
                return PlayerActionRuleResult.Deny("Kill is not allowed inside MeetingRoom.");

            return PlayerActionRuleResult.Allow();
        }

        private PlayerActionRuleResult CanCarryCorpse()
        {
            if (roomTracker.CurrentRoomType == OfficeRoomType.MeetingRoom)
                return PlayerActionRuleResult.Deny("Carrying corpse is not allowed inside MeetingRoom.");

            return PlayerActionRuleResult.Allow();
        }

        private PlayerActionRuleResult CanHideCorpse()
        {
            if (roomTracker.CurrentRoomType == OfficeRoomType.MeetingRoom)
                return PlayerActionRuleResult.Deny("Hiding corpse is not allowed inside MeetingRoom.");

            return PlayerActionRuleResult.Allow();
        }
    }
}