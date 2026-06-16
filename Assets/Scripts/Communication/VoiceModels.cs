using OFIS.Players;

namespace OFIS.Communication
{
    public enum VoiceChannelType
    {
        None = 0,
        Proximity = 1,
        Meeting = 2,
        Dead = 3
    }

    public readonly struct VoiceProviderState
    {
        public bool IsConnected { get; }
        public string ProviderName { get; }

        public VoiceProviderState(bool isConnected, string providerName)
        {
            IsConnected = isConnected;
            ProviderName = string.IsNullOrWhiteSpace(providerName) ? "unknown_voice_provider" : providerName;
        }
    }

    public readonly struct VoiceRouteResult
    {
        public VoiceChannelType ChannelType { get; }
        public bool CanTransmit { get; }
        public bool CanReceive { get; }
        public float Volume { get; }
        public string Reason { get; }

        public VoiceRouteResult(VoiceChannelType channelType, bool canTransmit, bool canReceive, float volume, string reason)
        {
            ChannelType = channelType;
            CanTransmit = canTransmit;
            CanReceive = canReceive;
            Volume = volume < 0f ? 0f : volume;
            Reason = string.IsNullOrWhiteSpace(reason) ? "Voice route resolved." : reason;
        }
    }

    public readonly struct VoiceLocalSettings
    {
        public bool IsMuted { get; }
        public bool IsDeafened { get; }
        public string PushToTalkKey { get; }

        public VoiceLocalSettings(bool isMuted, bool isDeafened, string pushToTalkKey)
        {
            IsMuted = isMuted;
            IsDeafened = isDeafened;
            PushToTalkKey = string.IsNullOrWhiteSpace(pushToTalkKey) ? "V" : pushToTalkKey;
        }
    }

    public readonly struct VoiceUiStatus
    {
        public VoiceChannelType ChannelType { get; }
        public bool IsSpeaking { get; }
        public bool IsMuted { get; }
        public bool IsDeafened { get; }
        public string Label { get; }

        public VoiceUiStatus(VoiceChannelType channelType, bool isSpeaking, bool isMuted, bool isDeafened, string label)
        {
            ChannelType = channelType;
            IsSpeaking = isSpeaking;
            IsMuted = isMuted;
            IsDeafened = isDeafened;
            Label = string.IsNullOrWhiteSpace(label) ? "Voice" : label;
        }
    }

    public readonly struct VoiceReconnectSnapshot
    {
        public string PlayerId { get; }
        public PlayerLifeState LifeState { get; }
        public VoiceChannelType RestoredChannel { get; }
        public VoiceLocalSettings LocalSettings { get; }

        public VoiceReconnectSnapshot(
            string playerId,
            PlayerLifeState lifeState,
            VoiceChannelType restoredChannel,
            VoiceLocalSettings localSettings)
        {
            PlayerId = string.IsNullOrWhiteSpace(playerId) ? "unknown_player" : playerId;
            LifeState = lifeState;
            RestoredChannel = restoredChannel;
            LocalSettings = localSettings;
        }
    }
}
