using System;

public class PYTHDryadBehaviour : MonsterBehaviour
{
    #region Variables
    public event EventHandler OnWeakenAction;
    #endregion

    #region Methods
    public override void PerformEmote(object sender, EventArgs e)
    {
        OnWeakenAction?.Invoke(this, EventArgs.Empty);
    }
    #endregion
}
