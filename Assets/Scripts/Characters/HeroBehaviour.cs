using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeroBehaviour : CharacterBehaviour
{
    private const float WEAPON_HIDE_DRAW_DURATION_TRESHOLD = 1.5f;

    #region Variables
    [Header("Hero and Class Related Variables")]
    [Tooltip("Create an Empty inside a weapon bone in the character skeleton and drag it here")]
    [SerializeField] private Transform _weaponTransformSlot;
    [Tooltip("Starting Hero Class")]
    [SerializeField] private HeroClassBehaviour _heroClass;
    protected List<LockablePerk> _perks;
    protected List<ChoosablePerkSet> _perkSets;
    public event EventHandler OnPerkChange;
    public WeaponBehaviour EquippedWeapon { get; protected set; }
    public bool CanDash { get; set; }
    //to prevent spamming draw/hide weapon aminations
    protected float _nextDrawStateChangeTimer;
    #endregion

    #region Mono
    protected override void Awake()
    {
        base.Awake();
        _perks = new();
        _perkSets = new();
        if(Globals.Instance != null)
        {
            foreach (var perk in Globals.Instance.RegisteredPerks)
            {
                _perks.Add(new LockablePerk(perk));
            }
            foreach (var perkSet in Globals.Instance.RegisteredPerkSets)
            {
                _perkSets.Add(new ChoosablePerkSet(perkSet));
            }
        }
        CanDash = true;
        _nextDrawStateChangeTimer = 0;
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if(_nextDrawStateChangeTimer>0)
        {
            _nextDrawStateChangeTimer -= Time.fixedDeltaTime;
            if (_nextDrawStateChangeTimer < 0)
                _nextDrawStateChangeTimer = 0;
        }
        //if the weapon draw was locked during combat start, it will trigger once available
        if ( IsInCombat && !CombatStance && _nextDrawStateChangeTimer == 0)
            ChangeWeaponOutState(true);
    }
    #endregion

    #region Methods
    public virtual void UnlockPerk(LockablePerk perk)
    {
        if (GetAllPerks().Contains(perk))
        {
            perk.Unlocked = true;
            EnablePerk(perk);
        }
    }
    public virtual void LockPerk(LockablePerk perk)
    {
        if (GetAllPerks().Contains(perk))
        {
            perk.Unlocked = false;
            DisablePerk(perk);
        }
    }
    public virtual void EnablePerk(LockablePerk perk)
    {
        if (GetAllPerks().Contains(perk))
        {
            perk.Enable();
            ManagePerkChange();
        }
    }
    public virtual void DisablePerk(LockablePerk perk)
    {
        if (GetAllPerks().Contains(perk))
        {
            perk.Disable();
            ManagePerkChange();
        }
    }
    protected virtual void ChangeClass(HeroClassBehaviour nextClass)
    {
        //Not fully tested yet
        if(_heroClass !=null)
        {
            _heroClass.SetHero(null);
        }
        ManagePerkChange();
        nextClass.SetHero(this);
        _heroClass = nextClass;
        if(_animationBehaviour!=null)
        {
            if (_heroClass != null && _heroClass.AnimatorController != null)
                _animationBehaviour.SetController(_heroClass.AnimatorController);
            else
                _animationBehaviour.SetController(DefaultAnimatorController);
            _animationBehaviour.TriggerAnimation("Init");
        }
        OnPerkChange?.Invoke(this, EventArgs.Empty);
    }
    private void ManagePerkChange()
    {
        Stats.Reset(CharacterSO);
        foreach (var perk in GetAllPerks())
        {
            if ((perk.Perk.Class == _heroClass.HeroClassSO || perk.Perk.Class == null) && perk.Enabled && perk.Unlocked)
            {
                if(perk.Perk.Functional)
                {
                    _heroClass.ManageAddedPerk(perk.Perk);
                }
                else
                {
                    Stats.ModifyAccordingToPerk(perk.Perk);
                }
            }
        }
        OnPerkChange?.Invoke(this, EventArgs.Empty);
    }
    public override int GetEffectiveDamage()
    {
        if(EquippedWeapon!= null)
            return (int)((Stats.BaseDamage + EquippedWeapon.WeaponSO.GetDamage()) * GetDamageModifiers());
        else
            return (int)(Stats.BaseDamage * GetDamageModifiers());
    }
    public virtual void EquipWeapon(WeaponBehaviour weapon)
    {
        if (EquippedWeapon != null)
        {
            EquippedWeapon.Unequip(this);         
        }
        EquippedWeapon = weapon;       
        EquippedWeapon.Equip(this, _weaponTransformSlot);
    }
    public virtual void ChangeWeaponOutState(object sender, EventArgs e)
    {
        //the second check is because the character can't hide weapon in combat
        if ((Globals.Instance.IsMenuOpen == false || sender == null) && _nextDrawStateChangeTimer==0 && CanAct())
        {
            _nextDrawStateChangeTimer = WEAPON_HIDE_DRAW_DURATION_TRESHOLD;
            if (!CombatStance)
            {
                CombatStance = true;
                EquippedWeapon?.AnimateWeaponDraw();
                if (_animationBehaviour is HeroAnimationBehaviour)
                    ((HeroAnimationBehaviour)_animationBehaviour).ScheduleWeaponDraw();
            }
            else if (!IsInCombat)
            {
                CombatStance = false;
                EquippedWeapon?.AnimateWeaponHide();
                if(_animationBehaviour is HeroAnimationBehaviour)
                    ((HeroAnimationBehaviour)_animationBehaviour).ScheduleWeaponHide();
            }
        }
    }
    public virtual void ChangeWeaponOutState(bool weaponOut)
    {
        //force draw/hide variant, ignore stun effects
        if (_nextDrawStateChangeTimer == 0)
        {
            _nextDrawStateChangeTimer = WEAPON_HIDE_DRAW_DURATION_TRESHOLD;
            if (weaponOut)
            {
                CombatStance = true;
                EquippedWeapon?.AnimateWeaponDraw();
                if (_animationBehaviour is HeroAnimationBehaviour)
                    ((HeroAnimationBehaviour)_animationBehaviour).ScheduleWeaponDraw();
            }
            else
            {
                CombatStance = false;
                EquippedWeapon?.AnimateWeaponHide();
                if (_animationBehaviour is HeroAnimationBehaviour)
                    ((HeroAnimationBehaviour)_animationBehaviour).ScheduleWeaponHide();
            }
        }
    }
    public override void EnterCombat(CharacterBehaviour character, bool fightProvokedByGroup)
    {
        base.EnterCombat(character, fightProvokedByGroup);
        if (!character.IsDead)
        {
            if (IsInCombat && !CombatStance)
            {
                ChangeWeaponOutState(true);
            }
        }
    }
    public override void LeaveCombat()
    {
        //to hide weapon after combat
        base.LeaveCombat();
        StartCoroutine(DelayedWeaponHide());
    }
    public override void Kill(CharacterBehaviour killer)
    {
        base.Kill(killer);
        CanDash = false;
    }
    public override void EndCutscene()
    {
        base.EndCutscene();
        if(_heroClass?.AnimatorController != null)
        {
            _animationBehaviour.SetController(_heroClass.AnimatorController);
        }
        if (!IsInCombat && CombatStance)
        {
            ChangeWeaponOutState(false);
        }
    }
    public override void CancelAllAbilities()
    {
        base.CancelAllAbilities();
        if (_heroClass != null)
            _heroClass.CancelAllAbilities();
    }
    public void DrawWeaponForCutscene(bool animate)
    {
        if(EquippedWeapon != null)
        {
            if (animate)
            {
                EquippedWeapon.AnimateWeaponDraw();
            }
            else
            {
                EquippedWeapon.SetWeaponDraw();
            }
        }
    }
    public void EndDrawingWeaponForCutscene(bool animate)
    {
        if (EquippedWeapon != null && CombatStance == false)
        {
            if (animate)
            {
                EquippedWeapon.AnimateWeaponHide();
            }
            else
            {
                EquippedWeapon.SetWeaponHide();
            }
        }
    }
    protected IEnumerator DelayedWeaponHide()
    {
        yield return new WaitForSeconds(1);
        if(!Globals.Instance.IsCutscenePlaying && !IsInCombat)
            ChangeWeaponOutState(false);
    }
    public override void SetCanAct(bool canAct, bool ownAbilityDriven)
    {
        base.SetCanAct(canAct, ownAbilityDriven);
        if(canAct == false && ownAbilityDriven == false)
        {
            CancelAllAbilities();
        }
    }
    #endregion

    #region StatGetters
    public HeroClassBehaviour GetHeroClass()
    {
        return _heroClass;
    }
    public LockablePerk GetPerk(string name)
    {
        var ret = _perks.FirstOrDefault(p => p.Perk.Name == name);
        if(ret == null && _perkSets.Any())
        {
            ret = _perkSets.FirstOrDefault(s => s.Perks.Any(p => p.Perk.Name == name)).Perks.FirstOrDefault(p => p.Perk.Name == name);
        }
        return ret;
    }
    public LockablePerk GetPerk(PerkSO perkSO)
    {
        var ret = _perks.FirstOrDefault(p => p.Perk == perkSO);
        if (ret == null)
        {
            ret = _perkSets.FirstOrDefault(s => s.Perks.Any(p => p.Perk == perkSO)).Perks.FirstOrDefault(p => p.Perk == perkSO);
        }
        return ret;
    }
    public List<LockablePerk> GetAllPerks()
    {
        var ret = new List<LockablePerk>();
        foreach (var perk in _perks)
            ret.Add(perk);
        foreach(var perkSet in _perkSets)
            foreach(var perk in perkSet.Perks)
                ret.Add(perk);
        return ret;
    }
    public override float GetMovementSpeedModifiers()
    {
        if (CombatStance)
            return base.GetMovementSpeedModifiers() * GetHeroClass().HeroClassSO.CombatMovementSpeedMultiplier;
        else
            return base.GetMovementSpeedModifiers();
    }
    #endregion
}