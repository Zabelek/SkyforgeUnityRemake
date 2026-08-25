using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PYTHTriffidBossBehaviour : PYTHTriffidBehaviour
{
    #region Variables
    [Header("Triffid Boss")]
    public AbilityBehaviour SecondSpecialAbility;
    public AbilityBehaviour ThirdSpecialAbility;
    private List<CharacterBehaviour> _safetyPillars;
    public Transform PillarSpot1, PillarSpot2, PillarSpot3, TroopsSpawnPlace1, TroopsSpawnPlace2, TroopsSpawnPlace3;
    [SerializeField] private RestrictedAreaBehaviour _area;
    #endregion

    #region Mono
    protected override void Awake()
    {
        base.Awake();
        _safetyPillars = new();
        if (SecondSpecialAbility != null)
        {
            SecondSpecialAbility = Instantiate(SecondSpecialAbility, this.transform);
            SecondSpecialAbility.Init();
            SecondSpecialAbility.gameObject.SetActive(false);
        }
        if (ThirdSpecialAbility != null)
        {
            ThirdSpecialAbility = Instantiate(ThirdSpecialAbility, this.transform);
            ThirdSpecialAbility.Init();
            ThirdSpecialAbility.gameObject.SetActive(false);
        }
    }
    #endregion

    #region Methods
    public void AddPillar(CharacterBehaviour pillar)
    {
        _safetyPillars.Add(pillar);
        pillar.OnDeathEvent += RemovePillar;
        CheckForPillars();
    }
    private void RemovePillar(object sender, EventArgs e)
    {
        if (sender is CharacterBehaviour)
        {
            (sender as CharacterBehaviour).OnDeathEvent -= RemovePillar;
            _safetyPillars.Remove((sender as CharacterBehaviour));
        }
        CheckForPillars();
    }
    private void CheckForPillars()
    {
        if(_safetyPillars.Any())
        {
            this.SetInvulnerable(true);
        }
        else
        {
            this.SetInvulnerable(false);
        }
    }
    public int GetPillarAmount()
    {
        return _safetyPillars.Count();
    }
    public override void EnterCombat(CharacterBehaviour character, bool fightProvokedByGroup)
    {
        //reseting abilities cooldowns on combat enter, so if player died and returns to try again, triffid won't immediately spawn troops etc
        if (!IsInCombat)
        {
            SecondSpecialAbility.CurrentCooldown = 0;
            ThirdSpecialAbility.CurrentCooldown = 40;
        }
        base.EnterCombat(character, fightProvokedByGroup);
        _area.SetActive(true);
    }
    public override void UpdateAbilitiesCooldown()
    {
        base.UpdateAbilitiesCooldown();
        SecondSpecialAbility?.UpdateCooldown();
        ThirdSpecialAbility?.UpdateCooldown();
    }
    public override void LeaveCombat()
    {
        base.LeaveCombat();
        _area.SetActive(false);
        CheckForPillars();
    }
    public override void Kill(CharacterBehaviour killer, AbilityBehaviour killingAbility)
    {
        base.Kill(killer, killingAbility);
        _area.SetActive(false);
    }
    #endregion

    #region EventHandlers
    #endregion
}
