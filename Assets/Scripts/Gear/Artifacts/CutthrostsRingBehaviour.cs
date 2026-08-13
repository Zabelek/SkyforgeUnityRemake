using System;
using UnityEngine;

public class CutthrostsRingBehaviour : GearPeaceBehaviour
{
    #region Variables
    public ArtifactSO ArtifactSO;
    [SerializeField] private GameplayEffectBehaviour _critEffectBase;
    private float _nextStackTimer;
    private short _statcksAlreadyAdded;
    private HeroBehaviour _hero;
    #endregion

    #region Mono
    protected override void Awake()
    {
        base.Awake();
        GearBonus.HealthPercentBonus += ArtifactSO.HealthBonus;
        GearBonus.DamagePercentBonus += ArtifactSO.DamageBonus;
    }
    private void FixedUpdate()
    {
        if(_nextStackTimer>0)
        {
            _nextStackTimer -= Time.fixedDeltaTime;
        }
    }
    #endregion

    #region Methods
    public override void Equip(HeroBehaviour hero, bool onlyVisual)
    {
        base.Equip(hero, onlyVisual);
        _hero = hero;
        hero.OnAttackPerformedEvent += AttackPerformed;
        hero.OnCriticalAttackPerformedEvent += CriticalAttackPerformed;
    }
    public override void Unequip(HeroBehaviour hero, bool onlyVisual)
    {
        _hero.RemoveEffect(_critEffectBase);
        hero.OnAttackPerformedEvent -= AttackPerformed;
        hero.OnCriticalAttackPerformedEvent -= CriticalAttackPerformed;
        _hero = null;
        base.Unequip(hero, onlyVisual);
    }
    #endregion

    #region EventHandlers
    private void AttackPerformed(object sender, EventArgs e)
    {
        if (_nextStackTimer <= 0 && _statcksAlreadyAdded<20)
        {
            _statcksAlreadyAdded++;
            _nextStackTimer = 1;
            _hero.AddEffect(_critEffectBase);
        }
    }
    private void CriticalAttackPerformed(object sender, EventArgs e)
    {
        _hero.RemoveEffect(_critEffectBase);
        _statcksAlreadyAdded = 0;
    }
    #endregion
}
