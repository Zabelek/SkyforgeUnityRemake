public class HeroAnimationBehaviour : CharacterAnimationBehaviour
{
    #region Variables
    #endregion

    #region Mono
    protected override void Start()
    {
        base.Start();
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    #endregion

    #region Methods
    public virtual void ScheduleWeaponHide()
    {

        _animator.SetTrigger("WeaponHide");
        _animator.SetBool("IsWeaponOut", false);
    }
    public virtual void ScheduleWeaponDraw()
    {
        _animator.SetTrigger("WeaponDraw"); 
        _animator.SetBool("IsWeaponOut", true);
    }
    #endregion
}
