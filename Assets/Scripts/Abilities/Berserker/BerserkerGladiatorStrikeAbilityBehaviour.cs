using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class BerserkerGladiatorStrikeAbilityBehaviour : AbilityBehaviour
{
    #region Variables
    private bool _leap;
    private CharacterBehaviour _casuality;
    [Header("Gladiator Strike")]
    [SerializeField] private float _archHeight = 1.5f;
    private Vector3 _destinationVector, _initialPlayerPosition, _alreadyMovedAmount;
    private bool _chainswordSoundPlayed, _groundImpactSoundPlayed, _animationTriggered, _secondHitPerformed, _thirdHitPerformed;
    [SerializeField] private ParticleSystem _particlesBase;
    private ParticleSystem _currentParticles;
    private List<Damage> _currentDamages;
    //timers
    [SerializeField]
    private float _chainswordSoundTimerBase, _groundImpactSoundTimerBase, _effectTimerStartBase, _startLeapTimerBase, _untilHitTimerBaseLeap;
    private float _chainswordSoundTimer, _groundImpactSoundTimer, _effectTimerStart, _startLeapTimer;
    #endregion

    #region Methods
    public override void Init()
    {
        base.Init();
        _currentDamages = new();
    }
    public override void UpdateAbility(CharacterBehaviour performer, HeroClassBehaviour heroClass)
    {
        base.UpdateAbility(performer, heroClass);
        if (!Finishing)
        {
            if (_leap)
            {
                if (_animationTriggered == false && performer is PlayerBehaviour)
                {
                    DetermineDestinationVector(performer);
                    _initialPlayerPosition = performer.transform.position;
                    var fowardDir = new Vector3(_destinationVector.x, performer.transform.position.y, _destinationVector.z);
                    performer.FaceTheTarget((fowardDir - performer.transform.position).normalized);
                    performer.PlayAnimation("GladiatorLeap", true);
                    _animationTriggered = true;
                }
                if (_performingTimer >= _startLeapTimer && _performingTimer < _untilHitTimer)
                {
                    if (performer.GetComponent<Rigidbody>()?.isKinematic == false)
                    {
                        performer.GetComponent<Rigidbody>().isKinematic = true;
                    }
                    var timeMod = (_performingTimer - _startLeapTimer) / (_untilHitTimer - _startLeapTimer);
                    var vec = Vector3.Lerp(_initialPlayerPosition, _destinationVector, timeMod);
                    vec -= _initialPlayerPosition;
                    vec.y += Mathf.Sin((timeMod * 1.1f) * Mathf.PI) * _archHeight;
                    vec = vec - _alreadyMovedAmount;
                    _alreadyMovedAmount += vec;
                    performer.MoveBySkill(vec, MovingAbilityBehaviour.MovingType.Raw);
                }
                if (_currentParticles == null && _performingTimer >= _effectTimerStart)
                {
                    _currentParticles = Instantiate(_particlesBase, performer.transform);
                }
                if (_performingTimer > _chainswordSoundTimer && _chainswordSoundPlayed == false)
                {
                    ((HeroBehaviour)performer).EquippedWeapon?.PlaySound("Chainsword_Hit");
                    ((HeroBehaviour)performer).EquippedWeapon?.PlaySound("Chainsword_Woosh");
                    _chainswordSoundPlayed = true;
                }
                if (_performingTimer > _groundImpactSoundTimer && _groundImpactSoundPlayed == false)
                {
                    ((HeroBehaviour)performer).EquippedWeapon?.PlaySound("Chainsword_Ground_Impact");
                    _groundImpactSoundPlayed = true;
                }
                if (_performingTimer >= _untilHitTimer)
                {
                    if (performer.GetComponent<Rigidbody>()?.isKinematic == true)
                    {
                        performer.GetComponent<Rigidbody>().isKinematic = false;
                    }
                }
            }
            else
            {
                if (_animationTriggered == false && performer is PlayerBehaviour)
                {
                    var fowardDir = new Vector3(_casuality.transform.position.x, performer.transform.position.y, _casuality.transform.position.z);
                    performer.FaceTheTarget( (fowardDir - performer.transform.position).normalized);
                    performer.PlayAnimation("GladiatorStrike1", true);
                    _animationTriggered = true;
                }
                if (_performingTimer > _untilHitTimer + 0.1f && !_secondHitPerformed)
                {
                    PerformHit(performer);
                    _secondHitPerformed = true;
                    try { ((HeroBehaviour)performer).EquippedWeapon?.PlaySound("Gladiator_Strike"); } catch { }
                }
                if (_performingTimer > _untilHitTimer + 0.2f && !_thirdHitPerformed)
                {
                    PerformHit(performer);
                    _thirdHitPerformed = true;
                }
            }
        }
    }
    public override void LaunchAbility(CharacterBehaviour performer)
    {
        base.LaunchAbility(performer);
        float enemyWidth = 0;
        try { enemyWidth += _casuality.GetComponentInChildren<Collider>().transform.localScale.x; } catch { }
        if ((performer.transform.position - _casuality.transform.position).magnitude < 3.5f + enemyWidth)
        {
            _leap = false;
        }
        else
        {
            _leap = true;
        }
        //needs to be executed second time because LaunchAbility is performed before _leap is set
        CalculateAttackSpeed(performer.GetEffectiveAttackSpeed());

    }
    public override bool CheckPerformAvailability(CharacterBehaviour player)
    {
        var ret = base.CheckPerformAvailability(player);
        if(ret)
            ret = FindCasuality(player);
        return ret;
    }
    protected override void CalculateAttackSpeed(float speed)
    {
        //it's combo, but in this case attack speed isn't calculated, so base.CalculateAttackSpeed(speed) would break things.
        _attackTimerNext = AbilitySO.AttackTimerNext;
        _attackTimerMax = AbilitySO.AttackTimerMax;
        _untilHitTimer = _untilHitTimerBase;
        _startLeapTimer = _startLeapTimerBase;
        _effectTimerStart = _effectTimerStartBase;
        _chainswordSoundTimer = _chainswordSoundTimerBase;
        _groundImpactSoundTimer = _groundImpactSoundTimerBase;
        if(_leap)
        {
            _untilHitTimer = _untilHitTimerBaseLeap;
        }
    }
    public bool FindCasuality(CharacterBehaviour player)
    {
        if (player is PlayerBehaviour)
        {
            var playerplayer = (PlayerBehaviour)player;
            if (playerplayer.SelectedCharacter != null)
            {
                if (playerplayer.Faction.FactionType != playerplayer.SelectedCharacter.Faction.FactionType && !playerplayer.Faction.Allies.Contains(playerplayer.SelectedCharacter.Faction.FactionType))
                {
                    if ((player.transform.position - playerplayer.SelectedCharacter.transform.position).magnitude < 24)
                    {
                        _casuality = playerplayer.SelectedCharacter;
                        return true;
                    }
                }
            }
        }
        return false;
    }
    private void DetermineDestinationVector(CharacterBehaviour player)
    {
        if ((_casuality.transform.position - player.transform.position).magnitude > 24)
        {
            var targetDest = (player.transform.position + (_casuality.transform.position - player.transform.position).normalized * 24);
            float enemyWidth = 0;
            try { enemyWidth += _casuality.GetComponentInChildren<Collider>().transform.localScale.x; } catch { }
            if(enemyWidth>0)
            {
                targetDest -= targetDest.normalized * enemyWidth;
            }
            if (NavMesh.SamplePosition(targetDest, out NavMeshHit newhit, 5f, LayerMask.GetMask("Default")))
            {
                _destinationVector = new Vector3(newhit.position.x, newhit.position.y + 0.5f, newhit.position.z);
                return;
            }
            _destinationVector = player.transform.position;
        }
        else
        {
            var targetDest = _casuality.transform.position;
            float enemyWidth = 0;
            try { enemyWidth += _casuality.GetComponentInChildren<Collider>().transform.localScale.x; } catch { }
            if (enemyWidth > 0)
            {
                targetDest -= (targetDest - player.transform.position).normalized * enemyWidth;
            }
            if (NavMesh.SamplePosition(targetDest, out NavMeshHit newhit, 5f, LayerMask.GetMask("Default")))
            {
                _destinationVector = new Vector3(newhit.position.x, newhit.position.y + 0.5f, newhit.position.z); 
                return;
            }
            _destinationVector = player.transform.position;
        }
    }
    public override void PerformHit(CharacterBehaviour player)
    {
        if(_leap || _hitPerformed == false)
        {
            OnAbilityHit?.Invoke(this, EventArgs.Empty);
        }
        _hitPerformed = true;
        var rageIncarnateEffect = player.GetActiveEffects().FirstOrDefault(e => e.EffectSO.Name == "Rage Incarnate");
        float rageIncarnateMultiplier = 1;
        if(rageIncarnateEffect!=null)
        {
            rageIncarnateMultiplier =  1 + ((float)(rageIncarnateEffect.Stacks) / 300);
        }
        if (_casuality != null && !_casuality.IsDead)
        {
            if(_leap)
            {
                var damage = CalculateDamage(new Damage(player, (int)(player.GetEffectiveDamage() * 3 * rageIncarnateMultiplier), false, false), player.GetEffectiveCriticalChance());
                _casuality.TakeDamage(damage);
                if(rageIncarnateEffect != null)
                {
                    player.Stats.CurrentMana += 50;
                }
            }
            else
            {
                var oldDamage = _currentDamages?.FirstOrDefault(dam => dam == _casuality.LastDamage);
                if (oldDamage != null)
                {
                    var newDamageAmount = CalculateDamageForMultishot(new Damage(player, (int)(player.GetEffectiveDamage() * rageIncarnateMultiplier), false, false), oldDamage);
                    oldDamage.AddMultishot(newDamageAmount);
                    _casuality.TakeDamage(oldDamage);
                }
                else
                {
                    _currentDamages = new();
                    var damage = CalculateDamage(new Damage(player, (int)(player.GetEffectiveDamage() * rageIncarnateMultiplier), false, false), player.GetEffectiveCriticalChance());
                    _casuality.TakeDamage(damage);
                    _currentDamages.Add(damage);
                    if (rageIncarnateEffect != null)
                    {
                        player.Stats.CurrentMana += 50;
                    }
                }
            }
        }
    }
    protected Damage CalculateDamageForMultishot(Damage damage, Damage oldDamage)
    {
        damage.Amount = (int)(damage.Amount * AbilitySO.DamageMultiplier);
        if (oldDamage.Critical)
        {
            damage.Amount = damage.Amount * 2;
            damage.Critical = true;
        }
        damage.Amount = (int)(damage.Amount * ExternalDamageMultiplier);
        return damage;
    }
    public override void Reset()
    {
        base.Reset();
        _leap = false;
        _chainswordSoundPlayed = false;
        _groundImpactSoundPlayed = false;
        _animationTriggered = false;
        _alreadyMovedAmount = Vector3.zero;
        _secondHitPerformed = false;
        _thirdHitPerformed = false;
        _currentDamages = null;
    }
    #endregion
}
