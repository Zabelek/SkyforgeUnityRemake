using System.Collections.Generic;
using UnityEngine;

public class BerserkerDestructiveAttackVisual : MonoBehaviour
{
    #region Variables
    protected MeshRenderer _spawnedMesh;
    [SerializeField] protected float _timerMax, _maxEmit, _startFadeoutTimer;
    [SerializeField] protected List<Transform> _meshBaseVariants;
    [SerializeField] protected Transform _particlesBase;
    protected float _animationTimer, _currentEmit;
    protected int _shaderPropertyIdEmission, _shaderPropertyIdAlpha;
    #endregion

    #region Mono
    void Start()
    {
        _animationTimer = 0;
        _currentEmit = _maxEmit;
        _spawnedMesh = Instantiate(_meshBaseVariants[Random.Range(0, _meshBaseVariants.Count - 1)].GetComponent<MeshRenderer>(), this.transform);
        _spawnedMesh.gameObject.SetActive(true);
        if(_particlesBase != null)
            Instantiate(_particlesBase, this.transform).gameObject.SetActive(true);
        _shaderPropertyIdEmission = Shader.PropertyToID("_Emission_Intensity");
        _shaderPropertyIdAlpha = Shader.PropertyToID("_Alpha_Value");
    }
    private void Update()
    {
        _animationTimer += Time.deltaTime;
        if(_currentEmit > 0)
        {
            _currentEmit -= _maxEmit * Time.deltaTime;
            if(_currentEmit<0)
                _currentEmit = 0;
            SetPropertyBlock(_shaderPropertyIdEmission, _currentEmit);
        }
        if (_animationTimer > _startFadeoutTimer)
        {
            float currentAlpha = 1 - (_animationTimer - _startFadeoutTimer);
            SetPropertyBlock(_shaderPropertyIdAlpha, currentAlpha);
        }
        if (_animationTimer>_timerMax)
        {
            Destroy(_spawnedMesh.gameObject);
            Destroy(this.gameObject);
        }
    }
    #endregion

    #region Methods
    public void SetPropertyBlock(int propertyId, float value)
    {
        var block = new MaterialPropertyBlock();
        _spawnedMesh.GetPropertyBlock(block);
        block.SetFloat(propertyId, value);
        _spawnedMesh.SetPropertyBlock(block);
    }
    #endregion
}
