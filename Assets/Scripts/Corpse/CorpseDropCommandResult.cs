using UnityEngine;

namespace OFIS.Corpse
{
    public readonly struct CorpseDropCommandResult
    {
        public bool Success { get; }
        public CorpsePlaceholder DroppedCorpse { get; }
        public Vector3 DropWorldPosition { get; }
        public bool CarryStateCleared { get; }
        public string Message { get; }

        private CorpseDropCommandResult(
            bool success,
            CorpsePlaceholder droppedCorpse,
            Vector3 dropWorldPosition,
            bool carryStateCleared,
            string message)
        {
            Success = success;
            DroppedCorpse = droppedCorpse;
            DropWorldPosition = dropWorldPosition;
            CarryStateCleared = carryStateCleared;
            Message = message;
        }

        public static CorpseDropCommandResult Dropped(
            CorpsePlaceholder corpse,
            Vector3 dropWorldPosition,
            bool carryStateCleared)
        {
            return new CorpseDropCommandResult(
                true,
                corpse,
                dropWorldPosition,
                carryStateCleared,
                "Corpse dropped.");
        }

        public static CorpseDropCommandResult Rejected(string message)
        {
            return new CorpseDropCommandResult(
                false,
                null,
                default,
                false,
                message);
        }

        public override string ToString()
        {
            string corpseName = DroppedCorpse == null ? "none" : DroppedCorpse.VictimName;
            return $"Success={Success}, Corpse={corpseName}, Position={DropWorldPosition}, Cleared={CarryStateCleared}, Message={Message}";
        }
    }
}
