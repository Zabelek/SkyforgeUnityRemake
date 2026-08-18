using UnityEngine;

public class SunnyPrismBehaviour : GearPeaceBehaviour
{
    #region Variables
    public ArtifactSO ArtifactSO;
    [SerializeField] private LootRecord[] _records;
    [SerializeField] private LootRecord[] _finisherRecords;
    #endregion

    #region Mono
    protected override void Awake()
    {
        base.Awake();
        GearBonus.HealthPercentBonus += ArtifactSO.HealthBonus;
        GearBonus.DamagePercentBonus += ArtifactSO.DamageBonus;
    }
    private void FixedUpdate()
    {
    }
    #endregion

    #region Methods
    public override void Equip(HeroBehaviour hero, bool onlyVisual)
    {
        base.Equip(hero, onlyVisual);
        hero.OnEnemyKillEvent += EnemyKillAction;
    }
    public override void Unequip(HeroBehaviour hero, bool onlyVisual)
    {
        hero.OnEnemyKillEvent -= EnemyKillAction;
        base.Unequip(hero, onlyVisual);
    }
    #endregion

    #region EventHandlers
    private void EnemyKillAction(object sender, CharacterBehaviour.EnemyKillEventArgs e)
    {
        if(e?.KilledCharacter != null)
        {
            var lootMgr = e?.KilledCharacter?.GetComponentInChildren<LootManager>();
            if(lootMgr != null)
            {
                if (e.KilledByFinisher == true)
                    foreach (var record in _finisherRecords)
                    {
                        lootMgr.LootRecords.Add(record);
                    }
                else
                    foreach (var record in _records)
                    {
                        lootMgr.LootRecords.Add(record);
                    }
            }
        }
    }
    #endregion
}
