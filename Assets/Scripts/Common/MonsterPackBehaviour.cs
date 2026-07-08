using UnityEngine;

public class MonsterPackBehaviour : MonoBehaviour
{
    #region Mono
    private void Start()
    {
        foreach(var child in GetComponentsInChildren<CharacterBehaviour>())
        {
            child.OnCombatStartEvent += CombatStarted;
        }
    }
    #endregion

    #region Methods
    private void CombatStarted(object sender, CharacterBehaviour.StartCombatEventArgs e)
    {
        if (e.FightProvokedByGroup == false)
            foreach (var child in GetComponentsInChildren<CharacterBehaviour>())
            {
                if (e.FightProvokedByGroup == false)
                {
                    child.EnterCombat(e.Enemy, true);
                    //it is false in the second one, so if two groups engage each other, the enemy can alarm the rest of its pack.
                    e.Enemy.EnterCombat(child, false);
                }
            }
    }
    #endregion
}
