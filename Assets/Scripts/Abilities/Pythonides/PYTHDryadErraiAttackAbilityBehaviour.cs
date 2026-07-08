using UnityEngine;

public class PYTHDryadErraiAttackAbilityBehaviour : AbilityBehaviour
{
    private const float EXPLOSION_ORIGIN_DISTANCE = 3.5f;
    private const float EXPLOSION_HEIGHT_OVERRIDE = 0.5f;

    #region Variables
    [Header("Dryad Errai Strike")]
    [SerializeField] private float _timeBetweenExplisions;
    [SerializeField] private Collider _explosionCollider;
    [SerializeField] private ParticleSystem _particlesBase;
    [SerializeField] private ParticleSystem _explosionParticlesBase;
    [SerializeField] private SoundEffectSO _explosionSoundEffect;
    [SerializeField] private SoundEffectSO _impactSoundEffect;
    private float _explosionsAlreadyDone;
    private Vector3 _initialExplosionPosition;
    private ParticleSystem _currentParticles;
    #endregion

    #region Methods
    public override void UpdateAbility(CharacterBehaviour performer, HeroClassBehaviour heroClass)
    {
        base.UpdateAbility(performer, heroClass);
        if (!Finishing)
        {
            if (_performingTimer >= _untilHitTimer + _explosionsAlreadyDone * _timeBetweenExplisions && _hitPerformed && _explosionsAlreadyDone < 3)
            {
                if (_explosionsAlreadyDone == 0)
                {
                    _initialExplosionPosition = performer.transform.position + performer.transform.forward * EXPLOSION_ORIGIN_DISTANCE
                        + new Vector3(0, EXPLOSION_HEIGHT_OVERRIDE, 0);
                    _currentParticles = Instantiate(_particlesBase, _initialExplosionPosition, Quaternion.Euler(new Vector3(0, 0, 0)));
                    Globals.Instance.SoundManager.PlaySFXFast(_impactSoundEffect, _initialExplosionPosition);
                }
                Instantiate(_explosionParticlesBase, _initialExplosionPosition, Quaternion.Euler(new Vector3(0, 0, 0)));
                Globals.Instance.SoundManager.PlaySFXFast(_explosionSoundEffect, _initialExplosionPosition);
                _explosionsAlreadyDone++;
                if (_explosionsAlreadyDone == 3)
                {
                    Destroy(_currentParticles.gameObject);
                }
                PerformExplosion(performer, _initialExplosionPosition);
            }
        }
    }
    private void PerformExplosion(CharacterBehaviour performer, Vector3 explosionCenter)
    {
        var collider = Instantiate(_explosionCollider, explosionCenter, Quaternion.Euler(new Vector3(0, 0, 0)));
        collider.gameObject.SetActive(true);
        Physics.SyncTransforms();
        var potentialCasualities = Physics.OverlapSphere(performer.transform.position, 9);
        if (collider != null)
        {
            foreach (var casuality in potentialCasualities)
            {
                if (casuality == collider || casuality == performer.GetComponent<Collider>())
                    continue;
                if (casuality.bounds.Intersects(collider.bounds) && CharacterBehaviour.FindEnemyCharacterInCollider(casuality, performer, out var character))
                {
                    var damage = CalculateDamage(new Damage(performer, performer.GetEffectiveDamage() * 1, false, false), performer.GetEffectiveCriticalChance());
                    character.TakeDamage(damage);
                    break;
                }
            }
        }
        Destroy(collider.gameObject);
    }
    public override void LaunchAbility(CharacterBehaviour performer)
    {
        base.LaunchAbility(performer);
        if (performer.TryGetComponent<MonsterAnimationBehaviour>(out var animationBehav))
        {
            animationBehav.TriggerAttackAnimation();
        }
    }
    public override void Reset()
    {
        base.Reset();
        _explosionsAlreadyDone = 0;
        _initialExplosionPosition = Vector3.zero;
        _currentParticles = null;
    }
    #endregion
}
