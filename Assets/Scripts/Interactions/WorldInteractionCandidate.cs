using UnityEngine;

namespace OFIS.Interactions
{
    public readonly struct WorldInteractionCandidate
    {
        public WorldInteractionType Type { get; }
        public string DisplayName { get; }
        public float Distance { get; }
        public bool IsValid { get; }
        public int Priority => WorldInteractionPriority.GetPriority(Type);

        public WorldInteractionCandidate(
            WorldInteractionType type,
            string displayName,
            float distance,
            bool isValid = true)
        {
            Type = type;
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? type.ToString() : displayName;
            Distance = Mathf.Max(0f, distance);
            IsValid = isValid;
        }

        public override string ToString()
        {
            return $"{DisplayName} [{Type}] Priority={Priority}, Distance={Distance:0.00}, IsValid={IsValid}";
        }
    }
}
