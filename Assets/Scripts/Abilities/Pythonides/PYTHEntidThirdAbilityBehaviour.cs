using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class PYTHEntidThirdAbilityBehaviour : AbilityBehaviour
{
    #region Variables
    [Header("Entid Step Ability")]
    [SerializeField] private DecalProjector _projector;
    [SerializeField] private Transform _projectorContainer;
    private DecalProjector _currentProjector;
    private Transform _currentProjectorContainer;
    private bool _effectsSpawned, _projectorFadedOut;
    [SerializeField] private GameplayEffectBehaviour _slowEffect;
    [SerializeField] private SoundEffectSO _soundEffect;
    //timers
    [SerializeField] private float _projectorFadeoutTimer;
    #endregion

    #region Methods
    public override void UpdateAbility(CharacterBehaviour performer, HeroClassBehaviour heroClass)
    {
        base.UpdateAbility(performer, heroClass);
        if (_performingTimer >= _untilHitTimer && !_effectsSpawned)
        {
            _effectsSpawned = true;
            _currentProjectorContainer = Instantiate(_projectorContainer, performer.transform.position, performer.transform.rotation);
            _currentProjector = _currentProjectorContainer.GetComponentInChildren<DecalProjector>();
            if (TryGetComponent<CinemachineImpulseSource>(out CinemachineImpulseSource imp))
            {
                imp.GenerateImpulse();
            }
            if (_soundEffect != null)
                Globals.Instance.SoundManager.PlaySFXFast(_soundEffect, performer.transform.position);
        }
        if (_performingTimer >= _projectorFadeoutTimer && _projectorFadedOut == false)
        {
            var factor = 1 - (_performingTimer - _projectorFadeoutTimer);
            if (factor <= 0 && _currentProjectorContainer != null)
            {
                _projectorFadedOut = true;
                Destroy(_currentProjectorContainer.gameObject);
            }
            else if (_currentProjector != null)
            {
                _currentProjector.fadeFactor = factor;
            }
        }
    }
    public override void LaunchAbility(CharacterBehaviour performer)
    {
        base.LaunchAbility(performer);
        performer.PlayAnimation("Skill_Step", true);
    }
    public override void PerformHit(CharacterBehaviour performer)
    {
        base.PerformHit(performer);
        Physics.SyncTransforms();
        var potentialCasualities = Physics.OverlapSphere(performer.transform.position, 7);
        var collider = Instantiate(transform.Find("Collider").GetComponent<Collider>(), performer.transform, false);
        if (collider != null)
        {
            foreach (var casuality in potentialCasualities)
            {
                if (casuality == collider || casuality == performer.GetComponent<Collider>())
                    continue;
                if (casuality.bounds.Intersects(collider.bounds) && CharacterBehaviour.FindEnemyCharacterInCollider(casuality, performer, out var character))
                {
                    var damage = CalculateDamage(new Damage(performer, performer.GetEffectiveDamage() * 4, false, false), performer.GetEffectiveCriticalChance());
                    character.TakeDamage(damage);
                    character.AddEffect(_slowEffect);
                    break;
                }
            }
        }
        Destroy(collider.gameObject);    
    }
    public override void Reset()
    {
        base.Reset();
        _effectsSpawned = false;
        _projectorFadedOut = false;
    }
    #endregion
}
