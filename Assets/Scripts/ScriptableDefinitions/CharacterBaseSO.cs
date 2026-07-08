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
    #endregion
}
