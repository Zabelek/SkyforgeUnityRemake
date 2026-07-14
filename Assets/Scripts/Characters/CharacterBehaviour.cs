using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CharacterBehaviour : MonoBehaviour
{
    //When a character is walking, it's speed is multiplied by this value
    public const float WALKING_SPEED_MOD = 0.3f;
    public const float INSTANT_DEATH_HEIGHT = -150;

    #region MainVariables
    public event EventHandler OnHurt;
    public event EventHandler OnDeath;
    public event EventHandler OnResurrect;
    public event EventHandler OnCutscene;
    [Header("Character Related Variables")]
    [Tooltip("Character Scriptable Object with all basic information")]
    [SerializeField] public CharacterBaseSO CharacterSO;
    [Tooltip("Script responsible for updating visual effects of receiving damage/healing")]
    [SerializeField] private VisualHitReceiver _visualHitReceiver;
    [Tooltip("Script responsible for changing meshes depending on equipped outfit")]
    [SerializeField] protected OutfitManager _outfitManager;
    [Tooltip("When you need a reference to any specific bone from the character's skeleton, you can use this array so that external scripts can access them")]
    public Transform[] RegisteredBones;
    [Tooltip("Characters play sounds via this component")]
    public SpeakingBehaviour SpeakingBehaviour;
    //handles effects applied to the character
    private EffectManager _effectManager;
    //animation controllers
    protected CharacterAnimationBehaviour _animationBehaviour;
    [Tooltip("Animator Controller that would be assigned to the character's AnimationBehaviour if no changes are done by any script")]
    public RuntimeAnimatorController DefaultAnimatorController;
    #endregion

    #region StatVariables
    private bool _canAct;
    private bool _canMove;
    [Tooltip("This overrides the name set in Character Base Scriptable Object, if not empty")]
    public string Name;
    public CharacterStats Stats { get; private set; }
    [Tooltip("Faction Scriptable Object attached to character")]
    public FactionSO Faction;
    public bool IsInCombat { get; protected set; }
    public bool IsDead { get; protected set; }
    public bool IsFalling { get; set; }
    public bool Selectable { get; set; }
    public bool CanBeDamaged { get; set; }
    #endregion

    #region VariablesRelatedToGameSystems
    //Previously received damage. Stored to check if the attack is continued or is it new damage source. Lasts for 3 seconds.
    public Damage LastDamage { get; protected set; }
    private float _LastDamageExpireTimer;
    //variables used for fading in after spawn or fading out after death
    private float _fadeOutTimer, _fadeInTimer;
    private static float _fadeOutTimerMax = 3;
    protected float _afterDeathTimer = 5;
    protected Collider[] _colliders;
    [Tooltip("Used mainly to modify alpha value of the characters at spawn or death. may be replaced in the future")]
    [SerializeField] private Renderer[] _renderers;
    //variables related to combat and out of combat behaviour
    private int _lastOffCombatHealAmount;
    private float _healTimer;
    public bool CombatStance { get; protected set; }
    public List<CharacterBehaviour> ActiveEnemies { get; set; }
    public class StartCombatEventArgs : EventArgs
    {
        public CharacterBehaviour Enemy;
        public bool FightProvokedByGroup;
        public StartCombatEventArgs(CharacterBehaviour enemy, bool fightProvokedByGroup)
        {
            Enemy = enemy;
            FightProvokedByGroup = fightProvokedByGroup;
        }
    }
    public event EventHandler<StartCombatEventArgs> OnCombatStartEvent;
    public event EventHandler OnCombatEndEvent;
    //healing orbs dropping
    private short _droppedHealingOrbsInTheFight;
    public event EventHandler OnHealingOrbDrop;
    private float _orbDroppingCollisionRadius;
    [Tooltip("By default, when the character appears in the scene, it fades in for a second. If you want to disable the fading animation, set this to false")]
    public bool FadedIntoScene = true;
    [Tooltip("If this variable is set to true, the character will be killed f they fall too low.")]
    public bool KillBelowScene = true;
    private Rigidbody _rigidbody;
    #endregion

    #region Mono
    protected virtual void Awake()
    {
        Stats = new();
        Stats.Reset(CharacterSO);
        Stats.CurrentHP = Stats.MaxHP;
        _colliders = GetComponentsInChildren<Collider>();
        var navAgent = GetComponent<NavMeshAgent>();
        if (navAgent != null)
            _orbDroppingCollisionRadius = navAgent.radius;
        else
            _orbDroppingCollisionRadius = 1;
        _effectManager = new EffectManager(this);
        ActiveEnemies = new();
        if(this is not PlayerBehaviour)
        {
            Stats.ModifyAccordingToDifficultyLevel();
        }
        _rigidbody = GetComponent<Rigidbody>();
    }
    protected virtual void Start()
    {
        _canMove = true;
        SetCanAct(true);
        IsDead = false;
        Selectable = true;
        CanBeDamaged = true;
        _fadeOutTimer = 0;
        _lastOffCombatHealAmount = 0;
        _healTimer = 1;
        if (FadedIntoScene)
            _fadeInTimer = _fadeOutTimerMax;
        else
            _fadeInTimer = 0;
        _droppedHealingOrbsInTheFight = 0;
        _effectManager.ClearEffects();
    }
    protected virtual void Update()
    {
        UpdateFadeInOut();
    }
    protected virtual void FixedUpdate()
    {
        _effectManager.UpdateEffects();
        if (_LastDamageExpireTimer > 0 && !IsDead)
        {
            _LastDamageExpireTimer -= Time.fixedDeltaTime;
            if (_LastDamageExpireTimer <= 0)
                LastDamage = null;
        }
        UpdateEnemyList();
        if (IsInCombat && ActiveEnemies.Count() == 0)
        {
            LeaveCombat();
        }
        UpdateOutOfCombatHealing();
        if(KillBelowScene && transform.position.y < INSTANT_DEATH_HEIGHT && !IsDead)
        {
            KillOffCombat();
        }
    }
    protected virtual void OnDestroy()
    {
        _effectManager.Destroy();
    }
    #endregion

    #region UpdateMethods
    private void UpdateFadeInOut()
    {
        if (IsDead && _afterDeathTimer > 0 && !(this is PlayerBehaviour || this is DestroyableObjectBehaviour))
        {
            _afterDeathTimer -= Time.deltaTime;
            if (_afterDeathTimer < 0)
            {
                _fadeOutTimer = _fadeOutTimerMax;
            }
        }
        if (_fadeOutTimer != 0)
        {
            _fadeOutTimer -= Time.deltaTime;
            foreach(var renderer in _renderers)
            {
                int FadeID = Shader.PropertyToID("_Alpha_Value");
                var block = new MaterialPropertyBlock();
                renderer.GetPropertyBlock(block);
                var value = _fadeOutTimer / _fadeOutTimerMax;
                if (value < 0)
                    value = 0;
                block.SetFloat(FadeID, value);
                renderer.SetPropertyBlock(block);
            }
        }
        if (_fadeOutTimer < 0)
        {
            Destroy(this.gameObject);
        }
        if (_fadeInTimer > 0)
        {
            _fadeInTimer -= Time.deltaTime;
            foreach(var renderer in _renderers)
            {
                int FadeID = Shader.PropertyToID("_Alpha_Value");
                var block = new MaterialPropertyBlock();
                renderer.GetPropertyBlock(block);
                var value = 1 - (_fadeInTimer / _fadeOutTimerMax);
                if (value > 1)
                    value = 1;
                block.SetFloat(FadeID, value);
                renderer.SetPropertyBlock(block);
            }
        }
    }
    private void UpdateEnemyList()
    {
        int listWritePosition = 0;
        for(int i = 0; i < ActiveEnemies.Count(); i++)
        {
            var enemy = ActiveEnemies[i];
            if(!enemy.IsDead)
                ActiveEnemies[listWritePosition++] = enemy;
        }
        ActiveEnemies.RemoveRange(listWritePosition, ActiveEnemies.Count() - listWritePosition);
    }
    private void UpdateOutOfCombatHealing()
    {
        if (!IsInCombat && Stats.CurrentHP < Stats.MaxHP && !IsDead)
        {
            _healTimer -= Time.fixedDeltaTime;
            if (_healTimer <= 0)
            {
                _healTimer = 1;
                Stats.CurrentHP += _lastOffCombatHealAmount;
                _lastOffCombatHealAmount = (int)(_lastOffCombatHealAmount * 1.2f);
            }
        }
    }
    #endregion

    #region RegularMethods
    public virtual void TakeDamage(Damage damage)
    {
        TakeDamage(damage, false);
    }
    public virtual void TakeDamage(Damage damage, bool cutsceneOverride)
    {
        //first check is to prevent characters from randomly dying during a cutscene
        if (Globals.Instance.IsCutscenePlaying == false || cutsceneOverride)
        {
            LastDamage = damage;
            _LastDamageExpireTimer = 3f;
            _effectManager.OnDamageTaken(damage);
            if (this is PlayerBehaviour)
            {
                SpeakingBehaviour.PerformHitSound(true);
            }
            else
            {
                SpeakingBehaviour?.PerformHitSound(false);
            }
            //This vector math is to display the hit effect slightly towards the one that dealt damage, if it's melee damage.
            //Range damage will be displayed in the center of the character anyway
            _visualHitReceiver?.GetHit(damage.Range, (damage.Source.transform.position).normalized * -1f);
            SpeakingBehaviour?.PerformHurtSound(1f);
            EnterCombat(damage.Source, false);
            damage.Source.EnterCombat(this, false);
            Stats.CurrentHP -= damage.Amount;
            OnHurt?.Invoke(this, EventArgs.Empty);
            if (Stats.CurrentHP <= 0)
            {
                Kill(damage.Source);
            }
            else
            {
                CheckDropHealingOrb();
            }
            if (IsDead)
            {
                OnDeath?.Invoke(this, EventArgs.Empty);
            }
        }
    }
    public virtual void TakeEmptyDamage()
    {

    }
    public virtual void Kill(CharacterBehaviour killer)
    {
        IsDead = true;
        _canMove = false;
        SetCanAct(false, false);
        Stats.CurrentHP = 0;
        if (_colliders.Length > 0)
        {
            foreach (var col in _colliders)
                col.enabled = false;
        }
        _effectManager.OnDeath(this);
        _effectManager.ClearEffects();
        if (TryGetComponent<Rigidbody>(out Rigidbody rig))
        {
            if (!rig.isKinematic)
            {
                rig.angularVelocity = Vector3.zero;
                rig.linearVelocity = Vector3.zero;
            }
            rig.isKinematic = true;
        }
        CheckDropHealingOrb();
        if(TryGetComponent<AIHandlerBehaviour>(out AIHandlerBehaviour aiHandler) == true)
        {
            aiHandler.enabled = false;
        }
        if(killer is PlayerBehaviour && SkyforgeLoader.CurrentProfile!=null)
        {
            SkyforgeLoader.CurrentProfile.Prestige += 69;
        }
    }
    public virtual void KillOffCombat()
    {
        Kill(this);
        _lastOffCombatHealAmount = (int)(Stats.MaxHP * 0.06f);
        _healTimer = 1;
        OnDeath?.Invoke(this, EventArgs.Empty);
    }
    public virtual void Resurrect()
    {
        IsDead = false;
        Stats.CurrentHP = 1;
        _canMove = true;
        SetCanAct(true, false);
        if (_colliders.Length > 0)
        {
            foreach (var col in _colliders)
                col.enabled = true;
        }
        if (TryGetComponent<Rigidbody>(out Rigidbody rig))
        {
            rig.angularVelocity = Vector3.zero;
            rig.linearVelocity = Vector3.zero;
            rig.isKinematic = false;
        }
        LeaveCombat();
        OnResurrect?.Invoke(this, EventArgs.Empty);
    }
    public virtual void Heal(int healAmount, bool effect)
    {
        if(IsDead == false)
        {
            healAmount = _effectManager.OnHealingReceived(healAmount);
            Stats.CurrentHP += healAmount;
            if (effect)
            {
                _visualHitReceiver.GetHeal();
                SpeakingBehaviour.PerformHealSound();
            }
        }       
    }
    //used with float from 0 to 1
    public void HealPercent(float percent, bool effect)
    {
        Heal((int)(Stats.MaxHP * percent), effect);
    }
    public void AddEffect(GameplayEffectBehaviour effect)
    {
        if(!IsDead)
        {
            if (CharacterSO.CCResistant)
            {
                if (effect.EffectSO.Types.Contains(EffectSO.EffectType.Fear) || effect.EffectSO.Types.Contains(EffectSO.EffectType.MoveAround)
                    || effect.EffectSO.Types.Contains(EffectSO.EffectType.Slow) || effect.EffectSO.Types.Contains(EffectSO.EffectType.Stun))
                {

                }
                else
                {
                    _effectManager.AddEffect(effect);
                }
            }
            else
            {
                _effectManager.AddEffect(effect);
            }
        }
    }
    public void RemoveEffect(GameplayEffectBehaviour effect)
    {
        _effectManager.RemoveEffect(effect);
    }
    public List<GameplayEffectBehaviour> GetActiveEffects()
    {
        return _effectManager.GetActiveEffects();
    }
    public virtual void MoveBySkill(Vector3 moveTarget, MovingAbilityBehaviour.MovingType movingType)
    {
        if (movingType == MovingAbilityBehaviour.MovingType.Raw)
            MovePosition(moveTarget, true);
        else
            MovePosition(transform.forward * moveTarget.magnitude, true);
    }
    public virtual void EnterCombat(CharacterBehaviour character, bool fightProvokedByGroup)
    {
        if(!character.IsDead)
        {
            if (!ActiveEnemies.Contains(character))
                ActiveEnemies.Add(character);
            if (!IsInCombat)
            {
                IsInCombat = true;
                OnCombatStartEvent?.Invoke(this, new StartCombatEventArgs(character, fightProvokedByGroup));
            }
        }
    }
    public virtual void LeaveCombat()
    {
        if(IsInCombat)
        {
            _lastOffCombatHealAmount = (int)(Stats.MaxHP * 0.06f);
            _healTimer = 1;
            IsInCombat = false;
            ActiveEnemies.Clear();
            _droppedHealingOrbsInTheFight = 0;
            OnCombatEndEvent?.Invoke(this, null);
        }
    }
    public void StopMovement()
    {
        if (TryGetComponent<AIHandlerBehaviour>(out var handler))
        {
            handler.StopMovement();
        }
    }
    public virtual void SetCanAct(bool canAct, bool ownAbilityDriven)
    {
        _canAct = canAct;
    }
    public virtual void SetCanAct(bool canAct)
    {
        SetCanAct(canAct, false);
    }
    public virtual void SetCanMove(bool canMove)
    {
        _canMove = canMove;
    }
    #endregion

    #region AnimationRelatedMethods
    public virtual void PerformEmote(object sender, EventArgs e)
    {
    }
    public virtual void PlayCutscene(RuntimeAnimatorController cutsceneController)
    {
        _animationBehaviour.SetController(cutsceneController);
        OnCutscene?.Invoke(this, EventArgs.Empty);
    }
    public virtual void EndCutscene()
    {
        _animationBehaviour.SetController(DefaultAnimatorController);
    }
    public virtual void SetAnimationBehaviour(CharacterAnimationBehaviour animationBehaviour)
    {
        _animationBehaviour = animationBehaviour;
        if(DefaultAnimatorController != null)
        {
            _animationBehaviour.SetController(DefaultAnimatorController);
        }
    }
    public virtual void PlayAnimation(string animationName, bool stopsMovement)
    {
        if (stopsMovement)
            _animationBehaviour?.StopMovementAnimation();
        _animationBehaviour?.TriggerAnimation(animationName);
    }
    public void PlayAnimation(string animationName)
    {
        PlayAnimation(animationName, false);
    }
    public virtual void SetAnimationState(string animationName, bool value)
    {
        _animationBehaviour?.SetAnimationBool(animationName, value);
    }
    public virtual void ResetAnimation()
    {
        _animationBehaviour?.ResetAnimation();
    }
    public virtual void CancelAllAbilities()
    {
        //for inherriting classes
    }
    #endregion

    #region StatGetters
    public virtual float GetEffectiveMovementSpeed()
    {
        return Stats.MovementSpeed * GetMovementSpeedModifiers();
    }
    public virtual float GetMovementSpeedModifiers()
    {
        float speedMod = 1;
        speedMod = _effectManager.GetMovementSpeedModifiers(speedMod);
        return speedMod;
    }
    public virtual int GetEffectiveDamage()
    {
        return (int)(Stats.BaseDamage * GetDamageModifiers());
    }
    public virtual float GetDamageModifiers()
    {
        float damageMod = 1;
        damageMod = _effectManager.GetDamageModifiers(damageMod);
        return damageMod;
    }
    public virtual float GetEffectiveAttackSpeed()
    {
        return Stats.AttackSpeed * GetAttackSpeedModifiers();
    }
    public virtual float GetAttackSpeedModifiers()
    {
        float speedMod = 1;
        speedMod = _effectManager.GetAttackSpeedModifiers(speedMod);
        return speedMod;
    }
    public virtual float GetEffectiveCriticalChance()
    {
        return Stats.CriticalChance + GetCriticalChanceModifiers();
    }
    public virtual float GetCriticalChanceModifiers()
    {
        float chanceMod = 0;
        chanceMod += _effectManager.GetCriticalChanceModifiers(chanceMod);
        return chanceMod;
    }
    public virtual int GetEffectiveCombatManaRegen()
    {
        return (int)(Stats.CombatManaRegen * GetCombatManaRegenModifiers());
    }
    public virtual float GetCombatManaRegenModifiers()
    {
        float regenMod = 1;
        regenMod = _effectManager.GetCombatManaRegenModifiers(regenMod);
        return regenMod;
    }
    private void CheckDropHealingOrb()
    {
        if(CharacterSO.Category.DropsHealingOrbs)
        {
            if(CharacterSO.Category.HealthBarsAmount <= 1 && IsDead && _droppedHealingOrbsInTheFight == 0)
            {
                DropHealingOrb();
            }
            //in case it's multiple health bar enemy, it should drop one after each bar destruction (only first time in a battle. Regenerated bar won't drop the orb again
            else if(CharacterSO.Category.HealthBarsAmount > 1)
            {
                float currentHpPercent = (float)this.Stats.CurrentHP / (float)this.Stats.MaxHP;
                while (CharacterSO.Category.HealthBarsAmount - _droppedHealingOrbsInTheFight > currentHpPercent * CharacterSO.Category.HealthBarsAmount+1)
                {
                    DropHealingOrb();
                }
                if(IsDead)
                {
                    DropHealingOrb();
                }
            }
        }
    }
    private void DropHealingOrb()
    {
        _droppedHealingOrbsInTheFight++;
        //by dewfault it needs to be anything greater than zero (preferably small amount)
        float dropRadius = 0.2f;
        if (_orbDroppingCollisionRadius>0)
        {
            dropRadius = _orbDroppingCollisionRadius;
        }
        //orb will be spawned and gently pushed in a random direction from the enemy
        Vector3 dir = new Vector3(UnityEngine.Random.Range(-1f,1f), 0, UnityEngine.Random.Range(-1f, 1f));
        if(dir.x == 0 && dir.y == 0)
        {
            dir += new Vector3(0.1f, 0, 0.1f);
        }
        dir = dir.normalized;
        var spawnPoint = this.transform.position + (dropRadius * dir) + new Vector3(0, _orbDroppingCollisionRadius/2+1, 0);
        Instantiate(Globals.Instance.HealingOrbBase, spawnPoint, this.transform.rotation).GetComponent<Rigidbody>().AddForce(dir * 8, ForceMode.Impulse);
        OnHealingOrbDrop?.Invoke(this, EventArgs.Empty);
    }
    public virtual bool CanAct()
    {
        //As for now, effects set canAct every tick, so it's not necessary to scan through them to check if any are stuns. This may change in the fututre
        return _canAct;
        if (_canAct)
            return _effectManager.CanAct();
        else
            return false;
    }
    public virtual bool CanMove()
    {
        //As for now, effects set canMove every tick, so it's not necessary to scan through them to check if any are stuns. This may change in the fututre
        return _canMove;
        if (_canMove)
            return _effectManager.CanMove();
        else
            return false;
    }
    #endregion

    #region RigidbodyMethods
    public void MovePosition(Vector3 moveTargetPosition, bool addToCurrent)
    {
        if (_rigidbody != null)
        {
            if(addToCurrent)
                _rigidbody.MovePosition(_rigidbody.position + moveTargetPosition);
            else
                _rigidbody.MovePosition(moveTargetPosition);
        }
        else
        {
            if (addToCurrent)
                this.transform.position = this.transform.position + moveTargetPosition;
            else
                this.transform.position = moveTargetPosition;
        }
    }
    public void MovePosition(Vector3 moveTargetPosition)
    {
        MovePosition(moveTargetPosition, false);
    }
    public void MoveRotation(Vector3 eulerAngles, bool addToCurrent)
    {
        if (_rigidbody != null)
        {
            if (addToCurrent)
                _rigidbody.MoveRotation(Quaternion.Euler(_rigidbody.rotation.eulerAngles + eulerAngles));
            else
                _rigidbody.MoveRotation(Quaternion.Euler(eulerAngles));
        }
        else
        {
            if (addToCurrent)
                this.transform.eulerAngles = this.transform.eulerAngles + eulerAngles;
            else
                this.transform.position = eulerAngles;
        }
    }
    public void MoveRotation(Vector3 eulerAngles)
    {
        MoveRotation(eulerAngles, false);
    }
    public void FaceTheTarget(Vector3 target)
    {
        
        if (_rigidbody != null)
        {
            Quaternion targetRotation = Quaternion.LookRotation(target);
            _rigidbody.MoveRotation(targetRotation);
        }
        else
        {
                this.transform.forward = target;
        }
    }
    #endregion

    #region GlowSetters
    public void SetGlow(float alpha, Color color, float value)
    {
        _visualHitReceiver.SetGlow(alpha, color, value);
    }
    public void SetGlow(float alpha, Color color)
    {
        _visualHitReceiver.SetGlow(alpha, color);
    }
    public void SetGlow(float alpha)
    {
        _visualHitReceiver.SetGlow(alpha);
    }
    public void SetDamageGlow(float alpha, bool add)
    {
        _visualHitReceiver.SetDamageGlow(alpha, add);
    }
    #endregion

    #region StaticMethods
    public static bool FindEnemyCharacterInCollider(Collider collider, CharacterBehaviour exclCharacter, out CharacterBehaviour character)
    {
        if (collider.tag == "Hit_Collider")
        {
            character = collider.GetComponent<CharacterBehaviour>();
            if (character == null)
                character = collider.GetComponentInParent<CharacterBehaviour>();
            if (character != null && character != exclCharacter && character.CanBeDamaged && character.Faction.FactionType != exclCharacter.Faction.FactionType &&
                !exclCharacter.Faction.Allies.Contains(character.Faction.FactionType))
                return true;
        }
        character = null;
        return false;
    }
    public static bool FindCharacterInCollider(Collider collider, CharacterBehaviour exclCharacter, out CharacterBehaviour character)
    {
        if (collider.tag == "Hit_Collider")
        {
            character = collider.GetComponent<CharacterBehaviour>();
            if (character == null)
                character = collider.GetComponentInParent<CharacterBehaviour>();
            if (character != null && character != exclCharacter && character.CanBeDamaged)
                return true;
        }
        character = null;
        return false;
    }
    public static bool FindCharacterInCollider(Collider collider, out CharacterBehaviour character)
    {
        if (collider.tag == "Hit_Collider")
        {
            character = collider.GetComponent<CharacterBehaviour>();
            if (character == null)
                character = collider.GetComponentInParent<CharacterBehaviour>();
            if (character != null)
                return true;
        }
        character = null;
        return false;
    }
    #endregion
}