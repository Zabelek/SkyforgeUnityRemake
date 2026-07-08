using System;
using System.Linq;
using UnityEngine;

public class BerserkerClassBehaviour : HeroClassBehaviour
{
    #region Variables
    [Header("Berserker Related Variables")]
    [Tooltip("Reference of Thrill of Victory to add to the player if they have a proper perk enabled")]
    [SerializeField] private GameplayEffectBehaviour _thrillOfVictoryEffect;
    #endregion
    #region Mono
    protected override void Awake()
    {
        base.Awake();
        CurrentStance = _stances[0];
    }
    #endregion

    #region Methods    
    private void UnlockAbilityFromPerk(string perkName)
    {
        if (_hero.GetPerk(perkName)?.Enabled == true)
        {
            _stances[0].GetAbility(perkName).Unlocked = true;
        }
        else
        {
            _stances[0].GetAbility(perkName).Unlocked = false;
        }
    }
    public override void ManageMana()
    {
        if (_hero.IsInCombat)
        {
            if (_hero.Stats.CurrentMana < _hero.Stats.MaxMana)
            {
                _manaRegenTimer -= Time.fixedDeltaTime;
                if (_manaRegenTimer <= 0)
                {
                    _manaRegenTimer = 1f;
                    _hero.Stats.CurrentMana += _hero.GetEffectiveCombatManaRegen();
                }
            }
        }
        else
        {
            if (_hero.Stats.CurrentMana < (int)(_hero.Stats.MaxMana / 2f))
            {
                _hero.Stats.CurrentMana = (int)(_hero.Stats.MaxMana / 2f);
            }
            else if(_hero.Stats.CurrentMana > (int)(_hero.Stats.MaxMana / 2f))
            {
                _manaRegenTimer -= Time.fixedDeltaTime;
                if (_manaRegenTimer <= 0)
                {
                    _manaRegenTimer = 1f;
                    _hero.Stats.CurrentMana -= 1;
                }
            }
        }
    }
    public override void SetHero(HeroBehaviour hero)
    {
        //to make sure listener is removed from previous hero, if exist
        if (_hero != null)
        {
            _hero.OnCombatEndEvent -= Hero_OnCombatEnded;
        }
        base.SetHero(hero);
        if (_hero != null)
        {
            _hero.OnCombatEndEvent += Hero_OnCombatEnded;
        }
    }
    public override void GetOpportunityAbilities(out AbilityBehaviour leftAbility, out AbilityBehaviour rightAbility,
    out string leftKey, out string rightKey)
    {
        base.GetOpportunityAbilities(out leftAbility, out rightAbility, out leftKey, out rightKey);
        AbilityBehaviour cripplingBlow = CurrentStance.GetAbility("Crippling Blow")?.Ability;
        if (cripplingBlow != null && cripplingBlow.CheckPerformAvailability(_hero)
            && _hero.GetActiveEffects().FirstOrDefault(e => e.EffectSO.Name == "Burning Chain") != null)
        {
            leftAbility = cripplingBlow;
            leftKey = "3";
        }
    }
    public override int TranslateAbilityNumbers(int inputNumber)
    {
        if (inputNumber == 8)
            return 6;
        else if (inputNumber == 6 || inputNumber == 7)
            return 100;
        else
            return inputNumber;
    }
    #endregion

    #region EventHandlers
    public override void Hero_OnMove()
    {
        CurrentlyPerformedComboState = ComboState.N;
    }
    private void Hero_OnCombatEnded(object sender, EventArgs e)
    {
        if (_hero.GetPerk("Thrill Of Victory")?.Enabled == true)
        {
            if (_thrillOfVictoryEffect != null)
                _hero.AddEffect(_thrillOfVictoryEffect);
        }
    }
    protected override void Hero_OnPerkChange(object sender, EventArgs e)
    {
        base.Hero_OnPerkChange(sender, e);
        UnlockAbilityFromPerk("Crippling Blow");
        UnlockAbilityFromPerk("Tectonic Blast");
        UnlockAbilityFromPerk("Thirst for Battle");
        UnlockAbilityFromPerk("Thundering Roar");
        UnlockAbilityFromPerk("Gladiator");
        if (_hero.GetPerk("Firestorm")?.Enabled == true)
        {
            _stances[0].GetAbility("Whirlwind").Ability.UpgradedVersionUnlocked = true;
        }
        else
        {
            _stances[0].GetAbility("Whirlwind").Ability.UpgradedVersionUnlocked = false;
        }
        if (_thrillOfVictoryEffect != null)
        {
            if (_hero.GetPerk("Thrill Of Victory")?.Enabled == true)
                _hero.AddEffect(_thrillOfVictoryEffect);
            else
                _hero.RemoveEffect(_thrillOfVictoryEffect);
        }
    }
    public override void RightReleasePerformed()
    {
        if (CurrentlyPerformedComboState == HeroClassBehaviour.ComboState.R && AbilityCharge < AbilityChargeMax)
        {
            AbilityChargeMax = 0;
        }
    }
    #endregion
}
