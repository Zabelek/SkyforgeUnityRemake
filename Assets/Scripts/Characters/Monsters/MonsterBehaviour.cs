using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterBehaviour : CharacterBehaviour
{
    #region Variables
    [Header("Monster Related Variables")]
    [Tooltip("Normal attack that monster tries to use every AI tick")]
    public AbilityBehaviour BaseAttack;
    [Tooltip("Second attack, usualy stronger and with longer cooldown")]
    public AbilityBehaviour SpecialAttack;
    [HideInInspector] public List<AbilityBehaviour> CurrentlyUpdatedAbilities, RecentlyFinishedAbilities;
    [HideInInspector] public event EventHandler OnHurtAction;
    [HideInInspector] public event EventHandler OnEmoteAction;
    #endregion

    #region Mono
    protected override void Awake()
    {
        base.Awake();
        if (BaseAttack != null)
        {
            //Once the skill is instantiated, the base is no longer needed, so it's overwritten. Same story goes for all the abilities, as they're just prefab references at first.
            BaseAttack = Instantiate(BaseAttack, this.transform);
            BaseAttack.Init();
            BaseAttack.gameObject.SetActive(false);
        }
        if (SpecialAttack != null)
        {
            SpecialAttack = Instantiate(BaseAttack, this.transform);
            SpecialAttack.Init();
            SpecialAttack.gameObject.SetActive(false);
        }
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        foreach (var ability in CurrentlyUpdatedAbilities)
        {
            ability.UpdateAbility(this, null);
        }
        foreach (var ability in RecentlyFinishedAbilities)
        {
            CurrentlyUpdatedAbilities.Remove(ability);
            ability.Finishing = false;
        }
        RecentlyFinishedAbilities.Clear();
        UpdateAbilitiesCooldown();
    }
    #endregion

    #region Methods
    public virtual bool TryPerformAbility(AbilityBehaviour ability)
    {
        if (ability.CheckPerformAvailability(this) && !CurrentlyUpdatedAbilities.Any(ab => ab.GetType() == ability.GetType()))
        {
            ability.LaunchAbility(this);
            return true;
        }
        return false;
    }
    public void InterruptAbilities()
    {
        foreach(var ability in CurrentlyUpdatedAbilities)
        {
            ability.Interrupt(this);
        }
    }
    public override void TakeDamage(Damage damage)
    {
        base.TakeDamage(damage);
        OnHurtAction?.Invoke(this, EventArgs.Empty);
    }
    public override void TakeEmptyDamage()
    {
        OnHurtAction?.Invoke(this, EventArgs.Empty);
    }
    public override void PerformEmote(object sender, EventArgs e)
    {
        OnEmoteAction?.Invoke(this, EventArgs.Empty);
    }
    public virtual void UpdateAbilitiesCooldown()
    {
        BaseAttack?.UpdateCooldown();
        SpecialAttack?.UpdateCooldown();
    }
    public override void SetCanAct(bool canAct, bool ownAbilityDriven)
    {
        base.SetCanAct(canAct, ownAbilityDriven);
        if(canAct == false && !ownAbilityDriven)
        {
            InterruptAbilities();
        }
    }
    #endregion
}
