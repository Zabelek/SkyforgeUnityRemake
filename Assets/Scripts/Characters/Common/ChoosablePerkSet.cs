using System;
using System.Collections.Generic;

public class ChoosablePerkSet
{
    #region Variables
    public PerkSetSO PerkSetSO { get; private set; }
    public List<LockablePerk> Perks { get; private set; }
    #endregion

    #region Methods
    public ChoosablePerkSet(PerkSetSO perkSet)
    {
        PerkSetSO = perkSet;
        Perks = new();
        foreach (var perk in perkSet.Perks)
        {
            AddPerk(new LockablePerk(perk));
        }
    }
    public void AddPerk(LockablePerk perk)
    {
        perk.OnEnabled += PerkEnabled;
        Perks.Add(perk);
    }
    private void RemovePerk(LockablePerk perk)
    {
        perk.OnEnabled -= PerkEnabled;
        Perks.Remove(perk);
    }
    public void ClearPerks()
    {
        foreach(var perk in Perks)
        {
            perk.OnEnabled -= PerkEnabled;
        }
        Perks.Clear();
    }
    #endregion

    #region EventHandlers
    private void PerkEnabled(object sender, EventArgs e)
    {
        foreach (var perk in Perks)
        {
            if (perk != sender)
            {
                perk.Disable();
            }
        }
    }
    #endregion
}