using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BerserkerWhirlwindAbilityBehaviour : AbilityBehaviour
{
    private static float CAMERA_ZOOM_AMOUNT = 2f;
    private static float FIRESTORM_ADDITIONAL_TIME = 2.5f;

    #region Variables
    [Header("Whirlwind")]
    [SerializeField] private ParticleSystem _whirlwindParticles;
    [SerializeField] private ParticleSystem _firestormParticles;
    [SerializeField] private BerserkerWhirlwindPullEffect _pullEffect;
    private ParticleSystem _currentParticles;
    private short _timesAlreadyHit;
    private List<Damage> _currentDamages;
    private bool _cameraOffsetCurrentlyApplied;
    //for firestorm
    private float _maxHits;
    private bool _firestorm;
    //timers
    [SerializeField] private float _eachHitTimerBase;
    private float _eachHitTimer;
    #endregion

    #region Methods
    public override void Init()
    {
        base.Init();
        _producesDefaultTrace = false;
        _currentDamages = new();
        _cameraOffsetCurrentlyApplied = false;
    }
    public override void UpdateAbility(CharacterBehaviour performer, HeroClassBehaviour heroClass)
    {
        _performingTimer += Time.fixedDeltaTime;
        if (_performingTimer >= _untilHitTimer + (_eachHitTimer * _timesAlreadyHit) && _timesAlreadyHit < _maxHits)
        {
            if(_timesAlreadyHit==0)
            {
                ApplyPullToEnemies(performer);
                ((HeroBehaviour)performer).EquippedWeapon?.SetTrail(true);
            }
            _timesAlreadyHit++;
            PerformHit(performer);
        }
        //Here the class needs to update its charge to display in the interface
        if (_performingTimer < _attackTimerNext)
        {
            heroClass.AbilityCharge = heroClass.AbilityChargeMax - _performingTimer;
        }
        if(_performingTimer >= _attackTimerNext && AbilityCurrentlyLockingControl)
        {
            StopCasting(performer, true);
        }
        if (_performingTimer >= _attackTimerMax)
        {
            EndAbility(performer);
        }
    }
    public override void LaunchAbility(CharacterBehaviour performer)
    {
        base.LaunchAbility(performer);
        Globals.Instance.CurrentCinemachineCamera.GetComponent<CinemachineCustomZoom>().DistanceOffset += CAMERA_ZOOM_AMOUNT;
        _cameraOffsetCurrentlyApplied = true;
        SetUpValuesBasedOnVersion(performer);
        if (performer is HeroBehaviour)
        {
            ((HeroBehaviour)performer).GetHeroClass().AbilityChargeMax = _attackTimerNext;
            ((HeroBehaviour)performer).EquippedWeapon?.PlayLongSound("Whirlwind");
            ((HeroBehaviour)performer).EquippedWeapon?.PlayLongSound("Chainsword_Chains_Long");
        }
        performer.PlayAnimation("WhirlwindStart", true);
        performer.SetAnimationState("Whirlwind", true);
    }
    public override bool CheckPerformAvailability(CharacterBehaviour performer)
    {
        var ret = base.CheckPerformAvailability(performer);
        if(ret==false && UpgradedVersionUnlocked && UpgradedVersionCooldown == UpgradedVersionCooldownMax)
        {
            //player can cast firestorm regardless of mana level
            ret = true;
        }
        return ret;
    }
    private void SetUpValuesBasedOnVersion(CharacterBehaviour performer)
    {
        if (UpgradedVersionUnlocked && UpgradedVersionCooldown == UpgradedVersionCooldownMax)
        {
            UpgradedVersionCooldown = 0;
            _firestorm = true;
            _maxHits = 9;
            _attackTimerNext = AbilitySO.AttackTimerNext + FIRESTORM_ADDITIONAL_TIME;
            _attackTimerMax = AbilitySO.AttackTimerMax + FIRESTORM_ADDITIONAL_TIME;
            _currentParticles = Instantiate(_firestormParticles, performer.transform);
        }
        else
        {
            _firestorm = false;
            _maxHits = 4;
            _attackTimerNext = AbilitySO.AttackTimerNext;
            _attackTimerMax = AbilitySO.AttackTimerMax;
            _currentParticles = Instantiate(_whirlwindParticles, performer.transform);
        }
    }
    protected override void ReleaseControl(CharacterBehaviour performer)
    {
        base.ReleaseControl(performer);
        if(performer is HeroBehaviour)
        {
            ((HeroBehaviour)performer).GetHeroClass().AbilityChargeMax = 0;
        }
    }
    public void StopCasting(CharacterBehaviour performer, bool normally)
    {
        //short moment before the abiliy ends, where the chracter stops for a second to slow down the momentum of the weapon
        AbilityCurrentlyLockingControl = false;
        if(normally)
        {
            performer.SetCanAct(true, true);
            performer.SetCanMove(false);
        }
        if(_cameraOffsetCurrentlyApplied)
        {
            Globals.Instance.CurrentCinemachineCamera.GetComponent<CinemachineCustomZoom>().DistanceOffset -= CAMERA_ZOOM_AMOUNT;
            _cameraOffsetCurrentlyApplied = false;
        }
        if(performer is HeroBehaviour)
        {
            ((HeroBehaviour)performer).EquippedWeapon?.StopLongSound("Whirlwind");
            ((HeroBehaviour)performer).EquippedWeapon?.StopLongSound("Chainsword_Chains_Long");
            ((HeroBehaviour)performer).EquippedWeapon?.SetTrail(false);
            ((HeroBehaviour)performer).GetHeroClass().AbilityChargeMax = 0;
        }
        performer.SetAnimationState("Whirlwind", false);
    }
    public override void Interrupt(CharacterBehaviour performer)
    {
        EndAbility(performer);
        if(_currentParticles != null)
        {
            _currentParticles.Stop();
        }
        StopCasting(performer, false);
    }
    protected override void LockControl(CharacterBehaviour performer)
    {
        base.LockControl(performer);
        performer.SetCanMove(true);
        if (performer is PlayerBehaviour)
            ((PlayerBehaviour)performer).CanDash = true;
    }
    public override void EndAbility(CharacterBehaviour performer)
    {
        base.EndAbility(performer);
        ReleaseControl(performer);
        if (_currentParticles != null && !_currentParticles.gameObject.IsDestroyed())
            Destroy(_currentParticles.gameObject);
    }
    private void ApplyPullToEnemies(CharacterBehaviour performer)
    {
        _pullEffect.BerserkerPosition = performer.transform.position;
        var newIns = Instantiate(this, performer.transform, false);
        newIns.transform.localPosition = Vector3.zero;
        Physics.SyncTransforms();
        var potentialCasualities = Physics.OverlapSphere(performer.transform.position, 15);
        foreach (var casuality in potentialCasualities)
        {
            if(CharacterBehaviour.FindEnemyCharacterInCollider(casuality, performer, out CharacterBehaviour character) == true)
            {
                character.AddEffect(_pullEffect);
            }
        }
        Destroy(newIns.gameObject);
    }
    public override void PerformHit(CharacterBehaviour performer)
    {
        base.PerformHit(performer);
        Physics.SyncTransforms();
        var potentialCasualities = Physics.OverlapSphere(performer.transform.position, 10);
        var collider = Instantiate(transform.Find("Collider").GetComponent<Collider>(), performer.transform, false);
        float potentialHealing = 0;
        if (collider != null)
        {
            foreach (var casuality in potentialCasualities)
            {
                if (casuality == collider || casuality == performer.GetComponent<Collider>())
                    continue;
                if (casuality.bounds.Intersects(collider.bounds) && CharacterBehaviour.FindEnemyCharacterInCollider(casuality, performer, out var character))
                {
                    Damage newDamage = null;
                    if (_firestorm)
                    {
                        newDamage = CalculateDamage(new Damage(performer, performer.GetEffectiveDamage() * 2, false, false), performer.GetEffectiveCriticalChance());
                    }
                    else
                    {
                        newDamage = CalculateDamage(new Damage(performer, performer.GetEffectiveDamage(), false, false), performer.GetEffectiveCriticalChance());
                    }
                    var oldDamage = _currentDamages.FirstOrDefault(dam => dam == character.LastDamage);
                    if (oldDamage != null)
                    {
                        oldDamage.AddMultishot(newDamage);
                        character.TakeDamage(oldDamage);
                    }
                    else
                    {
                        character.TakeDamage(newDamage);
                        _currentDamages.Add(newDamage);
                    }
                    if (character.CharacterSO.Category.HealthBarsAmount > 1)
                    {
                        potentialHealing += 2.5f;
                    }
                    else
                    {
                        potentialHealing++;
                    }
                }
            }
        }
        if(!_firestorm)
        {
            performer.Stats.CurrentMana -= AbilitySO.ManaCost / 4;
            if (performer.Stats.CurrentMana < 0)
            {
                performer.Stats.CurrentMana = 0;
            }
        }
        else if(potentialHealing > 0 && ((HeroBehaviour)performer).GetPerk("Madman's Lot")?.Enabled == true)
        { 
            if (potentialHealing > 5)
                potentialHealing = 5;
            potentialHealing = potentialHealing / 100;
            performer.HealPercent(potentialHealing, true);
        }
        Destroy(collider.gameObject);
    }
    public override void UpdateCooldown()
    {
        base.UpdateCooldown();
        if(UpgradedVersionUnlocked)
        {
            UpgradedVersionCooldownMax = AbilitySO.UpgradedCooldown;
            UpgradedVersionCooldown += Time.fixedDeltaTime;
            if (UpgradedVersionCooldown > UpgradedVersionCooldownMax)
                UpgradedVersionCooldown = UpgradedVersionCooldownMax;
        }
        else
        {
            UpgradedVersionCooldownMax = 0f;
        }
    }
    protected override void CalculateAttackSpeed(float speed)
    {
        base.CalculateAttackSpeed(speed);
        _eachHitTimer = _eachHitTimerBase;
    }
    public override void Reset()
    {
        base.Reset();
        _timesAlreadyHit = 0;
        _currentDamages?.Clear();
        _maxHits = 4;
        _firestorm = false;
    }
    #endregion
}
