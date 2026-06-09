using System;

namespace OFIS.Core.Ids
{
    [Serializable]
    public readonly struct MatchId : IEquatable<MatchId>
    {
        public readonly string Value;

        public MatchId(string value)
        {
            Value = string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("MatchId cannot be empty.") : value;
        }

        public static MatchId New() => new MatchId($"match_{Guid.NewGuid():N}");

        public bool Equals(MatchId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is MatchId other && Equals(other);
        public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;
        public override string ToString() => Value;

        public static bool operator ==(MatchId left, MatchId right) => left.Equals(right);
        public static bool operator !=(MatchId left, MatchId right) => !left.Equals(right);
    }
}