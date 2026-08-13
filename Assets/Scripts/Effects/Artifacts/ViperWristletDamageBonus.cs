public class ViperWristletDamageBonus : GameplayEffectBehaviour
{
    public override void OnDamageDealt(Damage damage)
    {
        damage.Amount = (int)(damage.Amount * 1.2f);
    }
}
