using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public class BerserkerWildLeapAbilityBehaviour : EscapeAbilityBehaviour
{
    private const float MAX_JUMP_LENGTH = 24;

    #region Variables 
    private Vector3 _destinationVector, _initialPlayerPosition;
    private Vector3 _alreadyMovedAmount;
    [Header("Wild Leap")]
    [SerializeField] private float _archHeight = 1.5f;
    [SerializeField] private ParticleSystem _particles;
    private bool _chainswordSoundPlayed, _groundImpactSoundPlayed;
    private ParticleSystem _currentParticles;
    //timers
    [SerializeField] private float _startLeapTimerBase, _chainswordSoundTimerBase, _groundImpactSoundTimerBase, _effectTimerStartBase;
    private float _startLeapTimer, _chainswordSoundTimer, _groundImpactSoundTimer, _effectTimerStart;
    #endregion

    #region Methods
    public override void Init()
    {
        base.Init();
        _producesDefaultTrace = false; 
    }
    public override void UpdateAbility(CharacterBehaviour performer, HeroClassBehaviour heroClass)
    {
        base.UpdateAbility(performer, heroClass);
        if (!Finishing)
        {
            if (_performingTimer >= _startLeapTimer && _performingTimer < _untilHitTimer)
            {
                if (performer.GetComponent<Rigidbody>()?.isKinematic == false)
                {
                    performer.GetComponent<Rigidbody>().isKinematic = true;
                }
                var timeMod = (_performingTimer - _startLeapTimer) / (_untilHitTimer - _startLeapTimer);
                var vec = Vector3.Lerp(_initialPlayerPosition, _destinationVector, timeMod);
                vec -= _initialPlayerPosition;
                vec.y += Mathf.Sin((timeMod * 1.1f) * Mathf.PI) * _archHeight;
                vec = vec - _alreadyMovedAmount;
                _alreadyMovedAmount += vec;
                performer.MoveBySkill(vec, MovingAbilityBehaviour.MovingType.Raw);
            }
            if (_currentParticles == null && _performingTimer >= _effectTimerStart)
            {
                _currentParticles = Instantiate(_particles, performer.transform);
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
        }
    }
    public override void Interrupt(CharacterBehaviour performer)
    {
        base.Interrupt(performer);
        PullPerformerToTheGround(performer);
    }
    public override void LaunchAbility(CharacterBehaviour performer)
    {
        base.LaunchAbility(performer);
        AfterAnyLaunch(performer);
    }
    public override void LaunchAbilityEscape(CharacterBehaviour performer)
    {
        base.LaunchAbilityEscape(performer);
        AfterAnyLaunch(performer);
    }
    private void AfterAnyLaunch(CharacterBehaviour performer)
    {
        DetermineDestinationVector(performer);
        _initialPlayerPosition = performer.transform.position;
        var fowardDir = new Vector3(_destinationVector.x, performer.transform.position.y, _destinationVector.z);
        performer.FaceTheTarget((fowardDir - performer.transform.position).normalized);
        RemoveAllImmobilizingEffects(performer);
        performer.PlayAnimation("WildLeap", true);
    }
    private void RemoveAllImmobilizingEffects(CharacterBehaviour performer)
    {
        var effectsToRemove = new List<GameplayEffectBehaviour>();
        foreach(var effect in performer.GetActiveEffects())
        {
            if(effect.EffectSO.Types.Any(t => t == EffectSO.EffectType.Fear || t == EffectSO.EffectType.MoveAround 
            || t == EffectSO.EffectType.Stun || t == EffectSO.EffectType.Slow))
            {
                effectsToRemove.Add(effect);
            }
        }
        foreach (var effect in effectsToRemove)
        {
            performer.RemoveEffect(effect);
        }
    }
    private void DetermineDestinationVector(CharacterBehaviour performer)
    {
        CharacterBehaviour target = null;
        if(performer is PlayerBehaviour)
            target = ((PlayerBehaviour)performer).SelectedCharacter;
        if (target == null)
        {
            var cam = Globals.Instance.ViewportCamera;
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.55f, 0f));
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, LayerMask.GetMask("Default")))
            {
                if ((hit.point - performer.transform.position).magnitude > MAX_JUMP_LENGTH
                    && (cam.transform.position - hit.point).magnitude > (cam.transform.position - performer.transform.position).magnitude)
                {
                    hit.point = (performer.transform.position + (hit.point - performer.transform.position).normalized * MAX_JUMP_LENGTH);
                }
                if (NavMesh.SamplePosition(hit.point, out NavMeshHit newhit, 5f, LayerMask.GetMask("Default"))
                    && (cam.transform.position - hit.point).magnitude > (cam.transform.position - performer.transform.position).magnitude)
                {
                    _destinationVector = newhit.position;
                    return;
                }
            }
            else
            {
                _destinationVector = performer.transform.position;
            }
        }
        else
        {
            if ((target.transform.position - performer.transform.position).magnitude > 24)
            {
                var targetDest = (performer.transform.position + (target.transform.position - performer.transform.position).normalized * 24);
                if (NavMesh.SamplePosition(targetDest, out NavMeshHit newhit, 5f, LayerMask.GetMask("Default")))
                {
                    _destinationVector = new Vector3(newhit.position.x, newhit.position.y + 0.5f, newhit.position.z);
                    return;
                }
                _destinationVector = performer.transform.position;
            }
            else
            {
                var targetDest = target.transform.position;
                if (NavMesh.SamplePosition(targetDest, out NavMeshHit newhit, 5f, LayerMask.GetMask("Default")))
                {
                    _destinationVector = new Vector3(newhit.position.x, newhit.position.y + 0.5f, newhit.position.z);
                    return;
                }
                _destinationVector = performer.transform.position;
            }
        }
    }
    private void PullPerformerToTheGround(CharacterBehaviour performer)
    {
        if (NavMesh.SamplePosition(performer.transform.position, out NavMeshHit newhit, 5f, LayerMask.GetMask("Default")))
        {
            performer.MovePosition(new Vector3(newhit.position.x, newhit.position.y, newhit.position.z));
        }
        var rigidbody = performer.GetComponent<Rigidbody>();
        if (rigidbody?.isKinematic == true)
        {
            rigidbody.isKinematic = false;
            rigidbody.linearVelocity = Vector3.zero;
        }
    }
    public override void PerformHit(CharacterBehaviour performer)
    {
        PullPerformerToTheGround(performer);
        base.PerformHit(performer);
        Physics.SyncTransforms();
        var potentialCasualities = Physics.OverlapSphere(performer.transform.position, 5);
        var collider = Instantiate(transform.Find("Collider").GetComponent<Collider>(), performer.transform, false);
        if (collider != null)
        {
            foreach (var casuality in potentialCasualities)
            {
                if (casuality == collider || casuality == performer.GetComponent<Collider>())
                    continue;
                if (casuality.bounds.Intersects(collider.bounds) && CharacterBehaviour.FindEnemyCharacterInCollider(casuality, performer, out var character))
                {
                    var damage = CalculateDamage(new Damage(performer, performer.GetEffectiveDamage(), false, false), performer.GetEffectiveCriticalChance());
                    character.TakeDamage(damage);
                }
            }
        }
        if (TryGetComponent<CinemachineImpulseSource>(out CinemachineImpulseSource imp))
        {
            imp.GenerateImpulse();
        }
        Destroy(collider.gameObject);
    }
    protected override void CalculateAttackSpeed(float speed)
    {
        base.CalculateAttackSpeed(speed);
        _startLeapTimer = _startLeapTimerBase;
        _effectTimerStart = _effectTimerStartBase;
        _chainswordSoundTimer = _chainswordSoundTimerBase;
        _groundImpactSoundTimer = _groundImpactSoundTimerBase;
    }
    public override void Reset()
    {
        base.Reset();
        _alreadyMovedAmount = Vector3.zero;
        _chainswordSoundPlayed = false;
        _groundImpactSoundPlayed = false;
    }
    #endregion
}
