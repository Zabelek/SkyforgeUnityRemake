using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUICharacterBars : MonoBehaviour
{
    #region Variables
    [Header("Character Bar Related Variables")]
    [Tooltip("Reference to the HP bar above thwe character's mesh")]
    [SerializeField] private StatBarBehaviour _hpBar;
    [Tooltip("Refernece to this specific bar's character")]
    [SerializeField] private CharacterBehaviour _character;
    [Tooltip("Reference to the canvas that contains all the bars")]
    [SerializeField] private Canvas _canvas;
    [Tooltip("Image to display when the character is selected automatically by the player")]
    [SerializeField] private Image _autoSelect;
    [Tooltip("Image to display when the manual selection (selection locking) is turned on")]
    [SerializeField] private Image _manualSelect;
    [Tooltip("When the character gets hit, the damage is displayed in this canvas")]
    [SerializeField] private GUIDamageCanvas _damageCanvas;
    [Tooltip("this image is displayed for a short while when the character enters the battle")]
    [SerializeField] private Image _alertImage;
    [Tooltip("This sound is played when the character enters the battle")]
    [SerializeField] private SoundEffectSO _alertSound;
    [Tooltip("Text box that displayes character's distance to the Player")]
    [SerializeField] private TextMeshProUGUI _characterDistanceTextBox;
    public bool IsAutoSelected { get; set; }
    public bool IsManualSelected { get; set; }
    public bool IsInRange { get; set; }
    public bool IsHealthBarVisible { get; set; }
    public int DisplayedDistance { get; set; }
    #endregion

    #region Mono
    private void Awake()
    {
        if(_character.CharacterSO.Category.ShowBackgroundAboveModel)
        {
            _canvas.transform.Find("Icons").Find("Icon_Strength")
                .GetComponent<Image>().sprite = _character.CharacterSO.Category.IconBackground;
            IsAutoSelected = false;
            IsManualSelected = false;
            IsInRange = false;
            IsHealthBarVisible = false;
        }
        else
        {
            _canvas.transform.Find("Icons").Find("Icon_Strength").gameObject.SetActive(false);
        }
        _canvas.transform.Find("Icons").Find("Icon_Type").GetComponent<Image>().sprite = _character.CharacterSO.Category.Icon;
        if(_alertImage != null)
            _alertImage.gameObject.SetActive(false);
        _character.OnCombatStartEvent += ShowAlertIcon;
    }
    private void Update()
    {
        if (!_character.IsDead)
        {
            if (IsInRange)
            {
                if (IsHealthBarVisible)
                    _hpBar.SetValue(_character.Stats.CurrentHP, _character.Stats.MaxHP);
                if (!IsManualSelected && !IsAutoSelected)
                {
                    if (IsHealthBarVisible)
                    {
                        _canvas.transform.Find("HP_Bar").GetComponent<CanvasGroup>().alpha = 0.3f;
                        _canvas.transform.Find("HP_Bar").gameObject.SetActive(true);
                    }
                    else
                    {
                        _canvas.transform.Find("HP_Bar").gameObject.SetActive(false);
                    }
                    _canvas.transform.Find("Icons").GetComponent<CanvasGroup>().alpha = 0.3f;
                    _canvas.transform.Find("Icons").gameObject.SetActive(true);
                }
                else
                {
                    if (IsHealthBarVisible)
                    {
                        _canvas.transform.Find("HP_Bar").GetComponent<CanvasGroup>().alpha = 0.9f;
                        _canvas.transform.Find("HP_Bar").gameObject.SetActive(true);
                    }
                    else
                    {
                        _canvas.transform.Find("HP_Bar").gameObject.SetActive(false);
                    }
                    _canvas.transform.Find("Icons").GetComponent<CanvasGroup>().alpha = 0.9f;
                    _canvas.transform.Find("Icons").gameObject.SetActive(true);
                }
            }
            else
            {
                _canvas.transform.Find("HP_Bar").gameObject.SetActive(false);
                _canvas.transform.Find("Icons").gameObject.SetActive(false);
            }
            float distance = Vector3.Distance(Globals.Instance.ViewportCamera.transform.position, _canvas.transform.position);
            _canvas.transform.localScale = Vector3.one * distance * 0.05f;
            _canvas.transform.forward = Globals.Instance.ViewportCamera.transform.forward;
            if (_autoSelect != null)
            {
                if (IsAutoSelected)
                {
                    _autoSelect.gameObject.SetActive(true);
                }
                else
                {
                    _autoSelect.gameObject.SetActive(false);
                }
                if (IsManualSelected)
                {
                    _manualSelect.gameObject.SetActive(true);
                }
                else
                {
                    _manualSelect.gameObject.SetActive(false);
                }
            }
            if(_characterDistanceTextBox!= null)
            {
                _characterDistanceTextBox.text = DisplayedDistance + "m";
                if (IsAutoSelected || IsManualSelected)
                    _characterDistanceTextBox.gameObject.SetActive(true);
                else
                    _characterDistanceTextBox.gameObject.SetActive(false);
            }
        }
        else
        {
            _canvas.gameObject.SetActive(false);
        }
        _damageCanvas.UpdateDamage(_character.LastDamage);
    }
    #endregion

    #region Methods
    private void ShowAlertIcon(object sender, EventArgs e)
    {
        if(_alertImage!= null)
        {
            _alertImage.gameObject.SetActive(true);
            StartCoroutine(HideAlertIcon(1.5f));
        }
        if(_alertSound!= null)
            Globals.Instance.SoundManager.PlaySFXFast(_alertSound, _character.transform.position);
    }
    private IEnumerator HideAlertIcon(float delay)
    {
        yield return new WaitForSeconds(delay);
        _alertImage.gameObject.SetActive(false);
    }
    #endregion
}
