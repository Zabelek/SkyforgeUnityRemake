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
    private void UnlockAbilityFromPerk(string perkID, string abilityName)
    {
        if(abilityName.Length>0)
        {
            if (_hero.GetPerk(perkID)?.Enabled == true)
            {
                _stances[0].GetAbility(abilityName).Unlocked = true;
            }
            else
            {
                _stances[0].GetAbility(abilityName).Unlocked = false;
            }
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
        if (_hero.GetPerk("Base_Berserker_ThrillOfVictory")?.Enabled == true)
        {
            if (_thrillOfVictoryEffect != null && !_hero.GetActiveEffects().FirstOrDefault(e => e.EffectSO.Name == "Thrill of Victory Cast"))
                _hero.AddEffect(_thrillOfVictoryEffect);
        }
    }
    protected override void Hero_OnPerkChange(object sender, HeroBehaviour.PerkChangeEventArgs e)
    {
    }
    public override void ManageAddedPerk(PerkSO perk)
    {
        if (perk.ID == "Base_Berserker_CripplingBlow")
            UnlockAbilityFromPerk("Base_Berserker_CripplingBlow", "Crippling Blow");
        if (perk.ID == "Base_Berserker_TectonicBlast")
            UnlockAbilityFromPerk("Base_Berserker_TectonicBlast", "Tectonic Blast");
        if (perk.ID == "Base_Berserker_ThirstFforBattle")
            UnlockAbilityFromPerk("Base_Berserker_ThirstFforBattle", "Thirst for Battle");
        if (perk.ID == "Base_Berserker_ThunderingRoar")
            UnlockAbilityFromPerk("Base_Berserker_ThunderingRoar", "Thundering Roar");
        if (perk.ID == "Base_Berserker_Gladiator")
            UnlockAbilityFromPerk("Base_Berserker_Gladiator", "Gladiator");
        if(perk.ID == "Base_Berserker_Firestorm")
        {
            if (_hero.GetPerk("Base_Berserker_Firestorm")?.Enabled == true)
            {
                _stances[0].GetAbility("Whirlwind").Ability.UpgradedVersionUnlocked = true;
            }
            else
            {
                _stances[0].GetAbility("Whirlwind").Ability.UpgradedVersionUnlocked = false;
            }
        }
        if (perk.ID == "Base_Berserker_ThrillOfVictory" && _thrillOfVictoryEffect != null)
        {
            if (_hero.GetPerk("Base_Berserker_ThrillOfVictory")?.Enabled == true)
                _hero.AddEffect(_thrillOfVictoryEffect);
            else
                _hero.RemoveEffect(_thrillOfVictoryEffect);
        }
    }
    public override void ManageRemovedPerk(PerkSO perk)
    {
        ManageAddedPerk(perk);
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
