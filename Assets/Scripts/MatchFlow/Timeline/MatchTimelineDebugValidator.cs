using System.Text;
using OFIS.MatchFlow.States;
using UnityEngine;

namespace OFIS.MatchFlow.Timeline
{
    public sealed class MatchTimelineDebugValidator : MonoBehaviour
    {
        [SerializeField] private bool validateOnStart = true;
        [SerializeField] private bool logDebugTimeline = false;

        private void Start()
        {
            if (!validateOnStart)
                return;

            ValidateProductionTimeline();

            if (logDebugTimeline)
                ValidateFastDebugTimeline();
        }

        [ContextMenu("Validate Production Timeline")]
        public void ValidateProductionTimeline()
        {
            MatchTimelineDefinition timeline = MatchTimelineFactory.CreateProductionTimeline();
            bool totalDurationMatches = Mathf.Approximately(
                timeline.TotalDurationSeconds,
                MatchTimelineFactory.ProductionTotalDurationSeconds);

            StringBuilder builder = new();
            builder.AppendLine("[MatchTimeline] Production timeline validation");
            builder.AppendLine($"TotalDurationSeconds: {timeline.TotalDurationSeconds}");
            builder.AppendLine($"ExpectedTotalDurationSeconds: {MatchTimelineFactory.ProductionTotalDurationSeconds}");
            builder.AppendLine($"TotalDurationMatches: {totalDurationMatches}");
            builder.AppendLine($"0s -> {timeline.GetStateAtTime(0f)}");
            builder.AppendLine($"239s -> {timeline.GetStateAtTime(239f)}");
            builder.AppendLine($"240s -> {timeline.GetStateAtTime(240f)}");
            builder.AppendLine($"359s -> {timeline.GetStateAtTime(359f)}");
            builder.AppendLine($"360s -> {timeline.GetStateAtTime(360f)}");
            builder.AppendLine($"599s -> {timeline.GetStateAtTime(599f)}");
            builder.AppendLine($"600s -> {timeline.GetStateAtTime(600f)}");
            builder.AppendLine($"719s -> {timeline.GetStateAtTime(719f)}");
            builder.AppendLine($"720s -> {timeline.GetStateAtTime(720f)}");
            builder.AppendLine($"959s -> {timeline.GetStateAtTime(959f)}");
            builder.AppendLine($"960s -> {timeline.GetStateAtTime(960f)}");
            builder.AppendLine($"1079s -> {timeline.GetStateAtTime(1079f)}");
            builder.AppendLine($"1080s -> {timeline.GetStateAtTime(1080f)}");

            if (!totalDurationMatches)
                Debug.LogError(builder.ToString());
            else
                Debug.Log(builder.ToString());
        }

        [ContextMenu("Validate Fast Debug Timeline")]
        public void ValidateFastDebugTimeline()
        {
            MatchTimelineDefinition timeline = MatchTimelineFactory.CreateFastDebugTimeline();

            StringBuilder builder = new();
            builder.AppendLine("[MatchTimeline] Fast debug timeline validation");
            builder.AppendLine($"TotalDurationSeconds: {timeline.TotalDurationSeconds}");

            float cursor = 0f;
            for (int i = 0; i < timeline.Segments.Count; i++)
            {
                MatchTimelineSegment segment = timeline.Segments[i];
                builder.AppendLine($"{cursor:0.##}s -> {segment.State} ({segment.DurationSeconds:0.##}s)");
                cursor += segment.DurationSeconds;
            }

            builder.AppendLine($"End -> {timeline.GetStateAtTime(timeline.TotalDurationSeconds)}");
            Debug.Log(builder.ToString());
        }
    }
}
