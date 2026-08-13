using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

public class UserProfile
{
    #region Variables
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
    #endregion

    #region Constructors
    public UserProfile()
    {
        Difficulty = new();
        AcquiredPerks = new();
        GameplayResources = new();
        Equipment = new();
        _inventory = new();
    }
    public void Init()
    {
        Inventory.SetResourcesRef(GameplayResources);
    }
    #endregion
}
