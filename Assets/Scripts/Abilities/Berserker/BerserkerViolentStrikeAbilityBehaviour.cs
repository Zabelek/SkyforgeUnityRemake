using Unity.Cinemachine;
using UnityEngine;

public class BerserkerViolentStrikeAbilityBehaviour : MovingAbilityBehaviour
{
    #region Variables
    private bool _chainswordSoundPlayed;
    [Header("Violent Strike")]
    //additional trace
    [SerializeField] private Transform _additionalTraceBase;
    private Transform _currentAdditionalTrace;
    private bool _additionalTraceSpawned;
    //timers
    [SerializeField] private float _chainswordSoundTimerBase, _additionalTraceSpawnTimeBase, _additionalTraceDespawnTimeBase;
    private float _chainswordSoundTimer, _additionalTraceSpawnTime, _additionalTraceDespawnTime;
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
            UpdateCustomMove(performer);
        }
    }
    public override void LaunchAbility(CharacterBehaviour performer)
    {
        base.LaunchAbility(performer);
        performer.PlayAnimation("ViolentStrike", true);
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
                    casualityAmount++;
                }
            }
        }
        if (TryGetComponent<CinemachineImpulseSource>(out CinemachineImpulseSource imp))
        {
            imp.GenerateImpulse();
        }
        if (casualityAmount>0)
        {
            performer.Stats.CurrentMana += 3;
        }
        Destroy(collider.gameObject);
    }
    protected override void CalculateAttackSpeed(float speed)
    {
        base.CalculateAttackSpeed(speed);
        _customMoveTimerStart = _customMoveTimerStartBase / speed;
        _customMoveTimerEnd = _customMoveTimerEndBase / speed;
        _chainswordSoundTimer = _chainswordSoundTimerBase / speed;
        _additionalTraceSpawnTime = _additionalTraceSpawnTimeBase / speed;
        _additionalTraceDespawnTime = _additionalTraceDespawnTimeBase / speed;
    }
    public override void Reset()
    {
        base.Reset();
        _chainswordSoundPlayed = false;
        _additionalTraceSpawned = false;
    }
    #endregion
}
