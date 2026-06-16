using OFIS.Rooms;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Meetings
{
    public sealed class MeetingActionPanelCommandDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        [Header("Read Only Debug Output")]
        [SerializeField] private string lastCommandId;
        [SerializeField] private MeetingActionType lastActionType;
        [SerializeField] private bool lastSuccess;
        [SerializeField] private int lastProposalCount;
        [SerializeField] private string lastMessage;

        private readonly MeetingActionPanelCommandService _service =
            new MeetingActionPanelCommandService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidatePanelCommand();
        }

        [ContextMenu("Validate Meeting Action Panel Command")]
        public void ValidatePanelCommand()
        {
            ValidateOpenPanelCreatesProposal();
            ValidateClosedPanelRejectsCommand();
            ValidateInvalidTargetRejectsCommand();
            ValidateNoActionCreatesNoTargetProposal();
            ValidateMeetingMismatchRejectsCommand();
            ValidateDuplicateCommandRejectsSecondProposal();
        }

        private void ValidateOpenPanelCreatesProposal()
        {
            MeetingActionProposalService proposalService = new MeetingActionProposalService();

            MeetingActionPanelCommandResult result = _service.SubmitSelection(
                BuildPanelState("meeting_command_001", true),
                BuildCommand(
                    "command_room",
                    "meeting_command_001",
                    MeetingActionType.RoomInspection,
                    MeetingActionTargetData.ForRoom(OfficeRoomType.ArchiveRoom)),
                proposalService);

            bool passed = result.Success
                && proposalService.ProposalCount == 1
                && proposalService.HasProposal("command_room");

            LogResult("OpenPanelCreatesProposal", passed, result, proposalService);
        }

        private void ValidateClosedPanelRejectsCommand()
        {
            MeetingActionProposalService proposalService = new MeetingActionProposalService();

            MeetingActionPanelCommandResult result = _service.SubmitSelection(
                BuildPanelState("meeting_command_002", false),
                BuildCommand(
                    "command_closed",
                    "meeting_command_002",
                    MeetingActionType.PersonnelAudit,
                    MeetingActionTargetData.ForPlayer("player_01")),
                proposalService);

            bool passed = !result.Success && proposalService.ProposalCount == 0;
            LogResult("ClosedPanelRejectsCommand", passed, result, proposalService);
        }

        private void ValidateInvalidTargetRejectsCommand()
        {
            MeetingActionProposalService proposalService = new MeetingActionProposalService();

            MeetingActionPanelCommandResult result = _service.SubmitSelection(
                BuildPanelState("meeting_command_003", true),
                BuildCommand(
                    "command_invalid_target",
                    "meeting_command_003",
                    MeetingActionType.SecurityRecordReview,
                    MeetingActionTargetData.ForRoom(OfficeRoomType.SecurityRoom)),
                proposalService);

            bool passed = !result.Success && proposalService.ProposalCount == 0;
            LogResult("InvalidTargetRejectsCommand", passed, result, proposalService);
        }

        private void ValidateNoActionCreatesNoTargetProposal()
        {
            MeetingActionProposalService proposalService = new MeetingActionProposalService();

            MeetingActionPanelCommandResult result = _service.SubmitSelection(
                BuildPanelState("meeting_command_004", true),
                BuildCommand(
                    "command_no_action",
                    "meeting_command_004",
                    MeetingActionType.NoAction,
                    MeetingActionTargetData.None()),
                proposalService);

            bool passed = result.Success
                && proposalService.ProposalCount == 1
                && proposalService.HasProposal("command_no_action");

            LogResult("NoActionCreatesNoTargetProposal", passed, result, proposalService);
        }

        private void ValidateMeetingMismatchRejectsCommand()
        {
            MeetingActionProposalService proposalService = new MeetingActionProposalService();

            MeetingActionPanelCommandResult result = _service.SubmitSelection(
                BuildPanelState("meeting_command_005", true),
                BuildCommand(
                    "command_mismatch",
                    "meeting_other",
                    MeetingActionType.RoomInspection,
                    MeetingActionTargetData.ForRoom(OfficeRoomType.MeetingRoom)),
                proposalService);

            bool passed = !result.Success && proposalService.ProposalCount == 0;
            LogResult("MeetingMismatchRejectsCommand", passed, result, proposalService);
        }

        private void ValidateDuplicateCommandRejectsSecondProposal()
        {
            MeetingActionProposalService proposalService = new MeetingActionProposalService();
            MeetingActionPanelState panelState = BuildPanelState("meeting_command_006", true);
            MeetingActionPanelCommand command = BuildCommand(
                "command_duplicate",
                "meeting_command_006",
                MeetingActionType.PersonnelAudit,
                MeetingActionTargetData.ForPlayer("player_02"));

            MeetingActionPanelCommandResult firstResult =
                _service.SubmitSelection(panelState, command, proposalService);
            MeetingActionPanelCommandResult secondResult =
                _service.SubmitSelection(panelState, command, proposalService);

            bool passed = firstResult.Success
                && !secondResult.Success
                && proposalService.ProposalCount == 1;

            LogResult("DuplicateCommandRejectsSecondProposal", passed, secondResult, proposalService);
        }

        private static MeetingActionPanelState BuildPanelState(
            string meetingId,
            bool isOpen)
        {
            return new MeetingActionPanelState(
                meetingId,
                MeetingRuntimePhaseType.Meeting,
                isOpen,
                isOpen,
                isOpen,
                isOpen ? "Panel open." : "Panel closed.");
        }

        private static MeetingActionPanelCommand BuildCommand(
            string commandId,
            string meetingId,
            MeetingActionType actionType,
            MeetingActionTargetData target)
        {
            return new MeetingActionPanelCommand(
                commandId,
                meetingId,
                "player_proposer",
                actionType,
                target,
                "Panel command debug.");
        }

        private void LogResult(
            string testName,
            bool passed,
            MeetingActionPanelCommandResult result,
            MeetingActionProposalService proposalService)
        {
            lastCommandId = result.Command.CommandId;
            lastActionType = result.Command.ActionType;
            lastSuccess = result.Success;
            lastProposalCount = proposalService == null ? 0 : proposalService.ProposalCount;
            lastMessage = result.Message;

            if (passed)
                Debug.Log($"[MeetingActionPanelCommandValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[MeetingActionPanelCommandValidator] FAIL {testName}: {result}");
        }
    }
}
#pragma warning restore 0414
