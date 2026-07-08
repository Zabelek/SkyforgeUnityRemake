using System.Xml.Serialization;

public class UserProfile
{
    public string Name { get; set; }
    public int HatNumber { get; set; }
    public long Prestige { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    [XmlIgnore]
    public string FileName { get; set; }
}
