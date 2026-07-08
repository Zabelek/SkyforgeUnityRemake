using Unity.Cinemachine;
using UnityEngine;

public class BerserkerThirstForBattleAbilityBehaviour : AbilityBehaviour
{
    #region Variables
    [Header("Thirst For Battle")]
    [SerializeField] private BerserkerThirstForBattleEffect _buffEffect;
    [SerializeField] private ParticleSystem _initEffectBase;
    [SerializeField] private ParticleSystem _durationEffectBase;
    [SerializeField] private ParticleSystem _weaponEffectBase;
    private bool _initPerformed;
    //timers
    [SerializeField] private float _initEffectTimerBase;
    private float _initEffectTimer;
    #endregion

    #region Methods
    public override void Init()
    {
        base.Init();
        _demobillizesPerformer = false;
    }
    public override void UpdateAbility(CharacterBehaviour player, HeroClassBehaviour heroClass)
    {
        base.UpdateAbility(player, heroClass);
        if (!Finishing)
        {
            //init procedure is performed after some time after launching ability, so it can't be executed in LaunchAbility()
            if (_performingTimer > _initEffectTimer && !_initPerformed)
            {
                player.AddEffect(_buffEffect);
                Instantiate(_initEffectBase, player.transform);
                Instantiate(_durationEffectBase, player.transform);
                _initPerformed = true;
                if (TryGetComponent<CinemachineImpulseSource>(out CinemachineImpulseSource imp))
                {
                    imp.GenerateImpulse();
                }
                if (((HeroBehaviour)player).EquippedWeapon != null)
                {
                    Instantiate(_weaponEffectBase, ((HeroBehaviour)player).EquippedWeapon.transform);
                    ((HeroBehaviour)player).EquippedWeapon?.PlaySound("Thirst_For_Battle");
                }

            }
        }
    }
    public override void LaunchAbility(CharacterBehaviour performer)
    {
        base.LaunchAbility(performer);
        performer.PlayAnimation("ThirstForBattle", false);
    }
    protected override void CalculateAttackSpeed(float speed)
    {
        base.CalculateAttackSpeed(speed);
        _initEffectTimer = _initEffectTimerBase;
    }
    public override void Reset()
    {
        base.Reset();
        _initPerformed = false;
    }
    #endregion
}
