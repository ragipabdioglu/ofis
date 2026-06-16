using System.Collections.Generic;
using OFIS.Rooms;
using OFIS.Sabotage;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Meetings
{
    public sealed class MeetingDetectiveContradictionDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;

        [Header("Read Only Debug Output")]
        [SerializeField] private int lastEventCount;
        [SerializeField] private MeetingDetectiveContradictionFlagType lastFlagType;
        [SerializeField] private string lastMessage;
        [SerializeField] private bool lastVisibleTextIsSafe;

        private readonly MeetingDetectiveContradictionService _service =
            new MeetingDetectiveContradictionService();

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateDetectiveContradictions();
        }

        [ContextMenu("Validate Detective Contradictions")]
        public void ValidateDetectiveContradictions()
        {
            ValidatePlayerTargetMismatchRaisesEvent();
            ValidateRoomTargetMismatchRaisesEvent();
            ValidateNoActionAgainstActionableReportRaisesEvent();
            ValidateMatchingReportDoesNotRaiseEvent();
            ValidateUnresolvedProposalDoesNotRaiseEvent();
            ValidateEventsDoNotRevealRoleOrDefiniteKiller();
        }

        private void ValidatePlayerTargetMismatchRaisesEvent()
        {
            MeetingDetectiveContradictionResult result = _service.Evaluate(
                new[] { BuildReport("report_player_mismatch", MeetingReportType.Suspicion, "detective_01", "player_a", OfficeRoomType.MeetingRoom) },
                BuildResolution(BuildProposal(
                    "proposal_player_action",
                    MeetingActionType.OfficialAccusation,
                    MeetingActionTargetData.ForPlayer("player_b"))));

            bool passed = result.EventCount == 1
                && result.Events[0].FlagType == MeetingDetectiveContradictionFlagType.PlayerTargetMismatch;

            LogResult("PlayerTargetMismatchRaisesEvent", passed, result);
        }

        private void ValidateRoomTargetMismatchRaisesEvent()
        {
            MeetingDetectiveContradictionResult result = _service.Evaluate(
                new[] { BuildReport("report_room_mismatch", MeetingReportType.LocationClaim, "detective_01", "none", OfficeRoomType.SecurityRoom) },
                BuildResolution(BuildProposal(
                    "proposal_room_action",
                    MeetingActionType.RoomInspection,
                    MeetingActionTargetData.ForRoom(OfficeRoomType.ArchiveRoom))));

            bool passed = result.EventCount == 1
                && result.Events[0].FlagType == MeetingDetectiveContradictionFlagType.RoomTargetMismatch;

            LogResult("RoomTargetMismatchRaisesEvent", passed, result);
        }

        private void ValidateNoActionAgainstActionableReportRaisesEvent()
        {
            MeetingDetectiveContradictionResult result = _service.Evaluate(
                new[] { BuildReport("report_no_action", MeetingReportType.CorpseReport, "detective_01", "player_a", OfficeRoomType.Hallway) },
                BuildResolution(BuildProposal(
                    "proposal_no_action",
                    MeetingActionType.NoAction,
                    MeetingActionTargetData.None())));

            bool passed = result.EventCount == 1
                && result.Events[0].FlagType == MeetingDetectiveContradictionFlagType.NoActionAgainstActionableReport;

            LogResult("NoActionAgainstActionableReportRaisesEvent", passed, result);
        }

        private void ValidateMatchingReportDoesNotRaiseEvent()
        {
            MeetingDetectiveContradictionResult result = _service.Evaluate(
                new[] { BuildReport("report_match", MeetingReportType.Suspicion, "detective_01", "player_a", OfficeRoomType.MeetingRoom) },
                BuildResolution(BuildProposal(
                    "proposal_match",
                    MeetingActionType.PersonnelAudit,
                    MeetingActionTargetData.ForPlayer("player_a"))));

            LogResult("MatchingReportDoesNotRaiseEvent", result.EventCount == 0, result);
        }

        private void ValidateUnresolvedProposalDoesNotRaiseEvent()
        {
            MeetingDetectiveContradictionResult result = _service.Evaluate(
                new[] { BuildReport("report_unresolved", MeetingReportType.Suspicion, "detective_01", "player_a", OfficeRoomType.MeetingRoom) },
                new MeetingActionProposalResolutionResult(
                    false,
                    MeetingActionProposalResolutionType.None,
                    default(MeetingActionProposalData),
                    0,
                    0,
                    0,
                    "No resolution."));

            LogResult("UnresolvedProposalDoesNotRaiseEvent", result.EventCount == 0, result);
        }

        private void ValidateEventsDoNotRevealRoleOrDefiniteKiller()
        {
            MeetingDetectiveContradictionResult result = _service.Evaluate(
                new[] { BuildReport("report_safety", MeetingReportType.Suspicion, "detective_01", "player_a", OfficeRoomType.MeetingRoom) },
                BuildResolution(BuildProposal(
                    "proposal_safety",
                    MeetingActionType.OfficialAccusation,
                    MeetingActionTargetData.ForPlayer("player_b"))));

            string visibleText = result.EventCount > 0 ? result.Events[0].Message : string.Empty;
            bool safe = !ContainsText(visibleText, "killer")
                && !ContainsText(visibleText, "murderer")
                && !ContainsText(visibleText, "role");

            bool passed = result.EventCount == 1 && safe;
            LogResult("EventsDoNotRevealRoleOrDefiniteKiller", passed, result);
        }

        private static MeetingReportData BuildReport(
            string reportId,
            MeetingReportType reportType,
            string reporterPlayerId,
            string targetPlayerId,
            OfficeRoomType roomType)
        {
            return new MeetingReportData(
                reportId,
                reportType,
                reporterPlayerId,
                targetPlayerId,
                roomType,
                0,
                0,
                SabotageObjectiveState.None,
                "Detective contradiction debug report.");
        }

        private static MeetingActionProposalData BuildProposal(
            string proposalId,
            MeetingActionType actionType,
            MeetingActionTargetData target)
        {
            MeetingActionRequestData request = new MeetingActionRequestData(
                proposalId,
                "player_proposer",
                actionType,
                target,
                "Detective contradiction debug proposal.");

            return new MeetingActionProposalData(
                proposalId,
                "meeting_detective_debug",
                request,
                MeetingActionProposalStatus.Resolved);
        }

        private static MeetingActionProposalResolutionResult BuildResolution(
            MeetingActionProposalData proposal)
        {
            return new MeetingActionProposalResolutionResult(
                true,
                MeetingActionProposalResolutionType.MajorityReached,
                proposal,
                3,
                2,
                2,
                "Detective contradiction debug resolution.");
        }

        private static bool ContainsText(string source, string expected)
        {
            if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(expected))
                return false;

            return source.IndexOf(expected, System.StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void LogResult(
            string testName,
            bool passed,
            MeetingDetectiveContradictionResult result)
        {
            lastEventCount = result.EventCount;
            lastFlagType = result.EventCount > 0
                ? result.Events[0].FlagType
                : MeetingDetectiveContradictionFlagType.None;
            lastMessage = result.Message;
            lastVisibleTextIsSafe = result.EventCount == 0
                || (!ContainsText(result.Events[0].Message, "killer")
                    && !ContainsText(result.Events[0].Message, "role"));

            if (passed)
                Debug.Log($"[MeetingDetectiveContradictionValidator] PASS {testName}: {result}");
            else
                Debug.LogError($"[MeetingDetectiveContradictionValidator] FAIL {testName}: {result}");
        }
    }
}
#pragma warning restore 0414
