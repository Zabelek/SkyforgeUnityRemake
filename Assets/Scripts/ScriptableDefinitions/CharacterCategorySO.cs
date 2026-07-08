using UnityEngine;

[CreateAssetMenu(menuName = "Skyforge Scriptable Objects/CharacterCategorySO")]
public class CharacterCategorySO : ScriptableObject
{
    public string Name;
    [Tooltip("This background helps determine the character's strength")]
    public Sprite IconBackground;
    [Tooltip("This image is the character's type representation, for example tank icon, assasin icon etc.")]
    public Sprite Icon;
    [Tooltip("This background helps determine the character's strength")]
    public bool ShowBackgroundAboveModel;
    [Tooltip("For bosses and mini bosses. Full health of the character will be split into this amount of health bars. Each leahtl bar will drop a healing orb")]
    public int HealthBarsAmount;
    [Tooltip("Setting this to true will make the character drop school buses")]
    public bool DropsHealingOrbs;
}
