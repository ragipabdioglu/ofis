namespace OFIS.Meetings
{
    public sealed class MeetingRuntimeDecisionAggregatorService
    {
        private readonly MeetingRuntimeDecisionAggregatorConfig _config;
        private readonly MeetingPhaseRuntimeHookService _runtimeHookService;
        private readonly MeetingCompanyHealthPenaltyBridgeService _healthPenaltyBridgeService;
        private readonly MeetingEmptyStateRuntimeTracker _emptyStateRuntimeTracker;

        private bool _hasAppliedMissingPenalty;

        public MeetingRuntimeDecisionAggregatorService()
            : this(
                MeetingRuntimeDecisionAggregatorConfig.Default,
                new MeetingPhaseRuntimeHookService(),
                new MeetingCompanyHealthPenaltyBridgeService(),
                new MeetingEmptyStateRuntimeTracker())
        {
        }

        public MeetingRuntimeDecisionAggregatorService(
            MeetingRuntimeDecisionAggregatorConfig config,
            MeetingPhaseRuntimeHookService runtimeHookService,
            MeetingCompanyHealthPenaltyBridgeService healthPenaltyBridgeService,
            MeetingEmptyStateRuntimeTracker emptyStateRuntimeTracker)
        {
            _config = config;
            _runtimeHookService = runtimeHookService ?? new MeetingPhaseRuntimeHookService();
            _healthPenaltyBridgeService = healthPenaltyBridgeService ?? new MeetingCompanyHealthPenaltyBridgeService();
            _emptyStateRuntimeTracker = emptyStateRuntimeTracker ?? new MeetingEmptyStateRuntimeTracker();
            _hasAppliedMissingPenalty = false;
        }

        public MeetingRuntimeDecisionAggregatorConfig Config => _config;

        public void Reset()
        {
            _runtimeHookService.Reset();
            _emptyStateRuntimeTracker.Reset();
            _hasAppliedMissingPenalty = false;
        }

        public MeetingRuntimeDecisionResult Resolve(MeetingRuntimeDecisionInput input)
        {
            MeetingRuntimePhaseType phaseType = input.PhaseType;

            if (!IsMeetingPhase(phaseType))
            {
                return BuildSimpleResult(
                    MeetingRuntimeDecisionType.None,
                    phaseType,
                    "Current phase is not a meeting phase.");
            }

            MeetingCompanyHealthPenaltyBridgeResult penaltyResult = default;
            bool hasPenaltyResult = false;

            if (_config.EvaluateMissingPlayerPenalty && !_hasAppliedMissingPenalty)
            {
                penaltyResult = _healthPenaltyBridgeService.BuildBridgeResult(
                    input.AttendanceResult,
                    input.CurrentCompanyHealth);

                hasPenaltyResult = true;

                if (penaltyResult.ChangedHealth)
                {
                    _hasAppliedMissingPenalty = true;

                    return new MeetingRuntimeDecisionResult(
                        MeetingRuntimeDecisionType.ApplyMissingPlayerPenalty,
                        phaseType,
                        default,
                        penaltyResult,
                        default,
                        hasRuntimeHookResult: false,
                        hasHealthPenaltyBridgeResult: true,
                        hasEmptyStateResolutionResult: false,
                        shouldContinueMeeting: true,
                        shouldApplyHealthPenalty: true,
                        shouldCloseMeeting: false,
                        shouldResolveWinBranch: false,
                        shouldRunMeetingEndPipeline: false,
                        isTerminalDecision: false,
                        reason: "Missing eligible player penalty should be applied before other meeting decisions.");
                }
            }

            MeetingEmptyStateResolutionResult emptyStateResult = default;
            bool hasEmptyStateResult = false;

            if (_config.EvaluateEmptyStateResolution)
            {
                emptyStateResult = _emptyStateRuntimeTracker.Tick(
                    phaseType,
                    input.AttendanceResult,
                    input.DeltaTimeSeconds);

                hasEmptyStateResult = true;

                if (emptyStateResult.ShouldResolveWinBranch)
                {
                    return new MeetingRuntimeDecisionResult(
                        MeetingRuntimeDecisionType.ResolveFinalMeetingWinBranch,
                        phaseType,
                        default,
                        penaltyResult,
                        emptyStateResult,
                        hasRuntimeHookResult: false,
                        hasHealthPenaltyBridgeResult: hasPenaltyResult,
                        hasEmptyStateResolutionResult: true,
                        shouldContinueMeeting: false,
                        shouldApplyHealthPenalty: false,
                        shouldCloseMeeting: false,
                        shouldResolveWinBranch: true,
                        shouldRunMeetingEndPipeline: false,
                        isTerminalDecision: true,
                        reason: "Empty final meeting should resolve win branch.");
                }

                if (emptyStateResult.ShouldCloseMeeting)
                {
                    return new MeetingRuntimeDecisionResult(
                        MeetingRuntimeDecisionType.AutoCloseMeeting,
                        phaseType,
                        default,
                        penaltyResult,
                        emptyStateResult,
                        hasRuntimeHookResult: false,
                        hasHealthPenaltyBridgeResult: hasPenaltyResult,
                        hasEmptyStateResolutionResult: true,
                        shouldContinueMeeting: false,
                        shouldApplyHealthPenalty: false,
                        shouldCloseMeeting: true,
                        shouldResolveWinBranch: false,
                        shouldRunMeetingEndPipeline: false,
                        isTerminalDecision: true,
                        reason: "Empty normal meeting should auto-close.");
                }
            }

            MeetingPhaseRuntimeHookResult runtimeHookResult = default;
            bool hasRuntimeHookResult = false;

            if (_config.EvaluateMeetingEndPipeline)
            {
                runtimeHookResult = _runtimeHookService.Tick(
                    phaseType,
                    input.PhaseDurationSeconds,
                    input.PhaseElapsedSeconds,
                    input.Reports,
                    input.Votes,
                    input.CulpritPlayerIds,
                    _config.JoinLockThresholdSeconds);

                hasRuntimeHookResult = true;

                if (runtimeHookResult.HasPipelineResult)
                {
                    return new MeetingRuntimeDecisionResult(
                        MeetingRuntimeDecisionType.RunMeetingEndPipeline,
                        phaseType,
                        runtimeHookResult,
                        penaltyResult,
                        emptyStateResult,
                        hasRuntimeHookResult: true,
                        hasHealthPenaltyBridgeResult: hasPenaltyResult,
                        hasEmptyStateResolutionResult: hasEmptyStateResult,
                        shouldContinueMeeting: false,
                        shouldApplyHealthPenalty: false,
                        shouldCloseMeeting: false,
                        shouldResolveWinBranch: false,
                        shouldRunMeetingEndPipeline: true,
                        isTerminalDecision: true,
                        reason: "Meeting phase ended and end pipeline should run.");
                }
            }

            return new MeetingRuntimeDecisionResult(
                MeetingRuntimeDecisionType.ContinueMeeting,
                phaseType,
                runtimeHookResult,
                penaltyResult,
                emptyStateResult,
                hasRuntimeHookResult,
                hasPenaltyResult,
                hasEmptyStateResult,
                shouldContinueMeeting: true,
                shouldApplyHealthPenalty: false,
                shouldCloseMeeting: false,
                shouldResolveWinBranch: false,
                shouldRunMeetingEndPipeline: false,
                isTerminalDecision: false,
                reason: "No terminal meeting decision yet. Continue meeting.");
        }

        private static bool IsMeetingPhase(MeetingRuntimePhaseType phaseType)
        {
            return phaseType == MeetingRuntimePhaseType.Meeting
                || phaseType == MeetingRuntimePhaseType.FinalMeeting;
        }

        private static MeetingRuntimeDecisionResult BuildSimpleResult(
            MeetingRuntimeDecisionType decisionType,
            MeetingRuntimePhaseType phaseType,
            string reason)
        {
            return new MeetingRuntimeDecisionResult(
                decisionType,
                phaseType,
                default,
                default,
                default,
                hasRuntimeHookResult: false,
                hasHealthPenaltyBridgeResult: false,
                hasEmptyStateResolutionResult: false,
                shouldContinueMeeting: false,
                shouldApplyHealthPenalty: false,
                shouldCloseMeeting: false,
                shouldResolveWinBranch: false,
                shouldRunMeetingEndPipeline: false,
                isTerminalDecision: false,
                reason: reason);
        }
    }
}
