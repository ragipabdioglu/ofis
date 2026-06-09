using System;

namespace OFIS.Core.Ids
{
    [Serializable]
    public readonly struct MeetingId : IEquatable<MeetingId>
    {
        public readonly string Value;

        public MeetingId(string value)
        {
            Value = string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("MeetingId cannot be empty.") : value;
        }

        public static MeetingId First => new MeetingId("meeting_01");
        public static MeetingId Second => new MeetingId("meeting_02");
        public static MeetingId Final => new MeetingId("meeting_final");

        public bool Equals(MeetingId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is MeetingId other && Equals(other);
        public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;
        public override string ToString() => Value;

        public static bool operator ==(MeetingId left, MeetingId right) => left.Equals(right);
        public static bool operator !=(MeetingId left, MeetingId right) => !left.Equals(right);
    }
}