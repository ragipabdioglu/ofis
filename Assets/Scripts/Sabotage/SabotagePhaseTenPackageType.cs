namespace OFIS.Sabotage
{
    public enum SabotagePhaseTenPackageType
    {
        DevicesAndTypes = 0,
        KillerRoleValidation = 1,
        PhysicalRangeValidation = 2,
        CarryCorpseBlocked = 3,
        Cooldown = 4,
        SameTypeActiveLimit = 5,
        SameRoomActiveLimit = 6,
        RepairInteraction = 7,
        RepairSpeedByWorkerCount = 8,
        CompanyEffects = 9,
        TraceAndLogCreation = 10,
        UiAlertSafety = 11,
        NetworkCommandEventFlow = 12,
        PhaseClosure = 13
    }
}
