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
            if (_weaponSlot.Item != null)
            {
                _weaponSlot.Item.ItemSO = SkyforgeLoader.ItemRegistry.RegisteredItems.FirstOrDefault(i => i.ID == _weaponSlot.Item.ID);
            }
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
            if (_armorSlot.Item != null)
            {
                _armorSlot.Item.ItemSO = SkyforgeLoader.ItemRegistry.RegisteredItems.FirstOrDefault(i => i.ID == _armorSlot.Item.ID);
            }
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
            if (_artifactSlot.Item != null)
            {
                _artifactSlot.Item.ItemSO = SkyforgeLoader.ItemRegistry.RegisteredItems.FirstOrDefault(i => i.ID == _artifactSlot.Item.ID);
            }
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
            if (_ringSlot.Item != null)
            {
                _ringSlot.Item.ItemSO = SkyforgeLoader.ItemRegistry.RegisteredItems.FirstOrDefault(i => i.ID == _ringSlot.Item.ID);
            }
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
            if (_amuletSlot.Item != null)
            {
                _amuletSlot.Item.ItemSO = SkyforgeLoader.ItemRegistry.RegisteredItems.FirstOrDefault(i => i.ID == _amuletSlot.Item.ID);
            }
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
            if (_broochSlot.Item != null)
            {
                _broochSlot.Item.ItemSO = SkyforgeLoader.ItemRegistry.RegisteredItems.FirstOrDefault(i => i.ID == _broochSlot.Item.ID);
            }
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
            if (_braceletSlot.Item != null)
            {
                _braceletSlot.Item.ItemSO = SkyforgeLoader.ItemRegistry.RegisteredItems.FirstOrDefault(i => i.ID == _braceletSlot.Item.ID);
            }
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
            if (_sapphiteSlot.Item != null)
            {
                _sapphiteSlot.Item.ItemSO = SkyforgeLoader.ItemRegistry.RegisteredItems.FirstOrDefault(i => i.ID == _sapphiteSlot.Item.ID);
            }
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
            if (_rubySlot.Item != null)
            {
                _rubySlot.Item.ItemSO = SkyforgeLoader.ItemRegistry.RegisteredItems.FirstOrDefault(i => i.ID == _rubySlot.Item.ID);
            }
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
            if (_emeraldSlot.Item != null)
            {
                _emeraldSlot.Item.ItemSO = SkyforgeLoader.ItemRegistry.RegisteredItems.FirstOrDefault(i => i.ID == _emeraldSlot.Item.ID);
            }
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
            if (_topazSlot.Item != null)
            {
                _topazSlot.Item.ItemSO = SkyforgeLoader.ItemRegistry.RegisteredItems.FirstOrDefault(i => i.ID == _topazSlot.Item.ID);
            }
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
    public Item Equip(Item item, Equipment.InventoryType invType)
    {
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
    #endregion
}