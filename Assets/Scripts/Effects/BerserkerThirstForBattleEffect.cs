using UnityEngine;

public class BerserkerThirstForBattleEffect : GameplayEffectBehaviour
{
    #region Variables
    private float _currentRedAmount;
    private bool _isIncreasing;
    #endregion

    #region Methods
    public override void OnUpdate(CharacterBehaviour character)
    {
        base.OnUpdate(character);
        //slow glow pulsation
        if(_isIncreasing)
        {
            _currentRedAmount += Time.fixedDeltaTime;
        }
        else
        {
            _currentRedAmount -= Time.fixedDeltaTime;
        }
        if (_currentRedAmount > 0.6)
            _isIncreasing = false;
        else if (_currentRedAmount < 0)
            _isIncreasing = true;
        character.SetGlow(_currentRedAmount, Color.red);
    }
    public override float GetDamageModifiers(float damageMod)
    {
        return damageMod * 1.4f;
    }
    public override float GetCriticalChanceModifiers(float chanceMod)
    {
        return chanceMod + 0.3f;
    }
    public override void OnRemove(CharacterBehaviour character)
    {
        base.OnRemove(character);
        character.SetGlow(0);
    }
    #endregion
}
