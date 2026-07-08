using UnityEngine;

public class HealingOrbBehaviour : MonoBehaviour
{
    #region Variables
    [Header("Healing Orb Related Variables")]
    [Tooltip("The core of the healing orb. Gets smaller as the orb is about to disappear")]
    [SerializeField] private Transform _lightSprite;
    [Tooltip("Particle effect spawned on Player while healing")]
    [SerializeField] private Transform _healingEffect;
    [Tooltip("After this time, the healing orb will despawn")]
    [SerializeField] private float _lifetime;
    [Tooltip("After this time, the orb wil lstart to get smaller")]
    [SerializeField] private float _startFadingTimer;
    private float _currentLifetime;
    //sometomes healing triggers twice for some reason
    private bool _alreadyHealedDebug;
    #endregion

    #region Mono
    private void Awake()
    {
        _currentLifetime = 0;
        _alreadyHealedDebug = false;
    }
    private void FixedUpdate()
    {
        _currentLifetime += Time.fixedDeltaTime;
        if (_currentLifetime >= _startFadingTimer)
        {
            float currentSize = (_lifetime - _currentLifetime) / (_lifetime - _startFadingTimer);
            _lightSprite.localScale = new Vector3(currentSize, currentSize, currentSize);
        }
        if(_currentLifetime >= _lifetime)
        {
            Destroy(this.transform.parent.gameObject);
        }
    }
    #endregion

    #region Methods
    private void HealHero(HeroBehaviour hero)
    {
        if(!_alreadyHealedDebug)
        {
            if (hero.Stats.CurrentHP != hero.Stats.MaxHP)
            {
                Instantiate(_healingEffect, hero.transform).gameObject.SetActive(true);
                hero.HealPercent(0.1f, true);
                Destroy(this.transform.parent.gameObject);
                _alreadyHealedDebug = true;
            }
        }
        else
        {
            Debug.Log("HEALING ORB WARNING: Multiple enter triggers before destroy is called.");
        }
    }
    #endregion

    #region EventHandlers
    private void OnTriggerEnter(Collider other)
    {
        if(CharacterBehaviour.FindCharacterInCollider(other, out CharacterBehaviour character) == true && character is HeroBehaviour)
        {
            HealHero(character as HeroBehaviour);
        }
    }
    #endregion
}
