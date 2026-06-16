using System.Collections.Generic;

namespace OFIS.Meetings
{
    public sealed class MeetingAttendanceState
    {
        private readonly HashSet<string> _registeredPlayerIds;
        private readonly HashSet<string> _lateObserverPlayerIds;

        public IReadOnlyCollection<string> RegisteredPlayerIds => _registeredPlayerIds;
        public IReadOnlyCollection<string> LateObserverPlayerIds => _lateObserverPlayerIds;

        public bool IsRegistrationLocked { get; private set; }
        public bool HasStarted { get; private set; }

        public int RegisteredCount => _registeredPlayerIds.Count;
        public int LateObserverCount => _lateObserverPlayerIds.Count;

        public MeetingAttendanceState()
        {
            _registeredPlayerIds = new HashSet<string>();
            _lateObserverPlayerIds = new HashSet<string>();
            IsRegistrationLocked = false;
            HasStarted = false;
        }

        public void Reset()
        {
            _registeredPlayerIds.Clear();
            _lateObserverPlayerIds.Clear();
            IsRegistrationLocked = false;
            HasStarted = false;
        }

        public void MarkStarted()
        {
            HasStarted = true;
        }

        public void LockRegistration()
        {
            IsRegistrationLocked = true;
        }

        public bool RegisterPlayer(string playerId)
        {
            if (string.IsNullOrWhiteSpace(playerId))
                return false;

            if (IsRegistrationLocked)
                return false;

            HasStarted = true;
            return _registeredPlayerIds.Add(playerId.Trim());
        }

        public bool RegisterLateObserver(string playerId)
        {
            if (string.IsNullOrWhiteSpace(playerId))
                return false;

            HasStarted = true;
            return _lateObserverPlayerIds.Add(playerId.Trim());
        }

        public bool IsRegistered(string playerId)
        {
            if (string.IsNullOrWhiteSpace(playerId))
                return false;

            return _registeredPlayerIds.Contains(playerId.Trim());
        }

        public bool IsLateObserver(string playerId)
        {
            if (string.IsNullOrWhiteSpace(playerId))
                return false;

            return _lateObserverPlayerIds.Contains(playerId.Trim());
        }

        public override string ToString()
        {
            return $"Started={HasStarted}, Locked={IsRegistrationLocked}, Registered={RegisteredCount}, LateObservers={LateObserverCount}";
        }
    }
}
