namespace OFIS.MatchFlow.States
{
    public enum MatchState
    {
        None = 0,

        Lobby = 1,
        LoadingMatch = 2,

        RoleAssignment = 10,
        CharacterAssignment = 11,

        OfficePhase1 = 20,
        Meeting1 = 21,

        OfficePhase2 = 30,
        Meeting2 = 31,

        OfficePhase3 = 40,
        FinalMeeting = 41,

        ResolvingMatch = 90,
        MatchEnded = 100
    }
}