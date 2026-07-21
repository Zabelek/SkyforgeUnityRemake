using System;
using System.Globalization;
using TMPro;
using UnityEngine;

public class GUIGameMenuResourcesPanel : MonoBehaviour
{
    #region Variables
    [SerializeField] private TextMeshProUGUI _creditsTextBox;
    [SerializeField] private TextMeshProUGUI _aelionEidosesTextBox;
    [SerializeField] private Transform _creditsNotificationPosition, _aelionEidosNotificationPosition;
    [SerializeField] private GUIResourceChangeWidget _widgetBase;
    [SerializeField] private Sprite _iconAelionEidos, _iconCredits;
    private GUIResourceChangeWidget _currentCreditsWidget, _currentAelionEidosWidget;
    private CultureInfo _cultureInfo;
    #endregion

    #region Mono
    private void OnDestroy()
    {
        SkyforgeLoader.CurrentProfile.GameplayResources.ResourceChangedEvent -= ResourcesChanged;
    }
    private void OnEnable()
    {
        SkyforgeLoader.CurrentProfile.GameplayResources.ResourceChangedEvent += ResourcesChanged;
    }
    private void OnDisable()
    {
        SkyforgeLoader.CurrentProfile.GameplayResources.ResourceChangedEvent -= ResourcesChanged;
    }
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
    private void ResourcesChanged(object sender, GameplayResources.ResourceChangeEventArgs e)
    {
        if(gameObject.activeSelf)
        {
            if (e.ResourceType == GameplayResources.ResourceType.Credits)
            {
                _creditsTextBox.text = SkyforgeLoader.CurrentProfile.GameplayResources.Credits.ToString();
                if (_currentCreditsWidget != null)
                    _currentCreditsWidget.FadeOut();
                _currentCreditsWidget = Instantiate(_widgetBase, _creditsNotificationPosition);
                _currentCreditsWidget.SetValues(_iconCredits, e.Amount);
                _currentCreditsWidget.OnDestroyed += ClearCurrentWidget;

            }
            else if (e.ResourceType == GameplayResources.ResourceType.AelionEidos)
            {
                _aelionEidosesTextBox.text = SkyforgeLoader.CurrentProfile.GameplayResources.AelionEidoses.ToString();
                if (_currentAelionEidosWidget != null)
                    _currentAelionEidosWidget.FadeOut();
                _currentAelionEidosWidget = Instantiate(_widgetBase, _aelionEidosNotificationPosition);
                _currentAelionEidosWidget.SetValues(_iconAelionEidos, e.Amount);
                _currentAelionEidosWidget.OnDestroyed += ClearCurrentWidget;
            }
        }
    }
    private void ClearCurrentWidget(object sender, EventArgs e)
    {
        if(sender is GUIResourceChangeWidget)
        {
            if (_currentAelionEidosWidget == sender as GUIResourceChangeWidget)
            {
                _currentAelionEidosWidget.OnDestroyed -= ClearCurrentWidget;
                _currentAelionEidosWidget = null;
            }
            else if (_currentCreditsWidget == sender as GUIResourceChangeWidget)
            {
                _currentCreditsWidget.OnDestroyed -= ClearCurrentWidget;
                _currentCreditsWidget = null;
            }
        }
    }
    #endregion
}
