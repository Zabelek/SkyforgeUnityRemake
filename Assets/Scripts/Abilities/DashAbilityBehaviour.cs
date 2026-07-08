using UnityEngine;
using UnityEngine.AI;

public class DashAbilityBehaviour : AbilityBehaviour
{
    protected const float DASH_CHARGE_REQUIREMENT = 10;

    #region Variables
    [Header("Dash Ability")]
    [Tooltip("Particles to be spawned on dash")]
    [SerializeField] private Transform _particleBase;
    [SerializeField] private float _dashLength;
    [Tooltip("Sound effect to be played on dash")]
    [SerializeField] private SoundEffectSO _soundEffect;
    private Vector3 _destinationVector, _initialVector;
    private Transform _currentParticles;
    #endregion

    #region Methods
    public override void UpdateAbility(CharacterBehaviour performer, HeroClassBehaviour heroClass)
    {      
        _performingTimer += Time.fixedDeltaTime;
        if (_performingTimer < _attackTimerNext)
        {
            var timeMod = _performingTimer / _attackTimerNext;
            Vector3 vec = Vector3.Lerp(_initialVector, _destinationVector, timeMod);
            Vector3 vecAdd = (_destinationVector - _initialVector) * (Time.fixedDeltaTime / _attackTimerNext);
            //dash can only be performed on navmesh to avoid collision bugs
            if (NavMesh.SamplePosition(performer.transform.position + vecAdd, out NavMeshHit newhit, 5f, LayerMask.GetMask("Default")))
            {
                vec = newhit.position;
                vecAdd = vec - performer.transform.position;
            }
            performer.MovePosition(vecAdd, true);
            _currentParticles.transform.position = vec;
        }
        if (AbilityCurrentlyLockingControl && _performingTimer >= _attackTimerNext)
        {
            ReleaseControl(performer);
            if (_producesDefaultTrace && performer is HeroBehaviour)
                ((HeroBehaviour)performer).EquippedWeapon?.SetTrail(false);
        }
        if (_performingTimer >= _attackTimerMax)
        {
            EndAbility(performer);
            if(_currentParticles != null)
            {
                Destroy(_currentParticles.gameObject);
                _currentParticles = null;
            }
        }
    }
    public override void LaunchAbility(CharacterBehaviour performer)
    {
        base.LaunchAbility(performer);
        if(_particleBase != null)
            _currentParticles = Instantiate(_particleBase, performer.transform.position, performer.transform.rotation);
        _initialVector = performer.transform.position;
        if (performer is PlayerBehaviour)
        {
            _destinationVector = performer.transform.position + ((PlayerBehaviour)performer).LastMovementDirection * _dashLength;
            ((PlayerBehaviour)performer).DashCharge -= DASH_CHARGE_REQUIREMENT;
        }
        else
        {
            _destinationVector = performer.transform.position + performer.transform.forward * _dashLength;
        }
        if (_soundEffect != null)
            Globals.Instance.SoundManager.PlaySFXFast(_soundEffect, performer.transform);
        performer.PlayAnimation("Dash", true);
    }
    public override bool CheckPerformAvailability(CharacterBehaviour performer)
    {
        if(performer is HeroBehaviour)
        {
            if (((HeroBehaviour)performer).CanDash)
            {
                if (performer is PlayerBehaviour)
                {
                    if(((PlayerBehaviour)performer).DashCharge >= DASH_CHARGE_REQUIREMENT && ((PlayerBehaviour)performer).LastMovementDirection != Vector3.zero)
                        return base.CheckPerformAvailability(performer);
                }
                else
                    return base.CheckPerformAvailability(performer);
            }
        }
        return false;
    }
    public override void Reset()
    {
        base.Reset();
        _destinationVector = Vector3.zero;
        _initialVector = Vector3.zero;
    }
    #endregion
}
