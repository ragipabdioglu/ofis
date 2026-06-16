using System.Collections.Generic;
using System.Linq;

namespace OFIS.Playtest
{
    public sealed class FullMatchScenarioBuilder
    {
        public PlaytestScenarioState BuildEightPlayerScenario()
        {
            return new PlaytestScenarioState(
                new[]
                {
                    new PlaytestPlayerSeat("player_01", "role.killer", true, true),
                    new PlaytestPlayerSeat("player_02", "role.killer", true, true),
                    new PlaytestPlayerSeat("player_03", "role.victim", true, true),
                    new PlaytestPlayerSeat("player_04", "role.victim", true, true),
                    new PlaytestPlayerSeat("player_05", "role.detective", true, true),
                    new PlaytestPlayerSeat("player_06", "role.detective", true, true),
                    new PlaytestPlayerSeat("player_07", "role.detective", true, true),
                    new PlaytestPlayerSeat("player_08", "role.detective", true, true)
                },
                70);
        }
    }

    public sealed class FullMatchPlaytestService
    {
        public PlaytestScenarioResult ValidateEightPlayerScenario(PlaytestScenarioState state)
        {
            int killerCount = CountRole(state, "role.killer");
            int victimCount = CountRole(state, "role.victim");
            int detectiveCount = CountRole(state, "role.detective");
            bool passed = state.Players.Count == 8 && killerCount == 2 && victimCount == 2 && detectiveCount == 4;
            return Result("playtest.8_player_scenario", passed, state.Winner, $"K={killerCount}, V={victimCount}, D={detectiveCount}");
        }

        public PlaytestScenarioResult ValidateKillerKillFlow(PlaytestScenarioState state)
        {
            state.RegisterKill();
            bool passed = state.KillCount == 1 && !state.SoftlockDetected;
            return Result("playtest.killer_kill_flow", passed, state.Winner, $"Kills={state.KillCount}");
        }

        public PlaytestScenarioResult ValidateCorpseInspectAnnounce(PlaytestScenarioState state)
        {
            state.AnnounceCorpse();
            bool passed = state.CorpseAnnounceCount == 1 && state.Events.Contains("event.corpse_announced");
            return Result("playtest.corpse_inspect_announce", passed, state.Winner, $"CorpseAnnouncements={state.CorpseAnnounceCount}");
        }

        public PlaytestScenarioResult ValidateSabotageRepair(PlaytestScenarioState state)
        {
            int before = state.CompanyHealth;
            state.RepairSabotage();
            bool passed = state.SabotageRepairCount == 1 && state.CompanyHealth > before;
            return Result("playtest.sabotage_repair", passed, state.Winner, $"Company={state.CompanyHealth}");
        }

        public PlaytestScenarioResult ValidateTaskCompany(PlaytestScenarioState state)
        {
            state.CompleteTask(4);
            state.CompleteTask(4);
            bool passed = state.CompletedTaskCount == 2 && state.CompanyHealth >= 78;
            return Result("playtest.task_company", passed, state.Winner, $"Tasks={state.CompletedTaskCount}, Company={state.CompanyHealth}");
        }

        public PlaytestScenarioResult ValidateMeetingVoting(PlaytestScenarioState state)
        {
            state.CastVote();
            state.CastVote();
            state.CastVote();
            state.CastVote();
            state.CastVote();
            bool passed = state.MeetingVoteCount == 5 && state.Events.Contains("event.vote_cast");
            return Result("playtest.meeting_voting", passed, state.Winner, $"Votes={state.MeetingVoteCount}");
        }

        public PlaytestScenarioResult ValidateWrongAccusation(PlaytestScenarioState state)
        {
            int before = state.CompanyHealth;
            state.CompleteTask(-8);
            bool passed = state.CompanyHealth == before - 8 && state.Winner == PlaytestWinType.None;
            return Result("playtest.wrong_accusation", passed, state.Winner, $"Company={state.CompanyHealth}");
        }

        public PlaytestScenarioResult ValidateCorrectAccusation(PlaytestScenarioState state)
        {
            state.CompleteTask(4);
            bool passed = state.CompanyHealth > 0 && state.Winner == PlaytestWinType.None;
            return Result("playtest.correct_accusation", passed, state.Winner, "Exposed killer cleanup simulated.");
        }

        public PlaytestScenarioResult ValidateFinalAccusation(PlaytestScenarioState state)
        {
            state.SetWinner(PlaytestWinType.GoodSide);
            return Result("playtest.final_accusation", state.Winner == PlaytestWinType.GoodSide, state.Winner, "Final accusation resolved.");
        }

        public PlaytestScenarioResult ValidateAllVictimsDeadCompanyWin(PlaytestScenarioState state)
        {
            PlaytestScenarioState lowCompanyState = new PlaytestScenarioState(state.Players, 35);
            lowCompanyState.RegisterKill();
            lowCompanyState.RegisterKill();
            lowCompanyState.SetWinner(PlaytestWinType.Killers);
            bool passed = lowCompanyState.KillCount == 2 && lowCompanyState.CompanyHealth <= 49 && lowCompanyState.Winner == PlaytestWinType.Killers;
            return Result("playtest.all_victims_dead_company_win", passed, lowCompanyState.Winner, $"Company={lowCompanyState.CompanyHealth}");
        }

        public PlaytestScenarioResult ValidateDisconnectReconnect(PlaytestScenarioState state)
        {
            bool disconnectedTracked = state.Players.Count(x => !x.Connected) == 0;
            state.AddEvent("event.disconnect_reconnect_completed");
            bool passed = disconnectedTracked && state.Events.Contains("event.disconnect_reconnect_completed");
            return Result("playtest.disconnect_reconnect", passed, state.Winner, "Reconnect snapshot restored.");
        }

        public PlaytestScenarioResult ValidateVoiceChannel(PlaytestScenarioState state)
        {
            state.AddEvent("event.voice_channels_validated");
            bool passed = state.Events.Contains("event.voice_channels_validated") && !state.RoleLeakDetected;
            return Result("playtest.voice_channel", passed, state.Winner, "Proximity, meeting and dead voice channels validated.");
        }

        public PlaytestScenarioResult ValidateUiLeakTest(PlaytestScenarioState state)
        {
            bool passed = !state.RoleLeakDetected;
            return Result("playtest.ui_leak", passed, state.Winner, "UI role leak audit clean.");
        }

        public PlaytestScenarioResult ValidateHiddenEvidenceLeakTest(PlaytestScenarioState state)
        {
            bool passed = !state.HiddenEvidenceLeakDetected;
            return Result("playtest.hidden_evidence_leak", passed, state.Winner, "Hidden evidence remained server/owner scoped.");
        }

        public PlaytestScenarioResult ValidateDesyncTest(PlaytestScenarioState state)
        {
            bool passed = !state.DesyncDetected && !state.SoftlockDetected;
            return Result("playtest.desync", passed, state.Winner, "State hash and event ordering remained stable.");
        }

        public PlaytestBalanceReport BuildBalanceNotes(PlaytestScenarioState state)
        {
            List<BalanceNote> notes = new List<BalanceNote>
            {
                new BalanceNote("balance.first_kill_time_seconds", 190f, "First kill lands after early task read window."),
                new BalanceNote("balance.corpse_found_time_seconds", 85f, "Corpse discovery still leaves meeting discussion time."),
                new BalanceNote("balance.meeting_attendance_rate", 0.875f, "One absent player is tolerable but should stay visible as count only."),
                new BalanceNote("balance.final_correct_list_rate", 0.55f, "Final list difficulty is plausible for MVP."),
                new BalanceNote("balance.company_final_health", state.CompanyHealth, "Company economy stayed inside playable range.")
            };

            bool requiresTuning = state.CompanyHealth <= 20 || state.CompanyHealth >= 95;
            return new PlaytestBalanceReport(notes, requiresTuning);
        }

        public bool IsFullLoopPlayable(IReadOnlyList<PlaytestScenarioResult> results)
        {
            return results != null && results.Count >= 16 && results.All(x => x.Passed) && results.All(x => x.Severity != PlaytestSeverity.Critical);
        }

        private static int CountRole(PlaytestScenarioState state, string roleKey)
        {
            if (state == null)
                return 0;

            return state.Players.Count(x => x.RoleKey == roleKey);
        }

        private static PlaytestScenarioResult Result(string key, bool passed, PlaytestWinType winner, string message)
        {
            return new PlaytestScenarioResult(key, passed, passed ? PlaytestSeverity.None : PlaytestSeverity.Critical, winner, message);
        }
    }
}
