using System;
using System.Collections.Generic;
using UnityEngine;

public class GIUChestLootInterface : MonoBehaviour
{
    #region Variables
    [SerializeField] private GUIItemPickView _itemsView;
    private LootChestBehaviour _currentChest;
    [SerializeField] private GUICommonButton _takeAllButton, _closeButton;
    private List<GUIInventorySlot> _currentSlots;
    [Tooltip("Prefab used to spawn tooltips")]
    [SerializeField] private GUITooltip _tooltipBase;
    private GUITooltip _currentTooltip;
    [Tooltip("Canvas reference to handle tooltip positioning")]
    [SerializeField] private Canvas _tooltipCanvas;
    [Tooltip("Where tooltips will be spawned")]
    [SerializeField] private Transform _tooltipsParent;
    #endregion

    #region Mono
    private void Awake()
    {
        _takeAllButton.OnClick += TakeAllButton_Clicked;
        _closeButton.OnClick += CloseButton_Clicked;
    }
    #endregion

    #region Methods
    private void SetUpNewTooltip(ItemSO itemSO)
    {
        _currentTooltip = Instantiate(_tooltipBase, _tooltipsParent);
        _currentTooltip.SetCanvas(_tooltipCanvas);
        _currentTooltip.SetForItem(itemSO);
    }
    public void SetChest(LootChestBehaviour chest)
    {
        _currentChest = chest;
        _currentSlots = _itemsView.AssignSlots(_currentChest.Slots);
        foreach(var slot in _currentSlots)
        {
            slot.OnPointerDownEvent += SlotPointerDownAction;
            slot.OnPointerEnterEvent += SlotPointerEnterAction;
            slot.OnPointerExitEvent += SlotPointerExitAction;
        }
    }
    private void CloseView()
    {
        foreach (var slot in _currentSlots)
        {
            slot.OnPointerDownEvent -= SlotPointerDownAction;
            slot.OnPointerEnterEvent -= SlotPointerEnterAction;
            slot.OnPointerExitEvent -= SlotPointerExitAction;
        }
        _currentSlots.Clear();
        Globals.Instance.CurrentOpenChest?.AnimateClose();
        Globals.Instance.CurrentOpenChest = null;
        if (_currentTooltip != null)
        {
            Destroy(_currentTooltip.gameObject);
            _currentTooltip = null;
        }
    }
    private void TakeItem(GUIInventorySlot guiSlot)
    {
        if (guiSlot.InventorySlot.Item != null)
        {
            //add checking how much of the items has been transfered if the storage was filled during treansfer
            if (SkyforgeLoader.CurrentProfile.Inventory.AddItem(guiSlot.InventorySlot.Item.ID, guiSlot.InventorySlot.Item.Amount)) ;
            {
                guiSlot.InventorySlot.Item = null;
                guiSlot.UpdateSlot();
            }
        }
    }
    #endregion

    #region EventHandlers
    private void TakeAllButton_Clicked(object sender, EventArgs e)
    {
        foreach(var slot in _currentSlots)
        {
            TakeItem(slot);
        }
        CloseView();
    }
    private void CloseButton_Clicked(object sender, EventArgs e)
    {
        CloseView();
    }
    private void SlotPointerEnterAction(object sender, EventArgs e)
    {
        if (_currentTooltip == null && sender is GUIInventorySlot && (sender as GUIInventorySlot).InventorySlot?.Item != null)
        {
            SetUpNewTooltip((sender as GUIInventorySlot).InventorySlot.Item.ItemSO);
        }
    }
    private void SlotPointerExitAction(object sender, EventArgs e)
    {
        if (_currentTooltip != null)
        {
            Destroy(_currentTooltip.gameObject);
            _currentTooltip = null;
        }
    }
    private void SlotPointerDownAction(object sender, EventArgs e)
    {
        if (_currentTooltip != null)
        {
            Destroy(_currentTooltip.gameObject);
            _currentTooltip = null;
        }
        if (sender is GUIInventorySlot)
        {
            TakeItem(sender as GUIInventorySlot);
        }
    }
    #endregion
}
