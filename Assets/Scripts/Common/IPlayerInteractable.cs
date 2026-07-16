public interface IPlayerInteractable
{
    public enum InteractionType { DigitalInterface, GiveItem }
    public string GetInteractionTitle();
    public void Interact(PlayerBehaviour player);
    public InteractionType GetInteractionType();
}
