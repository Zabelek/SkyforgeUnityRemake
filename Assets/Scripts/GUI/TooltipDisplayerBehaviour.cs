using UnityEngine;

public class TooltipDisplayerBehaviour : MonoBehaviour
{
    #region Variables
    [Header("Tooltips")]
    [Tooltip("Prefab used to spawn tooltips")]
    [SerializeField] protected GUITooltip _tooltipBase;
    [Tooltip("Canvas reference needed for tooltips to be correctly positioned")]
    [SerializeField] protected Canvas _tooltipCanvas;
    [Tooltip("Where tooltips will be spawned")]
    [SerializeField] protected Transform _tooltipsParent;
    protected GUITooltip _currentTooltip;
    #endregion

    #region Mono
    protected virtual void OnDisable()
    {
        _currentTooltip?.gameObject.SetActive(false);
    }
    #endregion

    #region Methods
    protected virtual void SetUpNewTooltip()
    {
        _currentTooltip = Instantiate(_tooltipBase, _tooltipsParent);
        _currentTooltip.SetCanvas(_tooltipCanvas);
    }
    protected virtual void SetUpNewTooltip(ItemSO itemSO)
    {
        SetUpNewTooltip();
        _currentTooltip.SetForItem(itemSO);
    }
    protected void DestroyCurrentTooltip()
    {
        if (_currentTooltip != null)
        {
            Destroy(_currentTooltip.gameObject);
            _currentTooltip = null;
        }
    }
    #endregion
}
