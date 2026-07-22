using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GUIProfileViewInterface : MonoBehaviour
{
    #region Variables
    [SerializeField] private GUIProfileCreationView _creationView;
    [SerializeField] private GUIProfilePickView _pickView;
    [Tooltip("Player transform on the scene. It will be used to vizualize profile's character appearance")]
    public PlayerBehaviour PlayerVisualization;
    [Tooltip("Parent transform of the player when the Creation View is active")]
    [SerializeField] private Transform _creationPosition;
    [Tooltip("Parent transform of the player when the Profile Pick View is active")]
    [SerializeField] private Transform _pickPosition;
    [Tooltip("Button at the bottom of the screen, from the child view")]
    [SerializeField] private GUICommonButton _createButtonCancel, _createButtonCreate, _viewButtonBack, _viewButtonPlay;
    [Tooltip("Plus icon that creates a new profile, from the child view")]
    [SerializeField] private Button _createButton;
    [SerializeField] private GUISceneBlackFade _blackFade;
    public List<UserProfile> Profiles;
    [HideInInspector] public int CurrentProfile;
    [HideInInspector] public OutfitManager OutfitManager;
    public MapSO SelectedMap { get; private set; }
    private List<GUIMapSelectionButton> _registeredButtons;
    private bool _fadingProcess, _init, _initTrigger;
    #endregion

    #region Mono
    private void Awake()
    {
        _init = true;
        _initTrigger = true;
        OutfitManager = PlayerVisualization.GetComponent<OutfitManager>();
        _createButtonCancel.OnClick += CreateButtonCancel_Clicked;
        _createButtonCreate.OnClick += CreateButtonCreate_Clicked;
        _viewButtonBack.OnClick += ViewButtonBack_Clicked;
        _viewButtonPlay.OnClick += ViewButtonPlay_Clicked;
        _createButton.onClick.AddListener(CreateButton_Clicked);
        Profiles = SaveManager.LoadAllProfiles();
        foreach(var profile in Profiles)
        {
            UpdateProfileToCurrentVersion(profile);
        }
        if (_registeredButtons == null)
            _registeredButtons = new();
    }
    private void UpdateProfileToCurrentVersion(UserProfile profile)
    {
        if(profile.CurrentlyPickedClass == null || profile.CurrentlyPickedClass.Length == 0)
        {
            profile.CurrentlyPickedClass = "Base_Berserker";
        }
        if(profile.AcquiredPerks.Count == 0)
        {
            profile.AcquiredPerks.Add(new UserProfile.PerkState() { PerkID = "Base_Regular_Immortal", Enabled = true });
            profile.AcquiredPerks.Add(new UserProfile.PerkState() { PerkID = "Base_Berserker", Enabled = true });
            profile.GameplayResources.Credits = 1000;
        }
    }
    private void Update()
    {
        //to change after SceneLoadReady is implemented
        if(_init == true && _initTrigger == true)
        {
            _ = InitActions();
            _initTrigger = false;
        }
        if(SkyforgeLoader.LoadingScreenReady)
        {
            _ = _blackFade.StartFadeOut();
            PlayerVisualization.PlayAnimation("MenuStart", false);
            SkyforgeLoader.LoadingScreenReady = false;
        }
    }
    public void OnDestroy()
    {
        _createButtonCancel.OnClick -= CreateButtonCancel_Clicked;
        _createButtonCreate.OnClick -= CreateButtonCreate_Clicked;
        _viewButtonBack.OnClick -= ViewButtonBack_Clicked;
        _viewButtonPlay.OnClick -= ViewButtonPlay_Clicked;
    }
    #endregion

    #region Methods
    private async Task InitActions()
    {
        PlayerVisualization.SetAnimationState("Menu", true);
        if (Profiles.Count > 0)
        {
            SetCurrentProfile(0);
            await GoToPickView();
        }
        else
            await GoToCreationView();
        SkyforgeLoader.LoadedSceneReady = true;
        _init = false;
    }
    public async Task GoToPickView()
    {
        if(!_init)
            await StartBlackFadeIn();
        _creationView.gameObject.SetActive(false);
        _pickView.gameObject.SetActive(true);
        PlayerVisualization.transform.SetParent(_pickPosition, false);
        await _pickView.SetProfile(SkyforgeLoader.CurrentProfile, true);
        _ = StartBlackFadeOut();
    }
    public async Task GoToCreationView()
    {
        if (!_init)
            await StartBlackFadeIn();
        _creationView.gameObject.SetActive(true);
        _pickView.gameObject.SetActive(false);
        _creationView.Reset();
        PlayerVisualization.transform.SetParent(_creationPosition, false);
        //first setting weapon at hide state so that it doesn't play animation of hiding
        PlayerVisualization.EquippedWeapon.SetWeaponHide();
        PlayerVisualization.ChangeWeaponOutState(false);
        _ = StartBlackFadeOut();
    }
    public async Task GoToMenu()
    {
        await StartBlackFadeIn();
        _ = SkyforgeLoader.LoadScene("ProfileScene", "MainMenuScene");
    }
    public async Task StartGame()
    {
        await StartBlackFadeIn();
        _ = SkyforgeLoader.LoadScene("ProfileScene", SelectedMap);
    }
    public void SelectMap(MapSO map)
    {
        SelectedMap = map;
        _viewButtonPlay.SetLocked(false);
        foreach(var button in _registeredButtons)
        {
            if (button.Map != map)
                button.Deselect();
        }
    }
    public void RegisterMapButton(GUIMapSelectionButton button)
    {
        if (_registeredButtons == null)
            _registeredButtons = new();
        _registeredButtons.Add(button);
    }
    public async Task StartBlackFadeIn()
    {
        _fadingProcess = true;
        await _blackFade.StartFadeIn();
        _fadingProcess = false;
    }
    public async Task StartBlackFadeOut()
    {
        _fadingProcess = true;
        await _blackFade.StartFadeOut();
        _fadingProcess = false;
    }
    public void SetCurrentProfile(int profileNumber)
    {
        CurrentProfile = profileNumber;
        SkyforgeLoader.CurrentProfile = Profiles[CurrentProfile];
    }
    #endregion

    #region EventHandlers
    private void ViewButtonPlay_Clicked(object sender, EventArgs e)
    {
        if (_fadingProcess == false)
            _ = StartGame();
    }
    private void ViewButtonBack_Clicked(object sender, EventArgs e)
    {
        if (_fadingProcess == false)
            _ = GoToMenu();
    }
    private void CreateButtonCancel_Clicked(object sender, EventArgs e)
    {
        if (_fadingProcess == false)
            _ = GoToPickView();
    }
    private void CreateButtonCreate_Clicked(object sender, EventArgs e)
    {
        if (_fadingProcess == false)
        {
            Profiles.Add(_creationView.CreateNewProfile());
            SetCurrentProfile(Profiles.Count - 1);
            SaveManager.SaveProfile(Profiles[CurrentProfile]);
            _ = GoToPickView();
        }
    }
    private void CreateButton_Clicked()
    {
        if (_fadingProcess == false)
            _ = GoToCreationView();
    }
    #endregion
}
