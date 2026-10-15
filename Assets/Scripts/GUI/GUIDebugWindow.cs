using System;
using System.Linq;
using UnityEngine;

public class GUIDebugWindow : MonoBehaviour
{
    #region Variables
    [SerializeField] private GUICommonButton _menuButton, _quitButton, _debugStuffButton, _debugStuff2Button;
    #endregion

    #region Mono
    void Start()
    {
        _quitButton.OnClick += QuitButton_Clicked;
        _menuButton.OnClick += MenuButton_Clicked;
        _debugStuffButton.OnClick += DebugStuff_Clicked;
        _debugStuff2Button.OnClick += DebugStuff2_Clicked;
    }
    #endregion

    #region EventHandlers
    private void MenuButton_Clicked(object sender, EventArgs e)
    {
        _ = SkyforgeLoader.LoadScene(SkyforgeLoader.CurrentGameplayScene, "MainMenuScene");
    }
    private void QuitButton_Clicked(object sender, EventArgs e)
    {
        Application.Quit();
    }
    private void DebugStuff_Clicked(object sender, EventArgs e)
    {
        for (int i = 0; i < 20; i++)
        {
            SkyforgeLoader.CurrentProfile.Equipment.WeaponSlot.Item.ItemUpgradeState.EtherFibers[i] = 0;
        }
        if (SkyforgeLoader.CurrentProfile.Equipment.ArmorSlot.Item != null)
        {
            for (int i = 0; i < 20; i++)
            {
                SkyforgeLoader.CurrentProfile.Equipment.ArmorSlot.Item.ItemUpgradeState.EtherFibers[i] = 0;
            }
        }
        if (SkyforgeLoader.CurrentProfile.Equipment.ArtifactSlot.Item != null)
        {
            for (int i = 0; i < 20; i++)
            {
                SkyforgeLoader.CurrentProfile.Equipment.ArtifactSlot.Item.ItemUpgradeState.EtherFibers[i] = 0;
            }
        }
    }
    private void DebugStuff2_Clicked(object sender, EventArgs e)
    {
        var slot = SkyforgeLoader.CurrentProfile.Inventory.Slots.FirstOrDefault(s => s.IsLocked == true);
        if(slot != null)
        {
            slot.IsLocked = false;
        }
    }
    #endregion
}