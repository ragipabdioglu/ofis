using System.Collections.Generic;
using System.Linq;

namespace OFIS.ReleaseCandidate
{
    public sealed class ReleaseCandidateGateService
    {
        public ReleaseGateResult ValidateDebugOnlyUiSeparation(DebugUiPolicy policy)
        {
            bool passed = policy.DebugPanelEditorOnly && policy.RuntimeDebugHiddenInRelease && policy.AllowsBuild;
            return Gate("release.debug_ui", passed, "Debug UI is separated from release runtime.");
        }

        public ReleaseGateResult ValidateBuildSettings(BuildSettingsSnapshot snapshot)
        {
            bool scenesReady = snapshot.MainMenuScene.Contains("MainMenu")
                && snapshot.LobbyScene.Contains("Lobby")
                && snapshot.OfficeScene.Contains("Office_MVP");
            bool passed = scenesReady && snapshot.DevelopmentBuildDisabled && snapshot.ScriptDebuggingDisabled;
            return Gate("release.build_settings", passed, $"Target={snapshot.Target}");
        }

        public ReleaseGateResult ValidateBasicMainMenu(MenuFlowSnapshot flow)
        {
            return Gate("release.main_menu", flow.HasMainMenu && flow.HasLobbyEntry, "Main menu can enter lobby.");
        }

        public ReleaseGateResult ValidateLobbyFlow(MenuFlowSnapshot flow)
        {
            return Gate("release.lobby_flow", flow.HasLobbyEntry && flow.HasMatchStart, "Lobby can start MVP match.");
        }

        public ReleaseGateResult ValidateMatchStartEndFlow(MenuFlowSnapshot flow)
        {
            return Gate("release.match_start_end", flow.HasMatchStart && flow.HasMatchEndReturn, "Match can start and return after result.");
        }

        public ReleaseGateResult ValidateErrorHandling(ReleaseErrorHandlingPolicy policy)
        {
            bool passed = policy.HasUserSafeMessage && policy.HasRetryAction && policy.RedactsSensitiveData;
            return Gate("release.error_handling", passed, "Errors are user-safe and retryable.");
        }

        public ReleaseGateResult ValidatePerformance(PerformanceSnapshot snapshot)
        {
            bool frameOk = snapshot.AverageFrameMs <= 33.4f;
            bool gcOk = snapshot.MaxGarbageBytesPerTick <= 4096;
            bool validatorsOk = snapshot.ActiveValidatorCount <= 32;
            return Gate("release.performance", frameOk && gcOk && validatorsOk, $"FrameMs={snapshot.AverageFrameMs:0.##}, GC={snapshot.MaxGarbageBytesPerTick}");
        }

        public ReleaseGateResult ValidateNetworkTimeout(NetworkTimeoutPolicy policy)
        {
            bool passed = policy.TimeoutSeconds >= 10 && policy.ShowsReconnectState && policy.PreservesPlayerSlot;
            return Gate("release.network_timeout", passed, $"Timeout={policy.TimeoutSeconds}s");
        }

        public ReleaseGateResult ValidateCrashLogExport(CrashExportPolicy policy)
        {
            bool passed = policy.ExportsEditorLog && policy.ExportsPlayerLog && policy.RedactsPrivatePayloads && policy.ExportFolderName.Contains("OFIS");
            return Gate("release.crash_log_export", passed, policy.ExportFolderName);
        }

        public ReleaseGateResult ValidateMvpBuildDocument(ReleaseDocument document)
        {
            bool passed = document.Sections.Contains("build_steps")
                && document.Sections.Contains("launch_flow")
                && document.Sections.Contains("validation_checklist")
                && document.Sections.Contains("rollback_notes");
            return Gate("release.mvp_build_document", passed, $"Sections={document.Sections.Count}");
        }

        public ReleaseGateResult ValidateKnownIssues(KnownIssueList issues)
        {
            bool passed = !issues.HasCriticalIssue && issues.Issues.Count >= 1;
            ReleaseGateStatus status = passed ? ReleaseGateStatus.Warning : ReleaseGateStatus.Failed;
            return new ReleaseGateResult("release.known_issues", status, $"Issues={issues.Issues.Count}, Critical={issues.HasCriticalIssue}");
        }

        public ReleaseGateResult ValidateExternalTestPlan(ExternalTestPlan plan)
        {
            bool passed = plan.TesterCount >= 4 && plan.Steps.Count >= 5 && plan.IncludesFeedbackForm;
            return Gate("release.external_test_plan", passed, $"Testers={plan.TesterCount}, Steps={plan.Steps.Count}");
        }

        public bool CanShipMvpReleaseCandidate(IReadOnlyList<ReleaseGateResult> gates)
        {
            return gates != null
                && gates.Count >= 12
                && gates.All(x => x.Passed)
                && gates.All(x => x.Status != ReleaseGateStatus.Failed);
        }

        private static ReleaseGateResult Gate(string key, bool passed, string message)
        {
            return new ReleaseGateResult(key, passed ? ReleaseGateStatus.Passed : ReleaseGateStatus.Failed, message);
        }
    }

    public sealed class ReleaseCandidateFixtureFactory
    {
        public DebugUiPolicy BuildDebugUiPolicy()
        {
            return new DebugUiPolicy(true, true, true);
        }

        public BuildSettingsSnapshot BuildSettings()
        {
            return new BuildSettingsSnapshot(
                ReleaseBuildTarget.Windows,
                "Assets/Scenes/MainMenu/MainMenu.unity",
                "Assets/Scenes/Lobby/Lobby.unity",
                "Assets/Scenes/Office_MVP/Office_MVP.unity",
                true,
                true);
        }

        public MenuFlowSnapshot BuildMenuFlow()
        {
            return new MenuFlowSnapshot(true, true, true, true);
        }

        public ReleaseErrorHandlingPolicy BuildErrorPolicy()
        {
            return new ReleaseErrorHandlingPolicy(true, true, true);
        }

        public PerformanceSnapshot BuildPerformanceSnapshot()
        {
            return new PerformanceSnapshot(16.7f, 1024, 18);
        }

        public NetworkTimeoutPolicy BuildNetworkTimeoutPolicy()
        {
            return new NetworkTimeoutPolicy(15, true, true);
        }

        public CrashExportPolicy BuildCrashExportPolicy()
        {
            return new CrashExportPolicy(true, true, true, "OFIS_MVP_Logs");
        }

        public ReleaseDocument BuildMvpDocument()
        {
            return new ReleaseDocument(
                "release.document.mvp_build",
                new[] { "build_steps", "launch_flow", "validation_checklist", "rollback_notes" });
        }

        public KnownIssueList BuildKnownIssues()
        {
            return new KnownIssueList(
                new[] { "known_issue.balance_needs_more_external_data", "known_issue.visual_polish_deferred_to_product" },
                false);
        }

        public ExternalTestPlan BuildExternalTestPlan()
        {
            return new ExternalTestPlan(
                8,
                new[]
                {
                    "install_build",
                    "create_lobby",
                    "play_full_match",
                    "submit_bug_report",
                    "submit_balance_feedback"
                },
                true);
        }
    }
}
