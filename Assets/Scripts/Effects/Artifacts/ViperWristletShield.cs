using UnityEngine;

public class ViperWristletShield : GameplayEffectBehaviour
{
    #region Variables
    private float _nextHeal;
    private short _healsAmount;
    #endregion

    #region Methods
    public override void OnApply(CharacterBehaviour character)
    {
        base.OnApply(character);
        _nextHeal = 0;
        _healsAmount = 0;
    }
    public override void OnUpdate(CharacterBehaviour character)
    {
        base.OnUpdate(character);
        if(_nextHeal <= 0 && _healsAmount<6)
        {
            _nextHeal = 1;
            _healsAmount++;
            character.HealPercent(0.06f, true);
        }
        _nextHeal -= Time.fixedDeltaTime;
    }
    public override void OnDamageTaken(Damage damage)
    {
        damage.Amount -= (int)(damage.Amount * 0.8f);
    }
    public override void OnRemove(CharacterBehaviour character)
    {
        if(_healsAmount < 6 && !character.IsDead)
            character.HealPercent(0.06f, true);
        base.OnRemove(character);
    }
    #endregion
}
