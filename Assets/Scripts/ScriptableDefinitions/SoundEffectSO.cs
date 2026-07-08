using UnityEngine;

[CreateAssetMenu(menuName = "Skyforge Scriptable Objects/SoundEffectSO")]
public class SoundEffectSO : ScriptableObject
{
    [Tooltip("You can put more than one sound here, the clip will be chosen randomly")]
    public AudioClip[] AudioClips;
    public string Name;
    public float VolumeModifier = 1;
    [Tooltip("If not, the sound will be played by the static AudioSource instead of one from the pool in sound mamager")]
    public bool PlayedFromPool = true;
}