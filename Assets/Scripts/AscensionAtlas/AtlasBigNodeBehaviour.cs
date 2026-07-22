using UnityEngine;

public class AtlasBigNodeBehaviour : AtlasNodeBehaviour
{
    #region Variables
    [SerializeField] private SpriteRenderer _circle1, _circle2, _circle3;
    [SerializeField] private ParticleSystem _particles;
    #endregion

    #region Mono
    private void Update()
    {
        if (_circle1 != null)
            _circle1.transform.rotation = Quaternion.Euler(_circle1.transform.rotation.eulerAngles.x,
                _circle1.transform.rotation.eulerAngles.y, _circle1.transform.rotation.eulerAngles.z -Time.deltaTime * 4);
        if (_circle2 != null)
            _circle2.transform.rotation = Quaternion.Euler(_circle2.transform.rotation.eulerAngles.x,
                _circle2.transform.rotation.eulerAngles.y, _circle2.transform.rotation.eulerAngles.z - Time.deltaTime * 8);
        if (_circle3 != null)
            _circle3.transform.rotation = Quaternion.Euler(_circle3.transform.rotation.eulerAngles.x,
                _circle3.transform.rotation.eulerAngles.y, _circle3.transform.rotation.eulerAngles.z - Time.deltaTime * 12);
    }
    #endregion

    #region Variables
    public override void SetToActive(bool active)
    {
        base.SetToActive(active);
        _particles.gameObject.SetActive(active);
    }
    #endregion

    #region EventHandlers
    public override void OnPointerEnter()
    {
        base.OnPointerEnter();
        _atlas.NodeSelectionSprite.transform.localScale = new Vector3(2, 2, 2);
    }
    #endregion
}
