using System;

public class CharacterStats
{
    #region Variables
    private int _maxHP;
    public int MaxHP { 
        get => (int)((_maxHP + GearStats.HealthBonus) * (1 + GearStats.HealthPercentBonus));
        private set => _maxHP = value;
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
    private float _movementSpeed;
    public float MovementSpeed
    {
        get => _movementSpeed * (1+GearStats.MovementSpeedPercentBonus);
        private set => _movementSpeed = value;
    }
    private float _attackSpeed;
    public float AttackSpeed
    {
        get => _attackSpeed + GearStats.AttackSpeedBonus;
        private set => _attackSpeed = value;
    }
    private float _criticalChance;
    public float CriticalChance
    {
        get => _criticalChance + GearStats.CriticalChanceBonus;
        private set => _criticalChance = value;
    }
    private int _baseDamage;
    public int BaseDamage 
    { 
        get => (int)((_baseDamage + GearStats.DamageBonus) * (1 + GearStats.DamagePercentBonus));
        private set => _baseDamage = value; 
    }
    //max bonus value that the character can inflict. Every hit, the character inflicts a random damage betwen Base damage and Base damage + Max damage;
    public int _maxDamage;
    public int MaxDamage {
        get => (int)(_maxDamage * (1 + GearStats.DamagePercentBonus + GearStats.MaxDamagePercentBonus));
        private set => _maxDamage = value;
    }
    public int CombatManaRegen { get; set; }
    //healing percent of the character each time they deal damage. 1 vampirism means that they will heal by 100% of dealt damage
    private float _vampirism;
    public float Vampirism {
        get => (_vampirism + GearStats.VampirismBonus);
        private set => _vampirism = value;
    }
    //Percent damage reduction of the character
    public float Defense { get; set; }
    //percent negative effect duration reduction of the character
    private float _stability;
    public float Stability
    {
        get => _stability + GearStats.StabilityBonus;
        private set => _stability = value;
    }
    public GearBonus GearStats { get; private set; }
    #endregion

    #region Constructors
    public CharacterStats()
    {
        GearStats = new();
        GearStats.OnStatsChangedEvent += GearStatsChanged;
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
    public virtual void ModifyAccordingToPerk(PerkSO perk, int modifier)
    {
        if (perk.Stat == PerkSO.StatType.AttackSpeed)
        {
            _attackSpeed += perk.Value * modifier;
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
            _criticalChance += perk.Value * modifier;
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
            _vampirism += perk.Value * modifier;
        }
        else if (perk.Stat == PerkSO.StatType.Defense)
        {
            Defense += perk.Value * modifier;
        }
        else if (perk.Stat == PerkSO.StatType.Stability)
        {
            _stability += perk.Value * modifier;
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
