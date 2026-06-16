using OFIS.Rooms;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Meetings
{
    public sealed class MeetingActionPhaseSixIntegrationDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        [Header("Read Only Debug Output")]
        [SerializeField] private string lastScenario;
        [SerializeField] private bool lastPassed;
        [SerializeField] private string lastMessage;

        private readonly MeetingActionPanelCommandService _panelCommandService =
            new MeetingActionPanelCommandService();
        private readonly MeetingActionVoteService _voteService =
            new MeetingActionVoteService();
        private readonly MeetingActionProposalResolutionService _majorityResolutionService =
            new MeetingActionProposalResolutionService();
        private readonly MeetingActionTimeoutResolutionService _timeoutResolutionService =
            new MeetingActionTimeoutResolutionService();
        private readonly MeetingActionTieResolutionService _tieResolutionService =
            new MeetingActionTieResolutionService();
        private readonly MeetingOfficialActionEffectService _effectService =
            new MeetingOfficialActionEffectService();
        private readonly MeetingOfficialActionApplyService _applyService =
            new MeetingOfficialActionApplyService();
        private readonly MeetingActionReportPanelStateService _reportPanelService =
            new MeetingActionReportPanelStateService();
        private readonly MeetingActionReportSafetyGuardService _safetyGuardService =
            new MeetingActionReportSafetyGuardService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidatePhaseSixIntegration();
        }

        [ContextMenu("Validate Meeting Action Phase Six Integration")]
        public void ValidatePhaseSixIntegration()
        {
            ValidateMajorityOfficialActionFlow();
            ValidateNoActionFlowHasNoEffect();
            ValidateTimeoutHighestVoteFlow();
            ValidateTieCancelFlow();
            ValidateSingleOfficialActionGuardInFlow();
        }

        private void ValidateMajorityOfficialActionFlow()
        {
            MeetingActionProposalService proposalService = new MeetingActionProposalService();
            MeetingActionPanelCommandResult commandResult = _panelCommandService.SubmitSelection(
                BuildOpenPanel("meeting_phase6_majority"),
                BuildCommand(
                    "proposal_phase6_majority",
                    "meeting_phase6_majority",
                    MeetingActionType.RoomInspection,
                    MeetingActionTargetData.ForRoom(OfficeRoomType.ArchiveRoom)),
                proposalService);

            _voteService.ClearVotes();
            SubmitVote("vote_majority_1", "player_01", "proposal_phase6_majority", proposalService);
            SubmitVote("vote_majority_2", "player_02", "proposal_phase6_majority", proposalService);

            MeetingActionProposalResolutionResult resolution =
                _majorityResolutionService.ResolveMajority(
                    proposalService,
                    _voteService.Votes,
                    new[] { "player_01", "player_02", "player_03" });

            MeetingOfficialActionEffectResult effect =
                _effectService.Evaluate(resolution.Proposal);

            MeetingOfficialActionApplyState applyState =
                new MeetingOfficialActionApplyState("meeting_phase6_majority");
            MeetingOfficialActionApplyResult applyResult =
                _applyService.TryApply(applyState, resolution.Proposal);

            MeetingActionReportPanelState panelState =
                _reportPanelService.BuildState(resolution, effect);
            MeetingActionReportSafetyResult safetyResult =
                _safetyGuardService.Evaluate(panelState);

            bool passed = commandResult.Success
                && resolution.HasResolvedProposal
                && resolution.ResolutionType == MeetingActionProposalResolutionType.MajorityReached
                && effect.ShouldApplyEffect
                && applyResult.Success
                && safetyResult.IsSafe;

            LogResult("MajorityOfficialActionFlow", passed, safetyResult.Message);
        }

        private void ValidateNoActionFlowHasNoEffect()
        {
            MeetingActionProposalService proposalService = new MeetingActionProposalService();
            MeetingActionPanelCommandResult commandResult = _panelCommandService.SubmitSelection(
                BuildOpenPanel("meeting_phase6_no_action"),
                BuildCommand(
                    "proposal_phase6_no_action",
                    "meeting_phase6_no_action",
                    MeetingActionType.NoAction,
                    MeetingActionTargetData.None()),
                proposalService);

            _voteService.ClearVotes();
            SubmitVote("vote_no_action_1", "player_01", "proposal_phase6_no_action", proposalService);
            SubmitVote("vote_no_action_2", "player_02", "proposal_phase6_no_action", proposalService);

            MeetingActionProposalResolutionResult resolution =
                _majorityResolutionService.ResolveMajority(
                    proposalService,
                    _voteService.Votes,
                    new[] { "player_01", "player_02", "player_03" });

            MeetingOfficialActionEffectResult effect =
                _effectService.Evaluate(resolution.Proposal);

            MeetingOfficialActionApplyState applyState =
                new MeetingOfficialActionApplyState("meeting_phase6_no_action");
            MeetingOfficialActionApplyResult applyResult =
                _applyService.TryApply(applyState, resolution.Proposal);

            bool passed = commandResult.Success
                && resolution.HasResolvedProposal
                && !effect.ShouldApplyEffect
                && !applyResult.Success
                && !applyState.HasAppliedOfficialAction;

            LogResult("NoActionFlowHasNoEffect", passed, effect.Message);
        }

        private void ValidateTimeoutHighestVoteFlow()
        {
            MeetingActionProposalService proposalService = BuildTwoProposalService(
                "meeting_phase6_timeout",
                "proposal_timeout_a",
                "proposal_timeout_b");

            _voteService.ClearVotes();
            SubmitVote("vote_timeout_1", "player_01", "proposal_timeout_a", proposalService);
            SubmitVote("vote_timeout_2", "player_02", "proposal_timeout_a", proposalService);
            SubmitVote("vote_timeout_3", "player_03", "proposal_timeout_b", proposalService);

            MeetingActionProposalResolutionResult resolution =
                _timeoutResolutionService.ResolveTimeoutHighestVote(
                    proposalService,
                    _voteService.Votes);

            bool passed = resolution.HasResolvedProposal
                && resolution.ResolutionType == MeetingActionProposalResolutionType.TimeoutHighestVote
                && resolution.Proposal.ProposalId == "proposal_timeout_a";

            LogResult("TimeoutHighestVoteFlow", passed, resolution.Message);
        }

        private void ValidateTieCancelFlow()
        {
            MeetingActionProposalService proposalService = BuildTwoProposalService(
                "meeting_phase6_tie",
                "proposal_tie_a",
                "proposal_tie_b");

            _voteService.ClearVotes();
            SubmitVote("vote_tie_1", "player_01", "proposal_tie_a", proposalService);
            SubmitVote("vote_tie_2", "player_02", "proposal_tie_b", proposalService);

            MeetingActionProposalResolutionResult resolution =
                _tieResolutionService.ResolveTieCancel(
                    proposalService,
                    _voteService.Votes);

            MeetingActionProposalData proposalA;
            MeetingActionProposalData proposalB;
            bool passed = resolution.ResolutionType == MeetingActionProposalResolutionType.TieCancelled
                && proposalService.TryGetProposal("proposal_tie_a", out proposalA)
                && proposalA.Status == MeetingActionProposalStatus.Cancelled
                && proposalService.TryGetProposal("proposal_tie_b", out proposalB)
                && proposalB.Status == MeetingActionProposalStatus.Cancelled;

            LogResult("TieCancelFlow", passed, resolution.Message);
        }

        private void ValidateSingleOfficialActionGuardInFlow()
        {
            MeetingOfficialActionApplyState applyState =
                new MeetingOfficialActionApplyState("meeting_phase6_guard");

            MeetingActionProposalData firstProposal = BuildResolvedProposal(
                "meeting_phase6_guard",
                "proposal_guard_first",
                MeetingActionType.RoomInspection,
                MeetingActionTargetData.ForRoom(OfficeRoomType.ArchiveRoom));

            MeetingActionProposalData secondProposal = BuildResolvedProposal(
                "meeting_phase6_guard",
                "proposal_guard_second",
                MeetingActionType.PersonnelAudit,
                MeetingActionTargetData.ForPlayer("player_01"));

            MeetingOfficialActionApplyResult firstResult =
                _applyService.TryApply(applyState, firstProposal);
            MeetingOfficialActionApplyResult secondResult =
                _applyService.TryApply(applyState, secondProposal);

            bool passed = firstResult.Success
                && !secondResult.Success
                && applyState.AppliedProposalId == "proposal_guard_first";

            LogResult("SingleOfficialActionGuardInFlow", passed, secondResult.Message);
        }

        private static MeetingActionPanelState BuildOpenPanel(string meetingId)
        {
            return new MeetingActionPanelState(
                meetingId,
                MeetingRuntimePhaseType.Meeting,
                true,
                true,
                true,
                "Integration panel open.");
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
                "Phase six integration command.");
        }

        private MeetingActionProposalService BuildTwoProposalService(
            string meetingId,
            string firstProposalId,
            string secondProposalId)
        {
            MeetingActionProposalService proposalService = new MeetingActionProposalService();
            _panelCommandService.SubmitSelection(
                BuildOpenPanel(meetingId),
                BuildCommand(
                    firstProposalId,
                    meetingId,
                    MeetingActionType.RoomInspection,
                    MeetingActionTargetData.ForRoom(OfficeRoomType.ArchiveRoom)),
                proposalService);
            _panelCommandService.SubmitSelection(
                BuildOpenPanel(meetingId),
                BuildCommand(
                    secondProposalId,
                    meetingId,
                    MeetingActionType.RoomInspection,
                    MeetingActionTargetData.ForRoom(OfficeRoomType.SecurityRoom)),
                proposalService);
            return proposalService;
        }

        private static MeetingActionProposalData BuildResolvedProposal(
            string meetingId,
            string proposalId,
            MeetingActionType actionType,
            MeetingActionTargetData target)
        {
            MeetingActionRequestData request = new MeetingActionRequestData(
                proposalId,
                "player_proposer",
                actionType,
                target,
                "Phase six integration resolved proposal.");

            return new MeetingActionProposalData(
                proposalId,
                meetingId,
                request,
                MeetingActionProposalStatus.Resolved);
        }

        private void SubmitVote(
            string voteId,
            string voterId,
            string proposalId,
            MeetingActionProposalService proposalService)
        {
            _voteService.SubmitVote(
                new MeetingActionVoteData(
                    voteId,
                    voterId,
                    proposalId,
                    "Phase six integration vote."),
                proposalService.Proposals);
        }

        private void LogResult(
            string scenario,
            bool passed,
            string message)
        {
            lastScenario = scenario;
            lastPassed = passed;
            lastMessage = message;

            if (passed)
                Debug.Log($"[MeetingActionPhaseSixIntegrationValidator] PASS {scenario}: {message}");
            else
                Debug.LogError($"[MeetingActionPhaseSixIntegrationValidator] FAIL {scenario}: {message}");
        }
    }
}
#pragma warning restore 0414
