using UnityEngine;

[CreateAssetMenu(menuName = "Skyforge Scriptable Objects/AbilitySO")]
public class AbilitySO : ScriptableObject
{
    [Tooltip("Defines the time after user will be \"unlocked\" and be able to move/use other abilities")]
    public float AttackTimerNext = 1;
    [Tooltip("Defines the full length of ability casting")]
    public float AttackTimerMax = 1;
    [Tooltip("Set this to true if the ability ignores character's attack speed")]
    public bool IsSkill = false;
    [Tooltip("Default ability icon")]
    public Sprite Icon;
    [Tooltip("If the ability has an upgraded version, place its icon here")]
    public Sprite UpgradedIcon;
    [Tooltip("If the ability is escape ability, place its icon here")]
    public Sprite EscapeIcon;
    [Tooltip("For mouse right/left combos")]
    public HeroClassBehaviour.ComboState StateType;
    public string Name;
    public string Description;
    [Tooltip("Hero Class for which this ability is created")]
    public HeroClassSO HeroClass;
    [Tooltip("Base character's damage output will be multiplied by this value while calculating damage")]
    public float DamageMultiplier = 1;
    public int ManaCost = 0;
    [Tooltip("Determines a chance for the performing character to shout while using")]
    [Range(0, 1)]
    public float AbilityWeight = 0.1f;
    public float Cooldown = 0;
    [Tooltip("If the ability has an upgraded version, here's a separate cooldown for it")]
    public float UpgradedCooldown = 0;
    [Tooltip("If the ability is an escape ability, here's a separate cooldown for it")]
    public float EscapeCooldown = 0;
}
