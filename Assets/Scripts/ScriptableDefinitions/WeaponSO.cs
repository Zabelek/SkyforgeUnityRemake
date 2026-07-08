using UnityEngine;

[CreateAssetMenu(menuName = "Skyforge Scriptable Objects/WeaponSO")]
public class WeaponSO : ScriptableObject
{
    #region Variables
    [Tooltip("Type of the weapon tells for example which class it is for")]
    public WeaponTypeSO Type;
    [Tooltip("That might sound controversial, but it's actual mesh of the weapon")]
    [SerializeField] private Transform _meshRenderer;
    [Tooltip("damage modifier of the weapon. It'll be added to hero's damage when calculating output damage for each skill")]
    [SerializeField] private int _baseDamage;
    [Tooltip("Name your weapon whatever you want, just don't name them Doris. Doris is not a good name for a weapon.")]
    public string Name;
    #endregion

    #region Getters
    public Transform GetMesh()
    {
        return _meshRenderer;
    }
    public int GetDamage()
    {
        return _baseDamage;
    }
    #endregion
}
