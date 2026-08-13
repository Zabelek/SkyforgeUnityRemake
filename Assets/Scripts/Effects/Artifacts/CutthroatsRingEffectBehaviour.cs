public class CutthroatsRingEffectBehaviour : GameplayEffectBehaviour
{
    public override float GetCriticalChanceModifiers(float chanceMod)
    {
        chanceMod += 0.05f * Stacks;
        return chanceMod;
    }
}
