using System.Collections.Generic;
using OFIS.Core.Ids;
using OFIS.Roles;

namespace OFIS.Playable
{
    public sealed class PlayableParticipant
    {
        public PlayableParticipant(PlayerId playerId, string displayName, bool isLocalPlayer, bool isBot)
        {
            PlayerId = playerId;
            DisplayName = displayName;
            IsLocalPlayer = isLocalPlayer;
            IsBot = isBot;
            Role = PlayerRole.None;
            IsAlive = true;
            IsReady = false;
            CurrentRoom = "Lobby";
            CompletedTasks = 0;
        }

        public PlayerId PlayerId { get; }
        public string DisplayName { get; }
        public bool IsLocalPlayer { get; }
        public bool IsBot { get; }
        public PlayerRole Role { get; set; }
        public bool IsAlive { get; set; }
        public bool IsReady { get; set; }
        public bool IsCarryingCorpse { get; set; }
        public string CurrentRoom { get; set; }
        public int CompletedTasks { get; set; }
        public List<PlayerId> KnownVictimTargets { get; } = new List<PlayerId>();
    }

    public sealed class PlayableSessionSnapshot
    {
        public PlayableSessionSnapshot(
            PlayableSessionState state,
            IReadOnlyList<PlayableParticipant> participants,
            string activePlayerName,
            PlayerRole activeRole,
            int companyHealth,
            int meetingCount,
            int completedTasks,
            bool corpseSpawned,
            bool corpseInspected,
            bool sabotageActive,
            bool meetingTransientClean,
            bool resultReady,
            string status)
        {
            State = state;
            Participants = participants;
            ActivePlayerName = activePlayerName;
            ActiveRole = activeRole;
            CompanyHealth = companyHealth;
            MeetingCount = meetingCount;
            CompletedTasks = completedTasks;
            CorpseSpawned = corpseSpawned;
            CorpseInspected = corpseInspected;
            SabotageActive = sabotageActive;
            MeetingTransientClean = meetingTransientClean;
            ResultReady = resultReady;
            Status = status;
        }

        public PlayableSessionState State { get; }
        public IReadOnlyList<PlayableParticipant> Participants { get; }
        public string ActivePlayerName { get; }
        public PlayerRole ActiveRole { get; }
        public int CompanyHealth { get; }
        public int MeetingCount { get; }
        public int CompletedTasks { get; }
        public bool CorpseSpawned { get; }
        public bool CorpseInspected { get; }
        public bool SabotageActive { get; }
        public bool MeetingTransientClean { get; }
        public bool ResultReady { get; }
        public string Status { get; }
    }

    public readonly struct PlayableActionResult
    {
        public PlayableActionResult(bool passed, string message)
        {
            Passed = passed;
            Message = message;
        }

        public bool Passed { get; }
        public string Message { get; }
    }
}
