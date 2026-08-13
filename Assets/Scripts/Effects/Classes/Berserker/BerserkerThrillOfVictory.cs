public class BerserkerThrillOfVictory : GameplayEffectBehaviour
{
    #region Variables
    public override void OnDamageTaken(Damage damage)
    {
        if(damage.Source is HeroBehaviour && (damage.Source as HeroBehaviour).GetHeroClass() is BerserkerClassBehaviour)
        {
            damage.Amount = (int)(damage.Amount * 1.3f);
        }
        base.OnDamageTaken(damage);
    }
    #endregion
}
