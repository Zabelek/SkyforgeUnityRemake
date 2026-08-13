using UnityEngine;

public class BerserkerCrushingThrowKnockUp : GameplayEffectBehaviour
{
    #region Variables
    [Header("Crushing Throw Variables")]
    [Tooltip("Time in which the character will be pulled up by the effect")]
    public float TimeGoingUp;
    [Tooltip("Right after pull up, in this time the character won't be affected by any additional force")]
    public float TimeInTheAir;
    [Tooltip("Time when the character will be forcefully smashed onto the ground")]
    public float TimeGoingDown;
    [Tooltip("Between each of these timespans the character will receive additional empty damages (only visual usage)")]
    public float AdditionalHitTimer;
    private Vector3 _upDestination,_initialPos, _totalDestination;
    private int _additionalHits = 0;
    //for attack speed scaling
    private float _currentTimeGoingUp, _currentTimeInTheAir, _currentTimeGoingDown, _currentAdditionalHitTimer, _timerMax;
    [HideInInspector] public float AttackSpeedScale;
    #endregion

    #region Methods
    public override void OnApply(CharacterBehaviour character)
    {
        character.IsFalling = true;
        character.SetCanMove(false);
        character.SetCanAct(false, false);
        if (character.TryGetComponent<AIHandlerBehaviour>(out var handler))
        {
            handler.StopMovement();
        }
        _initialPos = new Vector3(0, 0, 0);
        _upDestination = new Vector3(0, 1.7f, 0);
        SetTimers();
        TimeLeft = _timerMax;
        _additionalHits = 0;
        _totalDestination = character.transform.position;
    }
    public override void OnUpdate(CharacterBehaviour character)
    {
        base.OnUpdate(character);
        character.IsFalling = true;
        character.SetCanMove(false);
        character.SetCanAct(false, false);
        var effectiveTime = _timerMax - TimeLeft;
        if (effectiveTime < _currentTimeGoingUp)
        {
            character.MovePosition(Vector3.Slerp(_initialPos, _upDestination, (Time.fixedDeltaTime / _currentTimeGoingUp)), true);
        }
        else if(effectiveTime < _currentTimeGoingUp + _currentTimeInTheAir + _currentTimeGoingDown)
        {
            character.MovePosition(- Vector3.Slerp(_initialPos, _upDestination, Time.fixedDeltaTime / _currentTimeGoingDown), true);
            //to prevent characters from falling through the ground
            if (character.transform.position.y < _totalDestination.y)
                character.MovePosition(_totalDestination);
        }
        else if (effectiveTime > _currentTimeGoingUp + _currentTimeInTheAir + _currentTimeGoingDown + _currentAdditionalHitTimer * _additionalHits && _additionalHits < 4)
        {
            character.TakeEmptyDamage();
            _additionalHits++;
        }
    }
    public void SetTimers()
    {
        _currentTimeGoingUp = TimeGoingUp / AttackSpeedScale;
        _currentTimeInTheAir = TimeInTheAir / AttackSpeedScale;
        _currentTimeGoingDown = TimeGoingDown / AttackSpeedScale;
        _currentAdditionalHitTimer = AdditionalHitTimer / AttackSpeedScale;
        _timerMax = _currentTimeGoingUp + _currentTimeInTheAir + _currentTimeGoingDown + _currentAdditionalHitTimer *3;
    }
    public override void OnRemove(CharacterBehaviour character)
    {
        character.SetCanMove(true);
        character.SetCanAct(true);
        character.IsFalling = false;
    }
    #endregion
}
