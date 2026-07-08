using System.Threading.Tasks;
using UnityEngine;

public class GUISceneBlackFade : MonoBehaviour
{
    public const float FADE_TIME = 1;

    #region Variables
    [SerializeField] CanvasGroup _group;
    private float _blackFadeInTimer, _blackFadeInTimerMax;
    private float _blackFadeOutTimer, _blackFadeOutTimerMax;
    #endregion

    #region Mono
    private void Update()
    {
        if (_blackFadeInTimer > 0)
        {
            _group.alpha = 1 - ((_blackFadeInTimer / _blackFadeInTimerMax) / FADE_TIME);
            _blackFadeInTimer -= Time.deltaTime;
            if (_blackFadeInTimer <= 0)
            {
                _group.alpha = 1;
            }
        }
        else if (_blackFadeOutTimer > 0)
        {
            _group.alpha = (_blackFadeOutTimer / _blackFadeOutTimerMax) / FADE_TIME;
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
        _blackFadeInTimer = FADE_TIME;
        _blackFadeInTimerMax = FADE_TIME;
        while( _blackFadeInTimer > 0)
        {
            await Task.Yield();
        }
    }
    public async Task StartFadeOut()
    {
        if (gameObject.activeSelf == false)
            gameObject.SetActive(true);
        _blackFadeOutTimer = FADE_TIME;
        _blackFadeOutTimerMax = FADE_TIME;
        while (_blackFadeOutTimer > 0)
        {
            await Task.Yield();
        }
    }
    #endregion
}
