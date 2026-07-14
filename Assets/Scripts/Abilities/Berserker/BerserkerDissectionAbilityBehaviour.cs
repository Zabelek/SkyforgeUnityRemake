using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

public class BerserkerDissectionAbilityBehaviour : MovingAbilityBehaviour
{
    private const int BURNING_CHAIN_CHANCE_PERCENT = 30;

    #region Variables
    [Header("Dissaction")]
    [SerializeField] private GameplayEffectBehaviour _slow;
    [SerializeField] private GameplayEffectBehaviour _slowCooldown;
    [SerializeField] private ParticleSystem _particles;
    [SerializeField] private GameplayEffectBehaviour _burningChain;
    private ParticleSystem _currentParticles;
    //timers
    [SerializeField] private float _effectTimerStartBase, _chainswordSoundTimerBase;
    private float _effectTimerStart, _chainswordSoundTimer;
    private bool _chainswordSoundPlayed;
    #endregion

    #region Methods
    public override void Init()
    {
        //In the base class Reset is called, so all values that have to be reset, place there.
        base.Init();
        _chainswordSoundPlayed = false;
    }
    public override void UpdateAbility(CharacterBehaviour performer, HeroClassBehaviour heroClass)
    {
        base.UpdateAbility(performer, heroClass);
        if (!Finishing)
        {
            if (_performingTimer > _chainswordSoundTimer && _chainswordSoundPlayed == false)
            {
                ((HeroBehaviour)performer).EquippedWeapon?.PlaySound("Chainsword_Hit");
                ((HeroBehaviour)performer).EquippedWeapon?.PlaySound("Chainsword_Woosh");
                _chainswordSoundPlayed = true;
            }
            UpdateCustomMove(performer);
            //The following code executes once, so the 0.5f value doesn't have to be changed.
            if (_performingTimer > _effectTimerStart && _performingTimer < _effectTimerStart + 0.5f && _currentParticles == null)
            {
                _currentParticles = Instantiate(_particles, performer.transform);
                _currentParticles.transform.SetParent(null);
            }
        }
    }
    public override void LaunchAbility(CharacterBehaviour performer)
    {
        base.LaunchAbility(performer);
        performer.PlayAnimation("Dissection", true);
    }
    public override void PerformHit(CharacterBehaviour performer)
    {
        base.PerformHit(performer);
        Physics.SyncTransforms();
        var potentialCasualities = Physics.OverlapSphere(performer.transform.position, 5);
        var collider = Instantiate(transform.Find("Collider").GetComponent<Collider>(), performer.transform, false);
        int casualityAmount = 0;
        if(collider!=null)
        {
            foreach(var casuality in potentialCasualities)
            {
                if (casuality == collider || casuality == performer.GetComponent<Collider>())
                    continue;
                if (casuality.bounds.Intersects(collider.bounds) && CharacterBehaviour.FindEnemyCharacterInCollider(casuality, performer, out var character))
                {
                    var damage = CalculateDamage(new Damage(performer, performer.GetEffectiveDamage(), false, false), performer.GetEffectiveCriticalChance());
                    character.TakeDamage(damage);
                    if (!character.GetActiveEffects().Any(eff => eff.EffectSO.Name == _slowCooldown.EffectSO.Name))
                    {
                        character.AddEffect(_slow);
                        character.AddEffect(_slowCooldown);
                    }
                    casualityAmount++;
                }
            }
        }
        if(casualityAmount>0)
        {
            if (performer is HeroBehaviour && ((HeroBehaviour)performer).GetHeroClass() is BerserkerClassBehaviour && ((HeroBehaviour)performer).GetPerk("Base_Berserker_BurningChain")?.Enabled == true)
            {
                if (Random.Range(1, 100) <= BURNING_CHAIN_CHANCE_PERCENT)
                {
                    performer.AddEffect(_burningChain);
                }
            }
        }    
        if (TryGetComponent<CinemachineImpulseSource>(out CinemachineImpulseSource imp))
        {
            imp.GenerateImpulse();
        }
        Destroy(collider.gameObject);
    }
    protected override void CalculateAttackSpeed(float speed)
    {
        base.CalculateAttackSpeed(speed);
        _customMoveTimerStart = _customMoveTimerStartBase / speed;
        _customMoveTimerEnd = _customMoveTimerEndBase / speed;
        _chainswordSoundTimer = _chainswordSoundTimerBase / speed;
        _effectTimerStart = _effectTimerStartBase / speed;
    }
    public override void Reset()
    {
        base.Reset();
        _chainswordSoundPlayed = false;
    }
    #endregion
}
