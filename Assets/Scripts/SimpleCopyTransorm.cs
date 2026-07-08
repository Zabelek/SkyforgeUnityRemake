using UnityEngine;

public class SimpleCopyTransorm : MonoBehaviour
{
    #region Variables
    [Tooltip("Transform of which rotation, location and scale will be copied every frame")]
    [SerializeField] private Transform _transformToCopy;
    public bool IsEnabled;
    #endregion

    #region Mono
    private void Update()
    {
        if(IsEnabled)
        {
            transform.position = _transformToCopy.position;
            transform.rotation = _transformToCopy.rotation;
            transform.localScale = _transformToCopy.localScale;
        }
    }
    #endregion
}
