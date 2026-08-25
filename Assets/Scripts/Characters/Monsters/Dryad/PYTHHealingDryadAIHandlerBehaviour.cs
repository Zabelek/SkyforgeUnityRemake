using UnityEngine;

public class PYTHHealingDryadAIHandlerBehaviour : AIHandlerBehaviour
{
    #region Variables
    private bool _isCorrect;
    private MonsterBehaviour _dryad;
    #endregion

    #region Mono
    protected void Awake()
    {
        if(_character is MonsterBehaviour && (_character as MonsterBehaviour).SpecialAttack is PYTHDryadHealAbilityBehaviour)
        {
            _dryad = _character as MonsterBehaviour;
            _isCorrect = true;
        }
    }
    #endregion

    #region Methods
    protected override void HandleCombatMovement()
    {
        bool healPerformed = false;
        if (_isCorrect && _dryad.SpecialAttack.CheckPerformAvailability(_dryad) == true)
        {
            var targets = Physics.OverlapSphere(_character.transform.position, 10);
            foreach (var collider in targets)
            {
                if (CharacterBehaviour.FindAllyCharacterInCollider(collider, _dryad, out var ally) == true && ally.Stats.CurrentHP < ally.Stats.MaxHP * 0.8f)
                {
                    (_dryad.SpecialAttack as PYTHDryadHealAbilityBehaviour).HealingTarget = ally;
                    _dryad.SpecialAttack.LaunchAbility(_dryad);
                    healPerformed = true;
                    break;
                }
            }
        }
        if(!healPerformed)
            base.HandleCombatMovement();
    }
    public override bool TryPerformAttack()
    {
        if (_character.CanAct())
        {
            if (_character.BaseAttack != null && _character.TryPerformAbility(_character.BaseAttack))
                return true;
        }
        return false;
    }
    #endregion
}
