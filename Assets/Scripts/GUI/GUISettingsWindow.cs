using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUISettingsWindow : MonoBehaviour
{
    #region Variables
    [Tooltip("Will be used to generate all checkbox in the window")]
    [SerializeField] private Toggle _patternCheckbox;
    [Tooltip("Will be used to generate all sliders in the window")]
    [SerializeField] private Slider _patternSlider;
    [Tooltip("Will be used to generate all slider title text in the window")]
    [SerializeField] private TextMeshProUGUI _patternSliderTitle;
    [Tooltip("Where all graphics related controls should be placed")]
    [SerializeField] private LayoutGroup _graphicsLayout;
    [Tooltip("Where all sound related controls should be placed")]
    [SerializeField] private LayoutGroup _soundLayout;
    [Tooltip("Buttons to save/load/restore settings")]
    [SerializeField] private GUICommonButton _saveButton, _loadButton, _restoreButton;
    [Tooltip("Settings Manager of the scene (every scene that needs to apply settings need this component)")]
    [SerializeField] private SettingsManager _manager;
    private Toggle _texturesToggle, _shadowsToggle, _antiAliasingToggle, _probeVolumeToggle, _ssrToggle, _ssaoToggle, _fullScreenToggle, _vSyncToggle;
    private Slider _gammaSlider, _sfxSlider, _voiceSlider, _musicSlider, _frameRateSlider;
    #endregion

    #region Mono
    private void Start()
    {
        _patternCheckbox.gameObject.SetActive(false);
        _patternSlider.gameObject.SetActive(false);
        _patternSliderTitle.gameObject.SetActive(false);
        _saveButton.OnClick += SaveSettings;
        _loadButton.OnClick += LoadSettings;
        _restoreButton.OnClick += RestoreDefaults;
        LoadFromSettings();
    }
    #endregion

    #region Methods
    private void SetUpGraphicsControls()
    {
        _texturesToggle = Instantiate(_patternCheckbox, _graphicsLayout.transform);
        _texturesToggle.GetComponentInChildren<TextMeshProUGUI>().text = "Full Resolution Textures";
        _texturesToggle.onValueChanged.AddListener((bool value) =>
        {
            _manager.SetTextureQuality(value);
        });
        _texturesToggle.gameObject.SetActive(true);
        _shadowsToggle = Instantiate(_patternCheckbox, _graphicsLayout.transform);
        _shadowsToggle.GetComponentInChildren<TextMeshProUGUI>().text = "High Quality Shadows";
        _shadowsToggle.onValueChanged.AddListener((bool value) =>
        {
            _manager.SetShadowsQuality(value);
        });
        _shadowsToggle.gameObject.SetActive(true);
        _antiAliasingToggle = Instantiate(_patternCheckbox, _graphicsLayout.transform);
        _antiAliasingToggle.GetComponentInChildren<TextMeshProUGUI>().text = "Anti-Aliasing";
        _antiAliasingToggle.onValueChanged.AddListener((bool value) =>
        {
            _manager.SetAntiAliasing(value);
        });
        _antiAliasingToggle.gameObject.SetActive(true);
        _probeVolumeToggle = Instantiate(_patternCheckbox, _graphicsLayout.transform);
        _probeVolumeToggle.GetComponentInChildren<TextMeshProUGUI>().text = "Adaptive Probe Volume";
        _probeVolumeToggle.onValueChanged.AddListener((bool value) =>
        {
            _manager.SetProbeVolume(value);
        });
        _probeVolumeToggle.gameObject.SetActive(true);
        _ssrToggle = Instantiate(_patternCheckbox, _graphicsLayout.transform);
        _ssrToggle.GetComponentInChildren<TextMeshProUGUI>().text = "Screen Space Reflections";
        _ssrToggle.onValueChanged.AddListener((bool value) =>
        {
            _manager.SetSSR(value);
        });
        _ssrToggle.gameObject.SetActive(true);
        _ssaoToggle = Instantiate(_patternCheckbox, _graphicsLayout.transform);
        _ssaoToggle.GetComponentInChildren<TextMeshProUGUI>().text = "Ambient Occlusion";
        _ssaoToggle.onValueChanged.AddListener((bool value) =>
        {
            _manager.SetSSAO(value);
        });
        _ssaoToggle.gameObject.SetActive(true);
        var gammaTitle = Instantiate(_patternSliderTitle, _graphicsLayout.transform);
        gammaTitle.text = "Gamma";
        gammaTitle.gameObject.SetActive(true);
        _gammaSlider = Instantiate(_patternSlider, _graphicsLayout.transform);
        _gammaSlider.minValue = -1;
        _gammaSlider.maxValue = 1;
        _gammaSlider.value = 0;
        _gammaSlider.onValueChanged.AddListener((float value) =>
        {
            _manager.SetGamma(value);
        });
        _gammaSlider.gameObject.SetActive(true);
        _fullScreenToggle = Instantiate(_patternCheckbox, _graphicsLayout.transform);
        _fullScreenToggle.GetComponentInChildren<TextMeshProUGUI>().text = "Full Screen";
        _fullScreenToggle.onValueChanged.AddListener((bool value) =>
        {
            _manager.SetFullscreen(value);
        });
        _fullScreenToggle.gameObject.SetActive(true);
        _vSyncToggle = Instantiate(_patternCheckbox, _graphicsLayout.transform);
        _vSyncToggle.GetComponentInChildren<TextMeshProUGUI>().text = "VSync";
        _vSyncToggle.onValueChanged.AddListener((bool value) =>
        {
            _manager.SetVSync(value);
        });
        _vSyncToggle.gameObject.SetActive(true);
        var frameRateTitle = Instantiate(_patternSliderTitle, _graphicsLayout.transform);
        frameRateTitle.text = "Frame Rate";
        frameRateTitle.gameObject.SetActive(true);
        _frameRateSlider = Instantiate(_patternSlider, _graphicsLayout.transform);
        _frameRateSlider.minValue = 30;
        _frameRateSlider.maxValue = 240;
        _frameRateSlider.value = 60;
        _frameRateSlider.wholeNumbers = true;
        _frameRateSlider.onValueChanged.AddListener((float value) =>
        {
            _manager.SetFrameRateLimit(value);
        });
        _frameRateSlider.gameObject.SetActive(true);
    }
    private void SetUpSoundControls()
    {

        var sfxTitle = Instantiate(_patternSliderTitle, _soundLayout.transform);
        sfxTitle.text = "SFX Volume";
        sfxTitle.gameObject.SetActive(true);
        _sfxSlider = Instantiate(_patternSlider, _soundLayout.transform);
        _sfxSlider.minValue = 0.0001f;
        _sfxSlider.maxValue = 1;
        _sfxSlider.onValueChanged.AddListener((float value) =>
        {
            _manager.SetSFXVolume(value);
        });
        _sfxSlider.gameObject.SetActive(true);
        var voiceTitle = Instantiate(_patternSliderTitle, _soundLayout.transform);
        voiceTitle.text = "Voice Volume";
        voiceTitle.gameObject.SetActive(true);
        _voiceSlider = Instantiate(_patternSlider, _soundLayout.transform);
        _voiceSlider.minValue = 0.0001f;
        _voiceSlider.maxValue = 1;
        _voiceSlider.onValueChanged.AddListener((float value) =>
        {
            _manager.SetVoiceVolume(value);
        });
        _voiceSlider.gameObject.SetActive(true);
        var musicTitle = Instantiate(_patternSliderTitle, _soundLayout.transform);
        musicTitle.text = "Music Volume";
        musicTitle.gameObject.SetActive(true);
        _musicSlider = Instantiate(_patternSlider, _soundLayout.transform);
        _musicSlider.minValue = 0.0001f;
        _musicSlider.maxValue = 1;
        _musicSlider.onValueChanged.AddListener((float value) =>
        {
            _manager.SetMusicVolume(value);
        });
        _musicSlider.gameObject.SetActive(true);
    }
    public void LoadFromSettings()
    {
        if (_texturesToggle == null)
            SetUpGraphicsControls();
        _texturesToggle.isOn = SkyforgeLoader.SettingsSet.HighTextures;
        _texturesToggle.GetComponent<GUICustomToggle>().SetByCode(SkyforgeLoader.SettingsSet.HighTextures);
        _shadowsToggle.isOn = SkyforgeLoader.SettingsSet.HighShadows;
        _shadowsToggle.GetComponent<GUICustomToggle>().SetByCode(SkyforgeLoader.SettingsSet.HighShadows);
        _antiAliasingToggle.isOn = SkyforgeLoader.SettingsSet.AntiAliasing;
        _antiAliasingToggle.GetComponent<GUICustomToggle>().SetByCode(SkyforgeLoader.SettingsSet.AntiAliasing);
        _ssrToggle.isOn = SkyforgeLoader.SettingsSet.SSR;
        _ssrToggle.GetComponent<GUICustomToggle>().SetByCode(SkyforgeLoader.SettingsSet.SSR);
        _ssaoToggle.isOn = SkyforgeLoader.SettingsSet.SSAO;
        _ssaoToggle.GetComponent<GUICustomToggle>().SetByCode(SkyforgeLoader.SettingsSet.SSAO);
        _probeVolumeToggle.isOn = SkyforgeLoader.SettingsSet.AdaptiveProbeVolume;
        _probeVolumeToggle.GetComponent<GUICustomToggle>().SetByCode(SkyforgeLoader.SettingsSet.AdaptiveProbeVolume);
        _gammaSlider.value = SkyforgeLoader.SettingsSet.Gamma;
        _fullScreenToggle.isOn = SkyforgeLoader.SettingsSet.Fullscreen;
        _fullScreenToggle.GetComponent<GUICustomToggle>().SetByCode(SkyforgeLoader.SettingsSet.Fullscreen);
        _vSyncToggle.isOn = SkyforgeLoader.SettingsSet.VSync;
        _vSyncToggle.GetComponent<GUICustomToggle>().SetByCode(SkyforgeLoader.SettingsSet.VSync);
        _frameRateSlider.value = SkyforgeLoader.SettingsSet.FrameRateLimit;
        if (_sfxSlider == null)
            SetUpSoundControls();
        _sfxSlider.value = SkyforgeLoader.SettingsSet.SFXVolume;
        _voiceSlider.value = SkyforgeLoader.SettingsSet.VoiceVolume;
        _musicSlider.value = SkyforgeLoader.SettingsSet.MusicVolume;
    }
    #endregion

    #region EventHandlers
    private void SaveSettings(object sender, EventArgs e)
    {
        _manager.SaveSettings();
    }
    private void LoadSettings(object sender, EventArgs e)
    {
        _manager.LoadSettings();
        LoadFromSettings();
    }
    private void RestoreDefaults(object sender, EventArgs e)
    {
        _manager.RestoreDefaults();
        LoadFromSettings();
    }
    #endregion
}
