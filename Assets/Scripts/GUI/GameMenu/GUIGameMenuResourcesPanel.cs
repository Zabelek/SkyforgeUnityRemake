using System.Globalization;
using TMPro;
using UnityEngine;

public class GUIGameMenuResourcesPanel : MonoBehaviour
{
    #region Variables
    [SerializeField] private TextMeshProUGUI _creditsTextBox;
    [SerializeField] private TextMeshProUGUI _aelionEidosesTextBox;
    private CultureInfo _cultureInfo;
    #endregion

    #region Mono
    #endregion

    #region Methods
    public void UpdateValues()
    {
        if (_cultureInfo == null)
        {
            _cultureInfo = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            _cultureInfo.NumberFormat.NumberGroupSeparator = " ";
        }
        _creditsTextBox.text = SkyforgeLoader.CurrentProfile.GameplayResources.Credits.ToString("#,##0", _cultureInfo);
        _aelionEidosesTextBox.text = SkyforgeLoader.CurrentProfile.GameplayResources.AelionEidoses.ToString("#,##0", _cultureInfo);
    }
    #endregion

    #region EventHandlers
    #endregion
}
