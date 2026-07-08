using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EffectManager
{
    #region Variables
    private List<GameplayEffectBehaviour> _effects;
    private CharacterBehaviour _character;
    #endregion

    #region Constructors
    public EffectManager(CharacterBehaviour character)
    {
        _character = character;
        _effects = new();
    }
    #endregion

    #region Methods
    public void UpdateEffects()
    {
        foreach (var effect in _effects)
        {
            effect.OnUpdate(_character);
        }
        var toRemoveList = _effects.FindAll(eff => eff.TimeLeft <= 0 && eff.TimeLeft > -100);
        foreach (var effect in toRemoveList)
        {
            RemoveEffect(effect);
        }
    }
    public void AddEffect(GameplayEffectBehaviour effect)
    {
        GameplayEffectBehaviour existingEffect = null;
        bool stacks = false;
        foreach(var existingEff in _effects)
        {
            if(existingEff.GetType() == effect.GetType())
            {
                if (existingEff.EffectSO.IsStackable)
                {
                    stacks = true;
                }
                existingEffect = existingEff;
            }
        }
        if(existingEffect != null)
        {
            if(stacks)
            {
                existingEffect.OnStackAdded(_character);
            }
            else
            {
                RemoveEffect(existingEffect);
            }
        }
        if(stacks == false)
        {
            var newEffect = GameObject.Instantiate(effect);
            _effects.Add(newEffect);
            newEffect.OnApply(_character);
        }
    }
    public void RemoveEffect(GameplayEffectBehaviour effect)
    {
        if(effect!=null)
        {
            var effectToRemove = _effects.FirstOrDefault(e => e.GetType() == effect.GetType());
            if (effectToRemove != null)
            {
                effectToRemove.TimeLeft = 0;
                _effects.Remove(effectToRemove);
                effectToRemove.OnRemove(_character);
                try { GameObject.Destroy(effectToRemove.gameObject); } catch { }
            }
        }
    }
    public void ClearEffects()
    {
        var effectsList = _effects.ToList();
        foreach(var effect in effectsList)
        {
            RemoveEffect(effect);
        }
    }
    public List<GameplayEffectBehaviour> GetActiveEffects()
    {
        return _effects;
    }
    public void Destroy()
    {
        _effects.Clear();
        _effects = null;
        _character = null;
    }
    #endregion

    #region ModifierGeters
    public float GetMovementSpeedModifiers(float speedMod)
    {
        foreach (var effect in _effects)
        {
            speedMod = effect.GetMovementSpeedModifiers(speedMod);
        }
        return speedMod;
    }
    public virtual float GetDamageModifiers(float damageMod)
    {
        foreach (var effect in _effects)
        {
            damageMod = effect.GetDamageModifiers(damageMod);
        }
        return damageMod;
    }
    public virtual float GetAttackSpeedModifiers(float speedMod)
    {
        foreach (var effect in _effects)
        {
            speedMod = effect.GetAttackSpeedModifiers(speedMod);
        }
        return speedMod;
    }
    public virtual float GetCriticalChanceModifiers(float chanceMod)
    {
        foreach (var effect in _effects)
        {
            chanceMod = effect.GetCriticalChanceModifiers(chanceMod);
        }
        return chanceMod;
    }
    public float GetCombatManaRegenModifiers(float regenMod)
    {
        foreach (var effect in _effects)
        {
            regenMod = effect.GetCombatManaRegenModifiers(regenMod);
        }
        return regenMod;
    }
    #endregion

    #region EventHandlers
    public void OnDamageDealt(Damage damage)
    {
        foreach (var effect in _effects)
        {
            effect.OnDamageDealt(damage);
        }
    }
    public void OnDamageTaken(Damage damage)
    {
        foreach (var effect in _effects)
        {
            effect.OnDamageTaken(damage);
        }
    }
    public virtual void OnDeath(CharacterBehaviour character)
    {
        foreach (var effect in _effects)
        {
            effect.OnDeath(character);
        }
    }
    public int OnHealingReceived(int healAmount)
    {
        float healingModifier = 1;
        foreach (var effect in _effects)
        {
            healingModifier *= effect.OnHealingReceived();
        }
        return (int)(healAmount * healingModifier);
    }
    public bool CanAct()
    {
        if (_effects.Any(e => e.EffectSO.Types.Any(t => t == EffectSO.EffectType.Fear || t == EffectSO.EffectType.MoveAround || t == EffectSO.EffectType.Stun)))
            return false;
        else
            return true;
    }
    public bool CanMove()
    {
        if (_effects.Any(e => e.EffectSO.Types.Any(t => t == EffectSO.EffectType.MovementStop)))
            return false;
        else
            return true;
    }
    #endregion
}
