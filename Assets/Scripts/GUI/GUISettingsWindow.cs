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
    private Toggle _texturesToggle, _shadowsToggle, _antiAliasingToggle, _probeVolumeToggle, _ssrToggle, _ssaoToggle;
    private Slider _gammaSlider, _sfxSlider, _voiceSlider, _musicSlider;
    #endregion

    #region Mono
    private void Start()
    {
        SetUpGraphicsControls();
        SetUpSoundControls();
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
        _shadowsToggle = Instantiate(_patternCheckbox, _graphicsLayout.transform);
        _shadowsToggle.GetComponentInChildren<TextMeshProUGUI>().text = "High Quality Shadows";
        _shadowsToggle.onValueChanged.AddListener((bool value) =>
        {
            _manager.SetShadowsQuality(value);
        });
        _antiAliasingToggle = Instantiate(_patternCheckbox, _graphicsLayout.transform);
        _antiAliasingToggle.GetComponentInChildren<TextMeshProUGUI>().text = "Anti-Aliasing";
        _antiAliasingToggle.onValueChanged.AddListener((bool value) =>
        {
            _manager.SetAntiAliasing(value);
        });
        _probeVolumeToggle = Instantiate(_patternCheckbox, _graphicsLayout.transform);
        _probeVolumeToggle.GetComponentInChildren<TextMeshProUGUI>().text = "Adaptive Probe Volume";
        _probeVolumeToggle.onValueChanged.AddListener((bool value) =>
        {
            _manager.SetProbeVolume(value);
        });
        _ssrToggle = Instantiate(_patternCheckbox, _graphicsLayout.transform);
        _ssrToggle.GetComponentInChildren<TextMeshProUGUI>().text = "Screen Space Reflections";
        _ssrToggle.onValueChanged.AddListener((bool value) =>
        {
            _manager.SetSSR(value);
        });
        _ssaoToggle = Instantiate(_patternCheckbox, _graphicsLayout.transform);
        _ssaoToggle.GetComponentInChildren<TextMeshProUGUI>().text = "Ambient Occlusion";
        _ssaoToggle.onValueChanged.AddListener((bool value) =>
        {
            _manager.SetSSAO(value);
        });
        Instantiate(_patternSliderTitle, _graphicsLayout.transform).text = "Gamma";
        _gammaSlider = Instantiate(_patternSlider, _graphicsLayout.transform);
        _gammaSlider.minValue = -1;
        _gammaSlider.maxValue = 1;
        _gammaSlider.value = 0;
        _gammaSlider.onValueChanged.AddListener((float value) =>
        {
            _manager.SetGamma(value);
        });
    }
    private void SetUpSoundControls()
    {

        Instantiate(_patternSliderTitle, _soundLayout.transform).text = "SFX Volume";
        _sfxSlider = Instantiate(_patternSlider, _soundLayout.transform);
        _sfxSlider.minValue = 0.0001f;
        _sfxSlider.maxValue = 1;
        _sfxSlider.onValueChanged.AddListener((float value) =>
        {
            _manager.SetSFXVolume(value);
        });
        Instantiate(_patternSliderTitle, _soundLayout.transform).text = "Voice Volume";
        _voiceSlider = Instantiate(_patternSlider, _soundLayout.transform);
        _voiceSlider.minValue = 0.0001f;
        _voiceSlider.maxValue = 1;
        _voiceSlider.onValueChanged.AddListener((float value) =>
        {
            _manager.SetVoiceVolume(value);
        });
        Instantiate(_patternSliderTitle, _soundLayout.transform).text = "Music Volume";
        _musicSlider = Instantiate(_patternSlider, _soundLayout.transform);
        _musicSlider.minValue = 0.0001f;
        _musicSlider.maxValue = 1;
        _musicSlider.onValueChanged.AddListener((float value) =>
        {
            _manager.SetMusicVolume(value);
        });
    }
    private void LoadFromSettings()
    {
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
