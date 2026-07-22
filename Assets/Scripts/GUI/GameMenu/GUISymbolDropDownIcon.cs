using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GUISymbolDropDownIcon : MonoBehaviour
{
    #region Variables
    public PerkSetSO PerkSet;
    [SerializeField] private Button _buttonReference;
    [SerializeField] private Button _openButton;
    [SerializeField] private Image _displayCurrentSymbolImage;
    private List<Button> _symbolButtons;
    [SerializeField] private CanvasGroup _dropDown;
    [SerializeField] private Sprite _lockedSprite;
    [SerializeField] private Camera _uiCamera;
    #endregion

    #region Mono
    private void Awake()
    {
        _symbolButtons = new();
        UpdatePerkList();
        _openButton.onClick.AddListener(() => { if (_symbolButtons.Count > 0) _dropDown.gameObject.SetActive(true); });
        //To Do: add onPointerEnter, adding tooltip displayd like in the atlas
    }
    private void Update()
    {
        //if the player clicks somewhere outside the drop down, it hides
        if (Input.GetMouseButtonDown(0))
        {
            var lastClickPosition = Input.mousePosition;
            var rectTransform = _dropDown.GetComponent<RectTransform>();
            if (!RectTransformUtility.RectangleContainsScreenPoint(rectTransform, lastClickPosition, _uiCamera))
            {
                _dropDown.gameObject.SetActive(false);
            }
        }
    }
    #endregion

    #region Methods
    public void UpdatePerkList()
    {
        //first clear all existing buttons
        var buttonsToRemove = _symbolButtons.ToList();
        foreach (var button in buttonsToRemove)
        {
            button.onClick.RemoveAllListeners();
            _symbolButtons.Remove(button);
            Destroy(button.gameObject);
        }
        // then add a button for every perk in the assigned perk that the player has unlocked
        if(SkyforgeLoader.CurrentProfile != null)
        {
            foreach(var perkSO in PerkSet.Perks)
            {
                if (SkyforgeLoader.CurrentProfile.AcquiredPerks.Any(p => p.PerkID == perkSO.ID && p.Enabled))
                    _displayCurrentSymbolImage.sprite = perkSO.SymbolIcon;
                if (SkyforgeLoader.CurrentProfile.AcquiredPerks.Any(p => p.PerkID == perkSO.ID))
                {
                    var button = Instantiate(_buttonReference, _dropDown.transform);
                    button.gameObject.SetActive(true);
                    button.GetComponent<Image>().sprite = perkSO.SymbolIcon;
                    button.onClick.AddListener(() =>
                    {
                        SkyforgeLoader.EnablePerk(perkSO);
                        _displayCurrentSymbolImage.sprite = perkSO.SymbolIcon;
                        _dropDown.gameObject.SetActive(false);
                    });
                    _symbolButtons.Add(button);
                }
            }
        }
        //if the player has no perks from this current perk set, it displays a lock icon.
        if(_symbolButtons.Count == 0)
        {
            _displayCurrentSymbolImage.sprite = _lockedSprite;
        }
        _dropDown.gameObject.SetActive(false);
    }
    #endregion
}
