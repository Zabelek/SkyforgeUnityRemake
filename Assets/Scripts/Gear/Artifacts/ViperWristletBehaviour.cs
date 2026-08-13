using System;
using System.Linq;
using UnityEngine;

public class ViperWristletBehaviour : GearPeaceBehaviour
{
    #region Variables
    public ArtifactSO ArtifactSO;
    [SerializeField] private GameplayEffectBehaviour _baseEffect, _damageEffect, _shieldEffect;
    private HeroBehaviour _hero;
    #endregion

    #region Mono
    protected override void Awake()
    {
        base.Awake();
        GearBonus.HealthPercentBonus += ArtifactSO.HealthBonus;
        GearBonus.DamagePercentBonus += ArtifactSO.DamageBonus;
    }
    private void Update()
    {
        if(_hero?.Stats.CurrentHP < (int)(_hero?.Stats.MaxHP * 0.15f) && _hero.IsMenuPreview == false)
        {
            var effect = _hero.GetActiveEffects().FirstOrDefault(e => e.EffectSO.ID == _baseEffect.EffectSO.ID);
            if(effect?.TimeLeft == -100)
            {
                effect.TimeLeft = 90;
                _hero.AddEffect(_shieldEffect);
            }
        }
    }
    public override void Equip(HeroBehaviour hero, bool onlyVisual)
    {
        base.Equip(hero, onlyVisual);
        _hero = hero;
        _hero.AddEffect(_baseEffect);
        _hero.OnCombatStartEvent += CombatStartedAction;
        _hero.OnResurrectEvent += ResurrectedAction;
    }

    private void ResurrectedAction(object sender, EventArgs e)
    {
        _hero.AddEffect(_baseEffect);
    }

    public override void Unequip(HeroBehaviour hero, bool onlyVisual)
    {
        _hero.OnCombatStartEvent -= CombatStartedAction;
        _hero.OnResurrectEvent -= ResurrectedAction;
        _hero.RemoveEffect(_baseEffect);
        _hero = null;
        base.Unequip(hero, onlyVisual);
    }
    #endregion

    #region EventHandlers
    private void CombatStartedAction(object sender, CharacterBehaviour.StartCombatEventArgs e)
    {
        _hero.AddEffect(_damageEffect);
    }
    #endregion
}
