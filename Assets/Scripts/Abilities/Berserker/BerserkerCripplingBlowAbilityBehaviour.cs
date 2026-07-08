using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BerserkerCripplingBlowAbilityBehaviour : MovingAbilityBehaviour
{
    #region Variables
    [Header("Crippling Blow")]
    [SerializeField] private ParticleSystem _particles;
    [SerializeField] private BerserkerCripplingBlowStunEffect _stunEffect;
    private ParticleSystem _currentParticles;
    private List<Damage> _currentDamages;
    private CharacterBehaviour _casuality;
    [SerializeField] private float _eachHitTimerBase, _firstHitTimerBase;
    private float _eachHitTimer, _firstHitTimer;
    private float _timesAlreadyHit;
    //for red glow on affected enemy
    private float _casualityAlpha = 0.5f;
    //for empty hits during ability
    [SerializeField] private float _emptyHitTimerBase;
    private float _emptyHitTimer, _emptyHits;
    private bool _burningChainApplied;
    #endregion

    #region Methods
    public override void Init()
    {
        base.Init();
        _producesDefaultTrace = false;
        _currentDamages = new();
        _ignoreMovementBlockers = true;
    }
    public override void UpdateAbility(CharacterBehaviour performer, HeroClassBehaviour heroClass)
    {
        _performingTimer += Time.fixedDeltaTime;
        if (_performingTimer >= _firstHitTimer + (_eachHitTimer * _timesAlreadyHit) && _timesAlreadyHit < 4)
        {
            if (_timesAlreadyHit == 0)
            {
                if (performer is HeroBehaviour && ((HeroBehaviour)performer).EquippedWeapon != null)
                {
                    _currentParticles = Instantiate(_particles, ((HeroBehaviour)performer).EquippedWeapon.transform);
                    ((HeroBehaviour)performer).EquippedWeapon.PlayLongSound("Chainsword_Sustain");
                }
                _casuality.AddEffect(_stunEffect);
                SetEnemyRed(true);
            }
            _timesAlreadyHit++;
            PerformHit(performer);
        }
        else if (_performingTimer >= _firstHitTimer + (_emptyHitTimer * _emptyHits))
        {
            _emptyHits++;
            _casuality.TakeEmptyDamage();
        }
        UpdateCustomMove(performer);
        //Here the class needs to update its charge to display in the interface
        if (_performingTimer < AbilitySO.AttackTimerNext)
        {
            heroClass.AbilityCharge = heroClass.AbilityChargeMax - _performingTimer;
        }
        if (_performingTimer >= AbilitySO.AttackTimerNext && AbilityCurrentlyLockingControl)
        {
            ReleaseControl(performer);
        }
        if (_performingTimer >= AbilitySO.AttackTimerMax)
        {
            EndAbility(performer);
        }
        if(_performingTimer > _firstHitTimer && performer is PlayerBehaviour && !((PlayerBehaviour)performer).GetAbilityPressed(3))
        {
            Interrupt(performer);
        }
    }
    public override void LaunchAbility(CharacterBehaviour performer)
    {
        base.LaunchAbility(performer);
        if(performer is HeroBehaviour)
        {
            ((HeroBehaviour)performer).GetHeroClass().AbilityChargeMax = AbilitySO.AttackTimerNext;
        }
        var effect = performer.GetActiveEffects().FirstOrDefault(e => e.EffectSO.Name == "Burning Chain");
        if (effect != null)
        {
            _burningChainApplied = true;
            performer.RemoveEffect(effect);
        }
        if(FindCasuality(performer) == false)
        {
            Interrupt(performer);
        }
        performer.PlayAnimation("CripplingBlowStart", true);
        performer.SetAnimationState("CripplingBlow", true);
        performer.FaceTheTarget((_casuality.transform.position - performer.transform.position).normalized);
    }
    public override bool CheckPerformAvailability(CharacterBehaviour performer)
    {
        if(base.CheckPerformAvailability(performer) && FindCasuality(performer))
            return true;
        return false;
    }
    public override void PerformHit(CharacterBehaviour performer)
    {
        base.PerformHit(performer);
        if(_casuality != null && _casuality.IsDead == false)
        {
            var oldDamage = _currentDamages.FirstOrDefault(dam => dam == _casuality.LastDamage);
            if (oldDamage != null)
            {
                var newDamageAmount = CalculateDamage(new Damage(performer, performer.GetEffectiveDamage(), false, false), performer.GetEffectiveCriticalChance());
                if (_burningChainApplied)
                    newDamageAmount.Amount = (int)(newDamageAmount.Amount * 2.5f);
                oldDamage.AddMultishot(newDamageAmount);
                _casuality.TakeDamage(oldDamage);
            }
            else
            {
                var damage = CalculateDamage(new Damage(performer, performer.GetEffectiveDamage(), false, false), performer.GetEffectiveCriticalChance());
                if (_burningChainApplied)
                    damage.Amount = (int)(damage.Amount * 2.5f);
                _casuality.TakeDamage(damage);
                _currentDamages.Add(damage);
            }
        }
        else
        {
            Interrupt(performer);
        }
    }
    public bool FindCasuality(CharacterBehaviour performer)
    {
        if(performer is PlayerBehaviour)
        {
            var player = (PlayerBehaviour)performer;
            if(player.SelectedCharacter!=null)
            {
                if(player.Faction.FactionType != player.SelectedCharacter.Faction.FactionType && !player.Faction.Allies.Contains(player.SelectedCharacter.Faction.FactionType))
                {
                    var magnitude = (performer.transform.position - player.SelectedCharacter.transform.position).magnitude;
                    var targetcollider = player.SelectedCharacter.GetComponentInChildren<Collider>();
                    if(targetcollider!=null)
                        magnitude -= targetcollider.transform.localScale.x;
                    if (magnitude < 3)
                    {
                        _casuality = player.SelectedCharacter;
                        if(_casuality!=null)
                            return true;
                    }
                }
            }
        }
        return false;
    }
    public override void Interrupt(CharacterBehaviour performer)
    {
        if (AbilityCurrentlyLockingControl)
        {
            ReleaseControl(performer);
        }
        EndAbility(performer);
    }
    protected override void ReleaseControl(CharacterBehaviour performer)
    {
        base.ReleaseControl(performer);
        AbilityCurrentlyLockingControl = false;
        if(performer is HeroBehaviour)
        {
            ((HeroBehaviour)performer).EquippedWeapon?.StopLongSound("Chainsword_Sustain");
            ((HeroBehaviour)performer).GetHeroClass().AbilityChargeMax = 0;
        }
        if (_casuality != null)
        {
            _casuality.RemoveEffect(_stunEffect);
            SetEnemyRed(false);
        }
        if(_currentParticles!=null)
            Destroy(_currentParticles.gameObject);
        performer.SetAnimationState("CripplingBlow", false);
    }
    private void SetEnemyRed(bool isRed)
    {
        if (isRed)
        {
            _casuality?.SetGlow(_casualityAlpha, Color.red);
        }
        else
        {
            _casuality?.SetGlow(0, Color.red);
        }
    }
    protected override void CalculateAttackSpeed(float speed)
    {
        base.CalculateAttackSpeed(speed);
        _eachHitTimer = _eachHitTimerBase;
        _firstHitTimer = _firstHitTimerBase;
        _emptyHitTimer = _emptyHitTimerBase;
        _customMoveTimerEnd = AbilitySO.AttackTimerNext;
        _customMoveTimerStart = _firstHitTimer;
    }
    public override void Reset()
    {
        base.Reset();
        _timesAlreadyHit = 0;
        _currentDamages?.Clear();
        _emptyHits = 0;
        _casuality = null;
        _burningChainApplied = false;
    }
    #endregion
}
