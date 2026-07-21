using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatBarBehaviour : MonoBehaviour
{
    #region Variables
    [Header("Stat Bar Related Variables")]
    [Tooltip("Bar that displays current state of the stat. For Diff Bar to work, both Diff Bar and Fill Bar have to have all Anchors set to zero!")]
    [SerializeField] private Image _fillBar;
    [Tooltip("bar displaying the difference between recent stat state and it's curent state. Fades out with time. For Diff Bar to work, both Diff Bar and Fill Bar have to have all Anchors set to zero!")]
    [SerializeField] private Image _diffBar;
    [Tooltip("Text box with numeric anount of stat")]
    [SerializeField] private TextMeshProUGUI _amountText;
    [Tooltip("Text box with percantage amount of stat")]
    [SerializeField] private TextMeshProUGUI _percentText;
    [Tooltip("Time in which the Diff Bar will fade out")]
    [SerializeField] private float _fadeoutTimerMax = 1;
    private float _fadeoutTimer;
    private float _speedPercent;
    //To prevent difference animation play at the start of the game
    private bool _firstTime = true;
    private int _previous = 0;
    private int _previousMax = 0;
    #endregion

    #region Mono
    private void Update()
    {
        //difference bar animations
        if(_fadeoutTimer>0)
        {
            if(_diffBar.TryGetComponent<CanvasGroup>(out var canvas))
            {
                canvas.alpha = _fadeoutTimer / _fadeoutTimerMax;
            }
            _fadeoutTimer -= Time.deltaTime * _speedPercent;
            if(_fadeoutTimer <0)
            {
                _fadeoutTimer = 0;
                canvas.alpha = 0;
            }
        }
    }
    public void Reset()
    {
        _firstTime = true;
        ResetDiffAlpha();
    }
    #endregion

    #region Methods
    public void ResetDiffAlpha()
    {
        _fadeoutTimer = 0;
        if (_diffBar.TryGetComponent<CanvasGroup>(out var canvas))
        {
            canvas.alpha = 0;
        }
    }
    public void SetValue(int current, int max)
    {
        if (_previous != current || _previousMax != max)
        {
            _previousMax = max;
            float percentCurrent = (float)current / (float)max;
            float percentPrevious = (float)_previous / (float)max;
            _fillBar.fillAmount = percentCurrent;
            if (_diffBar != null && _firstTime == false)
            {
                float diffPos = 0;
                float diffWidth = 0;
                float width = _fillBar.rectTransform.sizeDelta.x;
                if (_previous > current)
                {
                    diffPos = percentCurrent * width;
                    diffWidth = ((float)(_previous - current) / (float)max) * width;
                }
                else
                {
                    diffPos = percentPrevious * width;
                    diffWidth = ((float)(current - _previous) / (float)max) * width;
                }
                _diffBar.rectTransform.anchoredPosition = new Vector3(_fillBar.rectTransform.anchoredPosition.x + diffPos, _diffBar.rectTransform.anchoredPosition.y);
                _diffBar.rectTransform.sizeDelta = new Vector2(diffWidth, _diffBar.rectTransform.sizeDelta.y);
                _fadeoutTimer = _fadeoutTimerMax;
                //the smaller difference, the faster animation
                _speedPercent = 1 - Mathf.Abs(percentCurrent - percentPrevious);
            }
            //at the start of the game, the difference animation shouldn't play
            else if (_firstTime == true)
                _firstTime = false;
            if (_amountText != null)
                _amountText.text = current + " / " + max;
            if (_percentText != null)
                _percentText.text = ((float)current / (float)max).ToString("0%");
            _previous = current;
        }
    }
    public void SetColor(Color color)
    {
        _fillBar.color = color;
    }
    public Color GetColor()
    {
        return _fillBar.color;
    }
    public void SetValueDisplayOnly(int currentHP, int maxHP)
    {
        if (_amountText != null)
            _amountText.text = currentHP + " / " + maxHP;
        if (_percentText != null)
            _percentText.text = ((float)currentHP / (float)maxHP).ToString("0%");
    }
    #endregion
}
