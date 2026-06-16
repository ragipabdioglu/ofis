namespace OFIS.Evidence
{
    public readonly struct EvidenceTraceVisibilityResult
    {
        public bool CanView { get; }
        public EvidenceTraceVisibilityType VisibilityType { get; }
        public EvidenceTraceRecord TraceRecord { get; }
        public string Message { get; }

        public EvidenceTraceVisibilityResult(
            bool canView,
            EvidenceTraceVisibilityType visibilityType,
            EvidenceTraceRecord traceRecord,
            string message)
        {
            CanView = canView;
            VisibilityType = visibilityType;
            TraceRecord = traceRecord;
            Message = string.IsNullOrWhiteSpace(message) ? "No visibility message." : message;
        }
    }
}
