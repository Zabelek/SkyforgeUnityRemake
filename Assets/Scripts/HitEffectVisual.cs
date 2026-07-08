using UnityEngine;

public class HitEffectVisual : MonoBehaviour
{
    #region Variables
    private float _timer = 0;
    [SerializeField] private float maxLifespanTimer = 0.5f;
    #endregion

    #region Mono
    void Update()
    {
        if(_timer > maxLifespanTimer)
        {
            Destroy(this.gameObject);
        }
        _timer += Time.deltaTime;
    }
    #endregion
}