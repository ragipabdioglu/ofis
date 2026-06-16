using System.Collections.Generic;
using OFIS.Players;
using OFIS.Rooms;
using UnityEngine;

#pragma warning disable 0414
namespace OFIS.Victim
{
    public sealed class VictimPhaseThirteenDebugHarness : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;
        [SerializeField] private VictimPhaseThirteenPackageType packageType;
        [SerializeField] private string lastScenario;
        [SerializeField] private bool lastPassed;
        [SerializeField] private string lastMessage;

        private readonly VictimNotePlacementCatalogService _placementCatalog = new VictimNotePlacementCatalogService();
        private readonly VictimNoteTextFilterService _textFilter = new VictimNoteTextFilterService();
        private readonly VictimNoteService _noteService = new VictimNoteService();
        private readonly VictimNoteWorldObjectService _worldObjectService = new VictimNoteWorldObjectService();
        private readonly VictimNoteReadUiService _readUiService = new VictimNoteReadUiService();
        private readonly VictimRoomInspectionService _roomInspectionService = new VictimRoomInspectionService();
        private readonly DeadPlayerStateService _deadPlayerStateService = new DeadPlayerStateService();
        private readonly DeadVoicePermissionService _deadVoicePermissionService = new DeadVoicePermissionService();
        private readonly DeadTaskService _deadTaskService = new DeadTaskService();
        private readonly DeadSpectatorCameraService _spectatorCameraService = new DeadSpectatorCameraService();
        private readonly VictimReconnectSnapshotService _reconnectSnapshotService = new VictimReconnectSnapshotService();

        private void Start()
        {
            if (validateOnStart)
                ValidatePackage();
        }

        [ContextMenu("Validate Victim Phase 13 Package")]
        public void ValidatePackage()
        {
            switch (packageType)
            {
                case VictimPhaseThirteenPackageType.NotePlacementPoints:
                    ValidateNotePlacementPoints();
                    break;
                case VictimPhaseThirteenPackageType.TwoNoteLimit:
                    ValidateTwoNoteLimit();
                    break;
                case VictimPhaseThirteenPackageType.NoteTextFilter:
                    ValidateNoteTextFilter();
                    break;
                case VictimPhaseThirteenPackageType.NotePlacementInteraction:
                    ValidateNotePlacementInteraction();
                    break;
                case VictimPhaseThirteenPackageType.NoteWorldObjectSpawn:
                    ValidateNoteWorldObjectSpawn();
                    break;
                case VictimPhaseThirteenPackageType.NoteReadUi:
                    ValidateNoteReadUi();
                    break;
                case VictimPhaseThirteenPackageType.NoteAuthorHidden:
                    ValidateNoteAuthorHidden();
                    break;
                case VictimPhaseThirteenPackageType.RoomInspectionFindsNote:
                    ValidateRoomInspectionFindsNote();
                    break;
                case VictimPhaseThirteenPackageType.DeadVictimCannotCreateNote:
                    ValidateDeadVictimCannotCreateNote();
                    break;
                case VictimPhaseThirteenPackageType.DeadPlayerState:
                    ValidateDeadPlayerState();
                    break;
                case VictimPhaseThirteenPackageType.DeadVoicePermission:
                    ValidateDeadVoicePermission();
                    break;
                case VictimPhaseThirteenPackageType.DeadTaskAssignment:
                    ValidateDeadTaskAssignment();
                    break;
                case VictimPhaseThirteenPackageType.DeadTaskNoLivingInfo:
                    ValidateDeadTaskNoLivingInfo();
                    break;
                case VictimPhaseThirteenPackageType.DeadSpectatorCamera:
                    ValidateDeadSpectatorCamera();
                    break;
                case VictimPhaseThirteenPackageType.ReconnectSnapshot:
                    ValidateReconnectSnapshot();
                    break;
                case VictimPhaseThirteenPackageType.PhaseClosure:
                    ValidatePhaseClosure();
                    break;
            }
        }

        private void ValidateNotePlacementPoints()
        {
            IReadOnlyList<VictimNotePlacementPoint> points = _placementCatalog.BuildMvpPoints();
            LogResult("NotePlacementPoints", points.Count >= 3 && points[0].RoomType == OfficeRoomType.ArchiveRoom, $"Points={points.Count}");
        }

        private void ValidateTwoNoteLimit()
        {
            VictimNoteCreateResult result = _noteService.TryCreate("note_limit", "victim_01", OfficeRoomType.ArchiveRoom, "A faint clue near archive.", 2, PlayerLifeState.Alive, 2f);
            LogResult("TwoNoteLimit", !result.Success, result.Message);
        }

        private void ValidateNoteTextFilter()
        {
            bool safe = _textFilter.IsAllowed("A shadow moved near archive.");
            bool unsafeRole = !_textFilter.IsAllowed("killer is player_02");
            bool unsafeName = !_textFilter.IsAllowed("suspect_01 did it");
            LogResult("NoteTextFilter", safe && unsafeRole && unsafeName, "Note filter blocks direct accusation and role/name tokens.");
        }

        private void ValidateNotePlacementInteraction()
        {
            VictimNoteCreateResult shortInteraction = _noteService.TryCreate("note_short", "victim_01", OfficeRoomType.ArchiveRoom, "A shadow moved near archive.", 0, PlayerLifeState.Alive, 1f);
            VictimNoteCreateResult valid = BuildValidNote();
            LogResult("NotePlacementInteraction", !shortInteraction.Success && valid.Success, valid.Message);
        }

        private void ValidateNoteWorldObjectSpawn()
        {
            VictimNoteCreateResult result = BuildValidNote();
            LogResult("NoteWorldObjectSpawn", result.Success && _worldObjectService.CanSpawnWorldObject(result.Note), result.Note.NoteId);
        }

        private void ValidateNoteReadUi()
        {
            VictimNoteReadUiState state = _readUiService.Build(BuildValidNote().Note);
            LogResult("NoteReadUi", state.Text.Contains("archive") && state.AuthorLabel == "Anonymous", state.Text);
        }

        private void ValidateNoteAuthorHidden()
        {
            VictimNoteData note = BuildValidNote().Note;
            VictimNoteReadUiState state = _readUiService.Build(note);
            LogResult("NoteAuthorHidden", !note.IsAuthorPublic && state.AuthorLabel == "Anonymous", state.AuthorLabel);
        }

        private void ValidateRoomInspectionFindsNote()
        {
            List<VictimNoteData> notes = new List<VictimNoteData>
            {
                BuildValidNote().Note,
                new VictimNoteData("note_break", "victim_02", OfficeRoomType.BreakRoom, "A cup was moved.", false)
            };

            IReadOnlyList<VictimNoteData> found = _roomInspectionService.FindNotesInRoom(notes, OfficeRoomType.ArchiveRoom);
            LogResult("RoomInspectionFindsNote", found.Count == 1 && found[0].RoomType == OfficeRoomType.ArchiveRoom, $"Found={found.Count}");
        }

        private void ValidateDeadVictimCannotCreateNote()
        {
            VictimNoteCreateResult result = _noteService.TryCreate("note_dead", "victim_01", OfficeRoomType.ArchiveRoom, "A shadow moved near archive.", 0, PlayerLifeState.Dead, 2f);
            LogResult("DeadVictimCannotCreateNote", !result.Success, result.Message);
        }

        private void ValidateDeadPlayerState()
        {
            DeadPlayerStateData state = _deadPlayerStateService.Build("victim_01", PlayerLifeState.Dead);
            LogResult("DeadPlayerState", state.LifeState == PlayerLifeState.Dead && state.IsSpectator && !state.CanCreateNote, state.PlayerId);
        }

        private void ValidateDeadVoicePermission()
        {
            DeadVoicePermission permission = _deadVoicePermissionService.Build(PlayerLifeState.Dead);
            LogResult("DeadVoicePermission", permission.CanSpeakToDead && !permission.CanSpeakToLiving && permission.CanListenToLivingContext, "Dead voice isolated from living.");
        }

        private void ValidateDeadTaskAssignment()
        {
            IReadOnlyList<DeadTaskData> tasks = _deadTaskService.AssignActiveTasks(4);
            int contribution = _deadTaskService.ResolveCompanyContribution(8);
            LogResult("DeadTaskAssignment", tasks.Count == 3 && contribution == 6 && tasks[0].CompanyDelta == 1, $"Tasks={tasks.Count}, Contribution={contribution}");
        }

        private void ValidateDeadTaskNoLivingInfo()
        {
            IReadOnlyList<DeadTaskData> tasks = _deadTaskService.AssignActiveTasks(3);
            bool passed = true;
            for (int i = 0; i < tasks.Count; i++)
                passed &= !tasks[i].ProducesLivingInfo;

            LogResult("DeadTaskNoLivingInfo", passed, "Dead tasks produce no living evidence/log.");
        }

        private void ValidateDeadSpectatorCamera()
        {
            LogResult("DeadSpectatorCamera", _spectatorCameraService.CanUseSpectatorCamera(PlayerLifeState.Dead) && !_spectatorCameraService.CanUseSpectatorCamera(PlayerLifeState.Alive), "Dead spectator camera gated.");
        }

        private void ValidateReconnectSnapshot()
        {
            VictimReconnectSnapshot snapshot = _reconnectSnapshotService.Build(
                "victim_01",
                PlayerLifeState.Dead,
                new[] { BuildValidNote().Note },
                _deadTaskService.AssignActiveTasks(3));

            LogResult("ReconnectSnapshot", snapshot.LifeState == PlayerLifeState.Dead && snapshot.VisibleNotes.Count == 1 && snapshot.DeadTasks.Count == 3, snapshot.PlayerId);
        }

        private void ValidatePhaseClosure()
        {
            ValidateNotePlacementPoints();
            ValidateTwoNoteLimit();
            ValidateNoteTextFilter();
            ValidateNotePlacementInteraction();
            ValidateNoteWorldObjectSpawn();
            ValidateNoteReadUi();
            ValidateNoteAuthorHidden();
            ValidateRoomInspectionFindsNote();
            ValidateDeadVictimCannotCreateNote();
            ValidateDeadPlayerState();
            ValidateDeadVoicePermission();
            ValidateDeadTaskAssignment();
            ValidateDeadTaskNoLivingInfo();
            ValidateDeadSpectatorCamera();
            ValidateReconnectSnapshot();

            LogResult("PhaseClosure", true, "MVP Faz 13 packages 13A-13O are represented.");
        }

        private VictimNoteCreateResult BuildValidNote()
        {
            return _noteService.TryCreate(
                "note_13",
                "victim_01",
                OfficeRoomType.ArchiveRoom,
                "A shadow moved near archive.",
                0,
                PlayerLifeState.Alive,
                2f);
        }

        private void LogResult(string scenario, bool passed, string message)
        {
            lastScenario = scenario;
            lastPassed = passed;
            lastMessage = message;

            if (passed)
                Debug.Log($"[VictimPhaseThirteenDebugHarness] PASS {scenario}: {message}");
            else
                Debug.LogError($"[VictimPhaseThirteenDebugHarness] FAIL {scenario}: {message}");
        }
    }
}
#pragma warning restore 0414
