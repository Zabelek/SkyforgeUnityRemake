using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GUIGameMenuTopButtonsPanel : MonoBehaviour
{
    #region Variables
    [SerializeField] private GUIGameMenu _menu;
    [SerializeField] private Button _settingsButton, _exitButton, _closeButton;
    #endregion

    #region Mono
    private void Awake()
    {
        _settingsButton.onClick.AddListener(SettingsButton_Clicked);
        _exitButton.onClick.AddListener(ExitButton_Clicked);
        _closeButton.onClick.AddListener(CloseButton_Clicked);
    }
    #endregion

    #region Methods
    #endregion

    #region EventHandlers
    private void CloseButton_Clicked()
    {
        _menu.MenuClose_Clicked(this, EventArgs.Empty);
    }
    private void ExitButton_Clicked()
    {
        Application.Quit();
    }
    private void SettingsButton_Clicked()
    {
        _menu.ShowSettingsView();
    }
    #endregion
}
