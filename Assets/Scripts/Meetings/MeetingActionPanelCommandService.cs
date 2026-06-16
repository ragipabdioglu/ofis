namespace OFIS.Meetings
{
    public sealed class MeetingActionPanelCommandService
    {
        private readonly MeetingActionTargetSelectionStateService _targetSelectionService;

        public MeetingActionPanelCommandService()
        {
            _targetSelectionService = new MeetingActionTargetSelectionStateService();
        }

        public MeetingActionPanelCommandService(
            MeetingActionTargetSelectionStateService targetSelectionService)
        {
            _targetSelectionService = targetSelectionService
                ?? new MeetingActionTargetSelectionStateService();
        }

        public MeetingActionPanelCommandResult SubmitSelection(
            MeetingActionPanelState panelState,
            MeetingActionPanelCommand command,
            MeetingActionProposalService proposalService)
        {
            if (!panelState.IsOpen || !panelState.CanSelectAction)
                return Failed(command, "Meeting action panel is not open.");

            if (string.IsNullOrWhiteSpace(command.MeetingId)
                || command.MeetingId != panelState.MeetingId)
            {
                return Failed(command, "Command meeting id does not match panel state.");
            }

            MeetingActionTargetSelectionState targetState =
                _targetSelectionService.BuildState(command.ActionType, command.Target);

            if (!targetState.HasValidSelection)
                return Failed(command, targetState.Message);

            if (proposalService == null)
                return Failed(command, "Proposal service is missing.");

            MeetingActionRequestData request = new MeetingActionRequestData(
                command.CommandId,
                command.ProposerPlayerId,
                command.ActionType,
                command.Target,
                command.Reason);

            MeetingActionProposalCreateResult proposalResult =
                proposalService.CreateProposal(command.MeetingId, request);

            return new MeetingActionPanelCommandResult(
                proposalResult.Success,
                command,
                proposalResult,
                proposalResult.Message);
        }

        private static MeetingActionPanelCommandResult Failed(
            MeetingActionPanelCommand command,
            string message)
        {
            return new MeetingActionPanelCommandResult(
                false,
                command,
                default(MeetingActionProposalCreateResult),
                message);
        }
    }
}
