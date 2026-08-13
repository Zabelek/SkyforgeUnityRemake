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
        if (ArmorSO != null)
        {
            GearBonus.Armor += ArmorSO.BaseArmorAmount;
        }
    }
    #endregion

    #region Methods
    public void AssignWornOutfit(OutfitBehaviour outfit)
    {
        outfit.transform.SetParent(this.transform);
    }
    #endregion
}
