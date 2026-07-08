using UnityEngine;

[CreateAssetMenu(menuName = "Skyforge Scriptable Objects/WeaponTypeSO")]
public class WeaponTypeSO : ScriptableObject
{
    public string Name;
    [Tooltip("When you have to go close to the enemy to do bonk, it means the weapon is meelee")]
    public bool IsMeelee;
}
