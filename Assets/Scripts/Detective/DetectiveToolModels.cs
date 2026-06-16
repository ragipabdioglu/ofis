using System.Collections.Generic;
using OFIS.Roles;

namespace OFIS.Detective
{
    public enum DetectiveFlagSeverity
    {
        Low = 0,
        Medium = 1,
        High = 2
    }

    public enum DetectiveClaimComparisonType
    {
        TaskLog = 0,
        DoorAccess = 1,
        CorpseTraceAge = 2,
        MeetingAttendance = 3,
        CameraPassage = 4
    }

    public readonly struct DetectiveDashboardState
    {
        public bool IsVisible { get; }
        public bool IsOwnerOnly { get; }
        public int PinnedItemCount { get; }
        public int FlagCount { get; }

        public DetectiveDashboardState(bool isVisible, bool isOwnerOnly, int pinnedItemCount, int flagCount)
        {
            IsVisible = isVisible;
            IsOwnerOnly = isOwnerOnly;
            PinnedItemCount = pinnedItemCount < 0 ? 0 : pinnedItemCount;
            FlagCount = flagCount < 0 ? 0 : flagCount;
        }
    }

    public readonly struct DetectivePrivateNote
    {
        public string NoteId { get; }
        public string OwnerPlayerId { get; }
        public string Text { get; }

        public DetectivePrivateNote(string noteId, string ownerPlayerId, string text)
        {
            NoteId = string.IsNullOrWhiteSpace(noteId) ? "unknown_note" : noteId;
            OwnerPlayerId = string.IsNullOrWhiteSpace(ownerPlayerId) ? "unknown_owner" : ownerPlayerId;
            Text = string.IsNullOrWhiteSpace(text) ? "Empty note." : text;
        }
    }

    public readonly struct DetectivePinnedItem
    {
        public string PinId { get; }
        public string OwnerPlayerId { get; }
        public string SourceId { get; }
        public string Label { get; }

        public DetectivePinnedItem(string pinId, string ownerPlayerId, string sourceId, string label)
        {
            PinId = string.IsNullOrWhiteSpace(pinId) ? "unknown_pin" : pinId;
            OwnerPlayerId = string.IsNullOrWhiteSpace(ownerPlayerId) ? "unknown_owner" : ownerPlayerId;
            SourceId = string.IsNullOrWhiteSpace(sourceId) ? "unknown_source" : sourceId;
            Label = string.IsNullOrWhiteSpace(label) ? SourceId : label;
        }
    }

    public readonly struct DetectiveSuspectProfile
    {
        public string SuspectPlayerId { get; }
        public int ClaimCount { get; }
        public int FlagCount { get; }

        public DetectiveSuspectProfile(string suspectPlayerId, int claimCount, int flagCount)
        {
            SuspectPlayerId = string.IsNullOrWhiteSpace(suspectPlayerId) ? "unknown_suspect" : suspectPlayerId;
            ClaimCount = claimCount < 0 ? 0 : claimCount;
            FlagCount = flagCount < 0 ? 0 : flagCount;
        }
    }

    public readonly struct DetectiveClaimCard
    {
        public string ClaimId { get; }
        public string OwnerPlayerId { get; }
        public string SubjectPlayerId { get; }
        public string ClaimText { get; }

        public DetectiveClaimCard(string claimId, string ownerPlayerId, string subjectPlayerId, string claimText)
        {
            ClaimId = string.IsNullOrWhiteSpace(claimId) ? "unknown_claim" : claimId;
            OwnerPlayerId = string.IsNullOrWhiteSpace(ownerPlayerId) ? "unknown_owner" : ownerPlayerId;
            SubjectPlayerId = string.IsNullOrWhiteSpace(subjectPlayerId) ? "unknown_subject" : subjectPlayerId;
            ClaimText = string.IsNullOrWhiteSpace(claimText) ? "No claim." : claimText;
        }
    }

    public readonly struct DetectiveContradictionFlag
    {
        public string FlagId { get; }
        public DetectiveClaimComparisonType ComparisonType { get; }
        public DetectiveFlagSeverity Severity { get; }
        public string Message { get; }

        public DetectiveContradictionFlag(
            string flagId,
            DetectiveClaimComparisonType comparisonType,
            DetectiveFlagSeverity severity,
            string message)
        {
            FlagId = string.IsNullOrWhiteSpace(flagId) ? "unknown_flag" : flagId;
            ComparisonType = comparisonType;
            Severity = severity;
            Message = string.IsNullOrWhiteSpace(message) ? "Contradiction flag." : message;
        }
    }

    public readonly struct DetectiveTimelineItem
    {
        public float ServerTimeSeconds { get; }
        public string Label { get; }

        public DetectiveTimelineItem(float serverTimeSeconds, string label)
        {
            ServerTimeSeconds = serverTimeSeconds < 0f ? 0f : serverTimeSeconds;
            Label = string.IsNullOrWhiteSpace(label) ? "Timeline event." : label;
        }
    }

    public readonly struct DetectiveFinalTheory
    {
        public string OwnerPlayerId { get; }
        public IReadOnlyList<string> SuspectPlayerIds { get; }
        public string Summary { get; }
        public bool IsPrivate { get; }

        public DetectiveFinalTheory(string ownerPlayerId, IReadOnlyList<string> suspectPlayerIds, string summary, bool isPrivate)
        {
            OwnerPlayerId = string.IsNullOrWhiteSpace(ownerPlayerId) ? "unknown_owner" : ownerPlayerId;
            SuspectPlayerIds = suspectPlayerIds ?? new List<string>();
            Summary = string.IsNullOrWhiteSpace(summary) ? "No final theory." : summary;
            IsPrivate = isPrivate;
        }
    }

    public readonly struct DetectiveOwnerPayload
    {
        public string OwnerPlayerId { get; }
        public PlayerRole TargetRole { get; }
        public bool IsOwnerOnly { get; }
        public bool IsPublicSafe { get; }

        public DetectiveOwnerPayload(string ownerPlayerId, PlayerRole targetRole, bool isOwnerOnly, bool isPublicSafe)
        {
            OwnerPlayerId = string.IsNullOrWhiteSpace(ownerPlayerId) ? "unknown_owner" : ownerPlayerId;
            TargetRole = targetRole;
            IsOwnerOnly = isOwnerOnly;
            IsPublicSafe = isPublicSafe;
        }
    }
}
