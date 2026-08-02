using System.Xml.Serialization;

public class Item
{
    public string ID { get; set; }
    [XmlIgnore]
    public ItemSO ItemSO;
    public int Amount { get; set; } = 1;
}
