using System;

namespace OFIS.Core.Ids
{
    [Serializable]
    public readonly struct CorpseId : IEquatable<CorpseId>
    {
        public readonly string Value;

        public CorpseId(string value)
        {
            Value = string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("CorpseId cannot be empty.") : value;
        }

        public static CorpseId New() => new CorpseId($"corpse_{Guid.NewGuid():N}");

        public bool Equals(CorpseId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is CorpseId other && Equals(other);
        public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;
        public override string ToString() => Value;

        public static bool operator ==(CorpseId left, CorpseId right) => left.Equals(right);
        public static bool operator !=(CorpseId left, CorpseId right) => !left.Equals(right);
    }
}