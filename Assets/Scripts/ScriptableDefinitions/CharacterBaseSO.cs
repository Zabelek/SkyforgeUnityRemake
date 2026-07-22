using UnityEngine;

[CreateAssetMenu(menuName = "Skyforge Scriptable Objects/CharacterBaseSO")]
public class CharacterBaseSO : ScriptableObject
{
    #region BaseParameters
    public string Name;
    public CharacterCategorySO Category;
    [Tooltip("If it's true, the character will ignore stuns, slows, fears etc")]
    public bool CCResistant = false;
    #endregion 

    #region NumericStats
    public int MaxHealth;
    public int MaxMana;
    public float MovementSpeed;
    public float AttackSpeed;
    public int BaseDamage;
    public float CriticalChance;
    public int CombatManaRegen;
    [Tooltip("Percent damage reduction of the character. If 1, the damage will be reduced by 100%, but each hit will still deal 1 damage")]
    public float Defense;
    [Tooltip("Healing percent of the character each time they deal damage. 1 vampirism means that they will heal by 100% of dealt damage")]
    public float Vampirism;
    [Tooltip("percentage resistance to all the slows, stuns, fears etc. Max should be 1, meaning, 100% reduction, however even with 100%, the effect won't dissapear, but they all will ahve 0.1s duration.")]
    public float Stability;
    [Tooltip("Each damage output is a random value between BaseDamage and BaseDamage+MaxDamage")]
    public int MaxDamage;
    #endregion
}
