using System;
using UnityEngine;

public class GvardarBehaviour : WeaponBehaviour
{
    #region Variables
    private BerserkerCripplingBlowAbilityBehaviour _cripplingBlow;
    private int _timesHit;
    [SerializeField] private GameplayEffectBehaviour _shieldShort, _shieldLong, _healMultiplierEffect;
    #endregion

    #region Methods
    public override void Equip(HeroBehaviour hero, Transform slot, bool onlyVisual)
    {
        base.Equip(hero, slot, onlyVisual);
        if (!onlyVisual)
        {
            _cripplingBlow = hero.GetHeroClass()?.GetAbilityFromAnyStance("Crippling Blow") as BerserkerCripplingBlowAbilityBehaviour;
            if (_cripplingBlow != null)
            {
                _cripplingBlow.OnAbilityStart += CripplingBlow_Started;
                _cripplingBlow.OnAbilityHit += CripplingBlow_Hit;
                _cripplingBlow.OnAbilityEnd += CripplingBlow_Ended;
            }
            hero.AddEffect(_healMultiplierEffect);
            hero.OnResurrectEvent += Hero_Resurrected;
        }
    }
    public override void Unequip(HeroBehaviour hero, bool onlyVisual)
    {
        if (!onlyVisual)
        {
            if (_cripplingBlow != null)
            {
                _cripplingBlow.OnAbilityStart -= CripplingBlow_Started;
                _cripplingBlow.OnAbilityHit -= CripplingBlow_Hit;
                _cripplingBlow.OnAbilityEnd -= CripplingBlow_Ended;
                _cripplingBlow.ExternalDamageMultiplier = 1f;
                _cripplingBlow = null;
            }
            hero.RemoveEffect(_healMultiplierEffect);
            hero.OnResurrectEvent -= Hero_Resurrected;
        }
        base.Unequip(hero, onlyVisual);
    }
    #endregion

    #region EventHandlers
    private void CripplingBlow_Started(object sender, AbilityBehaviour.AbilityStateEventArgs e)
    {
        _cripplingBlow.ModAbilityDuration(3f);
        _cripplingBlow.MaxTimesHit = 8;
        _timesHit = 0;
    }
    private void CripplingBlow_Hit(object sender, AbilityBehaviour.AbilityStateEventArgs e)
    {
        if(_timesHit>3)
        {
            _cripplingBlow.ExternalDamageMultiplier = 3f;
            e.Performer.TakeDamage(new Damage(e.Performer, (int)(e.Performer.Stats.MaxHP * 0.07f)));
        }
        _timesHit++;
    }
    private void CripplingBlow_Ended(object sender, AbilityBehaviour.AbilityStateEventArgs e)
    {
        if (_timesHit == 8)
        {
            e.Performer.AddEffect(_shieldLong);
        }
        else if (_timesHit > 3)
        {
            e.Performer.AddEffect(_shieldShort);
        }
        _cripplingBlow.ExternalDamageMultiplier = 1f;
    }
    private void Hero_Resurrected(object sender, EventArgs e)
    {
        (sender as HeroBehaviour).AddEffect(_healMultiplierEffect);
    }
    #endregion
}
