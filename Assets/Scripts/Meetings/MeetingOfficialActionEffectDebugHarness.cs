using OFIS.Rooms;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Meetings
{
    public sealed class MeetingOfficialActionEffectDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        [Header("Read Only Debug Output")]
        [SerializeField] private string lastProposalId;
        [SerializeField] private MeetingActionType lastActionType;
        [SerializeField] private MeetingOfficialActionEffectType lastEffectType;
        [SerializeField] private bool lastShouldApplyEffect;
        [SerializeField] private string lastMessage;

        private readonly MeetingOfficialActionEffectService _service =
            new MeetingOfficialActionEffectService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateOfficialActionEffect();
        }

        [ContextMenu("Validate Meeting Official Action Effect")]
        public void ValidateOfficialActionEffect()
        {
            ValidateNoActionDoesNothing();
            ValidateResolvedOfficialActionCanApplyEffect();
            ValidateOpenProposalDoesNothing();
            ValidateCancelledProposalDoesNothing();
            ValidateInvalidActionDoesNothing();
        }

        private void ValidateNoActionDoesNothing()
        {
            MeetingOfficialActionEffectResult result = _service.Evaluate(
                BuildProposal(
                    "proposal_no_action",
                    MeetingActionType.NoAction,
                    MeetingActionTargetData.None(),
                    MeetingActionProposalStatus.Resolved));

            bool passed = !result.ShouldApplyEffect
                && result.EffectType == MeetingOfficialActionEffectType.None;

            LogResult("NoActionDoesNothing", passed, result);
        }

        private void ValidateResolvedOfficialActionCanApplyEffect()
        {
            MeetingOfficialActionEffectResult result = _service.Evaluate(
                BuildProposal(
                    "proposal_room_inspection",
                    MeetingActionType.RoomInspection,
                    MeetingActionTargetData.ForRoom(OfficeRoomType.ArchiveRoom),
                    MeetingActionProposalStatus.Resolved));

            bool passed = result.ShouldApplyEffect
                && result.EffectType == MeetingOfficialActionEffectType.ApplyOfficialAction;

            LogResult("ResolvedOfficialActionCanApplyEffect", passed, result);
        }

        private void ValidateOpenProposalDoesNothing()
        {
            MeetingOfficialActionEffectResult result = _service.Evaluate(
                BuildProposal(
                    "proposal_open",
                    MeetingActionType.PersonnelAudit,
                    MeetingActionTargetData.ForPlayer("player_01"),
                    MeetingActionProposalStatus.Open));

            bool passed = !result.ShouldApplyEffect
                && result.EffectType == MeetingOfficialActionEffectType.None;

            LogResult("OpenProposalDoesNothing", passed, result);
        }

        private void ValidateCancelledProposalDoesNothing()
        {
            MeetingOfficialActionEffectResult result = _service.Evaluate(
                BuildProposal(
                    "proposal_cancelled",
                    MeetingActionType.OfficialAccusation,
                    MeetingActionTargetData.ForPlayer("player_02"),
                    MeetingActionProposalStatus.Cancelled));

            bool passed = !result.ShouldApplyEffect
                && result.EffectType == MeetingOfficialActionEffectType.None;

            LogResult("CancelledProposalDoesNothing", passed, result);
        }

        private void ValidateInvalidActionDoesNothing()
        {
            MeetingOfficialActionEffectResult result = _service.Evaluate(
                BuildProposal(
                    "proposal_invalid",
                    MeetingActionType.None,
                    MeetingActionTargetData.None(),
                    MeetingActionProposalStatus.Resolved));

            bool passed = !result.ShouldApplyEffect
                && result.EffectType == MeetingOfficialActionEffectType.None;

            LogResult("InvalidActionDoesNothing", passed, result);
        }

        private static MeetingActionProposalData BuildProposal(
            string proposalId,
            MeetingActionType actionType,
            MeetingActionTargetData target,
            MeetingActionProposalStatus status)
        {
            MeetingActionRequestData request = new MeetingActionRequestData(
                proposalId,
                "player_proposer",
                actionType,
                target,
                "Official action effect debug request.");

            return new MeetingActionProposalData(
                proposalId,
                "meeting_effect_debug",
                request,
                status);
        }

        private void LogResult(
            string testName,
            bool passed,
            MeetingOfficialActionEffectResult result)
        {
            lastProposalId = result.Proposal.ProposalId;
            lastActionType = result.Proposal.Request.ActionType;
            lastEffectType = result.EffectType;
            lastShouldApplyEffect = result.ShouldApplyEffect;
            lastMessage = result.Message;

            if (passed)
                Debug.Log($"[MeetingOfficialActionEffectValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[MeetingOfficialActionEffectValidator] FAIL {testName}: {result}");
        }
    }
}
#pragma warning restore 0414
