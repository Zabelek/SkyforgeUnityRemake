public class InventorySlot
{
    public Item Item { get; set; }
    public bool IsLocked { get; set; }
    public InventorySlot()
    {
        IsLocked = false;
    }
}
