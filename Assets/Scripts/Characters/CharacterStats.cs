using System;
using UnityEngine;

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
    //max bonus value that the character can inflict. Every hit, the character inflicts a random damage betwen Base damage and Base damage + Max damage;
    public int MaxDamage { get; set; }
    public int CombatManaRegen { get; set; }
    //healing percent of the character each time they deal damage. 1 vampirism means that they will heal by 100% of dealt damage
    public float Vampirism { get; set; }
    //Percent damage reduction of the character
    public float Defense { get; set; }
    //percent negative effect duration reduction of the character
    public float Stability { get; set; }
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
        MaxDamage = baseSO.MaxDamage;
        Defense = baseSO.Defense;
        Vampirism = baseSO.Vampirism;
        Stability = baseSO.Stability;
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
        else if (perk.Stat == PerkSO.StatType.MaxDamage)
        {
            MaxDamage += (int)(perk.Value) * modifier;
        }
        else if (perk.Stat == PerkSO.StatType.CriticalChance)
        {
            CriticalChance += perk.Value * modifier;
        }
        else if (perk.Stat == PerkSO.StatType.MaxHP)
        {
            MaxHP += (int)(perk.Value) * modifier;
            if (CurrentHP > MaxHP)
                CurrentHP = MaxHP;
        }
        else if (perk.Stat == PerkSO.StatType.CombatManaRegen)
        {
            CombatManaRegen += (int)(perk.Value) * modifier;
        }
        else if (perk.Stat == PerkSO.StatType.Vampirism)
        {
            Vampirism += perk.Value * modifier;
        }
        else if (perk.Stat == PerkSO.StatType.Defense)
        {
            Defense += perk.Value * modifier;
        }
        else if (perk.Stat == PerkSO.StatType.Stability)
        {
            Stability += perk.Value * modifier;
        }
    }
    public void ModifyAccordingToDifficultyLevel()
    {
        if(SkyforgeLoader.CurrentProfile!=null)
        {
            float valueHP = MaxHP * SkyforgeLoader.CurrentProfile.Difficulty.EnemyHPMod;
            MaxHP = (int)(valueHP);
            float valueDamage = BaseDamage * SkyforgeLoader.CurrentProfile.Difficulty.EnemyDamageMod;
            BaseDamage = (int)(valueDamage);
            if (BaseDamage == 0)
                BaseDamage = 1;
            var initiammaxDamage = MaxDamage;
            float valueMaxDamage = MaxDamage * SkyforgeLoader.CurrentProfile.Difficulty.EnemyDamageMod;
            MaxDamage = (int)(valueMaxDamage);
            if (MaxDamage == 0 && initiammaxDamage != 0)
                MaxDamage = 1;
            if (CurrentHP != MaxHP)
                CurrentHP = MaxHP;
        }
    }
    #endregion
}
