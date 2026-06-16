using OFIS.Core.Ids;
using OFIS.Meetings;
using OFIS.Players;
using OFIS.Roles;
using OFIS.Rooms;

namespace OFIS.Kill
{
    public readonly struct KillCommandContext
    {
        public string CommandId { get; }
        public PlayerId SenderId { get; }
        public PlayerId TargetId { get; }
        public PlayerRole SenderRole { get; }
        public PlayerRole TargetRole { get; }
        public PlayerLifeState TargetLifeState { get; }
        public bool TargetIsKnownVictim { get; }
        public float DistanceToTarget { get; }
        public float MaxKillRange { get; }
        public float ServerTimeSeconds { get; }
        public float LastAcceptedKillTimeSeconds { get; }
        public float CooldownSeconds { get; }
        public MeetingRuntimePhaseType PhaseType { get; }
        public OfficeRoomType SenderRoom { get; }
        public bool SenderIsCarryingCorpse { get; }

        public KillCommandContext(
            string commandId,
            PlayerId senderId,
            PlayerId targetId,
            PlayerRole senderRole,
            PlayerRole targetRole,
            PlayerLifeState targetLifeState,
            bool targetIsKnownVictim,
            float distanceToTarget,
            float maxKillRange,
            float serverTimeSeconds,
            float lastAcceptedKillTimeSeconds,
            float cooldownSeconds,
            MeetingRuntimePhaseType phaseType,
            OfficeRoomType senderRoom,
            bool senderIsCarryingCorpse)
        {
            CommandId = commandId;
            SenderId = senderId;
            TargetId = targetId;
            SenderRole = senderRole;
            TargetRole = targetRole;
            TargetLifeState = targetLifeState;
            TargetIsKnownVictim = targetIsKnownVictim;
            DistanceToTarget = distanceToTarget;
            MaxKillRange = maxKillRange;
            ServerTimeSeconds = serverTimeSeconds;
            LastAcceptedKillTimeSeconds = lastAcceptedKillTimeSeconds;
            CooldownSeconds = cooldownSeconds;
            PhaseType = phaseType;
            SenderRoom = senderRoom;
            SenderIsCarryingCorpse = senderIsCarryingCorpse;
        }
    }
}
