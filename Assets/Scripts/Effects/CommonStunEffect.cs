public class CommonStunEffect : GameplayEffectBehaviour
{
    #region EventHandlers
    public override void OnUpdate(CharacterBehaviour character)
    {
        base.OnUpdate(character);
        character.SetCanMove(false);
        character.SetCanAct(false, false);
        if (character is MonsterBehaviour)
        {
            ((MonsterBehaviour)character).InterruptAbilities();
        }
        else if(character is HeroBehaviour)
        {
            ((HeroBehaviour)character).CancelAllAbilities();
            if (character is PlayerBehaviour)
            {
                ((PlayerBehaviour)character).CanDash = false;
            }
        }
    }
    public override void OnRemove(CharacterBehaviour character)
    {
        base.OnRemove(character);
        character.SetCanMove(true);
        character.SetCanAct(true);
        if (character is PlayerBehaviour)
        {
            ((PlayerBehaviour)character).CanDash = true;
        }
    }
    #endregion
}
