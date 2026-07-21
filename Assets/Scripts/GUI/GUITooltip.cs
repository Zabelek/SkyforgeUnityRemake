using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GUITooltip : MonoBehaviour
{
    public const float FADE_IN_TIME = 0.5f;
    public const float APPEAR_DELAY = 0.1f;

    #region Variables
    [SerializeField] protected Image _titleImage;
    [SerializeField] protected TextMeshProUGUI _titleText, _descriptionText;
    [SerializeField] Transform _costPanel, _costLayout, _costOpsitionTemplate;
    private float _fadeInTimer, _waitUntilAppearTimer;
    private CanvasGroup _group;

    [SerializeField] private RectTransform _tooltip;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Vector2 _offset = new Vector2(10f, 10f);
    private RectTransform _canvasRect;
    #endregion

    #region Mono
    private void Awake()
    {
        if(_costPanel!= null)
        {
            _costPanel.gameObject.SetActive(false);
        }
        _fadeInTimer = 0;
        _waitUntilAppearTimer = APPEAR_DELAY;
        _group = GetComponent<CanvasGroup>();
        if (_group != null)
            _group.alpha = 0;
    }
    private void Update()
    {
        //Vector2 mouse = Mouse.current.position.ReadValue();
        //this.transform.position = mouse;
        if (_waitUntilAppearTimer>0)
        {
            _waitUntilAppearTimer -= Time.deltaTime;
            if(_waitUntilAppearTimer<=0)
            {
                _fadeInTimer = FADE_IN_TIME;
            }
        }
        if(_fadeInTimer > 0 && _group != null)
        {
            _fadeInTimer -= Time.deltaTime;
            _group.alpha = Mathf.Lerp(1, 0, _fadeInTimer / FADE_IN_TIME);
        }
    }
    private void LateUpdate()
    {
        PositionTooltip();
    }

    
    #endregion

    #region Methods
    public void SetTitle(string title)
    {
        _titleText.text = title;
        //LayoutRebuilder.ForceRebuildLayoutImmediate(this.transform as RectTransform);
        //Canvas.ForceUpdateCanvases();
    }
    public void SetDescription(string description)
    {
        _descriptionText.text = description;
        GetComponent<VerticalLayoutGroup>().reverseArrangement = true;
        StartCoroutine(RebuildLayout());
    }
    private IEnumerator RebuildLayout()
    {
        //For reasons unknown to me, this must be done to rebuild the layout, as it goes bonkers every time a text ius set.It works in the editor, but doesn't work at runtime. No idea why.
        //This function may change when a better solution is discovered.
        yield return new WaitForSeconds(0.05f);
        GetComponent<VerticalLayoutGroup>().reverseArrangement = false;
    }
    public void SetTitleImage(Sprite image)
    {
        _titleImage.sprite = image;
    }
    public void SetCanvas(Canvas canvas)
    {
        _canvas = canvas;
        _canvasRect = _canvas.GetComponent<RectTransform>();
    }
    public void AddCost(Sprite costIcon, int costAmount)
    {
        _costPanel.gameObject.SetActive(true);
        var newPosition = Instantiate(_costOpsitionTemplate, _costLayout);
        newPosition.gameObject.SetActive(true);
        var costText = newPosition.GetComponentInChildren<TextMeshProUGUI>();
        if (costText != null)
        {
            costText.text = costAmount.ToString();
        }
        var costImage = newPosition.GetComponentInChildren<Image>();
        if(costImage != null)
        {
            costImage.sprite = costIcon;
        }
    }
    private void PositionTooltip()
    {
        Vector2 mousePos = new Vector2(Input.mousePosition.x/Screen.width*_canvas.renderingDisplaySize.x, Input.mousePosition.y/Screen.height*_canvas.renderingDisplaySize.y);
        Vector2 size = _tooltip.rect.size;
        float canvasWidth = _canvas.pixelRect.width;
        float canvasHeight = _canvas.pixelRect.height;

        Vector2 defaultPosition = mousePos;

        float topOffset = 0;
        if (size.y + defaultPosition.y > _canvas.renderingDisplaySize.y)
            topOffset -= size.y;
        float leftOffset = 0;
        if (defaultPosition.x - size.x < 0)
            leftOffset += size.x;
        _tooltip.anchoredPosition = new Vector2(defaultPosition.x + leftOffset, defaultPosition.y + topOffset);
    }
    #endregion

    #region EventHandlers
    #endregion
}
