public class PYTHDryadAnimationBehaviour : MonsterAnimationBehaviour
{
    #region Mono
    protected override void Start()
    {
        base.Start();
        if(_character is PYTHDryadBehaviour)
        {
            ((PYTHDryadBehaviour)_character).OnWeakenAction += Dryad_OnWeakenAction;
        }
    }
    #endregion

    #region Methods
    private void Dryad_OnWeakenAction(object sender, System.EventArgs e)
    {
        _animator.SetTrigger("Weaken");
    }
    public override void TriggerAttackAnimation()
    {
        base.TriggerAttackAnimation();
        _animator.SetBool("Second_Attack", !_animator.GetBool("Second_Attack"));
    }
    #endregion
}
