using OFIS.Rooms;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Meetings
{
    public sealed class MeetingActionReportPanelDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        [Header("Read Only Debug Output")]
        [SerializeField] private string lastActionSummary;
        [SerializeField] private string lastTargetSummary;
        [SerializeField] private string lastResolutionSummary;
        [SerializeField] private string lastEffectSummary;
        [SerializeField] private bool lastIsRoleSafe;
        [SerializeField] private bool lastRevealsDefiniteKiller;

        private readonly MeetingActionReportPanelStateService _service =
            new MeetingActionReportPanelStateService();

        private readonly MeetingOfficialActionEffectService _effectService =
            new MeetingOfficialActionEffectService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateReportPanelState();
        }

        [ContextMenu("Validate Meeting Action Report Panel State")]
        public void ValidateReportPanelState()
        {
            ValidateMajorityActionPanelState();
            ValidateNoActionPanelState();
            ValidateTieCancelledPanelState();
            ValidateNoResolutionPanelState();
            ValidatePanelDoesNotRevealRoleOrDefiniteKiller();
        }

        private void ValidateMajorityActionPanelState()
        {
            MeetingActionProposalData proposal = BuildProposal(
                "proposal_panel_majority",
                MeetingActionType.RoomInspection,
                MeetingActionTargetData.ForRoom(OfficeRoomType.ArchiveRoom),
                MeetingActionProposalStatus.Resolved);

            MeetingActionProposalResolutionResult resolutionResult = BuildResolution(
                true,
                MeetingActionProposalResolutionType.MajorityReached,
                proposal,
                2);

            MeetingActionReportPanelState state = _service.BuildState(
                resolutionResult,
                _effectService.Evaluate(proposal));

            bool passed = state.HasResolvedAction
                && state.HasAppliedEffect
                && ContainsText(state.ActionSummaryText, "room inspection")
                && ContainsText(state.ResolutionSummaryText, "majority");

            LogResult("MajorityActionPanelState", passed, state);
        }

        private void ValidateNoActionPanelState()
        {
            MeetingActionProposalData proposal = BuildProposal(
                "proposal_panel_no_action",
                MeetingActionType.NoAction,
                MeetingActionTargetData.None(),
                MeetingActionProposalStatus.Resolved);

            MeetingActionReportPanelState state = _service.BuildState(
                BuildResolution(
                    true,
                    MeetingActionProposalResolutionType.MajorityReached,
                    proposal,
                    2),
                _effectService.Evaluate(proposal));

            bool passed = state.HasResolvedAction
                && !state.HasAppliedEffect
                && ContainsText(state.EffectSummaryText, "NoAction");

            LogResult("NoActionPanelState", passed, state);
        }

        private void ValidateTieCancelledPanelState()
        {
            MeetingActionReportPanelState state = _service.BuildState(
                BuildResolution(
                    false,
                    MeetingActionProposalResolutionType.TieCancelled,
                    default(MeetingActionProposalData),
                    1),
                default(MeetingOfficialActionEffectResult));

            bool passed = state.HasResolvedAction
                && !state.HasAppliedEffect
                && ContainsText(state.ResolutionSummaryText, "tied");

            LogResult("TieCancelledPanelState", passed, state);
        }

        private void ValidateNoResolutionPanelState()
        {
            MeetingActionReportPanelState state = _service.BuildState(
                BuildResolution(
                    false,
                    MeetingActionProposalResolutionType.None,
                    default(MeetingActionProposalData),
                    0),
                default(MeetingOfficialActionEffectResult));

            bool passed = !state.HasResolvedAction
                && !state.HasAppliedEffect
                && ContainsText(state.ResolutionSummaryText, "No official action");

            LogResult("NoResolutionPanelState", passed, state);
        }

        private void ValidatePanelDoesNotRevealRoleOrDefiniteKiller()
        {
            MeetingActionProposalData proposal = BuildProposal(
                "proposal_panel_safety",
                MeetingActionType.OfficialAccusation,
                MeetingActionTargetData.ForPlayer("player_suspect"),
                MeetingActionProposalStatus.Resolved);

            MeetingActionReportPanelState state = _service.BuildState(
                BuildResolution(
                    true,
                    MeetingActionProposalResolutionType.TimeoutHighestVote,
                    proposal,
                    3),
                _effectService.Evaluate(proposal));

            string visibleText = state.HeaderText
                + state.ActionSummaryText
                + state.TargetSummaryText
                + state.ResolutionSummaryText
                + state.EffectSummaryText;

            bool passed = state.IsRoleSafe
                && !state.RevealsDefiniteKiller
                && !ContainsText(visibleText, "killer")
                && !ContainsText(visibleText, "role");

            LogResult("PanelDoesNotRevealRoleOrDefiniteKiller", passed, state);
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
                "Report panel debug request.");

            return new MeetingActionProposalData(
                proposalId,
                "meeting_panel_debug",
                request,
                status);
        }

        private static MeetingActionProposalResolutionResult BuildResolution(
            bool hasResolvedProposal,
            MeetingActionProposalResolutionType resolutionType,
            MeetingActionProposalData proposal,
            int voteCount)
        {
            return new MeetingActionProposalResolutionResult(
                hasResolvedProposal,
                resolutionType,
                proposal,
                3,
                2,
                voteCount,
                "Report panel debug resolution.");
        }

        private static bool ContainsText(string source, string expected)
        {
            if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(expected))
                return false;

            return source.IndexOf(expected, System.StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void LogResult(
            string testName,
            bool passed,
            MeetingActionReportPanelState state)
        {
            lastActionSummary = state.ActionSummaryText;
            lastTargetSummary = state.TargetSummaryText;
            lastResolutionSummary = state.ResolutionSummaryText;
            lastEffectSummary = state.EffectSummaryText;
            lastIsRoleSafe = state.IsRoleSafe;
            lastRevealsDefiniteKiller = state.RevealsDefiniteKiller;

            if (passed)
                Debug.Log($"[MeetingActionReportPanelValidator] PASS {testName}: {state}");
            else
                Debug.LogError($"[MeetingActionReportPanelValidator] FAIL {testName}: {state}");
        }
    }
}
#pragma warning restore 0414
