namespace OFIS.Interaction
{
    public interface IInteractable
    {
        string DisplayName { get; }
        InteractionType InteractionType { get; }
        bool CanInteract { get; }

        void Interact(InteractionContext context);
    }
}