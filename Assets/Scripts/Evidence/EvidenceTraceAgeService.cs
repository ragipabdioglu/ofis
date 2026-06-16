namespace OFIS.Evidence
{
    public readonly struct EvidenceTraceAgeResult
    {
        public EvidenceTraceAgeCategory Category { get; }
        public float AgeSeconds { get; }
        public string DisplayText { get; }

        public EvidenceTraceAgeResult(
            EvidenceTraceAgeCategory category,
            float ageSeconds,
            string displayText)
        {
            Category = category;
            AgeSeconds = ageSeconds < 0f ? 0f : ageSeconds;
            DisplayText = string.IsNullOrWhiteSpace(displayText) ? "Unknown age" : displayText;
        }

        public override string ToString()
        {
            return $"Category={Category}, Age={AgeSeconds:0.##}, Display={DisplayText}";
        }
    }

    public sealed class EvidenceTraceAgeService
    {
        public EvidenceTraceAgeResult CalculateAge(
            EvidenceTraceRecord traceRecord,
            float currentServerTimeSeconds)
        {
            float ageSeconds = currentServerTimeSeconds - traceRecord.ServerTimeSeconds;

            if (ageSeconds < 0f)
                ageSeconds = 0f;

            if (ageSeconds <= 60f)
                return new EvidenceTraceAgeResult(EvidenceTraceAgeCategory.Fresh, ageSeconds, "Fresh trace");

            if (ageSeconds <= 180f)
                return new EvidenceTraceAgeResult(EvidenceTraceAgeCategory.Old, ageSeconds, "Old trace");

            return new EvidenceTraceAgeResult(EvidenceTraceAgeCategory.VeryOld, ageSeconds, "Very old trace");
        }
    }
}
