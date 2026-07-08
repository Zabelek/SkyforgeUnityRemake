using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUICharacterTopBar : MonoBehaviour
{
    #region Variables
    [Header("Top Bar Related Variables")]
    [SerializeField] protected StatBarBehaviour _hpBar;
    [Tooltip("The type Image that displays if the enemy is tank, assassin, support etc")]
    [SerializeField] protected Image _enemyTypeImage;
    [Tooltip("Background of the type image that helps determine strength of the enemy")]
    [SerializeField] protected Image _enemyStrengthImage;
    [Tooltip("This might be quite shocking, but it's a text box that displayes the name of the enemy")]
    [SerializeField] protected TextMeshProUGUI _enemyNameText;
    [Tooltip("Here, the active effects on the enemy are displayed")]
    [SerializeField] protected GUIEffectsGroup _effectsGroup;
    protected CharacterBehaviour _character;
    #endregion

    #region Mono
    protected virtual void Awake()
    {
        _effectsGroup.TopVersion = true;
    }
    protected virtual void Update()
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
                _hpBar.SetValue(_character.Stats.CurrentHP, _character.Stats.MaxHP);
            }
        }
        else
        {
            this.gameObject.SetActive(false);
            _hpBar.Reset();
        }
    }
    #endregion

    #region Methods
    public virtual void SetCharacter(CharacterBehaviour character)
    {
        if(character!=null && character!= _character)
        {
            _hpBar.Reset();
            _character = character;
            this.gameObject.SetActive(true);
            _enemyTypeImage.sprite = character.CharacterSO.Category.Icon;
            if(character.CharacterSO.Category.IconBackground != null)
            {
                _enemyStrengthImage.gameObject.SetActive(true);
                _enemyStrengthImage.sprite = character.CharacterSO.Category.IconBackground;
            }
            else
            {
                _enemyStrengthImage.gameObject.SetActive(false);
            }
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
