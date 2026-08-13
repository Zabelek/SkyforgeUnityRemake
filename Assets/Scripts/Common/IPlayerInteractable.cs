public interface IPlayerInteractable
{
    public enum InteractionType { DigitalInterface, GiveItem, OpenClose }
    public string GetInteractionTitle();
    public void Interact(PlayerBehaviour player);
    public InteractionType GetInteractionType();
}
