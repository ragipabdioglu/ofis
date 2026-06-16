using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Meetings
{
    public sealed class MeetingActionReportSafetyDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        [Header("Read Only Debug Output")]
        [SerializeField] private bool lastIsSafe;
        [SerializeField] private bool lastRevealsRole;
        [SerializeField] private bool lastRevealsDefiniteKiller;
        [SerializeField] private string lastMessage;

        private readonly MeetingActionReportSafetyGuardService _service =
            new MeetingActionReportSafetyGuardService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateReportSafetyGuard();
        }

        [ContextMenu("Validate Meeting Action Report Safety Guard")]
        public void ValidateReportSafetyGuard()
        {
            ValidateSafePanelPasses();
            ValidateRoleWordingFails();
            ValidateKillerWordingFails();
            ValidateUnsafeFlagsFail();
            ValidateDefiniteKillerFlagFails();
        }

        private void ValidateSafePanelPasses()
        {
            MeetingActionReportSafetyResult result = _service.Evaluate(
                BuildState(
                    "Action selected: room inspection.",
                    "Target room: ArchiveRoom.",
                    "Resolved by majority with 2 vote(s).",
                    "Official action effect ready to apply.",
                    true,
                    false));

            LogResult("SafePanelPasses", result.IsSafe, result);
        }

        private void ValidateRoleWordingFails()
        {
            MeetingActionReportSafetyResult result = _service.Evaluate(
                BuildState(
                    "Action selected: personnel audit.",
                    "Target player: player_01.",
                    "Player role revealed.",
                    "No effect applied.",
                    true,
                    false));

            bool passed = !result.IsSafe && result.RevealsRole;
            LogResult("RoleWordingFails", passed, result);
        }

        private void ValidateKillerWordingFails()
        {
            MeetingActionReportSafetyResult result = _service.Evaluate(
                BuildState(
                    "Action selected: official accusation.",
                    "Target player: player_02.",
                    "Definite killer found.",
                    "No effect applied.",
                    true,
                    false));

            bool passed = !result.IsSafe && result.RevealsDefiniteKiller;
            LogResult("KillerWordingFails", passed, result);
        }

        private void ValidateUnsafeFlagsFail()
        {
            MeetingActionReportSafetyResult result = _service.Evaluate(
                BuildState(
                    "Action selected: no action.",
                    "No target.",
                    "No official action resolved.",
                    "No effect applied.",
                    false,
                    false));

            bool passed = !result.IsSafe;
            LogResult("UnsafeFlagsFail", passed, result);
        }

        private void ValidateDefiniteKillerFlagFails()
        {
            MeetingActionReportSafetyResult result = _service.Evaluate(
                BuildState(
                    "Action selected: security record review.",
                    "Target security area: CameraSystem.",
                    "Resolved by timeout highest vote with 2 vote(s).",
                    "No effect applied.",
                    true,
                    true));

            bool passed = !result.IsSafe;
            LogResult("DefiniteKillerFlagFails", passed, result);
        }

        private static MeetingActionReportPanelState BuildState(
            string actionSummary,
            string targetSummary,
            string resolutionSummary,
            string effectSummary,
            bool isRoleSafe,
            bool revealsDefiniteKiller)
        {
            return new MeetingActionReportPanelState(
                "Meeting Action Report",
                actionSummary,
                targetSummary,
                resolutionSummary,
                effectSummary,
                true,
                false,
                isRoleSafe,
                revealsDefiniteKiller);
        }

        private void LogResult(
            string testName,
            bool passed,
            MeetingActionReportSafetyResult result)
        {
            lastIsSafe = result.IsSafe;
            lastRevealsRole = result.RevealsRole;
            lastRevealsDefiniteKiller = result.RevealsDefiniteKiller;
            lastMessage = result.Message;

            if (passed)
                Debug.Log($"[MeetingActionReportSafetyValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[MeetingActionReportSafetyValidator] FAIL {testName}: {result}");
        }
    }
}
#pragma warning restore 0414
