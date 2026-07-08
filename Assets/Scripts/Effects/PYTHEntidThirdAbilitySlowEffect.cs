public class PYTHEntidThirdAbilitySlowEffect : GameplayEffectBehaviour
{
    #region Methods
    public override float GetMovementSpeedModifiers(float speedMod)
    {
        return speedMod * 0.5f;
    }
    #endregion
}
