using OFIS.MatchFlow.States;

namespace OFIS.MatchFlow
{
    public static class MatchStateExtensions
    {
        public static MatchPhaseType GetPhaseType(this MatchState state)
        {
            return state switch
            {
                MatchState.Lobby => MatchPhaseType.Lobby,
                MatchState.LoadingMatch => MatchPhaseType.Setup,
                MatchState.RoleAssignment => MatchPhaseType.Setup,
                MatchState.CharacterAssignment => MatchPhaseType.Setup,

                MatchState.OfficePhase1 => MatchPhaseType.Office,
                MatchState.OfficePhase2 => MatchPhaseType.Office,
                MatchState.OfficePhase3 => MatchPhaseType.Office,

                MatchState.Meeting1 => MatchPhaseType.Meeting,
                MatchState.Meeting2 => MatchPhaseType.Meeting,

                MatchState.FinalMeeting => MatchPhaseType.FinalMeeting,

                MatchState.ResolvingMatch => MatchPhaseType.Resolution,
                MatchState.MatchEnded => MatchPhaseType.Ended,

                _ => MatchPhaseType.None
            };
        }

        public static bool IsOfficePhase(this MatchState state)
        {
            return state is MatchState.OfficePhase1 or MatchState.OfficePhase2 or MatchState.OfficePhase3;
        }

        public static bool IsMeetingPhase(this MatchState state)
        {
            return state is MatchState.Meeting1 or MatchState.Meeting2 or MatchState.FinalMeeting;
        }

        public static bool IsFinalMeeting(this MatchState state)
        {
            return state == MatchState.FinalMeeting;
        }

        public static bool IsMatchRunning(this MatchState state)
        {
            return state is
                MatchState.OfficePhase1 or
                MatchState.Meeting1 or
                MatchState.OfficePhase2 or
                MatchState.Meeting2 or
                MatchState.OfficePhase3 or
                MatchState.FinalMeeting;
        }
    }
}