using System.Collections;
using UnityEngine;

public class SFScenarioManager : ScenarioManager
{
    #region Mono
    protected override void Awake()
    {
        base.Awake();
        if(SkyforgeLoader.CurrentProfile?.StoryVariables?.SkyForgeVisited==false)
        {
            StartCoroutine(PlayIntroDialogue(2f, _voicelines[0]));
            StartCoroutine(PlayIntroDialogue(16f, _voicelines[1]));
            SkyforgeLoader.CurrentProfile.StoryVariables.SkyForgeVisited = true;
        }
    }
    #endregion

    #region Methods
    private IEnumerator PlayIntroDialogue(float delay, VoicelineSO voiceline)
    {
        yield return new WaitForSeconds(delay);
        _interface.ShowCharacterMessage(voiceline);
    }
    #endregion
}
