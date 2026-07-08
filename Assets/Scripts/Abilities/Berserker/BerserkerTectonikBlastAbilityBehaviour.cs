using Unity.Cinemachine;
using UnityEngine;

public class BerserkerTectonikBlastAbilityBehaviour : AbilityBehaviour
{
    #region Variables
    [Header("Tectonic Blast")]
    [SerializeField] private BerserkerTectonicBlastVisualGroup _visuals;
    private bool _groundImpactSoundPlayed, _alreadySpawnedVisuals;
    //timers
    [SerializeField]  private float _effectTimerStartBase, _groundImpactSoundTimerBase;
    private float _effectTimerStart, _groundImpactSoundTimer;
    #endregion

    #region Methods
    public override void Init()
    {
        //In the base class Reset is called, so all values that have to be reset, place there.
        base.Init();
        _producesDefaultTrace = false;
    }
    public override void UpdateAbility(CharacterBehaviour performer, HeroClassBehaviour heroClass)
    {
        base.UpdateAbility(performer, heroClass);
        if (!Finishing)
        {
            if (_performingTimer > _groundImpactSoundTimer && _groundImpactSoundPlayed == false)
            {
                ((HeroBehaviour)performer).EquippedWeapon?.PlaySound("Tectonic_Blast");
                _groundImpactSoundPlayed = true;
            }
            if (_performingTimer > _effectTimerStart && _performingTimer < _effectTimerStart + 0.5f && _alreadySpawnedVisuals == false)
            {
                var part = Instantiate(_visuals, performer.transform);
                part.transform.SetParent(null);
                part.gameObject.SetActive(true);
                _alreadySpawnedVisuals = true;
            }
        }
    }
    public override void LaunchAbility(CharacterBehaviour performer)
    {
        base.LaunchAbility(performer);
        performer.PlayAnimation("TectonicBlast", true);
    }
    protected override void CalculateAttackSpeed(float speed)
    {
        base.CalculateAttackSpeed(speed);
        _effectTimerStart = _effectTimerStartBase;
        _groundImpactSoundTimer = _groundImpactSoundTimerBase;
    }
    public override void PerformHit(CharacterBehaviour performer)
    {
        base.PerformHit(performer);
        Physics.SyncTransforms();
        var potentialCasualities = Physics.OverlapSphere(performer.transform.position, 5);
        var collider = Instantiate(transform.Find("Collider").GetComponent<Collider>(), performer.transform, false);
        int casualityAmount = 0;
        if (collider != null)
        {
            foreach (var casuality in potentialCasualities)
            {
                if (casuality == collider || casuality == performer.GetComponent<Collider>())
                    continue;
                if (casuality.bounds.Intersects(collider.bounds) && CharacterBehaviour.FindEnemyCharacterInCollider(casuality, performer, out var character))
                {
                    var damage = CalculateDamage(new Damage(performer, (performer.GetEffectiveDamage()), false, false), performer.GetEffectiveCriticalChance());
                    character.TakeDamage(damage);
                    casualityAmount++;
                }
            }
        }
        if (TryGetComponent<CinemachineImpulseSource>(out CinemachineImpulseSource imp))
        {
            imp.GenerateImpulse();
        }
        if (casualityAmount > 2)
        {
            performer.Stats.CurrentMana +=100;
            for (int i=3; i<casualityAmount; i++)
            {
                performer.Stats.CurrentMana += 50;
                if (i >= 8)
                    break;
            }
        }
        Destroy(collider.gameObject);
    }
    public override void Reset()
    {
        base.Reset();
        _groundImpactSoundPlayed = false;
        _alreadySpawnedVisuals = false;
    }
    #endregion
}
