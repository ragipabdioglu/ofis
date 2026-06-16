namespace OFIS.Meetings
{
    public sealed class MeetingActionPanelStateService
    {
        public MeetingActionPanelState BuildState(
            string meetingId,
            MeetingPhaseRuntimeHookState phaseState,
            MeetingOfficialActionApplyState applyState)
        {
            bool hasAppliedOfficialAction = applyState != null
                && applyState.HasAppliedOfficialAction;

            bool isMeetingPhase = phaseState.PhaseType == MeetingRuntimePhaseType.Meeting
                || phaseState.PhaseType == MeetingRuntimePhaseType.FinalMeeting;

            bool shouldOpen = phaseState.HasActivePhase
                && isMeetingPhase
                && !phaseState.HasEnded
                && !hasAppliedOfficialAction;

            string message = BuildMessage(phaseState, hasAppliedOfficialAction, shouldOpen);

            return new MeetingActionPanelState(
                meetingId,
                phaseState.PhaseType,
                shouldOpen,
                shouldOpen,
                shouldOpen,
                message);
        }

        public MeetingActionPanelState BuildState(
            string meetingId,
            MeetingRuntimePhaseType phaseType,
            bool hasActivePhase,
            bool hasEnded,
            bool hasAppliedOfficialAction)
        {
            MeetingPhaseRuntimeHookState phaseState = new MeetingPhaseRuntimeHookState(
                phaseType,
                hasEnded ? 0f : 60f,
                hasEnded ? 60f : 0f,
                5f,
                hasActivePhase,
                false,
                "Meeting action panel debug phase.");

            MeetingOfficialActionApplyState applyState =
                new MeetingOfficialActionApplyState(meetingId);

            if (hasAppliedOfficialAction)
            {
                applyState.MarkApplied(new MeetingActionProposalData(
                    "proposal_applied",
                    meetingId,
                    new MeetingActionRequestData(
                        "proposal_applied",
                        "player_proposer",
                        MeetingActionType.RoomInspection,
                        MeetingActionTargetData.None(),
                        "Debug applied action."),
                    MeetingActionProposalStatus.Resolved));
            }

            return BuildState(meetingId, phaseState, applyState);
        }

        private static string BuildMessage(
            MeetingPhaseRuntimeHookState phaseState,
            bool hasAppliedOfficialAction,
            bool shouldOpen)
        {
            if (shouldOpen)
                return "Meeting action panel is open.";

            if (!phaseState.HasActivePhase)
                return "Meeting action panel is closed because there is no active phase.";

            if (!phaseState.IsMeetingPhase)
                return "Meeting action panel is closed outside meeting phase.";

            if (phaseState.HasEnded)
                return "Meeting action panel is closed because meeting phase ended.";

            if (hasAppliedOfficialAction)
                return "Meeting action panel is closed because official action already applied.";

            return "Meeting action panel is closed.";
        }
    }
}
