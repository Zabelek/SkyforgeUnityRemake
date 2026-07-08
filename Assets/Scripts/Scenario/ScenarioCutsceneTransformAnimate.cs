using UnityEngine;

public class ScenarioCutsceneTransformAnimate : ScenarioCutsceneTransform
{
    #region Variables
    [Header("Animation Related Variables")]
    [Tooltip("The character has to have Character Animation Behaviour with a proper Animator Controller to animate")]
    public CharacterBehaviour AnimatedCharacter;
    [Tooltip("The Animator Controller containing only one animation that the character will be switched into upon this transform activation")]
    [SerializeField] private RuntimeAnimatorController _animatorController;
    private bool _animationApplied;
    #endregion

    #region Mono
    protected override void Awake()
    {
        base.Awake();
        _animationApplied = false;
    }
    #endregion

    #region Methods
    public override void Perform(float cutsceneTimer)
    {
        if (AnimatedCharacter != null && _animationApplied == false)
        {
            AnimatedCharacter.PlayCutscene(_animatorController);
            _animationApplied = true;
        }
        else if (cutsceneTimer > StartTimer + Duration)
        {
            AnimatedCharacter.EndCutscene();
            Finished = true;
        }
    }
    #endregion
}
