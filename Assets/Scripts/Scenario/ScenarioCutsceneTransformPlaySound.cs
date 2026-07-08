using UnityEngine;

public class ScenarioCutsceneTransformPlaySound : ScenarioCutsceneTransform
{
    #region Variables
    [Header("Sound Related Variables")]
    public SoundEffectSO Effect;
    [Tooltip("The transform in which the sound will be played. The sound will be a child of this transform")]
    public Transform LocationTransform;
    #endregion

    #region Methods
    public override void Perform(float cutsceneTimer)
    {
        if (Effect != null)
        {
            Globals.Instance.SoundManager.PlaySFX(Effect.AudioClips[0], LocationTransform.position, Effect.VolumeModifier);
        }
        Finished = true;
    }
    #endregion
}
