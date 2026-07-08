using UnityEngine;
using UnityEngine.Video;

public class ScenarioCutsceneTransformPlayVideo : ScenarioCutsceneTransform
{
    #region Variables
    [Header("Video Related Variables")]
    [SerializeField] private VideoClip _clip;
    private ScenarioManager _manager;
    private bool _videoStarted;
    #endregion

    #region Mono
    protected override void Awake()
    {
        base.Awake();
        _videoStarted = false;
    }
    #endregion

    #region Methods
    public override void Perform(float cutsceneTimer)
    {
        if(_manager != null)
        {
            if (Duration == 0 || !_videoStarted)
            {
                _manager.PlayVideo(_clip);
                _videoStarted = true;
                if(Duration == 0)
                    Finished = true;
            }
            else if(cutsceneTimer > StartTimer + Duration)
            {
                _manager.StopVideo();
                Finished = true;
            }        
        }
        else
            Finished = true;
    }
    public void SetManager(ScenarioManager manager)
    {
        _manager = manager;
    }
    #endregion
}
