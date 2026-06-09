using System;

namespace OFIS.Core.Ids
{
    [Serializable]
    public readonly struct EvidenceTraceId : IEquatable<EvidenceTraceId>
    {
        public readonly string Value;

        public EvidenceTraceId(string value)
        {
            Value = string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("EvidenceTraceId cannot be empty.") : value;
        }

        public static EvidenceTraceId New() => new EvidenceTraceId($"trace_{Guid.NewGuid():N}");

        public bool Equals(EvidenceTraceId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is EvidenceTraceId other && Equals(other);
        public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;
        public override string ToString() => Value;

        public static bool operator ==(EvidenceTraceId left, EvidenceTraceId right) => left.Equals(right);
        public static bool operator !=(EvidenceTraceId left, EvidenceTraceId right) => !left.Equals(right);
    }
}