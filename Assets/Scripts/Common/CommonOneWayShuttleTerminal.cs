using UnityEngine;

public class CommonOneWayShuttleTerminal : MonoBehaviour, IPlayerInteractable
{
    #region variables
    public MapSO DestinationScene;
    public string InteractionDisplayName;
    #endregion

    #region Methods
    public string GetInteractionTitle()
    {
        return InteractionDisplayName;
    }
    public IPlayerInteractable.InteractionType GetInteractionType()
    {
        return IPlayerInteractable.InteractionType.DigitalInterface;
    }
    public void Interact(PlayerBehaviour player)
    {
        SkyforgeLoader.SceneTransferMidScenario = true;
        _ = SkyforgeLoader.LoadScene(SkyforgeLoader.CurrentGameplayScene, DestinationScene);
    }
    #endregion
}
