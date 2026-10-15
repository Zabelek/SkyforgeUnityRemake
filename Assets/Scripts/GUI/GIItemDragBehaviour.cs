using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GIItemDragBehaviour : TooltipDisplayerBehaviour
{
    protected const float DOUBLE_CLICK_TRESHOLD = 0.2f;

    #region Variables
    protected List<GUIInventorySlot> _currentSlots;
    protected GUIInventorySlot _currentlyDraggedSlot, _currentDragTargetSlot;
    [Tooltip("icon displayed when the item is dragged")]
    [SerializeField] protected Image _visualItemGhost;
    //mouse related variables
    private Vector3 _mouseOffsetFromFirstClick;
    private float _doubleClickTimer;
    [Header("Sound")]
    [Tooltip("Sound played when the item changes slot")]
    [SerializeField] protected SoundEffectSO _itemMoveSound;
    [Tooltip("Sound played when the item is equipped")]
    [SerializeField] protected SoundEffectSO _itemEquipSound;
    #endregion

    #region Mono
    protected virtual void Awake()
    {
        _currentSlots = new();
    }
    protected virtual void FixedUpdate()
    {
        if (_doubleClickTimer > 0)
        {
            _doubleClickTimer -= Time.fixedDeltaTime;
            if (_doubleClickTimer < 0)
                _doubleClickTimer = 0;
        }
    }
    protected virtual void Update()
    {
        if (_currentlyDraggedSlot != null)
        {
            PositionItemGhost();
        }
        if (Input.GetMouseButton(0) == false && _currentlyDraggedSlot != null)
        {
            _currentlyDraggedSlot = null;
            _visualItemGhost.gameObject.SetActive(false);
        }
    }
    protected virtual void OnDestroy()
    {
        var toRemoveList = _currentSlots.ToList();
        foreach (var slot in toRemoveList)
        {
            RemoveSlot(slot);
        }
    }
    #endregion

    #region Methods
    protected void AddSlot(GUIInventorySlot slotWidget)
    {
        _currentSlots.Add(slotWidget);
        slotWidget.OnPointerEnterEvent += SlotPointerEnterAction;
        slotWidget.OnPointerExitEvent += SlotPointerExitAction;
        slotWidget.OnPointerUpEvent += SlotPointerUpAction;
        slotWidget.OnPointerDownEvent += SlotPointerDownAction;
    }
    protected void RemoveSlot(GUIInventorySlot slotWidget)
    {
        slotWidget.OnPointerEnterEvent -= SlotPointerEnterAction;
        slotWidget.OnPointerExitEvent -= SlotPointerExitAction;
        slotWidget.OnPointerUpEvent -= SlotPointerUpAction;
        slotWidget.OnPointerDownEvent -= SlotPointerDownAction;
        _currentSlots.Remove(slotWidget);
    }
    protected void PositionItemGhost()
    {
        if ((_mouseOffsetFromFirstClick - Input.mousePosition).magnitude > 0.01f && _visualItemGhost.gameObject.activeSelf == false)
        {
            _visualItemGhost.gameObject.SetActive(true);
        }
        Vector2 mousePos = new Vector2(Input.mousePosition.x / Screen.width * _tooltipCanvas.renderingDisplaySize.x, Input.mousePosition.y / Screen.height * _tooltipCanvas.renderingDisplaySize.y);
        _visualItemGhost.rectTransform.anchoredPosition = new Vector2(mousePos.x, mousePos.y);
    }
    protected virtual void ActivateItem(InventorySlot inventorySlot)
    {

    }
    public void UpdateView()
    {
        foreach (var slot in _currentSlots)
        {
            slot.UpdateSlot();
        }
    }
    #endregion

    #region EventHandlers
    protected virtual void SlotPointerEnterAction(object sender, EventArgs e)
    {
        if (_currentlyDraggedSlot == null)
        {
            if (_currentTooltip == null && sender is GUIInventorySlot && (sender as GUIInventorySlot).InventorySlot.Item != null)
            {
                SetUpNewTooltip((sender as GUIInventorySlot).InventorySlot.Item);
            }
        }
        else if ((sender as GUIInventorySlot).InventorySlot.IsLocked == false)
        {
            _currentDragTargetSlot = sender as GUIInventorySlot;
        }
    }
    protected virtual void SlotPointerDownAction(object sender, EventArgs e)
    {
        DestroyCurrentTooltip();
        if (sender is GUIInventorySlot && (sender as GUIInventorySlot).InventorySlot.Item != null)
        {
            _currentlyDraggedSlot = sender as GUIInventorySlot;
            _mouseOffsetFromFirstClick = Input.mousePosition;
            _visualItemGhost.sprite = (sender as GUIInventorySlot).InventorySlot.Item.ItemSO.InterfaceSprite;
        }
        if (_doubleClickTimer == 0)
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
    protected virtual void SlotPointerUpAction(object sender, EventArgs e)
    {
        if (_currentlyDraggedSlot != null && _currentDragTargetSlot != null)
        {
            if (_currentlyDraggedSlot != _currentDragTargetSlot)
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
    protected virtual void SlotPointerExitAction(object sender, EventArgs e)
    {
        DestroyCurrentTooltip();
        _currentDragTargetSlot = null;
    }
    #endregion
}
