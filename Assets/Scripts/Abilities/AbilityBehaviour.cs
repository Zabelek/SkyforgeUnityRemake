using System;
using Unity.VisualScripting;
using UnityEngine;

public class AbilityBehaviour : MonoBehaviour
{
    #region Variables
    [Header("General Ability Variables")]
    [Tooltip("Ability Scriptable Object with basic needed values")]
    public AbilitySO AbilitySO;
    public bool AbilityCurrentlyLockingControl { get; protected set; }
    [Tooltip("If multiple instances of this ability being updated at once may break things, set this to true")]
    [SerializeField] protected bool ForcesInterruptionToSameType = false;
    [Tooltip("Time from launching ability to PerformHit() execution. This can be treated as the exact moment when the weapon/projectile strikes an enemy.")]
    [SerializeField] protected float _untilHitTimerBase;
    protected bool _hitPerformed, _producesDefaultTrace, _demobillizesPerformer;
    protected float _untilHitTimer, _performingTimer, _attackTimerMax, _attackTimerNext;
    //for Cooldown
    [HideInInspector] public float MaxCooldown, CurrentCooldown;
    //for upgraded ability versions
    [HideInInspector] public float UpgradedVersionCooldown, UpgradedVersionCooldownMax;
    public bool UpgradedVersionUnlocked { get; set; }
    //damage multiplier for external sources like weapons, class effects, etc.
    [HideInInspector] public float ExternalDamageMultiplier = 1;
    //Used when UpdateAbility in inheriting abilities needs to know if base.UpdateAbility ended the ability in the current iteration.
    [HideInInspector] public bool Finishing;
    //events
    public EventHandler OnAbilityStart, OnAbilityEnd, OnAbilityHit;
    #endregion

    #region Methods
    public virtual void Init()
    {
        Reset();
        MaxCooldown = AbilitySO.Cooldown;
        CurrentCooldown = MaxCooldown;
        _demobillizesPerformer = true;
        _performingTimer = AbilitySO.AttackTimerMax;
        Finishing = false;
    }
    public virtual void UpdateAbility(CharacterBehaviour performer, HeroClassBehaviour heroClass)
    {
        //each frame during which the ability is performed (after LaunchAbility
        _performingTimer += Time.fixedDeltaTime;
        //if _untilHitTimer is set to 0, program would think it was no set by the dev, so the hit shouldn't be performed in this ability. If you want to perform hit immediately after launch, just put something like 0.001
        if (!_hitPerformed && _untilHitTimer!=0 && _performingTimer >= _untilHitTimer)
        {
            PerformHit(performer);
            performer.SpeakingBehaviour?.PerformCombatShout(AbilitySO.AbilityWeight);
        }
        if (AbilityCurrentlyLockingControl && _performingTimer >= _attackTimerNext)
        {
            ReleaseControl(performer);
            if (_producesDefaultTrace && performer is HeroBehaviour)
                ((HeroBehaviour)performer).EquippedWeapon?.SetTrail(false);
        }
        if (_performingTimer >=_attackTimerMax)
        {
            EndAbility(performer);
        }
    }
    public virtual void LaunchAbility(CharacterBehaviour performer)
    {
        //Everything that needs to execute at first frame after ability is used
        Reset();
        CalculateAttackSpeed(performer.GetEffectiveAttackSpeed());
        LockControl(performer);
        if (_producesDefaultTrace && performer is HeroBehaviour)
            ((HeroBehaviour)performer).EquippedWeapon?.SetTrail(true);
        if (AbilitySO.Cooldown > 0)
            CurrentCooldown = 0;
        if (performer is HeroBehaviour)
        {
            //If the hero is spamming the same skill, it might be in CurrentlyUpdatedAbilities already.
            //It will also be in RecentlyFinishedAbilities, as it was scheduled to be removed this frame.
            //It needs to be removed from RecentlyFinishedAbilities, because it will be deleted next frame otherwise, and the hero may freeze. In most cases, that is.
            if (ForcesInterruptionToSameType && ((HeroBehaviour)performer).GetHeroClass()?.CurrentlyUpdatedAbilities.Contains(this) == true)
            {
                ((HeroBehaviour)performer).GetHeroClass().RecentlyFinishedAbilities.Remove(this);
                ((HeroBehaviour)performer).GetHeroClass().RecentlyFinishedAbilities.RemoveAll(a => a.GetType() == this.GetType());
            }
            else
                ((HeroBehaviour)performer).GetHeroClass()?.CurrentlyUpdatedAbilities.Add(this);
            ((HeroBehaviour)performer).GetHeroClass().CurrentlyPerformedComboState = AbilitySO.StateType;
        }
        else if (performer is MonsterBehaviour)
        {
            if (ForcesInterruptionToSameType && ((MonsterBehaviour)performer).CurrentlyUpdatedAbilities.Contains(this))
                ((MonsterBehaviour)performer).RecentlyFinishedAbilities.RemoveAll(a => a.GetType() == this.GetType());
            else
                ((MonsterBehaviour)performer).CurrentlyUpdatedAbilities.Add(this);
        }
        OnAbilityStart?.Invoke(this, EventArgs.Empty);
    }
    protected virtual void LockControl(CharacterBehaviour performer)
    {
        if (_demobillizesPerformer == true)
        {

            performer.SetCanMove(false);
            performer.SetCanAct(false, true);
            if (performer is HeroBehaviour)
            {
                ((HeroBehaviour)performer).GetHeroClass().SetBlockingAbility(this);
                if (performer is PlayerBehaviour)
                {
                    ((PlayerBehaviour)performer).CanDash = false;
                }
            }
        }
        AbilityCurrentlyLockingControl = true;
    }
    protected virtual void ReleaseControl(CharacterBehaviour performer)
    {
        if (_demobillizesPerformer == true && performer.IsDead == false)
        {
            performer.SetCanMove(true);
            performer.SetCanAct(true, true);
            if (performer is HeroBehaviour)
            {
                ((HeroBehaviour)performer).GetHeroClass().SetBlockingAbility(null);
                if (performer is PlayerBehaviour)
                {
                    ((PlayerBehaviour)performer).CanDash = true;
                }
            }
        }
        else if (performer is HeroBehaviour)
        {
            ((HeroBehaviour)performer).GetHeroClass().SetBlockingAbility(null);
        }
        AbilityCurrentlyLockingControl = false;
    }
    public virtual void EndAbility(CharacterBehaviour performer)
    {
        Reset();
        if(performer is HeroBehaviour)
        {
            ((HeroBehaviour)performer).GetHeroClass()?.RecentlyFinishedAbilities.Add(this);
            if(((HeroBehaviour)performer).GetHeroClass().CurrentlyPerformedComboState == AbilitySO.StateType)
            {
                ((HeroBehaviour)performer).GetHeroClass().CurrentlyPerformedComboState = HeroClassBehaviour.ComboState.N;
            }
        }
        else if (performer is MonsterBehaviour)
        {
            ((MonsterBehaviour)performer).RecentlyFinishedAbilities.Add(this);
        }
        Finishing = true;
        OnAbilityEnd?.Invoke(this, EventArgs.Empty);
    }
    public virtual bool CheckPerformAvailability(CharacterBehaviour performer)
    {
        if (performer.Stats.CurrentMana < AbilitySO.ManaCost)
        {
            return false;
        }
        if (CurrentCooldown < MaxCooldown)
            return false;
        return true;
    }
    public virtual void PerformHit(CharacterBehaviour performer)
    {
        _hitPerformed = true;
        OnAbilityHit?.Invoke(this, EventArgs.Empty);
    }
    public virtual void PerformHit(CharacterBehaviour performer, GameObject[] targets)
    {
        _hitPerformed = true;
    }
    protected virtual Damage CalculateDamage(Damage damage, float critChance)
    {
        damage.Amount = (int)(damage.Amount * AbilitySO.DamageMultiplier);
        if (UnityEngine.Random.Range(0f, 1f) < critChance)
        {
            damage.Amount = damage.Amount * 2;
            damage.Critical = true;
        }
        damage.Amount = (int)(damage.Amount * ExternalDamageMultiplier);
        return damage;
    }
    protected virtual void CalculateAttackSpeed(float speed)
    {
        if(!AbilitySO.IsSkill)
        {
            _attackTimerNext = AbilitySO.AttackTimerNext / speed;
            _attackTimerMax = AbilitySO.AttackTimerMax / speed;
            _untilHitTimer = _untilHitTimerBase / speed;
        }
        else
        {
            _attackTimerNext = AbilitySO.AttackTimerNext;
            _attackTimerMax = AbilitySO.AttackTimerMax;
            _untilHitTimer = _untilHitTimerBase;
        }
    }
    public virtual void Reset()
    {
        _performingTimer = 0;
        _hitPerformed = false;
        Finishing = false;
    }
    public virtual void Interrupt(CharacterBehaviour performer)
    {
        ReleaseControl(performer);
        EndAbility(performer);
        if (_producesDefaultTrace && performer is HeroBehaviour)
            ((HeroBehaviour)performer).EquippedWeapon?.SetTrail(false);
    }
    public virtual void UpdateCooldown()
    {
        if(CurrentCooldown < MaxCooldown)
        {
            CurrentCooldown += Time.fixedDeltaTime;
            if (CurrentCooldown > MaxCooldown)
            {
                CurrentCooldown = MaxCooldown;
            }
        }
    }
    #endregion
}
