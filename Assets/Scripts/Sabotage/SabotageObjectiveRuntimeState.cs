namespace OFIS.Sabotage
{
    public sealed class SabotageObjectiveRuntimeState
    {
        public SabotageObjectiveDefinition Definition { get; }
        public SabotageObjectiveState State { get; private set; }

        public bool IsActive => State == SabotageObjectiveState.Active || State == SabotageObjectiveState.Repairing;
        public bool IsRepaired => State == SabotageObjectiveState.Repaired;

        public SabotageObjectiveRuntimeState(SabotageObjectiveDefinition definition, SabotageObjectiveState initialState = SabotageObjectiveState.Inactive)
        {
            Definition = definition;
            State = initialState == SabotageObjectiveState.None ? SabotageObjectiveState.Inactive : initialState;
        }

        public void Activate()
        {
            if (State == SabotageObjectiveState.Repaired)
                return;

            State = SabotageObjectiveState.Active;
        }

        public void MarkRepairing()
        {
            if (State != SabotageObjectiveState.Active)
                return;

            State = SabotageObjectiveState.Repairing;
        }

        public void MarkRepaired()
        {
            State = SabotageObjectiveState.Repaired;
        }

        public void MarkExpired()
        {
            if (State == SabotageObjectiveState.Repaired)
                return;

            State = SabotageObjectiveState.Expired;
        }

        public override string ToString()
        {
            return $"Sabotage={Definition.DisplayName}, State={State}, IsActive={IsActive}, IsRepaired={IsRepaired}";
        }
    }
}
