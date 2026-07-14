using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeroClassBehaviour : MonoBehaviour
{
    public enum ComboState
    {
        N, L, LL, LLL, LLLL, R, LR, LLR, LLLR
    }
    #region Variables
    [Header("Class Related Variables")]
    [Tooltip("Hero Class Scriptable Object with basic information about the class")]
    public HeroClassSO HeroClassSO;
    protected HeroBehaviour _hero;
    [Tooltip("Animator Controller that will be switched in Animation behaviour when the character changes the class. Class-specific animations won't work without it")]
    public RuntimeAnimatorController AnimatorController;
    //stances and abilities
    protected AbilityBehaviour _blockingAbility;
    [HideInInspector] public List<AbilityBehaviour> CurrentlyUpdatedAbilities, RecentlyFinishedAbilities;
    [Tooltip("List of all stances that the hero can take with this class")]
    [SerializeField] protected List<HeroClassStance> _stances;
    [HideInInspector] public HeroClassStance CurrentStance;
    [Tooltip("Dash to perform while having this class picked")]
    [SerializeField] protected AbilityBehaviour _dash;
    [Tooltip("Finisher ability of the specific class (can be activated while selecting enemy with health below a certain treshold)")]
    [SerializeField] protected FinisherAbility _finisherAbility;
    [HideInInspector] public ComboState CurrentlyPerformedComboState;
    //for abilities that are charged before use
    public float AbilityCharge { get; set; }
    public float AbilityChargeMax { get; set; }
    //if charging ability may have multiple outcoms, it's save into this property
    public float ChargingReturn { get; set; }
    protected float _manaRegenTimer = 0.5f;
    #endregion

    #region Mono
    protected virtual void Awake()
    {
        CurrentlyUpdatedAbilities = new();
        RecentlyFinishedAbilities = new();
        if(_finisherAbility != null)
            _finisherAbility.Init();
        if (_dash != null)
            _dash.Init();
    }
    public virtual void FixedUpdate()
    {
        if (_hero != null)
        {
            foreach(var ability in RecentlyFinishedAbilities)
            {
                CurrentlyUpdatedAbilities.Remove(ability);
                ability.Finishing = false;
            }
            RecentlyFinishedAbilities.Clear();
            foreach (var ability in CurrentlyUpdatedAbilities)
            {
                ability.UpdateAbility(_hero, this);
            }
            ManageMana();
            ManageAbilityCooldowns();
        }
    }
    #endregion

    #region Methods
    public void SetBlockingAbility(AbilityBehaviour ability)
    {
        _blockingAbility = ability;
        if (ability == null)
            _blockingAbility = null;
    }
    public void ClearCurrentBlockingAbility()
    {
        if(_blockingAbility?.AbilityCurrentlyLockingControl == true)
        {
            _blockingAbility.Interrupt(_hero);
        }
        _blockingAbility = null;
    }
    public virtual ComboState TryPerformNextComboState(bool isR)
    {
        if (CurrentStance != null && CurrentStance.GetComboToPerform(GetNextState(isR), out AbilityBehaviour ability) == true)
        {
            if (_blockingAbility == null && ability.CheckPerformAvailability(_hero))
            {
                if (CurrentlyUpdatedAbilities.Any(ab => ab.GetType() == ability.GetType()))
                {
                    //so that the same ability can't be updated twice in some cases
                    ability.Interrupt(_hero);
                }
                ability.LaunchAbility(_hero);
                return ability.AbilitySO.StateType;
            }
        }
        return ComboState.N;
    }
    public virtual bool TryPerformAbility(int abilityNumber)
    {
        if (CurrentStance != null && CurrentStance.GetAbilityToPerform(TranslateAbilityNumbers(abilityNumber), out AbilityBehaviour ability) == true)
        {
            if (_blockingAbility == null && ability.CheckPerformAvailability(_hero) && !CurrentlyUpdatedAbilities.Any(ab => ab.GetType() == ability.GetType()))
            {
                ability.LaunchAbility(_hero);
                return true;
            }
        }
        return false;
    }
    public virtual bool TryPerformEscapeAbility()
    {
        if (CurrentStance != null && CurrentStance.TryFindEscapeAbility(out EscapeAbilityBehaviour ability) == true)
        {
            if (_blockingAbility == null && ability.CheckPerformAvailabilityEscape(_hero) && !CurrentlyUpdatedAbilities.Any(ab => ab.GetType() == ability.GetType()))
            {
                ability.LaunchAbilityEscape(_hero);
                return true;
            }
        }
        return false;
    }
    public virtual bool TryPerformEscapeAbility(int abilityNumber)
    {
        //made in case there are more than one escape ability in the stance. It then picks specific one unlike the normal method
        if (CurrentStance != null && CurrentStance.GetAbilityToPerform(TranslateAbilityNumbers(abilityNumber), out AbilityBehaviour ability) == true && ability is EscapeAbilityBehaviour)
        {
            if (_blockingAbility == null && ((EscapeAbilityBehaviour)ability).CheckPerformAvailabilityEscape(_hero) && !CurrentlyUpdatedAbilities.Any(ab => ab.GetType() == ability.GetType()))
            {
                ((EscapeAbilityBehaviour)ability).LaunchAbilityEscape(_hero);
                return true;
            }
        }
        return false;
    }
    public bool TryPerformFinisher()
    {
        if (_blockingAbility == null && _finisherAbility.CheckPerformAvailability(_hero))
        {
            if(_hero is PlayerBehaviour)
                _finisherAbility.Casuality = ((PlayerBehaviour)_hero).SelectedCharacter;
            _finisherAbility.LaunchAbility(_hero);
            return true;
        }
        return false;
    }
    public virtual bool TryPerformDash()
    {
        if (_dash != null && _dash.CheckPerformAvailability(_hero))
        {
            foreach (var ability in CurrentlyUpdatedAbilities)
            {
                ability.Interrupt(_hero);
            }
            _dash.LaunchAbility(_hero);
            return true;
        }
        return false;
    }
    public virtual ComboState GetNextState(bool isR)
    {
        var ret = CurrentlyPerformedComboState;
        if(isR)
        {
            if (ret == ComboState.N)
                return ComboState.R;
            else if (ret == ComboState.L)
                return ComboState.LR;
            else if (ret == ComboState.LL)
                return ComboState.LLR;
            else if (ret == ComboState.LLL)
                return ComboState.LLLR;
            else return ComboState.N;
        }
        else
        {
            if (ret == ComboState.N)
                return ComboState.L;
            else if (ret == ComboState.L)
                return ComboState.LL;
            else if (ret == ComboState.LL)
                return ComboState.LLL;
            else if (ret == ComboState.LLL)
                return ComboState.LLLL;
            else return ComboState.N;
        }

    }
    private void ManageAbilityCooldowns()
    {
        foreach(var stance in _stances)
        {
            stance.ManageAbilityCooldowns();
        }
        if(_finisherAbility != null)
        {
            _finisherAbility.UpdateCooldown();
        }
        if(_dash != null)
        {
            _dash.UpdateCooldown();
        }
    }
    public virtual void ManageMana()
    {
        if (_hero.IsInCombat)
        {
            if (_hero.Stats.CurrentMana < _hero.Stats.MaxMana)
            {
                _manaRegenTimer -= Time.fixedDeltaTime;
                if (_manaRegenTimer <=0)
                {
                    _manaRegenTimer = 0.5f;
                    _hero.Stats.CurrentMana += _hero.Stats.CombatManaRegen;
                }
            }
        }
        else
        {
            if(_hero.Stats.CurrentMana < _hero.Stats.MaxMana)
            {
                _hero.Stats.CurrentMana = _hero.Stats.MaxMana;
            }
        }
    }
    public virtual void ManageAddedPerk(PerkSO perk)
    {
        //for inheriting classes
    }
    public virtual void ManageRemovedPerk(PerkSO perk)
    {
        //for inheriting classes
    }
    public virtual void SetHero(HeroBehaviour player)
    {
        if(_hero!= null)
        {
            _hero.OnDeath -= Hero_OnDeath;
            _hero.OnPerkChange -= Hero_OnPerkChange;
        }
        _hero = player;
        if (_hero != null)
        {
            if (_hero is PlayerBehaviour)
            {
                foreach (var stance in _stances)
                {
                    stance.GUIPanel.SetPlayer(((PlayerBehaviour)_hero));
                }
            }
            _hero.OnDeath += Hero_OnDeath;
            _hero.OnPerkChange += Hero_OnPerkChange;
            Hero_OnPerkChange(this, null);
        }
        SetStance(0);
    }
    public void SetStance(int stanceNumber)
    {
        if(_stances.Count() > stanceNumber)
        {
            CurrentStance = _stances[stanceNumber];
            Globals.Instance?.GameplayControls.SetAbilitiesPanel(CurrentStance.GUIPanel);
        }
    }
    public virtual void GetOpportunityAbilities(out AbilityBehaviour leftAbility, out AbilityBehaviour RightAbility, out string leftKey, out string rightKey)
    {
        leftAbility = null;
        RightAbility = null;
        leftKey = "";
        rightKey = "";
        if(CurrentStance != null)
        {
            CurrentStance.GetOpportunityAbilities(out leftAbility, out RightAbility, out leftKey, out rightKey);
        }
        if (CurrentStance.TryFindEscapeAbility(out var escapeAbility) == true && escapeAbility.CheckPerformAvailabilityEscape(_hero))
        {
            RightAbility = escapeAbility;
            rightKey = "E";
            return;
        }
        if (_finisherAbility != null && _finisherAbility.CheckPerformAvailability(_hero))
        {
            RightAbility = _finisherAbility;
            rightKey = "E";
            return;
        }
    }
    public AbilityBehaviour GetAbilityFromAnyStance(string abilityName)
    {
        foreach(var stance in _stances)
        {
            var ability = stance.GetAbility(abilityName);
            if (ability != null)
            {
                return ability.Ability;
            }
        }
        return null;
    }
    public void CancelAllAbilities()
    {
        foreach (var ability in CurrentlyUpdatedAbilities)
        {
            ability.Interrupt(_hero);
        }
        ClearCurrentBlockingAbility();
    }
    //if the class doesn't have full 9 abilities, and will try to reach, say, ultimate ability, it would have to change the input number first
    public virtual int TranslateAbilityNumbers(int inputNumber)
    {
        return inputNumber;
    }
    #endregion

    #region EventHandlers
    private void Hero_OnDeath(object sender, EventArgs e)
    {
        CancelAllAbilities();
    }
    public virtual void Hero_OnMove()
    {
        //for inheriting classes
    }
    protected virtual void Hero_OnPerkChange(object sender, HeroBehaviour.PerkChangeEventArgs e)
    {
        //for inheriting classes
    }
    public virtual void RightReleasePerformed()
    {
        //for inheriting classes
    }
    #endregion
}
