public class BerserkerDissectionSlowEffect : GameplayEffectBehaviour
{
    #region Methods
    public override float GetMovementSpeedModifiers(float speedMod)
    {
        return speedMod * 0.1f;
    }
    #endregion
}
