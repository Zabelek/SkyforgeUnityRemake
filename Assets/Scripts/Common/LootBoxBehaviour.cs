using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootBoxBehaviour : MonoBehaviour
{
    public const float FADE_OUT_TIME = 1;

    #region Variables
    [Tooltip("To animate opening and idle floating")]
    [SerializeField] private Animator _animator;
    private List<Item> _items;
    private float _fadeOutTimer, _initialLightIntensity;
    [Tooltip("Main mesh of the lootbox")]
    [SerializeField] private SkinnedMeshRenderer _renderer;
    [SerializeField] private Light _light;
    [SerializeField] private SpriteRenderer _sprite;
    [Tooltip("Particle System to actyivate on box opening")]
    [SerializeField] private ParticleSystem _particles;
    public bool IsAlreadyOpen;
    #endregion

    #region Mono
    private void Awake()
    {
        _items = new();
        _fadeOutTimer = 0;
        if (Globals.Instance != null)
            Globals.Instance.RegisteredLootboxes.Add(this);
        _initialLightIntensity = _light.intensity;
        IsAlreadyOpen = false;
    }
    private void Update()
    {
        var target = Vector3.zero;
        if (Globals.Instance?.ViewportCamera != null)
            target = Globals.Instance.ViewportCamera.transform.forward * -1;
        else
            target = Camera.main.transform.forward * -1;
        target.y = 0;
        transform.forward = target;
        if(_fadeOutTimer!= 0)
        {
            _fadeOutTimer -= Time.deltaTime;
            //mesh
            int FadeID = Shader.PropertyToID("_Alpha_Value");
            var block = new MaterialPropertyBlock();
            _renderer.GetPropertyBlock(block);
            var value = _fadeOutTimer / FADE_OUT_TIME;
            if (value < 0)
                value = 0;
            block.SetFloat(FadeID, value);
            _renderer.SetPropertyBlock(block);
            _light.intensity = _initialLightIntensity * value;
            _sprite.color = new Color(1, 1, 1, value);
            if (_fadeOutTimer<=0)
            {
                if (Globals.Instance != null)
                    Globals.Instance.RegisteredLootboxes.Remove(this);
                Destroy(this.gameObject);
            }
        }
    }
    #endregion

    #region Methods
    public void AddItem(Item item)
    {
        _items.Add(item);
    }
    public void Loot()
    {
        _animator.SetTrigger("Open");
        IsAlreadyOpen = true;
        StartCoroutine(StartFadeout(1));
        foreach(var item in _items)
        {
            SkyforgeLoader.CurrentProfile.Inventory.AddItem(item);
        }
        _particles.gameObject.SetActive(true);
    }
    private IEnumerator StartFadeout(float time)
    {
        yield return new WaitForSeconds(time);
        _fadeOutTimer = 1;
    }
    #endregion
}
