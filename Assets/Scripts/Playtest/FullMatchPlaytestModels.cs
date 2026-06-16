using System.Collections.Generic;

namespace OFIS.Playtest
{
    public enum PlaytestWinType
    {
        None = 0,
        GoodSide = 1,
        Killers = 2
    }

    public enum PlaytestSeverity
    {
        None = 0,
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4
    }

    public readonly struct PlaytestPlayerSeat
    {
        public string PlayerId { get; }
        public string RoleKey { get; }
        public bool IsAlive { get; }
        public bool Connected { get; }

        public PlaytestPlayerSeat(string playerId, string roleKey, bool isAlive, bool connected)
        {
            PlayerId = string.IsNullOrWhiteSpace(playerId) ? "unknown_player" : playerId;
            RoleKey = string.IsNullOrWhiteSpace(roleKey) ? "role.unknown" : roleKey;
            IsAlive = isAlive;
            Connected = connected;
        }
    }

    public sealed class PlaytestScenarioState
    {
        private readonly List<PlaytestPlayerSeat> _players = new List<PlaytestPlayerSeat>();
        private readonly List<string> _events = new List<string>();

        public IReadOnlyList<PlaytestPlayerSeat> Players => _players;
        public IReadOnlyList<string> Events => _events;
        public int CompanyHealth { get; private set; }
        public int CompletedTaskCount { get; private set; }
        public int KillCount { get; private set; }
        public int CorpseAnnounceCount { get; private set; }
        public int SabotageRepairCount { get; private set; }
        public int MeetingVoteCount { get; private set; }
        public bool SoftlockDetected { get; private set; }
        public bool RoleLeakDetected { get; private set; }
        public bool HiddenEvidenceLeakDetected { get; private set; }
        public bool DesyncDetected { get; private set; }
        public PlaytestWinType Winner { get; private set; }

        public PlaytestScenarioState(IEnumerable<PlaytestPlayerSeat> players, int companyHealth)
        {
            if (players != null)
                _players.AddRange(players);

            CompanyHealth = companyHealth < 0 ? 0 : companyHealth > 100 ? 100 : companyHealth;
            Winner = PlaytestWinType.None;
        }

        public void AddEvent(string eventKey)
        {
            if (!string.IsNullOrWhiteSpace(eventKey))
                _events.Add(eventKey);
        }

        public void CompleteTask(int companyDelta)
        {
            CompletedTaskCount++;
            CompanyHealth = ClampCompany(CompanyHealth + companyDelta);
            AddEvent("event.task_completed");
        }

        public void RegisterKill()
        {
            KillCount++;
            AddEvent("event.kill_resolved");
        }

        public void AnnounceCorpse()
        {
            CorpseAnnounceCount++;
            AddEvent("event.corpse_announced");
        }

        public void RepairSabotage()
        {
            SabotageRepairCount++;
            CompanyHealth = ClampCompany(CompanyHealth + 3);
            AddEvent("event.sabotage_repaired");
        }

        public void CastVote()
        {
            MeetingVoteCount++;
            AddEvent("event.vote_cast");
        }

        public void SetWinner(PlaytestWinType winner)
        {
            Winner = winner;
            AddEvent("event.match_resolved");
        }

        public void MarkSoftlock()
        {
            SoftlockDetected = true;
        }

        public void MarkRoleLeak()
        {
            RoleLeakDetected = true;
        }

        public void MarkHiddenEvidenceLeak()
        {
            HiddenEvidenceLeakDetected = true;
        }

        public void MarkDesync()
        {
            DesyncDetected = true;
        }

        private static int ClampCompany(int value)
        {
            if (value < 0)
                return 0;

            return value > 100 ? 100 : value;
        }
    }

    public readonly struct PlaytestScenarioResult
    {
        public string ScenarioKey { get; }
        public bool Passed { get; }
        public PlaytestSeverity Severity { get; }
        public PlaytestWinType Winner { get; }
        public string Message { get; }

        public PlaytestScenarioResult(string scenarioKey, bool passed, PlaytestSeverity severity, PlaytestWinType winner, string message)
        {
            ScenarioKey = string.IsNullOrWhiteSpace(scenarioKey) ? "playtest.scenario.unknown" : scenarioKey;
            Passed = passed;
            Severity = severity;
            Winner = winner;
            Message = string.IsNullOrWhiteSpace(message) ? "Scenario resolved." : message;
        }
    }

    public readonly struct BalanceNote
    {
        public string MetricKey { get; }
        public float Value { get; }
        public string Note { get; }

        public BalanceNote(string metricKey, float value, string note)
        {
            MetricKey = string.IsNullOrWhiteSpace(metricKey) ? "balance.metric.unknown" : metricKey;
            Value = value;
            Note = string.IsNullOrWhiteSpace(note) ? "No note." : note;
        }
    }

    public sealed class PlaytestBalanceReport
    {
        private readonly List<BalanceNote> _notes = new List<BalanceNote>();

        public IReadOnlyList<BalanceNote> Notes => _notes;
        public bool RequiresTuning { get; }

        public PlaytestBalanceReport(IEnumerable<BalanceNote> notes, bool requiresTuning)
        {
            if (notes != null)
                _notes.AddRange(notes);

            RequiresTuning = requiresTuning;
        }
    }
}
