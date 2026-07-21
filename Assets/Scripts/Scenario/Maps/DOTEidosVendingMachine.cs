using UnityEngine;

public class DOTEidosVendingMachine : MonoBehaviour, IPlayerInteractable
{
    #region Methods
    public string GetInteractionTitle()
    {
        return "Buy Eidos";
    }
    public IPlayerInteractable.InteractionType GetInteractionType()
    {
        return IPlayerInteractable.InteractionType.DigitalInterface;
    }
    public void Interact(PlayerBehaviour player)
    {
        if(SkyforgeLoader.CurrentProfile != null)
        {
            if(SkyforgeLoader.CurrentProfile.GameplayResources.Credits >= 5)
            {
                SkyforgeLoader.CurrentProfile.GameplayResources.Credits -= 5;
                SkyforgeLoader.CurrentProfile.GameplayResources.AelionEidoses += 1;
            }
        }
    }
    #endregion
}
