using System.Linq;
using UnityEngine;

public class CharacterAnimationBehaviour : MonoBehaviour
{
    #region Variables
    [SerializeField] protected Animator _animator;
    [SerializeField] protected CharacterBehaviour _character;
    protected bool _hasWalkRunAnimations;
    #endregion

    #region Mono
    protected virtual void Start()
    {
        //In case the animator wasn't set in editor, but is present
        if(_animator == null)
            _animator = GetComponent<Animator>();
        if (_animator != null)
        {
            //if the animator controller has this variable, it means walk animation is supported
            _hasWalkRunAnimations = _animator.parameters.Any(p => p.name == "Speed_Indicator");
        }
        //makes sure that character and animator see each other
        if (_character != null)
        {
            _character.SetAnimationBehaviour(this);
        }
    }
    protected virtual void FixedUpdate() {}
    #endregion

    #region Methods
    public virtual void TriggerAnimation(string animationName)
    {
        if (_character != null && _animator != null)
        {
            _animator.SetTrigger(animationName);
        }
    }
    public virtual void SetAnimationBool(string boolName, bool value)
    {
        if (_character != null && _animator != null)
        {
            _animator.SetBool(boolName, value);
        }
    }
    public void StopMovementAnimation()
    {
        //used for static abilities that can be interrupted by movement in animator controller itself.
        //If the animator is already "running" and the ability would be triggered, it'll immediately stop, as Update won't turn off running animation in time.
        if(_animator.parameters.Any(p => p.name == "IsMoving"))
            _animator.SetBool("IsMoving", false);
    }
    public virtual void SetController(RuntimeAnimatorController controller)
    {
        _animator.runtimeAnimatorController = controller;
        _animator.ResetControllerState();
        if(_animator.parameters.Any(p => p.name == "Init"))
            _animator.SetTrigger("Init");
        //Recheck needs to be performed in case new animator controller is different than previous one in terms of walking support
        _hasWalkRunAnimations = _animator.parameters.Any(p => p.name == "Speed_Indicator");

    }
    public virtual void PerformEmote(EmoteSO emote)
    {
        _animator.SetTrigger("Emote");
        _animator.SetBool("Emote_Idle", true);
    }
    public virtual void StopEmote()
    {
        _animator.SetTrigger("Emote");
        _animator.SetBool("Emote_Idle", false);
    }
    public void ResetAnimation()
    {
        _animator.ResetControllerState();
    }
    #endregion
}
