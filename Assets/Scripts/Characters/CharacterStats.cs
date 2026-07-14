using System;

public class CharacterStats
{
    #region Variables
    public int MaxHP { get; set; }
    private int _currentHP;
    public int CurrentHP { 
        get => _currentHP;
        set => _currentHP = Math.Clamp(value, 0, MaxHP);
    }
    public int MaxMana { get; set; }
    private int _currentMana;
    public int CurrentMana {
        get => _currentMana;
        set => _currentMana = Math.Clamp(value, 0, MaxMana);
    }
    public float MovementSpeed { get; set; }
    public float AttackSpeed { get; set; }
    public float CriticalChance { get; set; }
    public int BaseDamage { get; set; }
    public int CombatManaRegen { get; set; }
    #endregion

    #region Methods
    public void Reset(CharacterBaseSO baseSO)
    {
        MaxHP = baseSO.MaxHealth;
        MaxMana = baseSO.MaxMana;
        MovementSpeed = baseSO.MovementSpeed;
        AttackSpeed = baseSO.AttackSpeed;
        CriticalChance = baseSO.CriticalChance;
        BaseDamage = baseSO.BaseDamage;
        CombatManaRegen = baseSO.CombatManaRegen;
        if (CurrentHP > MaxHP)
            CurrentHP = MaxHP;
        if (CurrentMana > MaxMana)
            CurrentMana = MaxMana;
    }
    public void ModifyAccordingToPerk(PerkSO perk, int modifier)
    {
        if (perk.Stat == PerkSO.StatType.AttackSpeed)
        {
            AttackSpeed += perk.Value * modifier;
        }
        else if (perk.Stat == PerkSO.StatType.BaseDamage)
        {
            BaseDamage += (int)(perk.Value) * modifier;
        }
        else if (perk.Stat == PerkSO.StatType.CriticalChance)
        {
            CriticalChance += perk.Value * modifier;
        }
        else if (perk.Stat == PerkSO.StatType.MaxHP)
        {
            MaxHP += (int)(perk.Value) * modifier;
        }
        else if (perk.Stat == PerkSO.StatType.CombatManaRegen)
        {
            CombatManaRegen += (int)(perk.Value) * modifier;
        }
    }
    public void ModifyAccordingToDifficultyLevel()
    {
        if(SkyforgeLoader.CurrentProfile!=null)
        {
            MaxHP = (int)(MaxHP * SkyforgeLoader.CurrentProfile.Difficulty.EnemyHPMod);
            BaseDamage = (int)(BaseDamage * SkyforgeLoader.CurrentProfile.Difficulty.EnemyDamageMod);
            if(BaseDamage == 0)
                BaseDamage = 1;
            if (CurrentHP != MaxHP)
                CurrentHP = MaxHP;
        }
    }
    #endregion
}
