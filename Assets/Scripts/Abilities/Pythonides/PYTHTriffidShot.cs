using System.Linq;
using UnityEngine;

public class PYTHTriffidShot : AbilityBehaviour
{
    #region Variables
    public ProjectileBehaviour _projectilePrefab;
    #endregion

    #region Methods
    public override void LaunchAbility(CharacterBehaviour performer)
    {
        base.LaunchAbility(performer);
        if (performer.TryGetComponent<MonsterAnimationBehaviour>(out var animationBehav))
        {
            animationBehav.TriggerAttackAnimation();
        }
    }
    public override void PerformHit(CharacterBehaviour performer)
    {
        base.PerformHit(performer);
        if(performer.ActiveEnemies.Any())
        {
            var newProjectile = Instantiate(_projectilePrefab, performer.transform.position, performer.transform.rotation);
            newProjectile.Caster = performer;
            newProjectile.Target = performer.ActiveEnemies[Random.Range(0, performer.ActiveEnemies.Count()-1)];
            newProjectile.Damage = CalculateDamage(new Damage(performer, performer.GetEffectiveDamage()),
                performer.GetEffectiveCriticalChance(), performer.Stats.GearStats.CriticalDamageBonus);
            newProjectile.Damage.Range = true;
        }
    }
    #endregion
}
