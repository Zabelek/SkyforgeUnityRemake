using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PYTHEntidSecondAbilityBehaviour : AbilityBehaviour
{
    private const float MOVEMENT_PREDICT_RATE = 5;
    private const float PROJECTILE_DESTROY_DISTANCE = 0.15f;

    #region Variables
    [Header("Entid Spit Ability")]
    [SerializeField] private Transform _projectileBase;
    [SerializeField] private PYTHEntidSecondAbilityAcidDrop _dropBase;
    [SerializeField] private SoundEffectSO _spitSound;
    [SerializeField] private SoundEffectSO _impactSound;
    [SerializeField] private float _projectileDelay, _projectileSlowScale, _archHeight;
    private Vector3 _target, _initialProjectilePosition, _alreadyMovedAmount;

    private Transform _currentProjectile;
    #endregion

    #region Methods
    public override void UpdateAbility(CharacterBehaviour performer, HeroClassBehaviour heroClass)
    {
        base.UpdateAbility(performer, heroClass);
        //ball flying trajectory, calculating the movement
        if (_currentProjectile != null && _performingTimer >= _projectileDelay)
        {
            var timeMod = ((_performingTimer - _projectileDelay) / (_untilHitTimer) / (_initialProjectilePosition - _target).magnitude) * _projectileSlowScale;
            var vec = Vector3.Lerp(_initialProjectilePosition, _target, timeMod);
            vec -= _initialProjectilePosition;
            vec.y += Mathf.Sin(timeMod * Mathf.PI) * _archHeight;
            vec = vec - _alreadyMovedAmount;
            _alreadyMovedAmount += vec;
            _currentProjectile.transform.position += vec;
        }
        if (_currentProjectile != null && _currentProjectile.gameObject.IsDestroyed() == false
            && (_currentProjectile.transform.position - _target).magnitude < PROJECTILE_DESTROY_DISTANCE)
        {
            var drop = Instantiate(_dropBase, _target, _currentProjectile.rotation);
            drop.Caster = performer;
            Destroy(_currentProjectile.gameObject);
            if (_impactSound != null)
                Globals.Instance.SoundManager.PlaySFXFast(_impactSound, _target);
        }
    }
    public override void LaunchAbility(CharacterBehaviour performer)
    {
        base.LaunchAbility(performer);
        _currentProjectile = Instantiate(_projectileBase, performer.transform.position, performer.transform.rotation);
        _initialProjectilePosition = performer.transform.position;
        if (_spitSound != null)
            Globals.Instance.SoundManager.PlaySFXFast(_spitSound, performer.transform.position);
        performer.PlayAnimation("Skill_Spit", true);
    }
    public override bool CheckPerformAvailability(CharacterBehaviour performer)
    {
        if (performer.ActiveEnemies.Any() && performer is MonsterBehaviour && ((MonsterBehaviour)performer).CurrentlyUpdatedAbilities.Any(a => a == this) == false)
        {
            var chosenEnemy = performer.ActiveEnemies[Random.Range(0, performer.ActiveEnemies.Count)];
            _target = chosenEnemy.transform.position;
            if (chosenEnemy is PlayerBehaviour)
            {
                _target += ((PlayerBehaviour)chosenEnemy).LastMovementDirection.normalized * MOVEMENT_PREDICT_RATE;
            }
            return base.CheckPerformAvailability(performer);
        }
        return false;
    }
    public override void Reset()
    {
        base.Reset();
        _alreadyMovedAmount = Vector3.zero;
        if (_currentProjectile != null && _currentProjectile.gameObject.IsDestroyed() == false)
        {
            _alreadyMovedAmount = Vector3.zero;
            Destroy(_currentProjectile.gameObject);
        }
    }
    #endregion
}
