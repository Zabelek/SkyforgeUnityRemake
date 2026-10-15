using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimpleSetTextFromSlider : MonoBehaviour
{
    #region Variables
    [SerializeField] private Slider _slider;
    private TextMeshProUGUI _text;
    public bool IsPercent;
    [HideInInspector] public bool IsFullNumbersOnly = false;
    #endregion

    #region Mono
    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
        if (_slider != null)
        {
            _slider.onValueChanged.AddListener(Slider_ValueChanged);
            IsFullNumbersOnly = _slider.wholeNumbers;
            SetValue(_slider.value);
        }
    }
    #endregion

    #region Methods
    private void Slider_ValueChanged(float arg0)
    {
        SetValue(arg0);
    }
    private void SetValue(float value)
    {
        if (_text != null)
        {
            if (IsPercent)
                _text.text = value.ToString("0.00%");
            else if (IsFullNumbersOnly)
                _text.text = value.ToString("0");
            else
                _text.text = value.ToString("0.00");
        }
    }
    #endregion
}
