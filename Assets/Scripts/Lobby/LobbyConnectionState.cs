namespace OFIS.Lobby
{
    public enum LobbyConnectionState
    {
        None = 0,
        Connecting = 1,
        Connected = 2,
        Ready = 3,
        LoadingMatch = 4,
        InMatch = 5,
        Disconnected = 6
    }
}
