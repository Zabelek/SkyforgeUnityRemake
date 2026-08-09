using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

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
    public Equipment Equipment { get; set; }
    private Inventory _inventory;
    public Inventory Inventory {

        get
        {
            return _inventory;
        }
        set
        {
            foreach (var invSlot in value.Slots)
            {
                if (invSlot.Item != null)
                {
                    invSlot.Item.ItemSO = SkyforgeLoader.ItemRegistry.RegisteredItems.FirstOrDefault(i => i.ID == invSlot.Item.ID);
                }
            }
            _inventory = value;
        }
    }

    public UserProfile()
    {
        Difficulty = new();
        AcquiredPerks = new();
        GameplayResources = new();
        Equipment = new();
    }
    //These methods will be moved away in the future so that equipping will be done directly through the equipment
}
