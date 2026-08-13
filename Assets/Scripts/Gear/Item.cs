using NUnit.Framework;
using System.Xml.Serialization;

public class Item
{
    public enum QuickAccessSlot { None, QA1, QA2, QA3, QA4, QA5, QA6 }

    #region Variables
    public string ID { get; set; }
    [XmlIgnore]
    public ItemSO ItemSO;
    public int Amount { get; set; } = 1;
    public QuickAccessSlot QASlotType = QuickAccessSlot.None;
    #endregion

    #region Constructors
    public Item()
    {

    }
    public Item(string itemID)
    {
        ID = itemID;
        ItemSO = SkyforgeLoader.LoadItem(itemID).Result;
    }
    public Item(string itemID, int amount)
    {
        ID = itemID;
        ItemSO = SkyforgeLoader.LoadItem(itemID).Result;
        Amount = amount;
    }
    #endregion

    #region Methods
    public bool Activate(InventorySlot parent)
    {
        if (ItemSO is WeaponSO)
        {
            parent.Item = SkyforgeLoader.CurrentProfile.Equipment.Equip(this, Equipment.InventoryType.Weapon);
            return true;
        }
        else if (ItemSO is ArmorSO)
        {
            parent.Item = SkyforgeLoader.CurrentProfile.Equipment.Equip(this, Equipment.InventoryType.Armor);
            return true;
        }
        else if (ItemSO is ArtifactSO)
        {
            parent.Item = SkyforgeLoader.CurrentProfile.Equipment.Equip(this, Equipment.InventoryType.Artifact);
            return true;
        }
        return false;
    }
    #endregion
}
