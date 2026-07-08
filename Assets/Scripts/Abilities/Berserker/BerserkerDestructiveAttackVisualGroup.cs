using UnityEngine;

public class BerserkerDestructiveAttackVisualGroup : MonoBehaviour
{
    #region Variables
    [Header("Destructive Attack")]
    [SerializeField] private BerserkerDestructiveAttackVisual _referenceCreviceVisual;
    [SerializeField] private Transform _distortion;
    [SerializeField] private Transform _visualGroup1, _visualGroup2, _visualGroup3, _visualGroup4;
    [SerializeField] private float _secondWaveTime, _thirdWaveTime, _fourthWaveTime, _effectFadeTime, _lightIntensity, _animationTimerMax;
    private Transform _currentEffect;
    private Light _currentLight;
    private float _animationTimer;
    private short _currentWave;
    public bool Strong { get; set; }
    #endregion

    #region Mono
    private void Awake()
    {
        _animationTimer = 0f;
        _currentWave = 0;
        Strong = false;
        _currentEffect = null;
    }
    void Update()
    {
        _animationTimer += Time.deltaTime;
        if (Strong)
        {
            if (_currentWave == 0)
            {
                StartStrong();
            }
            else if (_currentWave == 1 && _animationTimer > _secondWaveTime)
            {
                SpawnWave(_visualGroup2);
                _currentWave = 2;
            }
            else if (_currentWave == 2 && _animationTimer > _thirdWaveTime)
            {
                SpawnWave(_visualGroup3);
                _currentWave = 3;
            }
            else if (_currentWave == 3 && _animationTimer > _fourthWaveTime)
            {
                SpawnWave(_visualGroup4);
                _currentWave = 4;
            }
        }
        else
        {
            if (_currentWave == 0)
            {
                StartWeak();
            }
        }
        if (_currentEffect != null)
        {
            _currentLight.intensity = (_lightIntensity * (1 - (_animationTimer / _effectFadeTime)));
            if (_animationTimer > _effectFadeTime)
            {
                Destroy(_currentEffect.gameObject);
                _currentEffect = null;
            }
        }
        if (_animationTimer > _animationTimerMax)
        {
            Destroy(this.gameObject);
        }
    }
    #endregion

    #region Methods
    private void StartStrong()
    {
        SpawnWave(_visualGroup1);
        _currentEffect = Instantiate(_distortion, this.transform);
        _currentEffect.gameObject.SetActive(true);
        _currentLight = _currentEffect.GetComponentInChildren<Light>();
        _currentWave = 1;
    }
    private void StartWeak()
    {
        _currentEffect = Instantiate(_distortion, this.transform);
        _currentEffect.gameObject.SetActive(true);
        _currentLight = _currentEffect.GetComponentInChildren<Light>();
        _currentWave = 1;
    }
    private void SpawnWave(Transform wave)
    {
        foreach (var part in wave.GetComponentsInChildren<MeshRenderer>())
        {
            var partins = Instantiate(_referenceCreviceVisual, this.transform);
            partins.transform.position = part.transform.position;
            partins.transform.rotation = part.transform.rotation;
            partins.transform.localScale = part.transform.localScale/100;
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
