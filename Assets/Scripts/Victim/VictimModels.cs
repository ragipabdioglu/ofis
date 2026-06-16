using System.Collections.Generic;
using OFIS.Players;
using OFIS.Rooms;
using UnityEngine;

namespace OFIS.Victim
{
    public readonly struct VictimNotePlacementPoint
    {
        public string PointId { get; }
        public OfficeRoomType RoomType { get; }
        public Vector3 WorldPosition { get; }

        public VictimNotePlacementPoint(string pointId, OfficeRoomType roomType, Vector3 worldPosition)
        {
            PointId = string.IsNullOrWhiteSpace(pointId) ? "unknown_note_point" : pointId;
            RoomType = roomType;
            WorldPosition = worldPosition;
        }
    }

    public readonly struct VictimNoteData
    {
        public string NoteId { get; }
        public string AuthorPlayerId { get; }
        public OfficeRoomType RoomType { get; }
        public string Text { get; }
        public bool IsAuthorPublic { get; }

        public VictimNoteData(string noteId, string authorPlayerId, OfficeRoomType roomType, string text, bool isAuthorPublic)
        {
            NoteId = string.IsNullOrWhiteSpace(noteId) ? "unknown_note" : noteId;
            AuthorPlayerId = string.IsNullOrWhiteSpace(authorPlayerId) ? "unknown_author" : authorPlayerId;
            RoomType = roomType;
            Text = string.IsNullOrWhiteSpace(text) ? "Unreadable note." : text;
            IsAuthorPublic = isAuthorPublic;
        }
    }

    public readonly struct VictimNoteCreateResult
    {
        public bool Success { get; }
        public VictimNoteData Note { get; }
        public string Message { get; }

        public VictimNoteCreateResult(bool success, VictimNoteData note, string message)
        {
            Success = success;
            Note = note;
            Message = string.IsNullOrWhiteSpace(message) ? "No victim note message." : message;
        }
    }

    public readonly struct VictimNoteReadUiState
    {
        public string Text { get; }
        public string AuthorLabel { get; }

        public VictimNoteReadUiState(string text, string authorLabel)
        {
            Text = string.IsNullOrWhiteSpace(text) ? "Unreadable note." : text;
            AuthorLabel = string.IsNullOrWhiteSpace(authorLabel) ? "Unknown" : authorLabel;
        }
    }

    public readonly struct DeadTaskData
    {
        public string TaskId { get; }
        public int CompanyDelta { get; }
        public bool ProducesLivingInfo { get; }

        public DeadTaskData(string taskId, int companyDelta, bool producesLivingInfo)
        {
            TaskId = string.IsNullOrWhiteSpace(taskId) ? "unknown_dead_task" : taskId;
            CompanyDelta = companyDelta;
            ProducesLivingInfo = producesLivingInfo;
        }
    }

    public readonly struct DeadPlayerStateData
    {
        public string PlayerId { get; }
        public PlayerLifeState LifeState { get; }
        public bool CanCreateNote { get; }
        public bool IsSpectator { get; }

        public DeadPlayerStateData(string playerId, PlayerLifeState lifeState, bool canCreateNote, bool isSpectator)
        {
            PlayerId = string.IsNullOrWhiteSpace(playerId) ? "unknown_player" : playerId;
            LifeState = lifeState;
            CanCreateNote = canCreateNote;
            IsSpectator = isSpectator;
        }
    }

    public readonly struct DeadVoicePermission
    {
        public bool CanSpeakToDead { get; }
        public bool CanSpeakToLiving { get; }
        public bool CanListenToLivingContext { get; }

        public DeadVoicePermission(bool canSpeakToDead, bool canSpeakToLiving, bool canListenToLivingContext)
        {
            CanSpeakToDead = canSpeakToDead;
            CanSpeakToLiving = canSpeakToLiving;
            CanListenToLivingContext = canListenToLivingContext;
        }
    }

    public readonly struct VictimReconnectSnapshot
    {
        public string PlayerId { get; }
        public PlayerLifeState LifeState { get; }
        public IReadOnlyList<VictimNoteData> VisibleNotes { get; }
        public IReadOnlyList<DeadTaskData> DeadTasks { get; }

        public VictimReconnectSnapshot(
            string playerId,
            PlayerLifeState lifeState,
            IReadOnlyList<VictimNoteData> visibleNotes,
            IReadOnlyList<DeadTaskData> deadTasks)
        {
            PlayerId = string.IsNullOrWhiteSpace(playerId) ? "unknown_player" : playerId;
            LifeState = lifeState;
            VisibleNotes = visibleNotes ?? new List<VictimNoteData>();
            DeadTasks = deadTasks ?? new List<DeadTaskData>();
        }
    }
}
