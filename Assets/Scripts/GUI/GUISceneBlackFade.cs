using System.Threading.Tasks;
using UnityEngine;

public class GUISceneBlackFade : MonoBehaviour
{
    public const float FADE_TIME = 1;

    #region Variables
    [SerializeField] CanvasGroup _group;
    private float _blackFadeInTimer, _blackFadeInTimerMax;
    private float _blackFadeOutTimer, _blackFadeOutTimerMax;
    [Tooltip("The default value is defined as a const in the class. Put here anything other than zero to override it for this specific black fade")]
    public float CustomFadeTime = 0;
    #endregion

    #region Mono
    private void Awake()
    {
        if (CustomFadeTime == 0)
            CustomFadeTime = FADE_TIME;
    }
    private void Update()
    {
        if (_blackFadeInTimer > 0)
        {
            _group.alpha = 1 - ((_blackFadeInTimer / _blackFadeInTimerMax) / CustomFadeTime);
            _blackFadeInTimer -= Time.deltaTime;
            if (_blackFadeInTimer <= 0)
            {
                _group.alpha = 1;
            }
        }
        else if (_blackFadeOutTimer > 0)
        {
            _group.alpha = (_blackFadeOutTimer / _blackFadeOutTimerMax) / CustomFadeTime;
            _blackFadeOutTimer -= Time.deltaTime;
            if (_blackFadeOutTimer <= 0)
            {
                gameObject.SetActive(false);
            }
        }
    }
    #endregion

    #region Methods
    public async Task StartFadeIn()
    {
        _group.alpha = 0;
        if (gameObject.activeSelf == false)
            gameObject.SetActive(true);
        _blackFadeInTimer = CustomFadeTime;
        _blackFadeInTimerMax = CustomFadeTime;
        while( _blackFadeInTimer > 0)
        {
            await Task.Yield();
        }
    }
    public async Task StartFadeOut()
    {
        if (gameObject.activeSelf == false)
            gameObject.SetActive(true);
        _blackFadeOutTimer = CustomFadeTime;
        _blackFadeOutTimerMax = CustomFadeTime;
        while (_blackFadeOutTimer > 0)
        {
            await Task.Yield();
        }
    }
    #endregion
}
