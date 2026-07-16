using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Video;

public class ScenarioManager : MonoBehaviour
{

    #region Variables
    [HideInInspector] public int Stage;
    [Header("Scenario Related Variables")]
    [Tooltip("Main GUI reference")]
    [SerializeField] protected GUIGameplayControls _interface;
    [Tooltip("Black object that will cover the screen")]
    [SerializeField] protected CanvasGroup _cutsceneBlack;
    [Tooltip("Voicelines that are spoken during the scenario")]
    [SerializeField] protected VoicelineSO[] _voicelines;
    [Tooltip("Voicelines that would be spoken if the player dies. They have to be jokes, otherwise the gane will throw an error")]
    [SerializeField] protected VoicelineSO[] _deadJokes;
    [Tooltip("Player reference")]
    [SerializeField] protected PlayerBehaviour _player;
    protected Vector3 _spawnPoint;
    //cameras
    [Tooltip("Normal gameplay camera to switch after the cutscene is over")]
    [SerializeField] protected CinemachineCamera _followCamera;
    [Tooltip("Cinemachine camera that will be steered by the cutscenes")]
    [SerializeField] protected CinemachineCamera _cutsceneCanera;
    //cutscenes
    protected List<ScenarioCutscene> _cutscenes;
    protected ScenarioCutscene _currentCutscene;
    [Tooltip("Used for playing videos. Shocking, I know")]
    [SerializeField] protected VideoPlayer _videoPlayer;
    [Tooltip("Component that manages the scene fading out to black")]
    [SerializeField] protected GUISceneBlackFade _blackFade;
    [Tooltip("This will delay the start of the initial scenario scene. The screen will remain black at the beginning of the scenario.")]
    public float SceneStartDelay;
    #endregion

    #region Mono
    protected virtual void Awake()
    {
        Stage = 0;
        _followCamera.Priority = 5;
        _cutsceneCanera.Priority = 1;
        //Cutscenes set up
        _cutscenes = new();
        foreach (var child in GetComponentsInChildren<ScenarioCutscene>())
        {
            child.SetUpWithChildren();
            _cutscenes.Add(child);
            child.SetManager(this);
        }
        StartCoroutine(DelayedInitSequence(SceneStartDelay));
        StartCoroutine(_player.DelayedInitSequence());
        _player.OnPlayerRessurected += Resurrect_Performed;
        _spawnPoint = _player.transform.position;
    }
    protected virtual void Update()
    {
        //update cutscene if any is up
        if (_currentCutscene != null)
        {
            _currentCutscene.UpdateCutscene();
            if (_currentCutscene.Ended)
            {
                _currentCutscene = null;
            }
        }
        if(_videoPlayer.gameObject.activeSelf)
        {
            if(_videoPlayer.isPaused)
            {
                _videoPlayer.gameObject.SetActive(false);
            }
        }
    }
    #endregion

    #region Mathods
    protected virtual void Resurrect_Performed(object sender, EventArgs e)
    {
        _ = _blackFade.StartFadeIn();
        StartCoroutine(DelayedResurrect(GUISceneBlackFade.FADE_TIME));
    }
    protected virtual IEnumerator DelayedResurrect(float delay)
    {
        yield return new WaitForSeconds(delay);
        _player.Resurrect();
        _player.transform.position = _spawnPoint;
        if (_deadJokes != null && _deadJokes.Length > 0)
        {
            _interface.ShowCharacterMessage(_deadJokes[UnityEngine.Random.Range(0, _deadJokes.Count() - 1)]);
        }
        _ = _blackFade.StartFadeOut();
    }
    protected virtual IEnumerator DelayedInitSequence(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (_cutscenes != null && _cutscenes.Count > 0)
        {
            _currentCutscene = _cutscenes[0];
        }
    }
    protected IEnumerator DelayedCharacterRemoval(CharacterBehaviour character, float delay)
    {
        yield return new WaitForSeconds(delay);
        character.gameObject.SetActive(false);
    }
    protected bool IsGroupDead(CharacterBehaviour[] group)
    {
        foreach(var creature in group)
        {
            if (creature.IsDead == false)
            {
                return false;
            }
        }
        return true;
    }
    public void SetUpForScene(bool initialScene)
    {
        _player.CancelAllAbilities();
        //the difference is that the cutscene_black starts with alpha = 1 if it is an initial map cutscene
        if(initialScene)
        {
            _interface.gameObject.SetActive(false);
            _cutsceneBlack.gameObject.SetActive(true);
            _cutsceneBlack.alpha = 1;
            _followCamera.Priority = 1;
            _cutsceneCanera.Priority = 5;
            StartFadeOut();
        }
        else
        {
            _interface.gameObject.SetActive(false);
            _followCamera.Priority = 1;
            _cutsceneCanera.Priority = 5;
            StartFadeOut();
        }
    }
    public void StartFadeIn()
    {
        _ = _blackFade.StartFadeIn();
    }
    public void StartFadeOut()
    {
        _ = _blackFade.StartFadeOut();
    }
    public void EndScene()
    {
        _followCamera.Priority = 5;
        _cutsceneCanera.Priority = 1;
        StartFadeOut();
        _interface.gameObject.SetActive(true);
        Globals.Instance.IsCutscenePlaying = false;
    }
    public void PlayVideo(VideoClip clip)
    {
        _videoPlayer.clip = clip;
        _videoPlayer.gameObject.SetActive(true);
        _videoPlayer.Play();
    }
    public void StopVideo()
    {
        _videoPlayer.Pause();
    }
    #endregion
}
