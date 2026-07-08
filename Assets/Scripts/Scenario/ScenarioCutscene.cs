using System.Collections.Generic;
using UnityEngine;

public class ScenarioCutscene : MonoBehaviour
{
    #region Variables
    [HideInInspector] public bool Ended;
    [HideInInspector] public bool Init;
    [Header("Cutscene Related Variables")]
    [Tooltip("If set to true, the scene starts already faded to black. If set to false, the scene first will fade into black and dthen play")]
    public bool InitialCutscene;
    private ScenarioManager _manager;
    private List<ScenarioCutsceneTransform> _transforms;
    private float _cutsceneTimer;
    [Tooltip("How many seconds the scene will play")]
    public float _cutsceneDuration;
    private bool _sceneFidingOut;
    //time for the gameplay scene to properly fade out into the cutscene
    private float _cutsceneDelay;
    #endregion

    #region Mono
    protected virtual void Awake()
    {
        _cutsceneTimer = 0;
        _cutsceneDelay = 1;
    }
    #endregion

    #region Methods
    public void UpdateCutscene()
    {
        if (Ended == false)
        {
            if(InitialCutscene == true || _cutsceneDelay <= 0)
            {
                if (Init)
                {
                    Globals.Instance.IsCutscenePlaying = true;
                    _manager.SetUpForScene(InitialCutscene);
                    Init = false;
                }
                foreach (var trans in _transforms)
                {
                    if (!trans.Finished)
                    {
                        if (_cutsceneTimer >= trans.StartTimer)
                        {
                            trans.Perform(_cutsceneTimer);
                        }
                    }
                }
                if (_cutsceneTimer >= _cutsceneDuration - 1 && !_sceneFidingOut)
                {
                    _manager.StartFadeIn();
                    _sceneFidingOut = true;
                }
                else if (_cutsceneTimer >= _cutsceneDuration)
                {
                    _manager.EndScene();
                    Ended = true;
                }
                _cutsceneTimer += Time.deltaTime;
            }
            else if(InitialCutscene == false && _cutsceneDelay == 1)
            {
                _manager.StartFadeIn();
            }
            _cutsceneDelay -= Time.deltaTime;
        }
    }
    public void SetManager(ScenarioManager manager)
    {
        _manager = manager;
        foreach(var transform in _transforms)
        {
            if (transform is ScenarioCutsceneTransformBlackFade)
            {
                ((ScenarioCutsceneTransformBlackFade)transform).SetManager(_manager);
            }
            else if (transform is ScenarioCutsceneTransformPlayVideo)
            {
                ((ScenarioCutsceneTransformPlayVideo)transform).SetManager(_manager);
            }
        }
    }
    public void SetUpWithChildren()
    {
        _transforms = new();
        foreach (var child in GetComponentsInChildren<ScenarioCutsceneTransform>())
        {
            _transforms.Add(child);
        }
        Init = true;
        Ended = false;
        _sceneFidingOut = false;
    }
    #endregion
}
