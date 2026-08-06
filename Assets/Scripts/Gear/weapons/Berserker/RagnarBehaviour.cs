using System;
using System.Linq;
using UnityEngine;

public class RagnarBehaviour : WeaponBehaviour
{
    #region Variables
    [SerializeField] private GameplayEffectBehaviour _bloodlustEffectBase;
    private AbilityBehaviour _whirlwindAbility;
    #endregion

    #region Methods
    public override void Equip(HeroBehaviour hero, Transform slot, bool onlyVisual)
    {
        base.Equip(hero, slot, onlyVisual);
        if(!onlyVisual)
        {
            hero.OnEnemyKill += EnemyKilled;
            hero.OnResurrect += AddFreshEffect;
            hero.AddEffect(_bloodlustEffectBase);
            _whirlwindAbility = hero.GetHeroClass()?.GetAbilityFromAnyStance("Whirlwind");
            if (_whirlwindAbility != null)
            {
                _whirlwindAbility.OnAbilityStart += ModWhirlwindDamage;
            }
        }
    }
    public override void Unequip(HeroBehaviour hero, bool onlyVisual)
    {
        base.Unequip(hero, onlyVisual);
        if (!onlyVisual)
        {
            hero.OnEnemyKill -= EnemyKilled;
            hero.OnResurrect -= AddFreshEffect;
            hero.RemoveEffect(_bloodlustEffectBase);
            if (_whirlwindAbility != null)
            {
                _whirlwindAbility.OnAbilityStart -= ModWhirlwindDamage;
            }
        }
    }
    #endregion

    #region EventHandlers
    private void EnemyKilled(object sender, CharacterBehaviour.EnemyKillEventArgs e)
    {
        //The strength of the enemy is estimated based on the amount of health bars. If the enemy drops just one, healing orbs dropping is checked.
        //The weakedt enemies don't drop ones, so there will be less stacks for them. If the health bars are greater than one, there will be a stack for each of them
        if(sender is HeroBehaviour)
        {
            var hero = sender as HeroBehaviour;
            int bars = e.KilledCharacter.CharacterSO.Category.HealthBarsAmount;
            if (bars == 1)
            {
                if (e.KilledCharacter.CharacterSO.Category.DropsHealingOrbs)
                {
                    hero.AddEffectStacks(_bloodlustEffectBase, 3, 100);
                }
                else
                {
                    hero.AddEffectStacks(_bloodlustEffectBase, 1, 100);
                }
            }
            else
            {
                hero.AddEffectStacks(_bloodlustEffectBase, (short)bars, 100);
            }
        }
    }
    private void AddFreshEffect(object sender, EventArgs e)
    {
        if (sender is CharacterBehaviour)
        {
            (sender as CharacterBehaviour).AddEffect(_bloodlustEffectBase);
        }
    }
    private void ModWhirlwindDamage(object sender, AbilityBehaviour.AbilityStateEventArgs e)
    {
        if(sender is AbilityBehaviour)
        {
            var effect = e.Performer.GetActiveEffects().FirstOrDefault(e => e.EffectSO.ID == _bloodlustEffectBase.EffectSO.ID);
            if(effect != null)
            {
                (sender as AbilityBehaviour).ExternalDamageMultiplier = 1 + effect.Stacks * 0.04f;
            }
        }
    }
    #endregion
}
