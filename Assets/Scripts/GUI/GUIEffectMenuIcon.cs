using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIEffectMenuIcon : MonoBehaviour
{
    #region Variables
    [Tooltip("Effect's icon will be placed here")]
    [SerializeField] private Image _iconImage;
    [Tooltip("Time until the effect expires")]
    [SerializeField] private TextMeshProUGUI _timeText;
    [Tooltip("if the effect has stacks, they will be displayed here")]
    [SerializeField] private TextMeshProUGUI _stackText;
    private GameplayEffectBehaviour _effect;
    private GUIEffectsGroup _group;
    public bool TopVersion { get; set; }
    #endregion

    #region Mono
    private void Awake()
    {
    }
    private void Update()
    {
        UpdateIcons();
    }
    #endregion

    #region Methods
    private void UpdateIcons()
    {
        if (_effect != null)
        {
            int minutes = (int)(_effect.TimeLeft / 60);
            int seconds = (int)(_effect.TimeLeft % 60);
            if (seconds >= 10)
            {
                _timeText.text = minutes + ":" + seconds;
            }
            else
            {
                _timeText.text = minutes + ":0" + seconds;
            }
            if (_effect.TimeLeft <= -100)
            {
                _timeText.gameObject.SetActive(false);
            }
            else
            {
                _timeText.gameObject.SetActive(true);
            }
            if (_effect.TimeLeft <= 0 && _effect.TimeLeft > -100)
            {
                _group.Remove(this);
            }
            if (_effect.Stacks > 1)
            {
                if (!_stackText.gameObject.activeSelf)
                    _stackText.gameObject.SetActive(true);
                _stackText.text = _effect.Stacks.ToString();
            }
            else if (_stackText.gameObject.activeSelf)
                _stackText.gameObject.SetActive(false);
        }
        else
        {
            _group.Remove(this);
        }
    }
    public void Init(GameplayEffectBehaviour effect, GUIEffectsGroup group)
    {
        _effect = effect;
        _iconImage.sprite = effect.EffectSO.Image;
        _group = group;
        UpdateIcons();
    }
    public GameplayEffectBehaviour GetEffect()
    {
        return _effect;
    }
    #endregion
}