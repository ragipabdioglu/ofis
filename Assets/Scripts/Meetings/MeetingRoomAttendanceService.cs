using System.Collections.Generic;
using System.Linq;

namespace OFIS.Meetings
{
    public sealed class MeetingRoomAttendanceService
    {
        private readonly MeetingAttendanceState _state;

        public MeetingRoomAttendanceService()
        {
            _state = new MeetingAttendanceState();
        }

        public MeetingRoomAttendanceService(MeetingAttendanceState state)
        {
            _state = state ?? new MeetingAttendanceState();
        }

        public MeetingAttendanceState State => _state;

        public void Reset()
        {
            _state.Reset();
        }

        public MeetingAttendanceRegistrationResult RegisterMeetingStartAttendance(
            IReadOnlyList<MeetingAttendancePlayerSnapshot> players)
        {
            _state.Reset();
            _state.MarkStarted();

            if (players == null || players.Count == 0)
            {
                _state.LockRegistration();

                return new MeetingAttendanceRegistrationResult(
                    new List<string>(),
                    new List<string>(),
                    new List<string>(),
                    new List<string>(),
                    "Meeting started with no player snapshots.");
            }

            List<string> registered = new List<string>();
            List<string> missingEligible = new List<string>();
            List<string> ignored = new List<string>();

            for (int i = 0; i < players.Count; i++)
            {
                MeetingAttendancePlayerSnapshot player = players[i];

                if (!player.IsEligible)
                {
                    ignored.Add(player.PlayerId);
                    continue;
                }

                if (player.CanRegisterForMeeting)
                {
                    if (_state.RegisterPlayer(player.PlayerId))
                        registered.Add(player.PlayerId);

                    continue;
                }

                missingEligible.Add(player.PlayerId);
            }

            _state.LockRegistration();

            return new MeetingAttendanceRegistrationResult(
                registered,
                missingEligible,
                new List<string>(),
                ignored,
                "Meeting start attendance registered and locked.");
        }

        public MeetingAttendanceRegistrationResult RegisterLateJoinAttempt(
            MeetingAttendancePlayerSnapshot player)
        {
            if (!_state.HasStarted)
            {
                return new MeetingAttendanceRegistrationResult(
                    _state.RegisteredPlayerIds,
                    new List<string>(),
                    _state.LateObserverPlayerIds,
                    new List<string> { player.PlayerId },
                    "Meeting has not started yet. Late join ignored.");
            }

            if (!player.IsEligible)
            {
                return new MeetingAttendanceRegistrationResult(
                    _state.RegisteredPlayerIds,
                    new List<string>(),
                    _state.LateObserverPlayerIds,
                    new List<string> { player.PlayerId },
                    "Late join player is not eligible.");
            }

            if (_state.IsRegistered(player.PlayerId))
            {
                return new MeetingAttendanceRegistrationResult(
                    _state.RegisteredPlayerIds,
                    new List<string>(),
                    _state.LateObserverPlayerIds,
                    new List<string>(),
                    "Player is already registered.");
            }

            _state.RegisterLateObserver(player.PlayerId);

            return new MeetingAttendanceRegistrationResult(
                _state.RegisteredPlayerIds,
                new List<string>(),
                _state.LateObserverPlayerIds,
                new List<string>(),
                "Late join registered as observer.");
        }

        public bool IsPlayerAllowedToParticipate(string playerId)
        {
            return _state.IsRegistered(playerId);
        }

        public bool IsPlayerObserver(string playerId)
        {
            return _state.IsLateObserver(playerId);
        }

        public int CalculateMissingPlayerHealthPenalty(MeetingAttendanceRegistrationResult result, int penaltyPerMissingPlayer)
        {
            if (result == null || penaltyPerMissingPlayer <= 0)
                return 0;

            return result.MissingEligibleCount * penaltyPerMissingPlayer;
        }

        public IReadOnlyList<string> BuildParticipantList()
        {
            return _state.RegisteredPlayerIds.ToList();
        }
    }
}
