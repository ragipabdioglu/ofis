namespace OFIS.Meetings
{
    public sealed class MeetingActionReportPanelStateService
    {
        public MeetingActionReportPanelState BuildState(
            MeetingActionProposalResolutionResult resolutionResult,
            MeetingOfficialActionEffectResult effectResult)
        {
            MeetingActionProposalData proposal = ResolveProposal(resolutionResult, effectResult);
            bool hasProposal = !string.IsNullOrWhiteSpace(proposal.ProposalId);
            bool hasResolvedAction = resolutionResult.HasResolvedProposal
                || resolutionResult.ResolutionType == MeetingActionProposalResolutionType.TieCancelled;

            string actionSummary = hasProposal
                ? BuildActionSummary(proposal.Request.ActionType)
                : "No official action.";

            string targetSummary = hasProposal
                ? BuildTargetSummary(proposal.Request.Target)
                : "No target.";

            string resolutionSummary = BuildResolutionSummary(resolutionResult);
            string effectSummary = BuildEffectSummary(effectResult);

            return new MeetingActionReportPanelState(
                "Meeting Action Report",
                actionSummary,
                targetSummary,
                resolutionSummary,
                effectSummary,
                hasResolvedAction,
                effectResult.ShouldApplyEffect,
                true,
                false);
        }

        private static MeetingActionProposalData ResolveProposal(
            MeetingActionProposalResolutionResult resolutionResult,
            MeetingOfficialActionEffectResult effectResult)
        {
            if (!string.IsNullOrWhiteSpace(resolutionResult.Proposal.ProposalId))
                return resolutionResult.Proposal;

            return effectResult.Proposal;
        }

        private static string BuildActionSummary(MeetingActionType actionType)
        {
            switch (actionType)
            {
                case MeetingActionType.PersonnelAudit:
                    return "Action selected: personnel audit.";

                case MeetingActionType.RoomInspection:
                    return "Action selected: room inspection.";

                case MeetingActionType.TaskReportAudit:
                    return "Action selected: task report audit.";

                case MeetingActionType.SecurityRecordReview:
                    return "Action selected: security record review.";

                case MeetingActionType.OfficialAccusation:
                    return "Action selected: official accusation.";

                case MeetingActionType.NoAction:
                    return "Action selected: no action.";

                default:
                    return "No official action.";
            }
        }

        private static string BuildTargetSummary(MeetingActionTargetData target)
        {
            switch (target.TargetType)
            {
                case MeetingActionTargetType.Player:
                    return string.IsNullOrWhiteSpace(target.PlayerId)
                        ? "Target: player."
                        : $"Target player: {target.PlayerId}.";

                case MeetingActionTargetType.Room:
                    return $"Target room: {target.RoomType}.";

                case MeetingActionTargetType.Department:
                    return $"Target department: {target.DepartmentType}.";

                case MeetingActionTargetType.SecurityArea:
                    return $"Target security area: {target.SecurityAreaType}.";

                default:
                    return "No target.";
            }
        }

        private static string BuildResolutionSummary(
            MeetingActionProposalResolutionResult resolutionResult)
        {
            switch (resolutionResult.ResolutionType)
            {
                case MeetingActionProposalResolutionType.MajorityReached:
                    return $"Resolved by majority with {resolutionResult.VoteCount} vote(s).";

                case MeetingActionProposalResolutionType.TimeoutHighestVote:
                    return $"Resolved by timeout highest vote with {resolutionResult.VoteCount} vote(s).";

                case MeetingActionProposalResolutionType.TieCancelled:
                    return "Official action cancelled because the vote was tied.";

                default:
                    return "No official action resolved.";
            }
        }

        private static string BuildEffectSummary(MeetingOfficialActionEffectResult effectResult)
        {
            if (effectResult.ShouldApplyEffect)
                return "Official action effect ready to apply.";

            if (effectResult.Proposal.Request.ActionType == MeetingActionType.NoAction)
                return "NoAction selected; no effect applied.";

            return "No effect applied.";
        }
    }
}
