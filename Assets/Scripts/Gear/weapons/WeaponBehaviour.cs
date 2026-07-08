using UnityEngine;

public class WeaponBehaviour : MonoBehaviour
{
    #region Variables
    [Header("Weapon Related Variables")]
    [Tooltip("Weapon Scriptable Object wich base information")]
    public WeaponSO WeaponSO;
    [SerializeField] private Transform _weaponMesh;
    [Tooltip("Some abilities make the weapon leave trace behind while swinging. The particle effect for it is inside weapon prefab(should be adjusted to weapon size and shape)")]
    [SerializeField] private ParticleSystem _trailEffect;
    [Tooltip("All the sounds that the weapon can make. Usually same across the weapon type, unless some legendaries with custom sounds")]
    [SerializeField] SoundBundleBehaviour _sounds;
    [Header("Draw and Hide Animation")]
    [Tooltip("How much time the hiding/drawing animation will take")]
    [SerializeField] private float WeaponDrawAnimationTime = 0.2f;
    [Tooltip("To start draw animation after some time, to better align with the player character drawing animation")]
    [SerializeField] private float WeaponDrawDelay = 0.2f;
    [Tooltip("To start hide animation after some time, to better align with the player character hiding animation")]
    [SerializeField] private float WeaponHideDelay = 0.2f;
    [Tooltip("if the weapon should enter the scene already drawn, useful for cutscenes or map decoration")]
    [SerializeField] bool StartDrawn = false;
    private bool _drawScheduled, _hideScheduled, _isHidden;
    private float _drawTimer;
    #endregion

    #region Mono
    private void Start()
    {
        SetTrail(false);
        if (!StartDrawn)
            SetWeaponHide();
        else
            SetWeaponDraw();
    }
    private void Update()
    {
        if (_drawScheduled)
        {
            _drawTimer += Time.deltaTime;
            if (_drawTimer >= WeaponDrawDelay)
            {
                float drawAmount = ((WeaponDrawAnimationTime - (_drawTimer - WeaponDrawDelay)) / WeaponDrawAnimationTime) * 100;
                if (drawAmount < 0)
                {
                    drawAmount = 0;
                    _drawTimer = 0;
                    _drawScheduled = false;
                }
                SetHiddenAmount(drawAmount);
            }

        }
        else if (_hideScheduled)
        {
            _drawTimer += Time.deltaTime;
            if (_drawTimer >= WeaponHideDelay)
            {
                float drawAmount = (1 - ((WeaponDrawAnimationTime - (_drawTimer - WeaponHideDelay)) / WeaponDrawAnimationTime)) * 100;
                if (drawAmount > 100)
                {
                    drawAmount = 100;
                    _drawTimer = 0;
                    _hideScheduled = false;
                }
                SetHiddenAmount(drawAmount);
            }
        }
    }
    #endregion

    #region Methods
    public virtual void Equip(HeroBehaviour hero, Transform slot)
    {
        transform.SetParent(slot);
        gameObject.SetActive(true);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = new Vector3(1, 1, 1);
    }
    public virtual void Unequip(HeroBehaviour hero)
    {
        transform.SetParent(null);
        gameObject.SetActive(false);
    }
    public void PlaySound(string soundName)
    {
        _sounds.PlaySound(this.gameObject, soundName);
    }
    public void PlayLongSound(string soundName)
    {
        _sounds.Playlong(this.gameObject, soundName);
    }
    public void StopLongSound(string soundName)
    {
        _sounds.Fadeout(soundName);
    }
    public void AnimateWeaponDraw()
    {
        if(_isHidden)
        {
            _drawScheduled = true;
            _isHidden = false;
        }

    }
    public void AnimateWeaponHide()
    {
        if(!_isHidden)
        {
            _hideScheduled = true;
            _isHidden = true;
        }
    }
    public void SetWeaponDraw()
    {
        _isHidden = false;
        _hideScheduled = false;
        _drawScheduled = false;
        _drawTimer = 0;
        SetHiddenAmount(0);
    }
    public void SetWeaponHide()
    {
        _isHidden = true;
        _hideScheduled = false;
        _drawScheduled = false;
        _drawTimer = 0;
        SetHiddenAmount(100);
    }
    private void SetHiddenAmount(float amount)
    {
        var smr = _weaponMesh.GetComponent<SkinnedMeshRenderer>();
        if (smr != null)
            smr.SetBlendShapeWeight(0, amount);
    }
    public void SetTrail(bool on)
    {
        _trailEffect.gameObject.SetActive(on);
    }
    #endregion
}
