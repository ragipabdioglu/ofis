using System.Collections.Generic;

namespace OFIS.Product.Onboarding
{
    public enum OnboardingAudience
    {
        AllPlayers,
        KillerOnly,
        VictimOnly,
        DetectiveOnly
    }

    public enum OnboardingRole
    {
        Killer,
        Victim,
        Detective,
        Meeting,
        FinalAccusation,
        Company
    }

    public readonly struct OnboardingGateResult
    {
        public OnboardingGateResult(bool passed, string message)
        {
            Passed = passed;
            Message = message;
        }

        public bool Passed { get; }
        public string Message { get; }
    }

    public readonly struct TutorialScreenDefinition
    {
        public TutorialScreenDefinition(OnboardingRole role, OnboardingAudience audience, string titleKey, string assetPath, bool leaksHiddenRoleData, IReadOnlyList<string> coveredConcepts)
        {
            Role = role;
            Audience = audience;
            TitleKey = titleKey;
            AssetPath = assetPath;
            LeaksHiddenRoleData = leaksHiddenRoleData;
            CoveredConcepts = coveredConcepts;
        }

        public OnboardingRole Role { get; }
        public OnboardingAudience Audience { get; }
        public string TitleKey { get; }
        public string AssetPath { get; }
        public bool LeaksHiddenRoleData { get; }
        public IReadOnlyList<string> CoveredConcepts { get; }
    }

    public readonly struct FirstMatchHintDefinition
    {
        public FirstMatchHintDefinition(string hintKey, string iconPath, bool roleSafe, bool dismissible, int priority)
        {
            HintKey = hintKey;
            IconPath = iconPath;
            RoleSafe = roleSafe;
            Dismissible = dismissible;
            Priority = priority;
        }

        public string HintKey { get; }
        public string IconPath { get; }
        public bool RoleSafe { get; }
        public bool Dismissible { get; }
        public int Priority { get; }
    }

    public readonly struct TooltipDefinition
    {
        public TooltipDefinition(string actionKey, string iconPath, string localizationKey, bool hasErrorCopy)
        {
            ActionKey = actionKey;
            IconPath = iconPath;
            LocalizationKey = localizationKey;
            HasErrorCopy = hasErrorCopy;
        }

        public string ActionKey { get; }
        public string IconPath { get; }
        public string LocalizationKey { get; }
        public bool HasErrorCopy { get; }
    }

    public readonly struct AccessibilityProfile
    {
        public AccessibilityProfile(string swatchPath, string highContrastPanelPath, string monoIconPath, bool colorBlindAlternative, bool uiScaleSupported, bool minimumIconMode)
        {
            SwatchPath = swatchPath;
            HighContrastPanelPath = highContrastPanelPath;
            MonoIconPath = monoIconPath;
            ColorBlindAlternative = colorBlindAlternative;
            UiScaleSupported = uiScaleSupported;
            MinimumIconMode = minimumIconMode;
        }

        public string SwatchPath { get; }
        public string HighContrastPanelPath { get; }
        public string MonoIconPath { get; }
        public bool ColorBlindAlternative { get; }
        public bool UiScaleSupported { get; }
        public bool MinimumIconMode { get; }
    }

    public readonly struct MainMenuPolishDefinition
    {
        public MainMenuPolishDefinition(string backgroundPath, string logoPath, string buttonFramePath, string loadingIconPath, bool keyboardNavigation)
        {
            BackgroundPath = backgroundPath;
            LogoPath = logoPath;
            ButtonFramePath = buttonFramePath;
            LoadingIconPath = loadingIconPath;
            KeyboardNavigation = keyboardNavigation;
        }

        public string BackgroundPath { get; }
        public string LogoPath { get; }
        public string ButtonFramePath { get; }
        public string LoadingIconPath { get; }
        public bool KeyboardNavigation { get; }
    }

    public readonly struct LobbyReadyUxDefinition
    {
        public LobbyReadyUxDefinition(string readyIconPath, string notReadyIconPath, string playerSlotFramePath, string hostIconPath, string pingIconPath, bool readyStateReadable)
        {
            ReadyIconPath = readyIconPath;
            NotReadyIconPath = notReadyIconPath;
            PlayerSlotFramePath = playerSlotFramePath;
            HostIconPath = hostIconPath;
            PingIconPath = pingIconPath;
            ReadyStateReadable = readyStateReadable;
        }

        public string ReadyIconPath { get; }
        public string NotReadyIconPath { get; }
        public string PlayerSlotFramePath { get; }
        public string HostIconPath { get; }
        public string PingIconPath { get; }
        public bool ReadyStateReadable { get; }
    }

    public readonly struct MatchResultDetailDefinition
    {
        public MatchResultDetailDefinition(string goodWinBannerPath, string killerWinBannerPath, string summaryPanelPath, string roleRevealCardPath, IReadOnlyList<string> companyStateIcons, bool explainsWinCondition)
        {
            GoodWinBannerPath = goodWinBannerPath;
            KillerWinBannerPath = killerWinBannerPath;
            SummaryPanelPath = summaryPanelPath;
            RoleRevealCardPath = roleRevealCardPath;
            CompanyStateIcons = companyStateIcons;
            ExplainsWinCondition = explainsWinCondition;
        }

        public string GoodWinBannerPath { get; }
        public string KillerWinBannerPath { get; }
        public string SummaryPanelPath { get; }
        public string RoleRevealCardPath { get; }
        public IReadOnlyList<string> CompanyStateIcons { get; }
        public bool ExplainsWinCondition { get; }
    }
}
