using OFIS.Company;
using OFIS.Interaction;
using OFIS.Roles;
using UnityEngine;

namespace OFIS.Tasks
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class TaskStation : MonoBehaviour, IInteractable
    {
        [Header("Task Station")]
        [SerializeField] private string taskName = "Computer Work";
        [SerializeField] private TaskStationType stationType = TaskStationType.Computer;
        [SerializeField] private float durationSeconds = 2f;
        [SerializeField] private bool canInteract = true;

        [Header("Services")]
        [SerializeField] private CompanyHealthService companyHealthService;
        [SerializeField] private ActiveTaskProgressService activeTaskProgressService;

        private bool _isInProgress;
        private bool _isCompleted;
        private float _progressTimer;
        private InteractionContext _currentContext;
        private TaskAttemptResult _lastResult = TaskAttemptResult.None;

        public string DisplayName => _isCompleted
            ? $"{taskName} (Done)"
            : taskName;

        public InteractionType InteractionType => InteractionType.TaskStation;

        public bool CanInteract => canInteract && !_isInProgress && !_isCompleted;

        public bool IsInProgress => _isInProgress;
        public bool IsCompleted => _isCompleted;
        public float Progress01 => durationSeconds <= 0f ? 1f : Mathf.Clamp01(_progressTimer / durationSeconds);
        public string TaskName => taskName;
        public TaskStationType StationType => stationType;
        public TaskAttemptResult LastResult => _lastResult;

        public string DebugStatus
        {
            get
            {
                if (_isInProgress)
                    return "InProgress";

                if (_isCompleted)
                    return "Done";

        return "Available";
    }
}

        private void Reset()
        {
            var collider = GetComponent<Collider2D>();
            collider.isTrigger = true;
        }

        private void Awake()
        {
            var collider = GetComponent<Collider2D>();
            collider.isTrigger = true;

            if (companyHealthService == null)
                companyHealthService = FindFirstObjectByType<CompanyHealthService>();

            if (activeTaskProgressService == null)
                activeTaskProgressService = FindFirstObjectByType<ActiveTaskProgressService>();
        }

        private void Update()
        {
            if (!_isInProgress)
                return;

            _progressTimer += Time.deltaTime;

            if (_progressTimer >= durationSeconds)
                CompleteTask();
        }

        public void Interact(InteractionContext context)
        {
            if (!canInteract)
            {
                Debug.Log($"[TaskStation] {taskName} is blocked.");
                return;
            }

            if (_isCompleted)
            {
                Debug.Log($"[TaskStation] {taskName} is already completed. Interaction blocked.");
                return;
            }

            if (_isInProgress)
            {
                Debug.Log($"[TaskStation] {taskName} is already in progress.");
                return;
            }

            _currentContext = context;
            _progressTimer = 0f;
            _isInProgress = true;

            if (activeTaskProgressService != null)
                activeTaskProgressService.SetActiveTask(this);

            string actorName = GetActorName(context);

            Debug.Log(
                $"[TaskStation] {actorName} started task '{taskName}'. " +
                $"Station={stationType}, Duration={durationSeconds:0.0}s");
        }

        private void CompleteTask()
        {
            _isInProgress = false;
            _isCompleted = true;
            _progressTimer = durationSeconds;

            string actorName = GetActorName(_currentContext);
            PlayerRole role = GetActorRole(_currentContext);

            TaskAttemptResult result = role == PlayerRole.Killer
                ? TaskAttemptResult.FaultyCompleted
                : TaskAttemptResult.Completed;

            _lastResult = result;

            ApplyCompanyImpact(result);

            Debug.Log(
                $"[TaskStation] {actorName} completed task '{taskName}'. " +
                $"Station={stationType}, Role={role}, Result={result}, IsCompleted={_isCompleted}");

            if (activeTaskProgressService != null)
                activeTaskProgressService.ClearActiveTask(this);
        }

        private void ApplyCompanyImpact(TaskAttemptResult result)
        {
            if (companyHealthService == null)
            {
                Debug.LogWarning($"[TaskStation] CompanyHealthService missing. Task='{taskName}' result did not affect company health.");
                return;
            }

            if (result == TaskAttemptResult.Completed)
            {
                companyHealthService.ApplyTaskCompleted(taskName);
                return;
            }

            if (result == TaskAttemptResult.FaultyCompleted)
            {
                companyHealthService.ApplyFaultyTaskCompleted(taskName);
                return;
            }
        }

        private static string GetActorName(InteractionContext context)
        {
            if (context.IdentityBinding == null)
                return context.Actor == null ? "Unknown Actor" : context.Actor.name;

            return context.IdentityBinding.DisplayName;
        }

        private static PlayerRole GetActorRole(InteractionContext context)
        {
            if (context.IdentityBinding == null)
                return default;

            return context.IdentityBinding.OwnRole;
        }
    }
}