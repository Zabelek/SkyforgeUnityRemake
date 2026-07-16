using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimpleSetTextFromSlider : MonoBehaviour
{
    #region Variables
    [SerializeField] private Slider _slider;
    private TextMeshProUGUI _text;
    public bool IsPercent;
    #endregion

    #region Mono
    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
        if (_slider != null)
        {
            _slider.onValueChanged.AddListener(Slider_ValueChanged);
        }
    }
    #endregion

    #region Methods
    private void Slider_ValueChanged(float arg0)
    {
        if(_text != null)
        {
            if(IsPercent)
                _text.text = arg0.ToString("0.00%");
            else
                _text.text = arg0.ToString();
        }
    }
    #endregion
}
