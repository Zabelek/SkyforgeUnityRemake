using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Skyforge Scriptable Objects/HeroClassSO")]
public class HeroClassSO : ScriptableObject
{
    public string Name;
    [Tooltip("It'll be displayed in the interface(once implemented)")]
    public Image Icon;
    [Tooltip("The type of a weapon that can be used while this class is picked")]
    public WeaponTypeSO WeaponType;
    [Tooltip("When you draw the weapon, you will move slower/quicker, depending on class")]
    public float CombatMovementSpeedMultiplier;
}
