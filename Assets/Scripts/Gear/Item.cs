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
    #endregion
}
