using System.Collections.Generic;
using System.Linq;

public class Inventory
{
    public string Name { get; set; }
    public List<InventorySlot> Slots { get; set; }
    public Inventory()
    {
        Slots = new();
    }
    public Inventory(int size) : this()
    {
        for (int i = 0; i < size; i++)
        {
            Slots.Add(new InventorySlot());
        }
    }
    public Inventory(int size, int amountLocked) : this()
    {
        for (int i = 0; i < size; i++)
        {
            if(size - i < amountLocked) 
                Slots.Add(new InventorySlot() { IsLocked = true});
            else
                Slots.Add(new InventorySlot());
        }
    }
    public int CountItems()
    {
        return Slots.Where(s => s.Item != null).Count();
    }
    public bool AddItem(string itemID, int amount)
    {
        var item = SkyforgeLoader.LoadItem(itemID).Result;
        if(item != null)
        {
            if (item.IsStackable)
            {
                var slot = Slots.FirstOrDefault(s => s.IsLocked == false && s.Item?.ID == item.ID);
                if (slot != null && slot.Item.Amount < 9999)
                {
                    int remainingAmount = amount;
                    while (remainingAmount > 0 || slot.Item.Amount == 9999)
                    {
                        slot.Item.Amount++;
                        remainingAmount--;
                    }
                    if (remainingAmount > 0)
                    {
                        slot = Slots.FirstOrDefault(s => s.IsLocked == false && s.Item == null);
                        if (slot != null)
                        {
                            slot.Item = new Item() { ItemSO = item, ID = itemID, Amount = remainingAmount };
                            return true;
                        }
                        else
                            return false;
                    }
                    else return true;
                }
                else
                {
                    slot = Slots.FirstOrDefault(s => s.IsLocked == false && s.Item == null);
                    if (slot != null)
                    {
                        slot.Item = new Item() { ItemSO = item, ID = itemID, Amount = amount };
                        return true;
                    }
                    else
                        return false;
                }
            }
            else
            {
                var slot = Slots.FirstOrDefault(s => s.IsLocked == false && s.Item == null);
                if (slot != null)
                {
                    slot.Item = new Item() { ItemSO = item, ID = itemID, Amount = 1 };
                    return true;
                }
                else
                    return false;
            }
        }
        else
            return false;
    }
    public bool AddItem(string itemID)
    {
        return AddItem(itemID, 1);
    }
}
