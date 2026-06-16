using System.Collections.Generic;
using System.Linq;

namespace OFIS.Meetings
{
    public sealed class MeetingAttendanceRegistrationResult
    {
        private readonly List<string> _registeredPlayerIds;
        private readonly List<string> _missingEligiblePlayerIds;
        private readonly List<string> _lateObserverPlayerIds;
        private readonly List<string> _ignoredPlayerIds;

        public IReadOnlyList<string> RegisteredPlayerIds => _registeredPlayerIds;
        public IReadOnlyList<string> MissingEligiblePlayerIds => _missingEligiblePlayerIds;
        public IReadOnlyList<string> LateObserverPlayerIds => _lateObserverPlayerIds;
        public IReadOnlyList<string> IgnoredPlayerIds => _ignoredPlayerIds;

        public int RegisteredCount => _registeredPlayerIds.Count;
        public int MissingEligibleCount => _missingEligiblePlayerIds.Count;
        public int LateObserverCount => _lateObserverPlayerIds.Count;
        public int IgnoredCount => _ignoredPlayerIds.Count;

        public bool HasRegisteredPlayers => RegisteredCount > 0;
        public bool HasMissingEligiblePlayers => MissingEligibleCount > 0;
        public bool HasLateObservers => LateObserverCount > 0;

        public string Message { get; }

        public MeetingAttendanceRegistrationResult(
            IEnumerable<string> registeredPlayerIds,
            IEnumerable<string> missingEligiblePlayerIds,
            IEnumerable<string> lateObserverPlayerIds,
            IEnumerable<string> ignoredPlayerIds,
            string message)
        {
            _registeredPlayerIds = NormalizeIds(registeredPlayerIds);
            _missingEligiblePlayerIds = NormalizeIds(missingEligiblePlayerIds);
            _lateObserverPlayerIds = NormalizeIds(lateObserverPlayerIds);
            _ignoredPlayerIds = NormalizeIds(ignoredPlayerIds);

            Message = string.IsNullOrWhiteSpace(message)
                ? "Meeting attendance registration completed."
                : message;
        }

        private static List<string> NormalizeIds(IEnumerable<string> playerIds)
        {
            if (playerIds == null)
                return new List<string>();

            return playerIds
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Select(id => id.Trim())
                .Distinct()
                .ToList();
        }

        public override string ToString()
        {
            return $"Registered={RegisteredCount}, MissingEligible={MissingEligibleCount}, LateObservers={LateObserverCount}, Ignored={IgnoredCount}, Message={Message}";
        }
    }
}
