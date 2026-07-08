using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GUIDebugWindow : MonoBehaviour
{
    #region Variables
    private PlayerBehaviour _player;
    [SerializeField] private GUIDebugWindowPerkCheckBox _patternChechbox;
    [SerializeField] private LayoutGroup _checkboxeGroup;
    [SerializeField] private LayoutGroup _terraCheckboxGroup;
    [SerializeField] private GUIDebugWindowPerkCheckBox _a9ComboBox;
    private List<GUIDebugWindowPerkCheckBox> _checkboxes;
    private List<GUIDebugWindowPerkCheckBox> _terraCheckboxes;
    [SerializeField] private GUICommonButton _unlockButton, _menuButton, _quitButton;
    #endregion

    #region Mono
    void Start()
    {
        _checkboxes = new();
        _terraCheckboxes = new();
        var a9perk = Globals.Instance.RegisteredPerks.FirstOrDefault(p => p.Name == "Burning Chain");
        foreach (var perk in Globals.Instance.RegisteredPerks)
        {
            if(perk != a9perk)
            {
                var checkbox = Instantiate(_patternChechbox, _checkboxeGroup.transform);
                if (_player != null)
                {
                    checkbox.SetPlayer(_player);
                }
                checkbox.SetPerk(perk);
                _checkboxes.Add(checkbox);
            }
        }
        _a9ComboBox.SetPlayer(_player);
        _a9ComboBox.SetPerk(a9perk);
        foreach (var perk in Globals.Instance.RegisteredPerkSets[0].Perks)
        {
            var checkbox = Instantiate(_patternChechbox, _terraCheckboxGroup.transform);
            if (_player != null)
            {
                checkbox.SetPlayer(_player);
            }
            checkbox.SetPerk(perk);
            _terraCheckboxes.Add(checkbox);
        }
        _patternChechbox.gameObject.SetActive(false);
        _quitButton.OnClick += QuitButton_Clicked;
        _unlockButton.OnClick += UnlockButton_Clicked;
        _menuButton.OnClick += MenuButton_Clicked;
    }
    #endregion

    #region Methods
    public void SetPlayer(PlayerBehaviour player)
    {
        _player = player;
        if(_checkboxes == null) _checkboxes = new();
        if (_terraCheckboxes == null) _terraCheckboxes = new();
        foreach (var checkbox in _checkboxes)
        {
            checkbox.SetPlayer(player);
        }
        foreach (var checkbox in _terraCheckboxes)
        {
            checkbox.SetPlayer(player);
        }
        _a9ComboBox.SetPlayer(player);
    }
    #endregion

    #region EventHandlers
    private void MenuButton_Clicked(object sender, EventArgs e)
    {
        _ = SkyforgeLoader.LoadScene("DivineObservatoryScene", "MainMenuScene");
    }
    private void UnlockButton_Clicked(object sender, EventArgs e)
    {
        foreach (var checkBox in _checkboxes)
        {
            checkBox.Check();
        }
        _terraCheckboxes[0].Check();
        _a9ComboBox.Check();
    }
    private void QuitButton_Clicked(object sender, EventArgs e)
    {
        Application.Quit();
    }
    #endregion
}