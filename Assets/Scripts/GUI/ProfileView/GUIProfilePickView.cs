using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIProfilePickView : MonoBehaviour
{
    #region Variables
    [Tooltip("Child text boxes to display profile details")]
    [SerializeField] private TextMeshProUGUI _nameTextBox, _difficultyTextBox, _PrestigeTextBox;
    [Tooltip("Buttons used to switch between profiles")]
    [SerializeField] private Button _leftButton, _rightButton;
    [Tooltip("Parent component that manages the scene")]
    [SerializeField] GUIProfileViewInterface _profileView;
    [Tooltip("All maps available from here to pick and start as a game session")]
    [SerializeField] MapSO[] AvailableMaps;
    [Tooltip("Base prefab to instantiate buttons for each map")]
    [SerializeField] private GUIMapSelectionButton _mapButtonBase;
    [Tooltip("Parent transform where all created buttons go")]
    [SerializeField] private Transform _mapButtons;
    [SerializeField] private SoundEffectSO _switchSound;
    [SerializeField] private Image _classIcon;
    #endregion

    #region Mono
    private void Awake()
    {
        _leftButton.onClick.AddListener(PreviousProfileButton_Clicked);
        _rightButton.onClick.AddListener(NextProfileButton_Clicked);
        foreach(var map in AvailableMaps)
        {
            var button = Instantiate(_mapButtonBase, _mapButtons);
            button.SetProfileInterface(_profileView);
            button.SetMap(map);
            _profileView.RegisterMapButton(button);
        }
    }
    #endregion

    #region Methods
    public async Task SetProfile(UserProfile userProfile, bool animateRig)
    {
        _nameTextBox.text = userProfile.Name;
        _difficultyTextBox.text = userProfile.Difficulty.Name;
        _PrestigeTextBox.text = userProfile.Prestige.ToString();
        var task1 = _profileView.OutfitManager.EquipOutfit(0, OutfitSO.OutfitSlot.Body);
        var task2 = _profileView.OutfitManager.EquipOutfit(userProfile.HatNumber, OutfitSO.OutfitSlot.Head);
        await Task.WhenAll(task1, task2);
        if (animateRig)
        {
            _profileView.PlayerVisualization.PlayAnimation("MenuStart", true);
            _profileView.PlayerVisualization.ResetAnimation();
            //first setting weapon at draw state so that it doesn't play animation of drawing
            _profileView.PlayerVisualization.EquippedWeapon.SetWeaponDraw();
            _profileView.PlayerVisualization.ChangeWeaponOutState(true);
        }
        if(SkyforgeLoader.ClassRegistry != null)
        {
            _classIcon.sprite = SkyforgeLoader.ClassRegistry.RegisteredClasses.FirstOrDefault(c => c.ID == userProfile.CurrentlyPickedClass).SimplifiedIcon;
        }
    }
    #endregion

    #region EventHandlers
    private void NextProfileButton_Clicked()
    {
        if (_profileView.CurrentProfile < _profileView.Profiles.Count - 1)
        {
            _profileView.SetCurrentProfile(_profileView.CurrentProfile + 1);
            _ = SetProfile(_profileView.Profiles[_profileView.CurrentProfile], true);
            SoundManager.UIInstance.PlayGlobalSFX(_switchSound);
        }
    }
    private void PreviousProfileButton_Clicked()
    {
        if (_profileView.CurrentProfile > 0)
        {
            _profileView.SetCurrentProfile(_profileView.CurrentProfile - 1);
            _ = SetProfile(_profileView.Profiles[_profileView.CurrentProfile], true);
            SoundManager.UIInstance.PlayGlobalSFX(_switchSound);
        }
    }
    #endregion
}
