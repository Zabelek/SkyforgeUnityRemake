using UnityEngine;

public class BerserkerTectonicBlastVisualGroup : MonoBehaviour
{
    #region Variables
    [SerializeField] private Transform _firstWaveGroup;
    [SerializeField] private Transform _secondWaveGroup;
    [SerializeField] private Transform _thirdWaveGroup;
    [SerializeField] private Transform _distortion;
    [SerializeField] private BerserkerDestructiveAttackVisual _center;
    [SerializeField] private BerserkerDestructiveAttackVisual _referenceCreviceVisual;
    private Transform _currentEffect;
    private Light _light;
    [SerializeField] private float _secondWaveTimer, _thirdWaveTimer, _effectFadeTime, _lightIntensity, _timerMax;
    private float _timer;
    private short _currentWave;
    #endregion

    #region Mono
    private void Awake()
    {
        _timer = 0f;
        _currentWave = 0;
        _currentEffect = null;
    }
    void Update()
    {
        _timer += Time.fixedDeltaTime;
        if (_currentWave == 0)
        {
            StartEffects();
        }
        else if (_currentWave == 1 && _timer > _secondWaveTimer)
        {
            SpawnWave(_secondWaveGroup);
            _currentWave = 2;
        }
        else if (_currentWave == 2 && _timer > _thirdWaveTimer)
        {
            SpawnWave(_thirdWaveGroup);
            _currentWave = 3;
        }
        if (_currentEffect != null)
        {
            _light.intensity = (_lightIntensity * (1 - (_timer / _effectFadeTime)));
            if (_timer > _effectFadeTime)
            {
                Destroy(_currentEffect.gameObject);
                _currentEffect = null;
            }
        }
        if (_timer > _timerMax)
        {
            Destroy(this.gameObject);
        }
    }
    #endregion

    #region Methods
    private void StartEffects()
    {
        Instantiate(_center, this.transform).gameObject.SetActive(true);
        SpawnWave(_firstWaveGroup);
        _currentEffect = Instantiate(_distortion, this.transform);
        _currentEffect.gameObject.SetActive(true);
        _light = _currentEffect.GetComponentInChildren<Light>();
        _currentWave = 1;
    }
    private void SpawnWave(Transform wave)
    {
        foreach (var part in wave.GetComponentsInChildren<MeshRenderer>())
        {
            var partins = Instantiate(_referenceCreviceVisual, this.transform);
            partins.transform.position = part.transform.position;
            partins.transform.rotation = part.transform.rotation;
            if (Physics.Raycast((partins.transform.position + new Vector3(0, 10f, 0)), Vector3.down, out RaycastHit hit2, 20f, LayerMask.GetMask("Default")))
            {
                Vector3 spawnPos = hit2.point;
                spawnPos.y += 0.01f;
                partins.transform.position = spawnPos;
            }
            partins.gameObject.SetActive(true);
        }
    }
    #endregion
}
