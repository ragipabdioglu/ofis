using OFIS.Core.Ids;

namespace OFIS.Evidence
{
    public sealed class EvidenceCorpseTraceVisibilityService
    {
        public EvidenceTraceVisibilityType ResolveCorpseMovementVisibility(
            string corpseSourceId,
            string inspectedCorpseSourceId,
            bool hasInspectorKnowledge,
            bool isAnnouncedPublic)
        {
            if (isAnnouncedPublic)
                return EvidenceTraceVisibilityType.Public;

            if (hasInspectorKnowledge && corpseSourceId == inspectedCorpseSourceId)
                return EvidenceTraceVisibilityType.InspectorOnly;

            return EvidenceTraceVisibilityType.Hidden;
        }

        public bool CanInspectorSee(
            EvidenceTraceRecord record,
            PlayerId inspectorPlayerId,
            PlayerId ownerPlayerId,
            string inspectedCorpseSourceId)
        {
            EvidenceTraceVisibilityType visibility = ResolveCorpseMovementVisibility(
                record.SourceId,
                inspectedCorpseSourceId,
                inspectorPlayerId == ownerPlayerId,
                false);

            return visibility == EvidenceTraceVisibilityType.InspectorOnly;
        }
    }
}
