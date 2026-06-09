using System;

namespace OFIS.Core.Ids
{
    [Serializable]
    public readonly struct PlayerId : IEquatable<PlayerId>
    {
        public readonly string Value;

        public PlayerId(string value)
        {
            Value = string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("PlayerId cannot be empty.") : value;
        }

        public static PlayerId New() => new PlayerId($"player_{Guid.NewGuid():N}");

        public bool Equals(PlayerId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is PlayerId other && Equals(other);
        public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;
        public override string ToString() => Value;

        public static bool operator ==(PlayerId left, PlayerId right) => left.Equals(right);
        public static bool operator !=(PlayerId left, PlayerId right) => !left.Equals(right);
    }
}