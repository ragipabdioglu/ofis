using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Playtest
{
    public sealed class PlaytestPhaseSeventeenDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;
        [SerializeField] private PlaytestPhaseSeventeenPackageType packageType;
        [SerializeField] private string lastScenario;
        [SerializeField] private bool lastPassed;
        [SerializeField] private string lastMessage;

        private readonly FullMatchScenarioBuilder _scenarioBuilder = new FullMatchScenarioBuilder();
        private readonly FullMatchPlaytestService _playtestService = new FullMatchPlaytestService();

        private void Start()
        {
            if (validateOnStart)
                ValidatePackage();
        }

        [ContextMenu("Validate Playtest Phase 17 Package")]
        public void ValidatePackage()
        {
            switch (packageType)
            {
                case PlaytestPhaseSeventeenPackageType.EightPlayerScenario:
                    ValidateEightPlayerScenario();
                    break;
                case PlaytestPhaseSeventeenPackageType.KillerKillFlow:
                    ValidateKillerKillFlow();
                    break;
                case PlaytestPhaseSeventeenPackageType.CorpseInspectAnnounce:
                    ValidateCorpseInspectAnnounce();
                    break;
                case PlaytestPhaseSeventeenPackageType.SabotageRepair:
                    ValidateSabotageRepair();
                    break;
                case PlaytestPhaseSeventeenPackageType.TaskCompany:
                    ValidateTaskCompany();
                    break;
                case PlaytestPhaseSeventeenPackageType.MeetingVoting:
                    ValidateMeetingVoting();
                    break;
                case PlaytestPhaseSeventeenPackageType.WrongAccusation:
                    ValidateWrongAccusation();
                    break;
                case PlaytestPhaseSeventeenPackageType.CorrectAccusation:
                    ValidateCorrectAccusation();
                    break;
                case PlaytestPhaseSeventeenPackageType.FinalAccusation:
                    ValidateFinalAccusation();
                    break;
                case PlaytestPhaseSeventeenPackageType.AllVictimsDeadCompanyWin:
                    ValidateAllVictimsDeadCompanyWin();
                    break;
                case PlaytestPhaseSeventeenPackageType.DisconnectReconnect:
                    ValidateDisconnectReconnect();
                    break;
                case PlaytestPhaseSeventeenPackageType.VoiceChannel:
                    ValidateVoiceChannel();
                    break;
                case PlaytestPhaseSeventeenPackageType.UiLeakTest:
                    ValidateUiLeakTest();
                    break;
                case PlaytestPhaseSeventeenPackageType.HiddenEvidenceLeakTest:
                    ValidateHiddenEvidenceLeakTest();
                    break;
                case PlaytestPhaseSeventeenPackageType.DesyncTest:
                    ValidateDesyncTest();
                    break;
                case PlaytestPhaseSeventeenPackageType.BalanceNotes:
                    ValidateBalanceNotes();
                    break;
                case PlaytestPhaseSeventeenPackageType.PhaseClosure:
                    ValidatePhaseClosure();
                    break;
            }
        }

        private void ValidateEightPlayerScenario()
        {
            PlaytestScenarioState state = BuildState();
            PlaytestScenarioResult result = _playtestService.ValidateEightPlayerScenario(state);
            LogResult("EightPlayerScenario", result.Passed, result.Message);
        }

        private void ValidateKillerKillFlow()
        {
            PlaytestScenarioResult result = _playtestService.ValidateKillerKillFlow(BuildState());
            LogResult("KillerKillFlow", result.Passed, result.Message);
        }

        private void ValidateCorpseInspectAnnounce()
        {
            PlaytestScenarioResult result = _playtestService.ValidateCorpseInspectAnnounce(BuildState());
            LogResult("CorpseInspectAnnounce", result.Passed, result.Message);
        }

        private void ValidateSabotageRepair()
        {
            PlaytestScenarioResult result = _playtestService.ValidateSabotageRepair(BuildState());
            LogResult("SabotageRepair", result.Passed, result.Message);
        }

        private void ValidateTaskCompany()
        {
            PlaytestScenarioResult result = _playtestService.ValidateTaskCompany(BuildState());
            LogResult("TaskCompany", result.Passed, result.Message);
        }

        private void ValidateMeetingVoting()
        {
            PlaytestScenarioResult result = _playtestService.ValidateMeetingVoting(BuildState());
            LogResult("MeetingVoting", result.Passed, result.Message);
        }

        private void ValidateWrongAccusation()
        {
            PlaytestScenarioResult result = _playtestService.ValidateWrongAccusation(BuildState());
            LogResult("WrongAccusation", result.Passed, result.Message);
        }

        private void ValidateCorrectAccusation()
        {
            PlaytestScenarioResult result = _playtestService.ValidateCorrectAccusation(BuildState());
            LogResult("CorrectAccusation", result.Passed, result.Message);
        }

        private void ValidateFinalAccusation()
        {
            PlaytestScenarioResult result = _playtestService.ValidateFinalAccusation(BuildState());
            LogResult("FinalAccusation", result.Passed, result.Message);
        }

        private void ValidateAllVictimsDeadCompanyWin()
        {
            PlaytestScenarioResult result = _playtestService.ValidateAllVictimsDeadCompanyWin(BuildState());
            LogResult("AllVictimsDeadCompanyWin", result.Passed, result.Message);
        }

        private void ValidateDisconnectReconnect()
        {
            PlaytestScenarioResult result = _playtestService.ValidateDisconnectReconnect(BuildState());
            LogResult("DisconnectReconnect", result.Passed, result.Message);
        }

        private void ValidateVoiceChannel()
        {
            PlaytestScenarioResult result = _playtestService.ValidateVoiceChannel(BuildState());
            LogResult("VoiceChannel", result.Passed, result.Message);
        }

        private void ValidateUiLeakTest()
        {
            PlaytestScenarioResult result = _playtestService.ValidateUiLeakTest(BuildState());
            LogResult("UiLeakTest", result.Passed, result.Message);
        }

        private void ValidateHiddenEvidenceLeakTest()
        {
            PlaytestScenarioResult result = _playtestService.ValidateHiddenEvidenceLeakTest(BuildState());
            LogResult("HiddenEvidenceLeakTest", result.Passed, result.Message);
        }

        private void ValidateDesyncTest()
        {
            PlaytestScenarioResult result = _playtestService.ValidateDesyncTest(BuildState());
            LogResult("DesyncTest", result.Passed, result.Message);
        }

        private void ValidateBalanceNotes()
        {
            PlaytestBalanceReport report = _playtestService.BuildBalanceNotes(BuildState());
            bool passed = report.Notes.Count >= 5 && !report.RequiresTuning;
            LogResult("BalanceNotes", passed, $"Notes={report.Notes.Count}, RequiresTuning={report.RequiresTuning}");
        }

        private void ValidatePhaseClosure()
        {
            List<PlaytestScenarioResult> results = new List<PlaytestScenarioResult>();
            PlaytestScenarioState state = BuildState();

            results.Add(_playtestService.ValidateEightPlayerScenario(state));
            results.Add(_playtestService.ValidateKillerKillFlow(state));
            results.Add(_playtestService.ValidateCorpseInspectAnnounce(state));
            results.Add(_playtestService.ValidateSabotageRepair(state));
            results.Add(_playtestService.ValidateTaskCompany(state));
            results.Add(_playtestService.ValidateMeetingVoting(state));
            results.Add(_playtestService.ValidateWrongAccusation(state));
            results.Add(_playtestService.ValidateCorrectAccusation(state));
            results.Add(_playtestService.ValidateFinalAccusation(state));
            results.Add(_playtestService.ValidateAllVictimsDeadCompanyWin(state));
            results.Add(_playtestService.ValidateDisconnectReconnect(state));
            results.Add(_playtestService.ValidateVoiceChannel(state));
            results.Add(_playtestService.ValidateUiLeakTest(state));
            results.Add(_playtestService.ValidateHiddenEvidenceLeakTest(state));
            results.Add(_playtestService.ValidateDesyncTest(state));

            PlaytestBalanceReport report = _playtestService.BuildBalanceNotes(state);
            results.Add(new PlaytestScenarioResult("playtest.balance_notes", report.Notes.Count >= 5 && !report.RequiresTuning, report.RequiresTuning ? PlaytestSeverity.Medium : PlaytestSeverity.None, state.Winner, $"BalanceNotes={report.Notes.Count}"));

            bool passed = _playtestService.IsFullLoopPlayable(results);
            LogResult("PhaseClosure", passed, $"MVP Faz 17 packages 17A-17P are represented. Results={results.Count}");
        }

        private PlaytestScenarioState BuildState()
        {
            return _scenarioBuilder.BuildEightPlayerScenario();
        }

        private void LogResult(string scenario, bool passed, string message)
        {
            lastScenario = scenario;
            lastPassed = passed;
            lastMessage = message;

            if (passed)
                Debug.Log($"[PlaytestPhaseSeventeenDebugHarness] PASS {scenario}: {message}");
            else
                Debug.LogError($"[PlaytestPhaseSeventeenDebugHarness] FAIL {scenario}: {message}");
        }
    }
}
#pragma warning restore 0414
