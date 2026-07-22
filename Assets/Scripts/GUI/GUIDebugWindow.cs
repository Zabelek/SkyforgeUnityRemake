using System;
using UnityEngine;

public class GUIDebugWindow : MonoBehaviour
{
    #region Variables
    [SerializeField] private GUICommonButton _menuButton, _quitButton;
    #endregion

    #region Mono
    void Start()
    {
        _quitButton.OnClick += QuitButton_Clicked;
        _menuButton.OnClick += MenuButton_Clicked;
    }
    #endregion

    #region EventHandlers
    private void MenuButton_Clicked(object sender, EventArgs e)
    {
        _ = SkyforgeLoader.LoadScene("DivineObservatoryScene", "MainMenuScene");
    }
    private void QuitButton_Clicked(object sender, EventArgs e)
    {
        Application.Quit();
    }
    #endregion
}