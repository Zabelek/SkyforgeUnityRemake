using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GUIDebugWindow : MonoBehaviour
{
    #region Variables
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
        if(SkyforgeLoader.PerkRegistry != null)
        {
            /*var a9perk = SkyforgeLoader.PerkRegistry.Perks.FirstOrDefault(p => p.ID == "Base_Berserker_BurningChain");
            foreach (var perk in SkyforgeLoader.PerkRegistry.Perks)
            {
                if (perk != a9perk && !SkyforgeLoader.PerkRegistry.PerkSets[0].Perks.Contains(perk))
                {
                    var checkbox = Instantiate(_patternChechbox, _checkboxeGroup.transform);
                    checkbox.SetPerk(perk);
                    _checkboxes.Add(checkbox);
                }
            }
            _a9ComboBox.SetPerk(a9perk);*/
            foreach (var perk in SkyforgeLoader.PerkRegistry.PerkSets[0].Perks)
            {
                var checkbox = Instantiate(_patternChechbox, _terraCheckboxGroup.transform);
                checkbox.SetPerk(perk);
                _terraCheckboxes.Add(checkbox);
                checkbox.OnChange += TerraPerk_Changed;
            }
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
        if(_checkboxes == null) _checkboxes = new();
        if (_terraCheckboxes == null) _terraCheckboxes = new();
    }
    #endregion

    #region EventHandlers
    private void MenuButton_Clicked(object sender, EventArgs e)
    {
        _ = SkyforgeLoader.LoadScene("DivineObservatoryScene", "MainMenuScene");
    }
    private void UnlockButton_Clicked(object sender, EventArgs e)
    {
        /*foreach (var checkBox in _checkboxes)
        {
            checkBox.Check();
        }
        _a9ComboBox.Check();*/
        _terraCheckboxes[0].Check();
    }
    private void QuitButton_Clicked(object sender, EventArgs e)
    {
        Application.Quit();
    }
    private void TerraPerk_Changed(object sender, EventArgs e)
    {
        foreach(var checkbox in _terraCheckboxes)
        {
            if(sender is GUIDebugWindowPerkCheckBox && checkbox != (GUIDebugWindowPerkCheckBox)sender)
            {
                checkbox.UpdateSprite(this, EventArgs.Empty);
            }
        }
    }
    #endregion
}