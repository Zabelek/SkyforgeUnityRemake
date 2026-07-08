using UnityEngine;

public class PYTHDioneAttackAbilityBehaviour : AbilityBehaviour
{
    #region Methods
    public override void PerformHit(CharacterBehaviour performer)
    {
        base.PerformHit(performer);
        Physics.SyncTransforms();
        var potentialCasualities = Physics.OverlapSphere(performer.transform.position, 6);
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
                    break;
                }
            }
        }
        Destroy(collider.gameObject);
        performer.SpeakingBehaviour?.PerformCombatShout(1f);
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
    }
    #endregion
}
