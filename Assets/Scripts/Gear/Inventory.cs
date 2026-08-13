using System;
using System.Collections.Generic;
using System.Linq;

public class Inventory
{
    #region Variables
    public event EventHandler<OnItemChangeEventArgs> OnItemChangeEvent;
    public class OnItemChangeEventArgs : EventArgs
    {
        public ItemSO Item;
        public int DifferenceAmount;
        public OnItemChangeEventArgs(ItemSO itemSO, int differenceAmount)
        {
            Item = itemSO;
            DifferenceAmount = differenceAmount;
        }
    }
    public string Name { get; set; }
    public List<InventorySlot> Slots { get; set; }
    private GameplayResources _resources;
    #endregion

    #region Constructors
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
    #endregion

    #region Methods
    public int CountItems()
    {
        return Slots.Where(s => s.Item != null).Count();
    }
    public bool AddItem(string itemID, int amount)
    {
        var item = SkyforgeLoader.LoadItem(itemID).Result;
        bool result = false;
        if(item != null)
        {
            //First chechk if the item is a resource
            if(itemID == "Base_Resource_AelionEidos" || itemID == "Base_Resource_Credits")
            {
                switch(itemID)
                {
                    case "Base_Resource_AelionEidos":
                        _resources.AddResource(GameplayResources.ResourceType.AelionEidos, amount);
                        break;
                    case "Base_Resource_Credits":
                        _resources.AddResource(GameplayResources.ResourceType.Credits, amount);
                        break;
                }
            }
            //if not, add it normally
            else
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
                                var addedItem = new Item(item.ID, remainingAmount);
                                slot.Item = addedItem;
                                result = true;
                            }
                        }
                        else
                        {
                            result = true;
                        }
                    }
                    else
                    {
                        slot = Slots.FirstOrDefault(s => s.IsLocked == false && s.Item == null);
                        if (slot != null)
                        {
                            var addedItem = new Item(item.ID, amount);
                            slot.Item = addedItem;
                            result = true;
                        }
                    }
                }
                else
                {
                    var slot = Slots.FirstOrDefault(s => s.IsLocked == false && s.Item == null);
                    if (slot != null)
                    {
                        var addedItem = new Item(item.ID, amount);
                        slot.Item = addedItem;
                        result = true;
                    }
                }
            }
        }
        if(result == true)
        {
            OnItemChangeEvent?.Invoke(this, new OnItemChangeEventArgs(item, amount));
        }
        return result;
    }
    public bool AddItem(string itemID)
    {
        return AddItem(itemID, 1);
    }
    public bool AddItem(Item item)
    {
        return AddItem(item.ID, item.Amount);
    }
    public InventorySlot GetQuickAccessItem(Item.QuickAccessSlot slot)
    {
        foreach (var invSlot in Slots)
        {
            if (invSlot.Item != null && invSlot.Item.ItemSO.CanBeQuickAccessed == true && invSlot.Item.QASlotType == slot)
            {
                return invSlot;
            }
        }
        return null;
    }
    public void AssignQuickAccessSlot(Item.QuickAccessSlot slot, Item item)
    {
        if(slot != Item.QuickAccessSlot.None)
        {
            var currentSlot = GetQuickAccessItem(slot);
            if (currentSlot != null)
            {
                currentSlot.Item.QASlotType = Item.QuickAccessSlot.None;
            }
        }
        if(item.ItemSO.CanBeQuickAccessed)
        {
            item.QASlotType = slot;
        }
    }
    public void SetResourcesRef(GameplayResources resources)
    {
        _resources = resources;
    }
    #endregion
}
