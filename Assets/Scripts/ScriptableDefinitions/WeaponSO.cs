using UnityEngine;

[CreateAssetMenu(menuName = "Skyforge Scriptable Objects/ItemSO/WeaponSO")]
public class WeaponSO : ItemSO
{
    #region Variables
    [Tooltip("Type of the weapon tells for example which class it is for")]
    public WeaponTypeSO Type;
    [Tooltip("That might sound controversial, but it's a prefab of the weapon")]
    [SerializeField] private WeaponBehaviour _weaponPrefab;
    [Tooltip("damage modifier of the weapon. It'll be added to hero's damage when calculating output damage for each skill")]
    [SerializeField] private int _baseDamage;
    [Tooltip("Name your weapon whatever you want, just don't name it Doris. Doris is not a good name for a weapon.")]
    #endregion

    #region Getters
    public WeaponBehaviour GetPrefab()
    {
        return _weaponPrefab;
    }
    public int GetDamage()
    {
        return _baseDamage;
    }
    #endregion
}
