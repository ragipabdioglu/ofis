namespace OFIS.Meetings
{
    public readonly struct MeetingMissingPlayerPenaltyConfig
    {
        public int PenaltyPerMissingPlayer { get; }
        public int MaxPenaltyPerMeeting { get; }
        public bool ApplyPenaltyWhenNoRegisteredPlayers { get; }

        public MeetingMissingPlayerPenaltyConfig(
            int penaltyPerMissingPlayer,
            int maxPenaltyPerMeeting,
            bool applyPenaltyWhenNoRegisteredPlayers = true)
        {
            PenaltyPerMissingPlayer = penaltyPerMissingPlayer < 0 ? 0 : penaltyPerMissingPlayer;
            MaxPenaltyPerMeeting = maxPenaltyPerMeeting < 0 ? 0 : maxPenaltyPerMeeting;
            ApplyPenaltyWhenNoRegisteredPlayers = applyPenaltyWhenNoRegisteredPlayers;
        }

        public static MeetingMissingPlayerPenaltyConfig Default =>
            new MeetingMissingPlayerPenaltyConfig(
                penaltyPerMissingPlayer: 5,
                maxPenaltyPerMeeting: 25,
                applyPenaltyWhenNoRegisteredPlayers: true);

        public override string ToString()
        {
            return $"PenaltyPerMissingPlayer={PenaltyPerMissingPlayer}, MaxPenaltyPerMeeting={MaxPenaltyPerMeeting}, ApplyWhenNoRegistered={ApplyPenaltyWhenNoRegisteredPlayers}";
        }
    }
}
