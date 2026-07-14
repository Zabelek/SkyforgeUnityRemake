using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UserProfile;

public class GUIDebugWindowPerkCheckBox : MonoBehaviour
{
    #region Variables
    [SerializeField] private Button _checkboxButton;
    [SerializeField] private Sprite _checkedSprite;
    [SerializeField] private Sprite _uncheckedSprite;
    [SerializeField] private TextMeshProUGUI _nameText;
    private UserProfile.PerkState _perkState;
    private PerkSO _perkSO;
    public EventHandler OnChange;
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
        _perkSO = perkSO;
        if (perkSO!= null && SkyforgeLoader.CurrentProfile != null)
        {
            _perkState = SkyforgeLoader.CurrentProfile.AcquiredPerks.FirstOrDefault(p => p.PerkID == perkSO.ID);
            UpdateSprite(this, EventArgs.Empty);
            _nameText.text = perkSO.Name;
        }
    }
    public void UpdateSprite(object sender, EventArgs e)
    {
        if (_perkSO != null && _perkState!= null && _perkState.Enabled)
            _checkboxButton.image.sprite = _checkedSprite;
        else
            _checkboxButton.image.sprite = _uncheckedSprite;
    }
    private void ButtonClicked()
    {
        if(_perkSO != null)
        {
            if (_perkState == null)
            {
                _perkState = new PerkState() { PerkID = _perkSO.ID, Enabled = true };
                SkyforgeLoader.CurrentProfile.AcquiredPerks.Add(_perkState);
                SkyforgeLoader.EnablePerk(_perkSO);
            }
            else
            {
                if(_perkState.Enabled)
                    _perkState.Enabled = false;
                else
                    SkyforgeLoader.EnablePerk(_perkSO);

            }
            UpdateSprite(this, EventArgs.Empty);
            SkyforgeLoader.PerksChanged = true;
        }
        OnChange?.Invoke(this, EventArgs.Empty);
    }
    public void Check()
    {
        if (_perkSO != null)
        {
            if (_perkState == null)
            {
                _perkState = new PerkState() { PerkID = _perkSO.ID, Enabled = true };
                SkyforgeLoader.CurrentProfile.AcquiredPerks.Add(_perkState);
            }
            SkyforgeLoader.EnablePerk(_perkSO);
            UpdateSprite(this, EventArgs.Empty);
        }
        OnChange?.Invoke(this, EventArgs.Empty);
    }
    public void Uncheck()
    {
        if (_perkSO != null)
        {
            if (_perkState == null)
            {
                _perkState = new PerkState() { PerkID = _perkSO.ID, Enabled = true };
                SkyforgeLoader.CurrentProfile.AcquiredPerks.Add(_perkState);
            }
            else
            {
                _perkState.Enabled = false;
            }
            UpdateSprite(this, EventArgs.Empty);
        }
        OnChange?.Invoke(this, EventArgs.Empty);
    }
    #endregion
}