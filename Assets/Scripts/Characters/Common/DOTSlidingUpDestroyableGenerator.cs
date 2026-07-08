using UnityEngine;

public class DOTSlidingUpDestroyableGenerator : DestroyableObjectBehaviour
{
    #region Variables
    public bool IsUp { get; set; }
    public bool SlideAlreadyActivated { get; set; }
    public bool DeadMeshAppeared { get; set; }
    #endregion

    #region Mono
    protected override void Start()
    {
        Selectable = false;
        CanBeDamaged = false;
        SlideAlreadyActivated = false;
        IsUp = false;
        DeadMeshAppeared = false;
    }
    #endregion

    #region Methods
    public void RaiseUp()
    {
        Selectable = true;
        CanBeDamaged = true;
        this.IsUp = true;
    }
    public override void Kill(CharacterBehaviour killer)
    {
        base.Kill(killer);
        //reenabling colliders so that the player won't go through a destroyed generator
        if (_colliders.Length > 0)
        {
            foreach (var col in _colliders)
                col.enabled = true;
        }
        DeadMeshAppeared = true;
        Selectable = false;
        CanBeDamaged = false;
    }
    #endregion
}
