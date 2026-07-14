using UnityEngine;
using UnityEngine.UI;

public class GUICustomToggle : MonoBehaviour
{
    #region Variables
    [Tooltip("Sprite to be displayed on main image when the check box is set to true")]
    [SerializeField] private Sprite _checked;
    [Tooltip("Sprite to be displayed on main image when the check box is set to false")]
    [SerializeField] private Sprite _unchecked;
    [Tooltip("The image to display sprites")]
    [SerializeField] private Image _display;
    #endregion

    #region Mono
    private void Start()
    {
        var toggle = GetComponent<Toggle>();
        if(toggle!=null)
        {
            toggle.onValueChanged.AddListener((bool value) =>
            {
                if (value)
                    _display.sprite = _checked;
                else
                    _display.sprite = _unchecked;
            });
        }
    }
    #endregion

    #region Methods
    public void SetByCode(bool value)
    {
        if (value)
            _display.sprite = _checked;
        else
            _display.sprite = _unchecked;
    }
    #endregion
}
