using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class GUIGameMenuTopSwitchButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    #region Variables
    public EventHandler OnClick;
    [SerializeField] private Transform _selectionImages;
    [SerializeField] private CanvasGroup _textCanvasGroup;
    public bool IsToggled, LockedAtStart;
    private bool _isMouseOver, _isLocked;
    #endregion

    #region Mono
    private void Awake()
    {
        IsToggled = false;
        if (LockedAtStart)
            SetLocked(true);
        SetToggled(false);
    }
    protected void OnEnable()
    {
        OnPointerExit(null);
    }
    #endregion

    #region Methods
    public void SetLocked(bool locked)
    {
        _isLocked = true;
        _textCanvasGroup.alpha = 0.4f;
    }
    public void SetToggled(bool toggled)
    {
        IsToggled = toggled;
        _selectionImages.gameObject.SetActive(toggled);
    }
    #endregion

    #region EventHandlers
    public void OnPointerDown(PointerEventData eventData)
    {

    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_isLocked)
        {
            _textCanvasGroup.alpha = 1;
            _isMouseOver = true;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_isLocked)
        {
            _textCanvasGroup.alpha = 0.7f;
            _isMouseOver = false;
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_isLocked && !IsToggled)
        {
            if (_isMouseOver)
            {
                OnClick?.Invoke(this, EventArgs.Empty);
                SetToggled(true);

            }
            else
                _textCanvasGroup.alpha = 0.7f;
        }
    }
    #endregion
}
