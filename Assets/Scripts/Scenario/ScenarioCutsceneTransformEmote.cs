using System;
using UnityEngine;

public class ScenarioCutsceneTransformEmote : ScenarioCutsceneTransform
{
    #region Variables
    [Header("Emote Related Variables")]
    [Tooltip("For now only one emote set in a proper place in the class references is supported")]
    public CharacterBehaviour AnimatedCharacter;
    #endregion

    #region Methods
    public override void Perform(float cutsceneTimer)
    {
        if (AnimatedCharacter != null)
        {
            AnimatedCharacter.PerformEmote(this, EventArgs.Empty);
        }
        Finished = true;
    }
    #endregion
}
