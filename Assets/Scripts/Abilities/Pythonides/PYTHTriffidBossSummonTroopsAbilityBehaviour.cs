using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PYTHTriffidBossSummonTroopsAbilityBehaviour : AbilityBehaviour
{
    #region Variables
    [SerializeField] private CharacterBehaviour _dryadBase, _triffidBase, _dioneBase;
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
            var dryad = Instantiate(_dryadBase, triffidboss.TroopsSpawnPlace1);
            var triffid1 = Instantiate(_triffidBase, triffidboss.TroopsSpawnPlace1);
            var triffid2 = Instantiate(_triffidBase, triffidboss.TroopsSpawnPlace2);
            var triffid3 = Instantiate(_triffidBase, triffidboss.TroopsSpawnPlace2);
            var dione1 = Instantiate(_dioneBase, triffidboss.TroopsSpawnPlace2);
            var triffid4 = Instantiate(_triffidBase, triffidboss.TroopsSpawnPlace3);
            var dione2 = Instantiate(_dioneBase, triffidboss.TroopsSpawnPlace3);
            foreach (var character in triffidboss.ActiveEnemies)
            {
                CharacterBehaviour.EnterCombat(character, dryad);
                CharacterBehaviour.EnterCombat(character, triffid1);
                CharacterBehaviour.EnterCombat(character, triffid2);
                CharacterBehaviour.EnterCombat(character, triffid3);
                CharacterBehaviour.EnterCombat(character, triffid4);
                CharacterBehaviour.EnterCombat(character, dione1);
                CharacterBehaviour.EnterCombat(character, dione2);
            }
            dryad.AddComponent<KillAfterBattleBehaviour>();
            triffid1.AddComponent<KillAfterBattleBehaviour>();
            triffid2.AddComponent<KillAfterBattleBehaviour>();
            triffid3.AddComponent<KillAfterBattleBehaviour>();
            triffid4.AddComponent<KillAfterBattleBehaviour>();
            dione1.AddComponent<KillAfterBattleBehaviour>();
            dione2.AddComponent<KillAfterBattleBehaviour>();
        }
    }
    #endregion
}
