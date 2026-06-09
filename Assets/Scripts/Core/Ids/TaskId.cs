using System;

namespace OFIS.Core.Ids
{
    [Serializable]
    public readonly struct TaskId : IEquatable<TaskId>
    {
        public readonly string Value;

        public TaskId(string value)
        {
            Value = string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("TaskId cannot be empty.") : value;
        }

        public static TaskId New() => new TaskId($"task_{Guid.NewGuid():N}");

        public bool Equals(TaskId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is TaskId other && Equals(other);
        public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;
        public override string ToString() => Value;

        public static bool operator ==(TaskId left, TaskId right) => left.Equals(right);
        public static bool operator !=(TaskId left, TaskId right) => !left.Equals(right);
    }
}