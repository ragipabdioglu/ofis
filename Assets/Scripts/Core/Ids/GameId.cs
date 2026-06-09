using System;

namespace OFIS.Core.Ids
{
    [Serializable]
    public readonly struct GameId : IEquatable<GameId>
    {
        public readonly string Value;

        public GameId(string value)
        {
            Value = string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("GameId cannot be empty.") : value;
        }

        public static GameId New(string prefix)
        {
            return new GameId($"{prefix}_{Guid.NewGuid():N}");
        }

        public bool Equals(GameId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is GameId other && Equals(other);
        public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;
        public override string ToString() => Value;

        public static bool operator ==(GameId left, GameId right) => left.Equals(right);
        public static bool operator !=(GameId left, GameId right) => !left.Equals(right);
    }
}