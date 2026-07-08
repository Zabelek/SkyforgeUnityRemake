using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIAbilityWidget : MonoBehaviour
{
    #region Variables
    [Header("Ability Widget Related Variables")]
    [Tooltip("The image of ability icon")]
    [SerializeField] private Image _backgroundImage;
    [Tooltip("The image used to vizualize ability's cooldown")]
    [SerializeField] private Image _foregroundImage;
    [Tooltip("Used to display upgraded ability version cooldown")]
    [SerializeField] private Image _topBarBackgroundImage, _topBarFillImage;
    [Tooltip("Text box with cooldown in seconds")]
    [SerializeField] private TextMeshProUGUI _timeLeftText;
    [Tooltip("Text box displaying keyboard key for the aboility")]
    [SerializeField] private TextMeshProUGUI _keyText;
    [Tooltip("Ability reference")]
    [SerializeField] private LockableAbility _ability;
    #endregion

    #region Mono
    private void Update()
    {
        if (_ability.Ability.CurrentCooldown < _ability.Ability.MaxCooldown)
        {
            if (!_foregroundImage.gameObject.activeSelf)
            {
                _foregroundImage.gameObject.SetActive(true);
            }
            _foregroundImage.fillAmount = 1 - (_ability.Ability.CurrentCooldown / _ability.Ability.MaxCooldown);
            float timeRemaining = -(_ability.Ability.MaxCooldown - _ability.Ability.CurrentCooldown);
            if (_ability.Ability.MaxCooldown >= 3 && !_timeLeftText.gameObject.activeSelf)
            {
                _timeLeftText.gameObject.SetActive(true);
            }
            _timeLeftText.text = PrepareTimeString(timeRemaining);
        }
        else
        {
            if (_foregroundImage.gameObject.activeSelf)
            {
                _foregroundImage.gameObject.SetActive(false);
            }
            if (_timeLeftText.gameObject.activeSelf)
            {
                _timeLeftText.gameObject.SetActive(false);
            }

        }
        if (_ability.Ability.UpgradedVersionCooldownMax > 0)
        {
            if (_ability.Ability.UpgradedVersionCooldown < _ability.Ability.UpgradedVersionCooldownMax)
            {
                _topBarBackgroundImage.gameObject.SetActive(true);
                _topBarFillImage.gameObject.SetActive(true);
                _topBarFillImage.fillAmount = _ability.Ability.UpgradedVersionCooldown / _ability.Ability.UpgradedVersionCooldownMax;
                if (_backgroundImage.sprite != _ability.Ability.AbilitySO.Icon)
                    _backgroundImage.sprite = _ability.Ability.AbilitySO.Icon;
            }
            else
            {
                _topBarBackgroundImage.gameObject.SetActive(false);
                _topBarFillImage.gameObject.SetActive(false);
                if (_backgroundImage.sprite != _ability.Ability.AbilitySO.UpgradedIcon)
                    _backgroundImage.sprite = _ability.Ability.AbilitySO.UpgradedIcon;
            }
        }
        else
        {
            if (_backgroundImage.sprite != _ability.Ability.AbilitySO.Icon)
                _backgroundImage.sprite = _ability.Ability.AbilitySO.Icon;
            _topBarBackgroundImage.gameObject.SetActive(false);
            _topBarFillImage.gameObject.SetActive(false);
        }
    }
    #endregion

    #region Methods
    private string PrepareTimeString(float timeRemaining)
    {
        return ((int)timeRemaining / 60).ToString() + ":" + ((int)timeRemaining % 60).ToString();
    }
    public void AttachAbility(LockableAbility ability, string key)
    {
        _ability = ability;
        _backgroundImage.sprite = _ability.Ability.AbilitySO.Icon;
        _foregroundImage.fillAmount = 0;
        _topBarBackgroundImage.gameObject.SetActive(false);
        _topBarFillImage.gameObject.SetActive(false);
        _timeLeftText.gameObject.SetActive(false);
        _keyText.text = key;
    }
    public void CheckAvailability()
    {
        if (_ability != null && _ability.Unlocked)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    #endregion
}
