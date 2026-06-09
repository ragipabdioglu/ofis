namespace OFIS.Rooms
{
    public readonly struct OfficeRoomQueryResult
    {
        public bool Found { get; }
        public OfficeRoomType RoomType { get; }
        public string DisplayName { get; }
        public string Reason { get; }

        public OfficeRoomQueryResult(bool found, OfficeRoomType roomType, string displayName, string reason)
        {
            Found = found;
            RoomType = roomType;
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? roomType.ToString() : displayName;
            Reason = string.IsNullOrWhiteSpace(reason) ? "No reason." : reason;
        }

        public static OfficeRoomQueryResult None(string reason)
        {
            return new OfficeRoomQueryResult(false, OfficeRoomType.None, "None", reason);
        }

        public override string ToString()
        {
            return $"Found={Found}, RoomType={RoomType}, DisplayName={DisplayName}, Reason={Reason}";
        }
    }
}
