using System.Collections.Generic;
using OFIS.Core.Ids;

namespace OFIS.Kill
{
    public sealed class KillCooldownState
    {
        private readonly Dictionary<PlayerId, float> _lastAcceptedKillTimes = new Dictionary<PlayerId, float>();

        public bool TryGetLastAcceptedKillTime(PlayerId playerId, out float lastAcceptedKillTimeSeconds)
        {
            return _lastAcceptedKillTimes.TryGetValue(playerId, out lastAcceptedKillTimeSeconds);
        }

        public void RecordAcceptedKill(PlayerId playerId, float serverTimeSeconds)
        {
            _lastAcceptedKillTimes[playerId] = serverTimeSeconds;
        }

        public void Clear()
        {
            _lastAcceptedKillTimes.Clear();
        }
    }
}
