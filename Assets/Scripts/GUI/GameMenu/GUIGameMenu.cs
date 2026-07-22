using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;

public class GUIGameMenu : MonoBehaviour
{
    #region Variables
    [SerializeField] private PlayerInputBehaviour _playerInput;
    [Tooltip("The whole scene has to be disabled on menu exit so that the gameplay scene is displayed properly")]
    [SerializeField] private Transform _sceneRoot;
    //prevents player from entering and exiting menu too fast
    private float _menuCloseDelay = 0.5f;
    [Header("Top Buttons")]
    [SerializeField] private GUIGameMenuTopSwitchButton _settingsTopButton;
    [SerializeField] private GUIGameMenuTopSwitchButton _systemTopButton;
    [SerializeField] private GUIGameMenuTopSwitchButton _characterAtlasTopButton;
    [SerializeField] private GUIGameMenuTopSwitchButton _abilitiesTopButton;
    private List<GUIGameMenuTopSwitchButton> _topSwitchButtons;
    [Header("Cameras")]
    [Tooltip("Some views have 3D scene to display so that the proper Cinemachine Brain has to be set to a higher priority on the view enter.")]
    [SerializeField] private CinemachineCamera _atlasCinemachineBrain, _characterCinemachineBrain;
    [Tooltip("If the scene doesn't display 3d scene, the camsera would just stare into the void. This cinemachine can't face any object from 3d scenes.")]
    [SerializeField] private CinemachineCamera _emptyCinemachineBrain;
    [Header("MenuTabs")]
    [Tooltip("Parent of all settings view GUI elements placed inside Canvas")]
    [SerializeField] private CanvasGroup _settingsControlsGroup;
    [Tooltip("Parent of all system view GUI elements placed inside Canvas")]
    [SerializeField] private CanvasGroup _systemControlsGroup;
    [Tooltip("Parent of all atlas view GUI elements placed inside Canvas")]
    [SerializeField] private GUIAscensionAtlasControls _atlasControlsGroup;
    [Tooltip("Parent of all abilities view GUI elements placed inside Canvas")]
    [SerializeField] private GUIGameMenuAbilitiesPanelControls _abilitiesControlGroup;
    private List<CanvasGroup> _controlGroups;
    [Header("Settings")]
    [SerializeField] private SettingsManager _settingsManager;
    [SerializeField] private GUISettingsWindow _settingsWindow;
    [SerializeField] private GUISceneBlackFade _blackFade;
    [Header("DisplayPanels")]
    [Tooltip("Mostly things in the to bar, displaying prestige, profile name etc")]
    [SerializeField] private GUIGameMenuStatsPanel _statsPanel;
    [Tooltip("Bottom panel with resources")]
    [SerializeField] private GUIGameMenuResourcesPanel _resourcesPanel;
    #endregion

    #region Mono
    private void Awake()
    {
        SkyforgeLoader.GUIGameMenu = this;
        _playerInput.OpenMenuAction += MenuClose_Clicked;
        //so that the game won't start with menu open
        if(SkyforgeLoader.CurrentProfile != null)
            _ = CloseMenu();
        _settingsTopButton.OnClick += SettingsTopButton_Clicked;
        _systemTopButton.OnClick += SystemTopButton_Clicked;
        _characterAtlasTopButton.OnClick += CharacterAtlasTopButton_Clicked;
        _abilitiesTopButton.OnClick += AbilitiesTopButton_Clicked;
        _topSwitchButtons = new();
        _topSwitchButtons.Add(_settingsTopButton);
        _topSwitchButtons.Add(_systemTopButton);
        _topSwitchButtons.Add(_characterAtlasTopButton);
        _topSwitchButtons.Add(_abilitiesTopButton);
        foreach (var button in _topSwitchButtons)
        {
            button.OnClick += MenuButton_DeselectRest;
        }
        _controlGroups = new();
        _controlGroups.Add(_settingsControlsGroup);
        _controlGroups.Add(_systemControlsGroup);
        _controlGroups.Add(_atlasControlsGroup.GetComponent<CanvasGroup>());
        _controlGroups.Add(_abilitiesControlGroup.GetComponent<CanvasGroup>());
    }
    private void OnDestroy()
    {
        SkyforgeLoader.GUIGameMenu = null;
    }
    private void Update()
    {
        if(_menuCloseDelay>0)
            _menuCloseDelay -= Time.deltaTime;
    }
    #endregion

    #region Methods
    public async Task OpenMenu()
    {
        _sceneRoot.gameObject.SetActive(true);
        _menuCloseDelay = 0.5f;
        _statsPanel.UpdateValues();
        _resourcesPanel.UpdateValues();
        if (SkyforgeLoader.SettingsChanged)
        {
            _settingsManager.ApplySceneSettings();
            _settingsWindow.LoadFromSettings();
        }
        _ = _blackFade.StartFadeOut();
    }
    public async Task CloseMenu()
    {
        await _blackFade.StartFadeIn();
        _sceneRoot.gameObject.SetActive(false);
    }
    public void ShowCharacterAtlasView()
    {
        _atlasCinemachineBrain.Priority = 5;
        _emptyCinemachineBrain.Priority = 1;
        _characterCinemachineBrain.Priority = 1;
        CloseAllControlGroups();
        _atlasControlsGroup.gameObject.SetActive(true);
        _atlasControlsGroup.UpdateClassButton();
        _atlasControlsGroup.SwitchToCharacterAtlas(true);
        _characterAtlasTopButton.SetToggled(true);
        MenuButton_DeselectRest(_characterAtlasTopButton, EventArgs.Empty);
    }
    public void ShowSystemView()
    {
        _atlasCinemachineBrain.Priority = 1;
        _emptyCinemachineBrain.Priority = 5;
        _characterCinemachineBrain.Priority = 1;
        CloseAllControlGroups();
        _systemControlsGroup.gameObject.SetActive(true);
        _systemTopButton.SetToggled(true);
        MenuButton_DeselectRest(_systemTopButton, EventArgs.Empty);
    }
    public void ShowSettingsView()
    {
        _atlasCinemachineBrain.Priority = 1;
        _emptyCinemachineBrain.Priority = 5;
        _characterCinemachineBrain.Priority = 1;
        CloseAllControlGroups();
        _settingsControlsGroup.gameObject.SetActive(true);
        _settingsTopButton.SetToggled(true);
        MenuButton_DeselectRest(_settingsTopButton, EventArgs.Empty);
    }
    private async Task ShowAbilitiesView()
    {
        await _blackFade.StartFadeIn();
        _atlasCinemachineBrain.Priority = 1;
        _emptyCinemachineBrain.Priority = 1;
        _characterCinemachineBrain.Priority = 5;
        CloseAllControlGroups();
        _abilitiesControlGroup.gameObject.SetActive(true);
        await _abilitiesControlGroup.UpdateView(true);
        _abilitiesTopButton.SetToggled(true);
        MenuButton_DeselectRest(_settingsTopButton, EventArgs.Empty);
        _ =  _blackFade.StartFadeOut();
    }
    private void CloseAllControlGroups()
    {
        foreach (var controlGroup in _controlGroups)
        {
            controlGroup.gameObject.SetActive(false);
        }
    }
    #endregion

    #region EventHandlers
    public void MenuClose_Clicked(object sender, EventArgs e)
    {
        if (_sceneRoot.gameObject.activeSelf && _menuCloseDelay <= 0)
            _ = SkyforgeLoader.SetMenuOpen(false);
    }
    private void SettingsTopButton_Clicked(object sender, EventArgs e)
    {
        ShowSettingsView();
    }
    private void SystemTopButton_Clicked(object sender, EventArgs e)
    {
        ShowSystemView();
    }
    private void CharacterAtlasTopButton_Clicked(object sender, EventArgs e)
    {
        ShowCharacterAtlasView();
    }
    private void AbilitiesTopButton_Clicked(object sender, EventArgs e)
    {
        _ = ShowAbilitiesView();
    }
    private void MenuButton_DeselectRest(object sender, EventArgs e)
    {
        foreach (var button in _topSwitchButtons)
        {
            if (button.IsToggled && sender is GUIGameMenuTopSwitchButton && button != (GUIGameMenuTopSwitchButton)sender)
            {
                button.SetToggled(false);
            }
        }
    }
    #endregion
}
