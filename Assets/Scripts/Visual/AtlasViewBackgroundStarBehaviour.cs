using UnityEngine;

public class AtlasViewBackgroundStarBehaviour : MonoBehaviour
{
    public const float LIGHT_UP_TIME = 0.2f;
    public const float LIGHT_DOWN_TIME = 2.5f;

    #region Variables
    [SerializeField] private SpriteRenderer _renderer;
    private float _lightUpTimer, _lightDownTimer;
    #endregion

    #region Mono
    private void Awake()
    {
        _renderer.color = new Color(1, 1, 1, 0);
    }
    private void Update()
    {
        if (_lightUpTimer > 0)
        {
            _lightUpTimer -= Time.deltaTime;
            _renderer.color = new Color(1, 1, 1, Mathf.Lerp(1, 0, _lightUpTimer / LIGHT_UP_TIME));
            if (_lightUpTimer <= 0)
            {
                _lightUpTimer = 0;
                _renderer.color = new Color(1, 1, 1, 1);
                _lightDownTimer = LIGHT_DOWN_TIME;
            }
        }
        else if (_lightDownTimer > 0)
        {
            _lightDownTimer -= Time.deltaTime;
            _renderer.color = new Color(1, 1, 1, Mathf.Lerp(0, 1, _lightDownTimer / LIGHT_DOWN_TIME));
            if (_lightDownTimer <= 0)
            {
                _lightDownTimer = 0;
                _renderer.color = new Color(1, 1, 1, 0);
            }
        }
    }
    private void FixedUpdate()
    {
        if (_lightUpTimer == 0 && Random.Range(0, 4000) == 0)
        {
            _lightUpTimer = LIGHT_UP_TIME;
        }
    }
    #endregion
}
