using System.Collections.Generic;
using OFIS.Core.Ids;

namespace OFIS.Evidence
{
    public sealed class EvidenceTraceRegistry
    {
        private readonly List<EvidenceTraceRecord> _records = new List<EvidenceTraceRecord>();

        public IReadOnlyList<EvidenceTraceRecord> Records => _records;
        public int Count => _records.Count;

        public bool TryRecord(EvidenceTraceRecord record)
        {
            if (string.IsNullOrWhiteSpace(record.TraceId.Value))
                return false;

            if (record.TraceType == EvidenceTraceType.None)
                return false;

            _records.Add(record);
            return true;
        }

        public IReadOnlyList<EvidenceTraceRecord> GetBySource(string sourceId)
        {
            List<EvidenceTraceRecord> result = new List<EvidenceTraceRecord>();

            for (int i = 0; i < _records.Count; i++)
            {
                if (_records[i].SourceId == sourceId)
                    result.Add(_records[i]);
            }

            return result;
        }

        public bool Contains(EvidenceTraceId traceId)
        {
            for (int i = 0; i < _records.Count; i++)
            {
                if (_records[i].TraceId == traceId)
                    return true;
            }

            return false;
        }
    }
}
