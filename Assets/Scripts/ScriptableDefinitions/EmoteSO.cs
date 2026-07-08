using UnityEngine;

[CreateAssetMenu(menuName = "Skyforge Scriptable Objects/EmoteSO")]
public class EmoteSO : ScriptableObject
{
    public string Name;
    public Sprite Icon;
    [Tooltip("If the character uses this emote, their AnimatorController will be swapped with this one for the time of emote playing")]
    public RuntimeAnimatorController AnimatorController;
    [Tooltip("If the character using this emote has to make some noise")]
    public SoundEffectSO Sound;
    [Tooltip("if the sound can't play immediately after clicking the emote button")]
    public float SoundStartDelay;
    [Tooltip("Time between the player clicking to stop the emote and the emote actually logically ending")]
    public float EmoteEndDelay;
    public bool Loop = false;
    [Tooltip("If the character can use this emote during combat")]
    public bool CombatEnabled = false;
}
