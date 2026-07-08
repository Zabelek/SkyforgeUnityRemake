public class BerserkerCripplingBlowStunEffect : GameplayEffectBehaviour
{
    #region Methods
    public override void OnUpdate(CharacterBehaviour character)
    {
        base.OnUpdate(character);
        character.SetCanMove(false);
        character.SetCanAct(false, false);
    }
    public override void OnRemove(CharacterBehaviour character)
    {
        base.OnRemove(character);
        character.SetCanMove(true);
        character.SetCanAct(true);
    }
    #endregion
}
