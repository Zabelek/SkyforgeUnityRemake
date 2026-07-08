using UnityEngine;

public class DOTEntidBossAiHandlerBehaviour : AIHandlerBehaviour
{
    private const float SPIT_ABILITY_DISTANCE_TRESHOLD_MIN = 5;
    private const float SPIT_ABILITY_DISTANCE_TRESHOLD_MAX = 30;

    #region Methods
    protected override void HandleCombatMovement()
    {
        _agent.speed = _character.GetEffectiveMovementSpeed();
        CheckForNewEnemy();
        if (_currentEnemy != null && !_currentEnemy.IsDead)
        {
            var dist = CheckEnemyDistance();
            if (dist > _followCloseUpDistanceLimit)
            {
                var newDest = SetUpNewDestination();
                ActivateNavmeshAgent(newDest);
            }
            //if (AreCharactersInThePoint(_character.transform.position))
            //{
            //    var newDest = SetUpNewDestination();
            //    ActivateNavmeshAgent(newDest);
            //}
            if (TryPerformPassiveAbility() == false)
            {
                if (dist < _followCloseUpDistanceLimit + _attackRangeExtraLimit && TryPerformAttack())
                {
                    TurnTowardsAbilityTarget();
                }
                else if (dist > _followCloseUpDistanceLimit + SPIT_ABILITY_DISTANCE_TRESHOLD_MIN && dist < _followCloseUpDistanceLimit + SPIT_ABILITY_DISTANCE_TRESHOLD_MAX
                    && TryPerformRangedAttack())
                {
                    TurnTowardsAbilityTarget();
                }
            }
        }
    }
    public override bool TryPerformAttack()
    {
        if (_character.CanAct())
        {
            if(_character is PYTHEntidBehaviour)
            {
                var entid = ((PYTHEntidBehaviour)_character);
                if(entid != null)
                {
                    //it can't be return _character.TryPerformAbility(_character.SpecialAttack), because it won't check for niemal attack and just return false!
                    if (entid.ThirdSkill != null && entid.TryPerformAbility(entid.ThirdSkill))
                        return true;
                    else if (_character.BaseAttack != null && _character.TryPerformAbility(_character.BaseAttack))
                        return true;
                }
            }
        }
        return false;
    }
    private bool TryPerformRangedAttack()
    {
        if (_character.CanAct())
        {
            if (_character is PYTHEntidBehaviour)
            {
                var entid = ((PYTHEntidBehaviour)_character);
                if (entid != null)
                {
                    if (entid.SecondSkill != null)
                        return entid.TryPerformAbility(entid.SecondSkill);
                }
            }
        }
        return false;
    }
    private bool TryPerformPassiveAbility()
    {
        if (_character.CanAct())
        {
            if (_character is PYTHEntidBehaviour)
            {
                var entid = ((PYTHEntidBehaviour)_character);
                if (entid != null)
                {
                    if (entid.FirstSkill != null)
                        return entid.TryPerformAbility(entid.FirstSkill);
                }
            }
        }
        return false;
    }
    protected override Vector3 SetUpNewDestination()
    {
        //closest possible destination
        var destVector = Vector3.Lerp(_character.transform.position, _currentEnemy.transform.position, 1 - (_followCloseUpDistanceLimit / (_character.transform.position - _currentEnemy.transform.position).magnitude));
        var overlaps = Physics.OverlapSphere(destVector, _collisionRadius);
        return destVector;
    }
    #endregion
}
