using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIProfileCreationView : MonoBehaviour
{
    #region Variables
    [SerializeField] private TMP_InputField _nameInputBox;
    [Tooltip("Switches of the profile stats")]
    [SerializeField] private Button _nextHatButton, _previousHatButton, _nextDiff, _previousDiff;
    [Tooltip("Display text components of the switches")]
    [SerializeField] private TextMeshProUGUI _hatDisplayText, _diffDisplayText;
    [Tooltip("Buttons at the bottom of the screen")]
    [SerializeField] private GUICommonButton _backButton, _createButton;
    [Tooltip("Parent component that controls the scene")]
    [SerializeField] GUIProfileViewInterface _profileView;
    private int _currentHatNumber, _currentDiffNumber, _hatAmount;
    private List<DifficultyLevel> _diffLevels;
    #endregion

    #region Mono
    private void Awake()
    {
        _nextHatButton.onClick.AddListener(NextHatButton_Clicked);
        _previousHatButton.onClick.AddListener(PreviousHatButton_Clicked);
        _nextDiff.onClick.AddListener(NextDiffButton_Clicked);
        _previousDiff.onClick.AddListener(PreviousDiffButton_Clicked);
        _diffLevels = new();
        _diffLevels.Add(new DifficultyLevel() { EnemyDamageMod = 0.3f, EnemyHPMod = 0.4f, Name = "Average Lemon Squeezing Fan" });
        _diffLevels.Add(new DifficultyLevel() { EnemyDamageMod = 0.7f, EnemyHPMod = 0.8f, Name = "As Medium As It Gets" });
        _diffLevels.Add(new DifficultyLevel() { EnemyDamageMod = 1f, EnemyHPMod = 1f, Name = "Like In The Trailer" });
        _diffLevels.Add(new DifficultyLevel() { EnemyDamageMod = 2f, EnemyHPMod = 1.4f, Name = "Average Coffin Enjoyer" });
        _currentHatNumber = -1;
        _currentDiffNumber = 0;
        _hatAmount = 0;
    }
    private void Update()
    {
        if(_nameInputBox.text.Length>1)
        {
            _createButton.SetLocked(false);
        }
        else
        {
            _createButton.SetLocked(true);
        }
        if (_profileView.Profiles.Count > 0 && _backButton.gameObject.activeSelf == false)
        {
            _backButton.gameObject.SetActive(true);
        }
        else if (_profileView.Profiles.Count == 0 && _backButton.gameObject.activeSelf == true)
        {
            _backButton.gameObject.SetActive(false);
        }
    }
    #endregion

    #region Methods
    public UserProfile CreateNewProfile()
    {
        var ret = new UserProfile();
        ret.Name = _nameInputBox.text;
        ret.HatNumber = _currentHatNumber;
        ret.Difficulty = _diffLevels[_currentDiffNumber];
        ret.Prestige = 0;
        return ret;
    }
    private async Task UpdateHat()
    {
        var hat = await _profileView.OutfitManager.EquipOutfit(_currentHatNumber, OutfitSO.OutfitSlot.Head);
        _hatDisplayText.text = hat.OutfitSO.Name;
    }
    private void UpdateDifficulty()
    {
        _diffDisplayText.text = _diffLevels[_currentDiffNumber].Name;
    }
    public void Reset()
    {
        _ = _profileView.OutfitManager.EquipOutfit(0, OutfitSO.OutfitSlot.Body);
        _ = _profileView.OutfitManager.EquipOutfit(0, OutfitSO.OutfitSlot.Head);
        _currentDiffNumber = 0;
        _currentHatNumber = 0;
        UpdateDifficulty();
        _ = UpdateHat();
        _nameInputBox.text = "";
    }
    #endregion

    #region EventHandlers
    private void NextDiffButton_Clicked()
    {
        if (_currentDiffNumber < _diffLevels.Count - 1)
        {
            _currentDiffNumber++;
            UpdateDifficulty();
        }
    }
    private void PreviousDiffButton_Clicked()
    {
        if (_currentDiffNumber > 0)
        {
            _currentDiffNumber--;
            UpdateDifficulty();
        }
    }
    private void NextHatButton_Clicked()
    {
        //if true, the hat amount wasn't set yet
        if (_hatAmount == 0)
        {
            _hatAmount = SkyforgeLoader.OutfitRegistry.Outfits.Where(o => o.Slot == OutfitSO.OutfitSlot.Head).Count();
        }
        if (_currentHatNumber < _hatAmount - 1)
        {
            _currentHatNumber++;
            _ = UpdateHat();
        }
    }
    private void PreviousHatButton_Clicked()
    {
        //if true, the hat amount wasn't set yet
        if (_hatAmount == 0)
        {
            _hatAmount = SkyforgeLoader.OutfitRegistry.Outfits.Where(o => o.Slot == OutfitSO.OutfitSlot.Head).Count();
        }
        if (_currentHatNumber > 0)
        {
            _currentHatNumber--;
            _ = UpdateHat();
        }
    }
    #endregion
}
