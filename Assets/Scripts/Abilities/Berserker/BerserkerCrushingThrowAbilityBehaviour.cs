using Unity.Cinemachine;
using UnityEngine;

public class BerserkerCrushingThrowAbilityBehaviour : AbilityBehaviour
{
    private const int BURNING_CHAIN_CHANCE_PERCENT = 35;

    #region Variables
    [Header("Crushing Throw")]
    [SerializeField] private ParticleSystem _particles;
    [SerializeField] private GameplayEffectBehaviour _knockUp;
    [SerializeField] private GameplayEffectBehaviour _burningChain;
    private bool _groundImpactSoundPlayed, _particlesSpawned;
    private int _timesChainswordSoundPlayed;
    //timers
    [SerializeField] private float _chainswordSoundTimerBase, _groundImpactSoundTimerBase, _chainswordSoundTimerSecondBase, _effectTimerStartBase;
    private float _chainswordSoundTimer, _groundImpactSoundTimer, _chainswordSoundTimerSecond, _effectTimerStart;
    //additional trace
    [SerializeField] private float _additionalTraceSpawnTimeBase, _additionalTraceDespawnTimeBase, _thirdTraceSpawnTimeBase, _thirdTraceDespawnTimeBase;
    private float _additionalTraceSpawnTime, _additionalTraceDespawnTime, _thirdTraceSpawnTime, _thirdTraceDespawnTime;
    [SerializeField] private Transform _additionalTraceBase;
    private Transform _currentAdditionalTrace, _currentThirdTrace;
    private bool _additionalTraceSpawned, _thirdTraceSpawned;
    #endregion

    #region Methods
    public override void Init()
    {
        base.Init();
        _producesDefaultTrace = true;
    }
    public override void UpdateAbility(CharacterBehaviour performer, HeroClassBehaviour heroClass)
    {
        base.UpdateAbility(performer, heroClass);
        if (!Finishing)
        {
            HandleTraceSpawn(performer);
            if (_timesChainswordSoundPlayed < 2 && _performingTimer > _chainswordSoundTimer + _chainswordSoundTimerSecond * _timesChainswordSoundPlayed)
            {
                ((HeroBehaviour)performer).EquippedWeapon?.PlaySound("Chainsword_Hit");
                ((HeroBehaviour)performer).EquippedWeapon?.PlaySound("Chainsword_Woosh");
                _timesChainswordSoundPlayed++;
            }
            if (_performingTimer > _groundImpactSoundTimer && _groundImpactSoundPlayed == false)
            {
                ((HeroBehaviour)performer).EquippedWeapon?.PlaySound("Chainsword_Ground_Impact");
                ((HeroBehaviour)performer).EquippedWeapon?.PlaySound("Chainsword_Chains_Short");
                _groundImpactSoundPlayed = true;
            }
            if (_performingTimer > _effectTimerStart && _particlesSpawned == false)
            {
                var currentParticles = Instantiate(_particles, performer.transform);
                currentParticles.transform.localPosition = new Vector3(-0.02699921f, -0.245f, 2.286002f);
                currentParticles.transform.SetParent(null);
                _particlesSpawned = true;
            }
        }
    }
    private void HandleTraceSpawn(CharacterBehaviour performer)
    {
        if (_performingTimer > _additionalTraceSpawnTime && _additionalTraceSpawned == false)
        {
            _additionalTraceSpawned = true;
            if (performer is HeroBehaviour && ((HeroBehaviour)performer).EquippedWeapon != null)
            {
                _currentAdditionalTrace = Instantiate(_additionalTraceBase, ((HeroBehaviour)performer).EquippedWeapon?.transform);
                _currentAdditionalTrace.transform.localPosition += new Vector3(0, 3.2f, 0);
            }
        }
        if (_performingTimer > _additionalTraceDespawnTime && _currentAdditionalTrace != null)
        {
            Destroy(_currentAdditionalTrace.gameObject);
            _currentAdditionalTrace = null;
        }
        if (_performingTimer > _thirdTraceSpawnTime && _thirdTraceSpawned == false)
        {
            _thirdTraceSpawned = true;
            if (performer is HeroBehaviour && ((HeroBehaviour)performer).EquippedWeapon != null)
            {
                _currentThirdTrace = Instantiate(_additionalTraceBase, ((HeroBehaviour)performer).EquippedWeapon?.transform);
                _currentThirdTrace.transform.localPosition += new Vector3(0, 3.2f, 0);
                _currentThirdTrace.transform.localEulerAngles += new Vector3(0, 180, 0);
            }
        }
        if (_performingTimer > _thirdTraceDespawnTime && _currentThirdTrace != null)
        {
            Destroy(_currentThirdTrace.gameObject);
            _currentThirdTrace = null;
        }
    }
    public override void LaunchAbility(CharacterBehaviour performer)
    {
        base.LaunchAbility(performer);
        performer.PlayAnimation("CrushingThrow", true);
    }
    public override void Interrupt(CharacterBehaviour performer)
    {
        base.Interrupt(performer);
        if (_currentAdditionalTrace != null)
        {
            Destroy(_currentAdditionalTrace.gameObject);
            _currentAdditionalTrace = null;
        }
        if (_currentThirdTrace != null)
        {
            Destroy(_currentThirdTrace.gameObject);
            _currentThirdTrace = null;
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
            performer.Stats.CurrentMana += 10;
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
        _effectTimerStart = _effectTimerStartBase / speed;
        _chainswordSoundTimer = _chainswordSoundTimerBase / speed;
        _groundImpactSoundTimer = _groundImpactSoundTimerBase / speed;
        _chainswordSoundTimerSecond = _chainswordSoundTimerSecondBase / speed;
        if(_knockUp is BerserkerCrushingThrowKnockUp)
            ((BerserkerCrushingThrowKnockUp)_knockUp).AttackSpeedScale = speed;
        _additionalTraceSpawnTime = _additionalTraceSpawnTimeBase / speed;
        _additionalTraceDespawnTime = _additionalTraceDespawnTimeBase / speed;
        _thirdTraceSpawnTime = _thirdTraceSpawnTimeBase / speed;
        _thirdTraceDespawnTime = _thirdTraceDespawnTimeBase / speed;
    }
    public override void Reset()
    {
        base.Reset();
        _timesChainswordSoundPlayed = 0;
        _groundImpactSoundPlayed = false;
        _additionalTraceSpawned = false;
        _thirdTraceSpawned = false;
        _particlesSpawned = false;
    }
    #endregion
}
