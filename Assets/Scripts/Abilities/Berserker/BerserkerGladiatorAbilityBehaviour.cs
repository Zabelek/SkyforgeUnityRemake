using System.Linq;
using UnityEngine;

public class BerserkerGladiatorAbilityBehaviour : AbilityBehaviour
{
    #region Variables
    [Header("Gladiator")]
    [SerializeField] private GameplayEffectBehaviour _effectBase;
    [SerializeField] private ParticleSystem _particlesBase;
    [SerializeField] private GameplayEffectBehaviour _rageIncarnateEffect;
    private bool _effectApplied, _particlesApplied;
    //timers
    [SerializeField] private float _startEffectTimerBase, _particlesTimerBase;
    private float _startEffectTimer, _particlesTimer;
    #endregion

    #region Methods
    public override void UpdateAbility(CharacterBehaviour performer, HeroClassBehaviour heroClass)
    {
        base.UpdateAbility(performer, heroClass);
        if (!Finishing)
        {
            if (_performingTimer > _particlesTimer && _particlesApplied == false)
            {
                var part = Instantiate(_particlesBase, performer.transform);
                part.transform.SetParent(null);
                _particlesApplied = true;
                try { ((HeroBehaviour)performer).EquippedWeapon?.PlaySound("Gladiator_Enter"); } catch { }
            }
            if (_performingTimer > _startEffectTimer && _effectApplied == false)
            {
                performer.AddEffect(_effectBase);
                _effectApplied = true;
                if (((HeroBehaviour)performer).GetPerk("Base_Berserker_RageIncarnate")?.Enabled == true)
                {
                    performer.AddEffect(_rageIncarnateEffect);
                    float percent = (float)performer.Stats.CurrentMana / (float)performer.Stats.MaxMana;
                    short amount = (short)(150f * (1 - percent));
                    performer.GetActiveEffects().FirstOrDefault(e => e.EffectSO == _rageIncarnateEffect.EffectSO).Stacks = amount;
                }
                PerformHit(performer);
            }
        }
    }
    public override void LaunchAbility(CharacterBehaviour performer)
    {
        base.LaunchAbility(performer);
        performer.PlayAnimation("Gladiator", true);
    }
    protected override void CalculateAttackSpeed(float speed)
    {
        base.CalculateAttackSpeed(speed);
        _startEffectTimer = _startEffectTimerBase;
        _particlesTimer = _particlesTimerBase;
    }
    public override void Reset()
    {
        base.Reset();
        _effectApplied = false;
        _particlesApplied = false;
    }
    #endregion
}
