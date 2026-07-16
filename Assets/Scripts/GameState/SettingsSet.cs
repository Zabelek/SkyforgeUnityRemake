public class SettingsSet
{
    public bool HighTextures { get; set; } = true;
    public bool HighShadows { get; set; } = true;
    public bool AntiAliasing { get; set; } = true;
    public bool AdaptiveProbeVolume { get; set; } = true;
    public bool SSR { get; set; } = true;
    public bool SSAO { get; set; } = true;
    public float Gamma { get; set; } = 0;
    public float SFXVolume { get; set; } = 0.6f;
    public float VoiceVolume { get; set; } = 1;
    public float MusicVolume { get; set; } = 0.4f;
    public bool Fullscreen { get; set; } = true;
    public bool VSync { get; set; } = true;
    public int FrameRateLimit { get; set; } = 60;

}
