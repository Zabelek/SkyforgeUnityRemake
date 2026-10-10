using System;

public class HeroStats : CharacterStats
{
    #region Variables
    private float _dashChargeMax;
    public float DashChargeMax
    {
        get => _dashChargeMax * (1 + GearStats.DashChargesBonusPercent);
        private set => _dashChargeMax = value;
    }
    private float _currentDashCharge;
    public float CurrentDashCharge {
        get => _currentDashCharge;
        set => _currentDashCharge = Math.Clamp(value, 0, DashChargeMax);
    }
    private float _companionChargeMax;
    public float CompanionChargeMax
    {
        get => _companionChargeMax * (1 + GearStats.CompanionChargesBonusPercent);
        private set => _companionChargeMax = value;
    }
    private float _currentCompanionCharge;
    public float CurrentCompanionCharge
    {
        get => _currentCompanionCharge;
        set => _currentCompanionCharge = Math.Clamp(value, 0, CompanionChargeMax);
    }
    private int _companionDamage;
    public int CompanionDamage
    {
        get => (int)(_companionDamage * (1 + GearStats.CompanionDamagePercentBonus));
        private set => _companionDamage = value;
    }
    private float _moneyReceivedBonus;
    public float MoneyReceivedBonus
    {
        get => _moneyReceivedBonus + GearStats.MoneyReceivedBonus;
        private set => _moneyReceivedBonus = value;
    }
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
