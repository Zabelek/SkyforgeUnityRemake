using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

public class BerserkerAbilityBehaviour : MovingAbilityBehaviour
{
    private const int BURNING_CHAIN_CHANCE_PERCENT = 20;

    #region Variables
    [Header("Destructive Attack")]
    [SerializeField] private BerserkerDestructiveAttackVisualGroup _visuals;
    [SerializeField] private GameplayEffectBehaviour _destAttackEffect;
    [SerializeField] private GameplayEffectBehaviour _burningChain;
    [SerializeField] private GameplayEffectBehaviour _thrillOfVictory;
    private bool _chainswordSoundPlayed, _groundImpactSoundPlayed;
    private bool _charging, _strong, _alreadySpawnedVisuals;
    //timers
    [SerializeField] private float _chainswordSoundTimerStrongBase, _groundImpactSoundTimerStrongBase,_effectTimerStartStrongBase, _strongTresholdBase, 
        _untilHitTimerStrongBase, _chainswordSoundTimerWeakBase, _groundImpactSoundTimerWeakBase, _effectTimerStartWeakBase;
    private float _chainswordSoundTimer, _groundImpactSoundTimer, _effectTimerStart, _strongTreshold;
    #endregion

    #region Methods
    public override void Init()
    {
        base.Init();
        _producesDefaultTrace = false;
        _hitPerformed = false;
        _chainswordSoundPlayed = false;
        _groundImpactSoundPlayed = false;
    }
    public override void UpdateAbility(CharacterBehaviour performer, HeroClassBehaviour heroClass)
    {
        _performingTimer += Time.fixedDeltaTime;
        if (_charging)
        {
            if (_strong == false && _performingTimer >= _strongTreshold)
            {
                _strong = true;
            }
            if (_performingTimer >= _attackTimerNext || heroClass.AbilityChargeMax == 0)
            {
                EndCharging(performer);
            }
            heroClass.AbilityCharge = _performingTimer;
        }
        else if (_strong)
        {
            if (_performingTimer > _groundImpactSoundTimer && _groundImpactSoundPlayed == false)
            {
                ((HeroBehaviour)performer).EquippedWeapon?.PlaySound("Chainsword_Ground_Impact");
                _groundImpactSoundPlayed = true;
            }
            if (_performingTimer > _chainswordSoundTimer && _chainswordSoundPlayed == false)
            {
                ((HeroBehaviour)performer).EquippedWeapon?.PlaySound("Chainsword_Woosh");
                _chainswordSoundPlayed = true;
            }
            UpdateCustomMove(performer);
            if (_performingTimer > _effectTimerStart && _performingTimer < _effectTimerStart + 0.5f && _alreadySpawnedVisuals == false)
            {
                SpawnStrongEffect(performer);
                _alreadySpawnedVisuals = true;
            }
            if (!_hitPerformed && _performingTimer > _untilHitTimer)
            {
                PerformHit(performer);
                performer.SpeakingBehaviour?.PerformCombatShout(AbilitySO.AbilityWeight);
            }
        }
        else
        {
            if (_performingTimer > _groundImpactSoundTimer && _groundImpactSoundPlayed == false)
            {
                ((HeroBehaviour)performer).EquippedWeapon?.PlaySound("Chainsword_Ground_Impact");
                _groundImpactSoundPlayed = true;
            }
            if (_performingTimer > _chainswordSoundTimer && _chainswordSoundPlayed == false)
            {
                ((HeroBehaviour)performer).EquippedWeapon?.PlaySound("Chainsword_Woosh");
                _chainswordSoundPlayed = true;
            }
            if (_performingTimer > _effectTimerStart && _performingTimer < _effectTimerStart + 0.5f && _alreadySpawnedVisuals == false)
            {
                SpawnWeakEffect(performer);
                _alreadySpawnedVisuals = true;
            }
            if (!_hitPerformed && _performingTimer > _untilHitTimer)
            {
                PerformHit(performer);
                performer.SpeakingBehaviour?.PerformCombatShout(AbilitySO.AbilityWeight);
            }
        }
        if (AbilityCurrentlyLockingControl && _performingTimer >= _attackTimerNext)
        {
            ReleaseControl(performer);
            if (_producesDefaultTrace && performer is HeroBehaviour)
                ((HeroBehaviour)performer).EquippedWeapon?.SetTrail(false);
        }
        if (_performingTimer >= _attackTimerMax)
        {
            EndAbility(performer);
        }
    }
    private void EndCharging(CharacterBehaviour performer)
    {
        if(performer is HeroBehaviour)
        {          
            if (_strong)
            {
                ((HeroBehaviour)performer).GetHeroClass().ChargingReturn = 2;
                SetUpForStrong(performer.GetEffectiveAttackSpeed());
                performer.PlayAnimation("DestructiveAttack3", true);
            }
            else
            {
                ((HeroBehaviour)performer).GetHeroClass().ChargingReturn = 1;
                SetUpForWeak(performer.GetEffectiveAttackSpeed());
                performer.PlayAnimation("DestructiveAttack2", true);
            }
        } 
        _charging = false;
        _performingTimer = 0;
    }
    public override void LaunchAbility(CharacterBehaviour performer)
    {
        base.LaunchAbility(performer);
        CalculateAttackSpeed(performer.GetEffectiveAttackSpeed());
        performer.SetCanMove(false);
        performer.PlayAnimation("DestructiveAttack1", true);
        if (performer is HeroBehaviour)
        {
            ((HeroBehaviour)performer).GetHeroClass().AbilityChargeMax = _attackTimerNext;
            if (performer is PlayerBehaviour)
            {
                ((PlayerBehaviour)performer).CanDash = false;
            }
        }
        _charging = true;
    }
    protected override void ReleaseControl(CharacterBehaviour performer)
    {
        base.ReleaseControl(performer);
        if (performer is HeroBehaviour)
        {
            ((HeroBehaviour)performer).GetHeroClass().AbilityChargeMax = 0;
            ((HeroBehaviour)performer).GetHeroClass().ChargingReturn = 0;
        }
    }
    private void SpawnWeakEffect(CharacterBehaviour performer)
    {
        var part = Instantiate(_visuals, performer.transform);
        part.transform.SetParent(null);
        part.gameObject.SetActive(true);
        part.Strong = false;
    }
    private void SpawnStrongEffect(CharacterBehaviour performer)
    {
        var part = Instantiate(_visuals, performer.transform);
        part.transform.SetParent(null);
        part.gameObject.SetActive(true);
        part.Strong = true;
    }
    private void SetUpForStrong(float speed)
    {
        _groundImpactSoundTimer = _groundImpactSoundTimerStrongBase / speed;
        _chainswordSoundTimer = _chainswordSoundTimerStrongBase / speed;
        _customMoveTimerStart = _customMoveTimerStartBase / speed;
        _customMoveTimerEnd = _customMoveTimerEndBase / speed;
        _effectTimerStart = _effectTimerStartStrongBase / speed;
        _untilHitTimer = _untilHitTimerStrongBase / speed;
    }
    private void SetUpForWeak(float speed)
    {
        _groundImpactSoundTimer = _groundImpactSoundTimerWeakBase / speed;
        _chainswordSoundTimer = _chainswordSoundTimerWeakBase / speed;
        _effectTimerStart = _effectTimerStartWeakBase / speed;
        _untilHitTimer = _untilHitTimerBase / speed;
    }
    public override void PerformHit(CharacterBehaviour performer)
    {
        base.PerformHit(performer);
        Physics.SyncTransforms();
        Collider[] potentialCasualities;
        Collider collider;
        if(_strong)
        {
            potentialCasualities = Physics.OverlapSphere(performer.transform.position, 15);
            collider = Instantiate(transform.Find("Collider1").GetComponent<Collider>(), performer.transform, false);
        }
        else
        {
            potentialCasualities = Physics.OverlapSphere(performer.transform.position, 5);
            collider = Instantiate(transform.Find("Collider").GetComponent<Collider>(), performer.transform, false);
        }
        int casualityAmount = 0;
        var thrillOfVictoryEffect = performer.GetActiveEffects().FirstOrDefault(e => e.EffectSO.Name == "Thrill of Victory Cast");
        if (collider!=null)
        {
            foreach(var casuality in potentialCasualities)
            {
                if (casuality == collider || casuality == performer.GetComponent<Collider>())
                    continue;
                if (casuality.bounds.Intersects(collider.bounds) && CharacterBehaviour.FindEnemyCharacterInCollider(casuality, performer, out var character))
                {
                    Damage damage;
                    if (_strong)
                    {
                        damage = CalculateDamage(new Damage(performer, performer.GetEffectiveDamage() * 2, false, false), performer.GetEffectiveCriticalChance());
                    }
                    else
                    {
                        damage = CalculateDamage(new Damage(performer, performer.GetEffectiveDamage(), false, false), performer.GetEffectiveCriticalChance());
                    }
                    character.TakeDamage(damage);
                    character.AddEffect(_destAttackEffect);
                    if (thrillOfVictoryEffect != null)
                    {
                        character.AddEffect(_thrillOfVictory);
                    }
                    casualityAmount++;
                }
            }
        }
        if(TryGetComponent<CinemachineImpulseSource>(out CinemachineImpulseSource imp))
        {
            if (_strong)
                imp.DefaultVelocity = new Vector3(0f, 0.2f, 0f);
            else
                imp.DefaultVelocity = new Vector3(0f, 0.08f, 0f);
            imp.GenerateImpulse();
        }
        if(casualityAmount>0)
        {
            if(thrillOfVictoryEffect != null)
            {
                performer.RemoveEffect(thrillOfVictoryEffect);
            }
            performer.Stats.CurrentMana += 3;
            if (performer is HeroBehaviour && ((HeroBehaviour)performer).GetHeroClass() is BerserkerClassBehaviour && ((HeroBehaviour)performer).GetPerk("Base_Berserker_BurningChain")?.Enabled == true)
            {
                if (Random.Range(1, 100) <= BURNING_CHAIN_CHANCE_PERCENT)
                {
                    performer.AddEffect(_burningChain);
                }
            }
        }
        Destroy(collider.gameObject);
    }
    protected override void CalculateAttackSpeed(float speed)
    {
        base.CalculateAttackSpeed(speed);
        _strongTreshold = _strongTresholdBase / speed;
    }
    public override void Reset()
    {
        base.Reset();
        _chainswordSoundPlayed = false;
        _groundImpactSoundPlayed = false;
        _strong = false;
        _charging = false;
        _alreadySpawnedVisuals = false;
    }
    #endregion
}
