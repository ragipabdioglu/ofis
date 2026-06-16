using OFIS.Rooms;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Meetings
{
    public sealed class MeetingActionProposalDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        [Header("Read Only Debug Output")]
        [SerializeField] private string lastProposalId;
        [SerializeField] private string lastMeetingId;
        [SerializeField] private int lastProposalCount;
        [SerializeField] private bool lastSuccess;
        [SerializeField] private string lastMessage;

        private readonly MeetingActionProposalService _service =
            new MeetingActionProposalService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateMeetingActionProposalCore();
        }

        [ContextMenu("Validate Meeting Action Proposal Core")]
        public void ValidateMeetingActionProposalCore()
        {
            _service.ClearProposals();

            ValidateCreatesValidProposal();
            ValidateRejectsInvalidActionTarget();
            ValidateRejectsMissingMeetingId();
            ValidateRejectsDuplicateProposalId();
            ValidateClearProposals();
        }

        private void ValidateCreatesValidProposal()
        {
            MeetingActionProposalCreateResult result = _service.CreateProposal(
                "meeting_001",
                BuildRequest(
                    "proposal_valid",
                    MeetingActionType.RoomInspection,
                    MeetingActionTargetData.ForRoom(OfficeRoomType.ArchiveRoom)));

            bool passed = result.Success
                && result.Proposal.Status == MeetingActionProposalStatus.Open
                && _service.HasProposal("proposal_valid");

            LogResult("CreatesValidProposal", passed, result);
        }

        private void ValidateRejectsInvalidActionTarget()
        {
            MeetingActionProposalCreateResult result = _service.CreateProposal(
                "meeting_001",
                BuildRequest(
                    "proposal_invalid_target",
                    MeetingActionType.SecurityRecordReview,
                    MeetingActionTargetData.ForRoom(OfficeRoomType.SecurityRoom)));

            bool passed = !result.Success && !_service.HasProposal("proposal_invalid_target");
            LogResult("RejectsInvalidActionTarget", passed, result);
        }

        private void ValidateRejectsMissingMeetingId()
        {
            MeetingActionProposalCreateResult result = _service.CreateProposal(
                string.Empty,
                BuildRequest(
                    "proposal_missing_meeting",
                    MeetingActionType.NoAction,
                    MeetingActionTargetData.None()));

            bool passed = !result.Success && !_service.HasProposal("proposal_missing_meeting");
            LogResult("RejectsMissingMeetingId", passed, result);
        }

        private void ValidateRejectsDuplicateProposalId()
        {
            MeetingActionProposalCreateResult firstResult = _service.CreateProposal(
                "meeting_001",
                BuildRequest(
                    "proposal_duplicate",
                    MeetingActionType.PersonnelAudit,
                    MeetingActionTargetData.ForPlayer("player_01")));

            MeetingActionProposalCreateResult secondResult = _service.CreateProposal(
                "meeting_001",
                BuildRequest(
                    "proposal_duplicate",
                    MeetingActionType.PersonnelAudit,
                    MeetingActionTargetData.ForPlayer("player_02")));

            bool passed = firstResult.Success && !secondResult.Success;
            LogResult("RejectsDuplicateProposalId", passed, secondResult);
        }

        private void ValidateClearProposals()
        {
            _service.ClearProposals();

            MeetingActionProposalCreateResult result = new MeetingActionProposalCreateResult(
                _service.ProposalCount == 0,
                default(MeetingActionProposalData),
                "Proposal store cleared.");

            LogResult("ClearProposals", result.Success, result);
        }

        private static MeetingActionRequestData BuildRequest(
            string actionId,
            MeetingActionType actionType,
            MeetingActionTargetData target)
        {
            return new MeetingActionRequestData(
                actionId,
                "player_proposer",
                actionType,
                target,
                "Debug proposal request.");
        }

        private void LogResult(
            string testName,
            bool passed,
            MeetingActionProposalCreateResult result)
        {
            lastProposalId = result.Proposal.ProposalId;
            lastMeetingId = result.Proposal.MeetingId;
            lastProposalCount = _service.ProposalCount;
            lastSuccess = result.Success;
            lastMessage = result.Message;

            if (passed)
                Debug.Log($"[MeetingActionProposalValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[MeetingActionProposalValidator] FAIL {testName}: {result}");
        }
    }
}
#pragma warning restore 0414
