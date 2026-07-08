using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIDebugWindowPerkCheckBox : MonoBehaviour
{
    #region Variables
    [SerializeField] private Button _checkboxButton;
    [SerializeField] private Sprite _checkedSprite;
    [SerializeField] private Sprite _uncheckedSprite;
    [SerializeField] private TextMeshProUGUI _nameText;
    private LockablePerk _perk;
    private PlayerBehaviour _player;
    #endregion

    #region Mono
    private void Start()
    {
        _checkboxButton.onClick.AddListener(ButtonClicked);
        _checkboxButton.image.raycastTarget = true;
    }
    #endregion

    #region Methods
    public void SetPerk(PerkSO perkSO)
    {
        if(perkSO!= null && _player != null)
        {
            if (_perk != null)
            {
                _perk.OnDisabled -= UpdateSprite;
                _perk.OnEnabled -= UpdateSprite;
                _perk = null;
            }
            _perk = _player.GetPerk(perkSO);
            _perk.OnDisabled += UpdateSprite;
            _perk.OnEnabled += UpdateSprite;
            _nameText.text = _perk.Perk.Name;
            UpdateSprite(this, EventArgs.Empty);
        }
    }
    public void SetPlayer(PlayerBehaviour player)
    {
        _player = player;
        UpdateSprite(this, EventArgs.Empty);
    }
    public void UpdateSprite(object sender, EventArgs e)
    {
        if (_perk != null && _player != null)
        {
            if (_perk.Unlocked == true && _perk.Enabled == true)
            {
                _checkboxButton.image.sprite = _checkedSprite;
            }
            else
            {
                _checkboxButton.image.sprite = _uncheckedSprite;
            }
        }
    }
    private void ButtonClicked()
    {
        if(_perk != null)
        {
            if (_perk.Unlocked)
            {
                if (_perk.Enabled)
                    _player.DisablePerk(_perk);
                else
                    _player.EnablePerk(_perk);
            }
            else
                _player.UnlockPerk(_perk);
        }
    }
    public void Check()
    {
        if (_perk.Unlocked)
            _player.EnablePerk(_perk);
        else
            _player.UnlockPerk(_perk);
    }
    public void Uncheck()
    {
        if (_perk.Unlocked)
            _player.DisablePerk(_perk);
    }
    #endregion
}