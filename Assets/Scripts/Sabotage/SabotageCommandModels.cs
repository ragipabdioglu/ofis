using OFIS.Core.Ids;
using OFIS.Roles;
using UnityEngine;

namespace OFIS.Sabotage
{
    public readonly struct SabotageCommand
    {
        public PlayerId SenderPlayerId { get; }
        public PlayerRole SenderRole { get; }
        public SabotageDeviceDefinition Device { get; }
        public Vector3 PlayerPosition { get; }
        public bool IsCarryingCorpse { get; }
        public float ServerTimeSeconds { get; }

        public SabotageCommand(
            PlayerId senderPlayerId,
            PlayerRole senderRole,
            SabotageDeviceDefinition device,
            Vector3 playerPosition,
            bool isCarryingCorpse,
            float serverTimeSeconds)
        {
            SenderPlayerId = senderPlayerId;
            SenderRole = senderRole;
            Device = device;
            PlayerPosition = playerPosition;
            IsCarryingCorpse = isCarryingCorpse;
            ServerTimeSeconds = serverTimeSeconds < 0f ? 0f : serverTimeSeconds;
        }
    }

    public readonly struct SabotageCommandResult
    {
        public bool Success { get; }
        public string Message { get; }
        public SabotageObjectiveRuntimeState RuntimeState { get; }

        public SabotageCommandResult(bool success, string message, SabotageObjectiveRuntimeState runtimeState)
        {
            Success = success;
            Message = string.IsNullOrWhiteSpace(message) ? "No sabotage result message." : message;
            RuntimeState = runtimeState;
        }
    }
}
