using UnityEngine;

public class ArtifactBehaviour : GearPeaceBehaviour
{
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
        if (item?.ItemSO is ArtifactSO)
        {
            var healthBonus = 0.5f;
            if (item.ItemUpgradeState != null)
            {
                healthBonus += 0.1f * (item.ItemUpgradeState.Generation - 1);
            }
            ret.HealthPercentBonus += healthBonus * (item.ItemSO as ArtifactSO).HealthBonus;
            var damageBonus = 0.01f;
            if (item.ItemUpgradeState != null)
            {
                damageBonus += 0.01f * (item.ItemUpgradeState.Generation - 1);
            }
            ret.DamagePercentBonus += damageBonus * (item.ItemSO as ArtifactSO).DamageBonus;
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
                ret.HealthPercentBonus += 0.02f;
                foreach (float fiber in item.ItemUpgradeState.EtherFibers)
                {
                    ret.HealthPercentBonus += 0.01f * fiber;
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
                ret.DamagePercentBonus += 0.01f;
                foreach (float fiber in item.ItemUpgradeState.EtherFibers)
                {
                    ret.DamagePercentBonus += 0.005f * fiber;
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
                ret.CompanionDamagePercentBonus += 0.01f;
                foreach (float fiber in item.ItemUpgradeState.EtherFibers)
                {
                    ret.CompanionDamagePercentBonus += 0.005f * fiber;
                }
            }
            else if (item.ItemUpgradeState.PickedEtherDirection == ItemUpgradeState.EtherDirection.Sage)
            {
                ret.MoneyReceivedBonus += 0.02f;
                foreach (float fiber in item.ItemUpgradeState.EtherFibers)
                {
                    ret.MoneyReceivedBonus += 0.01f * fiber;
                }
            }
            else if (item.ItemUpgradeState.PickedEtherDirection == ItemUpgradeState.EtherDirection.JudgeHalf)
            {
                ret.ArmorPercentBonus += 0.01f;
                foreach (float fiber in item.ItemUpgradeState.EtherFibers)
                {
                    ret.ArmorPercentBonus += 0.005f * fiber;
                }
            }
            else if (item.ItemUpgradeState.PickedEtherDirection == ItemUpgradeState.EtherDirection.Judge)
            {
                ret.StabilityBonus += 0.01f;
                foreach (float fiber in item.ItemUpgradeState.EtherFibers)
                {
                    ret.StabilityBonus += 0.005f * fiber;
                }
            }
        }
        return ret;
    }
}
