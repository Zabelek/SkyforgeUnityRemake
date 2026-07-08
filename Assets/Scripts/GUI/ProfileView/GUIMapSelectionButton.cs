using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GUIMapSelectionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    #region Variables
    public MapSO Map;
    private bool _isPicked, _isHovered;
    [SerializeField] private Image _hoverImage, _selectImage, _mapImage;
    private GUIProfileViewInterface _profileInterface;
    #endregion

    #region Mono
    private void Start()
    {
        _isPicked = false;
        _isHovered = false;
    }
    protected void OnEnable()
    {
        OnPointerExit(null);
    }
    #endregion

    #region Methods
    public void SetProfileInterface(GUIProfileViewInterface profileInterface)
    {
        _profileInterface = profileInterface;
    }
    public void Deselect()
    {
        _isPicked = false;
        _selectImage.gameObject.SetActive(false);
    }
    public void SetMap(MapSO map)
    {
        Map = map;
        _mapImage.sprite = map.Thumbnail;
    }
    #endregion

    #region EventHandlers
    public void OnPointerDown(PointerEventData eventData)
    {
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!_isHovered)
        {
            _isHovered = true;
            _hoverImage.gameObject.SetActive(true);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (_isHovered)
        {
            _isHovered = false;
            _hoverImage.gameObject.SetActive(false);
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_isPicked)
        {
            _isPicked = true;
            _profileInterface.SelectMap(Map);
            _selectImage.gameObject.SetActive(true);
        }
    }
    #endregion
}
