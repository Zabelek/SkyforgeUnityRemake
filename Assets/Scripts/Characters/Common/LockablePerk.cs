using System;

public class LockablePerk
{
    #region variables
    public EventHandler OnEnabled, OnDisabled;
    public PerkSO Perk { get; set; }
    public bool Unlocked { get; set; }
    public bool Enabled { get; private set; }
    #endregion

    #region Constructors
    public LockablePerk(PerkSO perk, bool unlocked)
    {
        Perk = perk;
        Unlocked = unlocked;
    }
    public LockablePerk(PerkSO perk)
    {
        Perk = perk;
        Unlocked = false;
    }
    #endregion

    #region Methods
    public void Enable()
    {
        Enabled = true;
        OnEnabled?.Invoke(this, EventArgs.Empty);
    }
    public void Disable()
    {
        Enabled = false;
        OnDisabled?.Invoke(this, EventArgs.Empty);
    }
    #endregion
}