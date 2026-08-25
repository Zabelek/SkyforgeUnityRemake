using UnityEngine;

public class PYTHTriffidBossAreaAttackAbilityBehaviour : AbilityBehaviour
{
    #region Variables
    [SerializeField] private ParticleSystem _particlesBase;
    private bool _particlesSpawned;
    [SerializeField] private float _timesBetweenHits, _hitAreaRadious;
    [SerializeField] private int _maxTimesHit;
    private int _timesAlreadyHit;
    #endregion

    #region Methods
    public override void UpdateAbility(CharacterBehaviour performer, HeroClassBehaviour heroClass)
    {
        base.UpdateAbility(performer, heroClass);
        if(_performingTimer > (_untilHitTimer + (_timesBetweenHits * _timesAlreadyHit)) && _timesAlreadyHit< _maxTimesHit)
        {
            PerformHit(performer);
        }
    }
    public override void LaunchAbility(CharacterBehaviour performer)
    {
        base.LaunchAbility(performer);
        performer.PlayAnimation("Skill2");
    }
    public override void PerformHit(CharacterBehaviour performer)
    {
        base.PerformHit(performer);
        _timesAlreadyHit++;
        if(!_particlesSpawned)
        {
            _particlesSpawned = true;
            var particles = Instantiate(_particlesBase, performer.transform);
            particles.gameObject.SetActive(true);
        }
        foreach(var collider in Physics.OverlapSphere(performer.transform.position, _hitAreaRadious))
        {
            if(CharacterBehaviour.FindEnemyCharacterInCollider(collider, performer, out var casuality))
            {
                var damageMultiplier = 1 - ((casuality.transform.position - performer.transform.position).magnitude / _hitAreaRadious);
                var damage = CalculateDamage(new Damage(performer, performer.GetEffectiveDamage(), false, true),
                    performer.GetEffectiveCriticalChance(), performer.Stats.GearStats.CriticalDamageBonus);
                damage.Amount = (int)(damage.Amount * damageMultiplier);
                casuality.TakeDamage(damage);
            }
        }
    }
    public override void Reset()
    {
        base.Reset();
        _timesAlreadyHit = 0;
        _particlesSpawned = false;
    }
    #endregion
}
