public class ViperWristletCooldown : GameplayEffectBehaviour
{
    public override void OnUpdate(CharacterBehaviour character)
    {
        base.OnUpdate(character);
        if (TimeLeft <= 0)
            TimeLeft = -100;
    }
}
