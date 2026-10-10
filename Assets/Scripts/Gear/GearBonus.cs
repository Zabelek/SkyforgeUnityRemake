using System;

public class GearBonus
{
    #region Variables
    public event EventHandler OnStatsChangedEvent;
    public int HealthBonus;
    public float HealthPercentBonus;
    public int DamageBonus;
    public float DamagePercentBonus;
    public float Armor;
    public float CriticalDamagePercentBonus;

    public float CompanionDamagePercentBonus;
    public float CriticalChanceBonus;
    public float VampirismBonus;
    public float MaxDamagePercentBonus;
    public float AttackSpeedBonus;
    public float StabilityBonus;
    public float ArmorPercentBonus;
    public float DashChargesBonusPercent;
    public float CompanionChargesBonusPercent;
    public float MovementSpeedPercentBonus;
    public float MoneyReceivedBonus;
    #endregion

    #region Constructors
    public GearBonus()
    {
        Reset();
    }
    #endregion

    #region Methods
    public void Reset()
    {
        HealthBonus = 0;
        HealthPercentBonus = 0;
        DamageBonus = 0;
        DamagePercentBonus = 0;
        Armor = 0;
        CriticalDamagePercentBonus = 0;
        CompanionDamagePercentBonus = 0;
        CriticalChanceBonus = 0;
        VampirismBonus = 0;
        MaxDamagePercentBonus = 0;
        AttackSpeedBonus = 0;
        StabilityBonus = 0;
        ArmorPercentBonus = 0;
        DashChargesBonusPercent = 0;
        CompanionChargesBonusPercent = 0;
        MovementSpeedPercentBonus = 0;
        MoneyReceivedBonus = 0;
        OnStatsChangedEvent?.Invoke(this, EventArgs.Empty);
    }
    public void Add(GearBonus bonus)
    {
        if(bonus!= null)
        {
            HealthBonus += bonus.HealthBonus;
            HealthPercentBonus += bonus.HealthPercentBonus;
            DamageBonus += bonus.DamageBonus;
            DamagePercentBonus += bonus.DamagePercentBonus;
            Armor += bonus.Armor;

            CriticalDamagePercentBonus += bonus.CriticalDamagePercentBonus;
            CompanionDamagePercentBonus += bonus.CompanionDamagePercentBonus;
            CriticalChanceBonus += bonus.CriticalChanceBonus;
            VampirismBonus += bonus.VampirismBonus;
            MaxDamagePercentBonus += bonus.MaxDamagePercentBonus;
            AttackSpeedBonus += bonus.AttackSpeedBonus;
            StabilityBonus += bonus.StabilityBonus;
            ArmorPercentBonus += bonus.ArmorPercentBonus;
            DashChargesBonusPercent += bonus.DashChargesBonusPercent;
            CompanionChargesBonusPercent += bonus.CompanionChargesBonusPercent;
            MovementSpeedPercentBonus += bonus.MovementSpeedPercentBonus;
            MoneyReceivedBonus += bonus.MoneyReceivedBonus;
        }
        OnStatsChangedEvent?.Invoke(this, EventArgs.Empty);
    }
    public void Remove(GearBonus bonus)
    {
        if (bonus != null)
        {
            HealthBonus -= bonus.HealthBonus;
            HealthPercentBonus -= bonus.HealthPercentBonus;
            DamageBonus -= bonus.DamageBonus;
            DamagePercentBonus -= bonus.DamagePercentBonus;
            Armor -= bonus.Armor;

            CriticalDamagePercentBonus -= bonus.CriticalDamagePercentBonus;
            CompanionDamagePercentBonus -= bonus.CompanionDamagePercentBonus;
            CriticalChanceBonus -= bonus.CriticalChanceBonus;
            VampirismBonus -= bonus.VampirismBonus;
            MaxDamagePercentBonus -= bonus.MaxDamagePercentBonus;
            AttackSpeedBonus -= bonus.AttackSpeedBonus;
            StabilityBonus -= bonus.StabilityBonus;
            ArmorPercentBonus -= bonus.ArmorPercentBonus;
            DashChargesBonusPercent -= bonus.DashChargesBonusPercent;
            CompanionChargesBonusPercent -= bonus.CompanionChargesBonusPercent;
            MovementSpeedPercentBonus -= bonus.MovementSpeedPercentBonus;
            MoneyReceivedBonus -= bonus.MoneyReceivedBonus;
        }
        OnStatsChangedEvent?.Invoke(this, EventArgs.Empty);
    }
    #endregion
}
