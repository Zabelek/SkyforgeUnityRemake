using TMPro;
using UnityEngine;

public class GuiBossTopBarBehaviour : GUICharacterTopBar
{
    #region Variables
    [Header("Boss Bar Related Variables")]
    [Tooltip("The bar hidden behind the main one, on the top")]
    [SerializeField] protected StatBarBehaviour _secondHpBar;
    [Tooltip("The text box displaying the amount of remaining hp bars of the boss")]
    [SerializeField] protected TextMeshProUGUI _barsAmountText;
    private int _singleBarHpAmount;
    private Color _originalHpBarColor1, _originalHpBarColor2;
    private short _oldBarsAmount;
    #endregion

    #region Mono
    protected override void Awake()
    {
        _effectsGroup.TopVersion = true;
        _originalHpBarColor1 = _hpBar.GetColor();
        _originalHpBarColor2 = _secondHpBar.GetColor();
        _oldBarsAmount = 0;
    }
    protected override void Update()
    {
        if (_character != null)
        {
            if (_character.IsDead)
            {
                _character = null;
                this.gameObject.SetActive(false);
                _hpBar.Reset();
            }
            else
            {
                //cheching current bar's hp
                var current = _character.Stats.CurrentHP % _singleBarHpAmount;
                _hpBar.SetValue(current, _singleBarHpAmount);
                //checking remaining bars amount
                var barsAmount = (short)((_character.Stats.CurrentHP / _singleBarHpAmount) + 1);
                if (current == 0)
                    barsAmount--;
                if (_oldBarsAmount!= barsAmount)
                {
                    _oldBarsAmount = barsAmount;
                    _hpBar.SetValue(current + 1, _singleBarHpAmount);
                    _hpBar.SetValue(current, _singleBarHpAmount);
                }
                if (barsAmount == 1)
                {
                    _barsAmountText.gameObject.SetActive(false);
                    _secondHpBar.gameObject.SetActive(false);
                    _hpBar.SetColor(_originalHpBarColor2);
                }
                else
                {
                    _barsAmountText.gameObject.SetActive(true);
                    _secondHpBar.gameObject.SetActive(true);
                    _hpBar.SetColor(_originalHpBarColor1);
                    _barsAmountText.text = "x" + barsAmount.ToString();
                }
                if (current==0 && _character.Stats.CurrentHP>0)
                {
                    _hpBar.SetValue(_singleBarHpAmount, _singleBarHpAmount);
                    _hpBar.ResetDiffAlpha();
                }
                _hpBar.SetValueDisplayOnly(_character.Stats.CurrentHP, _character.Stats.MaxHP);                 
            }
        }
        else
        {
            this.gameObject.SetActive(false);
            _hpBar.Reset();
            _secondHpBar.Reset();
        }
    }
    #endregion

    #region Methods
    public override void SetCharacter(CharacterBehaviour character)
    {
        if (character != null && character != _character)
        {
            _singleBarHpAmount = (int)(character.Stats.MaxHP / character.CharacterSO.Category.HealthBarsAmount);
            _hpBar.Reset();
            _secondHpBar.Reset();
            _character = character;
            this.gameObject.SetActive(true);
            _enemyTypeImage.sprite = character.CharacterSO.Category.Icon;
            _enemyStrengthImage.sprite = character.CharacterSO.Category.IconBackground;
            _enemyTypeImage.color = Color.red;
            if (character.Name.Length > 0)
            {
                _enemyNameText.text = character.Name;
            }
            else
            {
                _enemyNameText.text = character.CharacterSO.Name;
            }
            _effectsGroup.SetCharacter(character);
        }
    }
    #endregion
}
