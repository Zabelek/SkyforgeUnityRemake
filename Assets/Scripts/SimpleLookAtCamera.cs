using UnityEngine;

public class SimpleLookAtCamera : MonoBehaviour
{
    private void Update()
    {
        transform.forward = Globals.Instance.ViewportCamera.transform.forward * -1;
    }
}
