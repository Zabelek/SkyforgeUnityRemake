using System;

public class HeroStats : CharacterStats
{
    #region Variables
    public float DashChargeMax { get; set; }
    private float _currentDashCharge;
    public float CurrentDashCharge {
        get => _currentDashCharge;
        set => _currentDashCharge = Math.Clamp(value, 0, DashChargeMax);
    }
    public float CompanionChargeMax { get; set; }
    private float _currentCompanionCharge;
    public float CurrentCompanionCharge
    {
        get => _currentCompanionCharge;
        set => _currentCompanionCharge = Math.Clamp(value, 0, CompanionChargeMax);
    }
    public int CompanionDamage { get; set; }
    #endregion

    #region Methods
    public override void ResetBase(CharacterBaseSO baseSO)
    {
        base.ResetBase(baseSO);
        if(baseSO is HeroBaseSO)
        {
            var heroBaseSO = baseSO as HeroBaseSO;
            DashChargeMax = heroBaseSO.DashChargeMax;
            CompanionChargeMax = heroBaseSO.CompanionChargeMax;
            CompanionDamage = heroBaseSO.CompanionDamage;
            if (CurrentDashCharge > DashChargeMax)
                CurrentDashCharge = DashChargeMax;
            if (CurrentCompanionCharge > CompanionChargeMax)
                CurrentCompanionCharge = CompanionChargeMax;
        }
    }
    public override void ModifyAccordingToPerk(PerkSO perk, int modifier)
    {
        base.ModifyAccordingToPerk(perk, modifier);
        if (perk.Stat == PerkSO.StatType.CompanionCharges)
        {
            CompanionChargeMax += (perk.Value) * modifier;
        }
        else if (perk.Stat == PerkSO.StatType.DashCharges)
        {
            DashChargeMax += (perk.Value) * modifier;
        }
        else if (perk.Stat == PerkSO.StatType.CompanionDamage)
        {
            CompanionDamage += (int)(perk.Value) * modifier;
        }
    }
    #endregion
}
