using System.Collections.Generic;
using UnityEngine;

public class PYTTHEntidFirstAbilityBehaviour : AbilityBehaviour
{
    private const float TRIFFID_DEATH_DISTANCE = 5;

    #region Variables
    [Header("Triffid Spawn Ability")]
    [SerializeField] private PYTHTriffidBehaviour _triffidBase;
    [SerializeField] private ParticleSystem _spawnParticlesBase;
    [SerializeField] private ParticleSystem _entidParticlesBase;
    public Transform[] TriffidSpawnPoints;
    private List<PYTHTriffidBehaviour> _summonedTriffids;
    #endregion

    #region Methods
    public override void UpdateAbility(CharacterBehaviour performer, HeroClassBehaviour heroClass)
    {
        base.UpdateAbility(performer, heroClass);
        if (!Finishing)
        {
            //managing killing triffids that went too close
            PYTHTriffidBehaviour triffidToDestroy = null;
            foreach (var triffid in _summonedTriffids)
            {
                if (triffid.IsDead || ((triffid.transform.position - performer.transform.position).magnitude < TRIFFID_DEATH_DISTANCE))
                {
                    triffidToDestroy = triffid;
                    break;
                }
            }
            if (triffidToDestroy != null)
            {
                _summonedTriffids.Remove(triffidToDestroy);
                if (!triffidToDestroy.IsDead)
                {
                    triffidToDestroy.Kill(triffidToDestroy);
                    performer.HealPercent(0.1f, true);
                }
            }
        }
    }
    public override void LaunchAbility(CharacterBehaviour performer)
    {
        base.LaunchAbility(performer);
        Instantiate(_entidParticlesBase, performer.transform);
        performer.PlayAnimation("Skill_Seed", true);
    }
    public override void PerformHit(CharacterBehaviour performer)
    {
        //This is the place where triffids are spawned
        base.PerformHit(performer);
        foreach(var spawnPoint in TriffidSpawnPoints)
        {
            Instantiate(_spawnParticlesBase, spawnPoint);
            var triffid = Instantiate(_triffidBase, spawnPoint);
            triffid.GetComponent<DOTHealingTriffidAIHandler>().FollowTarget = performer;
            _summonedTriffids.Add(triffid);
        }
    }
    public override void Reset()
    {
        base.Reset();
        if(_summonedTriffids!= null && _summonedTriffids.Count > 0)
        {
            foreach (var trif in _summonedTriffids)
            {
                trif.Kill(trif);
            }
        }
        _summonedTriffids = new();
    }
    #endregion
}