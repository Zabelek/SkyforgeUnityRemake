using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Skyforge Scriptable Objects/HeroClassSO")]
public class HeroClassSO : ScriptableObject
{
    [Tooltip("Must be unique. Sometimes SO are duplicated, so SO=SO doesm't always work. This way its safer")]
    public string ID;
    public string Name;
    [Tooltip("It'll be displayed in the interface(once implemented)")]
    public Image Icon;
    [Tooltip("The type of a weapon that can be used while this class is picked")]
    public WeaponTypeSO WeaponType;
    [Tooltip("When you draw the weapon, you will move slower/quicker, depending on class")]
    public float CombatMovementSpeedMultiplier;
    [Tooltip("Displayed where the icon will be small in the interface")]
    public Sprite SimplifiedIcon;
    [Tooltip("Displayed where the icon will be big enough to spot the details")]
    public Sprite RegularIcon;
}
