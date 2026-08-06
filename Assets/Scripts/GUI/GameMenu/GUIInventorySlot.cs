using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GUIInventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    #region Variables
    public EventHandler OnPointerEnterEvent, OnPointerExitEvent, OnPointerDownEvent, OnPointerUpEvent;
    public InventorySlot InventorySlot { get; set; }
    [SerializeField] private Image _itemImage, _rarityBar, _lockIcon;
    [SerializeField] private CanvasGroup _backgroundGroup;
    [SerializeField] private TextMeshProUGUI _amountTextBox;
    #endregion

    #region Methods
    public void UpdateSlot()
    {
        if(InventorySlot == null || InventorySlot.IsLocked == true)
        {
            _itemImage.gameObject.SetActive(false);
            _lockIcon.gameObject.SetActive(true);
            _rarityBar.gameObject.SetActive(false);
            _amountTextBox.text = "";
        }
        else if(InventorySlot.Item != null)
        {
            _lockIcon.gameObject.SetActive(false);
            _itemImage.gameObject.SetActive(true);
            _itemImage.sprite = InventorySlot.Item.ItemSO.InterfaceSprite;
            switch( InventorySlot.Item.ItemSO.Rarity)
            { 
                case ItemSO.RarityLevel.Uncommon:
                    _rarityBar.gameObject.SetActive(true);
                    _rarityBar.color = new Color(0.38f, 0.8f, 0.42f);
                    break;
                case ItemSO.RarityLevel.Rare:
                    _rarityBar.gameObject.SetActive(true);
                    _rarityBar.color = new Color(0.52f, 0.69f, 1f);
                    break;
                case ItemSO.RarityLevel.Epic:
                    _rarityBar.gameObject.SetActive(true);
                    _rarityBar.color = new Color(0.99f, 0.92f, 0.09f);
                    break;
                case ItemSO.RarityLevel.Legendary:
                    _rarityBar.gameObject.SetActive(true);
                    _rarityBar.color = new Color(0.96f, 0.56f, 0.29f);
                    break;
                default:
                    _rarityBar.gameObject.SetActive(false);
                    break;
            }
            _backgroundGroup.alpha = 0.5f;
            if (InventorySlot.Item.ItemSO.IsStackable)
                _amountTextBox.text = InventorySlot.Item.Amount.ToString();
            else
                _amountTextBox.text = "";
        }
        else
        {
            _lockIcon.gameObject.SetActive(false);
            _itemImage.sprite = null;
            _itemImage.gameObject.SetActive(false);
           _backgroundGroup.alpha = 0.35f;
            _amountTextBox.text = "";
            _rarityBar.gameObject.SetActive(false);
        }
    }
    #endregion

    #region EventHandlers
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnPointerEnterEvent?.Invoke(this, EventArgs.Empty);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        OnPointerExitEvent?.Invoke(this, EventArgs.Empty);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        OnPointerDownEvent?.Invoke(this, EventArgs.Empty);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        OnPointerUpEvent?.Invoke(this, EventArgs.Empty);
    }
    #endregion
}
