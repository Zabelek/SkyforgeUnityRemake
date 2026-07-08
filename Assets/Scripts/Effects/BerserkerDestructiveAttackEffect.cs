public class BerserkerDestructiveAttackEffect : GameplayEffectBehaviour
{
    #region Methods
    public override void OnDamageTaken(Damage damage)
    {
        base.OnDamageTaken(damage);
        if (damage.Source is HeroBehaviour && ((HeroBehaviour)damage.Source)?.GetHeroClass() is BerserkerClassBehaviour)
        {
            damage.Amount = (int)(damage.Amount * 1.3f);
        }
    }
    #endregion
}
