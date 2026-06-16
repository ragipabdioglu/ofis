using System.Collections.Generic;
using OFIS.Roles;

namespace OFIS.UI
{
    public enum UiAudienceType
    {
        Public = 0,
        OwnerOnly = 1,
        KillerOnly = 2,
        DetectiveOnly = 3,
        VictimOnly = 4,
        DeadOnly = 5
    }

    public enum UiActionCommandType
    {
        None = 0,
        StartTask = 1,
        JoinMeeting = 2,
        CreateProposal = 3,
        CastVote = 4,
        OpenReport = 5,
        SubmitFinalAccusation = 6,
        InspectCorpse = 7,
        AnnounceCorpse = 8,
        CarryCorpse = 9,
        DropCorpse = 10,
        StartSabotage = 11,
        StartRepair = 12,
        PinEvidence = 13,
        SaveVictimNote = 14,
        CompleteDeadTask = 15,
        AcknowledgeResult = 16,
        KillTarget = 17
    }

    public readonly struct UiActionBinding
    {
        public UiActionCommandType CommandType { get; }
        public string LocalizationKey { get; }
        public bool Enabled { get; }
        public string ErrorKey { get; }

        public UiActionBinding(UiActionCommandType commandType, string localizationKey, bool enabled, string errorKey)
        {
            CommandType = commandType;
            LocalizationKey = string.IsNullOrWhiteSpace(localizationKey) ? "ui.action.unknown" : localizationKey;
            Enabled = enabled;
            ErrorKey = string.IsNullOrWhiteSpace(errorKey) ? string.Empty : errorKey;
        }
    }

    public sealed class UiPanelState
    {
        private readonly List<UiActionBinding> _actions = new List<UiActionBinding>();

        public string PanelId { get; }
        public UiAudienceType AudienceType { get; }
        public bool IsVisible { get; }
        public bool IsOwnerOnly { get; }
        public bool IsRoleSpecific { get; }
        public IReadOnlyList<UiActionBinding> Actions => _actions;
        public IReadOnlyList<string> TextKeys { get; }

        public UiPanelState(
            string panelId,
            UiAudienceType audienceType,
            bool isVisible,
            bool isOwnerOnly,
            IEnumerable<UiActionBinding> actions,
            IEnumerable<string> textKeys)
        {
            PanelId = string.IsNullOrWhiteSpace(panelId) ? "ui.panel.unknown" : panelId;
            AudienceType = audienceType;
            IsVisible = isVisible;
            IsOwnerOnly = isOwnerOnly;
            IsRoleSpecific = audienceType != UiAudienceType.Public && audienceType != UiAudienceType.OwnerOnly;
            TextKeys = new List<string>(textKeys ?? new string[0]);

            if (actions == null)
                return;

            foreach (UiActionBinding action in actions)
                _actions.Add(action);
        }
    }

    public readonly struct HudViewModel
    {
        public PlayerRole OwnRole { get; }
        public string DepartmentKey { get; }
        public string TimerText { get; }
        public int CompanyHealth { get; }
        public string VoiceStatusKey { get; }
        public int TotalTaskCount { get; }
        public int CompletedTaskCount { get; }
        public bool IsOwnerOnly { get; }

        public HudViewModel(
            PlayerRole ownRole,
            string departmentKey,
            string timerText,
            int companyHealth,
            string voiceStatusKey,
            int totalTaskCount,
            int completedTaskCount,
            bool isOwnerOnly)
        {
            OwnRole = ownRole;
            DepartmentKey = string.IsNullOrWhiteSpace(departmentKey) ? "ui.department.unknown" : departmentKey;
            TimerText = string.IsNullOrWhiteSpace(timerText) ? "00:00" : timerText;
            CompanyHealth = companyHealth < 0 ? 0 : companyHealth > 100 ? 100 : companyHealth;
            VoiceStatusKey = string.IsNullOrWhiteSpace(voiceStatusKey) ? "ui.voice.none" : voiceStatusKey;
            TotalTaskCount = totalTaskCount < 0 ? 0 : totalTaskCount;
            CompletedTaskCount = completedTaskCount < 0 ? 0 : completedTaskCount;
            IsOwnerOnly = isOwnerOnly;
        }
    }

    public readonly struct RoleRevealViewModel
    {
        public PlayerRole OwnRole { get; }
        public IReadOnlyList<string> TargetIds { get; }
        public bool IsOwnerOnly { get; }

        public RoleRevealViewModel(PlayerRole ownRole, IReadOnlyList<string> targetIds, bool isOwnerOnly)
        {
            OwnRole = ownRole;
            TargetIds = targetIds ?? new List<string>();
            IsOwnerOnly = isOwnerOnly;
        }
    }

    public readonly struct MatchResultViewModel
    {
        public string WinnerKey { get; }
        public IReadOnlyList<string> RevealRows { get; }
        public bool AllowsEndRoleReveal { get; }

        public MatchResultViewModel(string winnerKey, IReadOnlyList<string> revealRows, bool allowsEndRoleReveal)
        {
            WinnerKey = string.IsNullOrWhiteSpace(winnerKey) ? "ui.result.unknown" : winnerKey;
            RevealRows = revealRows ?? new List<string>();
            AllowsEndRoleReveal = allowsEndRoleReveal;
        }
    }
}
