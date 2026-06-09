using System.Collections.Generic;
using OFIS.Core.Events;
using OFIS.MatchFlow.Config;
using OFIS.MatchFlow.Events;
using OFIS.MatchFlow.States;
using UnityEngine;

namespace OFIS.MatchFlow
{
    public sealed class MatchFlowService
    {
        private readonly MatchFlowConfig _config;
        private readonly GameEventBus _eventBus;

        private List<MatchTimelineEntry> _timeline;
        private MatchState _currentState;
        private float _matchTimeSeconds;
        private float _activeMatchDurationSeconds;
        private bool _isRunning;
        private bool _isFastTest;

        private bool _sentThirtySecondWarning;
        private bool _sentTenSecondWarning;

        public MatchState CurrentState => _currentState;
        public float MatchTimeSeconds => _matchTimeSeconds;
        public float ActiveMatchDurationSeconds => _activeMatchDurationSeconds;
        public float MatchRemainingSeconds => Mathf.Max(0f, _activeMatchDurationSeconds - _matchTimeSeconds);
        public bool IsRunning => _isRunning;
        public bool IsFastTest => _isFastTest;

        public float CurrentStateElapsedSeconds
        {
            get
            {
                var entry = GetCurrentTimelineEntry();
                return entry == null ? 0f : Mathf.Max(0f, _matchTimeSeconds - entry.startTimeSeconds);
            }
        }

        public float CurrentStateRemainingSeconds
        {
            get
            {
                var entry = GetCurrentTimelineEntry();
                return entry == null ? 0f : Mathf.Max(0f, entry.endTimeSeconds - _matchTimeSeconds);
            }
        }

        public MatchFlowService(MatchFlowConfig config, GameEventBus eventBus)
        {
            _config = config;
            _eventBus = eventBus;

            _timeline = _config.BuildDefaultTimeline();
            _activeMatchDurationSeconds = _config.totalMatchDurationSeconds;

            _currentState = MatchState.None;
            _matchTimeSeconds = 0f;
            _isRunning = false;
            _isFastTest = false;
        }

        public void StartNormalMatch()
        {
            _timeline = _config.BuildDefaultTimeline();
            _activeMatchDurationSeconds = _config.totalMatchDurationSeconds;
            _isFastTest = false;

            StartInternal();
        }

        public void StartFastTestMatch()
        {
            _timeline = _config.BuildFastTestTimeline();
            _activeMatchDurationSeconds = _config.GetFastTestDurationSeconds();
            _isFastTest = true;

            StartInternal();
        }

        public void StopMatch()
        {
            if (!_isRunning && _currentState == MatchState.MatchEnded)
                return;

            _isRunning = false;
            SetState(MatchState.MatchEnded);
            PublishTimerTick();

            Debug.Log("[MatchFlow] Match stopped.");
        }

        public void Tick(float deltaTime)
        {
            if (!_isRunning)
                return;

            _matchTimeSeconds += deltaTime;

            if (_matchTimeSeconds >= _activeMatchDurationSeconds)
            {
                _matchTimeSeconds = _activeMatchDurationSeconds;
                SetState(MatchState.ResolvingMatch);
                _isRunning = false;
                PublishTimerTick();
                return;
            }

            var nextState = GetStateForTime(_matchTimeSeconds);

            if (nextState != _currentState)
            {
                ResetMeetingWarnings();
                SetState(nextState);
            }

            PublishMeetingWarningsIfNeeded();
            PublishTimerTick();
        }

        private void StartInternal()
        {
            _matchTimeSeconds = 0f;
            _isRunning = true;

            _sentThirtySecondWarning = false;
            _sentTenSecondWarning = false;

            SetState(MatchState.OfficePhase1);

            Debug.Log(_isFastTest
                ? "[MatchFlow] Fast test match started."
                : "[MatchFlow] Normal match started.");
        }

        private MatchState GetStateForTime(float matchTimeSeconds)
        {
            for (int i = 0; i < _timeline.Count; i++)
            {
                if (_timeline[i].ContainsTime(matchTimeSeconds))
                    return _timeline[i].state;
            }

            return MatchState.ResolvingMatch;
        }

        private MatchTimelineEntry GetCurrentTimelineEntry()
        {
            for (int i = 0; i < _timeline.Count; i++)
            {
                if (_timeline[i].state == _currentState)
                    return _timeline[i];
            }

            return null;
        }

        private MatchTimelineEntry GetNextMeetingEntry()
        {
            for (int i = 0; i < _timeline.Count; i++)
            {
                var entry = _timeline[i];

                if (entry.startTimeSeconds <= _matchTimeSeconds)
                    continue;

                if (entry.state is MatchState.Meeting1 or MatchState.Meeting2 or MatchState.FinalMeeting)
                    return entry;
            }

            return null;
        }

        private void SetState(MatchState newState)
        {
            if (_currentState == newState)
                return;

            var previous = _currentState;
            _currentState = newState;

            Debug.Log($"[MatchFlow] State changed: {previous} -> {_currentState} at {_matchTimeSeconds:0.00}s");

            _eventBus.Publish(new MatchStateChangedEvent(
                previous,
                _currentState,
                _matchTimeSeconds,
                Time.realtimeSinceStartup));
        }

        private void PublishTimerTick()
        {
            _eventBus.Publish(new MatchTimerTickEvent(
                _currentState,
                _matchTimeSeconds,
                MatchRemainingSeconds,
                CurrentStateElapsedSeconds,
                CurrentStateRemainingSeconds,
                Time.realtimeSinceStartup));
        }

        private void PublishMeetingWarningsIfNeeded()
        {
            if (!_currentState.IsOfficePhase())
                return;

            var nextMeeting = GetNextMeetingEntry();

            if (nextMeeting == null)
                return;

            float secondsUntilMeeting = nextMeeting.startTimeSeconds - _matchTimeSeconds;

            if (!_sentThirtySecondWarning && secondsUntilMeeting <= _config.meetingAnnouncementSeconds)
            {
                _sentThirtySecondWarning = true;

                _eventBus.Publish(new MeetingWarningEvent(
                    nextMeeting.state,
                    secondsUntilMeeting,
                    false,
                    Time.realtimeSinceStartup));

                Debug.Log($"[MatchFlow] Meeting announcement: {nextMeeting.state} starts in {secondsUntilMeeting:0.0}s");
            }

            if (!_sentTenSecondWarning && secondsUntilMeeting <= _config.meetingRedWarningSeconds)
            {
                _sentTenSecondWarning = true;

                _eventBus.Publish(new MeetingWarningEvent(
                    nextMeeting.state,
                    secondsUntilMeeting,
                    true,
                    Time.realtimeSinceStartup));

                Debug.Log($"[MatchFlow] RED warning: {nextMeeting.state} starts in {secondsUntilMeeting:0.0}s");
            }
        }

        private void ResetMeetingWarnings()
        {
            _sentThirtySecondWarning = false;
            _sentTenSecondWarning = false;
        }
    }
}