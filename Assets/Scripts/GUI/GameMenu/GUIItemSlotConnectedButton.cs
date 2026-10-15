using System;
using UnityEngine;
using UnityEngine.EventSystems;

//If the item slot for some reason can't be accessed from the gui canvas (for example is part of in-world interface) this class, put in the actual gui canvas, can help access it anyway
public class GUIItemSlotConnectedButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    #region Variables
    public EventHandler OnPointerEnterEvent, OnPointerExitEvent, OnPointerDownEvent, OnPointerUpEvent;
    public GUIInventorySlot InventorySlotWidget;
    #endregion

    #region EventHandlers
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnterEvent?.Invoke(InventorySlotWidget, EventArgs.Empty);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        OnPointerExitEvent?.Invoke(InventorySlotWidget, EventArgs.Empty);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        OnPointerDownEvent?.Invoke(InventorySlotWidget, EventArgs.Empty);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        OnPointerUpEvent?.Invoke(InventorySlotWidget, EventArgs.Empty);
    }
    #endregion
}
