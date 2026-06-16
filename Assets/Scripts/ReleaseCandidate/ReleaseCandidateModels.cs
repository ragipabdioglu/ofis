using System.Collections.Generic;

namespace OFIS.ReleaseCandidate
{
    public enum ReleaseGateStatus
    {
        NotChecked = 0,
        Passed = 1,
        Warning = 2,
        Failed = 3
    }

    public enum ReleaseBuildTarget
    {
        Windows = 0,
        LinuxServer = 1
    }

    public readonly struct ReleaseGateResult
    {
        public string GateKey { get; }
        public ReleaseGateStatus Status { get; }
        public string Message { get; }

        public ReleaseGateResult(string gateKey, ReleaseGateStatus status, string message)
        {
            GateKey = string.IsNullOrWhiteSpace(gateKey) ? "release.gate.unknown" : gateKey;
            Status = status;
            Message = string.IsNullOrWhiteSpace(message) ? "Gate resolved." : message;
        }

        public bool Passed => Status == ReleaseGateStatus.Passed || Status == ReleaseGateStatus.Warning;
    }

    public readonly struct DebugUiPolicy
    {
        public bool DebugPanelEditorOnly { get; }
        public bool RuntimeDebugHiddenInRelease { get; }
        public bool AllowsBuild { get; }

        public DebugUiPolicy(bool debugPanelEditorOnly, bool runtimeDebugHiddenInRelease, bool allowsBuild)
        {
            DebugPanelEditorOnly = debugPanelEditorOnly;
            RuntimeDebugHiddenInRelease = runtimeDebugHiddenInRelease;
            AllowsBuild = allowsBuild;
        }
    }

    public readonly struct BuildSettingsSnapshot
    {
        public ReleaseBuildTarget Target { get; }
        public string MainMenuScene { get; }
        public string LobbyScene { get; }
        public string OfficeScene { get; }
        public bool DevelopmentBuildDisabled { get; }
        public bool ScriptDebuggingDisabled { get; }

        public BuildSettingsSnapshot(
            ReleaseBuildTarget target,
            string mainMenuScene,
            string lobbyScene,
            string officeScene,
            bool developmentBuildDisabled,
            bool scriptDebuggingDisabled)
        {
            Target = target;
            MainMenuScene = string.IsNullOrWhiteSpace(mainMenuScene) ? "Assets/Scenes/MainMenu/MainMenu.unity" : mainMenuScene;
            LobbyScene = string.IsNullOrWhiteSpace(lobbyScene) ? "Assets/Scenes/Lobby/Lobby.unity" : lobbyScene;
            OfficeScene = string.IsNullOrWhiteSpace(officeScene) ? "Assets/Scenes/Office_MVP/Office_MVP.unity" : officeScene;
            DevelopmentBuildDisabled = developmentBuildDisabled;
            ScriptDebuggingDisabled = scriptDebuggingDisabled;
        }
    }

    public readonly struct MenuFlowSnapshot
    {
        public bool HasMainMenu { get; }
        public bool HasLobbyEntry { get; }
        public bool HasMatchStart { get; }
        public bool HasMatchEndReturn { get; }

        public MenuFlowSnapshot(bool hasMainMenu, bool hasLobbyEntry, bool hasMatchStart, bool hasMatchEndReturn)
        {
            HasMainMenu = hasMainMenu;
            HasLobbyEntry = hasLobbyEntry;
            HasMatchStart = hasMatchStart;
            HasMatchEndReturn = hasMatchEndReturn;
        }
    }

    public readonly struct ReleaseErrorHandlingPolicy
    {
        public bool HasUserSafeMessage { get; }
        public bool HasRetryAction { get; }
        public bool RedactsSensitiveData { get; }

        public ReleaseErrorHandlingPolicy(bool hasUserSafeMessage, bool hasRetryAction, bool redactsSensitiveData)
        {
            HasUserSafeMessage = hasUserSafeMessage;
            HasRetryAction = hasRetryAction;
            RedactsSensitiveData = redactsSensitiveData;
        }
    }

    public readonly struct PerformanceSnapshot
    {
        public float AverageFrameMs { get; }
        public int MaxGarbageBytesPerTick { get; }
        public int ActiveValidatorCount { get; }

        public PerformanceSnapshot(float averageFrameMs, int maxGarbageBytesPerTick, int activeValidatorCount)
        {
            AverageFrameMs = averageFrameMs < 0f ? 0f : averageFrameMs;
            MaxGarbageBytesPerTick = maxGarbageBytesPerTick < 0 ? 0 : maxGarbageBytesPerTick;
            ActiveValidatorCount = activeValidatorCount < 0 ? 0 : activeValidatorCount;
        }
    }

    public readonly struct NetworkTimeoutPolicy
    {
        public int TimeoutSeconds { get; }
        public bool ShowsReconnectState { get; }
        public bool PreservesPlayerSlot { get; }

        public NetworkTimeoutPolicy(int timeoutSeconds, bool showsReconnectState, bool preservesPlayerSlot)
        {
            TimeoutSeconds = timeoutSeconds < 1 ? 1 : timeoutSeconds;
            ShowsReconnectState = showsReconnectState;
            PreservesPlayerSlot = preservesPlayerSlot;
        }
    }

    public readonly struct CrashExportPolicy
    {
        public bool ExportsEditorLog { get; }
        public bool ExportsPlayerLog { get; }
        public bool RedactsPrivatePayloads { get; }
        public string ExportFolderName { get; }

        public CrashExportPolicy(bool exportsEditorLog, bool exportsPlayerLog, bool redactsPrivatePayloads, string exportFolderName)
        {
            ExportsEditorLog = exportsEditorLog;
            ExportsPlayerLog = exportsPlayerLog;
            RedactsPrivatePayloads = redactsPrivatePayloads;
            ExportFolderName = string.IsNullOrWhiteSpace(exportFolderName) ? "OFIS_MVP_Logs" : exportFolderName;
        }
    }

    public sealed class ReleaseDocument
    {
        private readonly List<string> _sections = new List<string>();

        public string DocumentKey { get; }
        public IReadOnlyList<string> Sections => _sections;

        public ReleaseDocument(string documentKey, IEnumerable<string> sections)
        {
            DocumentKey = string.IsNullOrWhiteSpace(documentKey) ? "release.document.unknown" : documentKey;

            if (sections != null)
                _sections.AddRange(sections);
        }
    }

    public sealed class KnownIssueList
    {
        private readonly List<string> _issues = new List<string>();

        public IReadOnlyList<string> Issues => _issues;
        public bool HasCriticalIssue { get; }

        public KnownIssueList(IEnumerable<string> issues, bool hasCriticalIssue)
        {
            if (issues != null)
                _issues.AddRange(issues);

            HasCriticalIssue = hasCriticalIssue;
        }
    }

    public sealed class ExternalTestPlan
    {
        private readonly List<string> _steps = new List<string>();

        public int TesterCount { get; }
        public IReadOnlyList<string> Steps => _steps;
        public bool IncludesFeedbackForm { get; }

        public ExternalTestPlan(int testerCount, IEnumerable<string> steps, bool includesFeedbackForm)
        {
            TesterCount = testerCount < 0 ? 0 : testerCount;

            if (steps != null)
                _steps.AddRange(steps);

            IncludesFeedbackForm = includesFeedbackForm;
        }
    }
}
