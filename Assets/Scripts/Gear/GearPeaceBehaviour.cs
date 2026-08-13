using UnityEngine;

public class GearPeaceBehaviour : MonoBehaviour
{
    #region Variables
    public GearBonus GearBonus { get; protected set; }
    #endregion

    #region Mono
    protected virtual void Awake()
    {
        GearBonus = new();
    }
    #endregion

    #region Methods
    public virtual void Equip(HeroBehaviour hero, Transform slot, bool onlyVisual)
    {
        transform.SetParent(slot);
        Equip(hero, onlyVisual);
    }
    public virtual void Equip(HeroBehaviour hero, bool onlyVisual)
    {
        gameObject.SetActive(true);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = new Vector3(1, 1, 1);
        hero.Stats.GearStats.Add(GearBonus);
    }
    public virtual void Unequip(HeroBehaviour hero, bool onlyVisual)
    {
        hero.Stats.GearStats.Remove(GearBonus);
        transform.SetParent(null);
        gameObject.SetActive(false);
        Destroy(this.gameObject);
    }
    #endregion
}
