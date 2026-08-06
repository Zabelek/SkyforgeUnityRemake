using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Skyforge Scriptable Objects/ItemSO/ArmorSO")]
public class ArmorSO : ItemSO
{
    #region Variables
    [Tooltip("Visual outfit attached to toe armor. Outfits are for looks, armor itself is for stats, but by default, it also changes appearance.")]
    public OutfitSO OutfitSO;
    [Tooltip("1 is ALMOST 100% damage blocking (all damage will be 1), 0 is 0% damage blocking. Pick something in between.")]
    public float BaseArmorAmount;
    [Tooltip("This is the reference to the armor's creator private diary. Just joking, put the prefab here already!")]
    [SerializeField] private ArmorBehaviour _prefab;
    #endregion

    #region Getters
    public ArmorBehaviour GetPrefab()
    {
        return _prefab;
    }
    #endregion
}
