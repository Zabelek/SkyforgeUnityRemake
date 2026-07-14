using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class PlayerBehaviour : HeroBehaviour
{
    #region Variables
    [SerializeField] private PlayerInputBehaviour _inputBehaviour;
    [SerializeField] private Camera _camera;
    //to remove once equipment system is introduced
    [SerializeField] private WeaponSO _debugWeaponSlot;
    [SerializeField] private GameObject _armor;
    [SerializeField] private GameObject _hood;
    public event EventHandler OnPlayerRessurected;
    //emotes
    public EmoteSO DebugSleepEmote;
    private bool _emoteInterrupted;
    private EmoteSO _currentlyPerformedEmote;
    private float _emoteDelayTimer;
    public bool IsRunning { get; private set; }
    //selection related logic
    public CharacterBehaviour SelectedCharacter { get; set; }
    //placeholder for companion. To remove once companion system is introduced
    public int companionDamage = 5;
    public event EventHandler OnCompanionAttack;
    public float CompanionCharge;
    public float CompanionChargeMax;
    //dash
    public event EventHandler OnDash;
    public float DashCharge;
    public float DashChargeMax;
    public Vector3 LastMovementDirection;
    public float LastMovementDirectionExpire;
    public event EventHandler OnFinisher;
    #endregion

    #region Mono
    protected new void Awake()
    {
        base.Awake();
        EquipWeapon(Instantiate(_debugWeaponSlot.GetMesh().GetComponent<WeaponBehaviour>()));
        CombatStance = false;
        if(_inputBehaviour!= null)
        {
            _inputBehaviour.OnDrawAction += ChangeWeaponOutState;
            _inputBehaviour.OnLeftClickAction += PerformLeftCombo;
            _inputBehaviour.OnRightClickAction += PerformRightCombo;
            _inputBehaviour.OnRightHoldReleaseAction += PerformRightRelease;
            _inputBehaviour.OnEmoteClickAction += PerformEmote;
            _inputBehaviour.OnAbility1Action += PerformAbility1;
            _inputBehaviour.OnAbility2Action += PerformAbility2;
            _inputBehaviour.OnAbility3Action += PerformAbility3;
            _inputBehaviour.OnAbility4Action += PerformAbility4;
            _inputBehaviour.OnAbility5Action += PerformAbility5;
            _inputBehaviour.OnAbility6Action += PerformAbility6;
            _inputBehaviour.OnAbility7Action += PerformAbility7;
            _inputBehaviour.OnAbility8Action += PerformAbility8;
            _inputBehaviour.OnAbility9Action += PerformAbility9;
            _inputBehaviour.OnDashClickAction += PerformDash;
            _inputBehaviour.OnInteractionAction += Interact;
        }
        _emoteInterrupted = false;
        _emoteDelayTimer = 0;
        //SyncPerks(false);
        //ChangeClass(GetHeroClass());
    }
    protected override void Start()
    {
        base.Start();
        //_outfitManager.EquipOutfit(_armor.GetComponent<OutfitBehaviour>());
        //_outfitManager.EquipOutfit(_hood.GetComponent<OutfitBehaviour>());
        ChangeClass(GetHeroClass());
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        HandleMovement();
        UpdateCompanion();
        UpdateDash();
        UpdateEmoteState();
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        EquippedWeapon.Unequip(this);
    }
    #endregion

    #region EventHandlers
    private void Interact(object sender, EventArgs e)
    {
        //cutscene and menu checking will better be moved to the imput manager later
        if (Globals.Instance.IsMenuOpen == false && Globals.Instance.IsCutscenePlaying == false)
        {
            if (CanAct())
            {
                if (!IsDead && GetHeroClass().TryPerformEscapeAbility())
                {
                }
                else if (GetHeroClass().TryPerformFinisher())
                {
                    OnFinisher?.Invoke(this, EventArgs.Empty);
                }
            }
            if (!IsDead && GetHeroClass().TryPerformEscapeAbility())
            {

            }
        }
        if (IsDead)
        {
            OnPlayerRessurected?.Invoke(this, EventArgs.Empty);
        }
    }
    private void PerformAbility1(object sender, EventArgs e)
    {
        PerformAnyAbility(0);
    }
    private void PerformAbility2(object sender, EventArgs e)
    {
        PerformAnyAbility(1);
    }
    private void PerformAbility3(object sender, EventArgs e)
    {
        PerformAnyAbility(2);
    }
    private void PerformAbility4(object sender, EventArgs e)
    {
        PerformAnyAbility(3);
    }
    private void PerformAbility5(object sender, EventArgs e)
    {
        PerformAnyAbility(4);
    }
    private void PerformAbility6(object sender, EventArgs e)
    {
        PerformAnyAbility(5);
    }
    private void PerformAbility7(object sender, EventArgs e)
    {
        PerformAnyAbility(6);
    }
    private void PerformAbility8(object sender, EventArgs e)
    {
        PerformAnyAbility(7);
    }
    private void PerformAbility9(object sender, EventArgs e)
    {
        PerformAnyAbility(8);
    }
    private void PerformAnyAbility(int abilityNumber)
    {
        if (GetHeroClass().CurrentStance.GetAbilityToPerform(GetHeroClass().TranslateAbilityNumbers(abilityNumber), out AbilityBehaviour ability))
        {
            if (CanAct())
            {
                if(!GetHeroClass().TryPerformEscapeAbility(abilityNumber))
                {
                    if (GetHeroClass() != null)
                    {
                        if (!CombatStance)
                            ChangeWeaponOutState(this, EventArgs.Empty);
                        else if (GetHeroClass().TryPerformAbility(abilityNumber))
                        {
                        }
                    }
                }
            }
            else if (ability is EscapeAbilityBehaviour)
            {
                if (!CombatStance)
                    ChangeWeaponOutState(this, EventArgs.Empty);
                else if (GetHeroClass().TryPerformEscapeAbility(abilityNumber))
                {
                }
            }
        }
    }
    //For now it's just sleep emote. Will be changed once the emote system is introduced
    public override void PerformEmote(object sender, EventArgs e)
    {
        if (CanAct())
        {
            if (!CombatStance || DebugSleepEmote.CombatEnabled)
            {
                _emoteInterrupted = false;
                SetCanMove(false);
                _animationBehaviour.PerformEmote(DebugSleepEmote);
                _currentlyPerformedEmote = DebugSleepEmote;
            }
            else
            {
                _emoteInterrupted = true;
                _animationBehaviour.StopEmote();
            }
        }
        else if(_currentlyPerformedEmote != null)
        {
            _emoteInterrupted = true;
            _animationBehaviour.StopEmote();
        }
    }
    private void PerformRightRelease(object sender, EventArgs e)
    {
        if (Globals.Instance.IsMenuOpen == false && Globals.Instance.IsCutscenePlaying == false)
        {
            GetHeroClass().RightReleasePerformed();
        }
    }
    private void PerformDash(object sender, EventArgs e)
    {
        if (Globals.Instance.IsMenuOpen == false && Globals.Instance.IsCutscenePlaying == false && _currentlyPerformedEmote == null && CanDash)
        {
            if (!CombatStance)
                ChangeWeaponOutState(this, EventArgs.Empty);
            if (GetHeroClass().TryPerformDash())
                OnDash?.Invoke(this, EventArgs.Empty);
        }
    }
    private void PerformLeftCombo(object sender, EventArgs e)
    {
        if (CanAct() == true)
        {
            if (!CombatStance)
                ChangeWeaponOutState(this, EventArgs.Empty);
            else
                TryPerformNextCombo(false);
        }
    }
    private void PerformRightCombo(object sender, EventArgs e)
    {
        if (CanAct() == true)
        {
            if (!CombatStance)
                ChangeWeaponOutState(this, EventArgs.Empty);
            else
                TryPerformNextCombo(true);
        }
    }
    public bool GetAbilityPressed(int abilityNumber)
    {
        return _inputBehaviour.GetAbilityPressed(abilityNumber);
    }
    #endregion

    #region Methods
    private void UpdateCompanion()
    {
        if (CompanionChargeMax != 0)
        {
            if (CompanionCharge < CompanionChargeMax)
            {
                CompanionCharge += Time.fixedDeltaTime;
                if (CompanionCharge > CompanionChargeMax)
                    CompanionCharge = CompanionChargeMax;
            }
        }
    }
    private void UpdateDash()
    {
        if (DashChargeMax != 0)
        {
            if (DashCharge < DashChargeMax)
            {
                DashCharge += Time.fixedDeltaTime;
                if (DashCharge > DashChargeMax)
                    DashCharge = DashChargeMax;
            }
        }
    }
    private void UpdateEmoteState()
    {
        if(_currentlyPerformedEmote != null && _emoteInterrupted)
        {
            _emoteDelayTimer += Time.fixedDeltaTime;
            if (_emoteDelayTimer >= _currentlyPerformedEmote.EmoteEndDelay)
            {
                _currentlyPerformedEmote = null;
                _emoteDelayTimer = 0;
                SetCanMove(true);
            }
        }
    }
    public Vector2 GetRunVector()
    {
        if (_inputBehaviour != null)
            return _inputBehaviour.GetRunVector();
        else return Vector2.zero;
    }
    public Vector3 GetRunRotation(Vector3 realMovingVector3D)
    {
        if (CombatStance)
            return new Vector3(_camera.transform.forward.x, 0f, _camera.transform.forward.z);
        else
            return Vector3.Slerp(transform.forward, realMovingVector3D, Time.fixedDeltaTime * 10f);
    }
    public Vector3 GetCameraDirectedMovementVector(Vector2 movingVector)
    {
        Vector2 ret1 = new Vector2(_camera.transform.forward.x, _camera.transform.forward.z);
        Vector2 ret2 = new Vector2(_camera.transform.right.x, _camera.transform.right.z);
        Vector2 ret3 = ret1 * movingVector.y + ret2 * movingVector.x;
        return new Vector3(ret3.x, 0f, ret3.y);
    }
    public Vector3 GetTargetDirectedMovementVector(Vector3 movingVector)
    {
        return ((SelectedCharacter.transform.position - this.transform.position).normalized) * movingVector.magnitude;
    }
    private void HandleMovement()
    {
        if (Globals.Instance?.IsMenuOpen == false && Globals.Instance.IsCutscenePlaying == false)
        {
            var movingVector = GetRunVector();
            if (movingVector != Vector2.zero)
            {
                Vector3 realMovingVector3D = GetCameraDirectedMovementVector(movingVector);
                LastMovementDirection = realMovingVector3D.normalized;
                LastMovementDirectionExpire = 0.5f;
                if (CanMove())
                {
                    //movement
                    MovePosition(realMovingVector3D.normalized * Time.fixedDeltaTime * GetEffectiveMovementSpeed(), true);
                    IsRunning = true;
                    //rotation
                    FaceTheTarget(GetRunRotation(realMovingVector3D).normalized);
                    GetHeroClass()?.Hero_OnMove();
                }
                else
                {
                    IsRunning = false;
                }
            }
            else
            {
                IsRunning = false;
            }
            if(LastMovementDirectionExpire>0)
            {
                LastMovementDirectionExpire -= Time.fixedDeltaTime;
                if (LastMovementDirectionExpire <= 0)
                    LastMovementDirection = Vector3.zero;
            }
        }
    }
    public HeroClassBehaviour.ComboState TryPerformNextCombo(bool isR)
    {
        FaceTheTarget(new Vector3(_camera.transform.forward.x, 0f, _camera.transform.forward.z));
        return GetHeroClass().TryPerformNextComboState(isR);
    }
    public override void EnterCombat(CharacterBehaviour character, bool fightProvokedByGroup)
    {
        base.EnterCombat(character, fightProvokedByGroup);
    }
    public override void MoveBySkill(Vector3 moveTarget, MovingAbilityBehaviour.MovingType movingType)
    {
        if (Globals.Instance.IsMenuOpen == false && Globals.Instance.IsCutscenePlaying == false)
        {
            if (movingType == MovingAbilityBehaviour.MovingType.Forward)
            {
                MovePosition(transform.forward * moveTarget.magnitude, true);
            }
            else if (movingType == MovingAbilityBehaviour.MovingType.CameraBased)
            {
                MovePosition(GetCameraDirectedMovementVector(moveTarget), true);
            }
            else if(movingType == MovingAbilityBehaviour.MovingType.TargetBased)
            {
                var targ = GetTargetDirectedMovementVector(moveTarget);
                FaceTheTarget(targ.normalized);
                MovePosition(targ, true);
            }
            else
            {
                MovePosition(moveTarget, true);
            }
        }
    }
    public void ActivateCompanionAttack()
    {
        //will be changed once the companion system is introduced
        if(SelectedCharacter != null && SelectedCharacter.Faction != this.Faction && !this.Faction.Allies.Contains(SelectedCharacter.Faction.FactionType))
        {
            if(CompanionCharge >=10)
            {
                SelectedCharacter.TakeDamage(new Damage(this, companionDamage));
                OnCompanionAttack?.Invoke(this, EventArgs.Empty);
                CompanionCharge -= 10;
            }
        }
    }
    public override void Resurrect()
    {
        base.Resurrect();
        CanDash = true;
    }
    public virtual bool GetOpportunityAbilities(out AbilityBehaviour leftAbility, out AbilityBehaviour RightAbility, out string leftKey, out string rightKey)
    {
        if (!IsDead)
        {
            leftAbility = null;
            RightAbility = null;
            leftKey = "";
            rightKey = "";
            GetHeroClass().GetOpportunityAbilities(out leftAbility, out RightAbility, out leftKey, out rightKey);
            return true;
        }
        else
        {
            leftAbility = null;
            RightAbility = null;
            leftKey = "";
            rightKey = "";
            return false;
        }
    }
    public override bool CanAct()
    {
        if (Globals.Instance.IsMenuOpen == false && Globals.Instance.IsCutscenePlaying == false && _currentlyPerformedEmote == null)
            return base.CanAct();
        else return false;
    }
    public override void SetCanMove(bool canMove)
    {
        base.SetCanMove(canMove);
        if(canMove == false)
            IsRunning = false;
    }
    public override void SyncPerks(bool addOnly)
    {
        if(SkyforgeLoader.PerkRegistry != null && SkyforgeLoader.CurrentProfile != null)
        {
            if (!addOnly)
            {
                Stats.Reset(CharacterSO);
                AddRegisteredPerkSets();
                _perks.Clear();
                foreach (var perkState in SkyforgeLoader.CurrentProfile.AcquiredPerks)
                {
                    var perkSO = SkyforgeLoader.PerkRegistry.Perks.FirstOrDefault(p => p.ID == perkState.PerkID);
                    if (perkSO.Class == null || perkSO.Class == GetHeroClass().HeroClassSO)
                    {
                        AddPerk(perkSO, perkState.Enabled);
                    }
                }
            }
            else
            {
                foreach (var perkState in SkyforgeLoader.CurrentProfile.AcquiredPerks)
                {
                    var perkSO = SkyforgeLoader.PerkRegistry.Perks.FirstOrDefault(p => p.ID == perkState.PerkID);
                    if (perkSO.Class == null || perkSO.Class == GetHeroClass().HeroClassSO)
                    {
                        var perkToOvwerwrite = _perks.FirstOrDefault(p => p.Perk == perkSO);
                        if (perkToOvwerwrite == null)
                        {
                            AddPerk(perkSO, perkState.Enabled);
                        }
                        else if(perkToOvwerwrite.Enabled != perkState.Enabled)
                        {
                            if (perkState.Enabled)
                                EnablePerk(perkSO);
                            else
                                DisablePerk(perkSO);
                        }
                    }
                }
            }
        }
    }
    protected override void AddRegisteredPerkSets()
    {
        if (SkyforgeLoader.PerkRegistry != null && SkyforgeLoader.CurrentProfile != null)
        {
            _perkSets.Clear();
            foreach (var perkSetSO in SkyforgeLoader.PerkRegistry.PerkSets)
            {
                if(perkSetSO.HeroClassSO == null || perkSetSO.HeroClassSO == GetHeroClass().HeroClassSO)
                {
                    var perkSet = new ChoosablePerkSet(perkSetSO);
                    perkSet.ClearPerks();
                    _perkSets.Add(perkSet);
                }
            }
        }
    }
    public override void AddPerk(PerkSO perkSO, bool autoEnable, bool saveToProfile = false)
    {
        base.AddPerk(perkSO, autoEnable);
        if(saveToProfile && SkyforgeLoader.CurrentProfile != null && !SkyforgeLoader.CurrentProfile.AcquiredPerks.Any(ps => ps.PerkID == perkSO.ID))
        {
            SkyforgeLoader.CurrentProfile.AcquiredPerks.Add(new UserProfile.PerkState() { PerkID = perkSO.ID, Enabled = true });
        }
    }
    public IEnumerator DelayedInitSequence()
    {
        yield return new WaitForSeconds(2);
        SyncPerks(false);
    }
    #endregion
}
