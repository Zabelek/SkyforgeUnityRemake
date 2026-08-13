using System;
using UnityEngine;

public class CharacterStats
{
    #region Variables
    private int _maxHP;
    public int MaxHP { 
        get => (int)((_maxHP + GearStats.HealthBonus) * (1 + GearStats.HealthPercentBonus));
        set => _maxHP = value;
    }
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
    private int _baseDamage;
    public int BaseDamage 
    { 
        get => (int)((_baseDamage + GearStats.DamageBonus) * (1 + GearStats.DamagePercentBonus));
        set => _baseDamage = value; 
    }
    //max bonus value that the character can inflict. Every hit, the character inflicts a random damage betwen Base damage and Base damage + Max damage;
    public int _maxDamage;
    public int MaxDamage {
        get => (int)(_maxDamage * (1 + GearStats.DamagePercentBonus));
        set => _maxDamage = value;
    }
    public int CombatManaRegen { get; set; }
    //healing percent of the character each time they deal damage. 1 vampirism means that they will heal by 100% of dealt damage
    public float Vampirism { get; set; }
    //Percent damage reduction of the character
    public float Defense { get; set; }
    //percent negative effect duration reduction of the character
    public float Stability { get; set; }
    public GearBonus GearStats { get; private set; }
    #endregion

    #region Constructors
    public CharacterStats()
    {
        GearStats = new();
        GearStats.OnStatsChangedEvernt += GearStatsChanged;
    }
    #endregion

    #region Methods
    public virtual void ResetBase(CharacterBaseSO baseSO)
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
        CurrentHP = MaxHP;
        if (CurrentMana > MaxMana)
            CurrentMana = MaxMana;
    }
    public virtual void ResetGear()
    {
        GearStats.Reset();
    }
    public virtual void ModifyAccordingToPerk(PerkSO perk, int modifier)
    {
        if (perk.Stat == PerkSO.StatType.AttackSpeed)
        {
            AttackSpeed += perk.Value * modifier;
        }
        else if (perk.Stat == PerkSO.StatType.BaseDamage)
        {
            _baseDamage += (int)(perk.Value) * modifier;
        }
        else if (perk.Stat == PerkSO.StatType.MaxDamage)
        {
            _maxDamage += (int)(perk.Value) * modifier;
        }
        else if (perk.Stat == PerkSO.StatType.CriticalChance)
        {
            CriticalChance += perk.Value * modifier;
        }
        else if (perk.Stat == PerkSO.StatType.MaxHP)
        {
            _maxHP += (int)(perk.Value) * modifier;
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
    public virtual void ModifyAccordingToDifficultyLevel()
    {
        if(SkyforgeLoader.CurrentProfile!=null)
        {
            float valueHP = _maxHP * SkyforgeLoader.CurrentProfile.Difficulty.EnemyHPMod;
            MaxHP = (int)(valueHP);
            float valueDamage = _baseDamage * SkyforgeLoader.CurrentProfile.Difficulty.EnemyDamageMod;
            _baseDamage = (int)(valueDamage);
            if (_baseDamage == 0)
                _baseDamage = 1;
            var initiammaxDamage = _maxDamage;
            float valueMaxDamage = _maxDamage * SkyforgeLoader.CurrentProfile.Difficulty.EnemyDamageMod;
            _maxDamage = (int)(valueMaxDamage);
            if (_maxDamage == 0 && initiammaxDamage != 0)
                _maxDamage = 1;
            if (CurrentHP != MaxHP)
                CurrentHP = MaxHP;
        }
    }
    #endregion

    #region EventHandlers
    private void GearStatsChanged(object sender, EventArgs e)
    {
        if (CurrentHP > MaxHP)
            CurrentHP = MaxHP;
    }
    #endregion
}
