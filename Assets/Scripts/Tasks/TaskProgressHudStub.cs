using System.Collections.Generic;
using OFIS.Rooms;
using UnityEngine;

namespace OFIS.Tasks
{
    public sealed class TaskProgressHudStub : MonoBehaviour
    {
        [SerializeField] private bool showUi = true;
        [SerializeField] private Vector2 screenPosition = new Vector2(20f, 20f);
        [SerializeField] private Vector2 size = new Vector2(300f, 90f);

        private readonly PlayerTaskAssignmentService _assignmentService = new PlayerTaskAssignmentService();
        private readonly TaskProgressUiStateService _uiStateService = new TaskProgressUiStateService();
        private PlayerTaskList _taskList;

        public TaskProgressUiState CurrentState { get; private set; }

        private void Awake()
        {
            PlayerTaskAssignmentResult assignment = _assignmentService.AssignTasks("debug_player", BuildDebugDefinitions());
            _taskList = assignment.TaskList;
            RefreshState();
        }

        private void Update()
        {
            RefreshState();
        }

        [ContextMenu("Refresh Task Progress HUD State")]
        public TaskProgressUiState RefreshState()
        {
            CurrentState = _uiStateService.Build(_taskList);
            return CurrentState;
        }

        public void SetTaskList(PlayerTaskList taskList)
        {
            _taskList = taskList;
            RefreshState();
        }

        private void OnGUI()
        {
            if (!showUi)
                return;

            GUI.Box(new Rect(screenPosition.x, screenPosition.y, size.x, size.y), CurrentState.HeaderText);
            GUI.Label(new Rect(screenPosition.x + 10f, screenPosition.y + 30f, size.x - 20f, 22f), $"Progress: {CurrentState.ProgressText}");
            GUI.Label(new Rect(screenPosition.x + 10f, screenPosition.y + 55f, size.x - 20f, 22f), $"Status: {CurrentState.StatusText}");
        }

        private static List<OfficeTaskDefinition> BuildDebugDefinitions()
        {
            return new List<OfficeTaskDefinition>
            {
                new OfficeTaskDefinition("task_review_invoices", "Review invoices", OfficeRoomType.Accounting, 3f),
                new OfficeTaskDefinition("task_check_server_logs", "Check server logs", OfficeRoomType.ServerRoom, 4f),
                new OfficeTaskDefinition("task_sort_archive", "Sort archive files", OfficeRoomType.ArchiveRoom, 5f)
            };
        }
    }
}
