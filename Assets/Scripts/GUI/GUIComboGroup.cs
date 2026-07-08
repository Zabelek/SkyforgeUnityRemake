using UnityEngine;
using UnityEngine.UI;

public class GUIComboGroup : MonoBehaviour
{
    #region Variables
    [Tooltip("To handle interface animations")]
    [SerializeField] Animator _animator;
    private float _disableTimer;
    private bool _scheduledForFadeout;
    [Tooltip("For left mouse button combo")]
    public Image LeftAbilityImage;
    [Tooltip("For right mouse button combo")]
    public Image RightAbilityImage;
    #endregion

    #region Mono
    private void Awake()
    {
        Reset();
    }
    private void Reset()
    {
        _disableTimer = 0.2f;
        _scheduledForFadeout = false;
    }
    private void Update()
    {
        if(_scheduledForFadeout)
        {
            if(_disableTimer<0)
            {
                this.gameObject.SetActive(false);
                Reset();
            }
            else
            {
                _disableTimer -= Time.deltaTime;
            }
        }
    }
    #endregion

    #region Methods
    public void Show()
    {
        if(!this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(true);
            _animator.SetTrigger("FadeIn");
        }
    }
    public void Hide()
    {
        if (this.gameObject.activeSelf && _scheduledForFadeout == false)
        {
            _scheduledForFadeout = true;
            _animator.SetTrigger("FadeOut");
        }       
    }
    #endregion
}
