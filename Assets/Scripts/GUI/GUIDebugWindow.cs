using System;
using UnityEngine;

public class GUIDebugWindow : MonoBehaviour
{
    #region Variables
    [SerializeField] private GUICommonButton _menuButton, _quitButton, _debugStuffButton;
    #endregion

    #region Mono
    void Start()
    {
        _quitButton.OnClick += QuitButton_Clicked;
        _menuButton.OnClick += MenuButton_Clicked;
        _debugStuffButton.OnClick += DebugStuff_Clicked;
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
        //SkyforgeLoader.CurrentProfile.Equipment.WeaponSlot.Item.ItemUpgradeState.Generation = 3;
        //SkyforgeLoader.CurrentProfile.Equipment.WeaponSlot.Item.ItemUpgradeState.PickedEtherDirection = ItemUpgradeState.EtherDirection.SageHalf;
        //SkyforgeLoader.CurrentProfile.Equipment.WeaponSlot.Item.ItemUpgradeState.EtherFibers[0] = 1;
        //SkyforgeLoader.CurrentProfile.Equipment.WeaponSlot.Item.ItemUpgradeState.EtherFibers[1] = 1;
        //SkyforgeLoader.CurrentProfile.Equipment.WeaponSlot.Item.ItemUpgradeState.EtherFibers[2] = 1;
        SkyforgeLoader.CurrentProfile.Inventory.AddItem("Base_Resource_TearOfRebirth");
        SkyforgeLoader.CurrentProfile.Inventory.AddItem("Base_Resource_FlameOfRebirth");
        SkyforgeLoader.CurrentProfile.Inventory.AddItem("Base_Resource_BreezeOfRebirth");
        SkyforgeLoader.CurrentProfile.Inventory.AddItem("Base_Resource_BudOfRebirth");
        SkyforgeLoader.CurrentProfile.Inventory.AddItem("Base_Resource_StarOfRebirth");
        SkyforgeLoader.CurrentProfile.Inventory.AddItem("Base_Resource_SealOfRebirth");
        SkyforgeLoader.CurrentProfile.Inventory.AddItem("Base_Resource_SparkleOfRebirth");
    }
    #endregion
}