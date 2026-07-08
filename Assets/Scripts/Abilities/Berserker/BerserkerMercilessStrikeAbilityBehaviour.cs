using Unity.Cinemachine;
using UnityEngine;

public class BerserkerMercilessStrikeAbilityBehaviour : MovingAbilityBehaviour
{
    #region Variables
    [Header("Merciless Strike")]
    [SerializeField] private ParticleSystem _particles;
    private ParticleSystem _currentParticles;
    private bool _chainswordSoundPlayed, _groundImpactSoundPlayed;
    //additional trace
    [SerializeField] private Transform _additionalTraceBase;
    private Transform _currentAdditionalTrace;
    private bool _additionalTraceSpawned;
    //timers
    [SerializeField] private float _chainswordSoundTimerBase, _groundImpactSoundTimerBase, _effectTimerStartBase, _additionalTraceSpawnTimeBase, 
        _additionalTraceDespawnTimeBase;
    private float _chainswordSoundTimer, _groundImpactSoundTimer, _effectTimerStart, _additionalTraceSpawnTime, _additionalTraceDespawnTime;
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
            if (_performingTimer > _additionalTraceSpawnTime && _additionalTraceSpawned == false)
            {
                _additionalTraceSpawned = true;
                _currentAdditionalTrace = Instantiate(_additionalTraceBase, ((HeroBehaviour)performer).EquippedWeapon?.transform);
                _currentAdditionalTrace.transform.localPosition += new Vector3(0, 3.2f, 0);
            }
            if (_performingTimer > _additionalTraceDespawnTime && _currentAdditionalTrace != null)
            {
                Destroy(_currentAdditionalTrace.gameObject);
                _currentAdditionalTrace = null;
            }
            if (_performingTimer > _chainswordSoundTimer && _chainswordSoundPlayed == false)
            {
                ((HeroBehaviour)performer).EquippedWeapon?.PlaySound("Chainsword_Hit");
                ((HeroBehaviour)performer).EquippedWeapon?.PlaySound("Chainsword_Woosh");
                _chainswordSoundPlayed = true;
            }
            if (_performingTimer > _groundImpactSoundTimer && _groundImpactSoundPlayed == false)
            {
                ((HeroBehaviour)performer).EquippedWeapon?.PlaySound("Chainsword_Ground_Impact");
                _groundImpactSoundPlayed = true;
            }
            UpdateCustomMove(performer);
            //The following code executes once, so the 0.5f value doesn't have to be changed.
            if (_performingTimer > _effectTimerStart && _performingTimer < _effectTimerStart + 0.5f && _currentParticles == null)
            {
                _currentParticles = Instantiate(_particles, performer.transform);
                _currentParticles.transform.localPosition = new Vector3(-0.02699921f, -0.245f, 2.286002f);
                _currentParticles.transform.SetParent(null);
            }
        }
    }
    public override void LaunchAbility(CharacterBehaviour performer)
    {
        base.LaunchAbility(performer);
        performer.PlayAnimation("MercilessStrike", true);
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
                        ((PlayerBehaviour)performer).ActivateCompanionAttack();
                    }
                    else
                    {
                        damage.Amount = damage.Amount / 2;
                        character.TakeDamage(damage);
                    }
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
            performer.Stats.CurrentMana += 3;
        }
        Destroy(collider.gameObject);
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
    protected override void CalculateAttackSpeed(float speed)
    {
        base.CalculateAttackSpeed(speed);
        _customMoveTimerStart = _customMoveTimerStartBase / speed;
        _customMoveTimerEnd = _customMoveTimerEndBase / speed;
        _effectTimerStart = _effectTimerStartBase / speed;
        _groundImpactSoundTimer = _groundImpactSoundTimerBase / speed;
        _chainswordSoundTimer = _chainswordSoundTimerBase / speed;
        _additionalTraceSpawnTime = _additionalTraceSpawnTimeBase / speed;
        _additionalTraceDespawnTime = _additionalTraceDespawnTimeBase / speed;
    }
    public override void Reset()
    {
        base.Reset();
        _chainswordSoundPlayed = false;
        _groundImpactSoundPlayed = false;
        _additionalTraceSpawned = false;
    }
    #endregion
}
