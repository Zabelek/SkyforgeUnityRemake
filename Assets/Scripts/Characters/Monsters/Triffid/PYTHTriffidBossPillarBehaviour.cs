using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class PYTHTriffidBossPillarBehaviour : DestroyableObjectBehaviour
{
    public const float ANIMATION_TIMER = 2f;

    #region Variables
    private float _spawnAnimationTimer = ANIMATION_TIMER;
    [Header("Pillar")]
    [SerializeField] private ParticleSystem _particlesBase;
    private ParticleSystem _currentParticles;
    private SkinnedMeshRenderer _currentMesh;
    private bool _hideAnimationStarted = false;
    #endregion

    #region Mono
    protected override void Awake()
    {
        base.Awake();
        _currentMesh = _aliveMesh.GetComponent<SkinnedMeshRenderer>();
        _currentMesh.SetBlendShapeWeight(0, 100);
        _currentParticles = Instantiate(_particlesBase, this.transform);
        _currentParticles.gameObject.SetActive(true);
    }
    protected override void Update()
    {
        if(IsDead)
        {
            if(!_hideAnimationStarted)
            {
                _hideAnimationStarted = true;
                _spawnAnimationTimer = 2;
                _currentParticles = Instantiate(_particlesBase, this.transform);
                _currentParticles.gameObject.SetActive(true);
            }
            if (_spawnAnimationTimer > 0)
            {
                _spawnAnimationTimer -= Time.deltaTime;
                if (_currentParticles == null)
                {
                    _currentParticles = Instantiate(_particlesBase, this.transform);
                    _currentParticles.gameObject.SetActive(true);
                }
                var shapevalue = (1 - (_spawnAnimationTimer / ANIMATION_TIMER)) * 100;
                _currentMesh.SetBlendShapeWeight(0, shapevalue);
                if (_spawnAnimationTimer < 0)
                {
                    _spawnAnimationTimer = 0;
                    _currentMesh.SetBlendShapeWeight(0, 100);
                }
            }
        }
        else if (_spawnAnimationTimer > 0)
        {
            _spawnAnimationTimer -= Time.deltaTime;
            var shapevalue = (_spawnAnimationTimer / ANIMATION_TIMER) * 100;
            _currentMesh.SetBlendShapeWeight(0, shapevalue);
            if(_spawnAnimationTimer<0)
            {
                _spawnAnimationTimer = 0;
                _currentMesh.SetBlendShapeWeight(0, 0);
            }
        }
    }
    #endregion

    #region Methods
    #endregion

    #region EventHandlers
    #endregion
}
