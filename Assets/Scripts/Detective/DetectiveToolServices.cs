using System.Collections.Generic;
using OFIS.Roles;

namespace OFIS.Detective
{
    public sealed class DetectiveDashboardService
    {
        public DetectiveDashboardState Build(PlayerRole viewerRole, int pinnedItemCount, int flagCount)
        {
            bool isDetective = viewerRole == PlayerRole.Detective;
            return new DetectiveDashboardState(isDetective, isDetective, pinnedItemCount, flagCount);
        }
    }

    public sealed class DetectivePrivateNoteService
    {
        public DetectivePrivateNote Create(string noteId, string ownerPlayerId, string text)
        {
            return new DetectivePrivateNote(noteId, ownerPlayerId, text);
        }
    }

    public sealed class DetectivePinService
    {
        public DetectivePinnedItem Pin(string pinId, string ownerPlayerId, string sourceId, string label)
        {
            return new DetectivePinnedItem(pinId, ownerPlayerId, sourceId, label);
        }
    }

    public sealed class DetectiveSuspectProfileService
    {
        public DetectiveSuspectProfile Build(string suspectPlayerId, IReadOnlyList<DetectiveClaimCard> claims, IReadOnlyList<DetectiveContradictionFlag> flags)
        {
            return new DetectiveSuspectProfile(suspectPlayerId, CountClaims(claims, suspectPlayerId), flags == null ? 0 : flags.Count);
        }

        private static int CountClaims(IReadOnlyList<DetectiveClaimCard> claims, string suspectPlayerId)
        {
            if (claims == null)
                return 0;

            int count = 0;
            for (int i = 0; i < claims.Count; i++)
            {
                if (claims[i].SubjectPlayerId == suspectPlayerId)
                    count++;
            }

            return count;
        }
    }

    public sealed class DetectiveClaimCardService
    {
        public DetectiveClaimCard Create(string claimId, string ownerPlayerId, string subjectPlayerId, string claimText)
        {
            return new DetectiveClaimCard(claimId, ownerPlayerId, subjectPlayerId, claimText);
        }
    }

    public sealed class DetectiveClaimComparisonService
    {
        public IReadOnlyList<DetectiveContradictionFlag> Compare(DetectiveClaimCard claim, IReadOnlyList<DetectiveClaimComparisonType> comparisonTypes)
        {
            List<DetectiveContradictionFlag> flags = new List<DetectiveContradictionFlag>();

            if (comparisonTypes == null)
                return flags;

            for (int i = 0; i < comparisonTypes.Count; i++)
            {
                DetectiveClaimComparisonType type = comparisonTypes[i];
                flags.Add(new DetectiveContradictionFlag(
                    $"flag_{claim.ClaimId}_{type}",
                    type,
                    ResolveSeverity(type),
                    $"Claim comparison signal: {type}."));
            }

            return flags;
        }

        private static DetectiveFlagSeverity ResolveSeverity(DetectiveClaimComparisonType comparisonType)
        {
            switch (comparisonType)
            {
                case DetectiveClaimComparisonType.CorpseTraceAge:
                case DetectiveClaimComparisonType.CameraPassage:
                    return DetectiveFlagSeverity.High;
                case DetectiveClaimComparisonType.DoorAccess:
                case DetectiveClaimComparisonType.MeetingAttendance:
                    return DetectiveFlagSeverity.Medium;
                default:
                    return DetectiveFlagSeverity.Low;
            }
        }
    }

    public sealed class DetectiveNoCertaintyGuardService
    {
        private static readonly string[] ForbiddenPhrases =
        {
            "definite killer",
            "kesin katil",
            "guaranteed killer",
            "confirmed killer"
        };

        public bool IsSafeConclusion(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return true;

            string normalized = text.ToLowerInvariant();
            for (int i = 0; i < ForbiddenPhrases.Length; i++)
            {
                if (normalized.Contains(ForbiddenPhrases[i]))
                    return false;
            }

            return true;
        }
    }

    public sealed class DetectiveTimelineService
    {
        public IReadOnlyList<DetectiveTimelineItem> Build(params DetectiveTimelineItem[] items)
        {
            List<DetectiveTimelineItem> result = new List<DetectiveTimelineItem>();
            if (items != null)
                result.AddRange(items);

            result.Sort((left, right) => left.ServerTimeSeconds.CompareTo(right.ServerTimeSeconds));
            return result;
        }
    }

    public sealed class DetectiveFinalTheoryService
    {
        private readonly DetectiveNoCertaintyGuardService _certaintyGuard = new DetectiveNoCertaintyGuardService();

        public DetectiveFinalTheory BuildPrivateTheory(string ownerPlayerId, IReadOnlyList<string> suspectPlayerIds, string summary)
        {
            string safeSummary = _certaintyGuard.IsSafeConclusion(summary) ? summary : "Theory contains unsafe certainty language.";
            return new DetectiveFinalTheory(ownerPlayerId, suspectPlayerIds, safeSummary, true);
        }
    }

    public sealed class DetectiveOwnerOnlyNetworkService
    {
        public DetectiveOwnerPayload BuildOwnerPayload(string ownerPlayerId, PlayerRole targetRole)
        {
            bool isDetective = targetRole == PlayerRole.Detective;
            return new DetectiveOwnerPayload(ownerPlayerId, targetRole, isDetective, isDetective);
        }
    }

    public sealed class DetectiveRoleSafeUiService
    {
        private readonly DetectiveNoCertaintyGuardService _certaintyGuard = new DetectiveNoCertaintyGuardService();

        public bool CanShowDashboard(PlayerRole viewerRole)
        {
            return viewerRole == PlayerRole.Detective;
        }

        public bool IsSafeUiText(string text)
        {
            return _certaintyGuard.IsSafeConclusion(text);
        }
    }
}
