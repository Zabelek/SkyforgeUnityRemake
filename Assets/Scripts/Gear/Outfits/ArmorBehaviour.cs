public class ArmorBehaviour : GearPeaceBehaviour
{
    #region Variables
    public ArmorSO ArmorSO;
    public OutfitSO OutfitSO;
    #endregion

    #region Mono
    protected override void Awake()
    {
        base.Awake();
    }
    #endregion

    #region Methods
    public void AssignWornOutfit(OutfitBehaviour outfit)
    {
        outfit.transform.SetParent(this.transform);
    }
    public override GearBonus GetBaseStats(Item item)
    {
        return GetGeneralBaseStats(item);
    }
    public override GearBonus GetMarkedStats(Item item)
    {
        return GetGeneralMarkedStats(item);
    }
    public static GearBonus GetGeneralBaseStats(Item item)
    {
        var ret = new GearBonus();
        if (item?.ItemSO is ArmorSO)
        {
            var armor = 10;
            if (item.ItemUpgradeState != null)
            {
                for (int i = 0; i < item.ItemUpgradeState.Generation; i++)
                {
                    armor += i;
                }
            }
            if (item.ItemSO.Rarity == ItemSO.RarityLevel.Uncommon)
                armor *= 2;
            else if (item.ItemSO.Rarity == ItemSO.RarityLevel.Rare)
                armor *= 3;
            else if (item.ItemSO.Rarity == ItemSO.RarityLevel.Epic || item.ItemSO.Rarity == ItemSO.RarityLevel.Legendary)
                armor *= 4;
            armor = (int)(armor / 2);
            ret.Armor += (int)(armor * (item.ItemSO as ArmorSO).BaseArmorAmount);
        }
        return ret;
    }
    public static GearBonus GetGeneralMarkedStats(Item item)
    {
        var ret = new GearBonus();
        if (item?.ItemUpgradeState != null && item.ItemUpgradeState.EtherFibers[0] != 0)
        {
            if (item.ItemUpgradeState.PickedEtherDirection == ItemUpgradeState.EtherDirection.KingHalf)
            {
                ret.ArmorPercentBonus += 0.01f;
                foreach (float fiber in item.ItemUpgradeState.EtherFibers)
                {
                    ret.ArmorPercentBonus += 0.005f * fiber;
                }
            }
            else if (item.ItemUpgradeState.PickedEtherDirection == ItemUpgradeState.EtherDirection.King)
            {
                ret.DashChargesBonusPercent += 0.01f;
                foreach (float fiber in item.ItemUpgradeState.EtherFibers)
                {
                    ret.DashChargesBonusPercent += 0.005f * fiber;
                }
            }
            else if (item.ItemUpgradeState.PickedEtherDirection == ItemUpgradeState.EtherDirection.AssassinHalf)
            {
                ret.StabilityBonus += 0.01f;
                foreach (float fiber in item.ItemUpgradeState.EtherFibers)
                {
                    ret.StabilityBonus += 0.005f * fiber;
                }
            }
            else if (item.ItemUpgradeState.PickedEtherDirection == ItemUpgradeState.EtherDirection.Assassin)
            {
                ret.CompanionChargesBonusPercent += 0.01f;
                foreach (float fiber in item.ItemUpgradeState.EtherFibers)
                {
                    ret.CompanionChargesBonusPercent += 0.0035f * fiber;
                }
            }
            else if (item.ItemUpgradeState.PickedEtherDirection == ItemUpgradeState.EtherDirection.SageHalf)
            {
                ret.MovementSpeedPercentBonus += 0.01f;
                foreach (float fiber in item.ItemUpgradeState.EtherFibers)
                {
                    ret.MovementSpeedPercentBonus += 0.002f * fiber;
                }
            }
            else if (item.ItemUpgradeState.PickedEtherDirection == ItemUpgradeState.EtherDirection.Sage)
            {
                ret.MaxDamagePercentBonus += 0.01f;
                foreach (float fiber in item.ItemUpgradeState.EtherFibers)
                {
                    ret.MaxDamagePercentBonus += 0.0075f * fiber;
                }
            }
            else if (item.ItemUpgradeState.PickedEtherDirection == ItemUpgradeState.EtherDirection.JudgeHalf)
            {

                ret.VampirismBonus += 0.005f;
                foreach (float fiber in item.ItemUpgradeState.EtherFibers)
                {
                    ret.VampirismBonus += 0.001f * fiber;
                }
            }
            else if (item.ItemUpgradeState.PickedEtherDirection == ItemUpgradeState.EtherDirection.Judge)
            {
                ret.HealthPercentBonus += 0.01f;
                foreach (float fiber in item.ItemUpgradeState.EtherFibers)
                {
                    ret.HealthPercentBonus += 0.005f * fiber;
                }
            }
        }
        return ret;
    }
    #endregion
}
