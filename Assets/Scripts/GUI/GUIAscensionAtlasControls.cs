using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIAscensionAtlasControls : MonoBehaviour
{
    private enum DisplayAtlasState { None, Character, Class }
    #region Variables
    [Tooltip("Object in the scene that contains all atlases")]
    [SerializeField] private AscensionAtlasScene _atlasScene;
    [Tooltip("Buttons in the bottom-left side of the screen that switch between atlasts")]
    [SerializeField] private Button _characterAtlasButton, _classAtlasButton;
    [Tooltip("The same that is references in the game menu")]
    [SerializeField] private GUISceneBlackFade _blackFade;
    [Tooltip("Text box with the name of the current class")]
    [SerializeField] private TextMeshProUGUI _classButtonText;
    [Tooltip("Image with the icon of the current class")]
    [SerializeField] private Image _classButtonImage;
    //set so that the view can switch for just one atlas at a time, preventing multi-clicks
    private bool _isSwitching;
    //telling what atlas is now displayed
    private DisplayAtlasState _displayAtlasState;
    #endregion

    #region Mono
    private void Awake()
    {
        _displayAtlasState = DisplayAtlasState.None;
        _characterAtlasButton.onClick.AddListener(SwitchToCharacterAtlas);
        _classAtlasButton.onClick.AddListener(SwitchToClassAtlas);
    }
    #endregion

    #region Methods
    public void SwitchToClassAtlas()
    {
        if(!_isSwitching && _displayAtlasState != DisplayAtlasState.Class)
            _ = StartSwitchingToClassAtlas();
        else if(!_isSwitching)
            _atlasScene.SetCameraToCenter();
    }
    private async Task StartSwitchingToClassAtlas()
    {
        _isSwitching = true;
        await _blackFade.StartFadeIn();
        _atlasScene.ShowClassAtlas();
        _displayAtlasState = DisplayAtlasState.Class;
        await _blackFade.StartFadeOut();
        _isSwitching = false;
    }
    public void SwitchToCharacterAtlas(bool init)
    {
            if (!_isSwitching && _displayAtlasState != DisplayAtlasState.Character)
                _ = StartSwitchingToCharacterAtlas(init);
            else if (!_isSwitching)
                _atlasScene.SetCameraToCenter();
    }
    public void SwitchToCharacterAtlas()
    {
        SwitchToCharacterAtlas(false);
    }
    private async Task StartSwitchingToCharacterAtlas(bool init)
    {
        _isSwitching = true;
        if(!init)
            await _blackFade.StartFadeIn();
        _atlasScene.ShowCharacterAtlas();
        _displayAtlasState = DisplayAtlasState.Character;
        await _blackFade.StartFadeOut();
        _isSwitching = false;
    }
    public void UpdateClassButton()
    {
        if (SkyforgeLoader.CurrentProfile != null && SkyforgeLoader.ClassRegistry != null)
        {
            var classToOverwrite = SkyforgeLoader.ClassRegistry.RegisteredClasses.FirstOrDefault(c => c.ID == SkyforgeLoader.CurrentProfile.CurrentlyPickedClass);
            if (classToOverwrite != null)
            {
                _classButtonText.text = classToOverwrite.Name;
                _classButtonImage.sprite = classToOverwrite.SimplifiedIcon;
            }
        }
    }
    #endregion
}
