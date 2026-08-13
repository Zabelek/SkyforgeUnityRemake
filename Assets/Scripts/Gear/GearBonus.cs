using System;

public class GearBonus
{
    #region Variables
    public event EventHandler OnStatsChangedEvernt;
    public int HealthBonus;
    public float HealthPercentBonus;
    public int DamageBonus;
    public float DamagePercentBonus;
    public float Armor;
    public float CriticalDamageBonus;
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
        CriticalDamageBonus = 0;
        OnStatsChangedEvernt?.Invoke(this, EventArgs.Empty);
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
        }
        OnStatsChangedEvernt?.Invoke(this, EventArgs.Empty);
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
        }
        OnStatsChangedEvernt?.Invoke(this, EventArgs.Empty);
    }
    #endregion
}
