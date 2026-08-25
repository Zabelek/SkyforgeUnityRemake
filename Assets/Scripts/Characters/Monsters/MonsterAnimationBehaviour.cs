using System.Linq;
using UnityEngine;

public class MonsterAnimationBehaviour : CharacterAnimationBehaviour
{
    private const float WALK_ANIMATION_TRESHOLD = 0.4f;

    #region Variables
    [Header("Monster Related Variables")]
    [SerializeField] AIHandlerBehaviour _aiHandler;
    public int AvailableAttackVariants = 1;
    #endregion

    #region Mono
    protected override void Start()
    {
        base.Start();
        if(_character is MonsterBehaviour)
        {
            ((MonsterBehaviour)_character).OnHurtAction += Character_OnHurtAction;
            ((MonsterBehaviour)_character).OnEmoteAction += Character_OnEmoteAction;
            ((MonsterBehaviour)_character).OnDeathEvent += Character_OnDeathAction;
        }
        else
        {
            Debug.Log("WARNING! Monster Animation Behaviour attached to non-monster character! There will be errors!");
        }
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (_character.IsDead)
            _animator.SetBool("IsDead", true);
        if (_character.IsFalling)
            _animator.SetBool("IsFalling", true);
        else
            _animator.SetBool("IsFalling", false);
        if (TryGetMovingSpeedFromAiHandler(out var speed))
        {
            if (speed > 0)
            {
                _animator.SetBool("IsMoving", true);
                if(_hasWalkRunAnimations)
                {
                    if (speed < WALK_ANIMATION_TRESHOLD)
                        _animator.SetFloat("Speed_Indicator", 0);
                    else
                        _animator.SetFloat("Speed_Indicator", 1);
                }
                //smaller enemies have to be animated faster while maintaining the same speed so that the animation matches their actual movenment
                var animationSpeed = (2 / _character.GetComponentInChildren<Animator>().transform.localScale.x) * speed;
                _animator.SetFloat("Speed", animationSpeed);
            }
            else
            {
                _animator.SetBool("IsMoving", false);
            }
        }
    }
    #endregion

    #region Methods
    protected virtual void Character_OnEmoteAction(object sender, System.EventArgs e)
    {
        _animator.SetTrigger("Emote");
    }
    protected virtual void Character_OnHurtAction(object sender, System.EventArgs e)
    {
        if (_character.IsDead)
        {
            _animator.SetBool("IsDead", true);
        }
        _animator.SetFloat("Hurt_Variance", (Random.Range(0, 2)));
        _animator.SetTrigger("Hurt");
    }
    private void Character_OnDeathAction(object sender, System.EventArgs e)
    {
        if(_animator.parameters.Any(p => p.name == "Death" && p.type == AnimatorControllerParameterType.Trigger))
        {
            _animator.SetTrigger("Death");
        }
    }
    public virtual bool TryGetMovingSpeedFromAiHandler(out float speed)
    {
        if (_aiHandler != null)
        {
            speed = (_aiHandler.GetMovingSpeed() / _character.CharacterSO.MovementSpeed);
            return true;
        }
        else
        {
            speed = 0;
            return false;
        }
    }
    public virtual void TriggerAttackAnimation()
    {
        if (AvailableAttackVariants > 1)
        {
            _animator.SetFloat("Attack_Variance", (float)(Random.Range(0, AvailableAttackVariants)-1));
        }
        _animator.SetTrigger("Attack");
    }
    #endregion
}
