using System.Linq;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    #region Variables
    public CharacterBehaviour Caster;
    public CharacterBehaviour Target;
    public bool IsAutoRedirecting;
    public float MaxLifetime;
    private float _currentLifetime = 0;
    public Damage Damage;
    public float Speed;
    public bool CanBeStopped;
    private Collider _collider;
    public float HeightOffset;
    #endregion

    #region Mono
    protected virtual void Awake()
    {
        _collider = GetComponent<Collider>();
        if(_collider == null)
        {
            Debug.LogWarning("Projectile without collider has been spawned!");
            Destroy(this.gameObject);
        }
        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + HeightOffset, this.transform.position.z);
    }
    protected virtual void FixedUpdate()
    {
        if(!IsAutoRedirecting)
        {
            this.transform.position += this.transform.forward * Speed * Time.fixedDeltaTime;
        }
        else
        {
            this.transform.forward = new Vector3(Target.transform.position.x, Target.transform.position.y + HeightOffset, Target.transform.position.z) - this.transform.position;
            this.transform.position += this.transform.forward * Speed * Time.fixedDeltaTime;
        }
        ChechDetonation();
        _currentLifetime += Time.fixedDeltaTime;
        if(_currentLifetime > MaxLifetime)
        {
            Destroy(this.gameObject);
        }
    }
    #endregion

    #region Methods
    protected virtual void ChechDetonation()
    {
        Collider[] potentialColliders = Physics.OverlapSphere(this.transform.position, 1f);
        if (potentialColliders.Count() > 0)
        {
            if (CanBeStopped)
            {
                foreach (var collider in potentialColliders)
                {
                    if (CharacterBehaviour.FindEnemyCharacterInCollider(collider, Caster, out var enemy) == true)
                    {
                        Detonite(enemy);
                        break;
                    }
                }
            }
            else
            {
                foreach (var collider in potentialColliders)
                {
                    if (CharacterBehaviour.FindCharacterInCollider(collider, Caster, out var enemy) == true)
                    {
                        if (enemy == Target)
                        {
                            Detonite(enemy);
                            break;
                        }
                    }
                }
            }
        }
    }
    #endregion

    #region EventHandlers
    protected virtual void Detonite(CharacterBehaviour character)
    {
        if(character!= null)
        {
            character.TakeDamage(Damage);
        }
        Destroy(this.gameObject);
    }
    #endregion
}
