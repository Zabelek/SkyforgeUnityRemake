using System;
using UnityEngine;

public class ArmorBehaviour : MonoBehaviour
{
    #region Variables
    public ArmorSO ArmorSO;
    public OutfitSO OutfitSO;
    #endregion

    #region Methods
    public virtual void Equip(HeroBehaviour hero, bool onlyVisual)
    {
        gameObject.SetActive(true);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = new Vector3(1, 1, 1);
    }
    public virtual void Unequip(HeroBehaviour hero, bool onlyVisual)
    {
        transform.SetParent(null);
        gameObject.SetActive(false);
        Destroy(this.gameObject);
    }
    public void AssignWornOutfit(OutfitBehaviour outfit)
    {
        outfit.transform.SetParent(this.transform);
    }
    #endregion
}
