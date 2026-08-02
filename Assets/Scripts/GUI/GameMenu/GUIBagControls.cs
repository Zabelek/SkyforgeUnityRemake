using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GUIBagControls : MonoBehaviour
{
    #region Variables
    [SerializeField] private Transform _slotsParent, _tooltipsParent;
    [SerializeField] private GUIInventorySlot _slotPattern;
    private List<GUIInventorySlot> _currentSlots;
    [SerializeField] private GUITooltip _tooltipBase;
    private GUITooltip _currentTooltip;
    [SerializeField] private Canvas _tooltipCanvas;
    [SerializeField] private Image _visualItemGhost;
    private GUIInventorySlot _currentlyDraggedSlot, _currentDragTargetSlot;
    #endregion

    #region Mono
    public void Awake()
    {
        _currentSlots = new();
        if (SkyforgeLoader.CurrentProfile != null)
        {
            foreach(var slot in SkyforgeLoader.CurrentProfile.Inventory.Slots)
            {
                var slotWidget = Instantiate(_slotPattern, _slotsParent);
                slotWidget.InventorySlot = slot;
                _currentSlots.Add(slotWidget);
                slotWidget.OnPointerEnterEvent += SlotPointerEnterAction;
                slotWidget.OnPointerExitEvent += SlotPointerExitAction;
                slotWidget.OnPointerUpEvent += SlotPointerUpAction;
                slotWidget.OnPointerDownEvent += SlotPointerDownAction;
            }
        }
    }
    private void Update()
    {
        if(_currentlyDraggedSlot != null)
        {
            PositionItemGhost();
        }
        if (Input.GetMouseButton(0) == false && _currentlyDraggedSlot != null)
        {
            _currentlyDraggedSlot = null;
            _visualItemGhost.gameObject.SetActive(false);
        }
    }
    public void OnDestroy()
    {
        foreach(var slot in _currentSlots)
        {
            slot.OnPointerEnterEvent -= SlotPointerEnterAction;
            slot.OnPointerExitEvent -= SlotPointerExitAction;
            slot.OnPointerUpEvent -= SlotPointerUpAction;
            slot.OnPointerDownEvent -= SlotPointerDownAction;
        }
    }
    #endregion

    #region Methods
    public void UpdateView()
    {
        foreach(var slot in _currentSlots)
        {
            slot.UpdateSlot();
        }
    }
    private void SetUpNewTooltip(ItemSO itemSO)
    {
        _currentTooltip = Instantiate(_tooltipBase, _tooltipsParent);
        _currentTooltip.SetCanvas(_tooltipCanvas);
        _currentTooltip.SetTitle(itemSO.Name);
        _currentTooltip.SetDescription(itemSO.Description);
        _currentTooltip.SetTitleImage(itemSO.InterfaceSprite);
        if(itemSO is WeaponSO)
        {
            _currentTooltip.AddStatBonus("Damage Bonus: ", ((WeaponSO)itemSO).GetDamage(), false);
            _currentTooltip.SetDescription(((WeaponSO)itemSO).Type.Name);
            if (itemSO.Rarity == ItemSO.RarityLevel.Legendary)
            {
                _currentTooltip.SetSpecialDescription(itemSO.Description);
            }
        }
        else if(itemSO is ArmorSO)
        {
            _currentTooltip.AddStatBonus("Defense Bonus: ", ((ArmorSO)itemSO).BaseDefense, true);
        }
    }
    private void PositionItemGhost()
    {
        Vector2 mousePos = new Vector2(Input.mousePosition.x / Screen.width * _tooltipCanvas.renderingDisplaySize.x, Input.mousePosition.y / Screen.height * _tooltipCanvas.renderingDisplaySize.y);
        _visualItemGhost.rectTransform.anchoredPosition = new Vector2(mousePos.x, mousePos.y);
    }
    #endregion

    #region EventHandlers
    private void SlotPointerEnterAction(object sender, EventArgs e)
    {
        if(_currentlyDraggedSlot == null)
        {
            if (_currentTooltip == null && sender is GUIInventorySlot && (sender as GUIInventorySlot).InventorySlot.Item != null)
            {
                SetUpNewTooltip((sender as GUIInventorySlot).InventorySlot.Item.ItemSO);
            }
        }
        else if ((sender as GUIInventorySlot).InventorySlot.IsLocked == false)
        {
            _currentDragTargetSlot = sender as GUIInventorySlot;
        }
    }
    private void SlotPointerDownAction(object sender, EventArgs e)
    {
        if (_currentTooltip != null)
        {
            Destroy(_currentTooltip.gameObject);
            _currentTooltip = null;
        }
        if (sender is GUIInventorySlot && (sender as GUIInventorySlot).InventorySlot.Item != null)
        {
            _currentlyDraggedSlot = sender as GUIInventorySlot;
            _visualItemGhost.gameObject.SetActive(true);
            _visualItemGhost.sprite = (sender as GUIInventorySlot).InventorySlot.Item.ItemSO.InterfaceSprite;
        }
    }
    private void SlotPointerUpAction(object sender, EventArgs e)
    {
        if(_currentlyDraggedSlot != null && _currentDragTargetSlot != null)
        {
            if(_currentlyDraggedSlot != _currentDragTargetSlot)
            {
                var tempItem = _currentDragTargetSlot.InventorySlot.Item;
                _currentDragTargetSlot.InventorySlot.Item = _currentlyDraggedSlot.InventorySlot.Item;
                _currentlyDraggedSlot.InventorySlot.Item = tempItem;
                _currentDragTargetSlot.UpdateSlot();
                _currentlyDraggedSlot.UpdateSlot();
                _currentDragTargetSlot = null;
            }
        }
        _currentlyDraggedSlot = null;
        _visualItemGhost.gameObject.SetActive(false);
    }
    private void SlotPointerExitAction(object sender, EventArgs e)
    {
        if(_currentTooltip != null)
        {
            Destroy(_currentTooltip.gameObject);
            _currentTooltip = null;
            _currentDragTargetSlot = null;
        }
    }
    #endregion
}
