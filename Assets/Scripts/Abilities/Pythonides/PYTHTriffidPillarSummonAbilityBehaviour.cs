using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines.ExtrusionShapes;

public class PYTHTriffidPillarSummonAbilityBehaviour : AbilityBehaviour
{
    #region Variables
    [SerializeField] private CharacterBehaviour _pillarBase;
    [SerializeField] private ParticleSystem _particlesBase;
    #endregion

    #region Methods
    public override void LaunchAbility(CharacterBehaviour performer)
    {
        base.LaunchAbility(performer);
        performer.PlayAnimation("Skill1");
        var particles = Instantiate(_particlesBase, performer.transform);
        particles.gameObject.SetActive(true);
    }
    public override void PerformHit(CharacterBehaviour performer)
    {
        base.PerformHit(performer);
        if ((performer as PYTHTriffidBossBehaviour))
        {
            var triffidboss = performer as PYTHTriffidBossBehaviour;
            var pillar1 = Instantiate(_pillarBase, triffidboss.PillarSpot1);
            triffidboss.AddPillar(pillar1);
            var pillar2 = Instantiate(_pillarBase, triffidboss.PillarSpot2);
            triffidboss.AddPillar(pillar2);
            var pillar3 = Instantiate(_pillarBase, triffidboss.PillarSpot3);
            triffidboss.AddPillar(pillar3);
            foreach (var character in triffidboss.ActiveEnemies)
            {
                CharacterBehaviour.EnterCombat(pillar1, character);
                CharacterBehaviour.EnterCombat(pillar2, character);
                CharacterBehaviour.EnterCombat(pillar3, character);
            }
            pillar1.AddComponent<KillAfterBattleBehaviour>();
            pillar2.AddComponent<KillAfterBattleBehaviour>();
            pillar3.AddComponent<KillAfterBattleBehaviour>();
        }
    }
    #endregion
}
