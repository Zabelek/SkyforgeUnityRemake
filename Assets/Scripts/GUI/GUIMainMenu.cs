using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIMainMenu : MonoBehaviour
{
    #region Variables
    [SerializeField] private GUICommonButton _startButton;
    [SerializeField] private Button _exitButton, _discordButton;
    [SerializeField] private GUISceneBlackFade _blackFade;
    private bool _playPressed;
    private PlayerInputBehaviour _playerInput;
    [SerializeField] private TextMeshProUGUI _versionTextBox;
    [SerializeField] private VoicelineSO[] _voicelines;
    [SerializeField] private Transform _cameraPose;
    #endregion

    #region Mono
    private void Awake()
    {
        _startButton.OnClick += PlayButton_Clicked;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        _playPressed = false;
        _exitButton.onClick.AddListener(ExitButtonClicked);
        _discordButton.onClick.AddListener(DiscordButtonClicked);
        _playerInput = GetComponent<PlayerInputBehaviour>();
        _playerInput.OnAppExitAction += ExitButtonClicked;
        _ = _blackFade.StartFadeOut();
        SkyforgeLoader.LoadedSceneReady = true;
        _versionTextBox.text = "Version: " + Application.version;
        if(SkyforgeLoader.GameBeaten)
        {
            StartCoroutine(PlayEnGameVoice());
        }
    }
    private void OnDestroy()
    {
        _playerInput.OnAppExitAction -= ExitButtonClicked;
        _startButton.OnClick -= PlayButton_Clicked;
    }
    #endregion

    #region EventHandlers
    private void PlayButton_Clicked(object sender, EventArgs e)
    {
        if(_playPressed == false)
        {
            _playPressed = true;
            _ = SkyforgeLoader.LoadScene("MainMenuScene", "ProfileScene");
        }
    }
    private void ExitButtonClicked(object sender, EventArgs e)
    {
        Application.Quit();
    }
    private void ExitButtonClicked()
    {
        ExitButtonClicked(null, null);
    }
    private void DiscordButtonClicked()
    {
        Application.OpenURL("https://discord.gg/8a66zGFG44");
    }
    private IEnumerator PlayEnGameVoice()
    {
        if (SkyforgeLoader.HardestDiffBeaten)
        {
            SkyforgeLoader.HardestDiffBeaten = false;
            SkyforgeLoader.GameBeaten = false;
            AudioSource.PlayClipAtPoint(_voicelines[2].Voice, _cameraPose.position, _voicelines[2].Volume);
            yield return new WaitForSeconds(_voicelines[2].Time);
            AudioSource.PlayClipAtPoint(_voicelines[1].Voice, _cameraPose.position, _voicelines[1].Volume);
            yield return new WaitForSeconds(_voicelines[1].Time);
        }
        else
        {
            SkyforgeLoader.GameBeaten = false;
            AudioSource.PlayClipAtPoint(_voicelines[0].Voice, _cameraPose.position, _voicelines[0].Volume);
            yield return new WaitForSeconds(_voicelines[0].Time);
            AudioSource.PlayClipAtPoint(_voicelines[1].Voice, _cameraPose.position, _voicelines[1].Volume);
            yield return new WaitForSeconds(_voicelines[1].Time);
        }
    }

    #endregion
}
