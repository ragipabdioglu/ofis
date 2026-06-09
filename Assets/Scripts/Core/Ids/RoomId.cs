using System;

namespace OFIS.Core.Ids
{
    [Serializable]
    public readonly struct RoomId : IEquatable<RoomId>
    {
        public readonly string Value;

        public RoomId(string value)
        {
            Value = string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("RoomId cannot be empty.") : value;
        }

        public bool Equals(RoomId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is RoomId other && Equals(other);
        public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;
        public override string ToString() => Value;

        public static bool operator ==(RoomId left, RoomId right) => left.Equals(right);
        public static bool operator !=(RoomId left, RoomId right) => !left.Equals(right);
    }
}