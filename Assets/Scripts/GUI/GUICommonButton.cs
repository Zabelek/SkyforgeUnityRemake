using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class GUICommonButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    #region Variables
    public EventHandler OnClick;
    [Header("Common Button Variables")]
    [SerializeField] protected TextMeshProUGUI _textBox;
    [SerializeField] protected CanvasGroup _normalBackground, _greyBackground, _highlight;
    private bool _isMouseOver, _isLocked;
    public bool LockedAtStart;
    #endregion

    #region Mono
    protected void Awake()
    {
        if (LockedAtStart)
            SetLocked(true);
        else
            SetLocked(false);
        _isMouseOver = false;
    }
    protected void OnEnable()
    {
        OnPointerExit(null);
    }
    #endregion

    #region Mathods
    public void SetLocked(bool locked)
    {
        if(locked)
        {
            _normalBackground.gameObject.SetActive(false);
            _greyBackground.gameObject.SetActive(true);
        }
        else
        {
            _normalBackground.gameObject.SetActive(true);
            _greyBackground.gameObject.SetActive(false);
        }
        _isLocked = locked;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!_isLocked)
        {
            _highlight.alpha = 0.06f;
            _isMouseOver = true;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_isLocked)
        {
            _highlight.alpha = 0;
            _isMouseOver = false;
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_isLocked)
        {
            _highlight.alpha = 0.12f;
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_isLocked)
        {
            if (_isMouseOver)
            {
                _highlight.alpha = 0.06f;
                OnClick?.Invoke(this, EventArgs.Empty);
            }
            else
                _highlight.alpha = 0f;
        }
    }
    #endregion
}
