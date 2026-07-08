using System;
using UnityEngine;

public class IronHeartBehaviour : WeaponBehaviour
{
    #region Variables
    [Header("Iron Heart Related Variables")]
    [SerializeField] private BerserkerIronHeartGladiatorStrikeEffect _ironHeartEffect;
    [HideInInspector] public short CooldownResets = 0;
    private HeroBehaviour _hero;
    private AbilityBehaviour _gladiatorAbility, _gladiatorStrikeAbility;
    #endregion

    #region Methods
    public override void Equip(HeroBehaviour hero, Transform slot)
    {
        base.Equip(hero, slot);       
        if (hero is PlayerBehaviour)
        {
            ((PlayerBehaviour)hero).OnCompanionAttack += CompanionAttack_Performed;
        }
        _gladiatorAbility = hero.GetHeroClass()?.GetAbilityFromAnyStance("Gladiator");
        if (_gladiatorAbility != null)
        {
            _gladiatorAbility.OnAbilityHit += Gladiator_Enter;
        }
        _gladiatorStrikeAbility = hero.GetHeroClass()?.GetAbilityFromAnyStance("Gladiator Strike");
        {
            _gladiatorStrikeAbility.OnAbilityHit += GladiatorStrike_Performed; 
        }
        _hero = hero;
    }
    public override void Unequip(HeroBehaviour hero)
    {
        base.Unequip(hero);
        if (hero is PlayerBehaviour)
        {
            ((PlayerBehaviour)hero).OnCompanionAttack -= CompanionAttack_Performed;
        }
        if(_gladiatorAbility != null)
        {
            _gladiatorAbility.OnAbilityHit -= Gladiator_Enter;
        }
        _gladiatorStrikeAbility = hero.GetHeroClass()?.GetAbilityFromAnyStance("Gladiator Strike");
        {
            _gladiatorStrikeAbility.OnAbilityHit -= GladiatorStrike_Performed;
        }
        _hero = null;
        _gladiatorAbility = null;
        _gladiatorStrikeAbility = null;
    }
    #endregion

    #region EventHandlers
    private void CompanionAttack_Performed(object sender, EventArgs e)
    {
        if (CooldownResets < 3)
        {
            CooldownResets++;
            var ability = _hero.GetHeroClass()?.GetAbilityFromAnyStance("Gladiator");
            if (ability != null)
            {
                ability.CurrentCooldown += 27;
                if (ability.CurrentCooldown > ability.MaxCooldown)
                {
                    ability.CurrentCooldown = ability.MaxCooldown;
                }
            }
        }
    }
    private void Gladiator_Enter(object sender, EventArgs e)
    {
        CooldownResets = 0;
    }
    private void GladiatorStrike_Performed(object sender, EventArgs e)
    {
        var effect = _hero.GetActiveEffects().Find(eff => eff is BerserkerIronHeartGladiatorStrikeEffect);
        _gladiatorStrikeAbility.ExternalDamageMultiplier = 1;
        if (effect != null)
        {       
            for (int i = 0; i < effect.Stacks; i++)
            {
                _gladiatorStrikeAbility.ExternalDamageMultiplier += 0.4f;
            }
        }
        _hero?.AddEffect(_ironHeartEffect);
    }
    #endregion
}
