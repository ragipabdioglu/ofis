using System.Collections.Generic;
using OFIS.Players;
using OFIS.Rooms;
using UnityEngine;

namespace OFIS.Victim
{
    public sealed class VictimNotePlacementCatalogService
    {
        public IReadOnlyList<VictimNotePlacementPoint> BuildMvpPoints()
        {
            return new[]
            {
                new VictimNotePlacementPoint("note_point_archive", OfficeRoomType.ArchiveRoom, new Vector3(1f, 1f, 0f)),
                new VictimNotePlacementPoint("note_point_break", OfficeRoomType.BreakRoom, new Vector3(2f, 1f, 0f)),
                new VictimNotePlacementPoint("note_point_hallway", OfficeRoomType.Hallway, new Vector3(3f, 1f, 0f))
            };
        }
    }

    public sealed class VictimNoteTextFilterService
    {
        private static readonly string[] ForbiddenTokens =
        {
            "killer",
            "katil",
            "victim",
            "kurban",
            "detective",
            "dedektif",
            "player_",
            "suspect_",
            "murderer",
            "suclu",
            "suçlu"
        };

        public bool IsAllowed(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            string normalized = text.ToLowerInvariant();
            for (int i = 0; i < ForbiddenTokens.Length; i++)
            {
                if (normalized.Contains(ForbiddenTokens[i]))
                    return false;
            }

            return true;
        }
    }

    public sealed class VictimNoteService
    {
        private readonly VictimNoteTextFilterService _filter = new VictimNoteTextFilterService();

        public VictimNoteCreateResult TryCreate(
            string noteId,
            string authorPlayerId,
            OfficeRoomType roomType,
            string text,
            int existingNoteCount,
            PlayerLifeState authorLifeState,
            float interactionSeconds)
        {
            if (authorLifeState != PlayerLifeState.Alive)
                return new VictimNoteCreateResult(false, default, "Dead victim cannot create new note.");

            if (existingNoteCount >= 2)
                return new VictimNoteCreateResult(false, default, "Victim note limit reached.");

            if (interactionSeconds < 2f)
                return new VictimNoteCreateResult(false, default, "Victim note interaction requires 2 seconds.");

            if (!_filter.IsAllowed(text))
                return new VictimNoteCreateResult(false, default, "Victim note text rejected by filter.");

            VictimNoteData note = new VictimNoteData(noteId, authorPlayerId, roomType, text, false);
            return new VictimNoteCreateResult(true, note, "Victim note created.");
        }
    }

    public sealed class VictimNoteWorldObjectService
    {
        public bool CanSpawnWorldObject(VictimNoteData note)
        {
            return !string.IsNullOrWhiteSpace(note.NoteId) && note.RoomType != OfficeRoomType.None;
        }
    }

    public sealed class VictimNoteReadUiService
    {
        public VictimNoteReadUiState Build(VictimNoteData note)
        {
            string author = note.IsAuthorPublic ? note.AuthorPlayerId : "Anonymous";
            return new VictimNoteReadUiState(note.Text, author);
        }
    }

    public sealed class VictimRoomInspectionService
    {
        public IReadOnlyList<VictimNoteData> FindNotesInRoom(IReadOnlyList<VictimNoteData> notes, OfficeRoomType roomType)
        {
            List<VictimNoteData> result = new List<VictimNoteData>();

            if (notes == null)
                return result;

            for (int i = 0; i < notes.Count; i++)
            {
                if (notes[i].RoomType == roomType)
                    result.Add(notes[i]);
            }

            return result;
        }
    }

    public sealed class DeadPlayerStateService
    {
        public DeadPlayerStateData Build(string playerId, PlayerLifeState lifeState)
        {
            bool isDead = lifeState == PlayerLifeState.Dead;
            return new DeadPlayerStateData(playerId, lifeState, !isDead, isDead);
        }
    }

    public sealed class DeadVoicePermissionService
    {
        public DeadVoicePermission Build(PlayerLifeState lifeState)
        {
            bool isDead = lifeState == PlayerLifeState.Dead;
            return new DeadVoicePermission(isDead, false, isDead);
        }
    }

    public sealed class DeadTaskService
    {
        public IReadOnlyList<DeadTaskData> AssignActiveTasks(int count)
        {
            List<DeadTaskData> tasks = new List<DeadTaskData>();
            int taskCount = Mathf.Clamp(count, 0, 3);

            for (int i = 0; i < taskCount; i++)
                tasks.Add(new DeadTaskData($"dead_task_{i + 1}", +1, false));

            return tasks;
        }

        public int ResolveCompanyContribution(int completedTaskCount)
        {
            return Mathf.Clamp(completedTaskCount, 0, 6);
        }
    }

    public sealed class DeadSpectatorCameraService
    {
        public bool CanUseSpectatorCamera(PlayerLifeState lifeState)
        {
            return lifeState == PlayerLifeState.Dead;
        }
    }

    public sealed class VictimReconnectSnapshotService
    {
        public VictimReconnectSnapshot Build(
            string playerId,
            PlayerLifeState lifeState,
            IReadOnlyList<VictimNoteData> visibleNotes,
            IReadOnlyList<DeadTaskData> deadTasks)
        {
            return new VictimReconnectSnapshot(playerId, lifeState, visibleNotes, deadTasks);
        }
    }
}
