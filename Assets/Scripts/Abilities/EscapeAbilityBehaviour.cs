using System.Linq;
using UnityEngine;

public class EscapeAbilityBehaviour : AbilityBehaviour
{
    #region Variables
    [HideInInspector] public float EscapeVariantCooldownMax, EscapeVariantCooldown;
    #endregion

    #region Methods
    public override void Init()
    {
        base.Init();
        EscapeVariantCooldownMax = AbilitySO.EscapeCooldown;
    }
    public virtual bool CheckPerformAvailabilityEscape(CharacterBehaviour performer)
    {
        if (performer.GetActiveEffects().Any(e => e.EffectSO.Types.Any(t => t == EffectSO.EffectType.Slow || t == EffectSO.EffectType.Fear
            || t == EffectSO.EffectType.Stun || t == EffectSO.EffectType.MoveAround)) && EscapeVariantCooldownMax > 0 
            && EscapeVariantCooldown >= EscapeVariantCooldownMax)
        {
            return true;
        }
        else return false;
    }
    public virtual void LaunchAbilityEscape(CharacterBehaviour performer)
    {
        //cooldown is not zeroed, as escape abilities are "additional usages" of the main ability.
        var tempCooldown = CurrentCooldown;
        base.LaunchAbility(performer);
        CurrentCooldown = tempCooldown;
    }
    public override void UpdateCooldown()
    {
        base.UpdateCooldown();
        if(EscapeVariantCooldownMax > 0)
        {
            if(EscapeVariantCooldown < EscapeVariantCooldownMax)
            {
                EscapeVariantCooldown += Time.fixedDeltaTime;
                if (EscapeVariantCooldown > EscapeVariantCooldownMax)
                    EscapeVariantCooldown = EscapeVariantCooldownMax;
            }
        }
    }
    #endregion
}
