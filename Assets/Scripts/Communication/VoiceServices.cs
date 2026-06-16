using OFIS.Players;
using UnityEngine;

namespace OFIS.Communication
{
    public interface IVoiceProviderAdapter
    {
        VoiceProviderState Connect(string playerId);
    }

    public sealed class MockVoiceProviderAdapter : IVoiceProviderAdapter
    {
        public VoiceProviderState Connect(string playerId)
        {
            bool connected = !string.IsNullOrWhiteSpace(playerId);
            return new VoiceProviderState(connected, "MockVoice");
        }
    }

    public sealed class ProximityVoiceService
    {
        public const float FullVolumeRangeMeters = 4.5f;
        public const float FadeEndRangeMeters = 6f;
        public const float HardCutRangeMeters = 6.5f;

        public VoiceRouteResult Evaluate(float distanceMeters)
        {
            if (distanceMeters > HardCutRangeMeters)
                return new VoiceRouteResult(VoiceChannelType.Proximity, false, false, 0f, "Outside hard cut range.");

            if (distanceMeters <= FullVolumeRangeMeters)
                return new VoiceRouteResult(VoiceChannelType.Proximity, true, true, 1f, "Inside full voice range.");

            if (distanceMeters <= FadeEndRangeMeters)
            {
                float fade = 1f - ((distanceMeters - FullVolumeRangeMeters) / (FadeEndRangeMeters - FullVolumeRangeMeters));
                return new VoiceRouteResult(VoiceChannelType.Proximity, true, true, fade, "Inside proximity fade range.");
            }

            return new VoiceRouteResult(VoiceChannelType.Proximity, true, true, 0.1f, "Near hard cut edge.");
        }
    }

    public sealed class RoomAcousticPenaltyService
    {
        public float ApplyPenalty(float baseVolume, bool sameRoom)
        {
            return sameRoom ? baseVolume : Mathf.Clamp01(baseVolume * 0.55f);
        }
    }

    public sealed class MeetingVoiceRouteService
    {
        public VoiceRouteResult Evaluate(
            bool isRegisteredParticipant,
            bool isInsideMeetingRoom,
            bool isAlive,
            bool isLateObserver)
        {
            if (!isRegisteredParticipant || !isAlive || isLateObserver)
                return new VoiceRouteResult(VoiceChannelType.Meeting, false, false, 0f, "Meeting voice blocked.");

            if (!isInsideMeetingRoom)
                return new VoiceRouteResult(VoiceChannelType.Meeting, false, false, 0f, "Outside meeting room.");

            return new VoiceRouteResult(VoiceChannelType.Meeting, true, true, 1f, "Meeting voice active.");
        }
    }

    public sealed class DeadVoiceRouteService
    {
        public VoiceRouteResult Evaluate(PlayerLifeState speakerLifeState, PlayerLifeState listenerLifeState)
        {
            bool speakerDead = speakerLifeState == PlayerLifeState.Dead;
            bool listenerDead = listenerLifeState == PlayerLifeState.Dead;

            if (speakerDead && listenerDead)
                return new VoiceRouteResult(VoiceChannelType.Dead, true, true, 1f, "Dead voice active.");

            if (speakerDead && !listenerDead)
                return new VoiceRouteResult(VoiceChannelType.Dead, false, false, 0f, "Dead cannot speak to living.");

            return new VoiceRouteResult(VoiceChannelType.None, false, false, 0f, "Dead voice not applicable.");
        }
    }

    public sealed class ExposedKillerVoiceRouteService
    {
        public VoiceChannelType ResolveChannel(bool isExposedKiller)
        {
            return isExposedKiller ? VoiceChannelType.Dead : VoiceChannelType.Proximity;
        }
    }

    public sealed class PushToTalkService
    {
        public string DefaultKey => "V";
    }

    public sealed class VoiceLocalSettingsService
    {
        public VoiceLocalSettings Build(bool muted, bool deafened, string pushToTalkKey)
        {
            return new VoiceLocalSettings(muted, deafened, pushToTalkKey);
        }

        public VoiceRouteResult ApplyLocalSettings(VoiceRouteResult route, VoiceLocalSettings settings)
        {
            return new VoiceRouteResult(
                route.ChannelType,
                route.CanTransmit && !settings.IsMuted,
                route.CanReceive && !settings.IsDeafened,
                settings.IsDeafened ? 0f : route.Volume,
                route.Reason);
        }
    }

    public sealed class VoiceUiStatusService
    {
        public VoiceUiStatus Build(VoiceRouteResult route, VoiceLocalSettings settings)
        {
            string label = $"{route.ChannelType} voice";
            return new VoiceUiStatus(route.ChannelType, route.CanTransmit, settings.IsMuted, settings.IsDeafened, label);
        }
    }

    public sealed class VoiceReconnectRestoreService
    {
        public VoiceReconnectSnapshot Restore(string playerId, PlayerLifeState lifeState, VoiceLocalSettings settings)
        {
            VoiceChannelType channel = lifeState == PlayerLifeState.Dead ? VoiceChannelType.Dead : VoiceChannelType.Proximity;
            return new VoiceReconnectSnapshot(playerId, lifeState, channel, settings);
        }
    }
}
