using UnityEngine;

public class AtlasNodeSelection : MonoBehaviour
{
    [SerializeField] private GameObject _activeSprite, _inactiveSprite;

    public void SettActivated(bool activated)
    {
        _activeSprite.SetActive(activated);
        _inactiveSprite.SetActive(!activated);
    }
}
