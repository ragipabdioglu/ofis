using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.ReleaseCandidate
{
    public sealed class MvpAllPhasesValidationRunner : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;
        [SerializeField] private int expectedPhaseClosureCount = 11;
        [SerializeField] private string lastSummary;
        [SerializeField] private bool lastPassed;
        [SerializeField] private int lastValidatedCount;
        [SerializeField] private int lastIssueCount;

        private static readonly string[] RequiredHarnessTypeNames =
        {
            "EvidencePhaseEightDebugHarness",
            "LogsPhaseNineDebugHarness",
            "SabotagePhaseTenDebugHarness",
            "AccusationPhaseElevenDebugHarness",
            "DetectivePhaseTwelveDebugHarness",
            "VictimPhaseThirteenDebugHarness",
            "CommunicationPhaseFourteenDebugHarness",
            "UiPhaseFifteenDebugHarness",
            "NetworkingPhaseSixteenDebugHarness",
            "PlaytestPhaseSeventeenDebugHarness",
            "ReleaseCandidatePhaseEighteenDebugHarness"
        };

        private readonly List<string> _issues = new List<string>();

        private void Start()
        {
            if (validateOnStart)
                ValidateMvpAllPhases();
        }

        [ContextMenu("Validate MVP All Phases")]
        public void ValidateMvpAllPhases()
        {
            _issues.Clear();
            lastValidatedCount = 0;
            Application.logMessageReceived += CaptureIssueLog;

            try
            {
                MonoBehaviour[] behaviours = FindObjectsOfType<MonoBehaviour>(true);
                for (int i = 0; i < RequiredHarnessTypeNames.Length; i++)
                {
                    MonoBehaviour harness = FindHarness(behaviours, RequiredHarnessTypeNames[i]);
                    if (harness == null)
                    {
                        _issues.Add($"Missing harness: {RequiredHarnessTypeNames[i]}");
                        continue;
                    }

                    MethodInfo validatePackage = harness.GetType().GetMethod(
                        "ValidatePackage",
                        BindingFlags.Instance | BindingFlags.Public);

                    if (validatePackage == null)
                    {
                        _issues.Add($"Missing ValidatePackage: {RequiredHarnessTypeNames[i]}");
                        continue;
                    }

                    try
                    {
                        validatePackage.Invoke(harness, null);
                        lastValidatedCount++;
                    }
                    catch (Exception exception)
                    {
                        Exception root = exception.InnerException ?? exception;
                        _issues.Add($"{RequiredHarnessTypeNames[i]} threw {root.GetType().Name}: {root.Message}");
                    }
                }
            }
            finally
            {
                Application.logMessageReceived -= CaptureIssueLog;
            }

            lastIssueCount = _issues.Count;
            lastPassed = lastValidatedCount == expectedPhaseClosureCount && lastIssueCount == 0;
            lastSummary = $"Validated={lastValidatedCount}/{expectedPhaseClosureCount}, Issues={lastIssueCount}";

            if (lastPassed)
                Debug.Log($"[MvpAllPhasesValidationRunner] PASS MVP_ALL_PHASES: {lastSummary}");
            else
                Debug.LogError($"[MvpAllPhasesValidationRunner] FAIL MVP_ALL_PHASES: {lastSummary}. {string.Join(" | ", _issues)}");
        }

        private void CaptureIssueLog(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Exception || type == LogType.Error || condition.Contains(" FAIL "))
                _issues.Add(condition);
        }

        private static MonoBehaviour FindHarness(IReadOnlyList<MonoBehaviour> behaviours, string typeName)
        {
            for (int i = 0; i < behaviours.Count; i++)
            {
                MonoBehaviour behaviour = behaviours[i];
                if (behaviour != null && behaviour.GetType().Name == typeName)
                    return behaviour;
            }

            return null;
        }
    }
}
#pragma warning restore 0414
