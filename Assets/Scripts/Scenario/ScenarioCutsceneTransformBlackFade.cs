using UnityEngine;

public class ScenarioCutsceneTransformBlackFade : ScenarioCutsceneTransform
{
    #region Variables
    [Header("Fade related variables")]
    [Tooltip("Set to false if yo uwant the scene to fade into black. True is, obviously, the opposite")]
    public bool FadeOut;
    private ScenarioManager _manager;
    #endregion

    #region Methods
    public override void Perform(float cutsceneTimer)
    {
        if (_manager != null)
        {
            if(FadeOut)
            {
                _manager.StartFadeOut();
            }
            else
            {
                _manager.StartFadeIn();
            }
        }
        Finished = true;
    }
    public void SetManager(ScenarioManager manager)
    {
        _manager = manager;
    }
    #endregion
}
