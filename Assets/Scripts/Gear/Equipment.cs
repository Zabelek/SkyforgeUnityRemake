using System.Linq;

public class Equipment
{
    #region Variables
    public enum InventoryType { Armor, Weapon, Artifact, Ring, Amulet, Brooch, Bracelet, Sapphite, Ruby, Emerald, Topaz}
    private InventorySlot _weaponSlot;
    public InventorySlot WeaponSlot
    {
        get
        {
            return _weaponSlot;
        }
        set
        {
            _weaponSlot = value;
            SetUpSlotOnInit(_weaponSlot);
        }
    }
    private InventorySlot _armorSlot;
    public InventorySlot ArmorSlot
    {
        get
        {
            return _armorSlot;
        }
        set
        {
            _armorSlot = value;
            SetUpSlotOnInit(_armorSlot);
        }
    }
    private InventorySlot _artifactSlot;
    public InventorySlot ArtifactSlot
    {
        get
        {
            return _artifactSlot;
        }
        set
        {
            _artifactSlot = value;
            SetUpSlotOnInit(_artifactSlot);
        }
    }
    private InventorySlot _ringSlot;
    public InventorySlot RingSlot
    {
        get
        {
            return _ringSlot;
        }
        set
        {
            _ringSlot = value;
            SetUpSlotOnInit(_ringSlot);
        }
    }
    private InventorySlot _amuletSlot;
    public InventorySlot AmuletSlot
    {
        get
        {
            return _amuletSlot;
        }
        set
        {
            _amuletSlot = value;
            SetUpSlotOnInit(_amuletSlot);
        }
    }
    private InventorySlot _broochSlot;
    public InventorySlot BroochSlot
    {
        get
        {
            return _broochSlot;
        }
        set
        {
            _broochSlot = value;
            SetUpSlotOnInit(_broochSlot);
        }
    }
    private InventorySlot _braceletSlot;
    public InventorySlot BraceletSlot
    {
        get
        {
            return _braceletSlot;
        }
        set
        {
            _braceletSlot = value;
            SetUpSlotOnInit(_braceletSlot);
        }
    }
    private InventorySlot _sapphiteSlot;
    public InventorySlot SapphiteSlot
    {
        get
        {
            return _sapphiteSlot;
        }
        set
        {
            _sapphiteSlot = value;
            SetUpSlotOnInit(_sapphiteSlot);
        }
    }
    private InventorySlot _rubySlot;
    public InventorySlot RubySlot
    {
        get
        {
            return _rubySlot;
        }
        set
        {
            _rubySlot = value;
            SetUpSlotOnInit(_rubySlot);
        }
    }
    private InventorySlot _emeraldSlot;
    public InventorySlot EmeraldSlot
    {
        get
        {
            return _emeraldSlot;
        }
        set
        {
            _emeraldSlot = value;
            SetUpSlotOnInit(_emeraldSlot);
        }
    }
    private InventorySlot _topazSlot;
    public InventorySlot TopazSlot
    {
        get
        {
            return _topazSlot;
        }
        set
        {
            _topazSlot = value;
            SetUpSlotOnInit(_topazSlot);
        }
    }
    #endregion

    #region Constructors
    public Equipment()
    {
        //for now the rest of fields have to be null to make them locked in GUI
        WeaponSlot = new();
        ArmorSlot = new();
        ArtifactSlot = new();
    }
    #endregion

    #region Methods
    public Item Equip(Item item, Equipment.InventoryType invType)
    {
        //returns the previous item that was in a destination slot, or null if the item was null
        Item ret = null;
        if (invType == Equipment.InventoryType.Armor)
        {
            ret = ArmorSlot.Item;
            ArmorSlot.Item = item;
            SkyforgeLoader.EquipmentChanged = true;
        }
        else if (invType == Equipment.InventoryType.Weapon)
        {
            ret = WeaponSlot.Item;
            WeaponSlot.Item = item;
            SkyforgeLoader.EquipmentChanged = true;
        }
        else if (invType == Equipment.InventoryType.Artifact)
        {
            ret = ArtifactSlot.Item;
            ArtifactSlot.Item = item;
            SkyforgeLoader.EquipmentChanged = true;
        }
        return ret;
    }
    public Item GetEquipment(Equipment.InventoryType invType)
    {
        if (invType == Equipment.InventoryType.Armor)
        {
            return ArmorSlot.Item;
        }
        else if (invType == Equipment.InventoryType.Weapon)
        {
            return WeaponSlot.Item;
        }
        else if (invType == Equipment.InventoryType.Artifact)
        {
            return ArtifactSlot.Item;
        }
        else return null;
    }
    private void SetUpSlotOnInit(InventorySlot slot)
    {
        if (slot.Item != null)
        {
            slot.Item.ItemSO = SkyforgeLoader.ItemRegistry.RegisteredItems.FirstOrDefault(i => i.ID == slot.Item.ID);
            if (slot.Item != null && SkyforgeLoader.ItemRegistry != null)
            {
                slot.Item.ItemSO = SkyforgeLoader.ItemRegistry.RegisteredItems.FirstOrDefault(i => i.ID == slot.Item.ID);
                if (slot.Item.ItemSO.IsUpgradable && slot.Item.ItemUpgradeState == null)
                    slot.Item.ItemUpgradeState = new();
            }
        }
    }
    #endregion
}