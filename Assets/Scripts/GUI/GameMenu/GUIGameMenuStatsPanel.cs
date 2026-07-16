using System.Globalization;
using TMPro;
using UnityEngine;

public class GUIGameMenuStatsPanel : MonoBehaviour
{
    #region Variables
    [SerializeField] private TextMeshProUGUI _profileNameTextBox;
    [SerializeField] private TextMeshProUGUI _prestigeTextBox;
    private CultureInfo _cultureInfo;
    #endregion

    #region Mono
    #endregion

    #region Methods
    public void UpdateValues()
    {
        if(_cultureInfo == null)
        {
            _cultureInfo = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            _cultureInfo.NumberFormat.NumberGroupSeparator = " ";
        }
        _profileNameTextBox.text = SkyforgeLoader.CurrentProfile.Name;
        _prestigeTextBox.text = SkyforgeLoader.CurrentProfile.Prestige.ToString("#,##0", _cultureInfo);
    }
    #endregion

    #region EventHandlers
    #endregion
}
