using UnityEngine;

public class MovingAbilityBehaviour : AbilityBehaviour
{
    public enum MovingType { CameraBased, Raw, TargetBased, Forward}
    #region Variables 
    [Header("Ability Movement")]
    [Tooltip("Specifies which way the character moved by the ability will follow")]
    [SerializeField] protected MovingType _movingType;
    [Tooltip("In most cases, doesn't matter which axis will be chosen here, as only the vector's magnitude counts. Exceptions are Raw movement type or CameraBased for Player (for Player best is to use Y axis)")]
    [SerializeField] protected Vector3 _customMoveAmount;
    private Vector3 _alreadyMovedAmount;
    private bool _customMoveAllowed;
    [Tooltip("Can be used to specify when the movement starts or ends")]
    [SerializeField] protected float _customMoveTimerEndBase, _customMoveTimerStartBase;
    protected float _customMoveTimerEnd, _customMoveTimerStart;
    protected bool _considerCameraAngleWhileMoving = true;
    protected bool _ignoreMovementBlockers = false;
    #endregion

    #region Methods
    public override void Init()
    {
        base.Init();
        _customMoveAllowed = true;
    }
    public virtual void UpdateCustomMove(CharacterBehaviour performer)
    {
        if (_performingTimer > _customMoveTimerStart && _performingTimer < _customMoveTimerEnd && _customMoveAllowed)
        {
            var timeMod = (_performingTimer - _customMoveTimerStart) / (_customMoveTimerEnd - _customMoveTimerStart);
            var vec = Vector3.Lerp(Vector3.zero, _customMoveAmount, timeMod);
            vec = vec - _alreadyMovedAmount;
            _alreadyMovedAmount += vec;
            performer.MoveBySkill(vec, _movingType);
        }
    }
    public override void LaunchAbility(CharacterBehaviour performer)
    {
        base.LaunchAbility(performer);
        SearchForMovementBlockers(performer);
    }
    private void SearchForMovementBlockers(CharacterBehaviour performer)
    {
        if(!_ignoreMovementBlockers)
        {
            var potentialCasualities = Physics.OverlapSphere(performer.transform.position + (performer.transform.forward * _customMoveAmount.y), 0.5f);
            foreach (var hit in potentialCasualities)
            {
                if (CharacterBehaviour.FindCharacterInCollider(hit, performer, out CharacterBehaviour character) == true)
                {
                    _customMoveAllowed = false;
                    return;
                }
            }
        }
        _customMoveAllowed = true;
    }
    public override void Reset()
    {
        base.Reset();
        _alreadyMovedAmount = Vector3.zero;
        _customMoveAllowed = true;
    }
    #endregion
}
