using UnityEngine;

public class AtlasNodeUnlockVisualEffect : MonoBehaviour
{
    #region Variables
    [SerializeField] private SpriteRenderer _effect1Sprite, _effect2Sprite, _effect3Sprite, _effect4Sprite;
    [SerializeField] private ParticleSystem _particles;
    [SerializeField] private float _timer;
    [SerializeField] private float _effect1StartFadeOutTime, _effect1EndTime, _effect2StartTime, _effect2StartFadeOut, effect2EndTime,_effect3StartTime, 
        _effect3StartFadeOut, effect3EndTime, _effect4StartTime, _effect4StartFadeOut, effect4EndTime, _particlesSpawnTimer, _endTimer;
    private Vector3 _effect3InitialScale;
    private bool _particlesSpawned;
    #endregion

    #region Mono
    private void Awake()
    {
        _effect1Sprite.gameObject.SetActive(false);
        _effect2Sprite.gameObject.SetActive(false);
        _effect3Sprite.gameObject.SetActive(false);
        _effect4Sprite.gameObject.SetActive(false);
        _particles.gameObject.SetActive(false);
        _effect3InitialScale = _effect3Sprite.transform.localScale;
    }
    private void Update()
    {
        _timer += Time.deltaTime;
        //Effect1
        if (_timer < _effect1StartFadeOutTime)
        {
            if (!_effect1Sprite.gameObject.activeSelf)
                _effect1Sprite.gameObject.SetActive(true);
            _effect1Sprite.color = new Color(1, 1, 1, Mathf.Lerp(0, 1, _timer / _effect1StartFadeOutTime));
        }
        else if(_timer >= _effect1StartFadeOutTime && _timer < _effect1EndTime)
        {
            _effect1Sprite.color = new Color(1, 1, 1, Mathf.Lerp(1, 0, (_timer - _effect1StartFadeOutTime) / (_effect1EndTime - _effect1StartFadeOutTime)));
        }
        else if(_timer >= _effect1EndTime && _effect1Sprite.gameObject.activeSelf)
            _effect1Sprite.gameObject.SetActive(false);
        //Effect2
        if (_timer >= _effect2StartTime && _timer < _effect2StartFadeOut && !_effect2Sprite.gameObject.activeSelf)
        {
            _effect2Sprite.gameObject.SetActive(true);
            _effect2Sprite.color = new Color(1, 1, 1, Mathf.Lerp(0, 1, (_timer - _effect2StartTime) / (_effect2StartFadeOut - _effect2StartTime)));
        }
        else if (_timer >= _effect2StartTime && _timer < _effect2StartFadeOut)
        {
            _effect2Sprite.color = new Color(1, 1, 1, Mathf.Lerp(0, 1, (_timer - _effect2StartTime) / (_effect2StartFadeOut - _effect2StartTime)));
        }
        else if (_timer >= _effect2StartFadeOut && _timer < effect2EndTime)
        {
            _effect2Sprite.color = new Color(1, 1, 1, Mathf.Lerp(1, 0, (_timer - _effect2StartFadeOut) / (effect2EndTime - _effect2StartFadeOut)));
        }
        else if (_timer > effect2EndTime && _effect2Sprite.gameObject.activeSelf)
            _effect2Sprite.gameObject.SetActive(false);
        //Effect3
        if (_timer > _effect3StartTime && _timer < effect3EndTime)
        {
            _effect3Sprite.transform.rotation = Quaternion.Euler(_effect3Sprite.transform.rotation.eulerAngles.x, _effect3Sprite.transform.rotation.eulerAngles.y,
                _effect3Sprite.transform.rotation.eulerAngles.z-4);
            _effect3Sprite.transform.localScale = _effect3InitialScale * (1 + ((_timer - _effect3StartTime)));
        }
        if (_timer > _effect3StartTime && _timer < _effect3StartFadeOut && !_effect3Sprite.gameObject.activeSelf)
        {
            _effect3Sprite.gameObject.SetActive(true);
            _effect3Sprite.color = new Color(1, 1, 1, Mathf.Lerp(0, 1, (_timer - _effect3StartTime) / (_effect3StartFadeOut - _effect3StartTime)));
        }
        else if (_timer >= _effect3StartTime && _timer < _effect3StartFadeOut)
        {
            _effect3Sprite.color = new Color(1, 1, 1, Mathf.Lerp(0, 1, (_timer - _effect3StartTime) / (_effect3StartFadeOut - _effect3StartTime)));
        }
        else if (_timer >= _effect3StartFadeOut && _timer < effect3EndTime)
        {
            _effect3Sprite.color = new Color(1, 1, 1, Mathf.Lerp(1, 0, (_timer - _effect3StartFadeOut) / (effect3EndTime - _effect3StartFadeOut)));
        }
        else if (_timer > effect3EndTime && _effect3Sprite.gameObject.activeSelf)
            _effect3Sprite.gameObject.SetActive(false);
        //Effect4
        if (_timer >= _effect4StartTime && _timer < _effect4StartFadeOut && !_effect4Sprite.gameObject.activeSelf)
        {
            _effect4Sprite.gameObject.SetActive(true);
            _effect4Sprite.color = new Color(1, 1, 1, Mathf.Lerp(0, 1, (_timer - _effect4StartTime) / (_effect4StartFadeOut - _effect4StartTime)));
        }
        else if (_timer >= _effect4StartTime && _timer < _effect4StartFadeOut)
        {
            _effect4Sprite.color = new Color(1, 1, 1, Mathf.Lerp(0, 1, (_timer - _effect4StartTime) / (_effect4StartFadeOut - _effect4StartTime)));
        }
        else if (_timer >= _effect4StartFadeOut && _timer < effect4EndTime)
        {
            _effect4Sprite.color = new Color(1, 1, 1, Mathf.Lerp(1, 0, (_timer - _effect4StartFadeOut) / (effect4EndTime - _effect4StartFadeOut)));
        }
        else if (_timer > effect4EndTime && _effect4Sprite.gameObject.activeSelf)
            _effect4Sprite.gameObject.SetActive(false);
        //Perticles
        if(_timer >= _particlesSpawnTimer && !_particlesSpawned)
        {
            _particlesSpawned = true;
            _particles.gameObject.gameObject.SetActive(true);
        }
        //ending
        if(_timer >= _endTimer)
        {
            Destroy(this.gameObject);
        }
    }
    #endregion
}
