using Unity.Cinemachine;
using UnityEngine;

public class CinemachineCustomZoom : MonoBehaviour
{
    #region Variables
    private CinemachineOrbitalFollow _camera;
    [Tooltip("minimal distance the camera allows")]
    [SerializeField] private float _minDistance = 1;
    [Tooltip("Maximal distance the camera allows (when offset is applied, the distance from camera might actually be higher)")]
    [SerializeField] private float _maxDistance = 15;
    [Tooltip("Zoom smoothing animation speed")]
    [SerializeField] private int _easeSpeed = 1;
    private float _destinationDistance = 0;
    public float DistanceOffset = 0;
    #endregion

    #region Mono
    private void Start()
    {
        _camera = GetComponent<CinemachineOrbitalFollow>();
        _destinationDistance = _camera.Radius;
    }
    private void LateUpdate()
    {
        if(Input.mouseScrollDelta.y!= 0)
        {
            //_camera.Lens.FieldOfView = _camera.Lens.FieldOfView + Input.mouseScrollDelta.y;
            _destinationDistance -= Input.mouseScrollDelta.y;
            if(_destinationDistance < _minDistance + DistanceOffset)
            {
                _destinationDistance = _minDistance + DistanceOffset;
            }
            else if (_destinationDistance > _maxDistance + DistanceOffset)
            {
                _destinationDistance = _maxDistance + DistanceOffset;
            }
        }
        if(_destinationDistance - DistanceOffset != _camera.Radius)
        {
            //_camera.Lens.FieldOfView = _camera.Lens.FieldOfView + Input.mouseScrollDelta.y;
            for(int i=0; i < _easeSpeed; i++)
            {
                var currentAmount = (((_destinationDistance + DistanceOffset) - _camera.Radius) * Time.deltaTime);
                _camera.Radius += currentAmount;
                if (Mathf.Abs(currentAmount) < 0.0001f)
                {
                    _camera.Radius = _destinationDistance + DistanceOffset;
                    break;
                }
            }
        }
    }
    public void SetRadius(float camFreezeRadiusValue)
    {
        _destinationDistance = camFreezeRadiusValue;
    }
    #endregion
}
