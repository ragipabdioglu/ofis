using System.Collections.Generic;
using System.Linq;

namespace OFIS.Tasks
{
    public sealed class PlayerTaskList
    {
        private readonly List<OfficeTaskRuntimeState> _tasks;

        public string PlayerId { get; }
        public IReadOnlyList<OfficeTaskRuntimeState> Tasks => _tasks;
        public int TotalCount => _tasks.Count;
        public int CompletedCount => _tasks.Count(task => task != null && task.IsCompleted);
        public int RemainingCount => TotalCount - CompletedCount;
        public bool AllCompleted => TotalCount > 0 && CompletedCount == TotalCount;

        public PlayerTaskList(string playerId, IEnumerable<OfficeTaskRuntimeState> tasks)
        {
            PlayerId = string.IsNullOrWhiteSpace(playerId) ? "unknown_player" : playerId;
            _tasks = tasks == null
                ? new List<OfficeTaskRuntimeState>()
                : new List<OfficeTaskRuntimeState>(tasks.Where(task => task != null));
        }

        public OfficeTaskRuntimeState FindTaskById(string taskId)
        {
            if (string.IsNullOrWhiteSpace(taskId))
                return null;

            return _tasks.FirstOrDefault(task => task != null && task.Definition.TaskId == taskId);
        }

        public override string ToString()
        {
            return $"PlayerId={PlayerId}, Total={TotalCount}, Completed={CompletedCount}, Remaining={RemainingCount}, AllCompleted={AllCompleted}";
        }
    }
}
