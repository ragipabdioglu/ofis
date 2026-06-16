using OFIS.Rooms;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Meetings
{
    public sealed class MeetingOfficialActionApplyDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        [Header("Read Only Debug Output")]
        [SerializeField] private string lastMeetingId;
        [SerializeField] private string lastProposalId;
        [SerializeField] private bool lastSuccess;
        [SerializeField] private bool lastHasAppliedOfficialAction;
        [SerializeField] private string lastMessage;

        private readonly MeetingOfficialActionApplyService _service =
            new MeetingOfficialActionApplyService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateOfficialActionApply();
        }

        [ContextMenu("Validate Meeting Official Action Apply")]
        public void ValidateOfficialActionApply()
        {
            ValidateFirstOfficialActionApplies();
            ValidateSecondOfficialActionIsBlocked();
            ValidateNoActionDoesNotMarkApplied();
            ValidateOpenProposalDoesNotApply();
            ValidateDifferentMeetingIsBlocked();
            ValidateResetAllowsNextMeetingAction();
        }

        private void ValidateFirstOfficialActionApplies()
        {
            MeetingOfficialActionApplyState state =
                new MeetingOfficialActionApplyState("meeting_apply_debug");

            MeetingOfficialActionApplyResult result = _service.TryApply(
                state,
                BuildProposal(
                    "proposal_apply_a",
                    "meeting_apply_debug",
                    MeetingActionType.RoomInspection,
                    MeetingActionTargetData.ForRoom(OfficeRoomType.ArchiveRoom),
                    MeetingActionProposalStatus.Resolved));

            bool passed = result.Success
                && state.HasAppliedOfficialAction
                && state.AppliedProposalId == "proposal_apply_a";

            LogResult("FirstOfficialActionApplies", passed, result);
        }

        private void ValidateSecondOfficialActionIsBlocked()
        {
            MeetingOfficialActionApplyState state =
                new MeetingOfficialActionApplyState("meeting_apply_debug");

            _service.TryApply(
                state,
                BuildProposal(
                    "proposal_apply_a",
                    "meeting_apply_debug",
                    MeetingActionType.RoomInspection,
                    MeetingActionTargetData.ForRoom(OfficeRoomType.ArchiveRoom),
                    MeetingActionProposalStatus.Resolved));

            MeetingOfficialActionApplyResult result = _service.TryApply(
                state,
                BuildProposal(
                    "proposal_apply_b",
                    "meeting_apply_debug",
                    MeetingActionType.PersonnelAudit,
                    MeetingActionTargetData.ForPlayer("player_01"),
                    MeetingActionProposalStatus.Resolved));

            bool passed = !result.Success
                && state.HasAppliedOfficialAction
                && state.AppliedProposalId == "proposal_apply_a";

            LogResult("SecondOfficialActionIsBlocked", passed, result);
        }

        private void ValidateNoActionDoesNotMarkApplied()
        {
            MeetingOfficialActionApplyState state =
                new MeetingOfficialActionApplyState("meeting_apply_debug");

            MeetingOfficialActionApplyResult result = _service.TryApply(
                state,
                BuildProposal(
                    "proposal_no_action",
                    "meeting_apply_debug",
                    MeetingActionType.NoAction,
                    MeetingActionTargetData.None(),
                    MeetingActionProposalStatus.Resolved));

            bool passed = !result.Success && !state.HasAppliedOfficialAction;
            LogResult("NoActionDoesNotMarkApplied", passed, result);
        }

        private void ValidateOpenProposalDoesNotApply()
        {
            MeetingOfficialActionApplyState state =
                new MeetingOfficialActionApplyState("meeting_apply_debug");

            MeetingOfficialActionApplyResult result = _service.TryApply(
                state,
                BuildProposal(
                    "proposal_open",
                    "meeting_apply_debug",
                    MeetingActionType.SecurityRecordReview,
                    MeetingActionTargetData.ForSecurityArea(MeetingSecurityAreaType.CameraSystem),
                    MeetingActionProposalStatus.Open));

            bool passed = !result.Success && !state.HasAppliedOfficialAction;
            LogResult("OpenProposalDoesNotApply", passed, result);
        }

        private void ValidateDifferentMeetingIsBlocked()
        {
            MeetingOfficialActionApplyState state =
                new MeetingOfficialActionApplyState("meeting_apply_debug");

            MeetingOfficialActionApplyResult result = _service.TryApply(
                state,
                BuildProposal(
                    "proposal_other_meeting",
                    "meeting_other",
                    MeetingActionType.RoomInspection,
                    MeetingActionTargetData.ForRoom(OfficeRoomType.SecurityRoom),
                    MeetingActionProposalStatus.Resolved));

            bool passed = !result.Success && !state.HasAppliedOfficialAction;
            LogResult("DifferentMeetingIsBlocked", passed, result);
        }

        private void ValidateResetAllowsNextMeetingAction()
        {
            MeetingOfficialActionApplyState state =
                new MeetingOfficialActionApplyState("meeting_apply_debug");

            _service.TryApply(
                state,
                BuildProposal(
                    "proposal_apply_a",
                    "meeting_apply_debug",
                    MeetingActionType.RoomInspection,
                    MeetingActionTargetData.ForRoom(OfficeRoomType.ArchiveRoom),
                    MeetingActionProposalStatus.Resolved));

            state.Reset("meeting_apply_next");

            MeetingOfficialActionApplyResult result = _service.TryApply(
                state,
                BuildProposal(
                    "proposal_apply_next",
                    "meeting_apply_next",
                    MeetingActionType.PersonnelAudit,
                    MeetingActionTargetData.ForPlayer("player_02"),
                    MeetingActionProposalStatus.Resolved));

            bool passed = result.Success
                && state.HasAppliedOfficialAction
                && state.AppliedProposalId == "proposal_apply_next";

            LogResult("ResetAllowsNextMeetingAction", passed, result);
        }

        private static MeetingActionProposalData BuildProposal(
            string proposalId,
            string meetingId,
            MeetingActionType actionType,
            MeetingActionTargetData target,
            MeetingActionProposalStatus status)
        {
            MeetingActionRequestData request = new MeetingActionRequestData(
                proposalId,
                "player_proposer",
                actionType,
                target,
                "Official action apply debug request.");

            return new MeetingActionProposalData(proposalId, meetingId, request, status);
        }

        private void LogResult(
            string testName,
            bool passed,
            MeetingOfficialActionApplyResult result)
        {
            lastMeetingId = result.MeetingId;
            lastProposalId = result.EffectResult.Proposal.ProposalId;
            lastSuccess = result.Success;
            lastHasAppliedOfficialAction = result.HasAppliedOfficialAction;
            lastMessage = result.Message;

            if (passed)
                Debug.Log($"[MeetingOfficialActionApplyValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[MeetingOfficialActionApplyValidator] FAIL {testName}: {result}");
        }
    }
}
#pragma warning restore 0414
