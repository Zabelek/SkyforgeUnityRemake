using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GUIItemPickView : MonoBehaviour
{
    #region Variables
    public event EventHandler<InventorySlotEventArgs> ItemSlotSelectedEvent;
    public event EventHandler WindowDestroyEvent;
    public class InventorySlotEventArgs : EventArgs
    {
        public InventorySlot Slot;
        public InventorySlotEventArgs(InventorySlot slot)
        {
            Slot = slot;
        }
    }
    [Tooltip("Where slots will be spawned")]
    [SerializeField] private Transform _slotParent;
    [Tooltip("Prefab base for the slots")]
    [SerializeField] private GUIInventorySlot _slotBase;
    private List<GUIInventorySlot> _slots;
    [Tooltip("Camera displaying UI in the scene")]
    [SerializeField] private Camera _uiCamera;
    [Tooltip("Name of the window")]
    [SerializeField] private TextMeshProUGUI _nameText;
    [Tooltip("Set this to true if the window needs to be destroyed when the mouse is clicked outside")]
    public bool DestroyedByOutsideClick;
    [Tooltip("Set this to true if you want the view to be automatically resized depending on storage size")]
    [SerializeField] private bool _resizedByAmount = true;
    #endregion

    #region Mono
    public void Awake()
    {
        _slots = new();
    }
    private void Update()
    {
        //if the player clicks somewhere outside the window, it sends a signal to be destroyed by the parent, if the variable is set to true
        if (DestroyedByOutsideClick && Input.GetMouseButtonDown(0))
        {
            var lastClickPosition = Input.mousePosition;
            var rectTransform = GetComponent<RectTransform>();
            if (!RectTransformUtility.RectangleContainsScreenPoint(rectTransform, lastClickPosition, _uiCamera))
            {
                WindowDestroyEvent?.Invoke(this, EventArgs.Empty);
            }
        }
    }
    #endregion

    #region Methods
    public List<GUIInventorySlot> AssignSlots(List<InventorySlot> slots)
    {
        ClearSlots();
        foreach(var slot in slots)
        {
            var slotWidget = Instantiate(_slotBase, _slotParent);
            slotWidget.InventorySlot = slot;
            slotWidget.UpdateSlot();
            _slots.Add(slotWidget);

        }
        foreach (var slot in _slots)
        {
            slot.OnPointerUpEvent += SlotPointerUp;
        }
        if(_resizedByAmount)
        {
            if (_slots.Count > 4)
            {
                var rectTransform = GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 270);
            }
            else
            {
                var rectTransform = GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 170);
            }
        }
        return _slots.ToList();
    }
    protected void ClearSlots()
    {
        foreach(var slot in _slots)
        {
            slot.OnPointerUpEvent -= SlotPointerUp;
            Destroy(slot.gameObject);
        }
        _slots.Clear();
    }
    public List<GUIInventorySlot> GetSlots()
    {
        return _slots;
    }
    #endregion

    #region EventHandlers
    private void SlotPointerUp(object sender, EventArgs e)
    {
        if (sender is GUIInventorySlot)
        {
            ItemSlotSelectedEvent?.Invoke(this, new InventorySlotEventArgs((sender as GUIInventorySlot).InventorySlot));
        }
    }
    public void SetTitle(string title)
    {
        _nameText.text = title;
    }
    #endregion
}
