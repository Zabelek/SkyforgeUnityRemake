using UnityEngine;

public class GUIBagControls : GIItemDragBehaviour
{
    #region Variables
    [SerializeField] private GUIGameMenuEquipmentControls _equipmentControls;
    [Header("Slots")]
    [Tooltip("Prefab used to spawn new slots")]
    [SerializeField] private GUIInventorySlot _slotPattern;
    [Tooltip("Slot that destroyes any item it receives.")]
    [SerializeField] private GUIInventorySlot _destroySlot;
    [Tooltip("Where slots will be spawned")]
    [SerializeField] private Transform _slotsParent;
    #endregion

    #region Mono
    protected override void Awake()
    {
        base.Awake();
        if (SkyforgeLoader.CurrentProfile != null)
        {
            foreach(var slot in SkyforgeLoader.CurrentProfile.Inventory.Slots)
            {
                var slotWidget = Instantiate(_slotPattern, _slotsParent);
                slotWidget.InventorySlot = slot;
                AddSlot(slotWidget);
                _destroySlot.InventorySlot = new();
                AddSlot(_destroySlot);
                _destroySlot.UpdateSlot();
            }
        }
    }
    protected override void Update()
    {
        base.Update();
        if(_destroySlot.InventorySlot.Item!= null)
        {
            _destroySlot.InventorySlot.Item = null;
            _destroySlot.UpdateSlot();
        }
    }
    #endregion

    #region Methods
    protected override void ActivateItem(InventorySlot inventorySlot)
    {
        if (inventorySlot!= null && inventorySlot.ActivateItem())
        {
            _equipmentControls.UpdateValues();
        }
    }
    #endregion
}
