using System.Linq;
using UnityEngine;

public class DOTHealingTriffidAIHandler : AIHandlerBehaviour
{
    #region Variables
    [Header("Triffid Related Variables")]
    [Tooltip("The target towards which the healing triffid will slowly move")]
    public CharacterBehaviour FollowTarget;
    #endregion

    #region Methods
    protected override void HandleCombatMovement()
    {
        _agent.speed = _character.GetEffectiveMovementSpeed();
        if (FollowTarget!=null)
            ActivateNavmeshAgent(SetUpNewDestination());
    }
    protected override Vector3 SetUpNewDestination()
    {
        return FollowTarget.transform.position;
    }
    public override void CheckForNewEnemy()
    {
        if(FollowTarget.IsInCombat)
        {
            if (_currentEnemy?.IsDead == true)
                _currentEnemy = null;
            if ((_currentEnemy == null) && _character.ActiveEnemies.Any())
                _currentEnemy = _character.ActiveEnemies[Random.Range(0, _character.ActiveEnemies.Count - 1)];
        }
    }
    #endregion
}
