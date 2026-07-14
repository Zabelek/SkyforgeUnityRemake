using UnityEngine;

[CreateAssetMenu(menuName = "Skyforge Scriptable Objects/PerkSO")]
public class PerkSO : ScriptableObject
{
    public enum StatType
    {
        MaxHP, BaseDamage, CriticalChance, AttackSpeed, CombatManaRegen
    }
    [Tooltip("Must be unique. This string identifies the perk in the game systems. If you're modding the game, best use your name instead of Base_ at the beginning")]
    public string ID;
    [Tooltip("Specifies which stat this perk would modify. If Functional is set to true, it'll be ignored")]
    public StatType Stat;
    [Tooltip("Stat modifier. If Functional is set to true, it'll be ignored")]
    public float Value;
    [Tooltip("Setting it to true meant that this perk has a purpose other than stat modification")]
    public bool Functional;
    public string Name;
    [Tooltip("It'll be displayed in the interface(once implemented)")]
    public string Description;
    [Tooltip("If the perks applies to a specific class, it'll be active only when the player has this class picked")]
    public HeroClassSO Class;
}
