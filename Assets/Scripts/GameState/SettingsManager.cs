using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class SettingsManager : MonoBehaviour
{
    private const float MUTED_VOLUME_DB = -80f;

    #region Variables
    [Tooltip("Include only cameras that use graphics effects. Simply ignore interface cameras")]
    [SerializeField] private Camera[] _3dSceneCameras;
    [SerializeField] private GameObject _reflectionProbe;
    [SerializeField] private Volume _volume;
    [SerializeField] private AudioMixer _audioMixer;
    private ScreenSpaceReflection _ssrSettings;
    private ScreenSpaceAmbientOcclusion _ssaoSettings;
    private LiftGammaGain _gammaSettings;
    [Tooltip("Temporal Anti-Aliasing glitches with rendering 3d object on alpha channel, so if the scene uses such rendering, use SMAA instead")]
    public bool UseSMAA;
    #endregion

    #region Mono
    private void Awake()
    {
        if (SkyforgeLoader.SettingsSet == null)
        {
            LoadSettings();
        }
        else
        {
            ApplySceneSettings();
        }
    }
    #endregion

    #region Methods
    public void ApplyGlobalSettings()
    {
        SetTextureQuality(SkyforgeLoader.SettingsSet.HighTextures);
        SetShadowsQuality(SkyforgeLoader.SettingsSet.HighShadows);
        SetSFXVolume(SkyforgeLoader.SettingsSet.SFXVolume);
        SetVoiceVolume(SkyforgeLoader.SettingsSet.VoiceVolume);
        SetMusicVolume(SkyforgeLoader.SettingsSet.MusicVolume);
    }
    public void ApplySceneSettings()
    {
        SetSSR(SkyforgeLoader.SettingsSet.SSR);
        SetSSAO(SkyforgeLoader.SettingsSet.SSAO);
        SetProbeVolume(SkyforgeLoader.SettingsSet.AdaptiveProbeVolume);
        SetAntiAliasing(SkyforgeLoader.SettingsSet.AntiAliasing);
        SetGamma(SkyforgeLoader.SettingsSet.Gamma);
        SetFullscreen(SkyforgeLoader.SettingsSet.Fullscreen);
        SetVSync(SkyforgeLoader.SettingsSet.VSync);
        SetFrameRateLimit(SkyforgeLoader.SettingsSet.FrameRateLimit);
    }
    public void SaveSettings()
    {
        SaveManager.SaveSettings();
    }
    public void LoadSettings()
    {
        if (SaveManager.LoadSettings() == false)
            RestoreDefaults();
        ApplyGlobalSettings();
        ApplySceneSettings();
    }
    public void RestoreDefaults()
    {
        SkyforgeLoader.SettingsSet = new();
    }
    private void SetSettingsParameter(ref object globalParam, object localParam)
    {
        if (globalParam.GetType() == localParam.GetType())
        {
            if (globalParam != localParam)
                SkyforgeLoader.SettingsChanged = true;
            globalParam = localParam;
        }
        else { Debug.Log("Settings peremeter mismatch!"); }
    }
    private T SetSettingsParameter<T>(T globalParam, T localParam)
    {
        if (!EqualityComparer<T>.Default.Equals(globalParam, localParam))
            SkyforgeLoader.SettingsChanged = true;
        return localParam;
    }
    private static float LinearVolumeToDecibels(float value)
    {
        value = Mathf.Clamp01(value);
        return value <= 0.0001f ? MUTED_VOLUME_DB : Mathf.Log10(value) * 20f;
    }
    #endregion

    #region Setters
    public void SetTextureQuality(bool quality)
    {
        SkyforgeLoader.SettingsSet.HighTextures = SetSettingsParameter(SkyforgeLoader.SettingsSet.HighTextures, quality);
        if (quality)
        {
            QualitySettings.globalTextureMipmapLimit = 0;
        }
        else
        {
            QualitySettings.globalTextureMipmapLimit = 2;
        }
    }
    public void SetShadowsQuality(bool quality)
    {
        SkyforgeLoader.SettingsSet.HighShadows = SetSettingsParameter(SkyforgeLoader.SettingsSet.HighShadows, quality);
        if (quality)
        {
            QualitySettings.shadowDistance = 150;
            QualitySettings.shadowResolution = ShadowResolution.High;
            QualitySettings.shadows = ShadowQuality.All;
        }
        else
        {
            QualitySettings.shadowDistance = 25;
            QualitySettings.shadowResolution = ShadowResolution.Low;
            QualitySettings.shadows = ShadowQuality.HardOnly;
        }
    }
    public void SetAntiAliasing(bool value)
    {
        SkyforgeLoader.SettingsSet.AntiAliasing = SetSettingsParameter(SkyforgeLoader.SettingsSet.AntiAliasing, value);
        foreach (var camera in _3dSceneCameras)
        {
            if (value)
            {
                if(UseSMAA)
                    camera.GetComponent<HDAdditionalCameraData>().antialiasing = HDAdditionalCameraData.AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                else
                    camera.GetComponent<HDAdditionalCameraData>().antialiasing = HDAdditionalCameraData.AntialiasingMode.TemporalAntialiasing;
            }
            else
            {
                camera.GetComponent<HDAdditionalCameraData>().antialiasing = HDAdditionalCameraData.AntialiasingMode.None;
            }
        }
    }
    public void SetProbeVolume(bool value)
    {
        SkyforgeLoader.SettingsSet.AdaptiveProbeVolume = SetSettingsParameter(SkyforgeLoader.SettingsSet.AdaptiveProbeVolume, value);
        if (_reflectionProbe!=null)
        {
            if (value)
            {
                _reflectionProbe.SetActive(true);
            }
            else
            {
                _reflectionProbe.SetActive(false);
            }
        }
    }
    public void SetSSR(bool value)
    {
        SkyforgeLoader.SettingsSet.SSR = SetSettingsParameter(SkyforgeLoader.SettingsSet.SSR, value);
        if (_ssrSettings == null) _volume?.profile.TryGet(out _ssrSettings);
        if (_ssrSettings != null)
        {
            if (value)
            {
                _ssrSettings.active = true;
            }
            else
            {
                _ssrSettings.active = false;
            }
        }
    }
    public void SetGamma(float value)
    {
        SkyforgeLoader.SettingsSet.Gamma = SetSettingsParameter(SkyforgeLoader.SettingsSet.Gamma, value);
        if (_gammaSettings == null) _volume?.profile.TryGet(out _gammaSettings);
        if(_gammaSettings != null)
        {
            _gammaSettings.gamma.value = new Vector4(_gammaSettings.gamma.value.x, _gammaSettings.gamma.value.y, _gammaSettings.gamma.value.z, value);
        }
    }
    public void SetSFXVolume(float value)
    {
        SkyforgeLoader.SettingsSet.SFXVolume = SetSettingsParameter(SkyforgeLoader.SettingsSet.SFXVolume, value);
        if (_audioMixer != null)
        {
            _audioMixer.SetFloat("SFXVolume", LinearVolumeToDecibels(value));
        }
    }
    public void SetVoiceVolume(float value)
    {
        SkyforgeLoader.SettingsSet.VoiceVolume = SetSettingsParameter(SkyforgeLoader.SettingsSet.VoiceVolume, value);
        if (_audioMixer != null)
        {
            _audioMixer.SetFloat("VoiceVolume", LinearVolumeToDecibels(value));
        }
    }
    public void SetMusicVolume(float value)
    {
        SkyforgeLoader.SettingsSet.MusicVolume = SetSettingsParameter(SkyforgeLoader.SettingsSet.MusicVolume, value);
        if (_audioMixer != null)
        {
            _audioMixer.SetFloat("MusicVolume", LinearVolumeToDecibels(value));
        }
    }
    public void SetSSAO(bool value)
    {
        SkyforgeLoader.SettingsSet.SSAO = SetSettingsParameter(SkyforgeLoader.SettingsSet.SSAO, value);
        if (_ssaoSettings == null) _volume?.profile.TryGet(out _ssaoSettings);
        if (_ssaoSettings != null)
        {
            if (value)
            {
                _ssaoSettings.active = true;
            }
            else
            {
                _ssaoSettings.active = false;
            }
        }
    }
    public void SetFullscreen(bool value)
    {
        SkyforgeLoader.SettingsSet.Fullscreen = SetSettingsParameter(SkyforgeLoader.SettingsSet.Fullscreen, value);
        Screen.fullScreenMode = value ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }
    public void SetVSync(bool value)
    {
        SkyforgeLoader.SettingsSet.VSync = SetSettingsParameter(SkyforgeLoader.SettingsSet.VSync, value);
        QualitySettings.vSyncCount = value ? 1 : 0;
    }
    public void SetFrameRateLimit(float value)
    {
        int intVal = Mathf.RoundToInt(value);
        SkyforgeLoader.SettingsSet.FrameRateLimit = SetSettingsParameter(SkyforgeLoader.SettingsSet.FrameRateLimit, intVal);
        Application.targetFrameRate = intVal;
    }
    #endregion
}
