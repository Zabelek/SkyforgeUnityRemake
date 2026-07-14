using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

public class BerserkerFastAttackAbilityBehaviour : MovingAbilityBehaviour
{
    private const int BURNING_CHAIN_CHANCE_PERCENT = 25;

    #region Variables
    [Header("Fast Attack")]
    [SerializeField] private GameplayEffectBehaviour _stun;
    [SerializeField] private GameplayEffectBehaviour _stunCooldown;
    [SerializeField] private ParticleSystem _particles;
    [SerializeField] private GameplayEffectBehaviour _burningChain;
    private ParticleSystem _currentParticles;
    private bool _chainswordSoundPlayed;
    //timers
    [SerializeField] private float _effectTimerStartBase, _chainswordSoundTimerBase;
    private float _effectTimerStart, _chainswordSoundTimer;
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
            if (_performingTimer > _effectTimerStart && _performingTimer < _effectTimerStart + 0.5f && _currentParticles == null)
            {
                _currentParticles = Instantiate(_particles, performer.transform);
            }
        }
    }
    public override void LaunchAbility(CharacterBehaviour performer)
    {
        base.LaunchAbility(performer);
        performer.PlayAnimation("FastAttack", true);
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
                    if (!character.GetActiveEffects().Any(eff => eff.EffectSO.Name == _stunCooldown.EffectSO.Name))
                    {
                        if (performer is PlayerBehaviour && ((PlayerBehaviour)performer).SelectedCharacter == character)
                        {
                            character.AddEffect(_stunCooldown);
                            character.AddEffect(_stun);
                        }
                        else
                        {
                            damage.Amount = damage.Amount / 3;
                        }
                    }
                    else
                    {
                        damage.Amount = damage.Amount / 2;
                    }
                    character.TakeDamage(damage);
                    casualityAmount++;
                }
            }
        }
        if (TryGetComponent<CinemachineImpulseSource>(out CinemachineImpulseSource imp))
        {
            imp.GenerateImpulse();
        }
        if (casualityAmount>0)
        {
            if (casualityAmount > 0)
            {
                performer.Stats.CurrentMana -= AbilitySO.ManaCost;
            }
            if (performer is HeroBehaviour && ((HeroBehaviour)performer).GetHeroClass() is BerserkerClassBehaviour && ((HeroBehaviour)performer).GetPerk("Base_Berserker_BurningChain")?.Enabled == true)
            {
                if (Random.Range(1, 100) < BURNING_CHAIN_CHANCE_PERCENT)
                {
                    performer.AddEffect(_burningChain);
                }
            }
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
