using Unity.Cinemachine;
using UnityEngine;

public class BerserkerFractureAbilityBehaviour : AbilityBehaviour
{
    private const int BURNING_CHAIN_CHANCE_PERCENT = 40;

    #region Variables
    [Header("Fracture")]
    [SerializeField] private ParticleSystem _particles;
    [SerializeField] private GameplayEffectBehaviour _knockUp;
    [SerializeField] private GameplayEffectBehaviour _burningChain;
    private bool _groundImpactSoundPlayed;
    private int _timesChainswordSoundPlayed, _timesForWoosh;
    //additional trace
    [SerializeField] private Transform _additionalTraceBase;
    private Transform _currentAdditionalTrace;
    private bool _additionalTraceSpawned, _particlesSpawned, _secondCameraShakePerformed;
    //timers
    [SerializeField]
    private float _effectTimerStartBase, _chainswordSoundTimerBase, _groundImpactSoundTimerBase, _wooshStartTimerBase,
        _chainswordSoundTimerSecondBase, _wooshStartTimerNextBase, _additionalTraceSpawnTimeBase, _additionalTraceDespawnTimeBase, _secondCameraShakeTimeBase;
    private float _effectTimerStart, _chainswordSoundTimer, _groundImpactSoundTimer, _wooshStartTimer, _chainswordSoundTimerSecond,
        _wooshStartTimerNext, _additionalTraceSpawnTime, _additionalTraceDespawnTime, _secondCameraShakeTime;
    #endregion

    #region Methods
    public override void Init()
    {
        base.Init();
        _producesDefaultTrace = true;
    }
    public override void UpdateAbility(CharacterBehaviour player, HeroClassBehaviour heroClass)
    {
        base.UpdateAbility(player, heroClass);
        if (!Finishing)
        {
            if (_performingTimer > _additionalTraceSpawnTime && _additionalTraceSpawned == false)
            {
                _additionalTraceSpawned = true;
                _currentAdditionalTrace = Instantiate(_additionalTraceBase, ((HeroBehaviour)player).EquippedWeapon?.transform);
                _currentAdditionalTrace.transform.localPosition += new Vector3(0, 3.2f, 0);
            }
            if (_performingTimer > _additionalTraceDespawnTime && _currentAdditionalTrace != null)
            {
                Destroy(_currentAdditionalTrace.gameObject);
                _currentAdditionalTrace = null;
            }
            if (_timesChainswordSoundPlayed < 2 && _performingTimer > _chainswordSoundTimer + _chainswordSoundTimerSecond * _timesChainswordSoundPlayed)
            {
                ((HeroBehaviour)player).EquippedWeapon?.PlaySound("Chainsword_Hit");
                ((HeroBehaviour)player).EquippedWeapon?.PlaySound("Chainsword_Woosh");
                _timesChainswordSoundPlayed++;
            }
            if (_timesForWoosh < 3 && _performingTimer > _wooshStartTimer + _wooshStartTimerNext * _timesForWoosh)
            {
                ((HeroBehaviour)player).EquippedWeapon?.PlaySound("Chainsword_Woosh");
                _timesForWoosh++;
            }
            if (_performingTimer > _groundImpactSoundTimer && _groundImpactSoundPlayed == false)
            {
                ((HeroBehaviour)player).EquippedWeapon?.PlaySound("Chainsword_Ground_Impact");
                ((HeroBehaviour)player).EquippedWeapon?.PlaySound("Chainsword_Chains_Short");
                _groundImpactSoundPlayed = true;
                try { ((PlayerBehaviour)player).CanDash = true; } catch { }
            }
            if (_performingTimer > _effectTimerStart && _particlesSpawned == false)
            {
                var currentParticles = Instantiate(_particles, player.transform);
                currentParticles.transform.localPosition = new Vector3(-0.02699921f, -0.245f, 2.286002f);
                currentParticles.transform.SetParent(null);
                _particlesSpawned = true;
            }
            if(_performingTimer > _secondCameraShakeTime && _secondCameraShakePerformed == false)
            {
                if (TryGetComponent<CinemachineImpulseSource>(out CinemachineImpulseSource imp))
                {
                    imp.GenerateImpulse();
                }
                _secondCameraShakePerformed = true;
            }
        }
    }
    public override void LaunchAbility(CharacterBehaviour performer)
    {
        base.LaunchAbility(performer);
        performer.PlayAnimation("Fracture", true);
    }
    public override void Interrupt(CharacterBehaviour performer)
    {
        base.Interrupt(performer);
        if (_currentAdditionalTrace != null)
        {
            Destroy(_currentAdditionalTrace.gameObject);
            _currentAdditionalTrace = null;
        }
    }
    public override void PerformHit(CharacterBehaviour performer)
    {
        base.PerformHit(performer);
        Physics.SyncTransforms();
        var potentialCasualities = Physics.OverlapSphere(performer.transform.position, 5);
        var collider = Instantiate(transform.Find("Collider").GetComponent<Collider>(), performer.transform, false);
        int casualityAmount = 0;
        if(collider!=null)
        {
            foreach(var casuality in potentialCasualities)
            {
                if (casuality == collider || casuality == performer.GetComponent<Collider>())
                    continue;
                if (casuality.bounds.Intersects(collider.bounds) && CharacterBehaviour.FindEnemyCharacterInCollider(casuality, performer, out var character))
                {
                    var damage = CalculateDamage(new Damage(performer, performer.GetEffectiveDamage(), false, false), performer.GetEffectiveCriticalChance());
                    if (performer is PlayerBehaviour && ((PlayerBehaviour)performer).SelectedCharacter == character)
                    {
                        character.TakeDamage(damage);
                    }
                    else
                    {
                        damage.Amount = damage.Amount / 3;
                        character.TakeDamage(damage);
                    }
                    character.AddEffect(_knockUp);
                    casualityAmount++;
                }
            }
        }
        if(TryGetComponent<CinemachineImpulseSource>(out CinemachineImpulseSource imp))
        {
            imp.GenerateImpulse();
        }
        if(casualityAmount>0)
        {
            performer.Stats.CurrentMana -= AbilitySO.ManaCost;
            if(performer is HeroBehaviour && ((HeroBehaviour)performer).GetHeroClass() is BerserkerClassBehaviour)
            {
                var zerkClass = (BerserkerClassBehaviour)(((HeroBehaviour)performer).GetHeroClass());
                if (((HeroBehaviour)performer).GetPerk("Base_Berserker_MadmansLot")?.Enabled == true)
                {
                    var whirlwind = zerkClass.CurrentStance.GetAbility("Whirlwind").Ability;
                    whirlwind.UpgradedVersionCooldown += 10;
                    if (whirlwind.UpgradedVersionCooldown > whirlwind.UpgradedVersionCooldownMax)
                        whirlwind.UpgradedVersionCooldown = whirlwind.UpgradedVersionCooldownMax;
                }
                if (((HeroBehaviour)performer).GetPerk("Base_Berserker_BurningChain")?.Enabled == true)
                {
                    if (Random.Range(1, 100) <= BURNING_CHAIN_CHANCE_PERCENT)
                    {
                        performer.AddEffect(_burningChain);
                    }
                }
            }
        }
        Destroy(collider.gameObject);
    }
    protected override void CalculateAttackSpeed(float speed)
    {
        base.CalculateAttackSpeed(speed);
        _effectTimerStart = _effectTimerStartBase / speed;
        _chainswordSoundTimer = _chainswordSoundTimerBase / speed;
        _groundImpactSoundTimer = _groundImpactSoundTimerBase / speed;
        _wooshStartTimer = _wooshStartTimerBase / speed;
        _chainswordSoundTimerSecond = _chainswordSoundTimerSecondBase / speed;
        _wooshStartTimerNext = _wooshStartTimerNextBase / speed;
        _additionalTraceSpawnTime = _additionalTraceSpawnTimeBase / speed;
        _additionalTraceDespawnTime = _additionalTraceDespawnTimeBase / speed;
        _secondCameraShakeTime = _secondCameraShakeTimeBase / speed;
        try { ((BerserkerCrushingThrowKnockUp)_knockUp).AttackSpeedScale = speed; } catch { }
    }
    public override void Reset()
    {
        base.Reset();
        _timesChainswordSoundPlayed = 0;
        _groundImpactSoundPlayed = false;
        _timesForWoosh = 0;
        _additionalTraceSpawned = false;
        _particlesSpawned = false;
        _secondCameraShakePerformed = false;
    }
    #endregion
}
