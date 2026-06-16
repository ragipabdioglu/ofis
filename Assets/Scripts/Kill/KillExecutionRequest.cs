using OFIS.Core.Ids;
using UnityEngine;

namespace OFIS.Kill
{
    public readonly struct KillExecutionRequest
    {
        public KillCommandContext CommandContext { get; }
        public CorpseId CorpseId { get; }
        public string VictimDisplayName { get; }
        public Vector3 DeathPosition { get; }

        public KillExecutionRequest(
            KillCommandContext commandContext,
            CorpseId corpseId,
            string victimDisplayName,
            Vector3 deathPosition)
        {
            CommandContext = commandContext;
            CorpseId = corpseId;
            VictimDisplayName = victimDisplayName;
            DeathPosition = deathPosition;
        }
    }
}
