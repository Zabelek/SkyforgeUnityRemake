using System;
using UnityEngine;

public class KillAfterBattleBehaviour : MonoBehaviour
{
    #region Variables
    private CharacterBehaviour _character;
    #endregion

    #region Mono
    private void Awake()
    {
        _character = GetComponent<CharacterBehaviour>();
        _character.OnCombatEndEvent += KillAfterCombat;
    }
    #endregion

    #region EventHandlers
    private void KillAfterCombat(object sender, EventArgs e)
    {
        _character.KillOffCombat();
    }
    #endregion
}
