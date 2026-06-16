using System.Collections.Generic;
using OFIS.Core.Ids;

namespace OFIS.Corpse
{
    public sealed class CorpseMovementTraceMemory
    {
        private readonly List<CorpseMovementTraceEvent> _events = new List<CorpseMovementTraceEvent>();

        public IReadOnlyList<CorpseMovementTraceEvent> Events => _events;
        public int Count => _events.Count;

        public void Record(CorpseMovementTraceEvent traceEvent)
        {
            if (string.IsNullOrWhiteSpace(traceEvent.TraceId.Value))
                return;

            _events.Add(traceEvent);
        }

        public IReadOnlyList<CorpseMovementTraceEvent> GetEventsForCorpse(CorpseId corpseId)
        {
            List<CorpseMovementTraceEvent> result = new List<CorpseMovementTraceEvent>();

            for (int i = 0; i < _events.Count; i++)
            {
                if (_events[i].CorpseId == corpseId)
                    result.Add(_events[i]);
            }

            return result;
        }

        public void Clear()
        {
            _events.Clear();
        }
    }
}
