using Unity.Cinemachine;
using UnityEngine;

public class BerserkerThunderingRoarAbilityBehaviour : AbilityBehaviour
{
    #region Variables
    [Header("Thundering Roar")]
    [SerializeField] private ParticleSystem _effectBase;
    [SerializeField] private GameplayEffectBehaviour _fear;
    [SerializeField] private Transform _distortionBase;
    private bool _fearApplied, _distortionDestroyed;
    private Transform _currentDistortion;
    //timers
    [SerializeField] private float _fearEffectTimerBase;
    private float _fearEffectTimer;
    #endregion

    #region Methods
    public override void UpdateAbility(CharacterBehaviour performer, HeroClassBehaviour heroClass)
    {
        base.UpdateAbility(performer, heroClass);
        if (!Finishing)
        {
            if (_performingTimer > _fearEffectTimer && !_fearApplied)
            {
                Instantiate(_effectBase, performer.transform);
                _currentDistortion = Instantiate(_distortionBase, performer.transform);
                _currentDistortion.transform.forward = Globals.Instance.ViewportCamera.transform.forward * -1;
                _fearApplied = true;
                if (TryGetComponent<CinemachineImpulseSource>(out CinemachineImpulseSource imp))
                {
                    imp.GenerateImpulse();
                }
                var potentialCasualities = Physics.OverlapSphere(performer.transform.position, 10f);
                foreach (var casuality in potentialCasualities)
                {
                    if (CharacterBehaviour.FindEnemyCharacterInCollider(casuality, performer, out CharacterBehaviour character) == true)
                    {
                        character.AddEffect(_fear);
                    }
                }
                try { ((HeroBehaviour)performer).EquippedWeapon?.PlaySound("Thundering_Roar"); } catch { }
            }
            else if (_performingTimer > _attackTimerNext && !_distortionDestroyed)
            {
                Destroy(_currentDistortion.gameObject);
                _currentDistortion = null;
                _distortionDestroyed = true;
            }
        }
    }
    public override void Interrupt(CharacterBehaviour performer)
    {
        base.Interrupt(performer);
        if(_currentDistortion!= null)
        {
            Destroy(_currentDistortion.gameObject);
            _currentDistortion = null;
        }
    }
    public override void LaunchAbility(CharacterBehaviour performer)
    {
        base.LaunchAbility(performer);
        performer.PlayAnimation("ThunderingRoar", true);
    }
    protected override void CalculateAttackSpeed(float speed)
    {
        base.CalculateAttackSpeed(speed);
        _fearEffectTimer = _fearEffectTimerBase;
    }
    public override void Reset()
    {
        base.Reset();
        _fearApplied = false;
        _distortionDestroyed = false;
    }
    #endregion
}
