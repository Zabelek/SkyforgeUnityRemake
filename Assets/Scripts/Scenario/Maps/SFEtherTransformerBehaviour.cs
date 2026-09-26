using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

public class SFEtherTransformerBehaviour : MonoBehaviour, IPlayerInteractable
{
    #region Variables
    [Header("Interface Related Variables")]
    [Tooltip("Camera that is going to be moved towards the machine on interaction")]
    [SerializeField] private CinemachineCamera _interfacecamera;
    [Tooltip("Gameplay controls instance of the scene (should be only one)")]
    [SerializeField] private GUIGameplayControls _gameplayControls;
    [Tooltip("Parent of the menu referenced below. It's needed so that it can return to its initial position after the interaction is over")]
    [SerializeField] private RectTransform _menuParent;
    [Tooltip("Menu that will be displayed when the machine is interacted with")]
    [SerializeField] private RectTransform _menu;
    [Tooltip("Default scene black fade object")]
    [SerializeField] private GUISceneBlackFade _black;
    private bool alreadyFading = false;
    #endregion

    #region Mono
    private void Awake()
    {
        _menu.gameObject.SetActive(false);
    }
    #endregion

    #region Methods
    public string GetInteractionTitle()
    {
        return "Activate Ether Transformer";
    }
    public IPlayerInteractable.InteractionType GetInteractionType()
    {
        return IPlayerInteractable.InteractionType.DigitalInterface;
    }
    public void Interact(PlayerBehaviour player)
    {
        _ = ScheduleInterfaceTransition();
    }
    public async Task ScheduleInterfaceTransition()
    {
        if (!alreadyFading)
        {
            alreadyFading = true;
            await _black.StartFadeIn();
            _gameplayControls.SetSpecialWorldInterface(_menu, _menuParent, _interfacecamera);
            await _black.StartFadeOut();
            alreadyFading = false;
        }
    }
    #endregion
}
