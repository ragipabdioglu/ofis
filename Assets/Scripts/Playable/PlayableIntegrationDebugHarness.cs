using UnityEngine;

namespace OFIS.Playable
{
    public sealed class PlayableIntegrationDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = false;

        private void Start()
        {
            if (validateOnStart)
                ValidatePlayableIntegration();
        }

        [ContextMenu("Validate Playable Integration")]
        public void ValidatePlayableIntegration()
        {
            var service = new PlayableSessionService();
            var issue = RunHappyPath(service)
                ?? RunWrongFinalPath(new PlayableSessionService())
                ?? RunCleanupSecondMatchPath(new PlayableSessionService())
                ?? RunLeakAndSoftlockChecks(new PlayableSessionService());

            if (string.IsNullOrEmpty(issue))
                Debug.Log("[PlayableIntegrationDebugHarness] PASS PlayableIntegration: Local 8 bot full-match flow, office return, cleanup restart, role leak guard, and softlock checks passed.");
            else
                Debug.LogError($"[PlayableIntegrationDebugHarness] FAIL PlayableIntegration: {issue}");
        }

        private static string RunHappyPath(PlayableSessionService service)
        {
            return Expect(service.BootToMainMenu, PlayableSessionState.MainMenu, service)
                ?? Expect(service.EnterLobby, PlayableSessionState.Lobby, service)
                ?? Expect(service.ReadyLocalPlayerAndAutoReadyBots, PlayableSessionState.Lobby, service)
                ?? Expect(service.AssignRolesAndReveal, PlayableSessionState.RoleReveal, service)
                ?? Expect(service.AcknowledgeRoleReveal, PlayableSessionState.Office, service)
                ?? Expect(service.CompleteTask, PlayableSessionState.Office, service)
                ?? Expect(service.KillOrScriptVictim, PlayableSessionState.Office, service)
                ?? Expect(service.CarryCorpse, PlayableSessionState.Office, service)
                ?? Expect(service.DropCorpse, PlayableSessionState.Office, service)
                ?? Expect(service.InspectCorpse, PlayableSessionState.Office, service)
                ?? Expect(service.StartSabotage, PlayableSessionState.Office, service)
                ?? Expect(service.RepairSabotage, PlayableSessionState.Office, service)
                ?? Expect(service.StartMeeting, PlayableSessionState.Meeting, service)
                ?? Expect(service.ResolveMeetingVote, PlayableSessionState.Meeting, service)
                ?? Expect(service.ReturnToOffice, PlayableSessionState.Office, service)
                ?? Expect(service.EnterFinalAccusation, PlayableSessionState.FinalAccusation, service)
                ?? Expect(() => service.SubmitFinalAccusation(true), PlayableSessionState.MatchResult, service)
                ?? (service.ResultWinnerKey == "result.good_side_win" ? null : "Correct final path did not produce good-side win.");
        }

        private static string RunWrongFinalPath(PlayableSessionService service)
        {
            return Expect(service.BootToMainMenu, PlayableSessionState.MainMenu, service)
                ?? Expect(service.EnterLobby, PlayableSessionState.Lobby, service)
                ?? Expect(service.ReadyLocalPlayerAndAutoReadyBots, PlayableSessionState.Lobby, service)
                ?? Expect(service.AssignRolesAndReveal, PlayableSessionState.RoleReveal, service)
                ?? Expect(service.AcknowledgeRoleReveal, PlayableSessionState.Office, service)
                ?? Expect(service.EnterFinalAccusation, PlayableSessionState.FinalAccusation, service)
                ?? Expect(() => service.SubmitFinalAccusation(false), PlayableSessionState.MatchResult, service)
                ?? (service.ResultWinnerKey == "result.killer_side_win" ? null : "Wrong final path did not produce killer-side win.");
        }

        private static string RunCleanupSecondMatchPath(PlayableSessionService service)
        {
            var issue = RunHappyPath(service);
            if (!string.IsNullOrEmpty(issue))
                return issue;

            issue = Expect(service.CleanupToMainMenu, PlayableSessionState.MainMenu, service);
            if (!string.IsNullOrEmpty(issue))
                return issue;

            if (!service.MeetingTransientIsClean())
                return "Session cleanup left meeting transient state behind.";

            return Expect(service.EnterLobby, PlayableSessionState.Lobby, service)
                ?? Expect(service.ReadyLocalPlayerAndAutoReadyBots, PlayableSessionState.Lobby, service)
                ?? Expect(service.AssignRolesAndReveal, PlayableSessionState.RoleReveal, service);
        }

        private static string RunLeakAndSoftlockChecks(PlayableSessionService service)
        {
            var issue = Expect(service.BootToMainMenu, PlayableSessionState.MainMenu, service)
                ?? Expect(service.EnterLobby, PlayableSessionState.Lobby, service)
                ?? Expect(service.ReadyLocalPlayerAndAutoReadyBots, PlayableSessionState.Lobby, service)
                ?? Expect(service.AssignRolesAndReveal, PlayableSessionState.RoleReveal, service)
                ?? Expect(service.AcknowledgeRoleReveal, PlayableSessionState.Office, service)
                ?? Expect(service.KillOrScriptVictim, PlayableSessionState.Office, service);

            if (!string.IsNullOrEmpty(issue))
                return issue;

            if (service.HasCorpseTraceEarlyLeak())
                return "Corpse trace leaked before inspect.";

            if (!service.IsPublicTextSafe("meeting.report.public_safe"))
                return "Safe public report was rejected.";

            if (service.IsPublicTextSafe("target:killer"))
                return "Unsafe public role text was accepted.";

            issue = Expect(service.StartMeeting, PlayableSessionState.Meeting, service)
                ?? Expect(service.ResolveMeetingVote, PlayableSessionState.Meeting, service)
                ?? Expect(service.ReturnToOffice, PlayableSessionState.Office, service);

            if (!string.IsNullOrEmpty(issue))
                return issue;

            return service.MeetingTransientIsClean() ? null : "OfficeReturn did not clean meeting transient state.";
        }

        private static string Expect(System.Action action, PlayableSessionState expectedState, PlayableSessionService service)
        {
            action();
            return service.State == expectedState ? null : $"Expected {expectedState}, got {service.State}.";
        }

        private static string Expect(System.Func<PlayableActionResult> action, PlayableSessionState expectedState, PlayableSessionService service)
        {
            var result = action();
            if (!result.Passed)
                return result.Message;

            return service.State == expectedState ? null : $"Expected {expectedState}, got {service.State}.";
        }
    }
}
