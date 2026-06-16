using System.Collections.Generic;

namespace OFIS.Meetings
{
    public sealed class MeetingDetectiveContradictionResult
    {
        private readonly List<MeetingDetectiveContradictionEvent> _events;

        public IReadOnlyList<MeetingDetectiveContradictionEvent> Events => _events;
        public int EventCount => _events.Count;
        public bool HasContradictions => EventCount > 0;
        public string Message { get; }

        public MeetingDetectiveContradictionResult(
            IEnumerable<MeetingDetectiveContradictionEvent> events,
            string message)
        {
            _events = new List<MeetingDetectiveContradictionEvent>();

            if (events != null)
            {
                foreach (MeetingDetectiveContradictionEvent contradictionEvent in events)
                    _events.Add(contradictionEvent);
            }

            Message = string.IsNullOrWhiteSpace(message)
                ? "Detective contradiction evaluation completed."
                : message;
        }

        public override string ToString()
        {
            return $"Contradictions={EventCount}, Message={Message}";
        }
    }
}
