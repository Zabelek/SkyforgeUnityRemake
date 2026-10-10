using UnityEngine;
using UnityEngine.UI;

public class GUIMenuRebirthSpecialEffectBehaviour : MonoBehaviour
{
    #region Variables
    private float _animationTimer;
    public float _firstPhase, _secondPhase, _thirdPhase, _fourthPhase, _firstLightInitialScale, _firstLightScaleFactor, _firstLightScaleFactorSecondPhase;
    [SerializeField] private Image _firstLight, _sparkle, _afterglow;
    #endregion

    #region Mono
    private void Awake()
    {
        _animationTimer = 0;
        _sparkle.color = new Color(1, 1, 1, 0);
        var scaleFactor = _firstLightInitialScale + (_animationTimer / _firstPhase) * _firstLightScaleFactor;
        _firstLight.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
    }
    private void Update()
    {
        _animationTimer += Time.deltaTime;
        if (_animationTimer < _firstPhase)
        {
            var scaleFactor = _firstLightInitialScale + (_animationTimer / _firstPhase) * _firstLightScaleFactor;
            _firstLight.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            var alpha = _animationTimer  / _firstPhase;
            _firstLight.color = new Color(1, 1, 1, alpha);
        }
        else if (_animationTimer - _firstPhase < _secondPhase)
        { 
            var scaleFactor = _firstLightInitialScale + ((_animationTimer / _firstPhase) * _firstLightScaleFactor) + (((_animationTimer - _firstPhase) / _firstPhase) * _firstLightScaleFactorSecondPhase);
            _firstLight.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            var spalpha = (_animationTimer - _firstPhase) / _secondPhase;
            _sparkle.color = new Color(1, 1, 1, spalpha);
        }
        else if (_animationTimer - _firstPhase - _secondPhase < _thirdPhase)
        {
            var scaleFactor = _firstLightInitialScale + ((_animationTimer / _firstPhase) * _firstLightScaleFactor) + (((_animationTimer - _firstPhase) / _firstPhase) * _firstLightScaleFactorSecondPhase);
            _firstLight.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            var alpha = 1 - ((_animationTimer - _firstPhase - _secondPhase) / _thirdPhase);
            _firstLight.color = new Color(1, 1, 1, alpha);
        }
        if (_animationTimer - _firstPhase - _secondPhase < _fourthPhase && _animationTimer > _firstPhase + _secondPhase)
        {
            var spalpha = 1 - ((_animationTimer - _firstPhase - _secondPhase) / _fourthPhase);
            _sparkle.color = new Color(1, 1, 1, spalpha);
        }
        if (_animationTimer > _firstPhase + _secondPhase + _thirdPhase)
        {
            _firstLight.color = new Color(1, 1, 1, 0);
        }
        if (_animationTimer > _firstPhase + _secondPhase + _thirdPhase + _fourthPhase)
        {
            Destroy(this.gameObject);
        }
    }
    #endregion

    #region Methods
    #endregion
}
