using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIBagControls : TooltipDisplayerBehaviour
{
    private const float DOUBLE_CLICK_TRESHOLD = 0.2f;

    #region Variables
    [SerializeField] private GUIGameMenuEquipmentControls _equipmentControls;
    [Header("Slots")]
    [Tooltip("Prefab used to spawn new slots")]
    [SerializeField] private GUIInventorySlot _slotPattern;
    [Tooltip("Where slots will be spawned")]
    [SerializeField] private Transform _slotsParent;
    private List<GUIInventorySlot> _currentSlots;
    private GUIInventorySlot _currentlyDraggedSlot, _currentDragTargetSlot;
    [Tooltip("icon displayed when the item is dragged")]
    [SerializeField] private Image _visualItemGhost;
    //mouse related variables
    private Vector3 _mouseOffsetFromFirstClick;
    private float _doubleClickTimer;
    [Header("Sound")]
    [SerializeField] protected SoundEffectSO _itemMoveSound;
    [SerializeField] protected SoundEffectSO _itemEquipSound;
    #endregion

    #region Mono
    private void Awake()
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
    private void FixedUpdate()
    {
        if(_doubleClickTimer > 0)
        {
            _doubleClickTimer -= Time.fixedDeltaTime;
            if(_doubleClickTimer < 0)
                _doubleClickTimer = 0;
        }
    }
    private void OnDestroy()
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
    private void PositionItemGhost()
    {
        if ((_mouseOffsetFromFirstClick - Input.mousePosition).magnitude > 0.01f && _visualItemGhost.gameObject.activeSelf == false)
        {
            _visualItemGhost.gameObject.SetActive(true);
        }
        Vector2 mousePos = new Vector2(Input.mousePosition.x / Screen.width * _tooltipCanvas.renderingDisplaySize.x, Input.mousePosition.y / Screen.height * _tooltipCanvas.renderingDisplaySize.y);
        _visualItemGhost.rectTransform.anchoredPosition = new Vector2(mousePos.x, mousePos.y);
    }
    private void ActivateItem(InventorySlot inventorySlot)
    {
        if (inventorySlot!= null && inventorySlot.ActivateItem())
        {
            _equipmentControls.UpdateValues();
        }
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
        DestroyCurrentTooltip();
        if (sender is GUIInventorySlot && (sender as GUIInventorySlot).InventorySlot.Item != null)
        {
            _currentlyDraggedSlot = sender as GUIInventorySlot;
            _mouseOffsetFromFirstClick = Input.mousePosition;
            _visualItemGhost.sprite = (sender as GUIInventorySlot).InventorySlot.Item.ItemSO.InterfaceSprite;
        }
        if(_doubleClickTimer == 0)
        {
            _doubleClickTimer = DOUBLE_CLICK_TRESHOLD;
        }
        else
        {
            if (sender is GUIInventorySlot)
            {
                ActivateItem((sender as GUIInventorySlot).InventorySlot);
                (sender as GUIInventorySlot).UpdateSlot();
                SoundManager.UIInstance.PlayGlobalSFX(_itemEquipSound);
            }
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
                SoundManager.UIInstance.PlayGlobalSFX(_itemMoveSound);
            }
        }
        _currentlyDraggedSlot = null;
        _visualItemGhost.gameObject.SetActive(false);
    }
    private void SlotPointerExitAction(object sender, EventArgs e)
    {
        DestroyCurrentTooltip();
        _currentDragTargetSlot = null;
    }
    #endregion
}
