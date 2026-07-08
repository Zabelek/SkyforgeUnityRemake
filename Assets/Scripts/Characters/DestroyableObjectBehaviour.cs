using System.Collections.Generic;
using UnityEngine;

public class DestroyableObjectBehaviour : CharacterBehaviour
{
    #region Variables
    [Header("Destroyable Object")]
    [Tooltip("Mesh rendered while the object is alive")]
    [SerializeField] protected Transform _aliveMesh;
    [Tooltip("Mesh to swith after object's destruction")]
    [SerializeField] protected Transform _deadMesh;
    [Tooltip("particles to spawn when object is destroyed, for example, expliosion particles")]
    [SerializeField] protected ParticleSystem _deathParticles;
    [Tooltip("Time to spawn particles after object's destruction")]
    [SerializeField] protected float _particleSpawnTimer;
    [Tooltip("Time to switch meshes after object's destruction")]
    [SerializeField] protected float _deadMeshSpawnTimer;
    [Tooltip("In this radius, every character able to receive damage, will... well.. receive damage when the object is destroyed")]
    [SerializeField] protected float _explosionRadius;
    [Tooltip("The amount of damage other characters receive upon object's destruction. By default, the damage is dealt when destruction particles spawn.")]
    [SerializeField] protected int _explosionDamage;
    [Tooltip("Sound to play upon destruction")]
    [SerializeField] protected SoundEffectSO _deathSound;
    protected bool _deadParticlesSpawned, _deadMeshSpawned;
    #endregion

    #region Mono
    protected override void Start()
    {
        base.Start();
        _deadParticlesSpawned = false;
        _deadMeshSpawned = false;
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (IsDead && (_particleSpawnTimer > 0 || _deadMeshSpawnTimer > 0))
        {
            _particleSpawnTimer -= Time.fixedDeltaTime;
            _deadMeshSpawnTimer -= Time.fixedDeltaTime;
            if (_deadMeshSpawnTimer<=0 && _deadMeshSpawned == false)
            {
                _deadMeshSpawned = true;
                _aliveMesh.gameObject.SetActive(false);
                _deadMesh.gameObject.SetActive(true);
            }
            if(_particleSpawnTimer<=0 && _deadParticlesSpawned == false)
            {
                _deadParticlesSpawned = true;
                Globals.Instance.SoundManager.PlaySFXFast(_deathSound, transform.position);
                if(_deathParticles!=null)
                    _deathParticles.gameObject.SetActive(true);
                DealExplosionDamage();
            }
        }
    }
    #endregion

    #region Methods
    private void DealExplosionDamage()
    {
        var potentialCasualities = Physics.OverlapSphere(transform.position, _explosionRadius);
        if (potentialCasualities.Length > 1)
        {
            List<CharacterBehaviour> distinctCharacters = new();
            foreach (var casuality in potentialCasualities)
            {
                if (CharacterBehaviour.FindCharacterInCollider(casuality, this, out CharacterBehaviour character) && character != this)
                {
                    character.TakeDamage(new Damage(this, _explosionDamage, false, true));
                    distinctCharacters.Add(character);
                }
            }
        }
    }
    #endregion
}
