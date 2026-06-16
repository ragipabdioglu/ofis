using System.Collections.Generic;
using System.Linq;
using OFIS.Core.Ids;
using OFIS.Roles;

namespace OFIS.Playable
{
    public sealed class PlayableSessionService
    {
        private readonly RoleAssignmentService roleAssignmentService = new RoleAssignmentService(20260615);
        private readonly List<PlayableParticipant> participants = new List<PlayableParticipant>();
        private readonly List<string> transientMeetingReports = new List<string>();
        private readonly Queue<string> botDecisionQueue = new Queue<string>();

        private PlayableSessionState state = PlayableSessionState.Boot;
        private int companyHealth = 100;
        private int meetingCount;
        private int completedTasks;
        private bool corpseSpawned;
        private bool corpseCarried;
        private bool corpseInspected;
        private bool corpseTraceVisible;
        private bool sabotageActive;
        private bool meetingVoteResolved;
        private bool meetingTransientClean = true;
        private bool resultReady;
        private string resultWinnerKey = "none";
        private string status = "Booting playable session.";

        public IReadOnlyList<PlayableParticipant> Participants => participants;
        public PlayableSessionState State => state;
        public PlayableParticipant ActivePlayer => participants.Count > 0 ? participants[0] : null;
        public string ResultWinnerKey => resultWinnerKey;

        public PlayableSessionSnapshot Snapshot => new PlayableSessionSnapshot(
            state,
            participants,
            ActivePlayer != null ? ActivePlayer.DisplayName : "None",
            ActivePlayer != null ? ActivePlayer.Role : PlayerRole.None,
            companyHealth,
            meetingCount,
            completedTasks,
            corpseSpawned,
            corpseInspected,
            sabotageActive,
            meetingTransientClean,
            resultReady,
            status);

        public void BootToMainMenu()
        {
            ResetRuntimeState();
            CreateParticipants();
            state = PlayableSessionState.MainMenu;
            status = "Main menu ready. Start a local 8-player match.";
        }

        public PlayableActionResult EnterLobby()
        {
            if (state != PlayableSessionState.MainMenu)
                return Fail("Lobby can only be opened from MainMenu.");

            state = PlayableSessionState.Lobby;
            status = "Lobby ready. Local player can ready up; bots auto-ready.";
            return Pass("Lobby opened.");
        }

        public PlayableActionResult ReadyLocalPlayerAndAutoReadyBots()
        {
            if (state != PlayableSessionState.Lobby)
                return Fail("Ready flow requires Lobby state.");

            for (var i = 0; i < participants.Count; i++)
                participants[i].IsReady = true;

            status = "All local bot players auto-readied deterministically.";
            return Pass("All players ready.");
        }

        public PlayableActionResult AssignRolesAndReveal()
        {
            if (state != PlayableSessionState.Lobby || !participants.All(x => x.IsReady))
                return Fail("Role reveal requires all 8 players ready.");

            var roleResult = roleAssignmentService.AssignRoles(participants.Select(x => x.PlayerId).ToList());
            if (!roleResult.Success)
                return Fail(roleResult.ErrorMessage);

            foreach (var participant in participants)
            {
                var assignment = roleResult.Assignments.First(x => x.PlayerId == participant.PlayerId);
                participant.Role = assignment.Role;
                participant.KnownVictimTargets.Clear();
                participant.KnownVictimTargets.AddRange(assignment.KnownVictimTargets);
            }

            state = PlayableSessionState.RoleReveal;
            status = "Role reveal is owner-only. Active player sees only own role.";
            return Pass("Roles assigned using RoleAssignmentService.");
        }

        public PlayableActionResult AcknowledgeRoleReveal()
        {
            if (state != PlayableSessionState.RoleReveal)
                return Fail("Role reveal must be active.");

            state = PlayableSessionState.Office;
            status = "Office phase started. Task, kill/corpse, sabotage and meeting actions are available.";
            return Pass("Office phase entered.");
        }

        public PlayableActionResult CompleteTask()
        {
            if (state != PlayableSessionState.Office)
                return Fail("Tasks can only be completed in Office state.");

            if (ActivePlayer.CompletedTasks >= 4)
                return Fail("Active player's task list is already complete.");

            ActivePlayer.CompletedTasks++;
            completedTasks++;
            companyHealth = ClampCompany(companyHealth + (ActivePlayer.Role == PlayerRole.Killer ? -5 : 4));
            status = $"Task completed by {ActivePlayer.DisplayName}. Company health={companyHealth}.";
            return Pass("Task/company loop advanced.");
        }

        public PlayableActionResult KillOrScriptVictim()
        {
            if (state != PlayableSessionState.Office)
                return Fail("Kill flow requires Office state.");

            if (corpseSpawned)
                return Fail("A corpse already exists for this playable loop.");

            var victim = participants.FirstOrDefault(x => x.Role == PlayerRole.Victim && x.IsAlive);
            if (victim == null)
                return Fail("No alive victim is available.");

            victim.IsAlive = false;
            corpseSpawned = true;
            corpseCarried = false;
            corpseInspected = false;
            corpseTraceVisible = false;
            status = ActivePlayer.Role == PlayerRole.Killer
                ? $"Active killer eliminated {victim.DisplayName}. Corpse spawned."
                : $"Scripted bot kill eliminated {victim.DisplayName}. Corpse spawned for non-killer active player.";
            return Pass("Kill/corpse loop advanced.");
        }

        public PlayableActionResult CarryCorpse()
        {
            if (state != PlayableSessionState.Office || !corpseSpawned || corpseCarried)
                return Fail("Corpse carry requires an uncarried corpse in Office state.");

            if (sabotageActive)
                return Fail("Cannot start corpse carry while repair flow is active.");

            corpseCarried = true;
            ActivePlayer.IsCarryingCorpse = true;
            status = "Corpse carried. Sabotage is blocked while carrying.";
            return Pass("Corpse carry active.");
        }

        public PlayableActionResult DropCorpse()
        {
            if (!corpseCarried)
                return Fail("No corpse is currently carried.");

            corpseCarried = false;
            ActivePlayer.IsCarryingCorpse = false;
            status = "Corpse dropped. Inspect can reveal trace visibility.";
            return Pass("Corpse dropped.");
        }

        public PlayableActionResult InspectCorpse()
        {
            if (state != PlayableSessionState.Office || !corpseSpawned)
                return Fail("Corpse inspect requires a spawned corpse.");

            corpseInspected = true;
            corpseTraceVisible = true;
            status = "Corpse inspected. Trace is visible only after inspect.";
            return Pass("Corpse inspect opened trace visibility.");
        }

        public PlayableActionResult StartSabotage()
        {
            if (state != PlayableSessionState.Office)
                return Fail("Sabotage requires Office state.");

            if (ActivePlayer.IsCarryingCorpse)
                return Fail("Corpse carry blocks sabotage.");

            sabotageActive = true;
            companyHealth = ClampCompany(companyHealth - 2);
            status = "Sabotage started. Public UI does not reveal saboteur identity.";
            return Pass("Sabotage loop advanced.");
        }

        public PlayableActionResult RepairSabotage()
        {
            if (!sabotageActive)
                return Fail("No active sabotage to repair.");

            sabotageActive = false;
            companyHealth = ClampCompany(companyHealth + 3);
            status = "Sabotage repaired. Company health recovered.";
            return Pass("Repair loop advanced.");
        }

        public PlayableActionResult StartMeeting()
        {
            if (state != PlayableSessionState.Office)
                return Fail("Meeting requires Office state.");

            state = PlayableSessionState.Meeting;
            meetingCount++;
            meetingVoteResolved = false;
            meetingTransientClean = false;
            transientMeetingReports.Clear();
            transientMeetingReports.Add(corpseInspected ? "corpse.report.public_safe" : "meeting.report.no_corpse_announcement");
            botDecisionQueue.Clear();
            botDecisionQueue.Enqueue("vote.report_review");
            botDecisionQueue.Enqueue("vote.no_action");
            status = "Meeting started. Bots joined deterministically.";
            return Pass("Meeting opened.");
        }

        public PlayableActionResult ResolveMeetingVote()
        {
            if (state != PlayableSessionState.Meeting)
                return Fail("Vote resolve requires Meeting state.");

            meetingVoteResolved = true;
            status = "Meeting vote resolved without role or killer leak.";
            return Pass("Meeting vote resolved.");
        }

        public PlayableActionResult ReturnToOffice()
        {
            if (state != PlayableSessionState.Meeting || !meetingVoteResolved)
                return Fail("Office return requires resolved normal meeting.");

            state = PlayableSessionState.OfficeReturn;
            ClearMeetingTransientState();
            state = PlayableSessionState.Office;
            status = "OfficeReturn cleaned meeting transient data and resumed Office state.";
            return Pass("Office flow resumed after meeting.");
        }

        public PlayableActionResult EnterFinalAccusation()
        {
            if (state != PlayableSessionState.Office && state != PlayableSessionState.Meeting)
                return Fail("Final accusation requires Office or final Meeting state.");

            state = PlayableSessionState.FinalAccusation;
            status = "Final accusation panel opened.";
            return Pass("Final accusation opened.");
        }

        public PlayableActionResult SubmitFinalAccusation(bool correctList)
        {
            if (state != PlayableSessionState.FinalAccusation)
                return Fail("Final accusation must be active.");

            resultReady = true;
            resultWinnerKey = correctList ? "result.good_side_win" : "result.killer_side_win";
            state = PlayableSessionState.MatchResult;
            status = correctList ? "Correct final list. Good side wins." : "Wrong final list. Killer side wins.";
            return Pass("Final accusation resolved.");
        }

        public PlayableActionResult CleanupToMainMenu()
        {
            state = PlayableSessionState.Cleanup;
            ClearMeetingTransientState();
            corpseCarried = false;
            if (ActivePlayer != null)
                ActivePlayer.IsCarryingCorpse = false;
            sabotageActive = false;
            status = "Session cleanup completed. Returning to MainMenu.";
            BootToMainMenu();
            return Pass("Cleanup reset session and returned to MainMenu.");
        }

        public bool IsPublicTextSafe(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return true;

            var lower = value.ToLowerInvariant();
            return !lower.Contains("killer:")
                && !lower.Contains("victim:")
                && !lower.Contains("detective:")
                && !lower.Contains("target:")
                && !lower.Contains("saboteur");
        }

        public bool CanStartMatch()
        {
            return state == PlayableSessionState.Lobby && participants.Count == 8 && participants.All(x => x.IsReady);
        }

        public bool HasCorpseTraceEarlyLeak()
        {
            return corpseSpawned && !corpseInspected && corpseTraceVisible;
        }

        public bool MeetingTransientIsClean()
        {
            return meetingTransientClean && transientMeetingReports.Count == 0 && botDecisionQueue.Count == 0;
        }

        private void CreateParticipants()
        {
            participants.Clear();
            for (var i = 0; i < 8; i++)
            {
                var index = i + 1;
                participants.Add(new PlayableParticipant(
                    new PlayerId($"local_player_{index:00}"),
                    $"Player {index}",
                    i == 0,
                    i != 0));
            }
        }

        private void ResetRuntimeState()
        {
            state = PlayableSessionState.Boot;
            companyHealth = 100;
            meetingCount = 0;
            completedTasks = 0;
            corpseSpawned = false;
            corpseCarried = false;
            corpseInspected = false;
            corpseTraceVisible = false;
            sabotageActive = false;
            meetingVoteResolved = false;
            resultReady = false;
            resultWinnerKey = "none";
            ClearMeetingTransientState();
        }

        private void ClearMeetingTransientState()
        {
            transientMeetingReports.Clear();
            botDecisionQueue.Clear();
            meetingTransientClean = true;
        }

        private static int ClampCompany(int value)
        {
            if (value < 0)
                return 0;
            return value > 100 ? 100 : value;
        }

        private static PlayableActionResult Pass(string message)
        {
            return new PlayableActionResult(true, message);
        }

        private static PlayableActionResult Fail(string message)
        {
            return new PlayableActionResult(false, message);
        }
    }
}
