using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeroBehaviour : CharacterBehaviour
{
    private const float WEAPON_HIDE_DRAW_DURATION_TRESHOLD = 1.5f;

    public class PerkChangeEventArgs : EventArgs
    {
        public PerkSO PerkSO;
        public bool Enabled;
    }

    #region Variables
    [Header("Hero and Class Related Variables")]
    [Tooltip("Create an Empty inside a weapon bone in the character skeleton and drag it here")]
    [SerializeField] private Transform _weaponTransformSlot;
    [Tooltip("Starting Hero Class")]
    [SerializeField] private HeroClassBehaviour _heroClass;
    protected List<LockablePerk> _perks;
    protected List<ChoosablePerkSet> _perkSets;
    public event EventHandler<PerkChangeEventArgs> OnPerkChange;
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
    public virtual void AddPerk(PerkSO perkSO, bool autoEnable, bool saveToProfile = false)
    {
        if (!GetAllPerks().Any(p => p.Perk == perkSO))
        {
            var perk = new LockablePerk(perkSO);
            _perks.Add(perk);
            var perkSet = _perkSets.FirstOrDefault(perSet => perSet.PerkSetSO.Perks.Contains(perkSO));
            if (perkSet != null)
            {
                perkSet.AddPerk(perk);
            }
            if(autoEnable)
            {
                perk.Enable();
                ManagePerkChange(perk);
            }
        }
    }
    public virtual void RemovePerk(PerkSO perkSO, bool saveToProfile = false)
    {
        var perk = GetAllPerks().FirstOrDefault(p => p.Perk == perkSO);
        if (perk != null)
        {
            _perks.Remove(perk);
            var perkSet = _perkSets.FirstOrDefault(perSet => perSet.PerkSetSO.Perks.Contains(perkSO));
            if (perkSet != null)
            {
                perkSet.RemovePerk(perk);
            }
            ManagePerkRemoval(perk);
        }
    }
    public virtual void EnablePerk(PerkSO perkSO)
    {
        var perk = GetAllPerks().FirstOrDefault(p => p.Perk == perkSO);
        if (perk != null)
        {
            perk.Enable();
            ManagePerkChange(perk);
        }
    }
    public virtual void DisablePerk(PerkSO perkSO)
    {
        var perk = GetAllPerks().FirstOrDefault(p => p.Perk == perkSO);
        if (GetAllPerks().Contains(perk))
        {
            perk.Disable();
            ManagePerkChange(perk);
        }
    }
    protected virtual void ChangeClass(HeroClassBehaviour nextClass)
    {
        //Not fully tested yet
        if(_heroClass !=null)
        {
            _heroClass.SetHero(null);
        }
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
        SyncPerks(false);
    }
    protected virtual void ResetPerkEffects()
    {
        Stats.Reset(CharacterSO);
        foreach (var perk in GetAllPerks())
        {
            if ((perk.Perk.HeroClass?.ID == _heroClass.HeroClassSO.ID || perk.Perk.HeroClass == null) && perk.Enabled)
            {
                if(perk.Perk.Functional)
                {
                    if (perk.Perk.HeroClass?.ID == _heroClass.HeroClassSO.ID)
                        _heroClass.ManageAddedPerk(perk.Perk);
                }
                else
                {
                    Stats.ModifyAccordingToPerk(perk.Perk, 1);
                }
            }
        }
        OnPerkChange?.Invoke(this, null);
    }
    protected virtual void ManagePerkChange(LockablePerk perk)
    {
        if(perk.Enabled)
        {
            if (perk.Perk.Functional)
            {
                if(perk.Perk.HeroClass?.ID == _heroClass.HeroClassSO.ID)
                    _heroClass.ManageAddedPerk(perk.Perk);
            }
            else
            {
                Stats.ModifyAccordingToPerk(perk.Perk, 1);
            }
        }
        else
        {
            if (perk.Perk.Functional)
            {
                _heroClass.ManageRemovedPerk(perk.Perk);
            }
            else
            {
                Stats.ModifyAccordingToPerk(perk.Perk, -1);
            }
        }
        OnPerkChange?.Invoke(this, new PerkChangeEventArgs { PerkSO = perk.Perk, Enabled = perk.Enabled });
    }
    protected virtual void ManagePerkRemoval(LockablePerk perk)
    {
        if (perk.Enabled)
        {
            if (perk.Perk.Functional)
            {
                if (perk.Perk.HeroClass?.ID == _heroClass.HeroClassSO.ID)
                    _heroClass.ManageRemovedPerk(perk.Perk);
            }
            else
            {
                Stats.ModifyAccordingToPerk(perk.Perk, -1);
            }
        }
        OnPerkChange?.Invoke(this, new PerkChangeEventArgs { PerkSO = perk.Perk, Enabled = false });
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
    public virtual void SyncPerks(bool addOnly)
    {
        //for inheriting classes
    }
    protected virtual void AddRegisteredPerkSets()
    {
        //for inheriting classes
    }
    #endregion

    #region StatGetters
    public HeroClassBehaviour GetHeroClass()
    {
        return _heroClass;
    }
    public LockablePerk GetPerk(string perkID)
    {
        return _perks.FirstOrDefault(p => p.Perk.ID == perkID);
        //if(ret == null && _perkSets.Any())
        //{
        //    ret = _perkSets.FirstOrDefault(s => s.Perks.Any(p => p.Perk.ID == name)).Perks.FirstOrDefault(p => p.Perk.ID == name);
        //}
        //return ret;
    }
    public LockablePerk GetPerk(PerkSO perkSO)
    {
        return _perks.FirstOrDefault(p => p.Perk == perkSO);
        //if (ret == null)
        //{
        //    ret = _perkSets.FirstOrDefault(s => s.Perks.Any(p => p.Perk == perkSO)).Perks.FirstOrDefault(p => p.Perk == perkSO);
        //}
        //return ret;
    }
    public List<LockablePerk> GetAllPerks()
    {
        var ret = new List<LockablePerk>();
        foreach (var perk in _perks)
            ret.Add(perk);
        //foreach(var perkSet in _perkSets)
            //foreach(var perk in perkSet.Perks)
                //ret.Add(perk);
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