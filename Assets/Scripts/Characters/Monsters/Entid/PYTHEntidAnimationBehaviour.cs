public class PYTHEntidAnimationBehaviour : MonsterAnimationBehaviour
{
    #region Mono
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    #endregion

    #region Methods
    public override void TriggerAttackAnimation()
    {
        base.TriggerAttackAnimation();
        _animator.SetBool("Second_Attack", !_animator.GetBool("Second_Attack"));
    }
    #endregion
}
