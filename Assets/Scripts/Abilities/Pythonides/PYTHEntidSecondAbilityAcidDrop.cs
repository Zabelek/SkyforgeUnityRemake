using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class PYTHEntidSecondAbilityAcidDrop : MonoBehaviour
{
    #region Variables
    [SerializeField] private DecalProjector _projector;
    [SerializeField] private Collider _collider;
    public CharacterBehaviour Caster; 
    //timers
    [SerializeField] private float _maxDamageTimer, _acidFadeoutTimerMax, _afterFadeOutTimer;
    private float _nextDamage, _acidFadeoutTimer;
    #endregion

    #region Methods
    private void Start()
    {
        _nextDamage = _maxDamageTimer;
    }
    private void FixedUpdate()
    {
        _acidFadeoutTimer += Time.fixedDeltaTime;
        if (_acidFadeoutTimer <= _acidFadeoutTimerMax)
        {
            _nextDamage -= Time.fixedDeltaTime;
            if (_nextDamage<=0)
            {
                _nextDamage = _maxDamageTimer;
                DealDamage();
            }
        }
        else
        {
            if(_afterFadeOutTimer>0)
            {
                _afterFadeOutTimer -= Time.fixedDeltaTime;
                _projector.fadeFactor = _afterFadeOutTimer / _acidFadeoutTimerMax;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }
    private void DealDamage()
    {
        var potentialCasualities = Physics.OverlapSphere(transform.position, 7);
        if (_collider != null)
        {
            _collider.gameObject.SetActive(true);
            foreach (var casuality in potentialCasualities)
            {
                if (casuality == _collider)
                    continue;
                if (casuality.bounds.Intersects(_collider.bounds) && CharacterBehaviour.FindCharacterInCollider(casuality, Caster, out CharacterBehaviour character))
                {
                    var damage = new Damage(Caster, (int)(Caster.GetEffectiveDamage() / 2), false, true);
                    character.TakeDamage(damage);
                }
            }
            _collider.gameObject.SetActive(false);
        }
    }
    #endregion
}
