public class InventorySlot
{
    #region Variables
    public Item Item { get; set; }
    public bool IsLocked { get; set; }
    #endregion

    #region Constructors
    public InventorySlot()
    {
        IsLocked = false;
    }
    #endregion

    #region Methods
    public bool ActivateItem()
    {
        if (Item != null)
        {
            return Item.Activate(this);
        }
        else return false;
    }
    #endregion
}
