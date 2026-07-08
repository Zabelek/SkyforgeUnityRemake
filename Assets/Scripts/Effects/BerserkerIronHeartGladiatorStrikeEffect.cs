public class BerserkerIronHeartGladiatorStrikeEffect : GameplayEffectBehaviour
{
    #region Methods
    public override void OnStackAdded(CharacterBehaviour character)
    {
        base.OnStackAdded(character);
        HealUser(character);
    }
    public override void OnApply(CharacterBehaviour character)
    {
        base.OnApply(character);
        HealUser(character);
    }
    private void HealUser(CharacterBehaviour character)
    {
        character.HealPercent(0.1f, false);
    }
    #endregion
}
