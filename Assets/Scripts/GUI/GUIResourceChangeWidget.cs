using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIResourceChangeWidget : MonoBehaviour
{

    #region Variables
    public EventHandler OnDestroyed;
    [SerializeField] private Image _addIndicationImage, _resourceIconImage;
    [SerializeField] private TextMeshProUGUI _amoundTextBox;
    private float _initAnimationTimer, _upAnimationTimer, _fadeOutTimer, _currentLifetime;
    private Vector3 _defaultPos, _initialPos, _upPosePrevious, _upPoseTarget;
    private short _upPosition;
    private CanvasGroup _canvasGroup;
    [SerializeField] private Vector3 _initialAnimationTransform, _goUpAnimationTransform;
    [SerializeField] private bool _fadeInOnInit;
    [SerializeField] private float _targetAlphaValue, _maxLifetime, _animationTimer;
    #endregion

    #region Mono
    protected void Awake()
    {
        _initAnimationTimer = _animationTimer;
        _upPosition = 0;
        _defaultPos = transform.localPosition;
        _initialPos = transform.localPosition + _initialAnimationTransform;
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
            this.transform.localPosition = Vector3.Lerp(_defaultPos, _initialPos, _initAnimationTimer / _animationTimer);
            if(_fadeInOnInit)
            {
                _canvasGroup.alpha = Mathf.Lerp(_targetAlphaValue, 0, _fadeOutTimer / _animationTimer);
            }
            if (_initAnimationTimer <= 0)
            {
                this.transform.localPosition = _defaultPos;
                if (_fadeInOnInit)
                    _canvasGroup.alpha = _targetAlphaValue;
            }
        }
        if(_upAnimationTimer>0)
        {
            _upAnimationTimer -= Time.deltaTime;
            this.transform.localPosition = Vector3.Lerp(_upPoseTarget, _upPosePrevious, _upAnimationTimer / _animationTimer);
            if (_upAnimationTimer <= 0)
                this.transform.localPosition = _upPoseTarget;
        }
        if(_fadeOutTimer > 0)
        {
            _fadeOutTimer -= Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(0, _targetAlphaValue, _fadeOutTimer / _animationTimer);
            if (_fadeOutTimer <= 0)
            {
                Destroy(this.gameObject);
                return;
            }
        }
        if(_fadeOutTimer == 0 && _currentLifetime > _maxLifetime)
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
            _upAnimationTimer = _animationTimer;
            _upPosition++;
            _upPosePrevious = transform.localPosition;
            _upPoseTarget = transform.localPosition + _goUpAnimationTransform;
        }
        else
            FadeOut();
    }
    public void FadeOut()
    {
        if(_fadeOutTimer==0)
            _fadeOutTimer = _animationTimer;
    }
    public void SetValues(Sprite sprite, int value)
    {
        _resourceIconImage.sprite = sprite;
        if(value>0)
        {
            if(_addIndicationImage != null)
                _addIndicationImage.color = Color.greenYellow;
            _amoundTextBox.text = "+" + value;
        }
        else
        {
            if (_addIndicationImage != null)
                _addIndicationImage.color = Color.red;
            _amoundTextBox.text = value.ToString();
        }
    }
    #endregion
}
