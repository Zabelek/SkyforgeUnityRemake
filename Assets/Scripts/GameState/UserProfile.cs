using System.Collections.Generic;
using System.Xml.Serialization;

public class UserProfile
{
    public class PerkState
    {
        public string PerkID;
        public bool Enabled;
    }
    public string Name { get; set; }
    public int HatNumber { get; set; }
    public long Prestige { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    [XmlIgnore]
    public string FileName { get; set; }
    public List<PerkState> AcquiredPerks { get; set; }
    public GameplayResources GameplayResources { get; set; }
    public string CurrentlyPickedClass { get; set; }

    public UserProfile()
    {
        Difficulty = new();
        AcquiredPerks = new();
        GameplayResources = new();
    }
}
