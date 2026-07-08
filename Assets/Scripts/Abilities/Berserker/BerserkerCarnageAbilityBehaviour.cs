using Unity.Cinemachine;
using UnityEngine;

public class BerserkerCarnageAbilityBehaviour : FinisherAbility
{
    private const int EMPTY_HITS_AMOUNT = 3;

    #region Variables
    [Header("Carnage")]
    [SerializeField] private float _startLeapTimerBase;
    [SerializeField] private float _endLeapTimerBase;
    [SerializeField] private float _eachEmptyHitTimerBase;
    private float _startLeapTimer, _endLeapTimer, _eachEmptyHitTimer;
    private Vector3 _initialPlayerPosition, _destinationVector, _alreadyMovedAmount;
    [SerializeField] private float _archHeight;
    [SerializeField] private ParticleSystem _particles;
    private bool _coordinatesAlreadySet;
    private short _emptyHitsDone;
    #endregion

    #region Methods
    public override void UpdateAbility(CharacterBehaviour performer, HeroClassBehaviour heroClass)
    {
        base.UpdateAbility(performer, heroClass);
        if (!Finishing)
        {
            if (_performingTimer >= _startLeapTimer && _performingTimer < _endLeapTimer)
            {
                if (!_coordinatesAlreadySet)
                {
                    _coordinatesAlreadySet = true;
                    _initialPlayerPosition = performer.transform.position;
                    if (performer is HeroBehaviour && ((HeroBehaviour)performer).EquippedWeapon != null)
                    {
                        ((PlayerBehaviour)performer).EquippedWeapon.PlayLongSound("Carnage");
                        _destinationVector = Casuality.transform.position;
                        _destinationVector = _destinationVector - (_destinationVector - _initialPlayerPosition).normalized * 2f;
                    }
                    else { _destinationVector = performer.transform.position; }
                }
                if (performer.GetComponent<Rigidbody>()?.isKinematic == false)
                {
                    performer.GetComponent<Rigidbody>().isKinematic = true;
                }
                var timeMod = (_performingTimer - _startLeapTimer) / (_endLeapTimer - _startLeapTimer);
                var vec = Vector3.Lerp(_initialPlayerPosition, _destinationVector, timeMod);
                vec -= _initialPlayerPosition;
                vec.y += Mathf.Sin(timeMod * Mathf.PI) * _archHeight;
                vec = vec - _alreadyMovedAmount;
                _alreadyMovedAmount += vec;
                performer.MoveBySkill(vec, MovingAbilityBehaviour.MovingType.Raw);
            }
            if (_performingTimer > _endLeapTimer + _emptyHitsDone * _eachEmptyHitTimer && _emptyHitsDone < EMPTY_HITS_AMOUNT)
            {
                if (_emptyHitsDone == 0)
                {
                    Casuality.SetGlow(0.5f, Color.red);
                    Instantiate(_particles, Casuality.transform);
                }
                _emptyHitsDone++;
                Casuality.TakeEmptyDamage();
            }
            if (_performingTimer >= _untilHitTimer)
            {
                if (performer.GetComponent<Rigidbody>()?.isKinematic == true)
                {
                    performer.GetComponent<Rigidbody>().isKinematic = false;
                }
            }
        }
    }
    public override void LaunchAbility(CharacterBehaviour performer)
    {
        base.LaunchAbility(performer);
        Casuality.StopMovement();
        Casuality.CancelAllAbilities();
        Casuality.SetCanMove(false);
        Casuality.SetCanAct(false, true);
    }
    public override void PerformHit(CharacterBehaviour performer)
    {
        base.PerformHit(performer);
        if(performer is PlayerBehaviour)
        {
            Casuality.Kill(performer);
            performer.Stats.CurrentMana += 58;
        }
        if (TryGetComponent<CinemachineImpulseSource>(out CinemachineImpulseSource imp))
        {
            imp.GenerateImpulse();
        }
    }
    protected override void CalculateAttackSpeed(float speed)
    {
        base.CalculateAttackSpeed(speed);
        _startLeapTimer = _startLeapTimerBase;
        _endLeapTimer = _endLeapTimerBase;
        _eachEmptyHitTimer = _eachEmptyHitTimerBase;
    }
    public override void Reset()
    {
        base.Reset();
        _destinationVector = Vector3.zero;
        _alreadyMovedAmount = Vector3.zero;
        _coordinatesAlreadySet = false;
        _emptyHitsDone = 0;
    }
    #endregion
}
