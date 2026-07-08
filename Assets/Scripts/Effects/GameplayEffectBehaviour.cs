using UnityEngine;

public class GameplayEffectBehaviour : MonoBehaviour
{
    #region Variables
    [Header("Effect Related Variables")]
    [Tooltip("Effect Scriptable Objech with base information")]
    public EffectSO EffectSO;
    [Tooltip("-100 means it doesn't have a timer. Setting TimeLeft to -1 is a signal to EffectManager to remove the effect.")]
    public float TimeLeft;
    [HideInInspector] public short Stacks = 1;
    #endregion

    #region EventHandlers
    public virtual void OnApply(CharacterBehaviour character)
    {

    }
    public virtual void OnRemove(CharacterBehaviour character)
    {

    }
    public virtual void OnUpdate(CharacterBehaviour character)
    {
        TimeLeft -= Time.fixedDeltaTime;
    }
    public virtual void OnDamageTaken(Damage damage)
    {

    }
    public virtual void OnDamageDealt(Damage damage)
    {

    }
    public virtual void OnDeath(CharacterBehaviour character)
    {

    }
    public virtual void OnStackAdded(CharacterBehaviour character)
    {
        Stacks++;
    }
    #endregion

    #region ModifierGetters
    public virtual float OnHealingReceived()
    {
        return 1;
    }
    public virtual float GetMovementSpeedModifiers(float speedMod)
    {
        return speedMod;
    }
    public virtual float GetDamageModifiers(float damageMod)
    {
        return damageMod;
    }
    public virtual float GetAttackSpeedModifiers(float speedMod)
    {
        return speedMod;
    }
    public virtual float GetCriticalChanceModifiers(float chanceMod)
    {
        return chanceMod;
    }
    public virtual float GetCombatManaRegenModifiers(float regenMod)
    {
        return regenMod;
    }
    #endregion
}
