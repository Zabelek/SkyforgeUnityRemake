using UnityEngine;

public class SimpleLookAtCamera : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    private void Update()
    {
        if(_camera == null && Globals.Instance?.ViewportCamera != null)
            transform.forward = Globals.Instance.ViewportCamera.transform.forward * -1;
        else if(_camera == null)
            transform.forward = Camera.main.transform.forward * -1;
        else
            transform.forward = _camera.transform.forward * -1;
    }
}
