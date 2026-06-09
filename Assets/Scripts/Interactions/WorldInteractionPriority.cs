namespace OFIS.Interactions
{
    public static class WorldInteractionPriority
    {
        public static int GetPriority(WorldInteractionType type)
        {
            return type switch
            {
                WorldInteractionType.CorpseInspectOrCarry => 700,
                WorldInteractionType.MeetingJoin => 600,
                WorldInteractionType.SabotageRepair => 500,
                WorldInteractionType.Task => 400,
                WorldInteractionType.Sabotage => 300,
                WorldInteractionType.VictimNote => 200,
                WorldInteractionType.DoorPanel => 100,
                _ => 0
            };
        }
    }
}
