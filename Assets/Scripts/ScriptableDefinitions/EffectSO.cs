using UnityEngine;

[CreateAssetMenu(menuName = "Skyforge Scriptable Objects/EffectSO")]
public class EffectSO : ScriptableObject
{
    public enum EffectType
    {
        Stun, Slow, Fear, DamageOverTime, DamageDebuff, ArmorDebuff, MoveAround, Cooldown, MovementStop
    }
    [Tooltip("Type of the effect can help determine what the effect does. This list can be empty")]
    public EffectType[] Types;
    public string Name;
    public string Description;
    [Tooltip("This image will be displayed in the interface")]
    public Sprite Image;
    [Tooltip("If it's true, the effect won't be displayed in the interface")]
    public bool IsHidden;
    [Tooltip("Some effects accumulate stacks. If false, stack number will be hidden and staks will always be zeroed")]
    public bool IsStackable = false;
}