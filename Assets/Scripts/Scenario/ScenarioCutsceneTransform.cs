using UnityEngine;

public abstract class ScenarioCutsceneTransform : MonoBehaviour
{
    #region Variables
    [Header("Cutscene Transform Related Variables")]
    [Tooltip("How many seconds after the cutscene start this transform will be activated")]
    public float StartTimer;
    [Tooltip("Duration of the transform. For some, it is necessary to set this (TransformAnimate/TransformDrawWeapon), for some, it's optional (TransformMove/TransformPlayVideo), for the rest, it does nothing.")]
    public float Duration;
    [HideInInspector] public bool Finished;
    #endregion

    #region Mono
    protected virtual void Awake()
    {
         Finished = false;
    }
    #endregion

    #region Methods
    public virtual void Perform(float cutsceneTimer)
    {

    }
    #endregion
}