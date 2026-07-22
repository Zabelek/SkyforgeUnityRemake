using Unity.Cinemachine;
using UnityEngine;

public class Globals : MonoBehaviour
{
    #region Variables
    public static Globals Instance;
    [Tooltip("Sound effect used commonly across many scripts can be added here, for easy access")]
    public SoundEffectSO[] CommonSoundEffects;
    [Tooltip("Script responsible of managing all the sounds in the game")]
    public SoundManager SoundManager;
    [Tooltip("Main camera of the scene. The camera-based calculations in most of the scripts will use this one")]
    public Camera ViewportCamera;
    [Tooltip("Currently active cinemachine camera that the viewport camera uses")]
    public CinemachineCamera CurrentCinemachineCamera;
    [Tooltip("GUI of the game")]
    public GUIGameplayControls GameplayControls;
    [Tooltip("Base used by all actors to instantiate healing orbs")]
    public Transform HealingOrbBase;
    [HideInInspector] public bool IsMenuOpen, IsCutscenePlaying;
    #endregion

    #region Mono
    private void Awake()
    {
        Instance = this;
        IsMenuOpen = false;
        IsCutscenePlaying = false;
    }
    private void OnDestroy()
    {
        Instance = null;
    }
    #endregion
}
