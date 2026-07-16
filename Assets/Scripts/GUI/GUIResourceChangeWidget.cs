using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIResourceChangeWidget : MonoBehaviour
{
    public const float ANIMATION_TIMER = 0.3f;
    public const float MAX_LIFETIME = 5;

    #region Variables
    public EventHandler OnDestroyed;
    [SerializeField] private Image _addIndicationImage, _resourceIconImage;
    [SerializeField] private TextMeshProUGUI _amoundTextBox;
    private float _initAnimationTimer, _upAnimationTimer, _fadeOutTimer, _currentLifetime;
    private Vector3 _defaultPos, _initialPos, _upPosePrevious, _upPoseTarget;
    private short _upPosition;
    private CanvasGroup _canvasGroup;
    #endregion

    #region Mono
    protected void Awake()
    {
        _initAnimationTimer = ANIMATION_TIMER;
        _upPosition = 0;
        _defaultPos = transform.localPosition;
        _initialPos = transform.localPosition + new Vector3(130,0,0);
        transform.localPosition = _initialPos;
        _canvasGroup = GetComponent<CanvasGroup>();
        _currentLifetime = 0;
    }
    protected void Update()
    {
        _currentLifetime += Time.deltaTime;
        if (_initAnimationTimer>0)
        {
            _initAnimationTimer -= Time.deltaTime;
            this.transform.localPosition = Vector3.Lerp(_defaultPos, _initialPos, _initAnimationTimer / ANIMATION_TIMER);
            if (_initAnimationTimer <= 0)
                this.transform.localPosition = _defaultPos;
        }
        if(_upAnimationTimer>0)
        {
            _upAnimationTimer -= Time.deltaTime;
            this.transform.localPosition = Vector3.Lerp(_upPoseTarget, _upPosePrevious, _upAnimationTimer / ANIMATION_TIMER);
            if (_upAnimationTimer <= 0)
                this.transform.localPosition = _upPoseTarget;
        }
        if(_fadeOutTimer > 0)
        {
            _fadeOutTimer -= Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(0, 0.8f, _fadeOutTimer / ANIMATION_TIMER);
            if (_fadeOutTimer <= 0)
            {
                Destroy(this.gameObject);
                return;
            }
        }
        if(_fadeOutTimer == 0 && _currentLifetime>MAX_LIFETIME)
        {
            FadeOut();
        }
    }
    private void OnDestroy()
    {
        OnDestroyed?.Invoke(this, EventArgs.Empty);
    }
    #endregion

    #region Methods
    public void GoUp()
    {
        if (_upPosition < 3)
        {
            _upAnimationTimer = ANIMATION_TIMER;
            _upPosition++;
            _upPosePrevious = transform.localPosition;
            _upPoseTarget = transform.localPosition + new Vector3(0, 52, 0);
        }
        else
            FadeOut();
    }
    public void FadeOut()
    {
        if(_fadeOutTimer==0)
            _fadeOutTimer = ANIMATION_TIMER;
    }
    public void SetValues(Sprite sprite, int value)
    {
        _resourceIconImage.sprite = sprite;
        if(value>0)
        {
            _addIndicationImage.color = Color.greenYellow;
            _amoundTextBox.text = "+" + value;
        }
        else
        {
            _addIndicationImage.color = Color.red;
            _amoundTextBox.text = value.ToString();
        }
    }
    #endregion
}
