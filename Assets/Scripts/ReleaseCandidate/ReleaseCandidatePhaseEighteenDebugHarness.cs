using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.ReleaseCandidate
{
    public sealed class ReleaseCandidatePhaseEighteenDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;
        [SerializeField] private ReleaseCandidatePhaseEighteenPackageType packageType;
        [SerializeField] private string lastScenario;
        [SerializeField] private bool lastPassed;
        [SerializeField] private string lastMessage;

        private readonly ReleaseCandidateGateService _gateService = new ReleaseCandidateGateService();
        private readonly ReleaseCandidateFixtureFactory _fixtures = new ReleaseCandidateFixtureFactory();

        private void Start()
        {
            if (validateOnStart)
                ValidatePackage();
        }

        [ContextMenu("Validate Release Candidate Phase 18 Package")]
        public void ValidatePackage()
        {
            switch (packageType)
            {
                case ReleaseCandidatePhaseEighteenPackageType.DebugOnlyUiSeparation:
                    ValidateDebugOnlyUiSeparation();
                    break;
                case ReleaseCandidatePhaseEighteenPackageType.BuildSettings:
                    ValidateBuildSettings();
                    break;
                case ReleaseCandidatePhaseEighteenPackageType.BasicMainMenu:
                    ValidateBasicMainMenu();
                    break;
                case ReleaseCandidatePhaseEighteenPackageType.LobbyFlow:
                    ValidateLobbyFlow();
                    break;
                case ReleaseCandidatePhaseEighteenPackageType.MatchStartEndFlow:
                    ValidateMatchStartEndFlow();
                    break;
                case ReleaseCandidatePhaseEighteenPackageType.ErrorHandling:
                    ValidateErrorHandling();
                    break;
                case ReleaseCandidatePhaseEighteenPackageType.PerformanceCheck:
                    ValidatePerformanceCheck();
                    break;
                case ReleaseCandidatePhaseEighteenPackageType.NetworkTimeoutHandling:
                    ValidateNetworkTimeoutHandling();
                    break;
                case ReleaseCandidatePhaseEighteenPackageType.CrashLogExport:
                    ValidateCrashLogExport();
                    break;
                case ReleaseCandidatePhaseEighteenPackageType.MvpBuildDocument:
                    ValidateMvpBuildDocument();
                    break;
                case ReleaseCandidatePhaseEighteenPackageType.KnownIssuesList:
                    ValidateKnownIssuesList();
                    break;
                case ReleaseCandidatePhaseEighteenPackageType.FirstExternalTestPlan:
                    ValidateFirstExternalTestPlan();
                    break;
                case ReleaseCandidatePhaseEighteenPackageType.PhaseClosure:
                    ValidatePhaseClosure();
                    break;
            }
        }

        private void ValidateDebugOnlyUiSeparation()
        {
            ReleaseGateResult result = _gateService.ValidateDebugOnlyUiSeparation(_fixtures.BuildDebugUiPolicy());
            LogResult("DebugOnlyUiSeparation", result.Passed, result.Message);
        }

        private void ValidateBuildSettings()
        {
            ReleaseGateResult result = _gateService.ValidateBuildSettings(_fixtures.BuildSettings());
            LogResult("BuildSettings", result.Passed, result.Message);
        }

        private void ValidateBasicMainMenu()
        {
            ReleaseGateResult result = _gateService.ValidateBasicMainMenu(_fixtures.BuildMenuFlow());
            LogResult("BasicMainMenu", result.Passed, result.Message);
        }

        private void ValidateLobbyFlow()
        {
            ReleaseGateResult result = _gateService.ValidateLobbyFlow(_fixtures.BuildMenuFlow());
            LogResult("LobbyFlow", result.Passed, result.Message);
        }

        private void ValidateMatchStartEndFlow()
        {
            ReleaseGateResult result = _gateService.ValidateMatchStartEndFlow(_fixtures.BuildMenuFlow());
            LogResult("MatchStartEndFlow", result.Passed, result.Message);
        }

        private void ValidateErrorHandling()
        {
            ReleaseGateResult result = _gateService.ValidateErrorHandling(_fixtures.BuildErrorPolicy());
            LogResult("ErrorHandling", result.Passed, result.Message);
        }

        private void ValidatePerformanceCheck()
        {
            ReleaseGateResult result = _gateService.ValidatePerformance(_fixtures.BuildPerformanceSnapshot());
            LogResult("PerformanceCheck", result.Passed, result.Message);
        }

        private void ValidateNetworkTimeoutHandling()
        {
            ReleaseGateResult result = _gateService.ValidateNetworkTimeout(_fixtures.BuildNetworkTimeoutPolicy());
            LogResult("NetworkTimeoutHandling", result.Passed, result.Message);
        }

        private void ValidateCrashLogExport()
        {
            ReleaseGateResult result = _gateService.ValidateCrashLogExport(_fixtures.BuildCrashExportPolicy());
            LogResult("CrashLogExport", result.Passed, result.Message);
        }

        private void ValidateMvpBuildDocument()
        {
            ReleaseGateResult result = _gateService.ValidateMvpBuildDocument(_fixtures.BuildMvpDocument());
            LogResult("MvpBuildDocument", result.Passed, result.Message);
        }

        private void ValidateKnownIssuesList()
        {
            ReleaseGateResult result = _gateService.ValidateKnownIssues(_fixtures.BuildKnownIssues());
            LogResult("KnownIssuesList", result.Passed, result.Message);
        }

        private void ValidateFirstExternalTestPlan()
        {
            ReleaseGateResult result = _gateService.ValidateExternalTestPlan(_fixtures.BuildExternalTestPlan());
            LogResult("FirstExternalTestPlan", result.Passed, result.Message);
        }

        private void ValidatePhaseClosure()
        {
            List<ReleaseGateResult> gates = BuildAllGateResults();
            bool passed = _gateService.CanShipMvpReleaseCandidate(gates);
            LogResult("PhaseClosure", passed, $"MVP Faz 18 packages 18A-18L are represented. Gates={gates.Count}");
        }

        private List<ReleaseGateResult> BuildAllGateResults()
        {
            return new List<ReleaseGateResult>
            {
                _gateService.ValidateDebugOnlyUiSeparation(_fixtures.BuildDebugUiPolicy()),
                _gateService.ValidateBuildSettings(_fixtures.BuildSettings()),
                _gateService.ValidateBasicMainMenu(_fixtures.BuildMenuFlow()),
                _gateService.ValidateLobbyFlow(_fixtures.BuildMenuFlow()),
                _gateService.ValidateMatchStartEndFlow(_fixtures.BuildMenuFlow()),
                _gateService.ValidateErrorHandling(_fixtures.BuildErrorPolicy()),
                _gateService.ValidatePerformance(_fixtures.BuildPerformanceSnapshot()),
                _gateService.ValidateNetworkTimeout(_fixtures.BuildNetworkTimeoutPolicy()),
                _gateService.ValidateCrashLogExport(_fixtures.BuildCrashExportPolicy()),
                _gateService.ValidateMvpBuildDocument(_fixtures.BuildMvpDocument()),
                _gateService.ValidateKnownIssues(_fixtures.BuildKnownIssues()),
                _gateService.ValidateExternalTestPlan(_fixtures.BuildExternalTestPlan())
            };
        }

        private void LogResult(string scenario, bool passed, string message)
        {
            lastScenario = scenario;
            lastPassed = passed;
            lastMessage = message;

            if (passed)
                Debug.Log($"[ReleaseCandidatePhaseEighteenDebugHarness] PASS {scenario}: {message}");
            else
                Debug.LogError($"[ReleaseCandidatePhaseEighteenDebugHarness] FAIL {scenario}: {message}");
        }
    }
}
#pragma warning restore 0414
